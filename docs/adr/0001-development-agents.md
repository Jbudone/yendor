# 0001 — Keep AI authoring separate from the game runtime

Status: accepted initial architecture.

Unity owns input, simulation, procedural runs, rendering and saves. Node/Mastra
owns development orchestration. LiteLLM exposes OpenAI-compatible chat endpoints
and maps stable aliases to provider-specific deployments. No model call is needed
to play the game. This keeps combat latency and reproducibility independent of
model availability and token cost.

## Responsibilities

The supervisor decomposes a concrete task, delegates work and evaluates evidence.
Six specialists own combat, systems, world/Editor builders, art, design and QA.
Each specialist receives only scoped write tools. Shared type changes are handed
between owners sequentially. QA and the supervisor can run the fixed Unity
validation command; agents do not get a general shell tool. Build and runtime
smoke checks are explicit commands outside the agent tool loop for now.

Mastra's native `agents` relationship implements delegation. The supervisor has
12 steps, at most 8 delegations, 5 steps per delegation and a 10 minute abort
signal. These are execution bounds, not a currency budget. Configure actual
spend limits in the gateway/provider accounts. LiteLLM retries/timeouts are
bounded separately. Tool edits and validation results persist to `.runs`;
Mastra console traces record model/tool interactions with sensitive-field filtering.
Persistent agent memory, resumable tasks, isolated worktrees, queueing and a
Langfuse exporter are future extensions. Current runs operate on the shared local
workspace and need a single supervisor writer at a time.

## Subscription boundaries

Unity AI is an Editor authoring integration activated against a linked Unity Cloud
project. We do not treat its subscription as a generic LiteLLM provider. Use it
for scene authoring support, material/texture/audio experiments and Editor diagnosis.

Meshy is an asynchronous asset generation API. Art agents write briefs and can
read balance/task status. The CLI submits paid previews, recording task IDs.
Review previews before refinement, and review final topology, scale, materials,
collisions and license/source records before Unity import. Subscription and API
availability must be verified on the user's account; no credentials are assumed.

## Validation and evidence

Compilation plus scene validation establishes that source loads and a bootstrap
exists. A runtime smoke check covers spawn state, damage/status rules, death
counting, relic collection and extraction. It does not establish camera feel,
combat quality, route reachability under player control or visual quality. Those
need manual playtests and rendered captures. Live Mastra/LiteLLM and Meshy checks
need installed packages, reachable services and configured credentials.

Sources checked 2026-11-07:

- [Mastra subagents](https://mastra.ai/docs/subagents)
- [Mastra Agent reference](https://mastra.ai/reference/agents/agent)
- [LiteLLM configuration](https://docs.litellm.ai/docs/proxy/configs)
- [Unity AI activation](https://docs.unity.com/en-us/engine/6000.3/manual/unity-ai/ai-menu-access)
- [Meshy text-to-3D](https://docs.meshy.ai/en/api/text-to-3d)
- [Meshy authentication](https://docs.meshy.ai/en/api/authentication)
- [Mastra tracing](https://mastra.ai/docs/observability/overview)
