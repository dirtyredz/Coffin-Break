using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace CoffinBreak
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInProcess("Moonlight Peaks.exe")]
    public sealed class CoffinBreakPlugin : BaseUnityPlugin
    {
        public const string PluginGuid = "com.dirtyredz.moonlightpeaks.coffinbreak";
        public const string PluginName = "Coffin Break";
        // Keep in step with <Version> in the csproj - pack.ps1 names the archive from the
        // csproj while BepInEx reports this one. See 12-versioning-and-release.md.
        public const string PluginVersion = ModBuildInfo.Version;

        internal static ManualLogSource Log;

        private Harmony harmony;

        private void Awake()
        {
            Log = Logger;

            CoffinBreakConfig.Bind(Config);

            harmony = new Harmony(PluginGuid);
            PassOutGuard.Apply(harmony);

            gameObject.AddComponent<AfkWatcher>();

            Log.LogInfo(
                $"{PluginName} {PluginVersion} loaded. Read-only: nothing is written to your save.");
            Log.LogInfo(
                $"Clock stops after {CoffinBreakConfig.IdleSeconds.Value:0.#}s idle" +
                (CoffinBreakConfig.PauseOnFocusLoss.Value ? ", or on losing window focus." : ".") +
                $" Application.runInBackground={Application.runInBackground}.");
        }

        private void OnDestroy()
        {
            // Leaving a blocker behind would freeze the player's clock permanently, which is the
            // exact failure mode Serena's Grimoire had to warn people about. Always let go.
            DayTimeBlock.Release();
            harmony?.UnpatchSelf();
        }
    }
}
