---
id: dk-4c2efb57
type: task
created: 2026-10-05
status: done
since: 2026-10-05
area: structure
priority: P2
rank: t
parent:
fixes: []
blocked_by: []
relates: []
---
# C1: extract config schema into CoffinBreakConfig.cs

From docs/BACKLOG.md (Done)

- **2026-08-22 — [C1]** extracted the config schema into `src/CoffinBreakConfig.cs` (`BadgeCorner`
  enum + 13 entries + `Bind`); `Plugin.cs` 170 → 49 lines, consumers now depend on the config not the
  entry point. Surfaced that **[D2] is blocked on [C2]** (vendored `GameFonts` pins
  `CoffinBreakPlugin.Log`). Behaviour-preserving, build clean.
