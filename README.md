# Arma Reforger Dedicated Server

[![WindowsGSH](.github/assets/windowsgsh-badge.svg)](https://windowsgsh.com)
[![Status](https://img.shields.io/badge/status-needs_live_test-f59e0b)](#status)
[![Module version](https://img.shields.io/badge/dynamic/json?url=https%3A%2F%2Fraw.githubusercontent.com%2FWindowsGSH%2FWindowsGSH.ArmaReforger%2Fmain%2FArmaReforger.mod%2Fmodule.json&query=%24.version&prefix=v&label=module&color=1E8449)](ArmaReforger.mod/module.json)
[![Requires WindowsGSH](https://img.shields.io/badge/dynamic/json?url=https%3A%2F%2Fraw.githubusercontent.com%2FWindowsGSH%2FWindowsGSH.ArmaReforger%2Fmain%2FArmaReforger.mod%2Fmodule.json%3Fbadge%3Dminimum&query=%24.minimumWindowsGshVersion&prefix=v&label=requires%20WindowsGSH&color=2563EB)](ArmaReforger.mod/module.json)
[![Licence](https://img.shields.io/badge/licence-MIT-64748B)](LICENSE.md)

This WindowsGSH module installs, configures, starts, stops, monitors, and backs up the stable Arma Reforger dedicated server.

## Status

**NEEDS LIVE TEST.** The native module uses Bohemia Interactive's current JSON format and passes focused host loading and configuration tests. A fresh current server still needs the checklist below before this becomes a beta candidate.

## Installation

The module installs stable Steam app `1874900` anonymously through SteamCMD and launches `ArmaReforgerServer.exe`.

1. Import the `ArmaReforger.mod` folder or its repository root into WindowsGSH.
2. Add an Arma Reforger server and run Install.
3. Review the scenario, passwords, ports, and gameplay settings before starting it.

The experimental Steam branch/app is not managed by this module.

### Import an existing server

WindowsGSH can import either a normal server installation folder or a WindowsGSM server folder containing `serverfiles`. The preview verifies the server executable, reads supported settings when present, and lets you copy the installation into WindowsGSH or adopt it in place. Review every previewed/defaulted value before completing the import; the source installation is not modified during preview.

## Configuration

WindowsGSH writes `Configs/server.json` and manages:

- server name, join password, in-game admin password, scenario ID, maximum players, and public visibility;
- public/bind game port and A2S query port;
- BattlEye, third-person restriction, and public-server fast validation;
- the recommended maximum server FPS launch limit;
- optional Reforger UDP RCON endpoint configuration; and
- additional launch arguments.

Unknown JSON properties are preserved, including existing mods and advanced `game`, `gameProperties`, and `operating` settings. Writes use a temporary file followed by replacement. JSON comments cannot be preserved. Disabling RCON removes the managed `rcon` object so an old endpoint is not accidentally left active.

The default scenario is `{ECC61978EDCC2B5A}Missions/23_Campaign.conf`. Custom scenarios require an exact, case-sensitive resource ID.

## Networking

| Purpose | Default | Protocol | Exposure |
| --- | ---: | --- | --- |
| Public game traffic | `2001` | UDP | Required for direct internet players; eligible for WindowsGSH UPnP when the server opts in. |
| Server-browser query endpoint | `17777` | UDP | Public server discovery endpoint; eligible for WindowsGSH UPnP when opted in. |
| Reforger RCON | `19999` | UDP | Optional and private by default; WindowsGSH does not request external forwarding. |

Leave Bind Address empty in normal installations. Bohemia recommends this so the server binds all interfaces and detects the appropriate public address. Set it only for an advanced multi-NIC configuration.

Declaring public ports does not automatically forward them. UPnP remains a per-server, opt-in WindowsGSH policy.

## Query, console, and administration

- WindowsGSH reports process status only and does not currently query the endpoint or provide live player counts. A future A2S capability requires a successful current-build fixture and a real client implementation.
- Embedded process output is available through WindowsGSH.
- The in-game admin password is used with Reforger's `#login` command.
- Optional RCON means Arma Reforger's UDP protocol, not Source RCON and not BattlEye RCon.
- WindowsGSH can configure the UDP endpoint for a compatible external client but does not currently implement a Reforger RCON console.
- Enabling UDP RCON requires a password of at least three characters without spaces.

## Files and backups

| Purpose | Path |
| --- | --- |
| Executable | `ArmaReforgerServer.exe` |
| Server configuration | `Configs/server.json` |
| Profile, logs, downloaded addons, and saves | `Saved` |

WindowsGSH backs up `Configs` and `Saved`. The default launch arguments are `-profile .\Saved -maxFPS 60 -config .\Configs\server.json` followed by configured additional arguments.

Stop requests first ask the process window to close and allow ten seconds before the normal bounded forced-stop fallback. During Windows sign-out or shutdown, the module uses the host's graceful-only path and does not force-kill after its timeout.

## Known limitations

- The server's query endpoint and protocol still require live capture; WindowsGSH intentionally makes no A2S capability claim.
- Reforger UDP RCON is configuration-only in WindowsGSH.
- Mods and advanced properties are preserved but do not have dedicated editor fields.
- Existing WindowsGSM imports require a separate live migration test.
- Vendor-required passwords are stored in the game JSON; protect the server directory and support bundles accordingly.

## Beta verification checklist

- [ ] Fresh-install stable Steam app `1874900` and confirm the executable path.
- [ ] Save settings, validate the generated JSON, and confirm mods and advanced properties survive another save.
- [ ] Start the default scenario, confirm the card/PID, restart WindowsGSH, and verify process reattachment.
- [ ] Join from LAN and internet clients; verify UDP `2001` and the purpose/listener behavior of UDP `17777`. Capture a repeatable A2S response before implementing player counts.
- [ ] Test ordinary Stop, application exit, and Windows session ending; confirm world state is flushed.
- [ ] If enabled, test UDP RCON with a compatible Reforger client and confirm it is not forwarded automatically.
- [ ] Test update, Verify Files, crash diagnostics, Server Doctor, UPnP, backup, and restore.

## Support

Report module issues at <https://github.com/WindowsGSH/WindowsGSH.ArmaReforger>. Include the WindowsGSH version, module version, support bundle, and relevant server log lines, with passwords and private addresses removed.

## Support development

If you like the work I do and would like to support continued WindowsGSH and module development, you can contribute here:

- [Ko-fi](https://ko-fi.com/shenniko)
- [PayPal](https://paypal.me/shenniko)

## Trust and source

Modules execute with the same Windows permissions as WindowsGSH. Review `ArmaReforger.mod/module.json`, `ArmaReforger.mod/ArmaReforgerModule.cs`, and [SECURITY.md](SECURITY.md) before installing a build from an unfamiliar source.
