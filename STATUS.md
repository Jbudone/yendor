# Current project state

The Copper Wild greybox exists. Current milestone: finish foundation verification,
then improve combat feel. Read `CONTEXT.md` for direction and
`docs/design/first-expedition.md` for milestone gates.

The local workflow is installed and verified with synthetic model/real Unity checks. `docs/workflow-setup-status.md` records
verified integrations and account/environment requirements. Beads is the task
authority; query it instead of copying the full backlog here.

Prior verified gameplay checks are recorded in `docs/qa/foundation.md`; they are
historical evidence, not proof for changed source. A rendered/manual feel pass
is still required. Live model/Meshy/tracing credentials remain account gates.
The licensed CI runner is registered; final CI evidence is recorded in the setup
status. Research is indexed by `docs/workflow-research-ledger.md`.

## Current coordination boundary

2026-11-11: coordinator Codex, task `yen-lmb`, direct execution; start 03:41 UTC,
checkpoint 04:11 UTC, paid calls zero. Published source/test commit `14afb5c` in
[PR #13](https://github.com/Jbudone/yendor/pull/13); illustrated reporting and
PR-Agent readiness in [PR #17](https://github.com/Jbudone/yendor/pull/17).
PR #14 was superseded, not merged. No release or merge was performed.

[Interactive roadmap](https://github.com/Jbudone/yendor/issues/15) is pinned and
supports comments/reactions. The [latest illustrated report](docs/reporting/updates/2026-11-11.md)
shows an inspected Linux player capture and evidence. Local ten Unity tests,
smoke and build passed. Hosted validation/ten tests passed but smoke failed with
“Dry ember hit did not ignite”; investigate `yen-0r8` / issue #16, no blind retry.
Reporting Node checks and the no-cost reviewer readiness check passed.

PR-Agent installed; model reviews remain disabled awaiting selected model and
provider-enforced spending cap/key. Do not copy general local credentials or
start paid calls implicitly. No explicit user pause. Next boundary: configure and
verify one capped review when clarified, resolve CI smoke before integration,
then manual controls/camera acceptance. Human feel and hardware performance
remain unverified; captures used Xvfb/Mesa llvmpipe.


2026-11-11 03:38 UTC: `yen-pmd` complete locally; coordinator Codex, direct mode.
Commit `14afb5c` contains four Health lifecycle tests and their QA record; runtime
unchanged. Fresh-copy Unity tests passed 10/10 and runtime smoke passed on source
hash `51ba6426…`; syntax/11 Node tests and staged secret scan passed. Root cause
of npm ENOENT: local npm sandbox hides the home-based Unity executable; direct
project Node CLI succeeds, sandbox unchanged. Known Editor SearchDatabase startup
exception remains in smoke log; no rendered/manual feel/build acceptance.
Evidence: docs/qa/timed-health-lifecycle.md and
artifacts/pilots/yen-pmd/resume-2026-11-11/. Reporting pages refreshed locally.
Started 03:34 UTC; completed before 04:04 checkpoint; paid calls zero. Prior diff
preserved; no merge or release. No explicit pause or new running verification job.
Next boundary: publish/review the isolated verification commit and refresh CI;
then foundation manual controls/camera acceptance (`yen-kk5`). PR #14 reporting
changes need synchronization with these local updates before integration.

2026-11-10: `yen-ei3` complete, reporting pages published for review in PR #14
(stack on unmerged #13), source `d1f435e`. Entry points: docs/reporting/roadmap.md
and docs/reporting/README.md; coordinator updates at outcome/blocker boundaries.
Two fresh agents authored pages and independently assessed Unity access/reviewed
claims. Secret scans, syntax/11 Node tests, 27 local link targets and ADR memory
hashes passed. Published isolated branch; exact reporting files copied to primary,
prior diff preserved. No merge, release tag, Unity launch or paid call. Read-only
Unity assessment: executable/interpreter/libraries present, ENOENT cause unresolved;
no Unity connector exposed, interactive/rendered acceptance unverified. Evidence:
artifacts/pilots/yen-ei3/. No explicit pause. Next boundary: review reporting PR;
`yen-pmd` remains incomplete, with launch diagnosis as next bounded gameplay action.

2026-11-10 05:32 UTC: `yen-pmd` saved incomplete at checkpoint; coordinator Codex.
Two fresh native workers implemented/reviewed four actual-frame Health tests in
isolated worktrees. Worker commit `599bf7d`; exact files copied to primary as
uncommitted additions; runtime unchanged. Syntax/11 Node tests pass. Unity test
launch failed with `spawn .../Editor/Unity ENOENT` despite executable presence;
no compilation, test XML or smoke evidence. One attempt; no retry. Evidence:
`artifacts/pilots/yen-pmd/handoff.md` and `verification.json`. Start 04:38 UTC;
05:08 target exceeded (launch 05:30:58); stopped at discovery and saved state.
Paid calls zero; no explicit user pause. No new running jobs; existing background
processes untouched. Next boundary: diagnose launch environment, then fresh-copy
Unity tests and smoke; task remains incomplete. Prior documentation diff preserved.

2026-11-10: user resumed orchestration with bounded delegation. `yen-m27` complete:
PR #13 Node failure was HTTP 504 downloading pinned Gitleaks; one failed-job retry
passed on `b3dc709` (run 42579814055, attempt 2). Local syntax/11 Node tests pass.
Two fresh native agents diagnosed/reviewed; coordinator Codex retained sole source
ownership. Runner online/idle and required `node` protection verified read-only.
Evidence: `artifacts/pilots/yen-m27/`. Paid calls zero; no new Unity run or merge.
Started 03:53 UTC; completed before 04:23 checkpoint. No explicit pause active.
Next boundary: select a separate outcome; foundation manual feel/rendered evidence
remains outstanding. Prior documentation diff below is preserved and uncommitted.

2026-11-09: the user resumed implementation of the fresh-session handoff. Selected
increment: `yen-l2x`, durable outcome/pause/checkpoint rules; coordinator: Codex
(Beads claim uses local identity Josh Braendel). Starting source: `b3dc709`.
Execution: direct coordinator, documentation only; paid allowance zero.
State: documentation increment complete; `npm run check` passed (11 tests plus
syntax), diff whitespace and ADR memory hashes checked, recall returned a non-stale
record. Evidence: `artifacts/pilots/yen-l2x/verification.json`. Next boundary:
review this diff; select a separate outcome before CI integration, execution-guard
code or live pilots. This is a completed boundary, not a new user pause.
The earlier blanket pause is superseded for this increment; no services or existing
background jobs are changed. See docs/workflow.md for subsequent outcome briefs.

PR #13 remains unmerged. Node run 42579814055 is now green after the transient
download failure retry. Hosted checks cover committed source, not this uncommitted
documentation diff. Historical Unity evidence remains tied to `6700bfe`.

Normal fresh-session request: “Pick up Yendor” or “Work on task yen-…”.
