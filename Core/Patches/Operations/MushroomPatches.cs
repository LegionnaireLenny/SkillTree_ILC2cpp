using HarmonyLib;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.GameTime;
using Il2CppScheduleOne.Growing;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.Product;
using MelonLoader;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkillTree.Core.Patches.Operations
{
    [HarmonyPatch]
    public class MushroomPatches
    {
        [HarmonyPatch(typeof(ShroomColony), "ChangeGrowthPercentage")]
        [HarmonyPrefix]
        public static void Patch_ChangeGrowthPercentage(ShroomColony __instance, ref float change)
        {
            if (Core.SkillData.GrowthSpeed == 0 && Core.SkillData.GrowthSpeed2 == 0)
                return;

            change *= SkillModifiers.GetGrowthSpeedMultiplier();
        }

        private static readonly HashSet<int> processedIds = [];
        [HarmonyPatch(typeof(GrowingMushroom), "Harvest")]
        [HarmonyPrefix]
        public static void Patch_Harvest(GrowingMushroom __instance)
        {
            if (Core.SkillData.MoreQuality < 2)
                return;

            int id = __instance._parentColony.GetInstanceID();
            if (processedIds.Contains(id))
                return;

            EQuality original = ItemQuality.GetQuality(__instance._parentColony.NormalizedQuality);
            __instance._parentColony.ChangeQuality(SkillModifiers.GetShroomQualityBonus());
            MelonLogger.Msg($"Mushroom colony quality increased from {original} to {ItemQuality.GetQuality(__instance._parentColony.NormalizedQuality)}");
            processedIds.Add(id);
            MelonCoroutines.Start(CleanUp(id));
        }

        private static IEnumerator CleanUp(int id)
        {
            yield return new WaitForSeconds(120f);
            processedIds.Remove(id);
        }
    }
}
