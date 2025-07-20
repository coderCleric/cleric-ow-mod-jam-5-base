using HarmonyLib;
using NewHorizons.Components.Stars;
using NewHorizons.Handlers;
using NewHorizons.Utility.DebugTools;
using NewHorizons.Utility.OWML;
using UnityEngine;

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
    [HarmonyPatch(typeof(StarController), nameof(StarController.Awake))]
    public static void StarController_Awake(StarController __instance)
    {
        Delay.FireOnNextUpdate(() =>
        {
            if (__instance.Light.range > 5000)
            {
                __instance.Light.range = 5000;
            }
        });
    }
}
