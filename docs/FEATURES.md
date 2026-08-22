# FEATURES — Coffin Break

What the mod does. Status: ✅ shipped · 🚧 in progress · 💡 idea.

Current release: **v1.0.1** ([Nexus mod 121](https://www.nexusmods.com/moonlightpeaks/mods/121)).

## AFK clock protection

| Feature | Status | Config | Notes |
|---------|:------:|--------|-------|
| Stop clock on window focus loss (alt-tab) | ✅ | `PauseOnFocusLoss`, `FocusLossGraceSeconds` | The common case; also covers minimise/suspend |
| Stop clock on input idle | ✅ | `IdleSeconds` | Keyboard, mouse, gamepad buttons |
| Count character movement as activity | ✅ | `CountPlayerMovementAsActivity` | Controller safety net (analogue-stick gap) |
| Veto the 2am pass-out while held | ✅ | `BlockPassOutWhilePaused` | Harmony postfix; closes the end-of-day race |
| Restart clock instantly on any activity | ✅ | — | Regaining focus counts as activity |
| Master enable/disable | ✅ | `Enabled` | Off leaves the game completely untouched |

## Badge / feedback

| Feature | Status | Config | Notes |
|---------|:------:|--------|-------|
| "Time paused — away" badge | ✅ | `ShowBadge` | Own canvas, game's Gelica font + plate |
| Show elapsed away time | ✅ | `ShowPausedDuration` | s / m / h |
| Configurable corner | ✅ | `BadgePosition` | Live, no restart |
| Configurable font size | ✅ | `BadgeFontSize` | Live |
| Stay quiet when another holder is active | ✅ | `HideBadgeWhenAlreadyPaused` | Menu/decorate/other time mods |
| Verbose arm/disarm logging | ✅ | `VerboseLogging` | Diagnostics |

## Integration

| Feature | Status | Notes |
|---------|:------:|-------|
| Composes with other time mods | ✅ | Distinct-keyed `Blocker` id; no clobbering |
| Mod Menu / ConfigurationManager sections & labels | ✅ | Section titles via `ConfigDescription` tags |
| Read-only w.r.t. saves | ✅ | Nothing written; runtime state resumes |
| Releases the clock on unload | ✅ | `OnDestroy`; can't leave the clock frozen |

No known missing capabilities — the mod is feature-complete for its scope. Ideas live in
[ROADMAP.md](ROADMAP.md); structural cleanups in [BACKLOG.md](BACKLOG.md).
