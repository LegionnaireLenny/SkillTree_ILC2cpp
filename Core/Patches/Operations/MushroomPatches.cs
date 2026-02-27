using HarmonyLib;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.GameTime;
using Il2CppScheduleOne.Growing;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.Product;
using MelonLoader;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

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
        [HarmonyPatch(typeof(ShroomColony), "GetHarvestedShroom")]
        [HarmonyPrefix]
        public static void Patch_GetHarvestedShroom(ShroomColony __instance)
        {
            if (Core.SkillData.MoreQuality < 2)
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
