# ROADMAP — Coffin Break

Coffin Break is **shipped and feature-complete** for its scope (v1.0.1, Nexus mod 121). There is no
active feature milestone — this is a small, single-purpose mod that does one thing well. Work from here
is maintenance and opportunistic cleanup, not a phased build-out.

## Now
- **Maintain compatibility** with new game patches. The two reflection touchpoints to re-check after a
  game update: `GameDefaultState.IsPassOutNeeded` (patched by `PassOutGuard`) and
  `DayProgresser.AddDayTimeBlocker` / `Blocker` (used by `DayTimeBlock`). Both fail soft, but a moved
  member silently weakens protection.

## Next (opportunistic)
- Structural cleanups from the 2026-08-22 review — see [BACKLOG.md](BACKLOG.md). The workspace-level
  vendored-trio fix (C2) is the highest-leverage one because it helps every mod, not just this one.

## Ideas (unscheduled)
- Optional on-screen note of *what* armed the pause (idle vs focus loss) for debugging, without turning
  on verbose file logging.
- A configurable "hard" mode that also uses `GamePersistence.AddSaveBlocker` for players who want
  belt-and-braces — deliberately rejected for the default build (see [DECISIONS.md](DECISIONS.md) D-004).

No dates: releases are cut when there is something worth publishing, versioned per the workspace
convention (bump `<Version>` in the csproj at publish time only).
