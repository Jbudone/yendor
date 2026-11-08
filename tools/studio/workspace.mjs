import { lstat, mkdir, readFile, realpath, readdir, writeFile } from 'node:fs/promises';
import { resolve, dirname, relative, sep } from 'node:path';

export const readableRoots = ['CONTEXT.md', 'README.md', 'docs', 'content', 'game/Assets/Yendor'];
const inside = (path, base) => path === base || path.startsWith(base + sep);

// Agent tools are for trusted local development. Scoped paths prevent accidental
// edits outside a specialty; they are not an OS sandbox for untrusted code.
export async function scopedPath(root, requested, allowed) {
  const base = await realpath(root);
  const target = resolve(base, requested);
  if (!allowed.some(path => inside(target, resolve(base, path)))) throw new Error('Path outside allowed scope');
  let cursor = base;
  for (const part of relative(base, target).split(sep)) {
    cursor = resolve(cursor, part);
    try { if ((await lstat(cursor)).isSymbolicLink()) throw new Error('Symlinks are not allowed'); }
    catch (error) { if (error.code !== 'ENOENT') throw error; }
  }
  return target;
}

export async function readProject(root, path) {
  const target = await scopedPath(root, path, readableRoots);
  const data = await readFile(target, 'utf8');
  if (data.length > 100_000) throw new Error('File exceeds 100 KB tool limit');
  return data;
}

export async function listProject(root, path = 'game/Assets/Yendor') {
  const target = await scopedPath(root, path, readableRoots);
  return (await readdir(target, { withFileTypes: true })).map(entry => ({ name: entry.name, directory: entry.isDirectory() }));
}

export async function writeProject(root, path, content, scopes) {
  const target = await scopedPath(root, path, scopes);
  if (!/\.(cs|md|json)$/.test(target)) throw new Error('Only C#, Markdown, and JSON edits are allowed');
  if (content.length > 100_000) throw new Error('File exceeds 100 KB tool limit');
  if (target.endsWith('.json')) JSON.parse(content);
  await mkdir(dirname(target), { recursive: true });
  await writeFile(target, content, 'utf8');
  return { written: path, bytes: Buffer.byteLength(content) };
}
