# PR-Agent setup and operating status

PR-Agent is installed as a pinned GitHub Action in
`.github/workflows/ai-review.yml`; configuration is in `.pr_agent.toml`.
It remains advisory and cannot merge code. During inspection on November 11,
`YENDOR_AI_REVIEW` and the GitHub `OPENROUTER_API_KEY` secret were absent;
previous review jobs were skipped. No live model review is claimed.

Every eligible PR now gets a no-cost readiness job. The model job runs only
when enabled, prerequisites are present and the PR is from this repository.
It makes reviews only: automatic description rewriting and improvements are off.
No fallback model or application retry is configured; timeout is ten minutes.
These settings are not a dollar cap.

## Activate after selecting an account, model and budget

1. Create a dedicated OpenRouter key with a provider-enforced spending limit.
   Choose an available model ID from that account; use `openrouter/<model-id>`.
   Keep the key out of chat, files and reports.
2. Set the repository Actions secret `OPENROUTER_API_KEY`, and variable
   `YENDOR_AI_REVIEW_MODEL` to that model. Configure the key through GitHub's
   secret UI or `gh secret set OPENROUTER_API_KEY` with secure stdin.
3. After confirming the provider limit, set repository variable
   `YENDOR_AI_REVIEW_BUDGET_CONFIRMED=true`, then `YENDOR_AI_REVIEW=true`.
4. Open or synchronize a small same-repository PR. Require a successful action
   and an actual posted review showing model run details before calling it verified.
   Record provider/model, usage, review URL and findings in the task evidence.
5. To stop future model reviews, set `YENDOR_AI_REVIEW=false`. Do not automatically
   retry ambiguous provider failures. CI readiness and normal tests remain available.

The prior hardcoded model selection was unverified and was removed. Repository
variables now name the selection explicitly. Configuration and the pinned
[action definition](https://github.com/The-PR-Agent/pr-agent/blob/a76cb9111b6a944094e874b3d467c4e546946e7b/action.yaml)
were checked against the upstream [installation guide](https://github.com/The-PR-Agent/pr-agent/blob/a76cb9111b6a944094e874b3d467c4e546946e7b/docs/docs/installation/github.md)
and [model configuration guide](https://github.com/The-PR-Agent/pr-agent/blob/a76cb9111b6a944094e874b3d467c4e546946e7b/docs/docs/usage-guide/changing_a_model.md).
The pinned wrapper uses an upstream runtime container image; this is not a claim
that all transitive container contents are pinned.
