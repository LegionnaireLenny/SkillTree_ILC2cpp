using HarmonyLib;
using Il2CppFishNet;
using Il2CppScheduleOne.Growing;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.Product;
using MelonLoader;

namespace SkillTree.Core.Patches.Operations
{
    [HarmonyPatch]
    public class MoreQuality
    {
        [HarmonyPatch(typeof(LabOven), "Shatter")]
        [HarmonyPrefix]
        public static void Prefix(LabOven __instance)
        {
            if (__instance.CurrentOperation == null || Core.SkillData == null || Core.SkillData.MoreQualityMethCoca == 0)
                return;

            __instance.CurrentOperation.IngredientQuality = SkillModifiers.GetModifiedQuality(__instance.CurrentOperation.IngredientQuality, SkillModifiers.GetMethCocaProductQualityBonus());
        }

        [HarmonyPatch(typeof(ShroomColony), "GetHarvestedShroom")]
        [HarmonyPostfix]
        public static void Postfix(ShroomColony __instance, ref ShroomInstance __result)
        {
            if (__result == null || Core.SkillData == null || Core.SkillData.MoreQuality < 2)
                return;

            EQuality original = __result.Quality;
            __instance.ChangeQuality(SkillModifiers.GetShroomQualityBonus());
            __result.SetQuality(ItemQuality.GetQuality(__instance.NormalizedQuality));
            MelonLogger.Msg($"Mushroom quality increased from {original} to {__result.Quality}");
        }

        [HarmonyPatch(typeof(Plant), "GrowthDone")]
        [HarmonyPrefix]
        public static void GrowthDone_Prefix(Plant __instance)
        {
            if (!InstanceFinder.IsServer || !__instance.Pot.IsSpawned || Core.SkillData == null || (Core.SkillData.Operations == 0 && Core.SkillData.MoreQuality == 0))
                return;

            string potName = __instance.Pot.Name;
            float potBonus = 0f;

            if (potName.Equals("Grow Tent"))
                potBonus = SkillModifiers.GetGrowTentQualityBonus() + SkillModifiers.GetPlantQualityBonus(1);
            else if (potName.Equals("Plastic Pot"))
                potBonus = SkillModifiers.GetPlantQualityBonus(1);
            else if (potName.Equals("Moisture-Preserving Pot"))
                potBonus = SkillModifiers.GetPlantQualityBonus(1);
            else if (potName.Equals("Air Pot"))
                potBonus = SkillModifiers.GetPlantQualityBonus();

            float finalQuality = __instance.QualityLevel + potBonus;
            //MelonLogger.Msg($"[SkillTree] Plant Initialize: {potName} | Base Quality: {__instance.QualityLevel} | Pot Bonus: +{potBonus} | Final: {finalQuality}");
            __instance.QualityLevel = finalQuality;
        }
    }
}
