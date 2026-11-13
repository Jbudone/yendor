# Copper Wild: the first playable AI art checkpoint

[Download the Linux playtest](https://github.com/Jbudone/yendor/releases/tag/playtest-2026-11-13-ai-art) · [Art provenance](../../../content/art/) · [Verification](../../qa/waystone-integration.md)

The extraction landmark now uses Meshy-generated geometry, Unity AI sandstone
paving and a Unity AI activation cue. The combat, tactical cores, overlook and
Starheart return from the previous gameplay batch remain intact. This is an early
playtest with provisional art; the broader environment is still a greybox.

## In the expedition

![Waystone and sandstone extraction platform, actual Unity runtime capture](../images/waystone-integration-near.png)

The three-meter waystone stands beside the extraction pad with turquoise cores
visible from both faces. Meshy supplied the original ring-shaped mesh; native
Editor tooling authored the scale, base pivot, simple collider, distant LOD and
colored cores. Unity AI supplied the paving texture and extraction sound. The cue
plays once on successful extraction, stays silent on death, and resets on replay.

![Extraction landmark seen from forty meters at gameplay field of view](../images/waystone-integration-40m.png)

These are actual native Unity `Camera.Render` captures during controlled runtime
review on a GTX 1070 Ti, without the HUD. They show the integrated art assets;
they are not concept art, screenshots of the packaged executable, or frame-rate evidence.

## Meshy showcase: geometry and cleanup

![Original Meshy waystone geometry rendered in Unity](../images/meshy-waystone-source.png)

**Waystone:** one preview, 5,100 triangles. The imported FBX is unchanged; the
playable prefab adds a 396-triangle distant proxy and authored turquoise cores.
Thin side details remain rough. [Source and modifications](../../art/waystone-pilot.md).

| Generated ruin preview | Isolated arch candidate |
| --- | --- |
| ![Original Meshy result with extra columns and floating lintel](../images/meshy-ruin-original.png) | ![Cleaned single arch rendered in Unity](../images/ruin-arch-study.png) |

**Ruin arch:** the 4,032-triangle preview included unwanted interior columns and a
floating lintel. Native C# tooling extracted the single arch component, preserving
the original export. The resulting candidate is four meters tall and 1,077
triangles. It remains **unplaced**, without collision, pending route and modular-kit
review. No extra generation was needed. [Review](../../art/ruin-study.md).

## Unity AI showcase: materials and audio

| Sandstone source used as paving | Second material retained as a masonry study |
| --- | --- |
| ![Unity AI sandstone paving texture](../images/unity-ai-sandstone-source.jpg) | ![Unity AI masonry texture study](../images/ruin-masonry-study.jpg) |

Both are 512×512 base-color maps from Hand Painted Textures 2.0. The first reads as
paving and appears on the extraction platform. The second produced a brick grid
despite a continuous-stone brief, so it is **not applied to the arch or level**.
Neither is a complete PBR material set; final style acceptance remains open.

[Download/listen to the generated activation cue (.wav)](https://github.com/Jbudone/yendor/releases/download/playtest-2026-11-13-ai-art/WaystoneActivation.wav)

[Preview clip: native still frame with the original generated cue (.mp4)](https://github.com/Jbudone/yendor/releases/download/playtest-2026-11-13-ai-art/WaystoneActivation-showcase.mp4). This is an audio showcase, not a gameplay recording.

The original Unity AI cue uses the requested ElevenLabs Sound Effects v2 model,
lasts 2.04 seconds, and plays in-game at .25 volume. Native import and playback
assertions pass; subjective listening and combat-mix approval remain open.
[Unity AI provenance](../../art/unity-ai-pilot.md).

Across the two art rounds, account snapshots show **40 Meshy credits** and
**60 Unity points including Assistant overhead** used; **1,930 credits / 940 points**
remain. The latest round used 20 / 12. There were no purchases, refinements or
blind retries. Prompts, provider IDs, selected source hashes and modifications are
recorded in `content/art/`.

## Play and verification

Download the Linux archive from the release, extract it, and run `./play.sh`.
WASD moves, mouse aims, Space double jumps, Shift dashes, LMB fires, RMB casts
Ember, Q casts Storm, E interacts; R replays the seed and N chooses the next.
Recover the Starheart and return to the turquoise waystone.

| Check | Result |
| --- | --- |
| Unity EditMode tests | 26 passed, zero failed |
| Required resources and prefab audit | Passed |
| Linux development build | Passed |
| Editor runtime smoke | Passed |
| Exact packaged-player headless smoke | Passed; graphics not verified by this check |
| Node syntax and tests | Passed, 11 tests |

Build GUID: `31c46702cf284f51a9998c3ebc8ab443`. The 255-file player bundle hashes to
`51e4a84538d74865406f53089b154300fe51222396e06e56ef8f21cd2b0715ce`.
The release retains this exact built bundle. Source game directories are compared
to the build snapshot; documentation changes do not require a new gameplay build.
The earlier source-stability failure is retained in QA. Removing optional local
inference resolved it while preserving Assistant/Generators and analytics settings.

Human art/audio/feel review, fresh-player acceptance and measured hardware frame
timing remain open. Next work is that review and bounded ruin placement, collision
and LOD checks. The new arch and masonry study are showcases, not level additions.
