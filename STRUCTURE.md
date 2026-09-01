# STRUCTURE — Coffin Break

Where things live and why. Pairs with the code; for *how the system behaves* see
[docs/ARCHITECTURE.md](docs/ARCHITECTURE.md), for *why* see [docs/DECISIONS.md](docs/DECISIONS.md).

Coffin Break is a BepInEx 5 / HarmonyX plugin for the Unity Mono game **Moonlight Peaks**. It stops
the in-game clock while you are away (idle, alt-tabbed, or suspended) so going AFK never costs you a
day. Target `netstandard2.1`. `src/Plugin.cs` sits beside the `.csproj` at the `src/` root (BepInEx
entry point); every other `.cs` file lives in one of three homes — `src/game/`, `src/ui/`, `src/core/`
— per the [Layout](#layout) contract below, which is shared by every Moonlight Peaks mod.

## Layout

```
CoffinBreak/
├── pack.ps1                 # packaging script — workspace convention: lives at the mod root
├── STRUCTURE.md, CLAUDE.md, README.md, CHANGELOG.md, NEXUS.md, RELEASING.md, ...
├── docs/                    # the living-doc set (ARCHITECTURE, DECISIONS, FEATURES, ...)
├── scripts/                 # repo tooling — git hook installer + pre-commit formatter
├── screenshots/             # Nexus page art
└── src/
    ├── CoffinBreak.csproj
    ├── Plugin.cs            # BepInEx entry point — must sit beside the .csproj, not in a folder
    ├── game/                # interop with the live game
    │   ├── DayTimeBlock.cs           # the one clock toucher: our id in DayProgresser's Blocker
    │   ├── PassOutGuard.cs           # Harmony postfix on GameDefaultState.IsPassOutNeeded
    │   ├── GameFonts.cs              # locates the game's Gelica font + outline material (vendored)
    │   └── GamePalette.cs            # the game's colour constants (vendored)
    ├── ui/                  # the mod's own panels, widgets and generated art
    │   ├── PauseBadge.cs             # the "Time paused — away" badge (MonoBehaviour + canvas)
    │   └── PanelSprite.cs            # generated 9-slice plate sprite (vendored)
    └── core/                # the mod's own domain logic, state, config and guards
        ├── AfkWatcher.cs            # the state machine — idle in, active out (MonoBehaviour)
        ├── ActivityMonitor.cs       # "did the player do anything this frame?" — input polling
        ├── CoffinBreakConfig.cs     # the config schema + Bind(ConfigFile)
        └── Safe.cs                  # cross-cutting throw-guard helper (Safe.Get / Safe.Do)
```

**Enforced homes:**

- `src/game/` — Harmony patches and live-game bridges: anything whose PRIMARY responsibility is
  reading or intercepting the running game. (Primary, not "any": `core/ActivityMonitor.cs` makes one
  guarded `PlayerView` read as a fallback and is otherwise the mod's own input polling.)
- `src/ui/` — panels, widgets, presenters, views and runtime-generated sprites
- `src/core/` — the mod's own domain logic, state, config, input polling and diagnostics
- `src/Plugin.cs` — the BepInEx entry point; must sit beside the `.csproj` at the `src/` root
- `pack.ps1` — packaging script; workspace convention puts it at the mod root beside the docs
- `scripts/` — repo tooling: the git-hook installer and the pre-commit formatter

`ActivityMonitor` sits in `core/` rather than `game/` because it is deliberately built on legacy
`UnityEngine.Input` rather than the game's own input layer; its one live-game read (`PlayerView`) is a
fallback for analogue-stick movement, not its purpose.

## Code map

All source is under `src/`, in the homes above. One responsibility per file:

| File | Lines | Responsibility | Kind |
|------|------:|----------------|------|
| `src/Plugin.cs` | 49 | BepInEx entry point: calls `CoffinBreakConfig.Bind`, wires the Harmony patch + `AfkWatcher`, releases the clock on unload, owns the shared `Log`. Lifecycle/wiring only. | Own |
| `src/core/CoffinBreakConfig.cs` | 143 | The configuration schema and bound values: all 13 `ConfigEntry` fields, the `BadgeCorner` enum, section labels, and `Bind(ConfigFile)`. Consumers depend on this, not on the entry class. | Own |
| `src/core/AfkWatcher.cs` | 176 | The state machine — *idle in, active out*. A `MonoBehaviour` on the plugin's GameObject that arms/disarms the hold based on idle time, focus loss and cutscene state. Exposes `IsArmed` (this watcher's session state) to its own badge; the authority on whether the clock is held is `DayTimeBlock.IsHeld`. | Own |
| `src/core/ActivityMonitor.cs` | 144 | Answers "did the player do anything this frame?" from legacy `UnityEngine.Input` plus character-movement fallback. Pure poller, no Unity component. | Own |
| `src/game/DayTimeBlock.cs` | 108 | The **one** place that touches the game clock: adds/removes our id in `DayProgresser`'s `Blocker`, and reads whether anyone else holds it. Static facade over the game mechanism. | Own |
| `src/core/Safe.cs` | 54 | Cross-cutting guard helper: `Safe.Get`/`Safe.Do` run a Unity/game call that might throw, turning a throw into a fallback (or a logged warning). Names the guard shape once for the mod's own code. | Own |
| `src/game/PassOutGuard.cs` | 77 | The Harmony postfix on `GameDefaultState.IsPassOutNeeded` that refuses the 2am collapse during the split-second end-of-day race. | Own |
| `src/ui/PauseBadge.cs` | 203 | The "Time paused — away" badge: builds its own canvas, positions/sizes it, formats the caption. `MonoBehaviour` owned by `AfkWatcher`. | Own |
| `src/game/GameFonts.cs` | 178 | Locates the game's Gelica font + outline material from loaded assets. | **Vendored** |
| `src/game/GamePalette.cs` | 40 | The game's UI colours in one place. | **Vendored** |
| `src/ui/PanelSprite.cs` | 102 | Generates the 9-sliced rounded plate behind the badge. | **Vendored** |

**Vendored** = *copied verbatim from `mods/ChestLabels`* (namespace aside); the file headers say so and
say "fix bugs in both copies." See [Structural debt](#structural-debt) and
[docs/DECISIONS.md](docs/DECISIONS.md).

## Dependency shape

```
Plugin (entry, wiring)  ── Awake ──▶ CoffinBreakConfig.Bind   (the config schema; everyone reads it)
  ├─ PassOutGuard.Apply(harmony)        → reads DayTimeBlock.IsHeld + config
  ├─ AddComponent<AfkWatcher>()
  │     ├─ new ActivityMonitor()        → reads config
  │     ├─ DayTimeBlock.Hold/Release    → the only clock toucher
  │     └─ AddComponent<PauseBadge>()   → reads AfkWatcher.PausedSeconds/IsArmed + DayTimeBlock + config
  │            └─ GameFonts / GamePalette / PanelSprite   (vendored visual trio)
  └─ OnDestroy → DayTimeBlock.Release() + UnpatchSelf()

Cross-cutting: ActivityMonitor / DayTimeBlock / AfkWatcher / PassOutGuard → Safe (throw-guard)
                                                                            └→ CoffinBreakPlugin.Log
```

Runtime state flows **one way**: `ActivityMonitor` → `AfkWatcher` (decides) → `DayTimeBlock` (acts) /
`PauseBadge` (shows). `PassOutGuard` only *reads* `DayTimeBlock.IsHeld` (the single authority on
whether the clock is held). No cycles. `Safe` is a leaf utility everyone may call.

The one shape worth naming: consumers read config from `CoffinBreakConfig.<Entry>.Value` (its own
class since C1) but still reach **back** to `CoffinBreakPlugin.Log` for logging. The logger cannot move
cleanly because the **vendored** `GameFonts` references `CoffinBreakPlugin.Log` and must stay
verbatim-synced — so the residual entry-class dependency (debt D2) is gated on the vendored-trio fix
(C2), not on this repo alone.

## Build & release

- Single `.csproj` at `src/CoffinBreak.csproj`; references are the game's own shipped assemblies
  (`Private=false`, never copied next to the plugin).
- Version is single-sourced from `<Version>` in the csproj → a generated `ModBuildInfo.Version`
  constant (`GenerateModBuildInfo` target in `Directory.Build.props`). `Plugin.cs` never hardcodes it.
- `Directory.Build.props` and `pack.ps1` are **workspace-synced canonicals** — do not edit here; they
  are regenerated by `../../tools/sync-mod-files.ps1`.
- `pack.ps1` → `dist/CoffinBreak-<version>.zip` in Nexus layout. Publish via the workspace
  **nexus-publish** skill. Full chain: [../../docs/ARCHITECTURE.md](../../docs/ARCHITECTURE.md).

## Structural debt

Findings from the full-depth review on **2026-08-22** (componentization + abstraction Sonnet lenses +
a Codex cross-model pass). Verdict: **no P0/P1 — the componentization is sound for the mod's size.**
The two Sonnet lenses returned SOUND; Codex confirmed and added the M-items. All open items are P2 and
tracked in [docs/BACKLOG.md](docs/BACKLOG.md).

**Fixed in this pass:**

- **C4 — `DayTimeBlock.IsClockStopped` deleted.** Own code, `internal`, zero references (`PauseBadge`
  uses `IsHeldByAnyoneElse`). Removed with its doc comment.
- **M5 — `DayTimeBlock.BlockerId` tightened `internal` → `private`.** Referenced only inside
  `DayTimeBlock`; the exposure leaked implementation surface.
- **C1 — Config schema extracted to `CoffinBreakConfig` (new `src/core/CoffinBreakConfig.cs`).** The 13
  `ConfigEntry` fields, the `BadgeCorner` enum, the section labels and the ~90-line bind block moved out
  of `Plugin.cs` (170 → 49 lines) into a dedicated schema class with `Bind(ConfigFile)`. Every consumer
  now depends on `CoffinBreakConfig`, not on the entry point. `.cfg` section keys unchanged (no saved
  values orphaned). Behaviour-preserving; build clean.
- **D1 (M2) — Triple pause-ownership state collapsed.** `AfkWatcher.armed`, the static
  `AfkWatcher.IsPaused`, and `DayTimeBlock.held` all encoded "are we holding the clock?". The
  redundant static mirror `AfkWatcher.IsPaused` is deleted; `DayTimeBlock.IsHeld` is now the **single
  authority** on whether the clock is held — `PassOutGuard` reads it directly. `PauseBadge` reads the
  new instance property `AfkWatcher.IsArmed` from its bound watcher — a deliberately *distinct*
  question ("is this watcher's AFK session armed", paired with `PausedSeconds`), which coincides with
  `IsHeld` during normal arm/disarm but not at plugin teardown (`OnDestroy` releases the clock
  independently of the watcher). `armed` is now fully **private** to the watcher. The arm/disarm
  control flow is untouched, so behaviour is identical. Build clean. _(Approach A of two; the fuller
  variant — dropping `armed` too and driving the loop off `IsHeld` — was rejected as needless risk to
  the critical path. The Codex sign-off confirmed keeping the watcher-scoped `IsArmed` over collapsing
  the badge onto `IsHeld`, since the two facts diverge at teardown.)_

- **A1 — Repeated `try/catch` guard shape named as `Safe` (new `src/core/Safe.cs`).** The "reach into a
  Unity/game API that might throw; fall back / log" shape now goes through `Safe.Get<T>` (silent
  fallback) / `Safe.Do` (logged) at **all eight** own-code sites — `ActivityMonitor` (3 input reads),
  `DayTimeBlock` (`IsHeldByAnyoneElse`, `Hold`, `Release`), `AfkWatcher.IsInCutscene`, and the one-shot
  `PassOutGuard.Apply`. The **sole** exception is `ActivityMonitor.PlayerMoved` (its catch mutates
  state, which neither overload can express). Vendored files keep their own guards to stay
  verbatim-synced. Behaviour-preserving (identical fallbacks + log format); build clean. _The review
  first shipped six sites; its own change-review caught `IsInCutscene` + `Apply` and they were folded
  in._

**Open (backlogged):**

- **C2 — Vendored visual trio duplicated across mods (P2, workspace-level).** `GameFonts`,
  `GamePalette`, `PanelSprite` are copied verbatim from ChestLabels with a manual "fix bugs in both
  copies" burden. A shared *runtime assembly* conflicts with the workspace's standalone-DLL-per-mod
  architecture; Codex's lighter remedies — a **linked shared source file** compiled into each DLL, or
  generating both copies from one workspace canonical (like `pack.ps1`/`Directory.Build.props`) — keep
  standalone output while removing the drift. Workspace-level; documented, not fixed here. See
  [docs/DECISIONS.md](docs/DECISIONS.md) and [docs/GOTCHAS.md](docs/GOTCHAS.md).

- **C3 — Dead surface carried by the vendored trio (P2).** This mod uses only `GamePalette.NameCream`
  and `GameFonts.Apply(preferOutline:true)`; `HeavyFont` and every palette colour but `NameCream` are
  unused here. Inherent cost of C2 — do **not** trim only this copy; reduce the shared canonical
  surface after a cross-mod usage audit, once C2 is addressed.

- **M1 — `PanelSprite` re-inlines palette literals (P2, vendored/coordinated).** `PanelSprite.Fill`/
  `Edge` hardcode the same purple/rim `Color32` values as `GamePalette.NameplatePurple`/`NameplateRim`
  instead of referencing them, defeating the palette's "colours in one place" boundary (and this is
  why those two palette fields read as unused under C3). Fix is cheap but must land in **both** vendored
  copies together to preserve the verbatim-sync invariant — hence coordinated, not a unilateral edit.

- **D2 (M3) — Logging reaches back through the entry class (P2, gated by C2).** With C1 done, the only
  residual entry-class dependency is `CoffinBreakPlugin.Log`, read by `Safe`, `PassOutGuard`,
  `AfkWatcher` — **and by the vendored `GameFonts`**. A neutral logging adapter can't fully replace
  `CoffinBreakPlugin.Log` without either editing the vendored file (breaks verbatim-sync) or leaving a
  shim, so D2 is **blocked on the C2 resolution** rather than independently actionable. Do it as part
  of de-vendoring the trio.

- **M4 — `PauseBadge` retains `canvas` as a field + a dead guard (P2).** `canvas` is only used inside
  `Build()`, so it can be a local; and the `if (!built) return;` immediately after `Build()`
  (`PauseBadge.cs:57`) is unreachable because `Build()` sets `built = true` unconditionally at its end.
  Kept for now as intentional defensiveness (a future fallible `Build()` would want the guard) — noted
  so the choice is explicit, not accidental.

_Last full review: 2026-08-22_
