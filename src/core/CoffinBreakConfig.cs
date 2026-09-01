using BepInEx.Configuration;

namespace CoffinBreak
{
    /// <summary>
    /// The mod's configuration schema and its bound values, in one place.
    ///
    /// Split out of <see cref="CoffinBreakPlugin"/> so "what is configurable" is a single
    /// responsibility, and every consumer depends on the config rather than on the plugin's
    /// bootstrap class. Values are bound once from <c>Awake</c> via <see cref="Bind"/>.
    ///
    /// The <c>.cfg</c> section keys ("General", "Indicator", "Diagnostics") are untouched —
    /// renaming one would orphan every saved value. The <c>ModMenu.Section=</c> tags below are
    /// display names the Mod Menu reads to title its sections; they do not affect the file.
    /// </summary>
    internal static class CoffinBreakConfig
    {
        /// <summary>Where the paused badge sits on screen.</summary>
        internal enum BadgeCorner
        {
            TopCentre,
            TopLeft,
            TopRight,
            BottomCentre,
            BottomLeft,
            BottomRight,
        }

        internal static ConfigEntry<bool> Enabled;
        internal static ConfigEntry<float> IdleSeconds;
        internal static ConfigEntry<bool> PauseOnFocusLoss;
        internal static ConfigEntry<float> FocusLossGraceSeconds;
        internal static ConfigEntry<bool> PauseDuringCutscenes;
        internal static ConfigEntry<bool> BlockPassOutWhilePaused;
        internal static ConfigEntry<bool> CountPlayerMovementAsActivity;

        internal static ConfigEntry<bool> ShowBadge;
        internal static ConfigEntry<bool> HideBadgeWhenAlreadyPaused;
        internal static ConfigEntry<BadgeCorner> BadgePosition;
        internal static ConfigEntry<float> BadgeFontSize;
        internal static ConfigEntry<bool> ShowPausedDuration;

        internal static ConfigEntry<bool> VerboseLogging;

        // Mod Menu reads these out of ConfigDescription.Tags to title its sections. Display
        // names only - the .cfg section keys are untouched, since renaming one there would
        // orphan every saved value.
        private const string GeneralSection = "ModMenu.Section=When the clock stops";
        private const string IndicatorSection = "ModMenu.Section=Badge";
        private const string DiagnosticsSection = "ModMenu.Section=Diagnostics";

        internal static void Bind(ConfigFile config)
        {
            Enabled = config.Bind(
                "General", "Enabled", true,
                "Master switch. Off leaves the game completely untouched - no clock blocker is " +
                "ever added and the pass-out patch does nothing.");

            IdleSeconds = config.Bind(
                "General", "IdleSeconds", 60f,
                new ConfigDescription(
                    "Seconds without keyboard, mouse or controller input before the in-game clock " +
                    "stops. The clock restarts the instant you touch anything.\n" +
                    "Lower is safer but pauses during a long think; 60 is a compromise. Below about " +
                    "10 seconds you will notice it stopping while you read a dialogue box.",
                    new AcceptableValueRange<float>(5f, 600f),
                    GeneralSection, "ModMenu.Label=Idle for"));

            PauseOnFocusLoss = config.Bind(
                "General", "PauseOnFocusLoss", true,
                "Stop the clock as soon as the game loses window focus, without waiting out " +
                "IdleSeconds. This is the alt-tab case, and it is the one that costs whole days.");

            FocusLossGraceSeconds = config.Bind(
                "General", "FocusLossGraceSeconds", 1.5f,
                new ConfigDescription(
                    "How long the window must stay unfocused before PauseOnFocusLoss acts. Stops the " +
                    "clock flickering on and off when a notification steals focus for an instant.",
                    new AcceptableValueRange<float>(0f, 30f),
                    GeneralSection, "ModMenu.Label=Focus-loss grace"));

            PauseDuringCutscenes = config.Bind(
                "General", "PauseDuringCutscenes", false,
                "Whether to arm while a cutscene is playing. Off by default and it should stay " +
                "off: the game's own scripted waits are driven by day progression " +
                "(DayProgresser.WaitForSeconds), so freezing the clock mid-cutscene can stall the " +
                "scene rather than protect it. Coffin Break arms as soon as the cutscene ends.");

            BlockPassOutWhilePaused = config.Bind(
                "General", "BlockPassOutWhilePaused", true,
                "While the clock is stopped by this mod, also refuse the 2am pass-out outright.\n" +
                "A stopped clock already prevents it, so this only matters for the split second " +
                "between the day ending and the pause engaging. It is the difference between " +
                "'almost never lose a day' and 'never'.");

            CountPlayerMovementAsActivity = config.Bind(
                "General", "CountPlayerMovementAsActivity", true,
                "Treat your character physically moving as activity, on top of raw input.\n" +
                "This is the controller safety net: Unity's legacy input sees gamepad buttons but " +
                "not analogue stick movement, so walking with a stick alone would otherwise look " +
                "idle. Costs one position comparison per frame.");

            ShowBadge = config.Bind(
                "Indicator", "ShowBadge", true,
                "Show a small badge while the clock is stopped. Without it there is no way to " +
                "tell a stopped clock from a slow one, and you would not trust the mod.");

            HideBadgeWhenAlreadyPaused = config.Bind(
                "Indicator", "HideBadgeWhenAlreadyPaused", true,
                "Stay quiet when something else has already stopped the clock — the pause menu, " +
                "decorate mode, an open inventory, or another time mod. The badge is there to " +
                "tell you something you cannot otherwise see, and in a menu you can already see " +
                "it.\n" +
                "This only hides the badge. The clock stays held underneath, so closing the menu " +
                "while you are still away does not hand you back a running clock.");

            BadgePosition = config.Bind(
                "Indicator", "BadgePosition", BadgeCorner.TopCentre,
                "Where the badge sits. TopCentre is clear of the clock, the toolbar and the " +
                "minimap that Detailed Minimap adds.");

            BadgeFontSize = config.Bind(
                "Indicator", "BadgeFontSize", 26f,
                new ConfigDescription(
                    "Badge font size.",
                    new AcceptableValueRange<float>(10f, 60f),
                    IndicatorSection, "ModMenu.Label=Badge text size"));

            ShowPausedDuration = config.Bind(
                "Indicator", "ShowPausedDuration", true,
                "Append how long the clock has been stopped, so on returning you can see at a " +
                "glance how long you were gone.");

            VerboseLogging = config.Bind(
                "Diagnostics", "VerboseLogging", false,
                new ConfigDescription(
                    "Log every arm and disarm with the reason. Useful for working out why it did or " +
                    "did not trigger; noisy otherwise.",
                    null,
                    DiagnosticsSection, "ModMenu.Label=Verbose logging"));
        }
    }
}
