using HarmonyLib;
using Il2CppScheduleOne.Property;
using MelonLoader;
using SkillTree.Core.Serialization;
using SkillTree.Core.Skills;
using System.Linq;
using UnityEngine;

namespace SkillTree.Core.Patches.Hustler
{
    [HarmonyPatch]
    public static class BusinessPatches
    {
        public static void SetLaunderingCapacity()
        {
            if (SkillTreeData.SqueakyClean.CurrentLevel != 0)
            {
                MelonLogger.Msg($"[BusinessEvolving] Increasing business laundering capacity by {(int)(SkillModifiers.GetLaunderingCapacityMultiplier() % 1 * 100)}%");
            }

            Business[] businessList = Object.FindObjectsOfType<Business>();
            Cache.FillCache(businessList.ToList());
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
    }
}
