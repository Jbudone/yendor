# 0002 — Durable project workflow with scoped execution

Status: accepted implementation direction, authorized by the user on 2026-11-08.

Use repository guidance and source-linked decisions for session continuity.
Beads is the local task authority. GitHub, when connected, presents selected
user-facing work; synchronization must retain explicit field ownership and IDs.
Do not create a second independent task backlog.

Use project-local Agent Mail for communication among independent coding clients.
The coordinator assigns task-linked, bounded work; agents fetch relevant mail
instead of sharing all transcripts. A task's Mastra thread persists history in
local LibSQL. Curated project knowledge is versioned in docs/memory/records.json.
Semantic/vector or observational memory requiring extra model calls is disabled
until configured and evaluated. Agent observations do not become accepted policy.

Role/model routing is explicit and capability-aware. Model aliases and actual
serving deployments are distinct; unknown deployment identity stays unknown.
Keep single-worker and supervisor routes available for comparison. External
coding systems require their own verified adapters before being enabled.

One writer per checkout, separate worktrees for independent implementations,
source-hash checks on writes and cross-process serialization of Unity commands.
Locks are not automatically stolen; recovery requires a stopped old process.
Scoped writes remain trusted local tooling, not OS security isolation.

Run evidence includes source identity, unique logs, exit/result checks and build
bundle hashes. Validators do not repair required inputs. External paid jobs
record intent and remain ambiguous after lost responses until reconciled.
Unity child processes receive a narrow environment without model/Meshy keys.

Use fast local hooks and CI; heavy Unity checks run in a licensed runner. AI
review stays advisory until calibrated. Visual/feel acceptance requires captures
and human observation. Generated command-sequence regression tests are piloted
on health/status rules before wider adoption.

Continue to use Unity AI and Meshy as authoring tools, separate from runtime.
Deferred/rejected research candidates remain recorded, not automatically adopted.
