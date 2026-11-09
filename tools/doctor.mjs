import { existsSync } from 'node:fs';
import { join } from 'node:path';
import { root } from './workflow/core.mjs';
for (const [label, ok] of [
  ['Unity project', existsSync(join(root, 'game/ProjectSettings/ProjectVersion.txt'))],
  ['Mastra installed', existsSync(join(root, 'node_modules/@mastra/core/package.json'))],
  ['Beads binary', existsSync(join(root, '.tools/bin/bd'))],
  ['Beads database', existsSync(join(root, '.beads/metadata.json'))],
  ['Agent Mail binary', existsSync(join(root, '.tools/bin/mcp-agent-mail'))],
  ['Gitleaks binary', existsSync(join(root, '.tools/bin/gitleaks'))],
  ['LiteLLM installed', existsSync(join(root, '.tools/litellm/bin/litellm'))],
  ['Gateway local key configured', Boolean(process.env.LITELLM_API_KEY)],
  ['OpenRouter key configured', Boolean(process.env.OPENROUTER_API_KEY)],
  ['Meshy key configured', Boolean(process.env.MESHY_API_KEY)],
  ['Langfuse keys/region configured', Boolean(process.env.LANGFUSE_PUBLIC_KEY && process.env.LANGFUSE_SECRET_KEY && process.env.LANGFUSE_BASE_URL)],
  ['Langfuse export enabled', process.env.ENABLE_LANGFUSE === 'true'],
  ['Graphics environment', Boolean(process.env.DISPLAY || process.env.WAYLAND_DISPLAY)]
]) console.log(`${ok ? 'OK' : 'PENDING'}  ${label}`);
console.log('Presence is not live verification. Use gateway status, pilot:mail and a real traced run.');
console.log('Unity AI still requires Cloud project selection/activation in the Editor. No credentials are printed.');
