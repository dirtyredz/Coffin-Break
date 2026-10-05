---
id: dk-5a6c5a8a
type: task
created: 2026-10-05
status: todo
since: 2026-10-05
area: structure
priority: P2
rank: zt
parent:
fixes: []
blocked_by: []
relates: []
---
# C3: trim the vendored dead surface (after C2)

From docs/BACKLOG.md (P2 structural, 2026-08-22 review)

- **[C3] Trim the vendored dead surface — after C2.** `HeavyFont` and every `GamePalette` colour but
  `NameCream` are unused here. Reduce the shared canonical surface after a cross-mod usage audit; do not
  trim only this copy.
