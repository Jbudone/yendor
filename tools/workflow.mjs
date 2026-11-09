import { readFile, mkdir, writeFile, readdir } from 'node:fs/promises';
import { join, resolve } from 'node:path';
import { root, exec, readJSON, atomicJSON, sourceIdentity, acquireLock } from './workflow/core.mjs';
import { route } from './workflow/routing.mjs';
import { recall } from './workflow/memory.mjs';
import { startMail } from './mail.mjs';
const sharedRoot = (await readJSON(join(root, '.workflow/shared-root.json')).catch(() => null))?.root || root;
const [command = 'status', ...args] = process.argv.slice(2);
async function bd(params) {
  const release = await acquireLock(sharedRoot, 'beads');
  try { const { stdout } = await exec(join(sharedRoot, '.tools/bin/bd'), params, { cwd: sharedRoot, maxBuffer: 4_000_000, timeout: 120_000 }); return stdout; }
  finally { await release(); }
}
async function status() {
  const { stdout: revision } = await exec('git', ['rev-parse', '--short', 'HEAD'], { cwd: root });
  const { stdout: remote } = await exec('git', ['remote', 'get-url', 'origin'], { cwd: root }).catch(() => ({ stdout: '' }));
  let tasks;
  try { tasks = JSON.parse(await bd(['ready', '--json'])); } catch (error) { tasks = { error: error.message.slice(0, 500) }; }
  const configured = Object.fromEntries(['LITELLM_API_KEY', 'MESHY_API_KEY', 'LANGFUSE_PUBLIC_KEY', 'LANGFUSE_SECRET_KEY'].map(key => [key, Boolean(process.env[key])]));
  const locks = await readdir(join(root, '.workflow/locks')).catch(() => []);
  return { root, revision: revision.trim(), remote: remote.trim() || null, configured, locks, readyTasks: tasks, next: 'Read STATUS.md and CONTEXT.md; inspect one task and relevant sources.' };
}
try {
  if (command === 'status') console.log(JSON.stringify(await status(), null, 2));
  else if (command === 'start') { console.log(JSON.stringify(await startMail())); console.log(JSON.stringify(await status(), null, 2)); }
  else if (command === 'task') process.stdout.write(await bd(args));
  else if (command === 'brief') { const { taskBrief } = await import('./workflow/brief.mjs'); console.log(JSON.stringify(await taskBrief(args[0]), null, 2)); }
  else if (command === 'route') console.log(JSON.stringify(await route(args.join(' ')), null, 2));
  else if (command === 'recall') {
    const { stdout } = await exec('git', ['branch', '--show-current'], { cwd: root });
    console.log(JSON.stringify(await recall(args.join(' '), { branch: stdout.trim() }), null, 2));
  } else if (command === 'handoff') {
    if (!args[0] || !args[1]) throw new Error('handoff <task-id> <summary-file>');
    const summary = await readFile(resolve(args[1]), 'utf8');
    await mkdir(join(root, 'docs/handoffs'), { recursive: true });
    const name = args[0].replace(/[^a-zA-Z0-9-]/g, '');
    if (name !== args[0]) throw new Error('Invalid task ID');
    const identity = await sourceIdentity();
    const path = join(root, 'docs/handoffs', `${name}.md`);
    await writeFile(path, `# Handoff: ${name}\n\nDate: ${new Date().toISOString()}\nSource: ${identity.revision}\nSource hash: ${identity.sourceHash}\n\n${summary}\n`);
    console.log(path);
  } else if (command === 'worktree') {
    const id = args[0];
    if (!/^yen-[a-z0-9.-]+$/.test(id || '')) throw new Error('Use a valid Beads task ID');
    await bd(['show', id, '--json']);
    await mkdir(join(root, '.worktrees'), { recursive: true });
    const path = resolve(root, '.worktrees', id);
    const { stdout } = await exec('git', ['worktree', 'add', '-b', `task/${id}`, path, 'HEAD'], { cwd: root });
    await atomicJSON(join(path, '.workflow/shared-root.json'), { root: sharedRoot });
    console.log(stdout + `\nWorktree: ${path}\nUse the primary checkout for Beads and Agent Mail; each Unity copy owns its own Library.\n`);
  } else if (command === 'recover') {
    const runs = join(root, 'artifacts/runs');
    const unfinished = [];
    for (const name of await readdir(runs).catch(() => [])) {
      const manifest = await readJSON(join(runs, name, 'manifest.json')).catch(() => null);
      if (manifest?.status === 'running' || manifest?.status === 'failed') unfinished.push({ id: name, status: manifest.status, error: manifest.error });
    }
    const external = [];
    for (const name of await readdir(join(root, '.runs/meshy')).catch(() => [])) {
      const record = await readJSON(join(root, '.runs/meshy', name)).catch(() => null);
      if (['submitting', 'ambiguous'].includes(record?.state)) external.push({ file: name, state: record.state });
    }
    console.log(JSON.stringify({ unfinished, external, action: 'Inspect source diffs, lock owners and provider records. Recovery does not automatically retry or remove locks.' }, null, 2));
  } else if (command === 'backup') {
    await mkdir(join(root, 'artifacts/backups'), { recursive: true });
    console.log(await bd(['backup', 'sync']));
  } else throw new Error('Use start, status, task, route, recall, handoff, worktree, recover or backup.');
} catch (error) { console.error(error.message); process.exitCode = 1; }
