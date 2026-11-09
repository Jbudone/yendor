import { root, exec, readJSON, atomicJSON, acquireLock } from './workflow/core.mjs';
import { join } from 'node:path';
const release = await acquireLock(root, 'beads');
try {
  const bd = async args => { const { stdout } = await exec(join(root, '.tools/bin/bd'), args, { cwd: root, timeout: 120000, maxBuffer: 4000000 }); return JSON.parse(stdout); };
  const existing = await bd(['list', '--all', '--json']);
  const tasks = Array.isArray(existing) ? existing : existing.issues;
  const mapping = {};
  for (const item of await readJSON(join(root, 'config/workflow-seed.json'))) {
    const ref = `yendor-workflow:${item.key}`;
    let task = tasks.find(task => task.external_ref === ref);
    if (!task) task = await bd(['create', item.title, '--type', item.type, '--priority', String(item.priority), '--description', item.description, '--acceptance', 'Record actual evidence and limitations, update accepted records if direction changes, and leave a clear handoff.', '--external-ref', ref, '--json']);
    mapping[item.key] = task.id;
    console.log(`${item.key}: ${task.id}`);
  }
  await atomicJSON(join(root, 'artifacts/workflow-task-map.json'), mapping);
} finally { await release(); }
