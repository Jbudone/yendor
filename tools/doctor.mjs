import { existsSync } from 'node:fs';
import { root } from './unity.mjs';
import { resolve } from 'node:path';

for (const [label, ok] of [
  ['Unity project', existsSync(resolve(root, 'game/ProjectSettings/ProjectVersion.txt'))],
  ['Mastra installed', existsSync(resolve(root, 'node_modules/@mastra/core/package.json'))],
  ['LiteLLM key configured', Boolean(process.env.LITELLM_API_KEY)],
  ['Default model alias configured', Boolean(process.env.MODEL_DEFAULT)],
  ['Meshy key configured', Boolean(process.env.MESHY_API_KEY)]
]) console.log(`${ok ? 'OK' : 'MISSING'}  ${label}`);
console.log('Unity AI: verify the linked Cloud project and subscription in the Unity Editor.');
console.log('This check does not contact providers or spend credits.');
