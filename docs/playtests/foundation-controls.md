# Foundation controls and camera — yen-kk5

Status: prepared 2026-11-11; human session **not run**. Coordinator: Codex.
No gameplay tuning or accepted design changes. This checklist records observations;
it does not turn automated smoke checks into manual acceptance.

## Exact build and launch

CI source: `696edb2e9dafa5ace5bcb9b73e65b1eda8f2e9c2`.
Build GUID: `58e54fd84e464cd8b5b701f3b4fd047a`.
Bundle SHA-256: `d6f3ba6cf7338190ff6fbfdf1fabfae181498f709c671cb1cb1b5e61207ff298`.
Seed: `43117` on a fresh process. Preset: bundled defaults, no tuning overrides.

[Full CI run](https://github.com/Jbudone/yendor/actions/runs/42957926313)
passed validation, ten tests, Editor/player smoke, asset audit and Linux build.
Local bundle verification and all 67 recorded game-file hashes match.
Evidence: `artifacts/pilots/yen-kk5/readiness.json`; copied player:
`artifacts/pilots/yen-kk5/player/`. These ignored local files are not a release.
If absent on another machine, download the run's
`unity-evidence-696edb2e9dafa5ace5bcb9b73e65b1eda8f2e9c2` artifact and locate
build run `2026-11-11T04-15-54-946Z-a8c0a766/player/`; verify its bundle before use.

From the repository root in a desktop terminal:

```sh
mkdir -p artifacts/pilots/yen-kk5/session-01
./artifacts/pilots/yen-kk5/player/Yendor.x86_64 \
  -screen-fullscreen 0 -screen-width 1920 -screen-height 1080 \
  -logFile "$PWD/artifacts/pilots/yen-kk5/session-01/player.log"
```

Use the desktop window close control to quit. Escape releases the cursor and
does not pause the expedition; click to capture it again. R/N work after the
expedition ends. Do not pass the automated smoke flag during a human session.

## Controls from the tested source

Source: `PlayerMotor.cs`, `Expedition.cs`, `RunDiagnostics.cs` under
`game/Assets/Yendor/Runtime/`, plus `game/ProjectSettings/InputManager.asset`.

| Input | Behavior |
| --- | --- |
| WASD / arrow keys; mouse | Move relative to facing; turn/aim |
| Space, twice | Jump, then one airborne jump |
| Left Shift | Directional dash; forward when stationary; 1.3 s cooldown |
| Hold left / right mouse | Carbine / ember |
| Q | Storm pulse within 14 units; 5 s cooldown |
| E | Take nearby Starheart or extract at waystone with it |
| Escape; left click | Release cursor; capture cursor again |
| R / N after end | Replay same seed / increment seed |
| F8 | Export run JSON and request screenshot |

F8 logs `YENDOR_REPORT_WRITTEN: <directory>`. Copy that directory into the session
folder and confirm the screenshot actually exists and is legible. JSON includes
build GUID, seed, CPU/GPU, player position and recent events; it does not measure
frame timing. Preserve a video clip for movement/camera observations.

## Guided foundation pass

Use a returning tester here. Keep the fresh-player diagnostic below separate.
For every row record pass/fail/inconclusive, what happened, and report/clip time.
Expectations describe current source or basic acceptance questions, not results.

| Check | Action and observation to record |
| --- | --- |
| Movement | Walk forward/sideways/diagonally; turn while moving. Record responsiveness and unexpected speed/collision changes. |
| Double jump | Jump twice, attempt a third press, land and repeat. Record extra jumps, failed resets or awkward timing. |
| Dash | Dash standing, sideways, forward and in air; try again during cooldown. Record direction, collision, control and recovery. |
| Camera | Orbit near a tree, ruin wall and corner; approach/back away, look up/down and jump. Clip clipping, jitter, sudden zoom or hidden threats. |
| Aim alignment | Shoot an enemy in the open and from either side of nearby cover. Compare reticle, tracer and actual hit; record obstruction position with F8. |
| Cursor | Escape during play, click to recapture, then aim/fire. Record behavior and any unintended shot. |
| Encounter | Fight one encounter using carbine and abilities. Record readability, feedback, stalls and cause of damage/death. |
| Death/replay | Let the run end in death; R. Confirm seed 43117 and reset health/kills/relic; compare captured initial enemy/decoration positions. |
| Relic/extraction | Recover Starheart with E; return and extract with E. Record objective clarity and end state. |
| Next seed | After end, N. Confirm displayed seed 43118 and reset state; F8. |

Do not label 30/60/120 fps supported from a frame cap or subjective smoothness.
Measured hardware timing belongs to `yen-g8n`; note resolution, refresh and any
observed stutter here without claiming a performance pass.

## Session result record

Copy this record to `artifacts/pilots/yen-kk5/session-01/results.md` and fill it
only after observation. Preserve failed cases as evidence before proposing tuning.

- Participant alias / date / machine / OS / CPU / GPU:
- Build GUID / bundle hash / displayed seed:
- Resolution / display refresh / input device / preset:
- Each checklist row: pass, fail or inconclusive; observed behavior; clip/report:
- Most disruptive control or camera issue, with reproduction position:
- Overall controls/camera acceptance: unrun (replace with supported/unsupported/inconclusive):
- Next proposed change (not yet accepted), or acceptance decision and its owner:

The separate [first-expedition diagnostic](first-expedition.md) still calls for
three independent fresh players, one uncoached run each, with objective-recognition
time and their post-run interaction explanation. Do not show those participants
the guided elemental checklist first. Two demonstrating both behaviors is its
provisional gate; one coordinator run cannot satisfy that gate.
