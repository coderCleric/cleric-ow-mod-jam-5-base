using HarmonyLib;
using NewHorizons.Handlers;
using NewHorizons.Utility.DebugTools;
using NewHorizons.Utility.OWML;

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

    [HarmonyPostfix]
    [HarmonyPatch(typeof(SunLightController), nameof(SunLightController.Awake))]
    public static void SunLightController_Awake(SunLightController __instance)
    {
        Delay.FireOnNextUpdate(() =>
        {
            if (__instance._sunLight.range > 2500)
            {
                __instance._sunLight.range = 2500;
            }
        });
    }
}
