import { spawn } from 'node:child_process';
import { access, mkdir, writeFile, open, readFile } from 'node:fs/promises';
import { join } from 'node:path';
import { root, exec, hash, atomicJSON } from './workflow/core.mjs';
const dir = join(root, '.workflow/actions-runner');
const repo = 'Jbudone/yendor';
const command = process.argv[2] || 'status';
try {
  if (command === 'install') {
    await mkdir(dir, { recursive: true, mode: 0o700 });
    let configured = true; try { await access(join(dir, '.runner')); } catch { configured = false; }
    if (configured) throw new Error('Runner already configured; use start or status, not reinstallation.');
    const response = await fetch('https://github.com/actions/runner/releases/download/v2.338.2/actions-runner-linux-x64-2.338.2.tar.gz');
    if (!response.ok) throw new Error(`Runner download failed: ${response.status}`);
    const bytes = Buffer.from(await response.arrayBuffer());
    if (hash(bytes) !== 'baebf204307bac6193480852732f0aba2ae97b6c5597c244f4d5558390147c40') throw new Error('Runner archive checksum mismatch');
    const archive = join(root, '.workflow/actions-runner.tar.gz'); await writeFile(archive, bytes);
    await exec('tar', ['-xzf', archive, '-C', dir]);
    const { stdout } = await exec('gh', ['api', `repos/${repo}/actions/runners/registration-token`, '--method', 'POST']);
    const token = JSON.parse(stdout).token;
    try {
      const configured = await exec(join(dir, 'config.sh'), ['--unattended', '--url', `https://github.com/${repo}`, '--token', token, '--name', 'yendor-unity-linux', '--labels', 'yendor-unity', '--work', '_work'], { cwd: dir, timeout: 120000 });
      await writeFile(join(dir, 'configuration.log'), (configured.stdout + configured.stderr).replaceAll(token, '[redacted]'), { mode: 0o600 });
    } catch { throw new Error('Runner configuration failed. Registration token was not logged. Inspect platform prerequisites before retrying.'); }
    console.log('Pinned GitHub runner installed and registered; registration token was not printed.');
  } else if (command === 'start') {
    await access(join(dir, '.runner'));
    const { stdout } = await exec('gh', ['api', `repos/${repo}/actions/runners`]);
    if (JSON.parse(stdout).runners.some(x => x.name === 'yendor-unity-linux' && x.status === 'online')) { console.log('Unity runner already online.'); process.exit(0); }
    const log = await open(join(dir, 'service.log'), 'a', 0o600);
    const child = spawn(join(dir, 'run.sh'), [], { cwd: dir, detached: true, stdio: ['ignore', log.fd, log.fd], env: process.env });
    await new Promise((accept, reject) => { child.once('spawn', accept); child.once('error', reject); });
    await atomicJSON(join(dir, 'process.json'), { pid: child.pid, startedAt: new Date().toISOString() });
    child.unref(); await log.close(); console.log(`Unity runner started: ${child.pid}`);
  } else if (command === 'stop') {
    const state = JSON.parse(await readFile(join(dir, 'process.json'), 'utf8'));
    const cmdline = await readFile(`/proc/${state.pid}/cmdline`, 'utf8');
    if (!cmdline.includes(join(dir, 'run.sh'))) throw new Error('Recorded PID no longer belongs to this runner');
    process.kill(state.pid, 'SIGTERM'); console.log('Unity runner stopped.');
  } else {
    const { stdout } = await exec('gh', ['api', `repos/${repo}/actions/runners`, '--jq', '{total_count, runners: [.runners[] | {name,status,busy,labels:[.labels[].name]}]}']);
    process.stdout.write(stdout);
  }
} catch (error) { console.error(error.message); process.exitCode = 1; }
