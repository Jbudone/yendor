# Current project state

Yendor is a Unity third-person expedition shooter. Accepted direction:
`CONTEXT.md` and `docs/design/first-expedition.md`. Beads owns execution detail.
Query `node tools/workflow.mjs task ready --json`.

## Current outcome and authorization

2026-11-12: user reports the foundation feels unchanged and explicitly authorizes
several substantial gameplay iterations before the next playtest, with each chunk
sent to a fresh subagent, one at a time. This supersedes the old manual-foundation
prerequisite for this batch. Human feel, rendered performance and fresh-player
acceptance remain unpassed; no evidence is invented.

Coordinator Codex uses native sequential delegation, isolated worker worktrees,
and serial review/integration. External paid-call allowance zero. No explicit
pause. Continue automatically through these named increments after verified results:

1. `yen-gh7`: three enemy roles, attack telegraphs, combat feedback and cooldown HUD.
2. `yen-7a1`: deterministic pickups and tactical run-modifier choices.
3. `yen-kn2`: rewarded optional route and Starheart return pressure.

Integration branch `gameplay/expedition-batch` starts at master `12cc7c9` in
`.worktrees/gameplay-batch`. Combat integrated at `4023f12` (worker `9142a4d`), with
fourteen Unity tests, validation and smoke passed; coordinator log/XML/game-hash
audit saved in `artifacts/pilots/yen-gh7/coordinator-audit.json`.
Pickups integrated at `d9b15cc` (worker `efcef8d`), with
twenty Unity tests, validation and smoke passed; coordinator audited logs/XML/game
hashes. Exploration worker `56588bc` integrated; all three fresh workers are finished.
Twenty-six tests, validation and smoke pass; actual scene replay, physical route
traversal and persistent F8 are covered. Coordinator audited logs/XML/game bytes.
Asset audit, Linux build and packaged-player smoke pass. Exact player GUID
`f7476776753c49c09ed7acb96820a3e6` is preserved and rehashed at primary
`artifacts/playtest-latest/`. Rendered normal-input captures confirm Tempest,
overlook Heartwood (125 life) and Return phase/three hunters. Build source
`e44dca4`; later report/images only. Publication and source-safe Editor sync
active; third checkpoint remains 04:29 UTC.
Stop on failed acceptance evidence, ownership conflict, ambiguous paid side effect,
scope expansion, per-increment checkpoint or user pause; preserve running jobs.

## Established baseline and evidence

Combined foundation PR #19 merged to master `12cc7c9` on 2026-11-12.
[Node](https://github.com/Jbudone/yendor/actions/runs/43145761860) and
[full Unity verification](https://github.com/Jbudone/yendor/actions/runs/43145762111)
passed, including Linux build and packaged-player smoke. Integration evidence:
`artifacts/pilots/yen-7mm/`; prior playtest packet:
`docs/playtests/foundation-controls.md`. `yen-kk5` still awaits human evidence;
hardware timing and three-fresh-player diagnostic remain separate.

Primary checkout stays on `workflow/finalize-verification`; pre-existing dirty
documents, open Editor and other worktrees are preserved. Playtest delivery must
identify the NEW build explicitly. Prior STATUS saved locally as
`artifacts/status-before-gameplay-batch.md`; historical details remain in Beads.
