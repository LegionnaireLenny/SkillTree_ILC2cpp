using HarmonyLib;
using Il2CppScheduleOne.Property;
using MelonLoader;
using SkillTree.Core.Skills;
using SkillTree.Core.Utilities;
using UnityEngine;

namespace SkillTree.Core.Patches.Hustler
{
    [HarmonyPatch]
    public static class BusinessPatches
    {
        public static void SetLaunderingCapacity()
        {
            LogManager.LogMessage($"Increasing business laundering capacity by {Mathf.RoundToInt(SkillModifiers.GetLaunderingCapacityMultiplier() % 1 * 100)}%", LogLevel.Info);
            Business[] businessList = Object.FindObjectsOfType<Business>();
            foreach (Business business in businessList)
            {
                if (Cache.OriginalLaunderCapacity.TryGetValue(business.PropertyName, out float original))
                {
                    business.LaunderCapacity = original * SkillModifiers.GetLaunderingCapacityMultiplier();
                    LogManager.LogMessage($"{business.PropertyName}: ${original} -> ${business.LaunderCapacity}", LogLevel.Debug);
                }
            }
        }

        [HarmonyPatch(typeof(Business), "Start")]
        [HarmonyPostfix]
        public static void Patch_Dealer_Start(Business __instance)
        {
            Cache.FillCache(__instance);
        }
    }
}
