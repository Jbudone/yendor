// Stdio MCP adapter keeps the local bearer token out of client configuration.
import { Server } from '@modelcontextprotocol/sdk/server/index.js';
import { StdioServerTransport } from '@modelcontextprotocol/sdk/server/stdio.js';
import { ListToolsRequestSchema, CallToolRequestSchema, ListResourcesRequestSchema, ReadResourceRequestSchema, ListResourceTemplatesRequestSchema } from '@modelcontextprotocol/sdk/types.js';
import { connectMail, startMail } from './mail.mjs';
await startMail();
const upstream = await connectMail();
const server = new Server({ name: 'yendor-agent-mail', version: '1.0.0' }, { capabilities: { tools: {}, resources: {} } });
const allowed = new Set(['ensure_project', 'register_agent', 'list_agents', 'send_message', 'reply_message', 'fetch_inbox', 'acknowledge_message', 'file_reservation_paths', 'renew_file_reservations', 'release_file_reservations', 'search_messages', 'get_message_delivery_receipt']);
server.setRequestHandler(ListToolsRequestSchema, async () => ({ tools: (await upstream.listTools()).tools.filter(tool => allowed.has(tool.name)).map(tool => ({ ...tool, description: tool.description?.slice(0, 500) })) }));
server.setRequestHandler(CallToolRequestSchema, request => { if (!allowed.has(request.params.name)) throw new Error('Tool outside Yendor coordination scope'); return upstream.callTool(request.params); });
server.setRequestHandler(ListResourcesRequestSchema, () => upstream.listResources());
server.setRequestHandler(ListResourceTemplatesRequestSchema, () => upstream.listResourceTemplates());
server.setRequestHandler(ReadResourceRequestSchema, request => upstream.readResource(request.params));
await server.connect(new StdioServerTransport());
process.once('SIGTERM', async () => { await upstream.close(); await server.close(); });
