---
id: dk-3febf5ad
type: task
created: 2026-10-05
status: done
since: 2026-10-05
area: structure
priority: P2
rank: z
parent:
fixes: []
blocked_by: []
relates: []
---
# A1: introduce Safe.cs and route own-code guard sites through it

From docs/BACKLOG.md (Done)

- **2026-08-22 — [A1]** introduced `src/Safe.cs` (`Safe.Get`/`Safe.Do`) and routed all eight own-code
  try/catch guard sites through it (`ActivityMonitor` ×3, `DayTimeBlock` ×3, `AfkWatcher.IsInCutscene`,
  `PassOutGuard.Apply`); `PlayerMoved` deliberately excepted. Behaviour-preserving, build clean.
