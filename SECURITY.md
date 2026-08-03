# Security policy

## Security and trust

The Arma Reforger module runs with the same Windows user permissions as WindowsGSH. A module can start processes and read or modify files accessible to that account.

WindowsGSH cannot review, endorse, sandbox, or guarantee every module distributed by another person or website. You are responsible for deciding which modules and releases you trust and for protecting the Windows account that runs your game servers.

## Download modules safely

- Prefer this repository and its published releases.
- Check that the repository, module ID, source URL, and expected files agree before importing.
- Review `module.json` and any C# source, scripts, or bundled executables.
- Do not install a module merely because its folder name or artwork looks official.
- Treat unexpected binaries, obfuscated scripts, credential requests, and unrelated network access as warning signs.

## Protect credentials and server data

Game servers may require passwords or tokens in vendor configuration files. Restrict access to server directories and backups. Never post unredacted configuration, support bundles, logs, database files, tokens, passwords, private keys, or personal addresses in a public issue.

Use separate, least-privileged credentials where the game supports them. Rotate any secret that may have been exposed.

## Report a vulnerability

Do not open a public issue for an unpatched vulnerability or include exploit details or secrets in a public discussion.

Report it privately through the [WindowsGSH.ArmaReforger security advisory page](https://github.com/WindowsGSH/WindowsGSH.ArmaReforger/security/advisories/new). If private reporting is unavailable, open a minimal issue requesting a private contact channel without publishing technical details.

Vulnerabilities in the WindowsGSH host application should be reported to the WindowsGSH application repository rather than this module repository.

## Include in a report

Provide:

- affected module and WindowsGSH versions;
- the source and hash of the module package;
- clear reproduction steps and security impact;
- whether the issue requires a particular game-server configuration; and
- redacted logs or a minimal proof of concept.

Do not include live credentials or personal server data.

## Supported versions

Security fixes target the latest published module version unless a repository release notice explicitly states otherwise. Update to the latest trusted release before reporting an issue that may already be fixed.

