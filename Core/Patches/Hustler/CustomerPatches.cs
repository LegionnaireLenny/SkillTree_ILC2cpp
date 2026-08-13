using HarmonyLib;
using Il2CppScheduleOne.Economy;
using MelonLoader;
using SkillTree.Core.Serialization;
using SkillTree.Core.Skills;
using SkillTree.Core.Utilities;
using UnityEngine;

namespace SkillTree.Core.Patches.Hustler
{
    [HarmonyPatch]
    public class CustomerPatches
    {
        public static void SetCustomerSpendLimits()
        {
            LogManager.LogMessage($"Increasing customer weekly spending limits by {Mathf.RoundToInt(SkillModifiers.GetCustomerCashMultiplier() % 1 * 100)}%", LogLevel.Info);
            Customer[] customerList = Object.FindObjectsOfType<Customer>();
            foreach (Customer customer in customerList)
            {
                if (Cache.OriginalCustomers.ContainsKey(customer.CustomerData.name))
                {
                    float baseMin = Cache.OriginalCustomers[customer.CustomerData.name].MinWeeklySpend;
                    float baseMax = Cache.OriginalCustomers[customer.CustomerData.name].MaxWeeklySpend;
                    customer.CustomerData.MinWeeklySpend = baseMin * SkillModifiers.GetCustomerCashMultiplier();
                    customer.CustomerData.MaxWeeklySpend = baseMax * SkillModifiers.GetCustomerCashMultiplier();

                    LogManager.LogMessage($"{customer.NPC.FullName}'s spending range increased from {(int)baseMin}-{(int)baseMax} to {(int)customer.CustomerData.MinWeeklySpend}-{(int)customer.CustomerData.MaxWeeklySpend}", LogLevel.Debug);
                }
            }
        }

        public static void SetCustomerOrderLimits()
        {
            LogManager.LogMessage($"Increasing customer order limits by {SkillModifiers.GetCustomerOrderLimitBonus()}", LogLevel.Info);
            Customer[] customerList = Object.FindObjectsOfType<Customer>();
            foreach (Customer customer in customerList)
            {
                if (Cache.OriginalCustomers.ContainsKey(customer.CustomerData.name))
                {
                    int baseMin = Cache.OriginalCustomers[customer.CustomerData.name].MinOrdersPerWeek;
                    int baseMax = Cache.OriginalCustomers[customer.CustomerData.name].MaxOrdersPerWeek;

                    customer.CustomerData.MinOrdersPerWeek = baseMin + SkillModifiers.GetCustomerOrderLimitBonus();
                    customer.CustomerData.MaxOrdersPerWeek = baseMax + SkillModifiers.GetCustomerOrderLimitBonus();

                    LogManager.LogMessage($"{customer.NPC.FullName}'s order range increased from {baseMin}-{baseMax} to {customer.CustomerData.MinOrdersPerWeek}-{customer.CustomerData.MaxOrdersPerWeek}", LogLevel.Debug);
                }
            }
        }


        [HarmonyPatch(typeof(Customer), "GetSampleSuccess")]
        [HarmonyPostfix]
        public static void Postfix(ref float __result)
        {
            if (SkillTreeData.SilverTonguedDevil.CurrentLevel == 0)
                return;

            float origin = __result;
            __result = Mathf.Clamp(__result + SkillModifiers.GetCustomerSampleBonus(), 0f, 1f);
            LogManager.LogMessage($"[SkillTree] Free sample acceptance chance increased from {(int)(origin * 100)}% to {(int)(__result * 100)}%", LogLevel.Debug);
        }

        [HarmonyPatch(typeof(Customer), "Start")]
        [HarmonyPostfix]
        public static void Patch_Customer_Start(Customer __instance)
        {
            Cache.FillCache(__instance);
        }

        //[HarmonyPatch(typeof(Customer), "OnDestroy")]
        //[HarmonyPrefix]
        //public static void Patch_Customer_OnDestroy(Customer __instance)
        //{
        //    if (__instance == null) return;

        //    if (Cache.OriginalCustomers.ContainsKey(__instance.CustomerData.name))
        //    {
        //        Cache.OriginalCustomers.Remove(__instance.CustomerData.name);
        //        LogManager.LogMessage($"Removed {__instance.CustomerData.name} from cache", LogLevel.Debug);
        //    }
        //}
    }
}
