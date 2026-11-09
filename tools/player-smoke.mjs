import { spawn } from 'node:child_process';
import { join } from 'node:path';
import { readFile, readdir } from 'node:fs/promises';
import { root, readJSON, atomicJSON, directoryIdentity, hash } from './workflow/core.mjs';
import { checkUnityResult, unityEnvironment } from './unity.mjs';
let id = process.argv[2];
if (!id) {
  const builds = [];
  for (const name of await readdir(join(root, 'artifacts/runs'))) {
    const record = await readJSON(join(root, 'artifacts/runs', name, 'manifest.json')).catch(() => null);
    if (record?.kind === 'unity-build') builds.push({ id: name, startedAt: record.startedAt });
  }
  builds.sort((a, b) => b.startedAt.localeCompare(a.startedAt));
  id = builds[0]?.id;
}
if (!/^[a-zA-Z0-9-]+$/.test(id || '')) throw new Error('Pass a build run ID from artifacts/runs.');
const dir = join(root, 'artifacts/runs', id), player = join(dir, 'player');
const manifest = await readJSON(join(dir, 'manifest.json'));
if (manifest.kind !== 'unity-build' || manifest.status !== 'passed') throw new Error('Run is not a passing player build');
if ((await directoryIdentity(player)).bundleHash !== manifest.bundle.bundleHash) throw new Error('Player bundle differs from build evidence');
const log = join(dir, 'player-smoke.log');
let result;
try {
  const code = await new Promise((accept, reject) => {
    const child = spawn(join(player, 'Yendor.x86_64'), ['-batchmode', '-nographics', '--yendor-smoke', '-logFile', log], { cwd: player, env: unityEnvironment(), stdio: ['ignore', 'ignore', 'pipe'], timeout: 120000 });
    child.on('error', reject); child.on('close', (code, signal) => signal ? reject(new Error(`Player terminated: ${signal}`)) : accept(code));
  });
  const output = await readFile(log, 'utf8'); checkUnityResult({ code, output, mode: 'smoke' });
  if ((await directoryIdentity(player)).bundleHash !== manifest.bundle.bundleHash) throw new Error('Player changed its bundle during smoke');
  result = { passed: true, code, buildGuid: manifest.buildGuid, bundleHash: manifest.bundle.bundleHash, logHash: hash(output), log, graphicsVerified: false };
} catch (error) { result = { passed: false, error: error.message, log }; process.exitCode = 1; }
await atomicJSON(join(dir, 'player-smoke.json'), { ...result, at: new Date().toISOString() });
console.log(JSON.stringify(result, null, 2));
