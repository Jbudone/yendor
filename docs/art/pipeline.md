# Copper Wild asset pipeline

The art direction uses warm sandstone, copper foliage, deep moss greens and
turquoise magic. Clear silhouettes and restrained surface detail keep fast combat
readable. Beauty comes from composition, scale, atmosphere and cohesive materials,
then from individual asset detail.

## First asset batch

1. Waystone: tall, recognizable extraction landmark; brief in content/art/waystone.json.
2. Broken arch and pillar: modular ruin kit with consistent snapping dimensions.
3. Copperpine: trunk and foliage separated for wind, LOD and instancing.
4. Briar revenant: readable enemy silhouette; rig/animation after the combat role settles.
5. Starheart: small hero prop with a clear emissive core.

Start with the waystone to establish material and silhouette quality. Review its
untextured preview before spending on refinement. Generate one prop at a time
until the style is consistent. Paid generation is initiated with the Meshy CLI:

```sh
npm run meshy -- balance
npm run meshy -- preview content/art/waystone.json
npm run meshy -- status TASK_ID
```

The preview command spends credits and returns an asynchronous task ID. It does
not wait for generation, refine, download or import. Avoid resubmitting on an
ambiguous network failure: check the Meshy dashboard/request logs first. A lost
response can still correspond to a paid task. Use Meshy's site for review/refine
and FBX export in this first version.

For every reviewed asset, keep a manifest recording original prompt/reference,
task ID, model version, generation date, account/license context, chosen variant,
source/export files and manual modifications. Never overwrite the source export.

Import FBX plus textures to game/Assets/Yendor/Art after review. Normalize to
meters, set the pivot, simplify colliders, author LODs, review normals and material
slots, and build a prefab via an Editor builder. Unity does not import GLB natively
without an added importer, so use FBX initially. Keep prototypes functional when
an external art service is unavailable.

## Unity AI subscription

Open this project in Unity 6.6, link it to the intended Unity Cloud project, and
activate the AI menu using the existing subscription. First-time activation
requires accepting Unity's terms in the Editor. Use Assistant for scene setup
and diagnosis and Generators for material, texture, sound and animation studies.
Store chosen assets and their provenance; retain ordinary Unity assets so builds
do not require AI services. See [Unity's activation instructions](https://docs.unity.com/en-us/engine/6000.3/manual/unity-ai/ai-menu-access).

Suggested first study: weathered sandstone with broad readable forms, subtle
warm/cool variation, restrained moss and recessed turquoise rune inlays. Match
the waystone palette before generating a large library of assets.
