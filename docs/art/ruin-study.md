# Copper Wild ruin study

Task `yen-8ji`, coordinator Codex, direct existing-Editor authoring. User requested
use of the existing Unity and Meshy credits. Source HEAD `090a40c` plus preserved
local gameplay/waystone work. No runtime or level placement changes in this study.

Meshy `meshy-7.1` task `01a1d6f2-0dfe-74a1-86a7-d5258ae83aeb` completed for
20 credits (1950 → 1930). The original 4032-triangle result failed the open-passage
and no-floating-fragments brief: two interior columns, an unattached lintel and
small disconnected slivers accompanied the arch. All original exports remain in
`artifacts/pilots/yen-8ji/`; FBX import matches SHA256
`ca8405f985c2f2bd63961b40f987c24f4c766a30ebbfaed73ffb0c1599fed210`.

Native C# authoring extracted the single arch component without regenerating it.
The unplaced `Assets/Yendor/Art/Ruins/BrokenArchStudy.prefab` is 1077 triangles,
4m tall × 3.709m wide × 1.043m deep, with a centered base origin. Surface UVs use
per-face box projection at one metre per repeat. No collider is included until
route placement and clearance are reviewed. Four actual native Editor views were
inspected: the passage is open and the floating pieces are gone. Faceting and
uneven feet remain visible. This is a reusable candidate, not an accepted final
modular kit or a claim of human readability/performance acceptance.

![Isolated arch native render](../reporting/images/ruin-arch-study.png)

Unity AI job `01a1d6f2-6555-7668-b5f6-182beb3ccb41`, Hand Painted Textures
2.0, seed221943637, finished with one512×512 base-color map in
`Assets/Yendor/Art/Studies/RuinSandstone.mat`. The actual texture and a native
2×2 repeat render were inspected. It produced a prominent brick grid despite the
continuous-stone prompt; rejected for the arch and retained as a masonry study.
No PBR map generation, extra variants or retry was performed. Points952 →940
(12 total including Assistant; native asset quote9, individual charge unexposed).
The CLI transport timed out while the provider continued; the original job ID
was reconciled, no new submission. Final interrupted-download log is empty.
`content/art/ruin-material-study-manifest.json` retains provenance.

Native authoring and rendered inspections passed for the salvaged candidate;
no gameplay tests/build are claimed for this unplaced study. Human style review
remains open. Next: route placement/collision/LOD review, then modular dimensions;
do not blindly regenerate to consume the balance.

Reproduce the native extraction with `tools/authoring/RuinArt.cs` in Unity
Pipeline `run_script`, entry `RuinArt.Build`, only when outputs are absent. It
refuses overwrites. `RuinArt.ReviewArch` captures the existing study. Neither
method submits a paid request or modifies the active scene. Source FBX remains
untouched. Provenance: `content/art/broken-arch-preview-manifest.json`.
