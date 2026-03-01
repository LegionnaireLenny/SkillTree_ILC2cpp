using HarmonyLib;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.ObjectScripts;
using SkillTree.Core.FileManagement;

namespace SkillTree.Core.Patches.Operations
{
    [HarmonyPatch]
    public class QualityPatches
    {
        [HarmonyPatch(typeof(LabOven), "Shatter")]
        [HarmonyPrefix]
        public static void Prefix(LabOven __instance)
        {
            if (__instance.CurrentOperation == null || SkillTreeData.MoreQualityMethCoca.CurrentLevel == 0)
                return;

            __instance.CurrentOperation.IngredientQuality = ItemQuality.ShiftQuality(__instance.CurrentOperation.IngredientQuality, SkillModifiers.GetMethCocaProductQualityBonus());
        }
    }
}
