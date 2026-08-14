using HarmonyLib;
using Il2CppFishNet;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.GameTime;
using Il2CppScheduleOne.Growing;
using SkillTree.Core.Serialization;
using SkillTree.Core.Skills;
using SkillTree.Core.Utilities;

namespace SkillTree.Core.Patches.Provisioner
{
    [HarmonyPatch]
    public class PlantPatches
    {
        [HarmonyPatch(typeof(Plant), "MinPass")]
        [HarmonyPrefix]
        public static bool Prefix(Plant __instance, int mins)
        {
            if (__instance == null || __instance.NormalizedGrowthProgress >= 1f || NetworkSingleton<TimeManager>.Instance.IsEndOfDay)
                return true;

            LogManager.LogMessage("Plant MinPass", LogLevel.DebugVerbose);
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
            if (!InstanceFinder.IsServer ||
                __instance == null ||
                !__instance.Pot.IsSpawned ||
                (SkillTreeData.PitchinATent.CurrentLevel == 0 && SkillTreeData.AdvancedPotTechniques.CurrentLevel == 0 && SkillTreeData.BountifulHarvest.CurrentLevel == 0))
                return;

            SkillModifiers.GetPlantBonuses(__instance.Pot.Name, out float containerQualityBonus, out int containerYieldBonus, out float containerBonusYieldMultiplier);
            float baseQuality = __instance.QualityLevel;
            int baseYield = __instance.BaseYieldQuantity;
            float yieldMultiplier = __instance.YieldMultiplier;
            __instance.QualityLevel += containerQualityBonus;
            __instance.BaseYieldQuantity += containerYieldBonus;
            __instance.YieldMultiplier += containerBonusYieldMultiplier;
            LogManager.LogMessage($"[SkillTree] Plant GrowthDone | {__instance.Pot.GetManagementName()} | Quality: Base,Bonus,Final {baseQuality:0.00},{containerQualityBonus:0.00},{__instance.QualityLevel:0.00} | Yield: Base,Bonus,Final {baseYield},{containerYieldBonus},{__instance.BaseYieldQuantity} | Yield Multiplier: Base,Bonus,Final {yieldMultiplier},{containerBonusYieldMultiplier},{__instance.YieldMultiplier} | ({Il2CppScheduleOne.ItemFramework.ItemQuality.GetQuality(__instance.QualityLevel)} | Yield {UnityEngine.Mathf.RoundToInt(__instance.BaseYieldQuantity * __instance.YieldMultiplier)})", LogLevel.Debug);
        }
    }
}
