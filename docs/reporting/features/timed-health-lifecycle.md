# Timed Health lifecycle tests

Status: **Verified locally; committed**
Last reviewed: 2026-11-11 UTC
Task: [yen-pmd / issue #8](https://github.com/Jbudone/yendor/issues/8)
Source: local commit `14afb5c`; worker source `599bf7d`.

## Result and scope

Four Unity tests exercise actual Awake/Update dispatch and engine frames:
burn rate/expiry, extinguishing without burn resumption, wet expiry restoring
normal Storm damage and ignition, and one death from lethal burning across
later frames and hits. Runtime behavior is unchanged.

The fixture uses `Time.captureDeltaTime = 1/60`; it verifies simulation behavior,
not wall-clock performance or movement feel. Rules remain those in the
[interaction language](../../../CONTEXT.md#domain-language).

## Evidence

All ten Unity tests passed on 2026-11-11, including the four lifecycle tests and
six existing generated-history/oracle cases. Runtime smoke passed on the same
source hash; syntax and 11 Node tests passed, as did staged secret scanning.
The [QA record](../../qa/timed-health-lifecycle.md) preserves exact run IDs,
source identity, reproduction commands and limitations. [Hosted Unity CI](https://github.com/Jbudone/yendor/actions/runs/42954712829)
also passed all ten tests but failed subsequent smoke with “Dry ember hit did not
ignite.” Full local logs remain in ignored artifacts; hosted artifacts are
available from that CI run while retained. The full workflow is not green.

The launch blocker was diagnosed: this host's npm sandbox hides the home-based
Unity installation. Direct execution of the project Node CLI succeeded; sandbox
configuration was unchanged. Smoke emitted the previously recorded UnityEditor
SearchDatabase startup exception while gameplay assertions completed.

## Remaining acceptance

`yen-pmd` is complete locally. The commit is pushed in PR #13 and hosted test evidence is available.
Resolve the CI smoke inconsistency (`yen-0r8`) before integration. No production source mutation experiment, rendered
inspection, player build or manual feel pass was performed. This increment does
not accept the broader foundation milestone or create a playable release.
