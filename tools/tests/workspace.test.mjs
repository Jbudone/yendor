import { test } from 'node:test';
import assert from 'node:assert/strict';
import { mkdtemp, mkdir, symlink, readFile, rm } from 'node:fs/promises';
import { tmpdir } from 'node:os';
import { join } from 'node:path';
import { scopedPath, writeProject } from '../studio/workspace.mjs';
import { modelFor } from '../studio/roles.mjs';

test('specialists cannot escape their directories, follow symlinks or overwrite scene YAML', async () => {
  const root = await mkdtemp(join(tmpdir(), 'yendor-scope-'));
  try {
    await mkdir(join(root, 'combat'));
    await symlink(tmpdir(), join(root, 'combat', 'escape'));
    await assert.rejects(scopedPath(root, '../outside.cs', ['combat']), /scope/);
    await assert.rejects(scopedPath(root, 'combat-neighbor/file.cs', ['combat']), /scope/);
    await assert.rejects(scopedPath(root, 'combat/escape/file.cs', ['combat']), /Symlinks/);
    await assert.rejects(writeProject(root, 'combat/scene.unity', 'bad', ['combat']), /Only/);
    await assert.rejects(writeProject(root, 'combat/config.json', '{bad', ['combat']), SyntaxError);
    await writeProject(root, 'combat/New.cs', 'public class New {}', ['combat']);
    assert.equal(await readFile(join(root, 'combat/New.cs'), 'utf8'), 'public class New {}');
  } finally { await rm(root, { recursive: true, force: true }); }
});

test('model aliases are independently routed to LiteLLM and fail early without credentials', () => {
  const env = { MODEL_DEFAULT: 'cheap', MODEL_COMBAT: 'coder', LITELLM_API_KEY: 'test' };
  assert.equal(modelFor('combat', env).modelId, 'coder');
  assert.equal(modelFor('art', env).modelId, 'cheap');
  assert.equal(modelFor('art', env).api, 'chat');
  assert.throws(() => modelFor('world', {}), /MODEL_WORLD/);
  assert.throws(() => modelFor('world', { MODEL_DEFAULT: 'x' }), /LITELLM_API_KEY/);
});
