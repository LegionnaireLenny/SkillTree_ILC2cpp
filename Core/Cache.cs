using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.Equipping;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.Property;
using MelonLoader;
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

                    if (!OriginalMagazineSizes.ContainsKey(weapon.Magazine.ID))
                    {
                        foreach (var item in Resources.FindObjectsOfTypeAll<IntegerItemDefinition>())
                        {
                            if (weapon.Magazine.ID.Equals(item.ID))
                            {
                                OriginalMagazineSizes.Add(weapon.Magazine.ID, item.DefaultValue);
                            }
                        }
                    }
                }
            }
            //MelonLogger.Msg("[Cache] Cached original stats for ranged weapons");
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

        public static void FillCache(ItemDefinition item)
        {
            if (!OriginalItemStackSize.ContainsKey(item.name))
            {
                OriginalItemStackSize.Add(item.name, item.StackLimit);
            }
            MelonLogger.Msg($"[Cache] Cached original stack limit for {item.name}");
        }

        public static void FillCache(Business business)
        {
            if (!OriginalLaunderCapacity.ContainsKey(business.PropertyName))
            {
                OriginalLaunderCapacity.Add(business.PropertyName, business.LaunderCapacity);
                //MelonLogger.Msg($"[Cache] Cached original laundering capacity {business.PropertyName}");
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
                //MelonLogger.Msg($"[Cache] Cached original customer data for {customer.name}");
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
                //MelonLogger.Msg($"[Cache] Cached original dealer data for {dealer.name}");
            }
        }
    }
}
