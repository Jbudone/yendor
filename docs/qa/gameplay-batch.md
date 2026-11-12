# Gameplay batch — 2026-11-12

Tasks: `yen-gh7`, `yen-7a1`, `yen-kn2`. Coordinator Codex, native sequential
fresh workers in isolated worktrees. User explicitly authorized several major
gameplay increments before the next human playtest. Base: master `12cc7c9`.
External paid calls: zero. Human gates are deferred prerequisites for this batch,
not passed acceptance. Balance values and placeholder presentation remain provisional.

## Combat: yen-gh7

Worker `/root/combat_iteration`, commit `9142a4d`. Integrated at `4023f12`.
Three ordinal-assigned roles preserve the world random stream and seven spawn
positions. Brutes wind up melee; spitters commit aim and check cover at release;
flankers approach off-axis. Attacks cancel on death/end. HUD shows health, damage,
hit/kill/hurt confirmation and ability countdowns; audio uses procedural tones.

Validation, fourteen Unity tests and runtime smoke pass at source hash
`b4e18839205f92370b237eb69fcd4afea703aa120073c679c6705c7bdda99f6b`.
Coordinator inspected code, actual XML, log hashes and game-file hashes.
Local-only worker run directories:
- `artifacts/runs/2026-11-12T03-34-30-572Z-ded2d493`
- `artifacts/runs/2026-11-12T03-35-25-243Z-241d1d2d`
- `artifacts/runs/2026-11-12T03-36-00-764Z-398eba54`

These are under `.worktrees/yen-gh7/`; independent coordinator audit is at
primary `artifacts/pilots/yen-gh7/coordinator-audit.json`. Started 03:27, completed
03:37 UTC before 04:07 checkpoint. Labels currently show through cover within 30m.

## Pickups: yen-7a1

Worker `/root/pickup_iteration`, commit `efcef8d`. Integrated at `d9b15cc`.
Three exclusive cores support carbine, Ember or Storm tradeoffs; two supply types
restore life or increase maximum life. Definitions drive actual combat input
methods. A separate seeded RNG shuffles shrine slots without touching world RNG.
Consumed IDs and core state are per Wayfarer. Full-life salve remains available.
Core comparisons appear near the shrine, moved to z=-24 after aggro review.

Validation, twenty Unity tests and smoke pass at source hash
`c12e6e352cc10bd606ae1ea3f8c85f0ed8eac759f2994c7a5be918c0674923fe`.
Coordinator independently inspected XML/log hashes/game bytes and reviewed
collection guards, modifier effects, interaction priority and shrine safety.
Local-only final run directories under `.worktrees/yen-7a1/`:
- `artifacts/runs/2026-11-12T03-45-23-115Z-edba18f3`
- `artifacts/runs/2026-11-12T03-45-48-935Z-85f4f26c`
- `artifacts/runs/2026-11-12T03-46-35-431Z-3eecb60e`

Primary audit: `artifacts/pilots/yen-7a1/coordinator-audit.json`. Started 03:38,
completed 03:48 UTC before 04:18 checkpoint. Actual R/N input scene restart has
not been exercised; reset assertions cover new player/build state and source
contains no static powers or consumed IDs.

## Exploration and return: yen-kn2

Worker `/root/expedition_iteration` from `d9b15cc`. An eastern authored loop
connects the road at z=-12 and z=33, using 6m-wide ramps and a 4m-high overlook.
The cache adds unique Heartwood/salve supplies; clearing trees preserves original
RNG consumption. Gold/turquoise spires and bearing/distance hints distinguish
objectives. Taking the Starheart once adds three mixed hunters, with an open exit
and updated enemy count/phase. F8 includes phase, core, supplies and alarm state.

Twenty-six Unity tests pass at source hash
`8a7d4d205a548a5e973fc55d637c80b1b65ccfa9cd0e2b35771e2ff839110c35`.
New checks cover multi-lane support/clearance, actual CharacterController loop
walking in both directions, real cache effects, once-only safe mixed return
spawns, death/extraction guards, and actual build-scene same/next-seed reloads.
Replay now reproduces physical enemy/core positions and resets health/build/alarm.
A confirmed F8 regression lost its reporter after reload; a guarded persistent
reporter now survives both reload paths without duplicates. This resolves the
previous increment's scene-restart limitation.

Tests initially caught a trail cairn on the approach: it moved outside the
corridor. A subsequent sampler failure came from expecting flat ground where
the connector correctly overlaps the rising ramp; the expectation was corrected.
A relic-priority smoke assertion was moved before the second E press, which
legitimately collects the supply. The replay regression was captured before its
lifetime fix. Failed runs are retained, not counted as acceptance. Final worker commit `56588bc`; final validate/test/smoke runs are
`2026-11-12T04-05-15-006Z-e733a8fc`, `2026-11-12T04-05-43-482Z-d114bc47`
and `2026-11-12T04-06-45-304Z-dae8aebe`. Coordinator verified actual log hashes,
XML counts, game bytes and 29 unique asset GUIDs; primary evidence is at
`artifacts/pilots/yen-kn2/`, with full final run directories preserved. Started
03:49 UTC; worker completed 04:08 UTC, before the 04:29 checkpoint.
Coordinator build/render evidence follows after final delivery.

## Evidence boundaries and retained failures

Commit hooks passed real Gitleaks, syntax and all eleven Node cases for the first
two workers. The npm environment cannot resolve all external tools; direct Node
runners were used. Local dependency symlinks were excluded from source scanning.
First combat test import generated metadata; the wrapper correctly rejected its
source identity and exact-source gates were rerun. The pickup worker initially
edited during import; that run was rejected and all final gates used frozen source.
Earlier successful runs were superseded after source/placement refinements.

A graphics-enabled automated baseline capture of the OLD foundation player is
at primary local-only `artifacts/gameplay-batch/baseline-capture-focused/`.
F8 identifies GUID `58e54fd84e464cd8b5b701f3b4fd047a`, seed 43117, source
`696edb2`, software OpenGL llvmpipe at 1280x720. It demonstrates a capture path,
not a new gameplay result or hardware performance. The initial unfocused capture
failed to send inputs; explicit isolated X focus fixed that, and F8 plus movement
position were then verified. The primary interactive Editor was not modified.

## Integrated build and rendered inspection

Build source `e44dca4e4b1cb7b1221c31a9555cabf347b7b83f`; gameplay files match
final worker `56588bc`. Later reporting/images are documentation only.
Build GUID `f7476776753c49c09ed7acb96820a3e6`; 247-file bundle SHA-256
`084446674704dbe03f539337fe0da72c1595b245d69eeed7145021200da16a34`.
Exact player and rehashed copy are local-only at `artifacts/playtest-latest/`
in the primary checkout; launch with `./artifacts/playtest-latest/play.sh`.
No GitHub Release is implied.

Integrated source hash `165a2d5ff48cbc6ecc767d57386c8ddbbba0a8ba439e0e64a8a25e82f173b60f`.
Asset audit `2026-11-12T04-10-00-259Z-d1de0ed8` and Linux build
`2026-11-12T04-10-36-238Z-fedd243c` pass. Packaged-player smoke confirms
matching GUID/bundle. Full logs/manifests are preserved under integration worktree
`artifacts/runs/`, with primary player/log/manifest copies in `artifacts/playtest-latest/`.
The runner used the idle worker project after byte-for-byte Assets/Packages/
ProjectSettings equality checks. No mutable Library was shared with a running
worker or the primary Editor.

Coordinator inspected actual frames in primary local-only
`artifacts/gameplay-batch/verified-capture/`: Tempest selected; x=19.895,
y=4.080,z=9.524 on the deck; Heartwood collected (125 life, one supply); relic
recovered, Return phase, alarm true, three hunters and ten enemies. F8 GUID
matches the exact player. Renderer llvmpipe/OpenGL, 1280x720, seed 43117.
Published PNGs are exact copies. The initial new capture stopped near a full-life
salve and did not collect Heartwood; the route was adjusted and the first attempt
retained. Mouse-look, subjective feel, audio mix, hardware timing and human
acceptance remain unverified. Known Editor SearchDatabase startup exception
remains in Editor logs; no runtime exception observed during these player captures.
