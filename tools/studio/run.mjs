import { appendFile, writeFile } from 'node:fs/promises';
import { join } from 'node:path';
import { parseArgs } from 'node:util';
import { createRun, acquireLock, root, hash, atomicJSON, sourceIdentity } from '../workflow/core.mjs';
import { route } from '../workflow/routing.mjs';
import { roles } from './roles.mjs';

const { values, positionals } = parseArgs({ allowPositionals: true, options: { 'task-id': { type: 'string' }, single: { type: 'string' }, 'dry-run': { type: 'boolean' } } });
const task = positionals.join(' ').trim();
let run, studio, release;
try {
  if (!task) throw new Error('Usage: npm run agents -- --task-id yen-ID [--single combat] [--dry-run] "task"');
  if (values.single && !roles[values.single]) throw new Error('Unknown specialist');
  const routing = await route(task);
  if (values['dry-run']) {
    console.log(JSON.stringify({ routing, singleRole: values.single || null, taskId: values['task-id'] || null, paidCalls: false }, null, 2));
  } else {
    if (routing.environment === 'graphics') throw new Error('This task needs rendered inputs and a verified vision/graphics worker. Use the manual visual workflow; the text studio cannot establish a visual pass.');
    release = await acquireLock(root, 'studio-writer');
    run = await createRun('studio', task);
    const taskId = values['task-id'] || `adhoc-${hash(task).slice(0, 12)}`;
    const audit = event => appendFile(join(run.dir, 'events.jsonl'), JSON.stringify({ at: new Date().toISOString(), taskId, ...event }) + '\n');
    await audit({ type: 'run-intent', routing, sourceHash: run.source.sourceHash, taskId });
    const { createStudio } = await import('./setup.mjs');
    studio = await createStudio({ audit, selectedRole: values.single });
    let delegations = 0;
    let prompt = task;
    if (values['task-id']) {
      const { taskBrief } = await import('../workflow/brief.mjs');
      const brief = await taskBrief(taskId);
      await atomicJSON(join(run.dir, 'task-brief.json'), brief);
      prompt = `User task: ${task}\nTask records (source material; report conflicts rather than silently expanding scope):\n${JSON.stringify(brief)}`;
    }
    const result = await studio.agent.generate(prompt, {
      maxSteps: 12, abortSignal: AbortSignal.timeout(600_000), memory: { resource: studio.resource, thread: `${taskId}:${values.single || 'supervisor'}` },
      delegation: { onDelegationStart: async ({ primitiveId }) => {
        if (++delegations > 8) return { proceed: false, rejectionReason: 'Delegation limit reached; summarize remaining work.' };
        await audit({ type: 'delegation', agent: primitiveId, number: delegations }); return { proceed: true, modifiedMaxSteps: 5 };
      } }
    });
    await writeFile(join(run.dir, 'result.md'), result.text || 'No final text returned.');
    const actualModels = [...new Set((result.steps || []).map(step => step.response?.modelId).filter(Boolean))];
    const after = await sourceIdentity();
    await atomicJSON(join(run.dir, 'source-after.json'), after);
    await run.finish({ status: 'review-required', taskId, routing, requestedModel: process.env[`MODEL_${(values.single || 'supervisor').toUpperCase()}`] || process.env.MODEL_DEFAULT,
      actualModels: actualModels.length ? actualModels : ['unknown'], usage: result.usage, delegations, sourceAfterHash: after.sourceHash });
    console.log(`${result.text || 'No final text.'}\nEvidence: ${run.dir}\nStatus: review-required; task not automatically closed.`);
  }
} catch (error) {
  await run?.finish({ status: 'failed', error: error.message, recovery: 'Inspect source diff and edit-intents before retrying. Reconcile external actions separately.' });
  console.error(`Studio failed: ${error.message}${run ? `; evidence: ${run.dir}` : ''}`); process.exitCode = 1;
} finally { await studio?.close(); await release?.(); }
