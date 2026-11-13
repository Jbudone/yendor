# Unity AI first authoring pilot

Task `yen-7yi`: activation and first studies verified. Production art acceptance
remains separate. Coordinator Codex used the existing Editor directly.

Cloud project `yendor` is linked. Assistant `2.21.0-pre.2` includes Generators;
inference `2.6.1` was installed by the user. Explicit CLI targeting of `game/`
connects to the existing Editor, even though global discovery initially missed it.
The installed package documents `AssistantApi` / `IAgent.RunHeadless` in
`Documentation~/integration/assistant-api.md`. The study context exposed only
model discovery and asset generation.

## Generated candidates

- `game/Assets/Yendor/Art/Studies/WaystoneSandstone.mat`: Standard material with
  one 512×512 generated base-color map. Hand Painted Textures 2.0. Actual texture
  inspection found a suitable warm sandstone/moss palette but a large slab/paving
  pattern. Keep as a study; monolith suitability, tiling and lighting need review.
- `game/Assets/Yendor/Art/Studies/WaystoneActivation.wav`: requested two-second,
  non-looping activation cue using ElevenLabs Sound Effects V2. Native import
  verified: stereo, 44.1 kHz, 89,856 frames (2.03755 seconds). PCM peak 0.96756;
  zero clipped samples. Listening, aesthetics and combat masking remain unrun.

[Versioned provenance](../../content/art/unity-ai-study-manifest.json) contains
prompts, source hashes, model evidence, material provider job ID and limitations.
Original exports, native account/asset checks and full attempts are retained in
`artifacts/pilots/yen-7yi/`. That authoring pilot added no gameplay references; the
later `yen-rgi` presentation increment now uses both studies provisionally.

## Budget and permissions

Native account snapshots: 1,000 points before the pilot, 952 after; aggregate
observed decrease 48. Native generation quotes were 9 for material and 27 for
sound. Individual actual charges were not returned; the aggregate includes
Assistant conversation overhead. No purchase, top-up or refinement was submitted.

The user explicitly requested persistent approval. Native
`AssistantEditorPreferences.Permissions.AssetGenerationPolicy` was changed from
Ask to Allow and read back successfully. It persists in local Editor preferences
across projects. All other eight policy values were compared and unchanged.
Evidence: `artifacts/pilots/yen-7yi/permission-setting-verified.json`.
The popup's scope selector only remembers a conversation; it is not this persistent
setting. The user dismissed the existing modal once because `ShowModalUtility`
blocked the CLI configuration request. The subsequent sound completed without
another approval popup. Permission does not expand authorized spending scope.

## Retained failures and reconciliation

Initial conversations lacked cost/path preflight and repeated confirmation. After
native quote and authorization evidence was supplied, an omitted reference-image
argument became invalid ID 0. Source inspection established local validation
before provider submission; the corrected call explicitly used -1.

The material Assistant orchestration returned an unexpected error while the
provider job `01a1d3c2-4c2b-73fd-bb7a-6bf5bf75d467` continued in the background.
The original recovery record was copied. A guarded recovery check found no
resumable jobs and issued no requests. Background completion was reconciled by
loading the material's actual texture reference; no paid job was resubmitted.
The sound was then generated once. The permission-control timeout and one
read-only inspection overload error remain in the evidence directory.

## Next boundary

Review sandstone on a model in the actual scene and listen to the cue alongside
combat warnings before production integration. The authoring pilot is complete;
no human feel, visual gameplay, audio mix or hardware performance gate is closed.

## Subsequent integration

The later user “Proceed” authorized use of these existing candidates without
new spending. Sandstone is used as waystone-platform paving and the cue plays
once on successful extraction. The original study assets remain unmodified.
[Integration report](../reporting/updates/2026-11-13-waystone.md) and
[QA](../qa/waystone-integration.md) distinguish native/runtime passes from the
failed package-settings reproducibility gate. Human listening remains open.


## Authoring dependency profile

`yen-rgi` removed optional `com.unity.ai.inference`2.6.1 after tracing batch
settings drift to its analytics initializer and its App UI visualizer config.
The game's Assets have no inference API references. Assistant's packaged
`Documentation~/reference/preference.md` explicitly permits removing Sentis
before builds when Asset Knowledge is not needed. Asset Knowledge was already
disabled before removal; it remains disabled. Assistant/Generators remain in
`com.unity.ai.assistant`2.21.0-pre.2. Local visual embedding search is unavailable
until inference is reinstalled; credit-funded generation remains available.
Native removal also cleared only the package-owned SENTIS/App UI defines and App
UI config. EditorAnalytics was true before and after; no preference was changed.
Native compilation passes and account readback still returns940 points. Backup,
source inspection and native results: `artifacts/pilots/yen-rgi/package-stability/`.
