# The Copper Wild

Enter a forest where ancient machinery has grown into the roots. Follow a distant
turquoise beacon, discover a ruin under the canopy, steal its Starheart, and return
through a world made more dangerous by the theft.

The player should alternate between curiosity and decisive violence. Exploration
exposes routes, resources and clues. Combat turns that knowledge into an advantage.
Speed matters, but a clever interaction can be worth more than accurate shooting.

## Combining the references

| Influence | What we carry forward | First proof |
| --- | --- | --- |
| NetHack / Dungeon Crawl | Interacting rules, consequential items, seedable runs, knowledge as progression | Ignite oil; lure wet enemies into a storm attack; later discover unidentified relic properties |
| Dragonwilds | Inviting landscapes, vertical landmarks, secrets and discovery | Forest-to-ruin reveal, an optional elevated route, a readable destination |
| Deadlock | Movement freedom, rapid aim decisions, abilities that change positioning | Third-person aim, dash, double jump, carbine and elemental abilities |

## Slice target

One biome, one ruin, one loadout, three enemy roles, three interacting elements,
five meaningful pickups and one relic encounter. Fixed authored route first,
then seeded route modules once traversal and combat pacing work. Death resets
run power. Retain player knowledge; decide persistent unlocks after playtesting.

1. **Foundation (current):** compile/build, controls, hitscan combat, hazards,
   objective and replay. Complete runtime checks and a manual control pass.
2. **Combat feel:** acceleration, aim alignment, camera collision, grounded and
   airborne dash tuning, recoil/audio/hit reactions, enemy attack telegraphs,
   ranged enemy and flanker. Gate: a two-minute arena remains fun without loot.
3. **Systemic roguelike:** data-driven items, three relic modifiers, status
   interaction table, deterministic drop streams and explicit run state. Gate:
   two loadouts support visibly different tactics; replay restores the same drops.
4. **Exploration:** traversable route modules, loop/secret/shortcut, map hints,
   relic escalation and extraction pressure. Gate: destination stays legible and
   optional exploration provides a tactical advantage.
5. **Beauty:** URP, atmospheric lighting, terrain, foliage, cohesive materials,
   soundscape and reviewed Meshy assets. Gate: stable 60 fps at 1080p on a named
   test machine; readable enemies and hazards throughout the route.

## Deliberate boundaries

The first slice does not include PvP lanes, survival base building, a seamless
open world or a complete classic roguelike inventory. These may be explored after
the core expedition is compelling. Procedural generation must preserve a valid
path, encounter clearances, spawn safety and a reachable exit for every tested seed.

## Playtest questions

Can a new player reach the relic without verbal guidance? Do they move because
it feels good or only to avoid damage? Can they predict the elemental rules after
seeing them once? Does an optional detour create wonder or merely delay combat?
Does the trip back change after taking the relic? Which death teaches something?

The current greybox lacks relic escalation, rich itemization, mixed enemies,
authored environmental storytelling and production visuals. Measure these as
work remaining, not features implied by the reference games.

## 2026-11-12 — Authorized gameplay batch

The user reported that the integrated foundation feels unchanged and authorized
several substantial gameplay iterations before testing again. Execute fresh
subagents sequentially, reviewing and integrating each chunk before the next.
For this batch, the old manual foundation gate does not block implementation;
manual feel, performance and fresh-player milestone gates remain unpassed.

1. `yen-gh7`: mixed melee, ranged and flanker encounters, dodgeable attack
   telegraphs, hit/kill/damage feedback and visible ability cooldowns.
2. `yen-7a1`: five meaningful seeded pickups, including healing and three
   run modifiers, with visible effects and no persistent inventory. Independent
   random streams preserve world generation; replay restores offerings and clears power.
3. `yen-kn2`: a rewarded authored optional route, navigation landmarks, and
   bounded Starheart escalation that changes the return trip without sealing escape.

These are implementation directions inside the accepted first slice. Specific
balance values are provisional until playtested. Preserve the accepted Ember,
water and Storm rules, same-seed input reproducibility and one death per actor.
Exclude production-art migration, paid authoring calls, persistence and multiplayer.
Completion means an exact playable bundle with automated evidence and a concise
change list; it does not establish subjective fun or rendered hardware performance.

## Roadmap continuation — user authorization after gameplay batch

The user authorized starting the next three outcomes: playtest/tune the new
expedition, assess the first-expedition milestone, and begin art with a bounded
Meshy waystone pilot and rendered baseline. The pilot may run alongside human
acceptance preparation. It permits one preview from existing credits; review
precedes refinement, additional assets and production import. Broader Beauty
work follows gameplay acceptance and baseline evidence. See
`docs/playtests/expedition-acceptance.md`; human and hardware gates remain open.

## First presentation integration — provisional

After the authoring pilot, the user authorized proceeding with existing-asset
review and integration, with zero new paid calls. `yen-rgi` adds the reviewed
ring-marker waystone beside the existing extraction pad, uses the sandstone
study as paving rather than on the monolith, and plays the generated cue on
successful extraction only. Existing route, objective positions, seeded input
streams and combat rules remain the governing contracts. Authoring is native
Editor C#; source FBX and original studies are preserved.

This is provisional presentation, not final style or audio acceptance. Native
compile/resource audit and scripted runtime checks pass. The initial package-driven source-gate failure was resolved by removing the
unused optional inference addon. Exact-source 26 tests, asset audit, Linux build,
Editor smoke and packaged-player smoke pass; the art-equipped player is published. See
`docs/qa/waystone-integration.md` for evidence and next boundary.
