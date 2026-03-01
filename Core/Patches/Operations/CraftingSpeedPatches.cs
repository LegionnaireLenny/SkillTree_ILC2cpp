using HarmonyLib;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.Persistence;
using SkillTree.Core.FileManagement;

namespace SkillTree.Core.Patches.Operations
{
    [HarmonyPatch]
    public class CraftingSpeedPatches
    {
        [HarmonyPatch(typeof(ChemistryStation), "OnTimePass")]
        [HarmonyPrefix]
        public static void Prefix(ChemistryStation __instance, ref int minutes)
        {
            if (__instance.CurrentCookOperation == null || SkillTreeData.ChemistStationQuick.CurrentLevel == 0)
                return;

            minutes *= SkillModifiers.GetChemistStationSpeedMultiplier();
        }

        [HarmonyPatch(typeof(MixingStation), "OnTimePass")]
        [HarmonyPrefix]
        public static void Prefix(MixingStation __instance, ref int minutes)
        {
            if (__instance.CurrentMixOperation == null || SkillTreeData.ChemistStationQuick.CurrentLevel == 0)
                return;

            minutes *= SkillModifiers.GetChemistStationSpeedMultiplier();
        }

        [HarmonyPatch(typeof(LabOven), "OnUncappedMinPass")]
        [HarmonyPrefix]
        public static bool Prefix(LabOven __instance)
        {
            if (SkillTreeData.ChemistStationQuick.CurrentLevel == 0)
                return true;

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
