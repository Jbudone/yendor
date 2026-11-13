# Waystone presentation integration evidence

Task `yen-rgi`; coordinator Codex; direct existing-Editor execution.
User authorized the next increment with “Proceed” after the authoring pilot.
Starting revision `090a40c48b4aad79454cf12b970990564b09c25b`, preserving the prior
dirty gameplay sync, docs and user package/Cloud activation. No paid calls.
Evidence root: `artifacts/pilots/yen-rgi/integration/`.

## Scope and source

Native Editor import of the original FBX, reviewed prefab/materials/LOD proxy,
paving inset, extraction cue hookup in `Expedition.cs`, and required-resource
validation in `ProjectSetup.cs`. No scene YAML was edited. Original FBX SHA-256:
`f9bd85e79b2e08c39bad70e5e83ae6386a1a6201332cb8851e7180312b497775`.
Authoring source: `tools/authoring/WaystoneArt.cs`, invoked with Pipeline
`run_script` in the existing Editor. `Build` refuses missing source inputs or an
existing output prefab. It is explicit authoring, never an automatic validator.

`integration-files.json` records source/resource/builder hashes.
`expedition-art-increment.patch` isolates this hookup from the pre-existing large
gameplay diff. Original studies remain unmodified; versioned manifests record
derived materials and runtime references.

## Passed evidence

- `native-model-review.json` and `waystone-view-{0,1,2,3}.png`: real native views
  of the original mesh. `connectivity.json`: one connected component after
  welding original OBJ positions to five decimal places; 5,100 triangles.
- `normalization-result.json`: rendered-bounds normalization preserves the FBX
  axis rotation and asserts height 3m, base y=0 and centered landmark geometry.
- `prefab-review.json`, `prefab-view-{0..5}.png`: native rendered review, including
  four sides, paving and 40m at FOV 78. Simple collider approximately
  1.446×3×0.687m. Near/distant model triangle counts 5,100/396, plus persistent
  core primitives and paving. No missing materials or scripts.
- `runtime-controlled-result.json`, `runtime-review.json`,
  `replay-and-death.json`: seven initial enemies and three return hunters;
  spawn capsule clear above its actual support; prefab present and correctly
  grounded; cue plays only on successful extraction, repeated interaction does
  not restart it, same-seed replay resets state, death stays silent.
- `runtime-waystone-near.png`, `runtime-waystone-40m.png`: actual runtime world
  captures on NVIDIA GeForce GTX 1070 Ti. No hardware performance claim.
- `final-compile-status.json`, `native-audit-command.json`, `native-audit.json`:
  final compilation, `ProjectSetup.Validate` and `AuditAssets` pass. Scene remains
  CopperWild, clean and in edit mode. Required prefab, cue configuration and
  collider are validated without creating missing inputs.
- Original audio waveform analysis remains in `artifacts/pilots/yen-7yi/`:
  stereo 44.1 kHz, 2.03755 seconds, zero clipped samples. Runtime level is .25.
  `extraction-cue-audition.wav` is a local level-reference export, not proof of
  subjective listening or combat-mix acceptance.

## Failed or limited checks retained

The first prefab views exposed a world/local offset error. A correction using
raw mesh bounds then exposed the FBX's different native axes. The hypothesis was
changed to measuring actual renderer bounds with the imported rotation retained,
with explicit pivot/height invariants. Failed views remain as `displaced-*` and
`raw-axis-*`; final captures were inspected again.

Early interactive requests hit a play-mode transport transition and a stale
singleton while reusing a live session. The initial clearance check also
intersected the existing platform because it measured above y=0 rather than the
actual support height. The final controlled edit-to-play cycle and single complete
assertion script pass. Failed command/console/scene records remain; this is not a
claim of fixing general Editor hot-reload behavior.

The npm wrappers hide the Editor executable, causing startup ENOENT failures.
The equivalent project runner was then invoked directly:

```sh
UNITY_PROJECT_PATH=/home/jbud/jdrive/jstuff/work/personal/yendor/.workflow/verify-1794596024270/game \
  node --env-file-if-exists=.env tools/unity.mjs test
```

Actual run `2026-11-13T18-55-01-818Z-061ecb08` contains the complete log and XML:
26 passed, zero failed. Its **overall status is failed**: the package initializer
removed `SENTIS_ANALYTICS_ENABLED` and registered `com.unity.dt.app-ui` in the
copy's ProjectSettings. `first-import-differences.json` retains both file hashes;
`test-summary.json` and `test-scope-comparison.json` distinguish test assertions
from source reproducibility. Assets and Packages match the final runtime art;
the only later Assets difference is the tightened Editor validator, checked by
the native audit. No user analytics preference or package setting was changed to
silence the gate. No clean packaged build or new player smoke pass is claimed.

## Review boundary

The source presentation is playable in the current Editor. The existing packaged
player is unchanged. Next: stabilize the package-driven settings for a reproducible
batch gate and build, then review the art and listen to the cue during a real run.
The mesh's thin side details remain rough; the sandstone study is accepted here
only as provisional paving. Human feel/readability/audio and measured hardware
performance gates remain open. No new paid refinement is implied.


## Package stability resolved and exact player verified

Continuation under `yen-rgi`, 2026-11-13 22:12–22:21UTC, zero paid calls.
The earlier source-gate failure above is retained historical evidence. Native
Package Manager removed optional inference2.6.1 and its App UI dependency; the
game has no inference references. Assistant's installed documentation permits
removing Sentis for builds when Asset Knowledge is unnecessary. Native readback
showed Asset Knowledge was already disabled. Assistant/Generators remain loaded,
asset-generation policy remains Allow, account readback940points, analytics
preference unchanged true. Only package-owned config/defines were cleared.
Backups and native compilation failures before API names were corrected remain
in `artifacts/pilots/yen-rgi/package-stability/`. These failed scripts performed
no package mutation; the corrected native operation succeeded.

All subsequent runs use source hash
`82d4a6243af92b80ec839440919450f0071955d53a09978a09188a5bb5baa06c`
(HEAD090a40c plus preserved dirty gameplay/art/configuration, not HEAD alone):

| Gate | Result | Run under artifacts/runs/ |
| --- | --- | --- |
| EditMode tests | 26pass,0fail; source gate passes | 2026-11-13T22-14-24-251Z-8031ae65 |
| Required resource validation and prefab audit | Pass | 2026-11-13T22-16-54-232Z-0f02257b |
| Linux development player build | Pass | 2026-11-13T22-17-48-937Z-fc65ce47 |
| Editor runtime smoke | Pass | 2026-11-13T22-19-04-328Z-d17e11fc |
| Exact packaged-player headless smoke | Pass, graphicsVerified=false | Build run above |

`npm run check` passes syntax and11Node tests. Assets, Packages and ProjectSettings
match the source snapshots before and after every Unity run; the gate was not
relaxed. Build GUID `31c46702cf284f51a9998c3ebc8ab443`,255files, bundle SHA256
`51e4a84538d74865406f53089b154300fe51222396e06e56ef8f21cd2b0715ce`.
The copied bundle was independently hashed before promotion. `checkpoint-result.json`
and `promoted-player.json` record full identities. Later changes are reporting only;
final game directory hashes are compared with this build snapshot again.

`./artifacts/playtest-latest/play.sh` now launches this waystone-equipped player.
Previous player retained at `artifacts/playtest-before-waystone-2026-11-13/`.
Native Editor remains clean CopperWild in edit mode. The separate new ruin arch
and Unity masonry studies remain unplaced, so they are not promised in this level.
Human listening, visual/feel/fresh-player acceptance and measured hardware timing
remain open; headless smoke establishes none of those gates.
