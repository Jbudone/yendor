import { mkdir, readFile, writeFile, rename, rm, readdir, lstat } from 'node:fs/promises';
import { createHash, randomUUID } from 'node:crypto';
import { execFile } from 'node:child_process';
import { promisify } from 'node:util';
import { resolve, join, relative } from 'node:path';
import { fileURLToPath } from 'node:url';

export const root = resolve(fileURLToPath(new URL('../..', import.meta.url)));
export const exec = promisify(execFile);
export const hash = value => createHash('sha256').update(value).digest('hex');
export const readJSON = async path => JSON.parse(await readFile(path, 'utf8'));
export async function atomicJSON(path, value) {
  await mkdir(resolve(path, '..'), { recursive: true });
  const temp = `${path}.${randomUUID()}.tmp`;
  await writeFile(temp, JSON.stringify(value, null, 2) + '\n', { mode: 0o600, flag: 'wx' });
  await rename(temp, path);
}

// Never steal an expired lease: a paused worker could still execute. Recovery is explicit.
export async function acquireLock(base, name) {
  const path = join(base, '.workflow', 'locks', name);
  await mkdir(resolve(path, '..'), { recursive: true });
  try { await mkdir(path); }
  catch (error) {
    if (error.code !== 'EEXIST') throw error;
    throw new Error(`${name} is locked. Inspect ${path}/owner.json; recover only after its process has stopped.`);
  }
  const token = randomUUID();
  await atomicJSON(join(path, 'owner.json'), { token, pid: process.pid, host: (await import('node:os')).hostname(), startedAt: new Date().toISOString() });
  return async () => {
    if ((await readJSON(join(path, 'owner.json'))).token !== token) throw new Error('Lock ownership changed');
    await rm(path, { recursive: true });
  };
}

export async function sourceIdentity(base = root) {
  const { stdout: revision } = await exec('git', ['rev-parse', 'HEAD'], { cwd: base });
  const { stdout } = await exec('git', ['ls-files', '--cached', '--others', '--exclude-standard', '-z'], { cwd: base, maxBuffer: 8_000_000 });
  const files = {};
  for (const path of [...new Set(stdout.split('\0').filter(Boolean))].sort()) {
    if (path.startsWith('.beads/') || path.startsWith('.env') && path !== '.env.example') continue;
    try {
      const stat = await lstat(join(base, path));
      if (stat.isSymbolicLink()) throw new Error(`Source symlink is unsupported: ${path}`);
      if (stat.isFile()) files[path] = hash(await readFile(join(base, path)));
    } catch (error) { if (error.code === 'ENOENT') files[path] = 'deleted'; else throw error; }
  }
  return { revision: revision.trim(), sourceHash: hash(JSON.stringify(files)), files };
}

export async function directoryIdentity(path) {
  const entries = {};
  async function visit(dir) {
    for (const item of await readdir(dir, { withFileTypes: true })) {
      const full = join(dir, item.name);
      if (item.isSymbolicLink()) throw new Error('Artifact bundle cannot contain symlinks');
      if (item.isDirectory()) await visit(full);
      else if (item.isFile()) entries[relative(path, full)] = hash(await readFile(full));
    }
  }
  await visit(path);
  const sorted = Object.fromEntries(Object.entries(entries).sort(([a], [b]) => a.localeCompare(b)));
  return { bundleHash: hash(JSON.stringify(sorted)), files: sorted };
}

export async function createRun(kind, task, base = root) {
  const id = `${new Date().toISOString().replaceAll(/[:.]/g, '-')}-${randomUUID().slice(0, 8)}`;
  const dir = join(base, 'artifacts', 'runs', id);
  await mkdir(dir, { recursive: true });
  const source = await sourceIdentity(base);
  await atomicJSON(join(dir, 'manifest.json'), { id, kind, task, status: 'running', startedAt: new Date().toISOString(), ...source });
  const finish = async result => {
    const manifest = await readJSON(join(dir, 'manifest.json'));
    await atomicJSON(join(dir, 'manifest.json'), { ...manifest, ...result, finishedAt: new Date().toISOString() });
  };
  return { id, dir, source, finish };
}
