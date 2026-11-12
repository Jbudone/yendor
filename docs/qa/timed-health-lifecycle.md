# Timed Health lifecycle verification — 2026-11-11

Task: `yen-pmd`. Coordinator: Codex, direct execution. Runtime unchanged.
The four tests originate in reviewed worker commit `599bf7d` and were copied
exactly into the primary checkout before this verification.

Unity 6000.6.0f1 passed all ten EditMode tests: four actual-frame lifecycle tests
and the six existing generated-history/oracle cases. XML includes simulation
frame counts for burn rate/expiry, wet expiry, extinguishing without resumption
and lethal-burn death accounting. Controlled `Time.captureDeltaTime` is engine
simulation evidence; it does not establish performance or movement feel.

Test run: `2026-11-11T03-35-17-749Z-273354a8`.
Runtime smoke: `2026-11-11T03-36-05-272Z-3fa5183d`, exit 0 and
`YENDOR_RUNTIME_SMOKE_OK` for spawns, ground, elemental damage, death, relic
and extraction. Both checks verified unchanged source/copy hashes:
`51ba64266ec859ffa999eb36d470f75ed7930a076ec8487b708574970adf88d0`.
Starting Git revision: `b3dc709` plus the recorded working tree; full file hashes
are in each run manifest. Later evidence documentation changes that whole-tree
hash; the verified gameplay/test bytes remain identified in those manifests.

`npm run check` passed syntax checks and 11 Node tests. Independent fixture
review is retained under `artifacts/pilots/yen-pmd/review.md`; no production
mutation campaign was performed.

## Launch diagnosis and reproduction

The local `npm` shim uses `sandbox-npm`, which mounts an empty home directory
and exposes the workspace. A read-only visibility probe inside npm confirmed
that the Unity executable is hidden while the ELF interpreter and workspace are
visible. This explains the two recorded npm launch ENOENT failures. Direct Node
version probes reported 6000.6.0f1. The npm sandbox configuration was unchanged.

For this host, prepare a fresh copy with `node tools/prepare-verification.mjs`,
then set `UNITY_PROJECT_PATH` to its printed path and run
`node --env-file-if-exists=.env tools/unity.mjs test`, followed by the same
command with `smoke`. The tool retains source checks, environment allowlist,
locks and run evidence. Keep the interactive Editor closed and serialize runs.

## Limitations and artifacts

The smoke log includes the previously observed UnityEditor SearchDatabase
startup `ArgumentOutOfRangeException`; gameplay assertions still passed.
No graphics, manual controls/feel, player build or performance evidence was
collected. These checks do not accept the full foundation milestone.

Full local-only logs/XML/manifests are under `artifacts/runs/<run-id>/`.
The diagnosis, commands, Node output and structured verification record are in
`artifacts/pilots/yen-pmd/resume-2026-11-11/`. No paid calls, merge or release.
