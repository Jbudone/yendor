import { spawn } from 'node:child_process';
import { mkdir, open, access } from 'node:fs/promises';
import { join } from 'node:path';
import { root, atomicJSON } from './workflow/core.mjs';
const base = (process.env.LITELLM_BASE_URL || 'http://127.0.0.1:4000/v1').replace(/\/$/, '');
async function status() {
  if (!process.env.LITELLM_API_KEY) throw new Error('Configure LITELLM_API_KEY in .env');
  const response = await fetch(`${base}/models`, { headers: { Authorization: `Bearer ${process.env.LITELLM_API_KEY}` }, signal: AbortSignal.timeout(3000) });
  if (!response.ok) throw new Error(`Gateway model listing failed: ${response.status}`);
  return response.json();
}
try {
  if (process.argv[2] === 'start') {
    if (!process.env.OPENROUTER_API_KEY) throw new Error('Add OPENROUTER_API_KEY to the ignored .env before starting the gateway.');
    try { console.log(JSON.stringify({ ready: true, reused: true, models: await status() })); process.exit(0); } catch {}
    const binary = join(root, '.tools/litellm/bin/litellm'); await access(binary);
    await mkdir(join(root, '.workflow'), { recursive: true });
    const log = await open(join(root, '.workflow/gateway.log'), 'a', 0o600);
    const child = spawn(binary, ['--config', join(root, 'infra/litellm/config.yaml'), '--host', '127.0.0.1', '--port', '4000'], { cwd: root, env: process.env, detached: true, stdio: ['ignore', log.fd, log.fd] });
    await new Promise((accept, reject) => { child.once('spawn', accept); child.once('error', reject); });
    await atomicJSON(join(root, '.workflow/gateway-process.json'), { pid: child.pid, base, startedAt: new Date().toISOString() });
    child.unref(); await log.close();
    for (let attempt = 0; attempt < 40; attempt++) {
      try { const models = await status(); console.log(JSON.stringify({ ready: true, models })); process.exit(0); } catch {}
      await new Promise(accept => setTimeout(accept, 500));
    }
    throw new Error('Gateway did not become ready; inspect .workflow/gateway.log');
  } else console.log(JSON.stringify({ ready: true, models: await status() }, null, 2));
} catch (error) { console.error(error.message); process.exitCode = 1; }
