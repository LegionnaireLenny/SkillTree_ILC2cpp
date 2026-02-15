using HarmonyLib;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.GameTime;
using Il2CppScheduleOne.Growing;
using MelonLoader;

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

            //MelonLogger.Msg($"Plant_MinPass_Patch enter: growth progress {__instance.NormalizedGrowthProgress}");

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

            //MelonLogger.Msg($" Before Growth Plant  {__instance.NormalizedGrowthProgress}");
            //MelonLogger.Msg($" Add Growth Plant  {__instance.NormalizedGrowthProgress}");
            __instance.SetNormalizedGrowthProgress(__instance.NormalizedGrowthProgress + num);
            //MelonLogger.Msg($" After Growth Plant {__instance.NormalizedGrowthProgress}");

            //MelonLogger.Msg($"Plant_MinPass_Patch growth progress boosted {__instance.NormalizedGrowthProgress + num}");
            return false;
        }

        [HarmonyPatch(typeof(ShroomColony), "ChangeGrowthPercentage")]
        [HarmonyPrefix]
        public static void Prefix(ShroomColony __instance, ref float change)
        {
            if (Core.SkillData.GrowthSpeed == 0 && Core.SkillData.GrowthSpeed2 == 0)
                return;

            if (change > 0f)
            {
                MelonLogger.Msg($" Growth Shroom {__instance.GrowthProgress}");
                MelonLogger.Msg($" Before Shroom change {change}");
                change *= SkillModifiers.GetGrowthSpeedMultiplier();
                MelonLogger.Msg($" After Shroom change {change}");
            }
        }
    }
}
