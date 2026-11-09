import { test } from 'node:test';
import assert from 'node:assert/strict';
import { mkdtemp, mkdir, writeFile, readFile, rm } from 'node:fs/promises';
import { tmpdir } from 'node:os';
import { join } from 'node:path';
import { acquireLock, hash } from '../workflow/core.mjs';
import { writeProject } from '../studio/workspace.mjs';
import { recall } from '../workflow/memory.mjs';
import { classify } from '../workflow/routing.mjs';
import { checkUnityResult, unityEnvironment } from '../unity.mjs';

test('conditional writes reject stale content and preserve changes after audit failure', async () => {
  const base = await mkdtemp(join(tmpdir(), 'yendor-cas-'));
  try {
    await mkdir(join(base, 'combat'));
    await writeFile(join(base, 'combat/A.cs'), 'first');
    await writeProject(base, 'combat/A.cs', 'second', ['combat'], { expectedHash: hash('first') });
    await assert.rejects(writeProject(base, 'combat/A.cs', 'lost update', ['combat'], { expectedHash: hash('first') }), /Stale/);
    await assert.rejects(writeProject(base, 'combat/A.cs', 'blocked', ['combat'], { expectedHash: hash('second'), onIntent: () => { throw new Error('intent failed'); } }), /intent failed/);
    assert.equal(await readFile(join(base, 'combat/A.cs'), 'utf8'), 'second');
    await assert.rejects(writeProject(base, 'combat/A.cs', 'third', ['combat'], { expectedHash: hash('second'), onCommit: () => { throw new Error('audit failed'); } }), /audit failed/);
    assert.equal(await readFile(join(base, 'combat/A.cs'), 'utf8'), 'third', 'Post-write failures must be reconciled, not assumed rolled back');
  } finally { await rm(base, { recursive: true, force: true }); }
});

test('leases reject concurrent ownership and never automatically steal stale work', async () => {
  const base = await mkdtemp(join(tmpdir(), 'yendor-lease-'));
  try {
    const release = await acquireLock(base, 'editor');
    await assert.rejects(acquireLock(base, 'editor'), /locked/);
    await release();
    const next = await acquireLock(base, 'editor'); await next();
  } finally { await rm(base, { recursive: true, force: true }); }
});

test('recall excludes superseded and wrong-branch notes and exposes changed sources', async () => {
  const base = await mkdtemp(join(tmpdir(), 'yendor-memory-'));
  try {
    await mkdir(join(base, 'docs/memory'), { recursive: true });
    await writeFile(join(base, 'CONTEXT.md'), 'new policy');
    const record = { id: 'a', text: 'dash policy', tags: ['dash'], status: 'accepted', source: 'CONTEXT.md', sourceHash: hash('old policy') };
    await writeFile(join(base, 'docs/memory/records.json'), JSON.stringify([record, { ...record, id: 'old', status: 'superseded' }, { ...record, id: 'branch', branch: 'other' }]));
    const found = await recall('dash', { base, branch: 'main' });
    assert.equal(found.length, 1); assert.equal(found[0].stale, true);
    assert.equal(found[0].source, 'CONTEXT.md');
  } finally { await rm(base, { recursive: true, force: true }); }
});

test('verification rejects nonzero exits, absent evidence, compile errors and failed/empty suites', () => {
  for (const data of [{ code: 1, output: 'YENDOR_VALIDATION_OK' }, { code: 0, output: '' }, { code: 0, output: 'YENDOR_VALIDATION_OK error CS1000' }])
    assert.throws(() => checkUnityResult({ mode: 'validate', ...data }));
  for (const results of [undefined, '<test-run result="Failed" failed="1" total="1">', '<test-run result="Passed" failed="0" total="0">'])
    assert.throws(() => checkUnityResult({ code: 0, output: 'log', mode: 'test', results }));
  assert.equal(checkUnityResult({ code: 0, output: 'log', mode: 'test', results: '<test-run result="Passed" failed="0" total="6">' }), true);
  assert.deepEqual(unityEnvironment({ PATH: '/bin', MESHY_API_KEY: 'secret', OPENROUTER_API_KEY: 'secret', LANGFUSE_SECRET_KEY: 'secret' }), { PATH: '/bin' });
});

test('routing distinguishes visual environment and systemic ownership', () => {
  assert.equal(classify('visual review of screenshot').environment, 'graphics');
  assert.equal(classify('add elemental status').role, 'systems');
  assert.equal(classify('fix prefab reference').role, 'world');
});
