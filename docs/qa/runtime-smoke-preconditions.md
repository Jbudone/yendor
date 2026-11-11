# Runtime smoke preconditions — 2026-11-11

Task: `yen-0r8` / [issue #16](https://github.com/Jbudone/yendor/issues/16).
The failure was in the smoke fixture's assumption, not the accepted water rule.

## Reproduction and fix

Hosted run `42954712829` at `14afb5c` failed “Dry ember hit did not ignite.”
A fresh local instrumented baseline reproduced it in run
`2026-11-11T04-05-57-048Z-901c3418`: seed 43117 selected the healthy revenant at
(-5, 1, 14), already wet from the rain pool. Unsorted object discovery selected
a wet target for an assertion that assumed it was dry. The instrumentation patch
is retained locally and its file hash matches that run's source manifest.

The fixture now orders generated actors by their distinct starting positions (z, x, y),
prefers an ambient wet actor to exercise this regression, and explicitly
establishes dry then wet conditions before synchronous elemental assertions.
It logs the seed, initial enemy positions and target's ambient status. No frame
elapses between setup and assertions, so surfaces cannot reapply wet mid-check.
Health, water, world generation and accepted interaction rules are unchanged.

## First local acceptance (hierarchy-order iteration)

- Fixed smoke passed: `2026-11-11T04-07-06-556Z-f9849cc0`.
  It selected the same wet revenant, then verified ignition, doubled wet Storm
  damage, one-time death counting, relic pickup and extraction.
- All ten Unity tests passed with XML:
  `2026-11-11T04-07-57-519Z-ab84ca33`.
- Both runs verified source hash
  `98eb4a0ce471e1a456f3db1c51ae26e466cd5b968d058e673497ae010ea8f294`.
  This QA document was added afterward; manifests retain exact file identities.
- All seven enemy starting positions match between the instrumented baseline and
  fixed smoke for seed 43117. This is one seed check, not broad procedural proof.

## Hosted verification and limitations

The final full Unity CI successor is recorded below. The earlier failed run
and intermediate passing iteration remain visible for their exact sources.
Headless tests do not establish rendered quality, manual feel or hardware timing.
The known UnityEditor SearchDatabase startup exception remains in local smoke
logs; gameplay assertions passed and the process exited successfully.

Full local command logs, reproduction patch and structured evidence are under
`artifacts/pilots/yen-0r8/`; full run logs/XML are in
`.worktrees/yen-0r8/artifacts/runs/<run-id>/`. No paid calls or gameplay redesign.

## Cross-runtime ordering finding

The first fix passed full CI run `42957389692` at `0f313bb`, including packaged
player smoke. A stronger evidence comparison found the same seven positions in
a different player log order: hierarchy-based ordering had not normalized the
packaged player's root objects. The follow-up uses starting coordinates instead.
The earlier passing workflow is retained as evidence for its exact source, not
as proof of the follow-up. The final change was verified in the successor recorded below.

## Final coordinate-order local acceptance

The follow-up passed fresh-copy smoke in `2026-11-11T04-13-11-242Z-1a35f746`
and all ten Unity tests in `2026-11-11T04-13-36-732Z-56128bd4`, on source hash
`9892680939d26d1032210f92f2290c056f6cb75d84c35e47084b32a9ac53ef5d`.
The only changed game file remains RuntimeSmoke.cs; no metadata identities changed.
Full hosted verification of this final ordering passed as recorded below.

## Final hosted acceptance

[Full Unity CI 42957926313](https://github.com/Jbudone/yendor/actions/runs/42957926313)
passed at `696edb2e9dafa5ace5bcb9b73e65b1eda8f2e9c2`: validation, all ten tests,
Editor smoke, asset audit, Linux build, packaged-player smoke and artifact upload.
Every mode verified source hash
`dae4cf7745e9799bf5b9228b007348164fc71fa39c31df6ef4afcb73c0af2aae`.
Build GUID: `58e54fd84e464cd8b5b701f3b4fd047a`; player bundle hash:
`d6f3ba6cf7338190ff6fbfdf1fabfae181498f709c671cb1cb1b5e61207ff298`.

The final local and hosted manifests have identical game file hashes. The ordered
seed-43117 signature, containing all seven enemy positions, matches exactly in
local Editor, hosted Editor and packaged player logs. Both hosted smoke modes
selected the naturally wet enemy at (-5, 1, 14), normalized it for the dry check,
then passed wet damage, death-once and extraction checks. This remains one seed.

Hosted logs/XML/manifests are linked from the run and retained locally under
`artifacts/pilots/yen-0r8/ci-42957926313/`. The first green iteration and its
ordering limitation remain under `ci-42957389692/`. Later QA/reporting updates
change documentation hashes; the exact tested source and game file hashes above
remain the acceptance identities. No manual feel or hardware acceptance.
