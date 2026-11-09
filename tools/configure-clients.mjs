import { writeFile } from 'node:fs/promises';
import { join } from 'node:path';
import { root } from './workflow/core.mjs';
await writeFile(join(root, '.mcp.json'), JSON.stringify({ mcpServers: {
  'yendor-mail': { command: 'node', args: [join(root, 'tools/mail-proxy.mjs')] },
  'yendor-unity': { command: 'unity', args: ['mcp', '--project-path', join(root, 'game')] }
} }, null, 2) + '\n', { mode: 0o600 });
console.log('Created ignored project-local .mcp.json. Register this mail adapter with Codex:');
console.log(`codex mcp add yendor-mail -- node ${JSON.stringify(join(root, 'tools/mail-proxy.mjs'))}`);
