import { exec, root } from './workflow/core.mjs';
await exec('git', ['config', '--local', 'core.hooksPath', '.githooks'], { cwd: root });
console.log('Hooks enabled: fast pre-commit checks; opt-in Unity pre-push via YENDOR_PREPUSH_UNITY=1.');
