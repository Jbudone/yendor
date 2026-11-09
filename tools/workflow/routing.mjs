import { readJSON, root } from './core.mjs';
import { join } from 'node:path';

export function classify(task) {
  const text = task.toLowerCase();
  if (/screenshot|visual review|animation review/.test(text)) return { kind: 'visual', role: 'art', profile: 'vision', environment: 'graphics' };
  if (/meshy|texture|material|asset brief/.test(text)) return { kind: 'asset', role: 'art', profile: 'general', environment: 'editor' };
  if (/architecture|contract|design|specification/.test(text)) return { kind: 'design', role: 'design', profile: 'reasoning', environment: 'text' };
  if (/review|test|compile|build|regression/.test(text)) return { kind: 'verification', role: 'qa', profile: 'review', environment: 'unity' };
  if (/scene|prefab|world|route|terrain/.test(text)) return { kind: 'implementation', role: 'world', profile: 'code', environment: 'unity' };
  if (/status|item|relic|inventory|element/.test(text)) return { kind: 'implementation', role: 'systems', profile: 'code', environment: 'unity' };
  return { kind: 'implementation', role: 'combat', profile: 'code', environment: 'unity' };
}

export async function route(task, env = process.env) {
  const config = await readJSON(join(root, 'config', 'workers.json'));
  const result = classify(task);
  const profile = config.profiles[result.profile];
  const alias = env[profile.modelEnv] || env[`MODEL_${result.role.toUpperCase()}`] || env.MODEL_DEFAULT;
  return { ...result, alias: alias || null, tools: profile.capabilities, configured: Boolean(alias && env.LITELLM_API_KEY), runnable: false, runner: profile.runner,
    limitations: ['Classification is advisory; scopes and capabilities are enforced separately.', 'Actual serving deployment must be recorded or marked unknown.'] };
}
