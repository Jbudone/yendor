import { spawn } from 'node:child_process';
import { mkdir, readFile } from 'node:fs/promises';
import { resolve, dirname } from 'node:path';
import { fileURLToPath } from 'node:url';

export const root = resolve(dirname(fileURLToPath(import.meta.url)), '..');
export async function runUnity(mode = 'validate') {
  if (!['validate', 'build'].includes(mode)) throw new Error('Expected validate or build');
  const editor = process.env.UNITY_EDITOR_PATH || '/home/jbud/Unity/Hub/Editor/6000.6.0f1/Editor/Unity';
  const log = resolve(root, 'artifacts', `unity-${mode}.log`);
  await mkdir(dirname(log), { recursive: true });
  const args = ['-batchmode', '-nographics', '-projectPath', resolve(root, 'game'),
    '-executeMethod', `Yendor.Editor.ProjectSetup.${mode === 'build' ? 'Build' : 'Validate'}`, '-quit', '-logFile', log];
  const code = await new Promise((accept, reject) => {
    const child = spawn(editor, args, { cwd: root, stdio: ['ignore', 'ignore', 'pipe'], timeout: 600_000 });
    let error = '';
    child.stderr.on('data', chunk => { error = (error + chunk).slice(-4000); });
    child.on('error', reject);
    child.on('close', (status, signal) => signal ? reject(new Error(`Unity terminated: ${signal}. ${error}`)) : accept(status));
  });
  const output = await readFile(log, 'utf8');
  const marker = mode === 'build' ? 'YENDOR_BUILD_OK' : 'YENDOR_VALIDATION_OK';
  if (code !== 0 || !output.includes(marker) || /error CS\d+/.test(output))
    throw new Error(`Unity ${mode} failed; see ${log}\n${output.slice(-3000)}`);
  return { passed: true, mode, log };
}

if (process.argv[1] && resolve(process.argv[1]) === fileURLToPath(import.meta.url)) {
  try { console.log(JSON.stringify(await runUnity(process.argv[2]), null, 2)); }
  catch (error) { console.error(error.message); process.exitCode = 1; }
}
