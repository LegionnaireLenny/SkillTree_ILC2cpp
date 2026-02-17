using HarmonyLib;
using Il2CppScheduleOne.Economy;
using MelonLoader;
using UnityEngine;

namespace SkillTree.Core.Patches.Social
{
    [HarmonyPatch]
    public class CustomerPatches
    {
        [HarmonyPatch(typeof(Customer), "GetSampleSuccess")]
        [HarmonyPostfix]
        public static void Postfix(ref float __result, float __state)
        {
            if (Core.SkillData == null || Core.SkillData.Social == 0) 
                return;

            float origin = __result;
            __result = Mathf.Clamp(__result + SkillModifiers.GetCustomerSampleBonus(), 0f, 1f);
            MelonLogger.Msg($"[SkillTree] Free sample acceptance chance increased from {origin:P0} to {__result:P0}");

        }

        public static void SetCustomerSpendLimits()
        {
            Customer[] customerList = customerList = UnityEngine.Object.FindObjectsOfType<Customer>();
            Cache.FillCache(customerList.ToList());
            foreach (Customer customer in customerList)
            {
                if (Cache.OriginalMinSpend.TryGetValue(customer.CustomerData.name, out float baseMin) &&
                    Cache.OriginalMaxSpend.TryGetValue(customer.CustomerData.name, out float baseMax))
                {
                    customer.CustomerData.MinWeeklySpend = baseMin + (baseMin * SkillModifiers.GetCustomerCashMultiplier());
                    customer.CustomerData.MaxWeeklySpend = baseMax + (baseMax * SkillModifiers.GetCustomerCashMultiplier());

                    MelonLogger.Msg($"[CityEvolving] {customer.CustomerData.name}'s spending range increased from {baseMin}-{baseMax} to {customer.CustomerData.MinWeeklySpend}-{customer.CustomerData.MaxWeeklySpend}");
                }
            }
            MelonLogger.Msg($"Weekly spend increased by {SkillModifiers.GetCustomerCashMultiplier() % 1 * 100}%");
        }
    }
}
