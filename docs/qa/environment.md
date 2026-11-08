# Development environment diagnosis — 2026-11-07

## Fixed

The global Codex Unity MCP entry pointed at the repository root (`yendor`),
but the Unity project root is `yendor/game`. Updated it using:

```sh
unity mcp configure codex --project-path /home/jbud/jdrive/jstuff/work/personal/yendor/game --yes
```

Verified with `codex mcp get unity --json`. The configured server command is
`/home/jbud/.local/bin/unity`, with arguments `mcp --project-path` and the
absolute `yendor/game` path.

Added `com.unity.pipeline` version `0.6.0-exp.1` to the Unity manifest. The
CLI installer could not contact packages.unity.com in this session; Unity
successfully resolved the existing cached package and its dependencies.
The package lock now records the installed bridge.

Started a headless Editor for Yendor. A live `unity command editor_status`
request succeeded and returned:

```json
{
  "status": "ready",
  "compiling": false,
  "playMode": "stopped",
  "projectPath": "/home/jbud/jdrive/jstuff/work/personal/yendor/game",
  "unityVersion": "6000.6.0f1"
}
```

Its endpoint was localhost port 7800. This verified the Editor bridge against
Yendor, not the previous project. The chat's native Unity MCP tools were not
loaded during the diagnosis; reconnect/reload them after the config correction.

## Session restrictions still active

- Ordinary local TCP socket creation returned `EPERM`.
- npm DNS lookup returned `EAI_AGAIN`.
- Network access and sandbox escalation are disabled by the active execution policy.
- `.git`, `.codex` and other protected configuration directories are read-only
  through ordinary workspace execution.
- The live Editor PID is not visible inside ordinary sandboxed process inspection.
  Subsequent CLI discovery could not find the instance despite the earlier
  successful live request. Stable discovery needs retesting with the intended
  host access; the path correction alone does not resolve process/network isolation.
- `DISPLAY` and `WAYLAND_DISPLAY` are unset in the command environment, although
  the machine has X11 and Wayland display sockets. A usable graphical session
  still needs to be passed to the executor for visual tests.

The agent cannot change the active session security boundary by editing a
project file. Select the intended permission mode in the app (Full access for
the user's requested unrestricted local development), then reconnect the Unity
MCP server. If the tools do not refresh, restart the app and resume this task.
Managed policy may limit which modes are available.

## Retest before resuming game work

1. Confirm ordinary DNS/HTTPS and localhost access work.
2. Run `unity status --json` and confirm the project is the absolute `yendor/game` path.
3. Run `unity command editor_status --project-path /home/jbud/jdrive/jstuff/work/personal/yendor/game --json`.
4. Verify a native Unity MCP tool also reports Yendor after reconnection.
5. Run npm installation and verify dependency resolution.
6. Verify the executor's graphical environment before attempting rendered captures.

Permission controls: [official sandbox documentation](https://learn.chatgpt.com/docs/sandboxing).
Network configuration: [official security documentation](https://learn.chatgpt.com/docs/agent-approvals-security).
