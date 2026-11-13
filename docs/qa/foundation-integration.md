# Foundation integration candidate — yen-7mm

Coordinator Codex, direct execution, started 2026-11-11 05:47 UTC.
Checkpoint 06:17 UTC; paid calls zero. This increment prepares a draft candidate;
mainline merge and manual foundation acceptance remain separate boundaries.
Final Git action approval delayed publication until 06:49 UTC, beyond the
checkpoint. No new subsystem was started; the remaining work was recording results.

Combined local preparation/review head `090a40c` (includes source PR #13 head
`41ea2d4`) with reporting PR #17 head `cfa6d26`. The conflict-free merge is
`7a00536f0e9a55f5e4bb38b79f3ff925a9dd6018`, tree
`f8bfa0dce5a1973b93e22d0613fdf5e40393600e`.
All 67 game files match the prior passing hosted manifest. The reporting branch
was merged into the source branch; its older source never replaced the smoke fix.
No gameplay, scene or asset metadata changes were introduced by integration.

`npm run check`, `npm test` and redacted Gitleaks history scan passed on the
combined candidate. npm's sandbox hides the worktree's external binary symlink,
so those npm runs passed ten tests with one Gitleaks fixture skipped. Direct Node
and commit-hook runs passed all eleven tests, with zero skipped. Full local logs,
verification script and identity are in `artifacts/pilots/yen-7mm/` in the primary
checkout. The primary checkout's pre-existing dirty documents were preserved.

Full [hosted Unity verification](https://github.com/Jbudone/yendor/actions/runs/42969260953)
passed at exact integrated revision `8437f9183e1398c15fc113de6a4862d5fadff9c6`:
validation, ten Unity tests, Editor smoke, asset audit, Linux build, packaged-player
smoke and artifact upload. Local inspection passed 23 evidence checks: five
manifest results/source identities and log hashes, committed game bytes, ten XML
cases, smoke markers, player identity/log and the downloaded full bundle hash.
Full artifacts and CI log are preserved under `artifacts/pilots/yen-7mm/`.
Subsequent changes to this report and STATUS are documentation only; the actual
Unity-tested revision remains explicit above. No Unity rerun is claimed for them.

[Draft integration PR #19](https://github.com/Jbudone/yendor/pull/19) is a combined
alternative to merging stacked PRs #13/#17 individually. It includes the local
playtest packet and coordinator review. No mainline merge or superseding closure
has been performed. Current disposition: automated integration evidence passes;
the user can choose the combined candidate or original ordered PRs for merge.
The controls/camera playtest (`yen-kk5`), hardware/rendered baseline (`yen-g8n`) and
fresh-player diagnostic remain unrun. The known Editor SearchDatabase exception
is still a limitation of headless startup logs, not a new gameplay acceptance.
Paid calls zero. No accepted design changes.

## Integration disposition — 2026-11-12

The user's “please proceed” as project orchestrator continued the recommended
combined path. PR #19 merged at `12cc7c9addc3e4e67ee334bc31c74ab4884bd7e7`
on 2026-11-12 at 03:04:59 UTC. Its tree equals reviewed head `196cef3`;
game files equal the earlier Unity-tested candidate. This resolves the preparation
boundary above. Original PRs were left untouched.

Post-merge [Node checks](https://github.com/Jbudone/yendor/actions/runs/43145761860)
and [Unity verification](https://github.com/Jbudone/yendor/actions/runs/43145762111)
passed on the exact merge revision. Unity includes validation, tests, Editor smoke,
asset audit, Linux build, packaged-player smoke and artifact upload. Full CI logs
and run metadata are saved as `artifacts/pilots/yen-7mm/post-merge-{node,unity}.{log,json}`;
`integration-merge.json` records identity and disposition. No new independent
bundle audit or human acceptance is claimed. Coordinator Codex executed directly,
03:04–03:08 UTC before the 03:34 checkpoint; paid calls zero. The next gate is the
prepared human desktop controls/camera pass, with hardware timing and fresh-player
diagnostic still separate. Primary dirty documents and existing worktrees remain.
