> ⚠️ **Superseded — do not paste from this file.**
> The live pages were restyled on 2026-08-04 and this BBCode is the *pre-style* version.
> The live page is now the source of truth; pull its BBCode from the edit form's description
> field. Structure: [14-description-review.md](../../14-description-review.md). Look:
> [15-page-style.md](../../15-page-style.md). Mechanics: [13-nexus-page-standard.md](../../13-nexus-page-standard.md).

# Coffin Break — Nexus page source

**Nexus page:** [mod 121](https://www.nexusmods.com/moonlightpeaks/mods/121)

The description field is **SCEditor with a BBCode source**, so the block below is the literal
value that gets set. Structure per [14-description-review.md](../../14-description-review.md).

Description prose and Main features wording are **yours, unchanged** — except for one bullet
restored, below.

## Other fields

| Field | Change |
|---|---|
| Name | `Coffin Break` — **no change.** A subtitle was tried and reverted; see below |
| Category | Gameplay — no change |
| Tags | `User Interface` is arguably wrong for a mod with no interface beyond a badge, but that is a judgement call, not a fix. `afk` and `idle` are **not possible** — see below |
| Short description | no change, the live one is good |

**Renaming is off the table.** `Coffin Break - AFK Auto-Pause` was applied on 2026-08-04 and
reverted the same day at your call: you do not want detail bolted onto mod names. The earlier
draft note telling you not to drop the subtitle is superseded — the name stays as it is.

**`afk` and `idle` cannot be added.** Nexus tags are a fixed vocabulary per game, not free
text. Typing `afk` into the tag field returns *"No Tags found"*. The keyword sweep that found
those terms unused was measuring **search**, not tags — the place those words can actually do
work is the description body, where they already appear.

## Restored feature bullet

The live page is missing this one, which is in the draft. It is the line that keeps the
banner's "pause the game" reading as shorthand rather than a false claim, since you can still
move around while the clock is held:

> Stops the clock, not the game — you can still move around while it is held

It goes back in below, third from the top.

## Description source

```bbcode
[size=4][b]Description[/b][/size]
[color=#D4D4D8]You step away for a minute. The phone rings, someone knocks, dinner burns.

You come back and it is the next morning. The game passed 2am without you, your character collapsed, the day rolled over and it saved. A whole day you did not play, and no way back.

Coffin Break stops the clock when you stop. Alt-tab away and it pauses in a second and a half. Sit still at the keyboard and it pauses after a minute. Touch anything — key, mouse, controller — and time starts again immediately.

It uses the game's own pause, the same one that runs while you are in decorate mode. Nothing is written to your save.[/color]

[size=4][b]Main features[/b][/size]
[list]
[*]Alt-tab away and the clock stops after a second and a half
[*]Sit idle at the keyboard and it stops after a minute — adjustable
[*]Any key, mouse movement or controller button starts time again instantly
[*]Stops the clock, not the game — you can still move around while it is held
[*]Refuses the 2am collapse outright while you are away, so the day cannot roll over
[*]A small badge shows the clock is stopped, and how long you have been gone
[*]Works with a controller — walking with the stick counts as being present
[*]Stays out of the way during cutscenes
[*]Nothing is written to your save, and uninstalling leaves no trace
[*]Every timing and the badge itself can be adjusted or switched off
[/list]

[size=4][b]Requirements[/b][/size]
[list]
[*][b]BepInEx 5 (win_x64)[/b], version 5.4.23.5 or newer — the only thing this mod needs
[/list]
[color=#D4D4D8]PC/Steam only. The Switch and mobile builds cannot load BepInEx.[/color]

[size=4][b]Installation[/b][/size]
[b]With Vortex[/b]
[color=#D4D4D8]Open the Files tab, click the Vortex button, and enable the mod. Done.[/color]

[b]Manually[/b]
[list=1]
[*]Install [b]BepInEx 5 (win_x64)[/b] into your Moonlight Peaks folder, if you do not have it already. The BepInEx folder sits beside Moonlight Peaks.exe.
[*]Launch the game once, then quit. This creates the BepInEx/plugins folder.
[*]Download the archive from the Files tab and extract it over your Moonlight Peaks folder, so the file ends up at BepInEx/plugins/CoffinBreak/CoffinBreak.dll
[*]Launch the game.
[/list]
[color=#D4D4D8]To uninstall, delete the BepInEx/plugins/CoffinBreak folder. Your save is untouched, because nothing was ever written to it.[/color]

[size=4][b]Configuration[/b][/size]
[color=#D4D4D8]Settings are written to BepInEx/config/com.dirtyredz.moonlightpeaks.coffinbreak.cfg on first launch. The defaults are meant to be left alone.

Install [url=https://www.nexusmods.com/moonlightpeaks/mods/127][b]Mod Nook[/b][/url] and you can change them in game instead. Coffin Break shows up in it on its own, and the timings are the sort of thing you want to feel out rather than guess at once — set the idle delay on a slider and it applies the moment you close the menu. Nothing here needs it — it just makes this mod easier to live with.[/color]

[size=4][b]Compatibility[/b][/size]
[color=#D4D4D8]Works alongside the other time mods rather than fighting them. Moonlight Peaks pauses its clock with a list of named holds, and every mod uses its own name, so Coffin Break, Clock Pause, TimeControl and Serena's Grimoire Time Freeze can all be installed together. If two of them stop the clock at once, it stays stopped until both let go — which is the correct behaviour.[/color]

[size=4][b]Shout outs[/b][/size]
[list]
[*][b]Little Chicken Game Company[/b] for making a game worth spending this much time inside.
[*]The [b]BepInEx[/b] and [b]HarmonyX[/b] teams, without whom none of this scene exists.
[*][b]cherrikei[/b] for Clock Pause, which is where I confirmed the game's own day-time blocker was the right thing to build on rather than touching the clock directly.
[*][b]My Mate[/b], for being my inspiration.
[/list]
```
