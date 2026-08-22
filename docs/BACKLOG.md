# BACKLOG — Coffin Break

Prioritized trough of deferred work and known issues. P0 = do next, P1 = should, P2 = nice-to-have.
Structural items carry their STRUCTURE.md debt id. Seeded from the full review on **2026-08-22**.

## P0
_None._ The mod is shipped, feature-complete, and the review found no P0/P1 structural issues.

## P1
_None._

## Done
- **2026-08-22 — [C4]** deleted dead `DayTimeBlock.IsClockStopped`.
- **2026-08-22 — [M5]** tightened `DayTimeBlock.BlockerId` to `private`.
- **2026-08-22 — [A1]** introduced `src/Safe.cs` (`Safe.Get`/`Safe.Do`) and routed the six own-code
  try/catch guard sites through it; behaviour-preserving, build clean.

## P2 — structural (from the 2026-08-22 review)

- **[C1] Extract config schema out of `Plugin.cs`.** Move the 13 `ConfigEntry` fields, the
  `BadgeCorner` enum and the section-label constants into a `CoffinBreakConfig` static class with a
  `Bind(ConfigFile)`. Flips every consumer's dependency from the concrete entry point to the config.
  Deferred: 170-line file, ubiquitous BepInEx idiom — low payoff for the churn.

- **[C2] Kill the vendored-trio drift (workspace-level).** Replace verbatim copies of
  `GameFonts`/`GamePalette`/`PanelSprite` with a **linked shared source file** compiled into each mod's
  DLL, or generate the copies from one workspace canonical. Preserves standalone-DLL output while
  removing the "fix in both copies" burden. Belongs to the workspace, not just this mod.

- **[C3] Trim the vendored dead surface — after C2.** `HeavyFont` and every `GamePalette` colour but
  `NameCream` are unused here. Reduce the shared canonical surface after a cross-mod usage audit; do not
  trim only this copy.

- **[M1] `PanelSprite` should reference `GamePalette` instead of re-inlining purple/rim literals.**
  Cheap, but **coordinated**: must land in both vendored copies together. (Also removes the reason two
  palette fields read as unused under C3.)

- **[D1/M2] Collapse the triple pause-ownership state.** `AfkWatcher.armed`, static
  `AfkWatcher.IsPaused`, and `DayTimeBlock.held` all encode "are we holding the clock?". Pick one
  authority. Behaviour-sensitive (touches arm/disarm) — do with care and testing.

- **[D2/M3] Decouple logging from the entry class.** `CoffinBreakPlugin.Log` is reached from several
  files; a neutral logging adapter (or injected logger) removes the composition-root dependency that
  survives C1. Pairs with C1.

- **[M4] `PauseBadge`: make `canvas` a local; decide on the unreachable `if (!built)` guard.** Kept for
  now as intentional defensiveness — revisit if `Build()` is ever made fallible.

## Known issues
- **Residual window with `BlockPassOutWhilePaused` off.** Going idle within `IdleSeconds` of 2am can
  still lose the day. Mitigated by the default (on). *Working as designed; documented in README.*
- **Not a substitute for pausing.** Protects the day only; does not stop a hostile encounter or a
  timed quest with its own clock. *By design.*
