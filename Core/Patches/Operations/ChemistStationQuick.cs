using HarmonyLib;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.StationFramework;
using MelonLoader;

namespace SkillTree.Core.Patches.Operations
{
    [HarmonyPatch]
    public class ChemistStationQuick
    {
        // TODO: test
        [HarmonyPatch(typeof(Cauldron), "OnTimePass")]
        [HarmonyPrefix]
        public static void Prefix(Cauldron __instance, ref int minutes)
        {
            if (__instance.RemainingCookTime <= 0 || Core.SkillData == null || Core.SkillData.ChemistStationQuick == 0)
                return;

            minutes *= SkillModifiers.GetChemistStationSpeedMultiplier();
            MelonLogger.Msg($"Patch_Cauldron_OnTimePass progress {minutes} minutes");
        }

        [HarmonyPatch(typeof(ChemistryStation), "OnTimePass")]
        [HarmonyPostfix]
        public static void Postfix(ChemistryStation __instance, int minutes)
        {
            if (__instance.CurrentCookOperation == null || Core.SkillData == null || Core.SkillData.ChemistStationQuick == 0)
                return;

            // Reduce the multiplier by one to account for Progress being called in the original function
            __instance.CurrentCookOperation.Progress(minutes * (SkillModifiers.GetChemistStationSpeedMultiplier() - 1));
        }

        [HarmonyPatch(typeof(OvenCookOperation), "GetCookDuration")]
        [HarmonyPostfix]
        public static void Postfix(OvenCookOperation __instance, ref int __result)
        {
            if (Core.SkillData == null || Core.SkillData.ChemistStationQuick == 0)
                return;

            __result = __instance.Ingredient.StationItem.GetModule<CookableModule>().CookTime / SkillModifiers.GetChemistStationSpeedMultiplier();
        }

        // TODO: fix. Doesn't work. Mix timer goes into negative and completes at the normal time
        [HarmonyPatch(typeof(MixingStation), "GetMixTimeForCurrentOperation")]
        [HarmonyPostfix]
        public static void Postfix(MixingStation __instance, ref int __result)
        {
            if (__instance.CurrentMixOperation == null || Core.SkillData == null || Core.SkillData.ChemistStationQuick == 0)
                return;

            __result = __instance.MixTimePerItem * __instance.CurrentMixOperation.Quantity / SkillModifiers.GetChemistStationSpeedMultiplier();
        }
    }
}
