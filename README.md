# Yendor

A third-person expedition roguelike shooter: systemic discovery, inviting wilds,
and fast movement combat. Built with Unity; developed with a Mastra supervisor,
specialist agents, LiteLLM, Unity AI and Meshy.

Start with [the shared context](CONTEXT.md), [the first expedition design](docs/design/first-expedition.md)
and [the agent architecture](docs/adr/0001-development-agents.md).

Follow the [roadmap](docs/reporting/roadmap.md) and
[feature reports](docs/reporting/README.md) for progress, verification evidence
and remaining acceptance gates. Feature reports are maintained in GitHub with
the source; [GitHub Releases](https://github.com/Jbudone/yendor/releases) are
reserved for verified playable checkpoints.

## Working together

Open this project in Codex and say **“Pick up Yendor”** or name a Beads task.
`AGENTS.md` supplies startup instructions; `STATUS.md` links current work.
Use a fresh thread for a new task and keep the same thread for connected work.
The coordinator maintains decisions and handoffs so you do not need to repeat
project history. See [the workflow guide](docs/workflow.md) and
[setup status](docs/workflow-setup-status.md).

```sh
npm ci
npm run bootstrap                  # pinned Linux x64 Beads, Agent Mail, Gitleaks
npm run hooks:install              # staged/history Gitleaks and fast checks
npm start                         # local mail and current tasks
npm run workflow -- task ready --json
npm run workflow -- recall "seed replay"
npm run agents -- --task-id yen-ID --single combat "A bounded task"
```

On a fresh clone restore the shared task graph with `.tools/bin/bd bootstrap
--yes`, then run `npm run tasks:seed` only to add missing initial tasks.
Existing checkouts retain their database. Never reinitialize a populated store.
Gitleaks runs on commits, pushes and CI. Full-history scanning must pass before
public publication. Keys belong only in ignored `.env`; account activation and
Unity licensing remain local setup steps.

## Play the greybox

Open `game/` with Unity **6000.6.0f1**, open
`Assets/Yendor/Scenes/CopperWild.unity` and press Play. The scene generates the
forest, ruins and encounter when play starts. Recover the Starheart at the far
ruin and return to the turquoise waystone. Press E at both destinations.

WASD moves, mouse aims, Space double jumps, Shift dashes, LMB fires the carbine,
RMB casts ember, Q casts storm, E interacts. Escape releases the mouse; click
to capture it again. After death or escape, R replays the seed and N uses the
next seed. Ember ignites oil; water extinguishes fire and doubles Storm damage.

This is placeholder geometry and an initial combat loop. Deep itemization,
procedural dungeon layouts, richer encounters and production visuals are next
milestones. See the design document for acceptance criteria.

## Development studio

Requires Node **22.18+**. On a machine with npm registry access:

```sh
npm install
cp .env.example .env
npm run doctor
```

Set LiteLLM credentials and model aliases in `.env`. Start your existing LiteLLM
gateway, or install `litellm[proxy]` in a separate Python virtual environment and
run it with `infra/litellm/config.yaml`. Load `.env` into the gateway process and
set all four upstream model IDs and the relevant provider keys. Use the same
gateway key as `LITELLM_API_KEY`; for a local personal gateway this can equal
`LITELLM_MASTER_KEY`. The gateway should expose `/v1/chat/completions` and support
tool calls on the selected models. No specific provider or model subscription
is assumed.

```sh
npm run agents -- "Review the current movement controller and propose one improvement"
npm run agents -- "Add a ranged revenant with clear windup and ask QA to validate compilation"
```

The supervisor delegates to combat, systems, world, art, design and QA specialists.
Each has an independent `MODEL_*` alias. Tools can read project context, edit
owned C#/JSON/Markdown files and run fixed Unity validation. Runs write task,
edit/validation events and results under `.runs/`. Console traces expose model
and tool behavior. Run one supervisor at a time; edits affect the local workspace.
Keep the Unity Editor closed during batch validation. Step and time limits bound
execution; set financial limits in your model gateway/provider accounts.

Set `MESHY_API_KEY` locally to use Meshy. Begin with the free balance check;
`preview` submits a paid generation. See [the art pipeline](docs/art/pipeline.md).
Unity AI is activated in the Editor against your linked Cloud project; it is not
a generic model endpoint behind LiteLLM. Keep all keys outside Unity Assets.

## Checks

```sh
npm test
npm run unity:validate
npm run unity:build
```

Unity paths can be overridden with `UNITY_EDITOR_PATH`. The build target is
Linux under `game/Builds/Linux/`. In a restricted agent environment, use the Unity
executable directly if a Node-spawned Editor cannot reach licensing IPC:

```sh
/path/to/Unity -batchmode -nographics -projectPath "$PWD/game" -executeMethod Yendor.Editor.ProjectSetup.Validate -quit -logFile /tmp/yendor-validation.log
/path/to/Unity -batchmode -nographics -projectPath "$PWD/game" -executeMethod Yendor.Editor.ProjectSetup.Build -quit -logFile /tmp/yendor-build.log
/path/to/Unity -batchmode -nographics -projectPath "$PWD/game" -executeMethod Yendor.Editor.ProjectSetup.Smoke --yendor-smoke -logFile /tmp/yendor-smoke.log
```

Smoke runs intentionally omit `-quit` so Unity can enter play mode and exit after
the assertions. A development player also supports `--yendor-smoke`. These checks
cover runtime initialization, elemental damage, death counting and the objective
cycle. They do not replace manual movement, camera, encounter and visual testing.

See [validation status](docs/qa/foundation.md) for what has actually been checked.
