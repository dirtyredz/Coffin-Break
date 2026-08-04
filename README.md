# Coffin Break

Stops the in-game clock when you walk away, so going AFK never costs you a day.

**Status:** v1.0.0 — verified end to end, ready to publish. See [RELEASING.md](RELEASING.md).

Confirmed on 2026-08-04: the plugin loads, the pause arms, the badge draws in the game's own
Gelica on the game's own plate, the Harmony patch on `IsPassOutNeeded` applies without warning,
and **a real AFK cycle preserves the day** — the result the mod exists for.

**Nexus title:** `Coffin Break - AFK Auto-Pause`

The pun carries the page; the subtitle carries the search. A sweep of all 88 Moonlight Peaks
mods (2026-08-04) found `afk` and `idle` each returning **zero** results, so the first mod to
put either word in its title owns that search outright — but "Coffin Break" is what someone
repeats in a Discord. The scene already uses this shape: *Far Sight (Scroll out further)*,
*Endless Harvest - Plant on your schedule*, *Museum Flexible Donation (Remove Required
Quality)*.

Internals keep the literal names — `AfkWatcher`, `PauseBadge` — because a joke in a type name
stops being funny the second you are debugging at 2am. The joke lives on the mod page.

## The problem

Go AFK with the game running and the clock keeps moving. It reaches 2am, your character
passes out, the day advances and the game saves. You come back to a day you did not play
and a save you cannot undo.

## What it does

Three layers, in order of how often each one saves you:

1. **Focus loss** — alt-tab away and the clock stops after `FocusLossGraceSeconds` (1.5s).
   This is the common case and the cheapest to detect.
2. **Idle timer** — no keyboard, mouse or controller input for `IdleSeconds` (60s) and the
   clock stops. Covers walking away from a focused window.
3. **Pass-out veto** — while the clock is held, the 2am collapse is refused outright.

Touch anything and the clock restarts immediately.

## How it works

### The clock

`DayProgresser` keeps a `Chicken.Utilities.Blocker` of named string ids and stops advancing
the day while any id is present:

```csharp
public void AddDayTimeBlocker(string id)     // DayProgresser
public void RemoveDayTimeBlocker(string id)
public bool IsDayProgressionPaused => dayProgressBlocker.IsBlocked;
```

This is the game's own mechanism — decorate mode, the debug scrubber and the pause menu all
use it. `Blocker.Add` is distinct-keyed, so ids from different mods coexist and neither can
clobber the other. Ours is `com.dirtyredz.coffinbreak`.

`Update()` skips `ProgressTime`, the delayed room-load events and `NpcWalker.ProgressNpcs`
while blocked, so this is a real pause of world time, not a cosmetic one.

**Nothing is written to the save.** Day progression is runtime state that resumes where it
left off. See [11-mod-data-and-saves.md](https://github.com/dirtyredz/chest-labels/blob/main/11-mod-data-and-saves.md).

### Why the day is lost

`GameDefaultState`:

```csharp
public static bool IsPassOutNeeded
{
    get
    {
        if (Cutscene.IsInCutscene) return false;
        if (AddressableLibrary<VariableLibrary>.Instance.IsPassOutPrevented.GetBoolValue()) return false;
        if (GamePersistence.Instance.Time.DayProgression < 1f) return false;
        return true;
    }
}
```

Reaching `1f` starts `PassOutRoutine`, which dispatches `OnTimeToFaint` — day advances, game
saves. A held clock never reaches `1f`, so the primary fix needs no patch at all.

`PassOutGuard` adds one Harmony **postfix** on this getter that returns false while Coffin Break
holds the clock. It exists only for the split second between the day ending and the pause
engaging — go idle at 1:59am with a 60-second timer and layer 1 and 2 are both too slow.
Postfix, not prefix, so the game's own reasons to refuse still run first and are never
overridden; it can only turn `true` into `false`, never the reverse.

### Detecting idle

`UnityEngine.Input` (legacy module, which the game ships), not Rewired. Rewired needs a
player id and an action map that a patch could rename; `Input.anyKey` cannot break that way,
and an idle detector that wrongly thinks you are present is a mod that silently does nothing.

`Input` must be written as `UnityEngine.Input` — the game has its own global `Input` type
that otherwise wins the name lookup.

Legacy input sees gamepad **buttons** but not analogue stick movement, because sticks are
axes and this game's axes belong to Rewired. That gap is covered by
`CountPlayerMovementAsActivity`, which watches the character's world position — a truer
signal of presence than any button.

## Deliberate limits

- **Cutscenes.** `PauseDuringCutscenes` defaults off, and should stay off. The game's own
  scripted waits run on day progression (`DayProgresser.WaitForSeconds` spins while
  `IsDayProgressionPaused`), so freezing the clock mid-cutscene can stall the scene rather
  than protect it. Coffin Break arms on the first frame after the cutscene ends.
- **No save blocker.** `GamePersistence.AddSaveBlocker(string)` exists and would be another
  way to stop the overnight write, but blocking a save the game has decided to make is a
  larger promise than stopping a clock. Preventing the pass-out removes the reason for the
  save instead.
- **Residual window.** With `BlockPassOutWhilePaused` off, going idle within `IdleSeconds` of
  2am can still lose the day. On (the default), it cannot.
- **Not a substitute for pausing.** This protects the day. It does not stop a hostile
  encounter or a timed quest that has its own clock.

## Config

`BepInEx/config/com.dirtyredz.moonlightpeaks.coffinbreak.cfg`, or in-game via
[Mod Menu](https://www.nexusmods.com/moonlightpeaks/mods/102) / ConfigurationManager.

| Key | Default | Notes |
|---|---|---|
| `Enabled` | `true` | Off leaves the game entirely untouched |
| `IdleSeconds` | `60` | Below ~10s it stops while you read a dialogue box |
| `PauseOnFocusLoss` | `true` | The alt-tab case |
| `FocusLossGraceSeconds` | `1.5` | Stops flicker when a notification steals focus |
| `PauseDuringCutscenes` | `false` | See above — leave off |
| `BlockPassOutWhilePaused` | `true` | Closes the end-of-day race |
| `CountPlayerMovementAsActivity` | `true` | The controller safety net |
| `ShowBadge` | `true` | |
| `HideBadgeWhenAlreadyPaused` | `true` | Stay quiet in menus, where the clock is already stopped |
| `BadgePosition` | `TopCentre` | Clear of the clock, toolbar and Detailed Minimap |
| `BadgeFontSize` | `26` | |
| `ShowPausedDuration` | `true` | How long you were gone |
| `VerboseLogging` | `false` | Logs every arm/disarm with its reason |

`BadgePosition` is re-applied every frame, so it can be changed live without a restart.

## The badge

On its own canvas, which per [10-visual-integration.md](https://github.com/dirtyredz/chest-labels/blob/main/10-visual-integration.md) is
the case that inherits nothing — so font, colour and shape are all set explicitly from the
game's assets (`GameFonts`, `GamePalette`, `PanelSprite`, copied from ChestLabels). Sorting
order 600, above Plant Peek's hover at 500, because a message about the clock being stopped
is useless if something covers it.

It stays quiet when someone else has already stopped the clock — a menu, decorate mode, an
open inventory, another time mod. `DayProgresser` names its blocker
`DayProgresser_dayProgressBlocker` and `Blocker` keeps a static registry keyed by that, so
`Blocker.Get(...).Ids` is the live list of everyone holding the clock; any id but ours means
the player can already see time is stopped and does not need telling.

**The hold is not conditional, only the caption is.** Skipping the hold in a menu would leave
a frame where the menu closes, its blocker lifts, and the clock runs while the player is still
away — which is the entire thing this mod exists to prevent.

## Compatibility

Composes with every time mod on Nexus, because they all use the same blocker with different
ids:

- **[Clock Pause](https://www.nexusmods.com/moonlightpeaks/mods/81)** (id `DecoInvClockPause`)
  — pauses in decorate/inventory/chest screens. Different trigger, no overlap.
- **[Serena's Grimoire](https://www.nexusmods.com/moonlightpeaks/mods/23)** — Time Freeze and
  the ritual wheel both use the blocker. If Grimoire holds the clock and we release ours, the
  clock correctly stays stopped.
- **[TimeControl](https://www.nexusmods.com/moonlightpeaks/mods/85)** — manual hotkeys, also
  the blocker.

The blocker is released in `OnDestroy`, so unloading the mod can never leave the clock
frozen — the failure Grimoire had to warn people about with the old `Time Freeze Hotkey.dll`.

## Prior art

A full sweep of all 88 Moonlight Peaks mods on Nexus (2026-08-04) found nothing that pauses
on idle. Everything time-related is either manual or tied to a UI screen:

| Mod | Trigger | AFK? |
|---|---|---|
| Clock Pause | Decorate mode / inventory / chest open | No |
| TimeControl | `n` / `,` / `.` hotkeys | No |
| Serena's Grimoire — Time Freeze | Cast from the ritual wheel | No |
| Save Anywhere | `F5` | No |

Keyword searches for `afk` and `idle` return zero results. The gap is real.

## Build

```bash
dotnet build "src/CoffinBreak.csproj" -c Release
```

Auto-deploys to `BepInEx/plugins/MoonlightPeaksMods/CoffinBreak/`.
