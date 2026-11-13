# Working on Yendor

Open this checkout and say “Pick up Yendor” or name a task. Codex reads AGENTS.md;
Claude Code's CLAUDE.md points to the same agreement. The coordinator reads
STATUS.md and CONTEXT.md, then retrieves only relevant tasks, decisions and mail.
Start a fresh thread for unrelated work. Finish connected work with a durable
handoff so no conversation is the only copy of a decision.

## Project records

Beads is the task authority. GitHub issues preserve discussion and direction;
config/github-task-map.json maps them to task IDs. Studio runs with --task-id fetch a bounded brief plus the latest three linked
issue comments, preserving their sources and reporting conflicts. The initial publisher creates
briefs once and never overwrites later human discussion. It does not pretend to
provide bidirectional synchronization. Review issue updates into the Beads brief
before implementation. A task is not automatically closed by an agent response.

Accepted design lives in design docs/ADRs. Source-linked project notes live in
docs/memory/records.json; proposal, observation, accepted and superseded are
explicit. Memory is retrieved in bounded sets. Actual task conversations persist
in .workflow/studio.sqlite with a 6,000-token history budget per task thread.
Shared raw transcripts, paid embedding and automatic observational-memory calls
are disabled. Re-read current code for implementation facts; a stale hash or
conflicting source calls for review, not promotion by repetition.

```sh
npm start
npm run workflow -- task ready --json
npm run workflow -- task show yen-ID --json
npm run workflow -- task update yen-ID --claim
npm run workflow -- route "Fix a wet + Storm regression"
npm run workflow -- recall "seed replay"
npm run workflow -- handoff yen-ID /path/to/summary.md
```

Handoffs should explain current source, completed changes, commands actually run,
artifact paths, known failures, unresolved decisions and the next concrete step.
STATUS.md stays short; it does not duplicate the backlog.

## One outcome per increment

Before implementing, claim the Beads task and put this brief in its description
or notes. Use task-specific acceptance rather than the seeded generic paragraph.

```text
Task / coordinator:
Starting revision / relevant dependencies:
Observable outcome:
Owned paths / excluded work:
Acceptance evidence:
Execution mode / reason:
Started at / elapsed checkpoint (trial: 30–45 minutes):
Paid-call allowance / provider spend control (zero by default):
Attempts and failures across launches:
Stopping boundary / authorized continuation:
```

Use the coordinator directly for small coherent work. Choose fresh bounded workers
for separable implementation or independent review when useful; retain the worker
brief, separate worktrees and serialized Unity rules below. Select one supervisor
path per outcome. Native coordinator, independent mail clients and Mastra have
different enforcement boundaries; studio tools do not constrain arbitrary shell
writes. Worker count is not an acceptance measure.

At the elapsed checkpoint, save a reviewable result or blocker with evidence and
report it before expanding scope. A long running Unity command may exceed the
checkpoint: record its run/process identity and safe next step; do not launch a
second command or stop an existing background job without authorization. Routine
reversible steps inside the outcome continue autonomously. Several named increments
can be authorized up front, with continuation after each successful result.

Stop on a user pause, unresolved ownership/source conflict, ambiguous paid side
effect, elapsed/spend boundary, missing acceptance evidence or scope expansion.
After two same-class failures without new evidence, change the hypothesis or
return a blocker. Count attempts across worker/coordinator launches. Step limits
are not currency caps; use provider controls before unattended paid work. Preserve
ambiguous write/payment records and reconcile before retrying.

On explicit pause, update Beads notes and STATUS.md with owner, outcome, source/diff,
running jobs, evidence, pause reason and next boundary. Keep the existing task;
do not close it or invent a second backlog. An explicit user resume supersedes the
pause only for its stated scope. Record that scope in both places. At a completed
boundary, record the result and next action without inventing a user pause.
Handoffs should fit roughly a page with artifact links; fresh sessions belong at
stable boundaries rather than after every dependent step.

Keep routing/tracing experiments, advisory AI review, memory calibration and new
adapters optional until a real bounded task proves their value. Gameplay work uses
source and bounded briefs directly; mail serves actual independent clients. Keep
Git/PRs, Beads, secret scans, source/build evidence, Unity gates, source-aware writes,
locks and paid ambiguity handling. No stack replacement is implied by this policy.

Use `npm run workflow -- status` for credential-presence reporting: npm loads the
ignored `.env`; bare `node tools/workflow.mjs status` sees only the inherited
environment and may report false negatives. Presence does not verify a provider,
trace delivery or permission to spend. Never print secret values.

## Installation and clients

Node 22.18+ is required. Linux x64 binaries are pinned in config/toolchain.json.
The first Agent Mail archive was checked against its upstream signed manifest;
bootstrap verifies the pinned digest before extracting named binaries.

```sh
npm ci
npm run bootstrap
# Fresh clone: restore the shared task graph; never reinitialize a populated DB:
.tools/bin/bd bootstrap --yes
npm run tasks:seed
npm run hooks:install
git lfs install --local --skip-repo
npm start
```

The current checkout has Codex's yendor-mail MCP entry registered, and an ignored
.mcp.json for Claude with mail and Unity CLI. Reload/reconnect clients after
configuration changes. Mail's stdio adapter reads a private local token, limits
the exposed tool set and starts/reuses the service. It binds only loopback port
18765, leaving the unrelated service on 8765 alone.

For another clone, generate client configuration with
`node tools/configure-clients.mjs`; it writes local .mcp.json and prints the
Codex command for the clone's absolute path. It does not overwrite global config.

## Models, routing and tracing

The studio uses LiteLLM aliases; the gateway routes these to OpenRouter.
Candidate models in local .env were checked against OpenRouter's tools-capable
catalog but are not project benchmark winners. Edit upstream IDs as needed.
The role/profile registry is config/workers.json. Classification is advisory;
file scopes, worker environment and actual evidence are separate controls.
Visual tasks are refused by the text-only studio until a proper visual worker is
available. Devin/Antigravity adapters and other comparison candidates stay disabled.

```sh
uv venv .tools/litellm --python /usr/bin/python3
uv pip sync --python .tools/litellm/bin/python infra/litellm/requirements.lock
# Fill OPENROUTER_API_KEY in ignored .env; local gateway keys are already generated.
npm run gateway:start
npm run gateway
npm run agents -- --task-id yen-ID --dry-run "A bounded task"
npm run agents -- --task-id yen-ID --single combat "A bounded task"
npm run agents -- --task-id yen-ID "A bounded cross-role task"
```

Run one supervisor writer per checkout. Use the same task ID to resume its
history; single-worker threads and supervisor threads are separate. Failed runs
preserve manifests and write intents. Successful model execution ends at
review-required, not task completion. Serving deployment identity is reported
where available and marked unknown otherwise; an alias is not proof of a fixed
upstream model. No hard currency budget is claimed by step/time limits. Configure
provider spend controls before unattended runs.

For Langfuse Cloud, set LANGFUSE_BASE_URL to the actual project region, add its
public/secret keys, then set ENABLE_LANGFUSE=true. The native exporter flushes on
exit. Delivery is not verified until a real trace is fetched and audited; imports
and the mock model pilot do not establish that. Do not paste keys into chat.

## Agent communication and ownership

```sh
npm run mail -- status
npm run mail -- tools
npm run pilot:mail
npm run workflow -- worktree yen-ID
```

Use ensure_project with the canonical checkout path, register a generated agent
identity and put the Beads ID in thread_id. Send only assignments, blocking
questions, contract changes, review requests and results. Fetch relevant inbox
messages and acknowledge requests. Keep full logs in artifacts and send links.
The local pilot verified two independent clients, message delivery,
acknowledgment, reservation conflict and release. Remote workers need an adapter;
a local mailbox is not automatically reachable by a remote coding service.

Worktree creation defaults inside ignored .worktrees/. Use the primary checkout
for Beads and mail; each Unity copy owns its own Library. Never share a mutable
Library. Reservations are advisory. Studio/source-write and Unity locks prevent
cooperating local processes from overlapping. Locks are not stolen by timeouts:
inspect owner.json and confirm the old process has stopped before manual recovery.
After a lease conflict re-read source rather than retrying an old whole-file write.

## Verification and evidence

```sh
npm run check
npm run unity:validate
npm run unity:test
npm run unity:smoke
npm run unity:audit
npm run unity:build
npm run player:smoke -- <build-run-id>
```

Batch mode needs an available project and licensed Editor. When the main project
is locked, use `npm run unity:prepare` and put its printed path in the local
UNITY_PROJECT_PATH. That creates a source copy with a separate Library. The
runner verifies its Assets/Packages/ProjectSettings hashes match the main source
and rejects modifications during checking. Refresh the copy after source changes.
It does not close an Editor or delete another process's lock file.

Each command writes a unique artifacts/runs/<id>/ manifest, command and log.
Compilation/test-result checks must pass for that source identity. Builds preserve
the exact player bundle plus digest and build GUID. Validation never recreates a
missing scene. A pre-existing/wrong log or a zero-test suite is not a pass.
Unity receives an environment allowlist without provider/Meshy/tracing keys;
this remains trusted local tooling, not OS isolation.

The health/status pilot runs 500 action histories for each of three seeds and
reduces failures. Three faulty adapters test the independent oracle. They do not
claim full source mutation testing, burn/expiry coverage, exact replay or feel.
Timed status tests remain a tracked follow-up. Asset audit checks saved prefabs
for missing scripts/materials; runtime-generated geometry still needs captures.

F8 exports a local diagnostic report under Application.persistentDataPath/Reports:
build GUID, seed, player/run state, machine details and recent events. A graphics
session also requests a screenshot. The runtime smoke checks report export;
actual screenshot/readability review needs a graphics session.
Use docs/playtests/first-expedition.md and docs/experiments/template.md to compare
observed behavior against an explicit hypothesis. Preserve tuned presets and
failed spatial arrangements as reviewed fixtures when the corresponding task is
implemented.

## Git, CI and publication

Gitleaks v8.30.1 scans staged changes before commit and all history before push.
The hooks fail if its binary is missing. Node checks run before committing;
YENDOR_PREPUSH_UNITY=1 adds validation and tests before pushing. LFS handles FBX,
Blend, PSD, textures, audio, video and EXR. Keep source exports and provenance; avoid uploading local
state, keys, model transcripts or generated player builds as ordinary Git source.

Public repository: https://github.com/Jbudone/yendor. CI runs Node checks and
full-history Gitleaks. A separate Unity workflow uses the dedicated licensed runner
yendor-unity-linux, labelled yendor-unity, enabled through YENDOR_UNITY_CI and
UNITY_EDITOR_PATH. It runs main/master pushes and manual dispatch, not PR events.
Check/restart the local listener with `node tools/runner.mjs status` and
`node tools/runner.mjs start`; it must be online to accept jobs. AI review is separately opt-in with
YENDOR_AI_REVIEW=true and an OPENROUTER_API_KEY repository secret. It posts advisory
review findings and does not edit, approve or merge the PR. No paid AI review is
enabled by default. Require the actual passing check names after the first CI run.

## Recovery and backup

```sh
npm run workflow -- recover
npm run workflow -- backup
.tools/bin/bd backup status
```

Local native Beads backup is configured at .workflow/beads-backup and was synced.
The task graph is also pushed to the public repository's Dolt data ref;
use `bd dolt pull`/`bd dolt push` to coordinate changes across machines.
The local backup is a same-machine recovery copy, not independent disaster recovery. Choose
an external destination and rehearse restore in a disposable database before
claiming that protection. .beads/issues.jsonl export is not a native backup.

A file write can succeed before its audit append fails. Inspect base source
identity, current source and edit intents before retrying. Meshy records intent
before submission; submitting/ambiguous records require dashboard/provider
reconciliation before another paid request. Native task IDs are preserved after
successful responses, but a lost response cannot guarantee exactly-once execution.
Never restore an older database over the live task authority as a test.
