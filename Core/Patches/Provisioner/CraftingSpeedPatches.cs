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
        private static readonly HashSet<Guid> uncappedOvens = [];

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
            if (__instance == null || SkillTreeData.QuickCrafter.CurrentLevel == 0)
                return true;

            uncappedOvens.Add(__instance.GUID);
            __instance.OnTimePass(1 * SkillModifiers.GetChemistStationSpeedMultiplier());
            return false;
        }

        [HarmonyPatch(typeof(LabOven), "OnTimePass")]
        [HarmonyPrefix]
        public static bool Patch_LabOven_OnTimePass(LabOven __instance, int minutes)
        {
            if (__instance.CurrentOperation != null && !__instance.CurrentOperation.IsComplete())
            {
                if (uncappedOvens.Contains(__instance.GUID) || SkillTreeData.QuickCrafter.CurrentLevel == 0)
                {
                    __instance.CurrentOperation.UpdateCookProgress(minutes);
                    uncappedOvens.Remove(__instance.GUID);
                }
                else
                {
                    __instance.CurrentOperation.UpdateCookProgress(minutes * SkillModifiers.GetChemistStationSpeedMultiplier());
                }

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
