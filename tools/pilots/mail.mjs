import assert from 'node:assert/strict';
import { randomUUID } from 'node:crypto';
import { join } from 'node:path';
import { connectMail, mailCall } from '../mail.mjs';
import { root, atomicJSON } from '../workflow/core.mjs';
const unpack = result => result.structuredContent || JSON.parse(result.content.find(x => x.type === 'text').text);
const left = await connectMail(), right = await connectMail();
const thread = `setup-pilot-${randomUUID()}`;
let a, b;
try {
  await mailCall(left, 'ensure_project', { human_key: root });
  a = unpack(await mailCall(left, 'register_agent', { project_key: root, program: 'yendor-pilot-a', model: 'no-model', task_description: 'Local configuration test; no implementation work.' }));
  b = unpack(await mailCall(right, 'register_agent', { project_key: root, program: 'yendor-pilot-b', model: 'no-model', task_description: 'Local configuration test; no implementation work.' }));
  const sent = unpack(await mailCall(left, 'send_message', { project_key: root, sender_name: a.name, to: [b.name], subject: 'Workflow configuration pilot', body_md: 'Local test message. No action requested. Evidence only.', thread_id: thread, ack_required: true }));
  const inbox = unpack(await mailCall(right, 'fetch_inbox', { project_key: root, agent_name: b.name, include_bodies: true, limit: 10 }));
  const messages = Array.isArray(inbox) ? inbox : inbox.messages;
  const message = messages.find(x => x.thread_id === thread || x.subject === 'Workflow configuration pilot');
  assert.ok(message, 'Message must arrive');
  await mailCall(right, 'acknowledge_message', { project_key: root, agent_name: b.name, message_id: message.id });
  const paths = [`docs/pilots/${thread}.md`];
  const first = unpack(await mailCall(left, 'file_reservation_paths', { project_key: root, agent_name: a.name, paths, ttl_seconds: 60, exclusive: true, reason: thread }));
  assert.ok(first.granted?.length);
  const second = unpack(await mailCall(right, 'file_reservation_paths', { project_key: root, agent_name: b.name, paths, ttl_seconds: 60, exclusive: true, reason: thread }));
  assert.ok(second.conflicts?.length, 'Overlapping ownership must be visible');
  await mailCall(left, 'release_file_reservations', { project_key: root, agent_name: a.name });
  await mailCall(right, 'release_file_reservations', { project_key: root, agent_name: b.name });
  await atomicJSON(join(root, 'artifacts/pilots/mail.json'), { passed: true, at: new Date().toISOString(), thread, agents: [a.name, b.name], checks: ['separate client identities', 'delivery', 'acknowledgment', 'advisory conflict', 'release'] });
  console.log('Agent Mail pilot passed: two clients, delivery, acknowledgment and reservation conflict.');
} finally { await left.close(); await right.close(); }
