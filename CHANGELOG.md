# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.1] - 2026-08-15

### Fixed
- README images and GIFs not rendering on Thunderstore: replaced relative `docs/...` paths with absolute `raw.githubusercontent.com` URLs, since the published package does not include the `docs/` folder.

## [1.0.0] - 2026-08-14

### Added
- Configurable base Health, Stamina, and Eitr values, fully compatible with vanilla food bonuses.
- Optional extra regeneration for Health, Stamina, and Eitr, independent of food and Comfort.
- Optional infinite Stamina and infinite Eitr, without disabling vanilla actions, skills, or animations.
- Fixed-width HUD bars for Health, Stamina, and Eitr.
- Always-show option for the Stamina and Eitr bars.
- Server-authoritative gameplay settings in multiplayer, synced through Jötunn.
- Enforcement of matching mod and version between server and clients through Jötunn's network compatibility check.
- Local, per-player HUD preferences that are never synced from the server.

### Verified before release
- Single-player: all settings apply locally, with no server involved.
- Dedicated server, as a regular player without admin rights: gameplay settings correctly follow the server, both when raised and lowered relative to the client's own configuration.
- Dedicated server, disconnect and reconnect: local settings return immediately on disconnect and are correctly overridden again on reconnect.
- Clients missing the mod are blocked from joining a modded server, with a clear in-game message naming the missing mod.
- HUD settings stay local to each player in every multiplayer scenario tested, regardless of the server's own HUD configuration.
- Vanilla food bonuses, food decay, food-based regeneration, and skill experience gain all behave as in an unmodded game.
- No measurable performance impact: all patches piggyback on existing game update cycles, with no per-frame polling and no per-frame allocations.