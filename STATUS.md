# Current project state

Yendor's Copper Wild greybox exists. Finish foundation acceptance, then improve
combat feel. Direction: `CONTEXT.md` and `docs/design/first-expedition.md`.
Beads owns execution state; query `node tools/workflow.mjs task ready --json`.

## Current coordination boundary

2026-11-11: `yen-0r8` complete; coordinator Codex, direct execution. Started
04:05 UTC, completed 04:20 before the 04:35 checkpoint. Paid calls zero.
No explicit user pause. No merge, release or new running Unity task job.

The smoke fixture selected an already-wet enemy for a dry assertion. It now
sets dry/wet preconditions explicitly and uses starting-position ordering.
Accepted gameplay, seed generation and asset metadata are unchanged.

[Full Unity CI](https://github.com/Jbudone/yendor/actions/runs/42957926313)
passed at `696edb2`: validation, ten tests, Editor smoke, audit, Linux build,
packaged-player smoke and artifact upload. Ordered seed-43117 positions match
local Editor, hosted Editor and player. QA/status-only commits follow that code.
Source is published in [PR #13](https://github.com/Jbudone/yendor/pull/13).
Evidence: `docs/qa/runtime-smoke-preconditions.md`, `artifacts/pilots/yen-0r8/`.
The failed baseline and intermediate hierarchy-order limitation remain recorded.

The [illustrated roadmap](https://github.com/Jbudone/yendor/issues/15) and latest
report `docs/reporting/updates/2026-11-11-smoke.md` are updated in
[PR #17](https://github.com/Jbudone/yendor/pull/17), reporting commit `cfa6d26`.
Issue #16 is closed. Node/readiness checks pass; live PR-Agent reviews remain
disabled pending a selected model and provider-capped key (`yen-wco` / issue #18).

Next boundary: review source/report PRs, human controls/camera acceptance
(`yen-kk5` / #4), and hardware/readability baseline (`yen-g8n` / #5).
Headless assertions do not establish manual feel or hardware performance.
Known Editor SearchDatabase startup exception remains recorded.

Prior primary documentation edits remain uncommitted and preserved; reporting
files are refreshed from their isolated branch. Earlier coordination history is
retained in Beads and `git show ebd0ecc:STATUS.md`. Existing workers were untouched.
Workflow/integration details: `docs/workflow.md`, `docs/workflow-setup-status.md`.

Normal fresh-session request: “Pick up Yendor” or “Work on task yen-…”.
