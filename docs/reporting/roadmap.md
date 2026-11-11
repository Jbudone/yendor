# Roadmap

[Discuss and influence priorities on the pinned roadmap issue](https://github.com/Jbudone/yendor/issues/15).
Comment, react to feature issues, or subscribe for updates. Beads remains the task authority.


The goal is a readable 5–10 minute Copper Wild expedition: choose a route,
exploit an interaction, fight a mixed encounter, recover the Starheart and
escape or die. The [first expedition design](../design/first-expedition.md)
owns scope, sequence and acceptance criteria. This page summarizes that direction;
Beads owns task priority, claims and execution state.

| Milestone | Observable gate | Current position |
| --- | --- | --- |
| Foundation | Working controls, combat, hazards, objective and seed replay; runtime checks and manual control pass | Greybox implemented; [historical CI evidence](features/copper-wild-foundation.md), current-source and manual acceptance pending |
| Combat feel | A two-minute arena stays fun without loot | Next milestone after foundation acceptance |
| Systemic roguelike | Two loadouts enable different tactics; seed replay restores drops | Planned design direction |
| Exploration | Legible destination; optional routes provide tactical advantage | Planned design direction |
| Beauty | Readable enemies/hazards and stable 60 fps at 1080p on a named machine | Planned design direction |

The [feature index](README.md) records individual outcomes. The completed local
foundation verification increment is [timed Health lifecycle coverage](features/timed-health-lifecycle.md),
tracked by [yen-pmd / issue #8](https://github.com/Jbudone/yendor/issues/8).
The broader slice still needs richer enemies, itemization, relic escalation and
production visuals. No dates or playable-release claim are implied by this sequence.

The coordinator updates this roadmap and affected feature pages at completed
outcomes or blockers. Direction changes must first be accepted in the design or
an ADR; reporting must not silently create scope. See the [convention](template.md).

Last reviewed: 2026-11-11 UTC, verification task `yen-pmd`.
