import { readdir } from 'node:fs/promises';
import { join } from 'node:path';
import { exec, root } from './workflow/core.mjs';
async function visit(path) {
  for (const entry of await readdir(path, { withFileTypes: true })) {
    const full = join(path, entry.name);
    if (entry.isDirectory()) await visit(full);
    else if (full.endsWith('.mjs')) await exec(process.execPath, ['--check', full], { cwd: root });
  }
}
await visit(join(root, 'tools'));
const { stdout, stderr } = await exec(process.execPath, ['--test', ... (await readdir(join(root, 'tools/tests'))).filter(x => x.endsWith('.test.mjs')).map(x => join('tools/tests', x))], { cwd: root });
process.stdout.write(stdout); process.stderr.write(stderr);
console.log('Syntax and Node tests passed.');
