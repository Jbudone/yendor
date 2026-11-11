# Foundation source/reporting review — yen-w06

Coordinator review by Codex, 2026-11-11. This is not an independent worker review
or a posted GitHub approval. No merge, publication or paid review was performed.

## Disposition

No blocking correctness defect found in the reviewed changes. Integrating the
source PR first, then the reporting PR, is supported by the inspected source and
existing evidence. This is a recommendation, not foundation or release acceptance.
Human controls/camera, fresh-player readability and hardware performance remain
pending. Recheck current heads and CI before any actual integration.

| Snapshot | Exact identity |
| --- | --- |
| PR #13 base | `4dc6dac6061dcb129ff285b8b8dbb4d9b2adf391` |
| PR #13 head | `41ea2d43989588181de08c132fe682b65e6db88a` |
| PR #17 head | `cfa6d2629b81d82dbd06b23c4db32932b9168af4` |
| Reporting branch fork | `14afb5cd79f13200ed8f80e3ad76397c7bd25c27` |
| Local preparation source | `3fe3e7f` (adds playtest packet/status only) |
| Trial merged tree (#17 into local preparation) | `514c47b45664394c8fd3ac8ac2acc4895c9caa33` |

The reporting branch's raw head predates the smoke fix. It must not replace the
source branch wholesale or be presented as the exact Unity-tested source. Its
three-way merge preserves the fix with no conflicts. All 67 recorded game files
in both PR #13 and the trial merged tree match the final hosted manifests.
The initial checker incorrectly compared the raw reporting head; that failure
and corrected integration hypothesis are retained in the local attempt record.

## Source review

- `RuntimeSmoke.cs` explicitly sets dry/wet conditions and tests them synchronously,
  so a surface cannot reapply water between setup and the elemental assertion.
  Coordinate ordering normalizes the current distinct positions across runtimes.
  Neither seed generation nor Health/Surface/Expedition gameplay changes here.
- `HealthLifecycleTests.cs` exercises real Awake/Update frames. Four cases cover
  burn rate/expiry, extinguishing without resumption, wet expiry and one-time death
  under later frames/hits. Time settings are saved/restored; advancement has frame
  and real-time failure bounds. Controlled simulation time is not frame-performance
  evidence. The existing six generated-history/oracle cases remain distinct.
- The new test `.meta` identity is present; no existing Unity asset metadata or
  scene/prefab YAML changed. No loss of seed inputs or added death accounting found.
- The Unity workflow only adds a runner-local npm cache. PR events do not execute
  this self-hosted Unity workflow; actual Unity evidence is a manual dispatch at
  the recorded source. Successful Node PR checks alone are not Unity proof.

## Evidence inspected

[Full Unity CI](https://github.com/Jbudone/yendor/actions/runs/42957926313) is
completed/success at `696edb2e9dafa5ace5bcb9b73e65b1eda8f2e9c2`.
The five mode manifests identify source hash
`dae4cf7745e9799bf5b9228b007348164fc71fa39c31df6ef4afcb73c0af2aae`.
Their raw log hashes were recomputed and matched. XML contains ten passed test
cases with zero failures. Editor and packaged-player success markers are present.

The exact ordered seven-position signature for seed 43117 matches local Editor,
hosted Editor and packaged player logs. This is one seed and does not establish
broad generation or input-replay correctness. Player smoke identity and log hash
match build GUID `58e54fd84e464cd8b5b701f3b4fd047a`, bundle SHA-256
`d6f3ba6cf7338190ff6fbfdf1fabfae181498f709c671cb1cb1b5e61207ff298`.
The preceding playtest preparation also rehashed and preserved that full bundle.

Reporting's actual greybox PNG was inspected and its bytes match the PR #17 LFS
object. It is the older explicitly named build `78d3415d530b4caa99a856b08907116d`,
not a capture of the final smoke-fix build. Its Xvfb/software-rendering provenance
and limits are explicit. The roadmap illustration is labelled aspirational concept
art. Archived failure-era reports remain historical; the latest report links the
successful successor. No unsupported human feel/hardware acceptance found.

PR-Agent readiness passed with model review skipped; no live review is verified.
Activation still requires the selected model and provider-capped key in `yen-wco`.
The new readiness/configuration remains outside the gameplay path. The documented
upstream container-pinning limitation remains; no provider or action was invoked
for this review. The known Editor SearchDatabase startup exception remains in logs.

## Integration and next boundary

1. If integration is authorized, merge PR #13 first; preserve the timed tests and
   final explicit-precondition/coordinate-order smoke fixture.
2. Retarget PR #17 to the resulting mainline as needed and review its resulting
   diff. Preserve the source fix; do not replace source with raw reporting head.
3. Require current mechanical checks and the appropriate Unity gate on the actual
   integrated source before claiming integration verified. This review did not
   launch a new Unity command or test an integrated player build.
4. Run [the controls/camera packet](../playtests/foundation-controls.md) on a desktop;
   record observations before selecting combat tuning. `yen-kk5` remains blocked
   on that human evidence. `yen-g8n` owns measured hardware/rendered acceptance.

Local review artifacts: `artifacts/pilots/yen-w06/` contains exact PR JSON/diffs,
`integration.txt`, `verify-evidence.mjs`, `evidence-checks.json` (23 checks passed)
and `attempts.md`. These are existing-evidence consistency checks, not a new Unity
run or an independent playtest. Pre-existing dirty documentation was preserved.
