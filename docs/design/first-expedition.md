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
