import { cp, mkdir } from 'node:fs/promises';
import { join } from 'node:path';
import { root, atomicJSON, sourceIdentity } from './workflow/core.mjs';
const destination = join(root, '.workflow', `verify-${Date.now()}`, 'game');
await mkdir(destination, { recursive: true });
for (const name of ['Assets', 'Packages', 'ProjectSettings']) await cp(join(root, 'game', name), join(destination, name), { recursive: true });
await atomicJSON(join(destination, '..', 'source.json'), await sourceIdentity());
console.log(destination);
