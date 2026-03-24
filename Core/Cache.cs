using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.Property;
using MelonLoader;
using System.Collections.Generic;

namespace SkillTree.Core
{
    public static class Cache
    {
        // Businesses
        public static readonly Dictionary<string, float> OriginalLaunderCapacity = [];

        public static readonly Dictionary<string, OriginalCustomer> OriginalCustomers = [];
        public static readonly Dictionary<string, OriginalDealer> OriginalDealers = [];

        // Items
        public static readonly Dictionary<string, int> OriginalItemStackSize = [];

        public class OriginalCustomer
        {
            public string Name { get; set; }
            public float MinWeeklySpend { get; set; }
            public float MaxWeeklySpend { get; set; }
            public int MinOrdersPerWeek { get; set; }
            public int MaxOrdersPerWeek { get; set; }
        }

        public class OriginalDealer
        {
            public string Name { get; set; }
            public float Cut { get; set; }
            public float MoveSpeedMultiplier { get; set; }
        }

        public static void FillCache(List<ItemDefinition> items)
        {
            foreach (ItemDefinition item in items)
            {
                if (!OriginalItemStackSize.ContainsKey(item.name))
                {
                    OriginalItemStackSize.Add(item.name, item.StackLimit);
                }
            }
            MelonLogger.Msg("[Cache] Successfully cached stack limits for each item!");
        }

        public static void FillCache(Business business)
        {
            if (!OriginalLaunderCapacity.ContainsKey(business.PropertyName))
            {
                OriginalLaunderCapacity.Add(business.PropertyName, business.LaunderCapacity);
                MelonLogger.Msg($"[Cache] Cached original laundering capacity {business.PropertyName}");
            }
        }

        public static void FillCache(Customer customer)
        {
            if (!OriginalCustomers.ContainsKey(customer.CustomerData.name))
            {
                OriginalCustomers.Add(customer.CustomerData.name, new OriginalCustomer
                {
                    Name = customer.CustomerData.name,
                    MinWeeklySpend = customer.CustomerData.MinWeeklySpend,
                    MaxWeeklySpend = customer.CustomerData.MaxWeeklySpend,
                    MinOrdersPerWeek = customer.CustomerData.MinOrdersPerWeek,
                    MaxOrdersPerWeek = customer.CustomerData.MaxOrdersPerWeek
                });
                MelonLogger.Msg($"[Cache] Cached original customer data for {customer.name}");
            }
        }

        public static void FillCache(Dealer dealer)
        {
            if (!OriginalDealers.ContainsKey(dealer.name))
            {
                OriginalDealers.Add(dealer.name, new OriginalDealer
                {
                    Name = dealer.name,
                    Cut = dealer.Cut,
                    MoveSpeedMultiplier = dealer.Movement.MoveSpeedMultiplier
                });
                MelonLogger.Msg($"[Cache] Cached original dealer data for {dealer.name}");
            }
        }
    }
}
