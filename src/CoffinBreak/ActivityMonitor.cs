using System;
using Chicken.Utilities;
using UnityEngine;

namespace CoffinBreak
{
    /// <summary>
    /// Answers one question per frame: did the player do anything?
    ///
    /// Deliberately built on <see cref="Input"/> (the legacy module, which this game ships)
    /// rather than on Rewired, the game's own input layer. Rewired needs a player id and an
    /// action map that could be renamed by a patch; <c>Input.anyKey</c> cannot break that way and
    /// an idle detector that mistakenly thinks you are present is a mod that quietly does
    /// nothing.
    ///
    /// The one thing legacy input misses is analogue stick movement — sticks are axes, not keys,
    /// and this game's axes are owned by Rewired rather than Unity's InputManager. That gap is
    /// covered by watching the character actually move, which is a truer signal than any button
    /// anyway.
    /// </summary>
    internal sealed class ActivityMonitor
    {
        /// <summary>
        /// Mouse jitter below this many pixels in a frame is ignored. Optical mice drift by a
        /// pixel or two on a still desk, and treating that as presence would defeat the mod.
        /// </summary>
        private const float MouseMoveThreshold = 2f;

        /// <summary>
        /// The character drifts fractionally while an idle animation plays. Only real walking
        /// should count.
        /// </summary>
        private const float PlayerMoveThreshold = 0.05f;

        private Vector3 lastMousePosition;
        private Vector3 lastPlayerPosition;
        private bool hasPlayerPosition;
        private bool initialised;

        /// <summary>What last counted as activity. Shown in verbose logs, not to the player.</summary>
        internal string LastReason { get; private set; } = "startup";

        /// <summary>
        /// Poll everything once. Call exactly once per frame — mouse and player deltas are
        /// measured against the previous call.
        /// </summary>
        internal bool PollActivity()
        {
            if (!initialised)
            {
                initialised = true;
                lastMousePosition = SafeMousePosition();
                return true;
            }

            var active = false;

            // anyKey covers the keyboard, all mouse buttons and every gamepad button.
            if (SafeAnyKey())
            {
                LastReason = "key or button";
                active = true;
            }

            var mouse = SafeMousePosition();
            if ((mouse - lastMousePosition).sqrMagnitude > MouseMoveThreshold * MouseMoveThreshold)
            {
                LastReason = "mouse moved";
                active = true;
            }
            lastMousePosition = mouse;

            if (!active && SafeScrolled())
            {
                LastReason = "scroll wheel";
                active = true;
            }

            if (!active &&
                CoffinBreakPlugin.CountPlayerMovementAsActivity.Value &&
                PlayerMoved())
            {
                LastReason = "character moved";
                active = true;
            }

            return active;
        }

        /// <summary>
        /// Forget the cached mouse and player positions so the next poll cannot read a huge
        /// bogus delta. Used when regaining focus, where the cursor has usually jumped.
        /// </summary>
        internal void Resync()
        {
            lastMousePosition = SafeMousePosition();
            hasPlayerPosition = false;
        }

        private bool PlayerMoved()
        {
            Vector3 position;
            try
            {
                if (!MonoBehaviourSingleton<PlayerView>.Exists)
                {
                    hasPlayerPosition = false;
                    return false;
                }

                position = MonoBehaviourSingleton<PlayerView>.Instance.transform.position;
            }
            catch (Exception)
            {
                hasPlayerPosition = false;
                return false;
            }

            if (!hasPlayerPosition)
            {
                hasPlayerPosition = true;
                lastPlayerPosition = position;
                return false;
            }

            var moved = (position - lastPlayerPosition).sqrMagnitude >
                        PlayerMoveThreshold * PlayerMoveThreshold;
            lastPlayerPosition = position;
            return moved;
        }

        // Unity throws rather than returns a default when a platform lacks a device, and a
        // throwing idle check would take the whole Update loop down with it.

        private static bool SafeAnyKey()
        {
            try
            {
                return UnityEngine.Input.anyKey;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static Vector3 SafeMousePosition()
        {
            try
            {
                return UnityEngine.Input.mousePosition;
            }
            catch (Exception)
            {
                return Vector3.zero;
            }
        }

        private static bool SafeScrolled()
        {
            try
            {
                return Mathf.Abs(UnityEngine.Input.mouseScrollDelta.y) > 0.01f;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
