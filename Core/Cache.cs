using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.Equipping;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.Property;
using MelonLoader;
using SkillTree.Core.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace SkillTree.Core
{
    public static class Cache
    {
        // Businesses
        public static readonly Dictionary<string, float> OriginalLaunderCapacity = [];

        // NPCs
        public static readonly Dictionary<string, OriginalCustomer> OriginalCustomers = [];
        public static readonly Dictionary<string, OriginalDealer> OriginalDealers = [];

        // Items
        public static readonly Dictionary<string, int> OriginalItemStackSize = [];
        public static readonly Dictionary<string, OriginalRangedWeapon> OriginalRangedWeapons = [];
        public static readonly Dictionary<string, int> OriginalMagazineSizes = [];

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

        public class OriginalRangedWeapon
        {
            public string Name;
            public float AimDuration;
            public float AccuracyChangeDuration;
            public float Damage;
            public int MagazineSize;
            public float MaxSpread;
            public float MinSpread;
            public int PelletCount;
        }

        public static void FillCache(List<Equippable_RangedWeapon> weapons)
        {
            foreach (Equippable_RangedWeapon weapon in weapons)
            {
                if (!OriginalRangedWeapons.ContainsKey(weapon.name))
                {
                    OriginalRangedWeapons.Add(weapon.name, new OriginalRangedWeapon
                    {
                        Name = weapon.name,
                        AimDuration = weapon.AimDuration,
                        AccuracyChangeDuration = weapon.AccuracyChangeDuration,
                        Damage = weapon.Damage,
                        MagazineSize = weapon.MagazineSize,
                        MaxSpread = weapon.MaxSpread,
                        MinSpread = weapon.MinSpread,
                        PelletCount = weapon.TryCast<Equippable_PumpShotgun>()?.PelletCount ?? 0
                    });
                    LogManager.LogMessage($"[Cache] Cached original stats for {weapon.name}", LogLevel.Debug);

                    if (!OriginalMagazineSizes.ContainsKey(weapon.Magazine.ID))
                    {
                        foreach (var item in Resources.FindObjectsOfTypeAll<IntegerItemDefinition>())
                        {
                            if (weapon.Magazine.ID.Equals(item.ID))
                            {
                                OriginalMagazineSizes.Add(weapon.Magazine.ID, item.DefaultValue);
                                LogManager.LogMessage($"[Cache] Cached original magazine size for {weapon.Magazine.ID}", LogLevel.Debug);
                            }
                        }
                    }
                }
            }
            LogManager.LogMessage("[Cache] Cached original stats for ranged weapons", LogLevel.Info);
        }

        public static void FillCache(List<ItemDefinition> items)
        {
            foreach (ItemDefinition item in items)
            {
                if (!OriginalItemStackSize.ContainsKey(item.name))
                {
                    OriginalItemStackSize.Add(item.name, item.StackLimit);
                    LogManager.LogMessage($"[Cache] Cached stack limit for {item.name}!", LogLevel.Debug);
                }
            }
            LogManager.LogMessage($"[Cache] Cached stack limit for items", LogLevel.Info);
        }

        public static void FillCache(ItemDefinition item)
        {
            if (!OriginalItemStackSize.ContainsKey(item.name))
            {
                OriginalItemStackSize.Add(item.name, item.StackLimit);
                LogManager.LogMessage($"[Cache] Cached original stack limit for {item.name}", LogLevel.Debug);
            }
        }

        public static void FillCache(Business business)
        {
            if (!OriginalLaunderCapacity.ContainsKey(business.PropertyName))
            {
                OriginalLaunderCapacity.Add(business.PropertyName, business.LaunderCapacity);
                LogManager.LogMessage($"[Cache] Cached original laundering capacity {business.PropertyName}", LogLevel.Debug);
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
                LogManager.LogMessage($"[Cache] Cached original customer data for {customer.name}", LogLevel.Debug);
            }
        }

        public static void FillCache(Dealer dealer)
        {
            if (!OriginalDealers.ContainsKey(dealer.name))
            {
                OriginalDealers.Add(dealer.name, new OriginalDealer
                {
                    Name = dealer.name,
                    Cut = dealer.DealerData.SalesCutPercentage,
                    MoveSpeedMultiplier = dealer.Movement.MoveSpeedMultiplier
                });
                LogManager.LogMessage($"[Cache] Cached original dealer data for {dealer.name}", LogLevel.Debug);
            }
        }
    }
}
