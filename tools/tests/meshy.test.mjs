import { test } from 'node:test';
import assert from 'node:assert/strict';
import { previewPayload, meshStatus, meshBalance } from '../meshy.mjs';

test('Meshy previews validate prompts and explicitly choose geometry settings', () => {
  assert.throws(() => previewPayload({ prompt: '', target_polycount: 5000 }), /prompt/);
  assert.throws(() => previewPayload({ prompt: 'x'.repeat(801), target_polycount: 5000 }), /prompt/);
  assert.throws(() => previewPayload({ prompt: 'stone', target_polycount: -1 }), /polycount/);
  assert.deepEqual(previewPayload({ prompt: 'stone', target_polycount: 5000, secret: 'omit' }), {
    mode: 'preview', prompt: 'stone', ai_model: 'meshy-7.1', should_remesh: true, topology: 'triangle', target_polycount: 5000
  });
});

test('Meshy read-only commands use the correct endpoint and do not submit jobs', async () => {
  const original = process.env.MESHY_API_KEY;
  process.env.MESHY_API_KEY = 'test';
  const calls = [];
  const fetcher = async (url, options) => { calls.push({ url, options }); return { ok: true, json: async () => ({ balance: 5 }) }; };
  try {
    await meshBalance({ fetcher });
    await meshStatus('task-123', { fetcher });
    assert.equal(calls[0].url, 'https://api.meshy.ai/openapi/v1/balance');
    assert.equal(calls[1].url, 'https://api.meshy.ai/openapi/v2/text-to-3d/task-123');
    assert.ok(calls.every(c => c.options.method === 'GET' && !c.options.body));
    assert.throws(() => meshStatus('../balance'), /Invalid/);
    await assert.rejects(meshBalance({ fetcher: async () => ({ ok: false, status: 401, statusText: 'Unauthorized' }) }), /401/);
  } finally { if (original === undefined) delete process.env.MESHY_API_KEY; else process.env.MESHY_API_KEY = original; }
});
