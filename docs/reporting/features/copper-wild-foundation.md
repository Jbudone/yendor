# Copper Wild foundation

Status: **Implemented; historical CI verified; manual acceptance pending**
Last reviewed: 2026-11-10 UTC
Task: Foundation follow-ups remain in Beads; reporting task `yen-ei3`.
Source: historical verified checkpoint `6700bfe`; reporting baseline `b3dc709`.

## Result and scope

The Unity greybox supports a third-person expedition from the waystone to the
Starheart and back: double jump, dash, carbine, Ember and Storm, pursuing enemies,
oil/water interactions, extraction or death, and same/next-seed restart.
See [controls](../../../README.md#play-the-greybox) and the
[accepted foundation gate](../../design/first-expedition.md#slice-target).

This establishes primitives. Placeholder geometry, limited encounters and missing
relic escalation/itemization mean the full 5–10 minute milestone is not accepted.

## Evidence

The [successful Unity CI run](https://github.com/Jbudone/yendor/actions/runs/42579329482)
at source `6700bfe` completed verification, Linux build, packaged-player smoke and
artifact upload. The [setup evidence record](../../workflow-setup-status.md#verified-evidence)
documents the checks and earlier shader-retention fix. The
[Node/Gitleaks run](https://github.com/Jbudone/yendor/actions/runs/42576852702)
passed at setup commit `4dc6dac`.

These are historical checks tied to their recorded sources. They do not prove
later source, rendered quality or movement feel. Earlier local logs under ignored
`artifacts/` are local-only; [foundation QA](../../qa/foundation.md) records their
scope and limitations, with newer integration evidence in the setup record.

## Remaining acceptance

Verify the integrated current source and perform the manual camera, aiming, dash,
double-jump, restart and encounter pass. Rendered inspection and fresh-player
readability remain pending. Public references:
[foundation playtest / yen-kk5](https://github.com/Jbudone/yendor/issues/4) and
[rendered/performance baseline / yen-g8n](https://github.com/Jbudone/yendor/issues/5).

The [timed Health tests](timed-health-lifecycle.md) passed local Unity verification
and runtime smoke on a recorded source snapshot. Broader integrated CI and manual
acceptance remain pending. No playable GitHub Release is claimed by this page.
