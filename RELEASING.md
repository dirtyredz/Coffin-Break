# Releasing Coffin Break

Repo-wide rules live at the root; this file only covers what is specific to this mod.

- Versioning and archive layout: [12-versioning-and-release.md](https://github.com/dirtyredz/chest-labels/blob/main/12-versioning-and-release.md)
- Visual integration: [10-visual-integration.md](https://github.com/dirtyredz/chest-labels/blob/main/10-visual-integration.md)
- Save safety: [11-mod-data-and-saves.md](https://github.com/dirtyredz/chest-labels/blob/main/11-mod-data-and-saves.md)

Short version on numbering: the version is for players, not a build counter. Bump it only when
publishing, one CHANGELOG entry per release.

## Build a release

```bash
powershell -File pack.ps1
```

Produces `dist/CoffinBreak-<version>.zip`, reading the version from the csproj so the archive
can never disagree with the DLL; `Plugin.cs` derives that same version at build time via
`ModBuildInfo.Version`.

There is no test project. This mod's behaviour is timers, window focus and a Harmony patch
against live game state — none of which a headless runner can assert. The checklist below
carries the weight instead, and it is not optional here in the way it would be for a mod with
tests behind it.

## Pre-release checklist

Root checklist first: [12-versioning-and-release.md](https://github.com/dirtyredz/chest-labels/blob/main/12-versioning-and-release.md).
Then the items specific to this mod:

### The one that matters

- [x] **A real AFK cycle preserves the day.** Note the in-game date and time, leave the game
      running and untouched for longer than a full in-game day would take, come back and
      confirm the date and time are unchanged and no save was written. Everything else on this
      page is detail; this is the mod.

**Verified for 1.0.0 on 2026-08-04.** Together with the earlier confirmations — the plugin
loads, the pause arms, the badge draws in Gelica on the game's own plate, and the Harmony patch
applies without warning — the whole chain is now proven end to end.

Re-run this one after any change to `AfkWatcher`, `DayTimeBlock` or `PassOutGuard`. It is the
only check that tests the actual promise rather than a part of it.

### Arming and disarming

- [ ] Alt-tab away, wait past `FocusLossGraceSeconds`, confirm the badge appears
- [ ] Return to the game and confirm the clock restarts immediately
- [ ] Sit still past `IdleSeconds` with the window focused and confirm it arms
- [ ] Confirm a single keypress, a mouse nudge and a scroll each restart the clock
- [ ] Walk with a controller stick only and confirm it is *not* treated as idle
      (`CountPlayerMovementAsActivity`)
- [ ] Confirm it does **not** arm mid-cutscene, and does arm shortly after one ends

### Not stranding the clock

The failure mode with the worst blast radius — a permanently frozen clock looks like a broken
save, and Serena's Grimoire had to warn users about exactly this with the old
`Time Freeze Hotkey.dll`.

- [ ] Set `Enabled = false` while the mod is holding the clock; confirm time restarts
- [ ] Install alongside Clock Pause and confirm both can hold and release independently
- [ ] Remove the DLL and confirm the clock runs normally on the next launch

### Housekeeping

- [ ] `<Version>` is the single source of truth — `Plugin.cs` derives from it via `ModBuildInfo.Version`, but check the number
      is the one you meant
- [ ] CHANGELOG has one entry for this version
- [ ] `VerboseLogging` defaults to `false`
- [ ] Fresh install: delete `BepInEx/config/com.dirtyredz.moonlightpeaks.coffinbreak.cfg`,
      launch, confirm sensible defaults are written
- [ ] Screenshots show the current build
- [ ] Thumbnail is composed at **16:9** — listing tiles use `object-fit: fill`, so an
      off-ratio image is stretched, not cropped. See [NEXUS.md](NEXUS.md)
- [ ] Archive extracted onto a clean install and verified in game

## Verifying save safety

Easier to argue here than for ChestLabels: this mod has no storage of its own and writes
nothing. Day progression is runtime state, and holding the clock only prevents a write that
would otherwise have happened.

The check is therefore behavioural rather than a file diff — confirm that after a long AFK
period the in-game date is unchanged and the save file's modification time has not moved:

```powershell
$save = "$env:USERPROFILE\AppData\LocalLow\Little Chicken Game Company\Moonlight Peaks\<steam-id>\Saves\<save-guid>\GameData.json"
(Get-Item $save).LastWriteTime
```

Take the timestamp before going AFK and again after returning. It must not have changed.

## Licence

**MIT** — see [LICENSE](LICENSE) at the repo root. Permissive: anyone may use, modify and
redistribute, provided the copyright notice is kept.

Set the Nexus permissions to agree with it, or the page and the licence contradict each other:

| Nexus permission | Set to |
|---|---|
| Upload to other sites | Allowed |
| Convert to other games | Allowed |
| Modify and release | Allowed |
| Use assets in own files | Allowed |
| Include in mod packs / collections | Allowed |

Credit is customary rather than required under MIT. Asking for it in the description is fine;
do not set a permission that MIT already grants.

## Editing note

Do not round-trip these files through `Get-Content -Raw | Set-Content` in PowerShell. It
re-encodes non-ASCII characters and has corrupted em-dashes in this repo twice.

This matters more than usual here: the badge caption in `PauseBadge.cs` contains a real em dash
(`Time paused — away`), and it is on screen in every screenshot of the mod. `pack.ps1` reads
`Plugin.cs` with `Get-Content -Raw` but never writes it back, which is safe.
