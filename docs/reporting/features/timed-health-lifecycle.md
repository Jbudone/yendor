# Timed Health lifecycle tests

Status: **Verified locally and in CI**
Last reviewed: 2026-11-11 UTC
Task: [yen-pmd / issue #8](https://github.com/Jbudone/yendor/issues/8)
Source: tests `14afb5c`, worker `599bf7d`; full verification at `696edb2`.

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
source identity, reproduction commands and limitations. [Fresh full Unity CI](https://github.com/Jbudone/yendor/actions/runs/42957926313) passed at `696edb2`: ten tests, runtime smoke,
asset audit, Linux build and packaged-player smoke. The earlier run
`42954712829` passed tests but failed smoke because its target was already wet;
`yen-0r8` corrected the fixture preconditions. The [follow-up report](../updates/2026-11-11-smoke.md)
records the reproduction and cross-runtime ordering check. Local logs remain
ignored artifacts; hosted artifacts are linked from the CI run while retained.

The launch blocker was diagnosed: this host's npm sandbox hides the home-based
Unity installation. Direct execution of the project Node CLI succeeded; sandbox
configuration was unchanged. Smoke emitted the previously recorded UnityEditor
SearchDatabase startup exception while gameplay assertions completed.

## Remaining acceptance

`yen-pmd` and the smoke-fixture follow-up are verified. Source remains under
review in PR #13; human controls/camera, readability and hardware performance
remain broader foundation gates. No production mutation campaign or manual
feel acceptance is claimed. No playable release has been published.
