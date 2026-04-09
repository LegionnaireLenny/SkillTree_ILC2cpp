using HarmonyLib;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.Levelling;
using Il2CppScheduleOne.ObjectScripts;
using SkillTree.Core.Serialization;
using SkillTree.Core.Skills;
using SkillTree.Core.Utilities;
using static SkillTree.Core.Utilities.ConfigManager;

namespace SkillTree.Core.Patches.Provisioner
{
    [HarmonyPatch]
    public class QualityPatches
    {
        [HarmonyPatch(typeof(LabOven), "Shatter")]
        [HarmonyPrefix]
        public static void Prefix(LabOven __instance)
        {
            if (__instance?.CurrentOperation == null || SkillTreeData.HarderAndStronger.CurrentLevel == 0)
                return;

            __instance.CurrentOperation.IngredientQuality = ItemQuality.ShiftQuality(__instance.CurrentOperation.IngredientQuality, SkillModifiers.GetMethCocaProductQualityBonus());
            int xp = DrugProductionXP.GetValue(UseDefault.GetValue());
            LogManager.LogMessage($"[Meister] Drug Production XP (Shatter): {xp}", LogLevel.Debug);
            NetworkSingleton<LevelManager>.Instance.AddXP(xp);
        }
    }
}
