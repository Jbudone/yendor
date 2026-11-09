import { spawn } from 'node:child_process';
import { readFile, writeFile, cp } from 'node:fs/promises';
import { resolve, join } from 'node:path';
import { fileURLToPath } from 'node:url';
import { root, acquireLock, createRun, atomicJSON, hash, sourceIdentity, directoryIdentity } from './workflow/core.mjs';
export { root };

export function checkUnityResult({ code, output, mode, results }) {
  const markers = { validate: 'YENDOR_VALIDATION_OK', build: 'YENDOR_BUILD_OK', smoke: 'YENDOR_RUNTIME_SMOKE_OK', audit: 'YENDOR_ASSET_AUDIT_OK' };
  if (code !== 0 || !output || /error CS\d+|Aborting batchmode due to failure/i.test(output)) throw new Error(`Unity ${mode} failed or produced no current evidence`);
  if (mode === 'test') {
    const run = results?.match(/<test-run\b[^>]*>/)?.[0];
    const failed = run?.match(/failed="(\d+)"/);
    const total = run?.match(/total="(\d+)"/);
    if (!run?.includes('result="Passed"') || !failed || Number(failed[1]) || !total || !Number(total[1])) throw new Error('Unity tests missing counts or contain failures');
  } else if (!output.includes(markers[mode])) throw new Error(`Unity ${mode} missing success marker`);
  return true;
}

export function unityEnvironment(env = process.env) {
  const names = ['PATH', 'HOME', 'USER', 'LOGNAME', 'SHELL', 'TMPDIR', 'XDG_RUNTIME_DIR', 'XDG_CONFIG_HOME', 'XDG_CACHE_HOME', 'DISPLAY', 'WAYLAND_DISPLAY', 'DBUS_SESSION_BUS_ADDRESS', 'LANG', 'LC_ALL', 'UNITY_LICENSE', 'UNITY_SERIAL', 'UNITY_EMAIL', 'UNITY_PASSWORD'];
  return Object.fromEntries(names.filter(name => env[name] !== undefined).map(name => [name, env[name]]));
}

export async function runUnity(mode = 'validate') {
  if (!['validate', 'build', 'smoke', 'test', 'audit'].includes(mode)) throw new Error('Expected validate, build, smoke, test or audit');
  const release = await acquireLock(root, 'unity-editor');
  let run;
  try {
    run = await createRun(`unity-${mode}`, mode);
    const editor = process.env.UNITY_EDITOR_PATH || '/home/jbud/Unity/Hub/Editor/6000.6.0f1/Editor/Unity';
    const project = process.env.UNITY_PROJECT_PATH || resolve(root, 'game');
    const log = join(run.dir, 'unity.log');
    const resultsPath = join(run.dir, 'test-results.xml');
    const args = ['-batchmode', '-nographics', '-projectPath', project, '-logFile', log];
    if (mode === 'test') args.push('-runTests', '-testPlatform', 'EditMode', '-testResults', resultsPath);
    else {
      args.push('-executeMethod', `Yendor.Editor.ProjectSetup.${{ validate: 'Validate', build: 'Build', smoke: 'Smoke', audit: 'AuditAssets' }[mode]}`);
      if (mode === 'smoke') args.push('--yendor-smoke'); else args.push('-quit');
    }
    const snapshot = {};
    if (resolve(project) !== resolve(root, 'game')) {
      for (const part of ['Assets', 'Packages', 'ProjectSettings']) {
        const original = await directoryIdentity(join(root, 'game', part));
        const copied = await directoryIdentity(join(project, part));
        if (original.bundleHash !== copied.bundleHash) throw new Error(`Verification copy differs in ${part}; prepare a fresh snapshot`);
        snapshot[part] = copied.bundleHash;
      }
    }
    await atomicJSON(join(run.dir, 'command.json'), { editor, args, snapshot, environmentPolicy: 'Unity allowlist; provider/Meshy/tracing credentials excluded' });
    const code = await new Promise((accept, reject) => {
      const child = spawn(editor, args, { cwd: root, env: unityEnvironment(), stdio: ['ignore', 'ignore', 'pipe'], timeout: 600_000 });
      let stderr = '';
      child.stderr.on('data', chunk => { stderr = (stderr + chunk).slice(-4000); });
      child.on('error', reject);
      child.on('close', async (status, signal) => {
        try { await writeFile(join(run.dir, 'stderr.log'), stderr); }
        catch (error) { reject(error); return; }
        signal ? reject(new Error(`Unity terminated: ${signal}`)) : accept(status);
      });
    });
    let output = '', results;
    try { output = await readFile(log, 'utf8'); } catch (error) { if (error.code !== 'ENOENT') throw error; }
    if (mode === 'test') { try { results = await readFile(resultsPath, 'utf8'); } catch {} }
    checkUnityResult({ code, output, mode, results });
    const after = await sourceIdentity();
    if (after.sourceHash !== run.source.sourceHash) throw new Error('Unity changed project source. Review the diff; evidence is not for the starting source.');
    for (const part of Object.keys(snapshot)) {
      if ((await directoryIdentity(join(project, part))).bundleHash !== snapshot[part]) throw new Error(`Unity modified verification-copy ${part}; review and refresh before treating this as a source pass`);
    }
    const bundle = mode === 'build' ? await directoryIdentity(join(project, 'Builds', 'Linux')) : undefined;
    if (mode === 'build') await cp(join(project, 'Builds', 'Linux'), join(run.dir, 'player'), { recursive: true });
    const evidence = { passed: true, mode, runId: run.id, log, logHash: hash(output), code, sourceHash: run.source.sourceHash, buildGuid: output.match(/YENDOR_BUILD_GUID: ([a-zA-Z0-9-]+)/)?.[1] || null, bundle };
    await run.finish({ status: 'passed', ...evidence });
    return evidence;
  } catch (error) {
    await run?.finish({ status: 'failed', error: error.message });
    throw new Error(`${error.message}${run ? `; evidence: ${run.dir}` : ''}`);
  } finally { await release(); }
}

if (process.argv[1] && resolve(process.argv[1]) === fileURLToPath(import.meta.url)) {
  try {
    const result = await runUnity(process.argv[2]);
    console.log(JSON.stringify({ ...result, bundle: result.bundle ? { bundleHash: result.bundle.bundleHash, fileCount: Object.keys(result.bundle.files).length } : undefined }, null, 2));
  }
  catch (error) { console.error(error.message); process.exitCode = 1; }
}
