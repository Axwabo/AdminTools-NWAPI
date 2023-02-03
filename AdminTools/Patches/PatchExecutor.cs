using HarmonyLib;
using PluginAPI.Core;
using System;

namespace AdminTools.Patches
{
    public static class PatchExecutor
    {
        private static Harmony _harmony;

        public static void PatchAll()
        {
            _harmony ??= new Harmony("Axwabo.AdminTools-NWAPI");
            try
            {
                _harmony.PatchAll();
            }
            catch (Exception e)
            {
                Log.Error("Patching failed! TargetGhost will not work!\n" + e);
            }
        }

        public static void UnpatchAll() => _harmony.UnpatchAll();
    }
}
