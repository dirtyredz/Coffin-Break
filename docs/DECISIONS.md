# DECISIONS — Coffin Break

Design decisions and their rationale, newest first. Records *why*, including rejected alternatives.

## D-007 · Vendor the visual trio; don't share an assembly (2026-08 baseline)
`GameFonts`/`GamePalette`/`PanelSprite` are copied verbatim from ChestLabels rather than factored into
a shared library. **Why:** the workspace ships one self-contained DLL per mod (no inter-mod runtime
dependency to install or version), so a shared runtime assembly is out of scope. **Cost:** manual "fix
in both copies" drift (STRUCTURE.md debt C2/C3). **Rejected/deferred alternatives:** a shared runtime
DLL (breaks standalone install); a **linked shared source file** compiled into each DLL, or generating
the copies from one workspace canonical (like `pack.ps1`) — both preserve standalone output and are the
preferred future fix, backlogged at the workspace level.

## D-006 · Never hardcode the version in Plugin.cs
`PluginVersion` reads a generated `ModBuildInfo.Version` constant sourced from `<Version>` in the
csproj (the `GenerateModBuildInfo` target). **Why:** `[BepInPlugin]` and the packed archive name must
never drift from two hand-edited strings.

## D-005 · Pass-out veto is a Harmony *postfix*, not a prefix
`PassOutGuard` postfixes `GameDefaultState.IsPassOutNeeded` and only turns `true` → `false`. **Why:** a
prefix could suppress the game's own reasons to *allow or refuse* pass-out; a postfix lets cutscene and
quest checks run first and can only ever *add* a refusal, never cause a pass-out that wouldn't happen.
A failed patch logs a warning and the mod loads anyway — the clock hold is the primary protection.

## D-004 · Don't block the save; remove the reason to save
`GamePersistence.AddSaveBlocker` exists but is not used. **Why:** blocking a save the game has decided
to make is a larger promise than stopping a clock; preventing the pass-out removes the *reason* for the
overnight save instead.

## D-003 · Stay out of cutscenes (`PauseDuringCutscenes` defaults off)
**Why:** the game's own scripted waits run on day progression (`DayProgresser.WaitForSeconds` spins
while `IsDayProgressionPaused`), so freezing the clock mid-cutscene can *stall* the scene rather than
protect it. Coffin Break arms on the first frame after the cutscene ends.

## D-002 · Detect idle on legacy `UnityEngine.Input`, not Rewired
**Why:** Rewired needs a player id and an action map a patch could rename; `Input.anyKey` cannot break
that way, and an idle detector that wrongly thinks you are present is a mod that silently does nothing.
The one gap (analogue-stick movement, which is a Rewired axis) is covered by watching character
movement. `Input` must be written `UnityEngine.Input` — the game has its own global `Input` type.

## D-001 · Hold the clock unconditionally; only suppress the badge caption
When another holder (menu, decorate mode, another mod) already stopped the clock, the badge goes quiet
but the hold stays. **Why:** skipping the hold would leave a frame where that other holder releases,
the clock runs, and the player is still away — the exact failure the mod exists to prevent.
