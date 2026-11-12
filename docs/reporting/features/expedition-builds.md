# Combat, run builds and the Starheart return

Status: **Implemented; automated verification passes; human acceptance pending**
Last reviewed: 2026-11-12 UTC
Tasks: `yen-gh7`, `yen-7a1`, `yen-kn2`.
Source: workers `9142a4d`, `efcef8d`, `56588bc`; integrated at `7ce4e19`.

## Result and scope

Three enemy roles change positioning: brutes wind up melee, spitters warn before
committed aimed shots, and flankers approach off-axis. Hit/kill/hurt cues, health
bars, procedural combat audio and ability countdowns make the fight easier to read.

Choose one seeded shrine core: Needle strengthens slower carbine shots, Cinder
favors rapid Ember, and Tempest extends and accelerates Storm. Each has a tradeoff.
Salve heals; Heartwood increases maximum life. Powers and consumed pickups reset
on replay, while the same seed restores physical offerings and enemy positions.

An optional eastern overlook has broad ramps, firing cover and supply caches.
Gold and turquoise spires identify relic and extraction. Taking the Starheart
adds exactly three mixed hunters and changes the objective to return; the exit
stays open. F8 records build, core, phase and alarm and survives scene reloads.

This implements the [authorized gameplay batch](../../design/first-expedition.md#2026-11-12--authorized-gameplay-batch).
It retains placeholder art and provisional balance; production visuals,
persistence, multiplayer and human milestone acceptance remain outside this batch.

## Evidence

Twenty-six Unity tests pass, including actual collider/attack behavior, real
weapon and core effects, route walking in both directions, collection/extraction
priority, single death, and real same/next-seed scene reload. Validation and
runtime smoke pass on the same final worker source. Coordinator audited actual
logs, XML, game-file hashes and unique asset identities. Full run IDs and retained
failures are in the [QA record](../../qa/gameplay-batch.md).

Final build and inspected captures are recorded in the
[gameplay report](../updates/2026-11-12-gameplay.md). Local artifact paths in those
records are local-only; they do not imply a public download or a GitHub Release.

## Remaining acceptance

Try [one expedition](../../playtests/gameplay-batch.md) and report what changes
your decisions, what feels flat and any disruptive failure. Combat feel, input
readability, subjective exploration reward, named-hardware timing and the
three-fresh-player diagnostic remain unpassed. Automated physics and software
rendering establish different evidence from those human gates.
