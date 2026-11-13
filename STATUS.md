# Current project state

Yendor is a Unity third-person expedition shooter. Direction: `CONTEXT.md` and
`docs/design/first-expedition.md`. Beads owns execution detail.

## GitHub publication — yen-897

User authorized commit, push and a release update with screenshots/art showcases.
Coordinator Codex, direct; started 2026-11-13 22:25 UTC, checkpoint 23:00 UTC.
Publication branch `release/ai-art-showcase` is based on current upstream master
`3b6fbba`, preserving the earlier Editor checkout. Assets, Packages and
ProjectSettings exactly match the verified player snapshot. No paid calls.
Next boundary: pushed source, illustrated prerelease and verified Linux archive;
no merge to master is implied. Human acceptance gates remain open.

## Completed art and player outcome

Coordinator Codex, direct execution. User explicitly requested continuation and
use of Unity/Meshy credits. `yen-8ji` generated/reviewed the second studies;
`yen-rgi` completed package-stable verification of the integrated waystone.
Started22:05 UTC, result saved22:21 UTC before the 22:40 checkpoint. No explicit pause.
Next boundary: human art/audio/feel review and bounded ruin placement/collision
review; final style/fresh-player/hardware-performance gates remain open.
No generation/batch job remains running. No further paid call is active.

- Meshy: 20 credits used (1950→1930), one preview. Original combined prop failed
  placement brief; native extraction yielded unplaced 4m / 1,077-triangle arch study.
- Unity: 12 points used including Assistant (952→940; asset quote 9). One actual
  512×512 base-color material inspected in native 2×2 repeat render. Brick pattern
  contradicted continuous-stone brief; retained masonry study, kept off arch.
- Original exports, prompts, provider IDs, seeds and hashes preserved. No
  purchases/refinement/duplicate submission. Persistent AssetGeneration Allow
  remains verified; other permission policies unchanged.
- Optional inference 2.6.1 / App UI dependency caused batch settings drift. Native
  removal preserved Assistant/Generators, 940-point account readback and analytics
  preference true. Asset Knowledge was already disabled. Source gate unchanged.
- Exact-source Unity26 tests, resource validation/prefab audit, Linux build,
  Editor smoke and packaged-player smoke pass. `npm run check`: syntax + 11 tests.
  Headless smoke is not human/graphics/performance acceptance.

[Latest report](docs/reporting/updates/2026-11-13-ruins-and-player.md),
[ruin study](docs/art/ruin-study.md), [exact QA](docs/qa/waystone-integration.md).
Artifacts: `artifacts/pilots/yen-8ji/` and
`artifacts/pilots/yen-rgi/package-stability/`. Initial failed gates remain visible.

## Current playable build

Run `./artifacts/playtest-latest/play.sh`. Includes integrated 3m Meshy waystone,
Unity sandstone paving and success-only extraction cue (.25 volume); no cue on
replay/death. Native rendered review/clearance/interaction checks already pass.
New ruin studies remain unplaced. Thin side details and audio mix need human review.

Build GUID `31c46702cf284f51a9998c3ebc8ab443`, 255 files, bundle hash
`51e4a84538d74865406f53089b154300fe51222396e06e56ef8f21cd2b0715ce`.
Build source hash
`82d4a6243af92b80ec839440919450f0071955d53a09978a09188a5bb5baa06c`
identifies HEAD 090a40c plus dirty source, not HEAD alone. Reporting updated after
build; game/Assets,Packages,ProjectSettings remain identical to build snapshot.
Prior exact gameplay player retained at
`artifacts/playtest-before-waystone-2026-11-13/` (GUID f7476776753c49c09ed7acb96820a3e6).
Existing native Editor remains open, CopperWild clean in edit mode.

## Gameplay and human gates

PR 20 merged master 3b6fbba: mixed roles/telegraphs (`yen-gh7`), seeded pickups and
three cores (`yen-7a1`), rewarded overlook/Starheart return alarm (`yen-kn2`).
Human movement/camera/feel/audio, fresh-player acceptance and named-hardware timing
remain open (`yen-kk5`, `yen-g8n`). [Acceptance packet](docs/playtests/expedition-acceptance.md).
Authoring account setup/first studies `yen-7yi` complete; first waystone source
identity and manifests remain in `content/art/` and `docs/art/`.

## Checkout and publication

This publication checkout uses `release/ai-art-showcase`, based on master3b6fbba.
The separate primary Editor checkout remains on `workflow/finalize-verification`
with its pre-existing dirty source preserved. New source/media are committed for
this checkpoint; release tag `playtest-2026-11-13-ai-art` identifies the published
revision. [GitHub release](https://github.com/Jbudone/yendor/releases/tag/playtest-2026-11-13-ai-art).
The release archive includes the exact 255-file player, launcher and identity.
Full original local logs remain in artifacts; public evidence receipt is
`docs/reporting/evidence/2026-11-13-ai-art.json`. No master merge is implied.
