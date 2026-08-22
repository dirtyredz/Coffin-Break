# GOTCHAS — Coffin Break

Non-obvious traps. Read before changing the flagged areas.

- **`Input` is ambiguous — write `UnityEngine.Input`.** The game ships its own global `Input` type that
  wins the bare-name lookup. `ActivityMonitor` fully-qualifies every call for this reason. A bare
  `Input.anyKey` will bind to the wrong type.

- **The vendored trio must stay verbatim.** `src/GameFonts.cs`, `src/GamePalette.cs`,
  `src/PanelSprite.cs` are copied from `mods/ChestLabels` and their headers say "fix bugs in both
  copies." Do **not** edit one in isolation (including the tempting M1 fix of replacing `PanelSprite`'s
  literals with `GamePalette` references) — it must land in both copies together or the sync invariant
  breaks. See STRUCTURE.md debt C1/M1.

- **Never freeze the clock during a cutscene.** The game's scripted waits run on day progression
  (`DayProgresser.WaitForSeconds` spins while `IsDayProgressionPaused`). Holding the clock mid-cutscene
  can stall the scene. `PauseDuringCutscenes` defaults off and should stay off.

- **Hold is unconditional; only the caption is suppressed.** When tempted to "skip the hold if a menu
  already stopped the clock," don't — releasing on the same frame the menu closes hands back a running
  clock while the player is still away. Only `PauseBadge`'s visibility keys off other holders.

- **Measure absence in real time.** Use `Time.unscaledDeltaTime` / `realtimeSinceStartup`, never scaled
  time — another mod slowing/stopping game time would otherwise corrupt the idle count.

- **External calls are wrapped on purpose.** Unity throws (not returns default) when a platform lacks a
  device, and singletons can be absent before the game is up. The `try/catch` guards in
  `ActivityMonitor`/`DayTimeBlock` are load-bearing: a throwing idle check would take the whole `Update`
  loop down. Keep the guard if you refactor them (see STRUCTURE.md debt A1).

- **Always release the blocker on teardown.** `Plugin.OnDestroy` → `DayTimeBlock.Release()`. Leaving a
  blocker behind freezes the player's clock permanently — the failure mode the old "Time Freeze
  Hotkey" had to warn people about. `Release()` clears its `held` flag *before* the game call so a
  throw can't strand it.

- **`Directory.Build.props` and `pack.ps1` are workspace-synced.** Editing them here is pointless —
  `../../tools/sync-mod-files.ps1` regenerates them from the workspace canonical. Change them upstream.

- **`PauseBadge.Build()` sets `built = true` unconditionally**, so the `if (!built) return;` right after
  it never fires today (STRUCTURE.md debt M4). It's kept as defensive scaffolding for a future fallible
  `Build()`; don't mistake it for reachable error handling.
