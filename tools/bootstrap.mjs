import { platform, arch } from 'node:os';
import { mkdir, writeFile, readFile, chmod } from 'node:fs/promises';
import { join } from 'node:path';
import { root, exec, hash, readJSON } from './workflow/core.mjs';
if (platform() !== 'linux' || arch() !== 'x64') throw new Error('Pinned tool bootstrap currently supports Linux x64; select verified platform-specific releases for another host.');
const requested = new Set(process.argv.slice(2));
const dir = join(root, '.tools/bin');
await mkdir(dir, { recursive: true });
for (const [name, pin] of Object.entries(await readJSON(join(root, 'config/toolchain.json')))) {
  if (requested.size && !requested.has(name)) continue;
  const response = await fetch(pin.url, { signal: AbortSignal.timeout(120000) });
  if (!response.ok) throw new Error(`${name} download failed: ${response.status}`);
  const bytes = Buffer.from(await response.arrayBuffer());
  if (hash(bytes) !== pin.sha256) throw new Error(`${name} archive digest mismatch`);
  const archive = join(root, '.tools', `${name}.archive`);
  await writeFile(archive, bytes);
  for (const member of pin.members) {
    const { stdout } = await exec('tar', ['-xOf', archive, member], { encoding: 'buffer', maxBuffer: 180000000 });
    await writeFile(join(dir, member), stdout); await chmod(join(dir, member), 0o755);
  }
  console.log(`${name} ${pin.version}: pinned archive verified and installed locally`);
}
console.log('Next: npm ci; .tools/bin/bd init --prefix yen --skip-agents --skip-hooks; npm run hooks:install; npm start');
