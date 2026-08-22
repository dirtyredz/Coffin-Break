using UnityEngine;

namespace CoffinBreak
{
    /// <summary>
    /// The state machine: idle in, active out.
    ///
    /// Runs on the plugin's own GameObject, so it lives for the whole session and does not care
    /// which room or scene is loaded.
    /// </summary>
    internal sealed class AfkWatcher : MonoBehaviour
    {
        private readonly ActivityMonitor activity = new ActivityMonitor();

        private float idleSeconds;
        private float unfocusedSeconds;
        private bool armed;
        private float armedRealtime;

        private PauseBadge badge;

        /// <summary>True while this mod is holding the clock. Read by <see cref="PassOutGuard"/>.</summary>
        internal static bool IsPaused { get; private set; }

        /// <summary>Real seconds the clock has been stopped by this mod. Drives the badge.</summary>
        internal float PausedSeconds => armed ? Time.realtimeSinceStartup - armedRealtime : 0f;

        private void Awake()
        {
            badge = gameObject.AddComponent<PauseBadge>();
            badge.Bind(this);
        }

        private void OnDestroy()
        {
            Disarm("component destroyed");
        }

        private void Update()
        {
            if (!CoffinBreakPlugin.Enabled.Value)
            {
                if (armed)
                {
                    Disarm("disabled in config");
                }
                return;
            }

            // Real time, not scaled: a mod about wall-clock absence must not be measured by a
            // clock that other mods can slow down or stop.
            var delta = Time.unscaledDeltaTime;

            if (activity.PollActivity())
            {
                idleSeconds = 0f;
                if (armed)
                {
                    Disarm(activity.LastReason);
                }
            }
            else
            {
                idleSeconds += delta;
            }

            unfocusedSeconds = Application.isFocused ? 0f : unfocusedSeconds + delta;

            if (armed)
            {
                return;
            }

            if (CoffinBreakPlugin.PauseOnFocusLoss.Value &&
                unfocusedSeconds >= CoffinBreakPlugin.FocusLossGraceSeconds.Value)
            {
                Arm("window lost focus");
                return;
            }

            if (idleSeconds >= CoffinBreakPlugin.IdleSeconds.Value)
            {
                Arm($"idle for {idleSeconds:0}s");
            }
        }

        /// <summary>
        /// Regaining focus is activity in itself — you are back at the keyboard even if you have
        /// not clicked yet. Also resyncs the mouse, which has usually jumped while away.
        /// </summary>
        private void OnApplicationFocus(bool focused)
        {
            if (!focused)
            {
                return;
            }

            unfocusedSeconds = 0f;
            idleSeconds = 0f;
            activity.Resync();

            if (armed)
            {
                Disarm("window regained focus");
            }
        }

        // Alt-tabbing on Windows raises focus loss; minimising and some overlays raise pause
        // instead. Both mean gone.
        private void OnApplicationPause(bool paused)
        {
            if (paused && CoffinBreakPlugin.PauseOnFocusLoss.Value && !armed)
            {
                Arm("application suspended");
            }
        }

        private void Arm(string reason)
        {
            if (armed)
            {
                return;
            }

            // Freezing the clock mid-cutscene can stall the scene rather than protect it: the
            // game's own scripted waits run on day progression. Stay out of the way and pick it
            // up on the next frame after the cutscene ends.
            if (!CoffinBreakPlugin.PauseDuringCutscenes.Value && IsInCutscene())
            {
                return;
            }

            DayTimeBlock.Hold();
            if (!DayTimeBlock.IsHeld)
            {
                // The game is not up yet (main menu, still loading). Nothing to protect.
                return;
            }

            armed = true;
            IsPaused = true;
            armedRealtime = Time.realtimeSinceStartup;

            if (CoffinBreakPlugin.VerboseLogging.Value)
            {
                CoffinBreakPlugin.Log.LogInfo($"Clock stopped: {reason}.");
            }
        }

        private void Disarm(string reason)
        {
            if (!armed)
            {
                return;
            }

            var held = PausedSeconds;

            armed = false;
            IsPaused = false;
            DayTimeBlock.Release();

            if (CoffinBreakPlugin.VerboseLogging.Value)
            {
                CoffinBreakPlugin.Log.LogInfo($"Clock restarted after {held:0}s: {reason}.");
            }
        }

        private static bool IsInCutscene() => Safe.Get(() => Cutscene.IsInCutscene, false);
    }
}
