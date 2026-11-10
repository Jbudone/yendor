# Timed Health lifecycle tests

Status: **Implemented in worker source; verification blocked**
Last reviewed: 2026-11-10 UTC
Task: [yen-pmd / issue #8](https://github.com/Jbudone/yendor/issues/8)
Source: worker commit `599bf7d`; not included in reporting baseline `b3dc709`.

## Result and scope

Four Unity lifecycle tests were authored to exercise real Awake/Update dispatch
and elapsed engine frames: burn rate and expiry, wet extinguishing without burn
resumption, wet expiry restoring normal Storm damage and ignition, and exactly one
death from lethal burning across later frames and hits.

The fixture uses controlled engine time (`Time.captureDeltaTime = 1/60`) across
actual player-loop frames; it is not a wall-clock performance test.

This is test coverage for the accepted elemental rules, not new gameplay behavior
or a claim of passed runtime coverage. See the
[interaction language](../../../CONTEXT.md#domain-language) and
[foundation gate](../../design/first-expedition.md#slice-target).

## Evidence

The local-only worker commit contains `HealthLifecycleTests.cs` and its `.meta` file. The
coordinator reports four tests copied into the primary checkout as uncommitted
changes. [PR #13](https://github.com/Jbudone/yendor/pull/13) remains unmerged at
this checkpoint; worker source must not be described as integrated release source.

The Unity test launch at `2026-11-10T05:30:58Z` failed with `ENOENT` before runtime
verification. No pass is claimed for the four new tests. The coordinator reports
11 passing Node tests; those do not exercise Unity lifecycle behavior. Run details
remain in Beads and local-only ignored `artifacts/`; no remotely available runtime
result is asserted here. Historical synchronous Health test results in the
[setup record](../../workflow-setup-status.md#verified-evidence) cover different tests.

## Remaining acceptance

Resolve the Unity launch environment, run the four tests against a recorded source
identity, inspect their results and integrate the reviewed source. Update this page
with durable CI evidence when available. No production mutation experiment was run. This reporting page does not close
`yen-pmd` or claim a release.
