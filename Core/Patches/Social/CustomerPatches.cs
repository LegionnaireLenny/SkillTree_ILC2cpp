using HarmonyLib;
using Il2CppScheduleOne.Economy;
using MelonLoader;
using SkillTree.Core.Skills;
using System.Linq;
using UnityEngine;

namespace SkillTree.Core.Patches.Social
{
    [HarmonyPatch]
    public class CustomerPatches
    {
        [HarmonyPatch(typeof(Customer), "GetSampleSuccess")]
        [HarmonyPostfix]
        public static void Postfix(ref float __result)
        {
            if (SkillTreeData.Social.CurrentLevel == 0) 
                return;

            float origin = __result;
            __result = Mathf.Clamp(__result + SkillModifiers.GetCustomerSampleBonus(), 0f, 1f);
            MelonLogger.Msg($"[SkillTree] Free sample acceptance chance increased from {(int)(origin * 100)}% to {(int)(__result * 100)}%");

        }

        public static void SetCustomerSpendLimits()
        {
            Customer[] customerList = customerList = UnityEngine.Object.FindObjectsOfType<Customer>();
            Cache.FillCache(customerList.ToList());

            if (SkillTreeData.CityEvolving.CurrentLevel != 0)
            {
                MelonLogger.Msg($"[CityEvolving] Increasing customer weekly spending limit by {(int)(SkillModifiers.GetCustomerCashMultiplier() % 1 * 100)}%");
            }

            foreach (Customer customer in customerList)
            {
                if (Cache.OriginalMinSpend.TryGetValue(customer.CustomerData.name, out float baseMin) &&
                    Cache.OriginalMaxSpend.TryGetValue(customer.CustomerData.name, out float baseMax))
                {
                    customer.CustomerData.MinWeeklySpend = baseMin * SkillModifiers.GetCustomerCashMultiplier();
                    customer.CustomerData.MaxWeeklySpend = baseMax * SkillModifiers.GetCustomerCashMultiplier();

                    if (!Mathf.Approximately(baseMin, customer.CustomerData.MinWeeklySpend))
                    {
                        //MelonLogger.Msg($"[CityEvolving] {customer.NPC.fullName}'s spending range increased from {(int)baseMin}-{(int)baseMax} to {(int)customer.CustomerData.MinWeeklySpend}-{(int)customer.CustomerData.MaxWeeklySpend}");
                    }
                }
            }
        }
    }
}
