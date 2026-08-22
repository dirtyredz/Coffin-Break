# Nexus Mod Page — Coffin Break

> **Pasting into the upload form? Use [nexus-paste.md](nexus-paste.md), not this file.**
> The copy here is wrapped for reading, and the editor turns every wrap into a `<br>`.
> This mod's live page is the formatting reference for all six —
> see [13-nexus-page-standard.md](../../13-nexus-page-standard.md).

Draft copy for the Nexus listing. Same shape as
[ChestLabels/NEXUS.md](https://github.com/dirtyredz/chest-labels/blob/main/mods/ChestLabels/NEXUS.md); read that one's notes on the upload form
first, they still apply.

---

## Fields

| Field | Value |
|---|---|
| **Name** | Coffin Break - AFK Auto-Pause |
| **Summary** (short, shows in listings) | Walk away without losing the day. The clock stops when you do, and starts again the moment you touch anything. |
| **Category** | Gameplay — where Clock Pause and TimeControl both sit |
| **Version** | 1.0.1 |
| **Nexus page** | [mod 121](https://www.nexusmods.com/moonlightpeaks/mods/121) — live since 2026-08-04 |
| **Requirements** | BepInEx 5 (win_x64), 5.4.23.5 or newer — required |
| | [Mod Nook](https://www.nexusmods.com/moonlightpeaks/mods/127) — optional, for in-game settings |
| | Mod Menu — optional, the alternative to Mod Nook |
| **Tags** | quality of life, gameplay, save-safe, afk, idle |
| **Licence** | MIT (confirm before upload) |

**Keep `afk` and `idle` in the tags and the title.** A sweep of all 88 mods on 2026-08-04
found both terms returning zero results for this game — first mod to use them owns the search.

---

## Full description — paste into Nexus

### Description

You step away for a minute. The phone rings, someone knocks, dinner burns.

You come back and it is the next morning. The game passed 2am without you, your character
collapsed, the day rolled over and it saved. A whole day you did not play, and no way back.

Coffin Break stops the clock when you stop. Alt-tab away and it pauses in a second and a half.
Sit still at the keyboard and it pauses after a minute. Touch anything — key, mouse, controller
— and time starts again immediately.

It uses the game's own pause, the same one that runs while you are in decorate mode. Nothing is
written to your save.

---

### Installation instructions

**With Vortex**

Open the Files tab, click the Vortex button, and enable the mod. Done.

**Manually**

1. Install BepInEx 5 (win_x64) into your Moonlight Peaks folder, if you do not have it
   already. The BepInEx folder sits beside Moonlight Peaks.exe.
2. Launch the game once, then quit. This creates the BepInEx/plugins folder.
3. Download the archive from the Files tab and extract it over your Moonlight Peaks folder,
   so the file ends up at BepInEx/plugins/CoffinBreak/CoffinBreak.dll.
4. Launch the game.

Settings are written to a .cfg in BepInEx/config on first launch. With Mod Nook installed you
never need to open it — every setting appears under Pause > Mod Nook and applies immediately,
without a restart.

To uninstall, delete the BepInEx/plugins/CoffinBreak folder. Your save is untouched, because
nothing was ever written to it.

---

### Main features

- Alt-tab away and the clock stops after a second and a half
- Sit idle at the keyboard and it stops after a minute — adjustable
- Any key, mouse movement or controller button starts time again instantly
- Stops the clock, not the game — you can still move around while it is held
- Refuses the 2am collapse outright while you are away, so the day cannot roll over
- A small badge shows the clock is stopped, and how long you have been gone
- Works with a controller — walking with the stick counts as being present
- Stays out of the way during cutscenes
- Nothing is written to your save, and uninstalling leaves no trace
- Every timing and the badge itself can be adjusted or switched off

---

### Requirements

**Required**

- BepInEx 5 (win_x64), version 5.4.23.5 or newer

**Recommended companion**

- **Mod Nook** — my in-game settings menu. This mod's timings are the sort of thing you want
  to nudge and feel out rather than guess at once: set the idle delay on a slider and the
  change applies the moment you close the menu. Not needed; without it the settings live in a
  plain config file, and the defaults are meant to be left alone.
  https://www.nexusmods.com/moonlightpeaks/mods/127
- **Mod Menu** by Elsiabeth does the same job and is also supported. Mod Nook and Mod Menu can
  both be installed — each adds its own button and neither interferes with the other.

PC/Steam only. The Switch and mobile builds cannot load BepInEx.

**Compatibility**

Works alongside the other time mods rather than fighting them. Moonlight Peaks pauses its
clock with a list of named holds, and every mod uses its own name, so Coffin Break, Clock
Pause, TimeControl and Serena's Grimoire Time Freeze can all be installed together. If two of
them stop the clock at once, it stays stopped until both let go — which is the correct
behaviour.

---

### Shout outs

- **Little Chicken Game Company** for making a game worth spending this much time inside.
- The **BepInEx** and **HarmonyX** teams, without whom none of this scene exists.
- **cherrikei** for Clock Pause, which is where I confirmed the game's own day-time blocker was
  the right thing to build on rather than touching the clock directly.
- **Elsiabeth** for Mod Menu, which made the case that in-game settings were worth having, and
  which is why this mod never had to build a settings screen of its own.
- **My Mate**, for being my inspiration.

---

## Changelog entries for the Nexus page

Player-facing. Describe the **symptom**, not the cause — the repo README names the Harmony
patch and the blocker id; that belongs in the repo.

### 1.0.1

```
Fixed
- The "time paused" badge no longer appears while you are in a menu, in
  decorate mode or browsing a chest. Time was already stopped on those
  screens, so the badge had nothing to tell you.

New settings
- HideBadgeWhenAlreadyPaused - turn the above off if you would rather see
  the badge whenever the mod is holding the clock.
```

### 1.0.0

```
First release.

- The clock stops when you alt-tab away, or after a minute of sitting still.
- Time starts again the instant you touch anything.
- Your character will not collapse at 2am while you are away.
```

---

## Screenshots

Files live in `screenshots/`. The thumbnail is set separately in the upload form.

| # | Shot | File | Status |
|---|---|---|---|
| - | Thumbnail, 16:9 | `thumbnail.png` | ✅ 1672x941 (1.78:1) — exact, all three text elements proofread at 6x |
| - | Title banner | `banner.png` | ✅ 1400x396 (3.54:1) — approved for release |
| 1 | Badge showing a long absence — "away 8m" or more | `01-badge-away.png` | ⬜ to capture |
| 2 | The badge in place against the farm, clock visible in frame | `02-badge-in-world.png` | ⬜ to capture |
| 3 | Mod Menu settings panel *(optional)* | `03-settings.png` | ⬜ optional |
| - | Proof the badge renders — not for the gallery | `badge-working-crop.png` | ✅ captured |

`badge-working-crop.png` is kept as evidence, not as a listing image: it is a 502x302 crop
where the gallery wants a full frame, and it reads "away 5s", which undersells the mod. It does
confirm the badge draws in Gelica on the game's own plate — the thing
[10-visual-integration.md](https://github.com/dirtyredz/chest-labels/blob/main/10-visual-integration.md) exists to prevent getting wrong.

Shot 1 is the whole pitch. A badge reading **"Time paused — away 8m"** states the problem and
the fix in one line with no caption. Let it run for a few real minutes before capturing — "away
3s" says nothing.

Shot 2 wants the game's own clock in frame if it can be framed that way, since the point is
that the clock is not moving.

Night shots read better against the game's lighting and look unmistakably like Moonlight Peaks.

### The thumbnail must be 16:9, and the reason is worse than cropping

Measured off a live Moonlight Peaks listing tile on 2026-08-04:

```
natural 385x216   ratio 1.78 (16:9)   object-fit: fill
```

**`fill` stretches, it does not crop.** An off-ratio thumbnail is not letterboxed and does not
lose its edges — it is squashed to fit. A 1:1 image rendered into that tile comes out about
78% wider than drawn, which on a display serif like the "Coffin Break" wordmark is immediately
visible, and turns the coffin squat.

So the thumbnail has to be *composed* at 16:9, not merely padded to it. `1672x941` is a good
render size — same ratio, comfortably above the 385x216 the tile actually uses.

The ChestLabels notes already said 16:9, but not why. Cropping would have been survivable;
stretching is not.

Description images — including `banner.png` — are not force-fitted and simply scale, so the
banner's ratio only needs to be roughly right.

### Capturing the badge — read this first

**Steam's F12 will not work, and neither will PrintScreen.** Any keypress counts as activity,
so the mod disarms and hides the badge in the *same frame* the key lands — `PauseBadge.Update`
runs after `AfkWatcher.Update` on the same GameObject. The capture lands on a frame where the
badge alpha is already zero.

**Use the focus-loss path instead:**

1. Run the game borderless windowed.
2. Click into another application. The game loses focus, and the badge arms 1.5s later.
3. The game keeps rendering — `Application.runInBackground` is `True` on this build, confirmed
   in `LogOutput.log`.
4. Capture the game window *from the other application*.

Unity does not process input while unfocused, so nothing disarms it. The badge sits there
indefinitely and there is no timing race at all. This is also the easiest way to get a long
duration on the badge: leave it and come back.

For a shot with the game **focused**, set `IdleSeconds = 5` in the config, start a delayed
capture, and take your hands off the desk.

### Proofread generated art at 5x or more before accepting it

Generated lettering degrades in ways that are invisible at normal viewing size and obvious once
a region is cropped and upscaled, so check every text element rather than just the headline.
The thumbnail passed this cleanly at 6x — `Moonlight Peaks`, `Coffin Break` and `Pause Time.
Save Your Day.` all correct.

Use it at 5x or better:

```powershell
Add-Type -AssemblyName System.Drawing
$src  = [System.Drawing.Image]::FromFile('screenshots\banner.png')
$x=540; $y=25; $w=340; $h=65; $scale=6      # region to inspect
$crop = New-Object System.Drawing.Rectangle -ArgumentList $x,$y,$w,$h
$dest = New-Object System.Drawing.Rectangle -ArgumentList 0,0,($w*$scale),($h*$scale)
$bmp  = New-Object System.Drawing.Bitmap    -ArgumentList ($w*$scale),($h*$scale)
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
$g.DrawImage($src, $dest, $crop, [System.Drawing.GraphicsUnit]::Pixel)
$bmp.Save("$env:TEMP\wordmark.png", [System.Drawing.Imaging.ImageFormat]::Png)
$g.Dispose(); $bmp.Dispose(); $src.Dispose()
```

`New-Object` needs `-ArgumentList` here; the bare `New-Object Type(a,b,c)` form binds the
arguments as a single array and fails.

If a banner is ever regenerated, the thumbnail's ribbon wording is the better model —
**Pause Time**, rather than "pause the game", since the mod stops the clock and leaves you
free to move.

### Art direction

Palette is fixed by [10-visual-integration.md](https://github.com/dirtyredz/chest-labels/blob/main/10-visual-integration.md): `#1B0F2E` plum
fill, `#C7A25B` gold rim, `#F7D994` warm gold text, `#2A1B3D` ink.

Concept worth trying first: **a coffin-shaped hourglass with the sand stopped mid-fall.** The
coffin carries the name, the stopped sand carries the function, and it needs no caption.

### Worth knowing before using AI-assisted art

At least one mod in this scene advertises **"NO AI."** in its description as a selling point,
which implies the opposite draws comment here. Not a reason to avoid it — just a signal
specific to this community that is better known in advance than discovered in the comments.

---

## Notes before publishing

- ✅ **Play-tested.** v1.0.0 has been through a real AFK cycle on 2026-08-04 and preserved the
  day. The description's central claim is verified, not assumed.
- **The banner says "pause the game"; the mod stops the clock.** You can still move, farm and
  talk to villagers while it is held, because `DayProgresser` gates time and NPC movement but
  not the player. Harmless in practice — nobody is playing while AFK — but this scene draws the
  distinction (Serena's Grimoire spells it out for Time Freeze), so the feature list carries a
  "stops the clock, not the game" line to keep the banner reading as shorthand rather than a
  claim. Worth tightening the art if it is ever regenerated.
- State plainly that it is **save-safe** — this community reads for that.
- List BepInEx as **required** and Mod Menu as **optional**.
- Decide the licence / permissions stance before upload.
- The name is a pun and the subtitle is the search term. Do not drop the subtitle.
