using HarmonyLib;
using Il2CppFishNet;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.GameTime;
using Il2CppScheduleOne.Growing;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.Levelling;
using Il2CppScheduleOne.ObjectScripts;
using SkillTree.Core.Serialization;
using SkillTree.Core.Skills;
using SkillTree.Core.Utilities;
using System.Collections.Generic;
using static SkillTree.Core.Utilities.ConfigManager;

namespace SkillTree.Core.Patches.Provisioner
{
    [HarmonyPatch]
    public class MushroomPatches
    {
        [HarmonyPatch(typeof(ShroomColony), "OnMinPass")]
        [HarmonyPrefix]
        public static bool Prefix_OnMinPass(ShroomColony __instance)
        {
            if (SkillTreeData.GreenThumb.CurrentLevel == 0)
                return true;

            if (NetworkSingleton<TimeManager>.Instance.IsEndOfDay)
            {
                return false;
            }

            float original = __instance.GrowthProgress;
            __instance.ChangeGrowthPercentage(__instance.GetCurrentGrowthRate() / ((float)__instance._growTime * 60f) * SkillModifiers.GetGrowthSpeedMultiplier());
            LogManager.LogMessage($"OnMinPass: Colony Growth Percentage: {original} -> {__instance.GrowthProgress}", LogLevel.DebugVerbose);

            return false;
        }

        [HarmonyPatch(typeof(ShroomColony), "OnTimeSkipped")]
        [HarmonyPrefix]
        public static bool Prefix_OnTimeSkipped(ShroomColony __instance, int mins)
        {
            if (SkillTreeData.GreenThumb.CurrentLevel == 0)
                return true;

            __instance.ChangeGrowthPercentage(__instance.GetCurrentGrowthRate() / ((float)__instance._growTime * 60f) * SkillModifiers.GetGrowthSpeedMultiplier());
            if (InstanceFinder.IsServer)
            {
                __instance.SetGrowthPercentage_Local(null, __instance.GrowthProgress);
            }

            LogManager.LogMessage($"OnTimeSkipped: Colony Growth Percentage: {__instance.GrowthProgress}", LogLevel.Debug);
            return false;
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

            int xp = BaseHarvestXPGain.GetValue(UseDefault.GetValue()) * SkillModifiers.GetHarvestXPMultiplier();
            LogManager.LogMessage($"[Apprenticeship] Base Harvest XP (Shroom): {BaseHarvestXPGain.GetValue(UseDefault.GetValue())} | XP Gained: {xp} | Skill Multiplier: x{SkillModifiers.GetHarvestXPMultiplier()}", LogLevel.Debug);
            NetworkSingleton<LevelManager>.Instance.AddXP(xp);

            int id = __instance.CurrentColony.GetInstanceID();
            if (processedIds.Remove(id))
            {
                LogManager.LogMessage($"Removing fully harvested colony {id} from cache", LogLevel.Debug);
            }
        }
    }
}
