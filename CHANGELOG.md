# Changelog — Coffin Break

Written for us. The player-facing wording lives in [NEXUS.md](NEXUS.md); this file names
patches, ids and the things that were tried and dropped.

One entry per **released** version, not per build — see
[12-versioning-and-release.md](https://github.com/dirtyredz/chest-labels/blob/main/12-versioning-and-release.md).

## 1.0.1

### Fixed

- The badge announced "time paused" while sitting in a menu, where the clock was already
  stopped by the game — telling the player something they could plainly see. `DayProgresser`
  registers its blocker under `DayProgresser_dayProgressBlocker`, and `Blocker` keeps a static
  registry keyed by that name, so `Blocker.Get(...).Ids` gives the live list of everyone
  currently holding the clock. The badge now stays quiet whenever an id other than ours is in
  it. Covers the pause menu, decorate mode, inventory and chest screens, and any other time
  mod, without needing to know a single screen type.

  Only the caption is suppressed — the hold itself is deliberately unconditional. Skipping the
  hold would leave a frame where the menu closes, its blocker lifts and the clock runs while
  the player is still away, which is the whole thing this mod prevents.

### Added

- `HideBadgeWhenAlreadyPaused` (default `true`) — switches the above off for anyone who wants
  the badge whenever the mod is holding the clock, regardless of who else is.

  Existing config files keep their values on upgrade and simply gain this key at its default;
  see the config-upgrade notes in
  [12-versioning-and-release.md](https://github.com/dirtyredz/chest-labels/blob/main/12-versioning-and-release.md).

## 1.0.0

Published as [Nexus mod 121](https://www.nexusmods.com/moonlightpeaks/mods/121) on
2026-08-04.

First release. Built and iterated as `0.1.0` under the working name **AFK Guard**; renamed to
**Coffin Break** before publishing and collapsed into this single entry.

### Added

- Idle detection via `UnityEngine.Input` — `anyKey`, mouse delta, scroll wheel. Must be
  fully qualified: the game has its own global `Input` type that otherwise wins the lookup.
- Player-position fallback (`CountPlayerMovementAsActivity`) so analogue-stick walking counts
  as activity. Legacy input sees gamepad buttons but not sticks, whose axes belong to Rewired.
- The clock is held with `DayProgresser.AddDayTimeBlocker("com.dirtyredz.coffinbreak")` —
  the game's own mechanism, also used by decorate mode and the pause menu. `Blocker.Add` is
  distinct-keyed, so this composes with Clock Pause, TimeControl and Serena's Grimoire rather
  than fighting them.
- Focus-loss arming (`PauseOnFocusLoss`), with a grace period so a notification stealing focus
  for an instant does not flicker the clock.
- Harmony **postfix** on `GameDefaultState.IsPassOutNeeded`, returning false while the mod
  holds the clock. Closes the race where the day ends between going idle and the pause
  engaging. Postfix rather than prefix so the game's own refusals still run first; it can only
  turn `true` into `false`.
- Badge on the mod's own canvas at sorting order 600, using `GameFonts` / `GamePalette` /
  `PanelSprite` copied from ChestLabels.

### Decided against

- **Rewired for input.** Correct in principle — it is the game's own input layer — but it
  needs a player id and an action map a patch could rename. An idle detector that wrongly
  believes you are present is a mod that silently does nothing, so the dumber, unbreakable
  API won.
- **`GamePersistence.AddSaveBlocker`.** It exists and would also stop the overnight write, but
  blocking a save the game has decided to make is a much larger promise than stopping a clock.
  Removing the *reason* for the save is the smaller, safer intervention.
- **Arming during cutscenes.** `PauseDuringCutscenes` exists but defaults off and is documented
  as "leave it off": the game's scripted waits run on day progression
  (`DayProgresser.WaitForSeconds` spins while `IsDayProgressionPaused`), so freezing the clock
  mid-cutscene can stall the scene rather than protect it.

### Fixed during development

- Badge font size was only re-applied when the caption text changed, so with
  `ShowPausedDuration` off a size edited in Mod Menu would never have taken effect.
- The blocker is released in `OnDestroy`, so unloading cannot strand a held clock — the
  failure Serena's Grimoire had to warn users about with the old `Time Freeze Hotkey.dll`.
