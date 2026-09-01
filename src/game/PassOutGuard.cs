using System.Reflection;
using HarmonyLib;

namespace CoffinBreak
{
    /// <summary>
    /// Closes the one gap a stopped clock cannot close by itself.
    ///
    /// <c>GameDefaultState.IsPassOutNeeded</c> is what decides you collapse at the end of the
    /// day, and collapsing is what advances the day and writes the save — the exact thing this
    /// mod exists to prevent. Normally holding the clock is enough, because the check is
    /// <c>DayProgression &gt;= 1f</c> and a stopped clock never gets there.
    ///
    /// The gap is the moment in between: go idle at 1:59am with a 60 second timer and the day
    /// can end before the pause engages. This postfix makes that unreachable by refusing the
    /// pass-out outright whenever Coffin Break is holding the clock.
    ///
    /// A postfix, not a prefix: the game's own reasons for saying "no pass-out" — cutscene in
    /// progress, IsPassOutPrevented set by a quest — still run first and are never overridden.
    /// This only ever turns true into false, so it cannot cause a pass-out that would not
    /// otherwise happen.
    /// </summary>
    internal static class PassOutGuard
    {
        internal static void Apply(Harmony harmony)
        {
            // A failed patch must not stop the mod loading: the clock blocker is the primary
            // protection and works without this, so Safe.Do logs and swallows any throw.
            Safe.Do(() =>
            {
                var property = AccessTools.Property(typeof(GameDefaultState), "IsPassOutNeeded");
                var getter = property?.GetGetMethod(nonPublic: true);

                if (getter == null)
                {
                    CoffinBreakPlugin.Log.LogWarning(
                        "GameDefaultState.IsPassOutNeeded not found - the end-of-day guard is off. " +
                        "Stopping the clock still protects you; only the split-second race is " +
                        "unguarded. This usually means a game update moved it.");
                    return;
                }

                harmony.Patch(
                    getter,
                    postfix: new HarmonyMethod(
                        typeof(PassOutGuard).GetMethod(
                            nameof(RefusePassOutWhilePaused),
                            BindingFlags.Static | BindingFlags.NonPublic)));
            }, "End-of-day guard could not be applied");
        }

        private static void RefusePassOutWhilePaused(ref bool __result)
        {
            if (!__result)
            {
                return;
            }

            if (!CoffinBreakConfig.Enabled.Value || !CoffinBreakConfig.BlockPassOutWhilePaused.Value)
            {
                return;
            }

            if (!DayTimeBlock.IsHeld)
            {
                return;
            }

            __result = false;

            if (CoffinBreakConfig.VerboseLogging.Value)
            {
                CoffinBreakPlugin.Log.LogInfo("Refused an end-of-day pass-out: you are away.");
            }
        }
    }
}
