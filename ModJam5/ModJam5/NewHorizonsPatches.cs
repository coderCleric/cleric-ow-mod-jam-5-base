using HarmonyLib;
using NewHorizons.Handlers;
using NewHorizons.Utility.DebugTools;

namespace ModJam5;

[HarmonyPatch]
internal class NewHorizonsPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(DebugReload), "ReloadConfigs")]
    public static void DebugReload_ReloadConfigs() => ModJam5.Instance.FixCompatIssues();

    /// <summary>
    /// Don't let any one entry steal the spotlight
    /// </summary>
    [HarmonyPrefix]
    [HarmonyPatch(typeof(TitleSceneHandler), "DisplayBodiesOnTitleScreen")]
    public static bool TitleSceneHandler_DisplayBodiesOnTitleScreen() => false;
}
