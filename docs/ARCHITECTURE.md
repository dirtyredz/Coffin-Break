# ARCHITECTURE — Coffin Break

How the system behaves. For *where the code lives* see [../STRUCTURE.md](../STRUCTURE.md); for *why*
see [DECISIONS.md](DECISIONS.md).

## The problem it solves

Go AFK with Moonlight Peaks running and the in-game clock keeps advancing. It reaches 2am, your
character passes out, the day advances and the game **saves**. You lose a day you did not play, with no
undo. Coffin Break stops the clock while you are away and lets it run again the instant you return.

## Three layers of protection

Ordered by how often each one saves you (from `AfkWatcher.Update`):

1. **Focus loss** — the window loses focus (alt-tab) and, after `FocusLossGraceSeconds` (1.5s), the
   clock stops. The common case and the cheapest to detect. `OnApplicationPause(true)` covers
   minimise/overlay suspends too.
2. **Idle timer** — no keyboard/mouse/controller input (and, optionally, no character movement) for
   `IdleSeconds` (60s) and the clock stops. Covers walking away from a focused window.
3. **Pass-out veto** — while *we* hold the clock, `PassOutGuard` refuses the 2am collapse outright,
   closing the split-second race where the day can end between going idle and the pause engaging.

Any activity disarms all of it immediately; regaining focus counts as activity in itself.

## How the clock is actually stopped

`DayProgresser` (a game singleton) keeps a `Chicken.Utilities.Blocker` of named string ids and stops
advancing the day while any id is present. This is the **game's own mechanism** — decorate mode, the
pause menu and the debug scrubber all use it. `Blocker.Add` is distinct-keyed, so ids from different
mods coexist without clobbering each other. `DayTimeBlock` is the single facade that adds/removes our
id `com.dirtyredz.coffinbreak`:

- `Hold()` → `DayProgresser.AddDayTimeBlocker(id)` (no-op before the game is up).
- `Release()` → `RemoveDayTimeBlocker(id)`; called on disarm **and** in `Plugin.OnDestroy`, so
  unloading the mod can never leave the clock frozen.

**Nothing is written to the save.** Day progression is runtime state that resumes where it left off.

## The pass-out race and the Harmony patch

`GameDefaultState.IsPassOutNeeded` decides the end-of-day collapse; reaching `DayProgression >= 1f`
starts `PassOutRoutine`, which advances the day and saves. A held clock never reaches `1f`, so the
primary protection needs no patch. The gap is the instant *between* the day ending and the pause
engaging (go idle at 1:59am with a 60s timer). `PassOutGuard` adds one Harmony **postfix** on the
getter that turns `true` → `false` while Coffin Break holds the clock — postfix, so the game's own
reasons to refuse (cutscene, quest `IsPassOutPrevented`) still run first and are never overridden. A
failed patch is logged and the mod loads anyway; the blocker is the primary protection.

## Activity detection

`ActivityMonitor.PollActivity()` runs once per frame on **legacy `UnityEngine.Input`** (which the game
ships) rather than Rewired: `Input.anyKey` covers keyboard, mouse buttons and every gamepad button, and
cannot be broken by a renamed action map. Legacy input misses analogue-stick movement (sticks are axes
owned by Rewired), so `CountPlayerMovementAsActivity` watches the character's world position as a truer
presence signal. Every external read is wrapped so a throwing input check can never take down the
`Update` loop. Time is measured with `Time.unscaledDeltaTime` / `realtimeSinceStartup` — a wall-clock
absence must not be measured by a clock other mods can slow.

## The badge

`PauseBadge` draws "Time paused — away" on its **own** screen-space canvas (sorting order 600, above the
HUD and Plant Peek's hover), styled from the game's own assets via the vendored `GameFonts` /
`GamePalette` / `PanelSprite`. It stays quiet when someone else already holds the clock (a menu,
decorate mode, another time mod) — telling you nothing you cannot already see — but **the hold itself is
never conditional**, only the caption: skipping the hold in a menu would leave a frame where the menu
closes and the clock runs while you are still away.

## Compatibility

Composes with every Nexus time mod (Clock Pause, Serena's Grimoire Time Freeze, TimeControl) because
they all use the same `Blocker` with different ids. See [../README.md](../README.md) for the matrix.

## Build & release chain

Single `.csproj`, references the game's shipped assemblies (`Private=false`). Version single-sourced
from `<Version>` → generated `ModBuildInfo.Version`. `pack.ps1` → `dist/CoffinBreak-<version>.zip` in
Nexus layout → publish with the workspace **nexus-publish** skill. Full workspace chain:
[workspace ARCHITECTURE.md](../../../docs/ARCHITECTURE.md).
