import { spawn } from 'node:child_process';
import { mkdir, readFile, writeFile, open } from 'node:fs/promises';
import { randomBytes } from 'node:crypto';
import { join, resolve } from 'node:path';
import { fileURLToPath } from 'node:url';
import { root, atomicJSON, readJSON } from './workflow/core.mjs';
import { Client } from '@modelcontextprotocol/sdk/client/index.js';
import { StreamableHTTPClientTransport } from '@modelcontextprotocol/sdk/client/streamableHttp.js';

const port = process.env.YENDOR_MAIL_PORT || '18765';
export const mailURL = `http://127.0.0.1:${port}/mcp/`;
const dir = join(root, '.workflow', 'mail');
export async function connectMail() {
  const token = (await readFile(join(root, '.workflow', 'mail-token'), 'utf8')).trim();
  const client = new Client({ name: 'yendor-workflow', version: '1.0.0' });
  await client.connect(new StreamableHTTPClientTransport(new URL(mailURL), { requestInit: { headers: { Authorization: `Bearer ${token}` } } }));
  return client;
}
export async function mailCall(client, name, args) {
  const result = await client.callTool({ name, arguments: args });
  if (result.isError) throw new Error(JSON.stringify(result.content));
  return result;
}
export async function startMail() {
  try { const client = await connectMail(); await client.listTools(); await client.close(); return { ready: true, url: mailURL, reused: true }; } catch {}
  await mkdir(dir, { recursive: true });
  const tokenPath = join(root, '.workflow', 'mail-token');
  let token;
  try { token = (await readFile(tokenPath, 'utf8')).trim(); }
  catch (error) { if (error.code !== 'ENOENT') throw error; token = randomBytes(32).toString('hex'); await writeFile(tokenPath, token, { mode: 0o600, flag: 'wx' }); }
  const log = await open(join(dir, 'server.log'), 'a', 0o600);
  const child = spawn(join(root, '.tools', 'bin', 'mcp-agent-mail'), ['serve', '--host', '127.0.0.1', '--port', port, '--no-tui', '--no-reuse-running'], {
    cwd: root, detached: true, stdio: ['ignore', log.fd, log.fd],
    env: { PATH: process.env.PATH, HOME: process.env.HOME, STORAGE_ROOT: dir, HTTP_BEARER_TOKEN: token, AM_ATC_ENABLED: 'false', ATC_LEARNING_DISABLED: 'true' }
  });
  await new Promise((accept, reject) => { child.once('spawn', accept); child.once('error', reject); });
  await atomicJSON(join(dir, 'process.json'), { pid: child.pid, url: mailURL, startedAt: new Date().toISOString() });
  child.unref(); await log.close();
  for (let attempt = 0; attempt < 30; attempt++) {
    try { const client = await connectMail(); await client.listTools(); await client.close(); return { ready: true, url: mailURL, pid: child.pid }; } catch {}
    await new Promise(accept => setTimeout(accept, 300));
  }
  throw new Error(`Agent Mail did not become ready; inspect ${dir}/server.log`);
}

if (process.argv[1] && resolve(process.argv[1]) === fileURLToPath(import.meta.url)) {
  try {
    const [command = 'status', tool, args] = process.argv.slice(2);
    if (command === 'start') console.log(JSON.stringify(await startMail(), null, 2));
    else if (command === 'stop') {
      const state = await readJSON(join(dir, 'process.json'));
      // Ensure a stale PID was not reused before signalling it (Linux local runner).
      const cmdline = await readFile(`/proc/${state.pid}/cmdline`, 'utf8');
      if (!cmdline.includes(join(root, '.tools', 'bin', 'mcp-agent-mail'))) throw new Error('PID no longer belongs to this mail service');
      process.kill(state.pid, 'SIGTERM'); console.log('Agent Mail stopped.');
    } else {
      const client = await connectMail();
      try {
        if (command === 'status') console.log(JSON.stringify({ ready: true, url: mailURL, toolCount: (await client.listTools()).tools.length }));
        else if (command === 'tools') console.log(JSON.stringify((await client.listTools()).tools, null, 2));
        else if (command === 'call') console.log(JSON.stringify(await mailCall(client, tool, JSON.parse(args || '{}')), null, 2));
        else throw new Error('Use start, stop, status, tools, or call <tool> <json>.');
      } finally { await client.close(); }
    }
  } catch (error) { console.error(error.message); process.exitCode = 1; }
}
