using HarmonyLib;
using Il2CppScheduleOne;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.Levelling;
using Il2CppScheduleOne.UI.Shop;
using MelonLoader;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SkillTree.Core.Patches.Miscellaneous
{
    public static class ShopPatches
    {
        private static readonly Dictionary<string, FullRank> TargetRanks = new()
        {
            { "plasticpot",             new FullRank(ERank.Street_Rat, 5) },
            { "suspensionrack",         new FullRank(ERank.Street_Rat, 5) },
            { "halogengrowlight",       new FullRank(ERank.Street_Rat, 5) },
            { "moisturepreservingpot",  new FullRank(ERank.Hoodlum, 3) },
            { "ledgrowlight",           new FullRank(ERank.Hoodlum, 3) },
            { "dryingrack",             new FullRank(ERank.Hoodlum, 5) },
            { "airpot",                 new FullRank(ERank.Peddler, 5) },
            { "brickpress",             new FullRank(ERank.Bagman, 1) }
        };

        private static readonly List<string> itemIdsToInject =
        [
            "moisturepreservingpot",
            "ledgrowlight",
            "plasticpot",
            "halogengrowlight",
            "suspensionrack",
            "airpot",
            "dryingrack"
        ];

        public static void ChangeItemRankRequirements()
        {
            ItemDefinition[] allItems = Singleton<Registry>.Instance.GetAllItems().ToArray();

            int patchedCount = 0;
            foreach (var item in allItems)
            {
                if (item?.ID == null) continue;

                string id = item.ID.ToLowerInvariant();
                if (TargetRanks.TryGetValue(id, out FullRank targetRank))
                {
                    MelonLogger.Msg($"[SkillTree Unlocker] Target item found {item.Name} | {item.ID}");
                    var storable = item.TryCast<StorableItemDefinition>();
                    if (storable != null)
                    {
                        storable.RequiredRank = targetRank;
                        storable.RequiresLevelToPurchase = true;
                        patchedCount++;
                        MelonLogger.Msg($"[SkillTree Unlocker] Item {id} updated to Rank: {targetRank.Rank}, Tier: {targetRank.Tier}");
                    }
                }
            }
            MelonLogger.Msg($"[SkillTree Unlocker] Total of {patchedCount} items successfully remapped.");
        }

        [HarmonyPatch(typeof(ShopInterface), "Awake")]
        [HarmonyPostfix]
        public static void Patch_ShopInterface_Awake(ShopInterface __instance)
        {
            if (__instance == null) return;

            if (__instance.ShopCode.ToLower().Contains("hardware") || __instance.ShopCode.ToLower().Contains("handy_hanks"))
            {
                StorableItemDefinition[] allItems = Resources.FindObjectsOfTypeAll<StorableItemDefinition>();

                foreach (string id in itemIdsToInject)
                {
                    bool alreadyExists = false;

                    foreach (var listing in __instance.Listings)
                    {
                        if (listing?.Item?.ID.ToLower() == id.ToLower())
                        {
                            alreadyExists = true;
                            break;
                        }
                    }

                    if (alreadyExists)
                        continue;

                    StorableItemDefinition targetItem = Array.Find(allItems, x => x?.ID.ToLower() == id);

                    ShopListing newListing = new()
                    {
                        Item = targetItem
                    };

                    if (id == "moisturepreservingpot")
                    {
                        newListing.OverridePrice = true;
                        newListing.OverriddenPrice = 125f;
                    }
                    if (id == "ledgrowlight")
                    {
                        newListing.OverridePrice = true;
                        newListing.OverriddenPrice = 200f;
                    }
                    if (id == "plasticpot")
                    {
                        newListing.OverridePrice = true;
                        newListing.OverriddenPrice = 50f;
                    }
                    if (id == "halogengrowlight")
                    {
                        newListing.OverridePrice = true;
                        newListing.OverriddenPrice = 100f;
                    }
                    if (id == "suspensionrack")
                    {
                        newListing.OverridePrice = true;
                        newListing.OverriddenPrice = 100f;
                    }
                    if (id == "airpot")
                    {
                        newListing.OverridePrice = true;
                        newListing.OverriddenPrice = 300f;
                    }
                    if (id == "dryingrack")
                    {
                        newListing.OverridePrice = true;
                        newListing.OverriddenPrice = 400f;
                    }

                    Traverse trv = Traverse.Create(newListing);

                    if (newListing.IsUnlimitedStock)
                        trv.Field("isUnlimitedStock").SetValue(true);
                    else if (trv.Field("_isUnlimitedStock").FieldExists())
                        trv.Field("_isUnlimitedStock").SetValue(true);

                    __instance.Listings.Add(newListing);

                    newListing.Initialize(__instance);

                    __instance.CreateListingUI(newListing);
                }
            }
        }
    }
}
