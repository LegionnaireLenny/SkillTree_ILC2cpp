using HarmonyLib;
using Il2CppScheduleOne.Property;
using MelonLoader;
using SkillTree.Core.Skills;
using UnityEngine;

namespace SkillTree.Core.Patches.Hustler
{
    [HarmonyPatch]
    public static class BusinessPatches
    {
        public static void SetLaunderingCapacity()
        {
            MelonLogger.Msg($"Adjusting business laundering capacity by {(int)(SkillModifiers.GetLaunderingCapacityMultiplier() % 1 * 100)}%");
            Business[] businessList = Object.FindObjectsOfType<Business>();
            foreach (Business business in businessList)
            {
                if (Cache.OriginalLaunderCapacity.TryGetValue(business.PropertyName, out float original))
                {
                    business.LaunderCapacity = original * SkillModifiers.GetLaunderingCapacityMultiplier();
                    if (!Mathf.Approximately(original, business.LaunderCapacity))
                    {
                        MelonLogger.Msg($"[BusinessEvolving] {business.PropertyName}: ${original} -> ${business.LaunderCapacity}");
                    }
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
