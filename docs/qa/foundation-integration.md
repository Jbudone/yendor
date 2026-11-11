# Foundation integration candidate — yen-7mm

Coordinator Codex, direct execution, started 2026-11-11 05:47 UTC.
Checkpoint 06:17 UTC; paid calls zero. This increment prepares a draft candidate;
mainline merge and manual foundation acceptance remain separate boundaries.

Combined local preparation/review head `090a40c` (includes source PR #13 head
`41ea2d4`) with reporting PR #17 head `cfa6d26`. The conflict-free merge is
`7a00536f0e9a55f5e4bb38b79f3ff925a9dd6018`, tree
`f8bfa0dce5a1973b93e22d0613fdf5e40393600e`.
All 67 game files match the prior passing hosted manifest. The reporting branch
was merged into the source branch; its older source never replaced the smoke fix.
No gameplay, scene or asset metadata changes were introduced by integration.

`npm run check`, `npm test` and redacted Gitleaks history scan passed on the
combined candidate. Initial Node run skipped the Gitleaks fixture because the
worktree lacked its ignored tool binary; after linking the existing local binary,
the repeated run passed all eleven tests, with zero skipped. Full local logs,
verification script and identity are in `artifacts/pilots/yen-7mm/` in the primary
checkout. The primary checkout's pre-existing dirty documents were preserved.

Full hosted Unity verification on this integrated candidate is pending. Do not
infer it from earlier source evidence or passing Node checks. The controls/camera
playtest (`yen-kk5`), hardware/rendered baseline (`yen-g8n`) and fresh-player
diagnostic remain unrun. No paid provider call or mainline merge is authorized
by this report.
