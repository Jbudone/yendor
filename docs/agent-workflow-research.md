# Agent workflow research — 2026-11-08

Status: research and proposed pilots, not an accepted architecture or installation record.

## Recommendation

Yendor should evaluate persistent task tracking, cross-harness communication,
selective memory retrieval, and capability-based task routing early. Keep one
coordinator responsible for each task and one integration authority for main.
Retain Mastra/LiteLLM as the initial studio implementation while evaluating
whether its existing capabilities cover our needs. Do not install several
overlapping orchestrators or memory systems at once.

This report separates documented product features from recommendations for
Yendor. Sources are vendor documentation, maintained project repositories and
first-person practitioner reports. These establish capabilities and experience,
not an industry consensus or proof that a tool improves this Unity project.
No new services were installed, paid calls made, or model evaluations run.

## Practitioner findings

- Birgitta Böckeler's [Context Engineering for Coding Agents](https://martinfowler.com/articles/exploring-gen-ai/context-engineering-coding-agents.html)
  distinguishes rules, skills, subagents, tools and deterministic hooks. Her
  [harness engineering article](https://martinfowler.com/articles/harness-engineering.html)
  emphasizes guidance before an action and feedback after it. Yendor application:
  provide narrow Unity operations plus real build/runtime evidence.
- Anthropic's [context engineering guidance](https://www.anthropic.com/research/effective-context-engineering-for-ai-agents)
  supports selective retrieval, structured notes, compaction and scoped workers.
  This supports persistent memory but does not imply every coding project needs
  a vector database.
- Anthropic's [long-running agent report](https://www.anthropic.com/research/effective-harnesses-for-long-running-agents)
  describes initialization, incremental work, verification and useful artifacts
  for the next session. Yendor application: startup/resume and finish procedures.
- Steve Yegge's [Beads introduction](https://yegge.ai/essays/introducing-beads-a-coding-agent-memory-system/)
  advocates a dependency-aware work graph instead of decaying plan documents.
  Treat this as an experienced practitioner's report and advocacy for his tool,
  not independent comparative evidence. Current Beads storage differs from the
  original article; consult the current repository before implementation.
- Anthropic's [multi-agent research report](https://www.anthropic.com/research/multi-agent-research-system)
  describes delegation, evaluation and operational challenges. Research-agent
  results do not establish the same gains for agents editing shared Unity state.

## Memory: four different requirements

1. Authoritative project knowledge: accepted design, architecture, terminology,
   current source, asset manifests and reproducible evidence. Keep this reviewable
   in the repository and issue tracker.
2. Task memory: ready work, dependencies, ownership, blockers and discoveries.
   Use a structured task store rather than a growing STATUS.md backlog.
3. Experience memory: failed experiments, diagnosis history, playtest findings,
   preferences and lessons. Retrieve selectively across sessions.
4. Execution state: a particular run's current step, pending external task IDs,
   retries and checkpoints. Persist in the workflow engine; conversational
   memory is insufficient for reliable recovery.

### Candidate systems

| Candidate | Documented role | Proposed Yendor use |
| --- | --- | --- |
| [Beads](https://github.com/gastownhall/beads) | Persistent dependency-aware task graph and agent integrations | Pilot task memory and ready-work selection |
| [Mastra Memory](https://mastra.ai/docs/memory/overview) | History, working memory, observational memory and semantic recall | First candidate inside the existing supervisor |
| [Mem0](https://docs.mem0.ai/introduction) | Long-term memory across sessions/tools, including coding-agent integrations | Compare if cross-harness experience recall is the main need |
| [Graphiti](https://github.com/getzep/graphiti) | Temporal knowledge graph, provenance and hybrid retrieval | Consider when changing relationships and historical queries justify it |

Beads currently uses Dolt. Its documented embedded mode has a single writer;
server mode supports concurrent writers. Its JSONL export is not its source of
truth. Plan storage, backups and worktree access accordingly; do not copy old
SQLite/JSONL installation advice blindly.

Mastra resource-scoped memory can be shared across agents while separate thread
IDs keep their raw histories apart. Shared full transcripts should be an explicit
choice. Current online documentation may describe features newer than our
pinned dependencies; verify version compatibility before using them.

Start with one experience-memory implementation. A memory record should include
project, subsystem, source link, date, source revision where applicable, status
(observation/proposal/accepted/superseded), and replacement links. An agent's
inference should remain distinguishable from a user decision or verified fact.

Retrieval should return a small relevant set with sources. Re-read current code
for implementation facts. Invalidate derived indexes when source revisions
change. Keep raw logs as artifacts and consolidate useful lessons periodically.
Avoid automatically ingesting all transcripts or loading all memories at startup.

Pilot test: record approximately twenty meaningful decisions/observations,
including superseded ones. Start a fresh session and ask project questions.
Measure correct recall, source accuracy, stale-answer rate, irrelevant retrieval,
latency and prompt size. Repeat with a branch-specific fact and a changed rule.

## Communication across agent tools

[MCP Agent Mail Rust](https://github.com/Dicklesworthstone/mcp_agent_mail_rust)
provides persistent identities, inboxes, searchable threads, acknowledgments,
advisory file reservations and build slots. The original
[Python project](https://github.com/Dicklesworthstone/mcp_agent_mail) now marks
itself deprecated in favor of the Rust rewrite.

This is a useful candidate when Codex, Claude Code or other independent clients
must coordinate. Built-in subagent messaging may suffice within one harness.
Mail does not itself schedule tasks, launch agents or ensure a remote worker can
access the same server. The maintained project documents a same-machine/shared-
filesystem focus. Remote Devin execution needs an explicit adapter and delivery
design, not an assumption that every worker can join a local mailbox.

Use task-linked messages for assignment, blocking questions, contract changes,
review requests and completion. Pass evidence paths instead of full logs.
Deduplicate messages; support acknowledgments, timeouts and a visible failed
delivery state. Mail history should not be broadcast into every worker prompt.

Reservations are advisory, not filesystem isolation. Keep separate worktrees
and serialize shared Unity Editor operations even if workers reserve files.
Test expired leases, crashed workers and stale messages before trusting it for
unattended work. No external messages were sent during this research.

## Task classification and model/worker routing

Our current role aliases select a model per specialty. That is only one axis.
Add task kind, subsystem, complexity, risk, required capabilities, environment,
write scope, dependencies, evidence requirements and execution limits.

| Task | Worker requirements | Candidate route |
| --- | --- | --- |
| Label or summarize a report | Structured output; no writes | Fast model that passes classification tests |
| Bounded C# change | Reliable tools/patches; Unity checks | Coding model with scoped implementation worker |
| Cross-system design | Strong reasoning; contract analysis | Strong planning model, then separate implementation |
| Scene/prefab/material operation | Live Editor access; asset/reference awareness | Local Unity-capable worker |
| Visual inspection | Image input; actual screenshots | Vision-capable reviewer with a defined rubric |
| CI failure | Logs and build environment | QA worker; escalate after a bounded failed attempt |
| Meshy job | Asset brief; generation capability; task lifecycle | Art workflow, then import validation |
| Code review | Requirements, diff and test evidence | Separate reviewer; optionally a different model family |

Use hard capability/environment filters before model preference. Record exact
model/provider, harness, configuration, source revision and result. A classifier
can propose a route but must not silently expand tools, write scope or budget.
Start with explicit rules and a small registry; add learned routing after there
is task outcome data. Pin routing for a task initially; evaluate automatic
per-turn routing separately.

### Models, gateways and coding systems are different layers

- OpenRouter is a gateway/router. Its [Auto Router](https://openrouter.ai/docs/guides/routing/routers/auto-router)
  and [Auto Code](https://openrouter.ai/docs/guides/routing/routers/auto-code)
  select models under capability/provider constraints. Model routing and provider
  routing are different. Auto routing does not supply Unity tooling or our task
  dependency graph.
- DeepSeek and Claude model APIs are candidate backends. DeepSeek documents
  [tool calling](https://api-docs.deepseek.com/guides/tool_calls/), but individual
  model/provider combinations still need schema, tool-loop and failure tests.
- [Devin](https://docs.devin.ai/api-reference/overview) exposes a coding system
  through session-oriented APIs, not a drop-in chat model. Its adapter would
  submit work, poll/receive status, retrieve artifacts and bring changes through
  our review and Unity validation. Do not assume Unity is installed/licensed in
  its execution environment.
- If AGY means Google Antigravity, its [agent manager](https://antigravity.google/docs/cli/commands/agents/)
  is another harness to evaluate, rather than a model alias. The intended meaning
  of AGY has not been confirmed.
- Codex native delegation and our custom Mastra studio are separate execution
  mechanisms. Configuring one does not automatically extend the other to every
  provider. Subscription/API entitlements and billing need separate verification.

Evaluate candidate configurations on representative Yendor tasks: one bounded
combat fix, one status interaction, one scene/reference problem, one CI failure,
one review with a seeded bug, and one visual comparison. Measure correctness,
test evidence, human rework, time, tool failures and total usage. No model winner
or cost savings can be established without these runs.

## Additional tools worth evaluating

| Tool / pattern | Capability | Assessment |
| --- | --- | --- |
| [Serena](https://github.com/oraios/serena) | Symbol navigation, references and editing through language-server tooling; C# support | Pilot on generated Unity C# solution; does not replace Unity asset/GUID inspection |
| [Mastra Workflows](https://mastra.ai/docs/workflows/overview) | Explicit steps, workflow state, snapshots and suspend/resume | Extend existing studio before migrating orchestration frameworks |
| [LangGraph persistence](https://docs.langchain.com/oss/javascript/langgraph/persistence) | Checkpointed workflow state | Alternative if Mastra proves insufficient; not a second simultaneous coordinator |
| [Gas City](https://github.com/gastownhall/gascity) | Multi-agent orchestration builder with configurable runtimes | Comparison candidate for multi-harness operations; requires a controlled trial |
| [Langfuse](https://langfuse.com/docs/observability/overview) | Traces, usage, latency and evaluation signals | Instrument custom studio; distinguish missing telemetry from a successful run |
| [ACP](https://agentclientprotocol.com/get-started/introduction) | Client/editor communication with coding agents | Adapter option when both endpoints support it |
| [A2A](https://a2a-protocol.org/latest/) | Interoperation between agent systems | Later interoperability option, not a substitute for shared task state |
| [AltTester](https://alttester.com/docs/sdk/latest/) | Instrumented Unity object/UI test automation | Evaluate for player-level interaction tests when custom smoke checks become limiting |

Documented product availability is not verified compatibility with our Unity
version, OS, installed package versions or accounts. Do not add every available
MCP server to every worker. Expose only the tools needed for its role.

## Unity-specific controls

These are proposed project workflows rather than claims that one product provides
all of them:

- Give every worker a source revision, task ID and owned worktree. Do not share
  a mutable Unity Library directory across concurrent project copies.
- Treat the live Editor/project connection as a leased resource. Protect scene,
  prefab, project settings and shared contracts with ownership rules.
- Export searchable scene/prefab summaries and GUID/reference maps through
  Editor tooling. Text/code semantic search alone cannot explain serialized
  Unity state.
- Add asset checks for missing scripts/materials, colliders, scale and geometry
  budgets. Preserve .meta identity across asset moves.
- Keep seed regression sets, small test arenas, configurable experiment presets,
  comparable camera captures and player diagnostic bundles.
- Require actual compilation/runtime evidence and playable review for appropriate
  changes. A mailbox acknowledgment or confident reviewer is not a test pass.
- Persist Meshy task IDs before advancing the workflow. Recover external jobs
  by task ID after interruption; do not blindly retry a paid submission after an
  ambiguous response. Apply the same lifecycle discipline to other external work.

## Proposed early pilot and adoption gates

1. Define startup/finish instructions, authoritative records, task schema and
   worker/result schema. Verify fresh sessions recover the current state.
2. Trial Beads on one milestone, or implement equivalent dependency/claim state
   in the existing issue workflow. Select one task authority. If GitHub presents
   the work to the user while Beads executes it, define field ownership and
   idempotent mapping; do not create two independent backlogs.
3. Trial Agent Mail between two local harnesses, including restart, acknowledgment,
   reservation expiry and one shared-contract handoff. No parallel Unity writes.
4. Compare existing Mastra memory with one cross-harness alternative only if
   needed. Run the fresh-session recall and stale-memory tests.
5. Evaluate two or three model/harness configurations on representative tasks.
   Use results to populate the capability registry and routing rules.
6. Add resumable task states: proposed, ready, claimed, implementing, validating,
   reviewing, ready-to-integrate, complete; also blocked/failed/cancelled with
   explicit recovery. Persist external job IDs and test artifact references.
7. Integrate CI, player reports, Unity AI and Meshy into this flow. Expand worker
   concurrency only after ownership and recovery tests pass.

Adopt tools when they demonstrably improve resume accuracy, useful work delivered,
diagnosis or validation. Record setup/maintenance overhead as well as benefits.
Architecture decisions and installation require a subsequent implementation task;
this document makes no changes to the accepted ADR.
