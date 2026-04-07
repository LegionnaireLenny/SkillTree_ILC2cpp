using HarmonyLib;
using Il2CppScheduleOne.Growing;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.ObjectScripts;
using MelonLoader;
using SkillTree.Core.Serialization;
using SkillTree.Core.Skills;
using SkillTree.Core.Utilities;
using System.Collections.Generic;

namespace SkillTree.Core.Patches.Provisioner
{
    [HarmonyPatch]
    public class MushroomPatches
    {
        [HarmonyPatch(typeof(ShroomColony), "ChangeGrowthPercentage")]
        [HarmonyPrefix]
        public static void Patch_ChangeGrowthPercentage(ShroomColony __instance, ref float change)
        {
            if (SkillTreeData.GreenThumb.CurrentLevel == 0)
                return;

            change *= SkillModifiers.GetGrowthSpeedMultiplier();
        }


        private static readonly HashSet<int> processedIds = [];
        [HarmonyPatch(typeof(ShroomColony), "GetHarvestedShroom")]
        [HarmonyPrefix]
        public static void Patch_GetHarvestedShroom(ShroomColony __instance)
        {
            if (__instance == null || SkillTreeData.Mushroomancer.CurrentLevel == 0)
                return;

            int id = __instance.GetInstanceID();
            if (processedIds.Contains(id))
                return;

            float original = __instance.NormalizedQuality;
            __instance.ChangeQuality(SkillModifiers.GetShroomQualityBonus());
            LogManager.LogMessage($"Colony {id} | Quality increased from {ItemQuality.GetQuality(original)} to {ItemQuality.GetQuality(__instance.NormalizedQuality)}", LogLevel.Debug);
            processedIds.Add(id);
        }

        [HarmonyPatch(typeof(MushroomBed), "OnColonyFullyHarvested")]
        [HarmonyPrefix]
        public static void Patch_OnColonyFullyHarvested(MushroomBed __instance)
        {
            if (__instance == null) return;

            int id = __instance.CurrentColony.GetInstanceID();
            if (processedIds.Remove(id))
            {
                LogManager.LogMessage($"Removing fully harvested colony {id} from cache", LogLevel.Debug);
            }
        }
    }
}
