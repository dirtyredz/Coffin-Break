---
id: dk-2cfb223b
type: task
created: 2026-10-05
status: todo
since: 2026-10-05
area: structure
priority: P2
rank: zzw
parent:
fixes: []
blocked_by: []
relates: []
---
# ActivityMonitor reads the live game without insulation

From docs/BACKLOG.md (Placement follow-up, 2026-09-01 structure review)

- **P2 — `core/ActivityMonitor.cs` reads the live game without insulation.** `PlayerMoved()` reads
  `MonoBehaviourSingleton<PlayerView>.Instance.transform.position` directly and imports
  `Chicken.Utilities`. `game/DayTimeBlock.cs` exists precisely to insulate the mod from a game API;
  `ActivityMonitor` gets no equivalent, so a file classified as game-agnostic breaks if that
  singleton changes. Either route the read through a small `game/` bridge, or move the file. The
  `game/` bullet has been reworded to a primary-responsibility test so the docs no longer contradict
  the placement, but the underlying coupling is still real.
