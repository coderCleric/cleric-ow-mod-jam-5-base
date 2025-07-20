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
            // make it stars dont overlap with other systems too much
            // just same range as station range so planets near 2500 arent too dim
            if (__instance.Light.range > 10000)
            {
                __instance.Light.range = 10000;
            }
        });
    }
}
