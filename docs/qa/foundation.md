# Foundation verification — 2026-11-07

## Passed

- Unity 6000.6.0f1 created the project and compiled the gameplay C#.
- The Editor validation entry point found the saved expedition scene and its bootstrap.
- A Linux x86_64 development player build succeeded.
- An Editor play-mode smoke run passed: player/camera and seven enemy spawns,
  ground at extraction, dry ember ignition, doubled Storm damage on wet enemies,
  enemy death counting exactly once, distant interaction rejection, relic pickup
  and successful extraction.
- Node tests passed for specialist file scope restrictions, traversal/symlink
  rejection, protected scene extensions, valid JSON writes, model alias routing,
  Meshy preview validation and mocked read-only Meshy HTTP requests.
- Studio entry-point JavaScript passed a syntax check. Its Mastra imports and
  live model/tool behavior still require dependency installation and live testing.

Logs live under ignored `artifacts/`; the README contains commands to reproduce
the checks. The Linux player is `game/Builds/Linux/Yendor.x86_64`.

## Environment findings

The direct Unity executable has an existing approved execution path. Starting
the Editor as a child of the Node build helper in this restricted agent sandbox
failed to connect to licensing and Package Manager IPC. The direct Editor build
and smoke commands succeeded. Use the direct executable here; the Node helper
is intended for an ordinary local terminal/CI environment with Unity access.

The Editor smoke run also emitted an internal Unity SearchDatabase startup
exception from UnityEditor.Search.SearchInit. The gameplay assertions still ran
and passed, and the Editor exited successfully. This was not a gameplay-source
exception. Track it if it also reproduces during interactive Editor use.

npm registry access failed with DNS resolution errors. Offline installation could
not resolve the package metadata needed for installation. No completed install
or verified dependency lockfile was produced. Versions in package.json are pinned;
install them on the normal development machine and commit the resulting lockfile
after confirming supervisor delegation and model compatibility.

This execution environment has no display. A graphics-enabled Editor invocation
could not open one; no screenshot or visual approval was obtained.

## Still required

- Manual camera, aiming, dash, double-jump, restart and encounter playtesting.
- Rendered visual inspection and a URP art/lighting pass; current geometry is placeholder.
- Runtime coverage for oil/water surface overlap, burn expiry/extinguish, movement,
  death/restart, seed reproduction, camera obstruction and enemy navigation.
- npm installation and a live Mastra supervisor run through a configured LiteLLM
  gateway, including tool calls, failure recovery, time/step limits and trace review.
- Configure provider financial limits. Local step limits are not a dollar budget.
- Unity Cloud project selection and Unity AI activation using the user's account.
- Meshy API key/account capability verification and a balance check before generation.
- Review/refine/import the first Meshy asset with provenance. No generation was
  submitted and no subscription credits were spent during this session.
