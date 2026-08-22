using System;

namespace CoffinBreak
{
    /// <summary>
    /// Runs a call that reaches into Unity or the game and might throw, turning the throw into a
    /// quiet fallback (or a logged warning) rather than letting it take the caller down.
    ///
    /// Unity throws instead of returning a default when a platform lacks a device, and the game's
    /// singletons can be absent before the game is up — so a bare call in the per-frame Update loop
    /// or a badge redraw is a latent crash. Every such call in this mod's own code goes through
    /// here; the vendored files (GameFonts et al.) keep their own guards to stay verbatim-synced.
    /// </summary>
    internal static class Safe
    {
        /// <summary>
        /// Return <paramref name="op"/>'s result, or <paramref name="fallback"/> if it throws.
        /// For reads where a failure just means "assume nothing happened" — no log worth writing.
        /// </summary>
        internal static T Get<T>(Func<T> op, T fallback = default)
        {
            try
            {
                return op();
            }
            catch (Exception)
            {
                return fallback;
            }
        }

        /// <summary>
        /// Run <paramref name="op"/>; if it throws, log "<paramref name="warning"/>: {message}"
        /// instead of propagating. For actions where a failure is worth a line in the log.
        /// </summary>
        internal static void Do(Action op, string warning)
        {
            try
            {
                op();
            }
            catch (Exception e)
            {
                CoffinBreakPlugin.Log.LogWarning($"{warning}: {e.Message}");
            }
        }
    }
}
