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

test('paid submission intent survives an ambiguous transport failure without retry', async () => {
  const { mkdtemp, writeFile, readdir, readFile, rm } = await import('node:fs/promises');
  const { tmpdir } = await import('node:os');
  const { join } = await import('node:path');
  const { submitPreview } = await import('../meshy.mjs');
  const base = await mkdtemp(join(tmpdir(), 'yendor-meshy-recovery-'));
  const previousKey = process.env.MESHY_API_KEY;
  process.env.MESHY_API_KEY = 'synthetic-local-test';
  let calls = 0;
  try {
    const brief = join(base, 'brief.json'); await writeFile(brief, JSON.stringify({ prompt: 'Test prop', target_polycount: 1000 }));
    await assert.rejects(submitPreview(brief, { recordRoot: base, fetcher: async () => {
      calls++;
      const files = await readdir(join(base, '.runs/meshy'));
      const intent = JSON.parse(await readFile(join(base, '.runs/meshy', files[0]), 'utf8'));
      assert.equal(intent.state, 'submitting', 'Intent must exist before transport begins');
      throw new Error('Lost response after simulated acceptance');
    } }), /Reconcile/);
    const files = await readdir(join(base, '.runs/meshy'));
    const record = JSON.parse(await readFile(join(base, '.runs/meshy', files[0]), 'utf8'));
    assert.equal(record.state, 'ambiguous'); assert.equal(calls, 1);
  } finally {
    if (previousKey === undefined) delete process.env.MESHY_API_KEY; else process.env.MESHY_API_KEY = previousKey;
    await rm(base, { recursive: true, force: true });
  }
});
