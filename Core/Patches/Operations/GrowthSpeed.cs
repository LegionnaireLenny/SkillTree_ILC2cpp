using HarmonyLib;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.GameTime;
using Il2CppScheduleOne.Growing;
using Il2CppScheduleOne.ItemFramework;

namespace SkillTree.Core.Patches.Operations
{
    [HarmonyPatch]
    public class GrowthSpeed
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

            if (Core.SkillData.AbsorbentSoil == 1 && __instance.NormalizedGrowthProgress < 0.5f)
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

        [HarmonyPatch(typeof(ShroomColony), "ChangeGrowthPercentage")]
        [HarmonyPrefix]
        public static void Prefix(ShroomColony __instance, ref float change)
        {
            if (Core.SkillData.GrowthSpeed == 0 && Core.SkillData.GrowthSpeed2 == 0)
                return;

            change *= SkillModifiers.GetGrowthSpeedMultiplier();
        }
    }
}
