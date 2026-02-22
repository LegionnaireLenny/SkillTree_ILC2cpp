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
    public class QualityPatches
    {
        [HarmonyPatch(typeof(LabOven), "Shatter")]
        [HarmonyPrefix]
        public static void Prefix(LabOven __instance)
        {
            if (__instance.CurrentOperation == null || Core.SkillData == null || Core.SkillData.MoreQualityMethCoca == 0)
                return;

            __instance.CurrentOperation.IngredientQuality = ItemQuality.ShiftQuality(__instance.CurrentOperation.IngredientQuality, SkillModifiers.GetMethCocaProductQualityBonus());
        }
    }
}
