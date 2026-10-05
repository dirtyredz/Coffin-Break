---
id: dk-9fba4fac
type: task
created: 2026-10-05
status: todo
since: 2026-10-05
area: structure
priority: P2
rank: zn
parent:
fixes: []
blocked_by: []
relates: []
---
# C2: kill the vendored-trio drift (workspace-level)

From docs/BACKLOG.md (P2 structural, 2026-08-22 review)

- **[C2] Kill the vendored-trio drift (workspace-level).** Replace verbatim copies of
  `GameFonts`/`GamePalette`/`PanelSprite` with a **linked shared source file** compiled into each mod's
  DLL, or generate the copies from one workspace canonical. Preserves standalone-DLL output while
  removing the "fix in both copies" burden. Belongs to the workspace, not just this mod.
