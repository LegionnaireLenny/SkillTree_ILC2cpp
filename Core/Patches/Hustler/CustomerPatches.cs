using HarmonyLib;
using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.Quests;
using SkillTree.Core.Serialization;
using SkillTree.Core.Skills;
using SkillTree.Core.Utilities;
using System.Collections.Generic;
using UnityEngine;
using static SkillTree.Core.Utilities.Formatter;

namespace SkillTree.Core.Patches.Hustler
{
    [HarmonyPatch]
    public class CustomerPatches
    {
        public static void SetCustomerSpendLimits()
        {
            LogManager.LogMessage($"Increasing customer weekly spending limits by {FormatAsPercentage(SkillModifiers.GetCustomerCashMultiplier() - 1)}", LogLevel.Info);
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

            float original = __result;
            __result = Mathf.Clamp(__result + SkillModifiers.GetCustomerSampleBonus(), 0f, 1f);
            LogManager.LogMessage($"[SkillTree] Free sample acceptance chance increased from {FormatAsPercentage(original)} to {FormatAsPercentage(__result)}", LogLevel.Debug);
        }

        [HarmonyPatch(typeof(Customer), "Start")]
        [HarmonyPostfix]
        public static void Patch_Customer_Start(Customer __instance)
        {
            Cache.FillCache(__instance);
        }

        [HarmonyPatch(typeof(Contract), "GetProductListMatch")]
        [HarmonyPostfix]
        public static void Patch_Contract_GetProductListMatch(Contract __instance, List<ItemInstance> items, ref int matchedProductCount, ref float __result)
        {
            if (SkillTreeData.Munificent.CurrentLevel > 0)
            {
                if (matchedProductCount > __instance.ProductList.GetTotalQuantity())
                {
                    int excessProductCount = matchedProductCount - __instance.ProductList.GetTotalQuantity();
                    int bonusProduct = excessProductCount * SkillModifiers.GetGenerosityExcessBonus();
                    matchedProductCount += bonusProduct;
                    LogManager.LogMessage($"[Generous] Order Total: {__instance.ProductList.GetTotalQuantity()} | Excess: {excessProductCount} | Bonus: {bonusProduct} | Matched + Bonus: {matchedProductCount}", LogLevel.Debug);
                }

            }

            if (SkillTreeData.Charlatan.CurrentLevel > 0)
            {
                float bonus = SkillModifiers.GetProductShortChanceBonus();
                float original = __result;
                __result = Mathf.Clamp01(__result + bonus);
                LogManager.LogMessage($"[Charlatan] Customer short success chance increased by {FormatAsPercentage(bonus)} from {FormatAsPercentage(original)} to {FormatAsPercentage(__result)}", LogLevel.Debug);
            }
        }
    }
}
