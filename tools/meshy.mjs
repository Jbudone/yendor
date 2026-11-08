import { readFile, mkdir, writeFile } from 'node:fs/promises';
import { resolve } from 'node:path';
import { root } from './unity.mjs';

async function request(path, { method = 'GET', body, fetcher = fetch } = {}) {
  const key = process.env.MESHY_API_KEY;
  if (!key) throw new Error('Set MESHY_API_KEY in the local .env');
  const response = await fetcher(`https://api.meshy.ai${path}`, {
    method, headers: { Authorization: `Bearer ${key}`, 'Content-Type': 'application/json' },
    ...(body ? { body: JSON.stringify(body) } : {}), signal: AbortSignal.timeout(30_000)
  });
  if (!response.ok) throw new Error(`Meshy ${response.status}: ${response.statusText}`);
  return response.json();
}

export const meshBalance = options => request('/openapi/v1/balance', options);
export function meshStatus(id, options) {
  if (!/^[a-zA-Z0-9-]+$/.test(id)) throw new Error('Invalid Meshy task ID');
  return request(`/openapi/v2/text-to-3d/${id}`, options);
}

export function previewPayload(brief) {
  if (typeof brief.prompt !== 'string' || !brief.prompt.trim() || brief.prompt.length > 800) throw new Error('Meshy prompt must contain 1–800 characters');
  if (!Number.isInteger(brief.target_polycount) || brief.target_polycount < 100 || brief.target_polycount > 300_000) throw new Error('Invalid polycount target');
  return { mode: 'preview', prompt: brief.prompt, ai_model: 'meshy-7.1', should_remesh: true, topology: 'triangle', target_polycount: brief.target_polycount };
}

export async function submitPreview(briefPath, options) {
  const brief = JSON.parse(await readFile(briefPath, 'utf8'));
  const payload = previewPayload(brief);
  const response = await request('/openapi/v2/text-to-3d', { ...options, method: 'POST', body: payload });
  const dir = resolve(root, '.runs', 'meshy');
  await mkdir(dir, { recursive: true });
  await writeFile(resolve(dir, `${Date.now()}.json`), JSON.stringify({ brief: briefPath, submittedAt: new Date().toISOString(), payload, response }, null, 2));
  return response;
}
