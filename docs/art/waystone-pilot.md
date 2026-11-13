# First waystone study and provisional integration

Task `yen-rgi`. Original Meshy task `01a1d392-af59-7346-b974-1dfd2231d776`
succeeded for 20 credits (1,970 → 1,950). The later user “Proceed” authorized
review and provisional integration of existing assets; no new generation or
refinement was submitted.

Source brief: `content/art/waystone.json`. Provenance:
`content/art/waystone-preview-manifest.json`. Original PNG/GLB/FBX/OBJ exports,
provider identity and hashes remain in `artifacts/pilots/yen-rgi/`.
The imported FBX bytes match the original export.

## Reviewed result

Four original-mesh views and four authored-prefab views were actually rendered
and inspected in Unity. The ring/core silhouette is useful from front and back;
small side details remain rough. Topology analysis found one connected component
at five-decimal position welding. This does not imply production mesh quality.

Native Editor tooling authored `Assets/Yendor/Resources/YendorWaystone.prefab`:
three-metre landmark with base-center geometry pivot, approximately 1.446m width
and .687m depth, one box collider, two model LODs (5,100 and 396 triangles), plus
persistent turquoise core shapes. The original extraction platform stays in place;
the marker sits 3.5m to its side, outside the tested spawn capsule.

The Unity AI sandstone study reads as paving, so a derived material is used on an
inset inside the existing cyan extraction rim. The monolith keeps a simple warm
stone material. The imported activation cue plays at .25 volume on extraction,
with autoplay and looping disabled. The original study assets are unmodified.

Actual near and 40m runtime captures and scripted interaction/replay/death checks
are saved under `artifacts/pilots/yen-rgi/integration/`. Native final resource
validation and prefab audit pass. [QA](../qa/waystone-integration.md) retains
source hashes, failed attempts and the package-settings reproducibility failure.

## Current boundary

This remains provisional presentation, now playable both in the open Editor and
`./artifacts/playtest-latest/play.sh`. Human listening, combat mix, feel,
readability and hardware timing remain open. The earlier source-gate failure is
retained in QA; optional inference removal resolved it, and26tests, audit, build,
Editor smoke and packaged-player smoke now pass with exact source/bundle hashes.
Build GUID `31c46702cf284f51a9998c3ebc8ab443`. Original license/account-context
limitations remain in the provenance record; no public asset release is implied.
