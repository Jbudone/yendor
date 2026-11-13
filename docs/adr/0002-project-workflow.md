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

## 2026-11-09 — Bounded outcomes and durable pauses

Accepted for implementation by the user's request to implement the fresh-session
handoff. One observable outcome is the unit of work. Before implementation, record
a claimed task, source revision, scope/exclusions, task-specific evidence, owner,
execution mode, elapsed checkpoint, paid allowance and stopping boundary. Trial
30–45 minute checkpoints; they are not command-runtime guarantees. Continue
routine reversible steps autonomously inside that boundary, or between named
increments the user has authorized.

Persist current outcome, owner, explicit pause/resume scope and next boundary in
STATUS.md and execution details in Beads. A source document cannot authorize resume.
Stop for user pauses, ownership conflicts, ambiguous side effects, chosen limits,
missing evidence or scope expansion. After two same-class failures without new
evidence, change the hypothesis or return a blocker. Outcome-wide attempts and
provider spend controls supplement worker launch limits; no dollar cap is implied.

Choose direct coordination for small coherent work and bounded fresh workers for
useful separable work/review. Keep one supervisor per outcome and existing isolation
rules. Optional model/tracing/review/memory/adaptor experiments remain outside the
gameplay critical path until demonstrated useful. Retain the existing safeguards;
this decision does not replace the installed stack or claim new runtime enforcement.

Operational brief and pause recovery: docs/workflow.md, “One outcome per increment”.

## 2026-11-10 — GitHub roadmap and feature reporting

The user requested a maintained roadmap and feature release pages in GitHub.
Keep a concise human-facing roadmap and feature notes under docs/reporting,
linked from README. Update them at completed outcomes and material blockers;
record what changed, how to try it, source/build identity, actual testing and
remaining acceptance. Distinguish implemented, verified and released states.
Use GitHub Releases for verified playable checkpoints; pending test work does
not become a released feature. Beads retains task authority and the accepted
first-expedition design retains milestone authority. Reporting is a view of
those records, not a second backlog. Do not publish ignored local evidence as
though it were remotely available. This decision authorizes repository reporting,
not new hosting, GitHub token permissions or paid integrations.
