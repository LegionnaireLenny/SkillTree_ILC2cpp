using HarmonyLib;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.StationFramework;
using MelonLoader;

namespace SkillTree.Core.Patches.Operations
{
    [HarmonyPatch]
    public class ChemistStationQuick
    {
        [HarmonyPatch(typeof(Cauldron), "OnTimePass")]
        [HarmonyPrefix]
        public static void Prefix(Cauldron __instance, ref int minutes)
        {
            if (__instance.RemainingCookTime <= 0 || Core.SkillData == null || Core.SkillData.ChemistStationQuick == 0)
                return;

            minutes *= SkillModifiers.GetChemistStationSpeedMultiplier();
        }

        [HarmonyPatch(typeof(ChemistryStation), "OnTimePass")]
        [HarmonyPrefix]
        public static void Prefix(ChemistryStation __instance, ref int minutes)
        {
            if (__instance.CurrentCookOperation == null || Core.SkillData == null || Core.SkillData.ChemistStationQuick == 0)
                return;

            minutes *= SkillModifiers.GetChemistStationSpeedMultiplier();
        }

        [HarmonyPatch(typeof(MixingStation), "OnTimePass")]
        [HarmonyPrefix]
        public static void Prefix(MixingStation __instance, ref int minutes)
        {
            if (__instance.CurrentMixOperation == null || Core.SkillData == null || Core.SkillData.ChemistStationQuick == 0)
                return;

            minutes *= SkillModifiers.GetChemistStationSpeedMultiplier();
        }

        [HarmonyPatch(typeof(OvenCookOperation), "GetCookDuration")]
        [HarmonyPostfix]
        public static void Postfix(OvenCookOperation __instance, ref int __result)
        {
            if (Core.SkillData == null || Core.SkillData.ChemistStationQuick == 0)
                return;

            __result = __instance.Ingredient.StationItem.GetModule<CookableModule>().CookTime / SkillModifiers.GetChemistStationSpeedMultiplier();
        }

    }
}
