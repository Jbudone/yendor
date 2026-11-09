import { test } from 'node:test';
import { spawn } from 'node:child_process';
import { randomBytes } from 'node:crypto';
import { existsSync } from 'node:fs';
import { join } from 'node:path';
import assert from 'node:assert/strict';
import { root } from '../workflow/core.mjs';
const binary = join(root, '.tools/bin/gitleaks');
test('Gitleaks rejects a generated synthetic credential and redacts it', { skip: !existsSync(binary) }, async () => {
  const synthetic = ['gh', 'p_', randomBytes(20).toString('hex')].join('');
  const result = await new Promise((accept, reject) => {
    const child = spawn(binary, ['stdin', '--redact', '--no-banner', '--log-level', 'error']);
    let output = '';
    child.stdout.on('data', part => output += part); child.stderr.on('data', part => output += part);
    child.on('error', reject); child.on('close', code => accept({ code, output }));
    child.stdin.end(`token = "${synthetic}"\n`);
  });
  assert.equal(result.code, 1, 'Synthetic credential should block commit');
  assert.ok(!result.output.includes(synthetic), 'Credential must not appear in scan output');
});
