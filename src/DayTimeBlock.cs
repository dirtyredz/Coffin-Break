using System;
using Chicken.Utilities;

namespace CoffinBreak
{
    /// <summary>
    /// The one place that touches the game's clock.
    ///
    /// <see cref="DayProgresser"/> holds a <see cref="Blocker"/> of named string ids and stops
    /// advancing the day while any id is present. That is the game's own mechanism — decorate
    /// mode, the pause menu and the debug scrubber all use it, and <c>Blocker.Add</c> is
    /// distinct-keyed, so two mods holding different ids compose without either one clobbering
    /// the other. Clock Pause and Serena's Grimoire both use it too; <see cref="BlockerId"/> is
    /// ours alone.
    ///
    /// Nothing here writes to the save. Day progression is runtime state that resumes exactly
    /// where it left off.
    /// </summary>
    internal static class DayTimeBlock
    {
        /// <summary>Namespaced so it cannot collide with another mod's id.</summary>
        internal const string BlockerId = "com.dirtyredz.coffinbreak";

        /// <summary>
        /// The name <see cref="DayProgresser"/> gives its own blocker, from its field
        /// initialiser. <c>Blocker</c> registers every instance in a static dictionary keyed by
        /// this, so <c>Blocker.Get</c> hands back the live object and its current id list.
        /// </summary>
        private const string GameBlockerId = "DayProgresser_dayProgressBlocker";

        private static bool held;

        /// <summary>True while this mod is the reason (or a reason) the clock is stopped.</summary>
        internal static bool IsHeld => held;

        /// <summary>
        /// True when the clock is stopped by anyone — us, the pause menu, another mod. Used to
        /// keep the badge honest rather than to make decisions.
        /// </summary>
        internal static bool IsClockStopped
        {
            get
            {
                try
                {
                    var progresser = MonoBehaviourSingleton<DayProgresser>.Instance;
                    return progresser != null && progresser.IsDayProgressionPaused;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// True when something other than this mod is also holding the clock — the pause menu,
        /// decorate mode, an open inventory, Clock Pause, Grimoire's Time Freeze.
        ///
        /// Used only to decide whether saying "time paused" is worth saying. It deliberately
        /// does not gate <see cref="Hold"/>: staying held means that when the menu closes there
        /// is no frame in which the clock is free while the player is still away.
        /// </summary>
        internal static bool IsHeldByAnyoneElse
        {
            get
            {
                try
                {
                    var blocker = Blocker.Get(GameBlockerId);
                    if (blocker == null)
                    {
                        return false;
                    }

                    foreach (var id in blocker.Ids)
                    {
                        if (id != BlockerId)
                        {
                            return true;
                        }
                    }

                    return false;
                }
                catch (Exception)
                {
                    // Never let a cosmetic question break the badge, let alone the Update loop.
                    return false;
                }
            }
        }

        internal static void Hold()
        {
            if (held)
            {
                return;
            }

            try
            {
                // Not MonoBehaviourSingleton<T>.Instance directly: that can construct on access
                // in some singleton implementations, and there is nothing to block before the
                // game is running anyway.
                if (!MonoBehaviourSingleton<DayProgresser>.Exists)
                {
                    return;
                }

                MonoBehaviourSingleton<DayProgresser>.Instance.AddDayTimeBlocker(BlockerId);
                held = true;
            }
            catch (Exception e)
            {
                CoffinBreakPlugin.Log.LogWarning($"Could not stop the clock: {e.Message}");
            }
        }

        internal static void Release()
        {
            if (!held)
            {
                return;
            }

            // Cleared first: if the call below throws we must not be left believing we still hold
            // a blocker we cannot remove, because every later Release would then no-op.
            held = false;

            try
            {
                if (MonoBehaviourSingleton<DayProgresser>.Exists)
                {
                    MonoBehaviourSingleton<DayProgresser>.Instance.RemoveDayTimeBlocker(BlockerId);
                }
            }
            catch (Exception e)
            {
                CoffinBreakPlugin.Log.LogWarning($"Could not restart the clock: {e.Message}");
            }
        }
    }
}
