using HarmonyLib;
using Il2CppFishNet;
using Il2CppScheduleOne.Growing;
using MelonLoader;
using UnityEngine;

namespace SkillTree.Core.Patches.Operations
{
    /// <summary>
    /// CHANGE QUALITY SYSTEM BY POT TYPE -- BETTER POT = BETTER QUALITY
    /// </summary>



    /// <summary>
    /// ADD YIELD FROM PLANTS
    /// </summary>

    [HarmonyPatch(typeof(Plant), "GrowthDone")]
    public static class MoreYield
    {
        [HarmonyPrefix]
        public static void Prefix(Plant __instance)
        {
            if (!InstanceFinder.IsServer || Core.SkillData == null || Core.SkillData.MoreYield == 0)
                return;

            var currentMultiplier = __instance.YieldMultiplier;
            var originalBase = __instance.BaseYieldQuantity;

            MelonLogger.Msg($"[GrowthDone_SmartBasePatch] Yield multiplier {__instance.YieldMultiplier}. Base yield: {__instance.BaseYieldQuantity}");
            if (Mathf.Approximately(currentMultiplier, 1.0f) && originalBase == 12)
            {
                int finalBase = originalBase + SkillModifiers.YieldBonusPlants; 

                __instance.BaseYieldQuantity = finalBase; 
                MelonLogger.Msg($"[GrowthDone_SmartBasePatch] No additives detected. Skill applied. New Base: {finalBase}");
            }
        }
    }
}
