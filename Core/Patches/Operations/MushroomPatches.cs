using HarmonyLib;
using Il2CppScheduleOne.Growing;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.ObjectScripts;
using MelonLoader;
using SkillTree.Core.FileManagement;
using System.Collections.Generic;

namespace SkillTree.Core.Patches.Operations
{
    [HarmonyPatch]
    public class MushroomPatches
    {
        [HarmonyPatch(typeof(ShroomColony), "ChangeGrowthPercentage")]
        [HarmonyPrefix]
        public static void Patch_ChangeGrowthPercentage(ShroomColony __instance, ref float change)
        {
            if (SkillTreeData.GrowthSpeed.CurrentLevel == 0 && SkillTreeData.GrowthSpeed2.CurrentLevel == 0)
                return;

            change *= SkillModifiers.GetGrowthSpeedMultiplier();
        }


        private static readonly HashSet<int> processedIds = [];
        [HarmonyPatch(typeof(ShroomColony), "GetHarvestedShroom")]
        [HarmonyPrefix]
        public static void Patch_GetHarvestedShroom(ShroomColony __instance)
        {
            if (SkillTreeData.MoreQuality.CurrentLevel < 2)
                return;

            int id = __instance.GetInstanceID();
            if (processedIds.Contains(id))
                return;

            float original = __instance.NormalizedQuality;
            __instance.ChangeQuality(SkillModifiers.GetShroomQualityBonus());
            MelonLogger.Msg($"Colony {id} | Quality increased from {ItemQuality.GetQuality(original)} to {ItemQuality.GetQuality(__instance.NormalizedQuality)}");
            processedIds.Add(id);
        }

        [HarmonyPatch(typeof(MushroomBed), "OnColonyFullyHarvested")]
        [HarmonyPrefix]
        public static void Patch_OnColonyFullyHarvested(MushroomBed __instance)
        {
            int id = __instance.CurrentColony.GetInstanceID();
            if (processedIds.Remove(id))
            {
                MelonLogger.Msg($"Removing fully harvested colony {id} from cache"); ;
            }
        }
    }
}
