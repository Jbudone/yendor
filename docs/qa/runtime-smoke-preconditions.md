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

The fixture now orders generated actors by their scene-root sibling order,
prefers an ambient wet actor to exercise this regression, and explicitly
establishes dry then wet conditions before synchronous elemental assertions.
It logs the seed, initial enemy positions and target's ambient status. No frame
elapses between setup and assertions, so surfaces cannot reapply wet mid-check.
Health, water, world generation and accepted interaction rules are unchanged.

## Local acceptance

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

A fresh full Unity CI run is required for integration. Do not replace the earlier
failed run with a passing narrative; record its successor explicitly.
Headless tests do not establish rendered quality, manual feel or hardware timing.
The known UnityEditor SearchDatabase startup exception remains in local smoke
logs; gameplay assertions passed and the process exited successfully.

Full local command logs, reproduction patch and structured evidence are under
`artifacts/pilots/yen-0r8/`; full run logs/XML are in
`.worktrees/yen-0r8/artifacts/runs/<run-id>/`. No paid calls or gameplay redesign.
