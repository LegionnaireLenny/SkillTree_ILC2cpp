using HarmonyLib;
using Il2CppFishNet;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.GameTime;
using Il2CppScheduleOne.Growing;
using SkillTree.Core.FileManagement;

namespace SkillTree.Core.Patches.Operations
{
    [HarmonyPatch]
    public class PlantPatches
    {
        [HarmonyPatch(typeof(Plant), "MinPass")]
        [HarmonyPrefix]
        public static bool Prefix(Plant __instance, int mins)
        {
            if (__instance.NormalizedGrowthProgress >= 1f || NetworkSingleton<TimeManager>.Instance.IsEndOfDay)
                return true;

            float num = 1f / (__instance.GrowthTime * 60f) * mins;
            num *= __instance.Pot.GetTemperatureGrowthMultiplier();
            num *= __instance.Pot.GetAverageLightExposure(out var growSpeedMultiplier);
            num *= __instance.Pot.GrowSpeedMultiplier;
            num *= growSpeedMultiplier;
            num *= SkillModifiers.GetGrowthSpeedMultiplier();

            if (GameManager.IS_TUTORIAL)
                num *= 0.3f;

            if (__instance.Pot.NormalizedMoistureAmount <= 0f)
                num *= 0f;

            if (SkillTreeData.AbsorbentSoil.CurrentLevel == 1 && __instance.NormalizedGrowthProgress < 0.5f)
            {
                foreach (var additive in __instance.Pot.AppliedAdditives)
                {
                    if (additive.InstantGrowth > 0f)
                    {
                        num += additive.InstantGrowth;
                        break;
                    }
                }
            }

            __instance.SetNormalizedGrowthProgress(__instance.NormalizedGrowthProgress + num);
            return false;
        }

        [HarmonyPatch(typeof(Plant), "GrowthDone")]
        [HarmonyPrefix]
        public static void Patch_GrowthDone(Plant __instance)
        {
            if (!InstanceFinder.IsServer || !__instance.Pot.IsSpawned || 
                (SkillTreeData.Operations.CurrentLevel == 0 && SkillTreeData.MoreQuality.CurrentLevel == 0 && SkillTreeData.MoreYield.CurrentLevel == 0))
                return;

            //float baseQuality = __instance.QualityLevel;
            float potBonus = 0f;

            if (__instance.Pot.Name.Equals("Grow Tent"))
                potBonus = SkillModifiers.GetGrowTentQualityBonus();
            else if (__instance.Pot.Name.Equals("Plastic Pot") || __instance.Pot.Name.Equals("Moisture-Preserving Pot"))
                potBonus = SkillModifiers.GetPlantQualityBonus(1);
            else if (__instance.Pot.Name.Equals("Air Pot"))
                potBonus = 0.05f + SkillModifiers.GetPlantQualityBonus();

            float finalQuality = __instance.QualityLevel + potBonus;
            __instance.BaseYieldQuantity += SkillModifiers.GetPlantYieldBonus();
            __instance.QualityLevel = finalQuality;
            //MelonLogger.Msg($"[SkillTree] Plant GrowthDone | {__instance.Pot.GetManagementName()} | Base Quality {baseQuality:0.00} | Pot Bonus {potBonus:0.00} | Final Quality {finalQuality:0.00} ({ItemQuality.GetQuality(finalQuality)} | Yield {Mathf.RoundToInt(__instance.BaseYieldQuantity * __instance.YieldMultiplier)})");
        }
    }
}
