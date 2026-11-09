import { join } from 'node:path';
import { root, exec, readJSON, acquireLock } from './core.mjs';
export async function taskBrief(id) {
  if (!/^yen-[a-z0-9.-]+$/.test(id)) throw new Error('Invalid task ID');
  const shared = (await readJSON(join(root, '.workflow/shared-root.json')).catch(() => null))?.root || root;
  const release = await acquireLock(shared, 'beads');
  let task;
  try {
    const { stdout } = await exec(join(shared, '.tools/bin/bd'), ['show', id, '--json'], { cwd: shared, timeout: 120000, maxBuffer: 1000000 });
    const data = JSON.parse(stdout); task = Array.isArray(data) ? data[0] : data;
  } finally { await release(); }
  if (!task) throw new Error('Task not found');
  const result = { id: task.id, title: task.title, status: task.status, description: task.description?.slice(0, 5000), acceptance: task.acceptance_criteria?.slice(0, 2000), dependencies: task.dependencies };
  const map = await readJSON(join(shared, 'config/github-task-map.json')).catch(() => null);
  const issue = map?.tasks[id];
  if (issue) {
    try {
      const { stdout } = await exec('gh', ['issue', 'view', String(issue.issue), '--repo', map.repo, '--json', 'title,body,comments,labels'], { maxBuffer: 1000000, timeout: 20000 });
      const data = JSON.parse(stdout);
      result.github = { url: issue.url, title: data.title, body: data.body.slice(0, 5000), labels: data.labels.map(x => x.name), comments: data.comments.slice(-3).map(x => ({ author: x.author.login, body: x.body.slice(0, 1500), createdAt: x.createdAt })) };
    } catch (error) { result.github = { url: issue.url, unavailable: true, reason: 'Could not retrieve current issue; do not assume its discussion is unchanged.' }; }
  }
  return result;
}
