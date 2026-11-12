# Yendor

Yendor is an original third-person expedition roguelike shooter built in Unity.
Its pillars are NetHack/Dungeon Crawl style systemic discovery, Dragonwilds
inspired wilderness wonder, and Deadlock inspired movement and shooter intensity.
These references guide feel and structure; characters, worlds, art and mechanics
must have their own identity.

## Accepted direction

- Third-person camera, explicitly selected by the user.
- Unity is the game runtime. Mastra is the development supervisor framework.
- LiteLLM provides configurable model routing for specialist agents.
- Use the existing Unity AI and Meshy subscriptions as authoring tools.

## Initial assumptions to revisit

Single-player, desktop Linux development, eventual Windows target. One continuous
expedition with fast combat and quiet exploration between encounters. Death ends
the run; replay supports the same seed. Multiplayer, persistence, hunger, full
inventory, unidentified items, procedural dungeon topology and long-term progression
are later design decisions. AI currently assists development, not live gameplay.

## Domain language

- Expedition: one run, from the waystone to a relic and back, ending in escape or death.
- Wayfarer: the player character.
- Waystone: the expedition entry and extraction landmark.
- Starheart: the first objective relic; other relic types are future work.
- Copper Wild: the first biome, a copper-green forest surrounding ancient ruins.
- Element: damage family. Ember ignites dry targets and oil; water extinguishes
  burning and makes Storm more damaging. Kinetic damage has no status effect.
- Seed: reproducible world-decoration and enemy-position input. Current topology
  and objective positions are fixed; this is not yet a dungeon generator.

## Current implementation

Unity 6000.6.0f1. A built-in-render-pipeline greybox with a generated forest/ruin
route, third-person orbit camera, double jump, dash, hitscan carbine, ember attack,
storm pulse, pursuing enemies, water/oil surfaces, relic recovery, extraction,
death, and same/next seed restart. The gameplay batch adds melee/ranged/flanker
roles with attack telegraphs and combat feedback, three exclusive tactical cores,
healing/maximum-life supplies, a rewarded elevated overlook, objective beacons and
a bounded Starheart return alarm. Same-seed replay resets run power and preserves
physical offerings and initial enemy positions; F8 diagnostics survives reloads.
Generated geometry is placeholder art. The
visual target requires a later URP lighting/material/environment pass.

Runtime source lives in game/Assets/Yendor/Runtime. Editor builders live under
game/Assets/Yendor/Editor. CopperWild.unity contains a bootstrap; the run builds
its geometry on entry. Never manually rewrite Unity scene or prefab YAML.

## Development workflow

Mastra supervisor delegates scoped tasks to design, combat, systems, world, art,
and QA agents. Model aliases are independent from providers. Specialists read
current source before editing. Shared contracts are coordinated sequentially;
the world agent owns the scene builder. Tools enforce path scopes, reject symlink
traversal and restrict file extensions; this is trusted local tooling, not an OS
sandbox. Compile validation runs separately from runtime checks and playtesting.

Secrets stay in local environment files, outside Unity Assets. Meshy requests
record source briefs and task IDs; assets are reviewed before importing. Unity
AI is activated inside the Editor against the user's linked Cloud project.

The durable workflow is specified in docs/adr/0002-project-workflow.md. Fresh
sessions follow AGENTS.md and STATUS.md. Beads owns task execution state; Agent
Mail connects independent coding clients. Scoped task history is persisted in
local LibSQL; source-linked project notes stay versioned and are retrieved only
when relevant. Validation evidence is tied to source/build identity.

## First milestone

A readable 5–10 minute expedition: choose a route, discover one useful interaction,
fight a mixed encounter, recover a relic and escape or die. A fresh player should
understand the objective and name one interaction they deliberately exploited.
The current greybox establishes primitives; it does not yet meet this milestone.
