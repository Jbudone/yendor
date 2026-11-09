import { join } from 'node:path';
import { readJSON, root, hash } from './core.mjs';
import { readFile } from 'node:fs/promises';

export async function recall(query, { base = root, branch = null, limit = 5 } = {}) {
  const records = await readJSON(join(base, 'docs', 'memory', 'records.json'));
  const words = query.toLowerCase().match(/[a-z0-9]+/g) || [];
  const result = [];
  for (const record of records) {
    if (record.status === 'superseded' || record.branch && record.branch !== branch) continue;
    if (!['accepted', 'observation', 'proposal'].includes(record.status) || !record.source) throw new Error('Memory requires status and source');
    const score = words.reduce((n, word) => n + Number(`${record.text} ${record.tags.join(' ')}`.toLowerCase().includes(word)), 0);
    if (!score) continue;
    let stale = false;
    if (record.sourceHash) {
      try { stale = hash(await readFile(join(base, record.source))) !== record.sourceHash; }
      catch { stale = true; }
    }
    result.push({ ...record, score, stale });
  }
  return result.sort((a, b) => b.score - a.score).slice(0, Math.min(limit, 10));
}
