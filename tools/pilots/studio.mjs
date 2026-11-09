import { createServer } from 'node:http';
import { randomUUID } from 'node:crypto';
import assert from 'node:assert/strict';
import { join } from 'node:path';
import { createStudio } from '../studio/setup.mjs';
import { atomicJSON, root } from '../workflow/core.mjs';
const requests = [];
const server = createServer(async (req, res) => {
  let body = ''; for await (const part of req) body += part;
  const data = JSON.parse(body); requests.push(data);
  const read = data.tools?.find(tool => tool.function.description.includes('Read source'));
  const hasTool = data.messages.some(message => message.role === 'tool');
  const message = !hasTool && read
    ? { role: 'assistant', content: null, tool_calls: [{ id: 'pilot-read', type: 'function', function: { name: read.function.name, arguments: JSON.stringify({ path: 'CONTEXT.md' }) } }] }
    : { role: 'assistant', content: 'Local mock pilot completed. No Unity, visual or live-provider checks were run.' };
  res.writeHead(200, { 'Content-Type': 'application/json' });
  res.end(JSON.stringify({ id: `mock-${requests.length}`, object: 'chat.completion', created: Math.floor(Date.now()/1000), model: 'yendor-mock', choices: [{ index: 0, message, finish_reason: message.tool_calls ? 'tool_calls' : 'stop' }], usage: { prompt_tokens: 30, completion_tokens: 20, total_tokens: 50 } }));
});
await new Promise(accept => server.listen(0, '127.0.0.1', accept));
const env = { MODEL_DEFAULT: 'yendor-mock', LITELLM_API_KEY: 'local-test-only', LITELLM_BASE_URL: `http://127.0.0.1:${server.address().port}/v1` };
const thread = `pilot-${randomUUID()}`;
let studio;
try {
  studio = await createStudio({ env, selectedRole: 'qa' });
  await studio.agent.generate('First turn: inspect project context.', { maxSteps: 3, memory: { resource: studio.resource, thread } });
  await studio.close(); studio = await createStudio({ env, selectedRole: 'qa' });
  await studio.agent.generate('Second turn: recall the earlier context inspection.', { maxSteps: 2, memory: { resource: studio.resource, thread } });
  assert.ok(requests.some(request => request.messages.some(message => message.role === 'tool')), 'A tool result must reach the model');
  assert.ok(requests.at(-1).messages.some(message => JSON.stringify(message.content).includes('First turn')), 'Task history must survive reopening storage');
  await atomicJSON(join(root, 'artifacts/pilots/studio.json'), { passed: true, at: new Date().toISOString(), mockOnly: true, calls: requests.length, checks: ['model request', 'tool call', 'tool result', 'persisted task history after reopening'] });
  console.log('Studio mock pilot passed. Live model behavior and Langfuse delivery remain unverified.');
} finally { await studio?.close(); await new Promise(accept => server.close(accept)); }
