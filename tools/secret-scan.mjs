import { spawn } from 'node:child_process';
import { access } from 'node:fs/promises';
import { join } from 'node:path';
import { root } from './workflow/core.mjs';
const binary = join(root, '.tools/bin/gitleaks');
try { await access(binary); }
catch { console.error('Gitleaks is required. Run: node tools/bootstrap.mjs gitleaks'); process.exit(1); }
const mode = process.argv[2] || 'staged';
if (!['staged', 'history'].includes(mode)) throw new Error('Use staged or history');
const args = ['git', '--redact', '--no-banner', '--config', join(root, '.gitleaks.toml')];
if (mode === 'staged') args.push('--staged', '--pre-commit');
else args.push('--log-opts=--all');
const child = spawn(binary, args, { cwd: root, stdio: 'inherit' });
child.on('error', error => { console.error(error.message); process.exitCode = 1; });
child.on('close', code => { process.exitCode = code ?? 1; });
