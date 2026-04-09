using HarmonyLib;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Levelling;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.Persistence;
using Il2CppSystem;
using SkillTree.Core.Serialization;
using SkillTree.Core.Skills;
using SkillTree.Core.Utilities;
using System.Collections.Generic;
using static SkillTree.Core.Utilities.ConfigManager;

namespace SkillTree.Core.Patches.Provisioner
{
    [HarmonyPatch]
    public class CraftingSpeedPatches
    {
        private static readonly HashSet<Guid> operations = [];

        [HarmonyPatch(typeof(ChemistryStation), "OnTimePass")]
        [HarmonyPrefix]
        public static void Prefix_ChemistryStation_OnTimePass(ChemistryStation __instance, ref int minutes)
        {
            if (__instance.CurrentCookOperation == null || SkillTreeData.QuickCrafter.CurrentLevel == 0)
                return;

            if (!operations.Contains(__instance.GUID))
            {
                operations.Add(__instance.GUID);
            }

            minutes *= SkillModifiers.GetChemistStationSpeedMultiplier();
        }

        [HarmonyPatch(typeof(ChemistryStation), "OnTimePass")]
        [HarmonyPostfix]
        public static void Postfix_ChemistryStation_OnTimePass(ChemistryStation __instance)
        {
            if (__instance.CurrentCookOperation != null)
                return;

            if (operations.Contains(__instance.GUID))
            {
                if (SkillTreeData.Apprenticeship.CurrentLevel > 0)
                {
                    int xp = DrugProductionXP.GetValue(UseDefault.GetValue());
                    LogManager.LogMessage($"[Apprenticeship] Drug Production XP (Chemistry Station): {xp}", LogLevel.Debug);
                    NetworkSingleton<LevelManager>.Instance.AddXP(xp);
                }
                operations.Remove(__instance.GUID);
            }
        }

        [HarmonyPatch(typeof(MixingStation), "OnTimePass")]
        [HarmonyPrefix]
        public static void Patch_MixingStation_OnTimePass(ref int minutes)
        {
            if (SkillTreeData.QuickCrafter.CurrentLevel == 0)
                return;

            minutes *= SkillModifiers.GetChemistStationSpeedMultiplier();
        }

        [HarmonyPatch(typeof(LabOven), "OnUncappedMinPass")]
        [HarmonyPrefix]
        public static bool Patch_LabOven_OnUncappedMinPass(LabOven __instance)
        {
            if (SkillTreeData.QuickCrafter.CurrentLevel == 0)
                return true;

            if (__instance == null) return false;

            if (__instance.CurrentOperation != null && !__instance.CurrentOperation.IsComplete())
            {
                __instance.CurrentOperation.UpdateCookProgress(1 * SkillModifiers.GetChemistStationSpeedMultiplier());
                if (__instance.CurrentOperation.IsComplete() && !Singleton<LoadManager>.Instance.IsLoading)
                {
                    __instance.DingSound.Play();
                }
            }
            __instance.UpdateOvenAppearance();
            __instance.UpdateLiquid();

            return false;
        }
    }
}
