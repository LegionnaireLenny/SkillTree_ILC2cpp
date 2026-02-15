using Il2CppScheduleOne;
using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.Employees;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.Management;
using Il2CppScheduleOne.Money;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.Property;
using Il2CppScheduleOne.Tools;
using Il2CppScheduleOne.UI;
using Il2CppScheduleOne.UI.ATM;
using MelonLoader;
using SkillTree.Json;
using SkillTree.SkillPatchOperations;
using SkillTree.SkillSpecial.SkillEmployee;
using System.Reflection;
using UnityEngine;
using static SkillTree.SkillActive.SkillActive;

namespace SkillTree.SkillEffect
{
    public static class SkillSystem
    {
        private static Player localPlayer;
        private static PlayerMovement playerMovement;
        private static Customer[] customerList;
        private static Business[] businessList;
        private static Dealer[] dealerList;
        private static Registry registry;

        public static void ApplySkill(string skillId, SkillTreeData data)
        {
            localPlayer = Player.Local;
            playerMovement = PlayerMovement.Instance;
            registry = Registry.Instance;
            customerList = UnityEngine.Object.FindObjectsOfType<Customer>();
            dealerList = UnityEngine.Object.FindObjectsOfType<Dealer>();
            businessList = UnityEngine.Object.FindObjectsOfType<Business>();
            Il2CppSystem.Collections.Generic.List<ItemDefinition> allItems = registry.GetAllItems();

            switch (skillId)
            {
                // Stats
                case "Stats":
                    MelonLogger.Msg("Player Health Before: " + localPlayer.Health.CurrentHealth);
                    localPlayer.Health.SetHealth(SkillModifiers.GetPlayerMaxHealth());
                    localPlayer.Health.RecoverHealth(SkillModifiers.GetPlayerMaxHealth());
                    MelonLogger.Msg("Player Health Now: " + localPlayer.Health.CurrentHealth);
                    break;
                case "MoreMovespeed":
                    MelonLogger.Msg("MoveSpeed Before: " + playerMovement.MoveSpeedMultiplier);
                    playerMovement.MoveSpeedMultiplier = SkillModifiers.GetPlayerMoveSpeed();
                    MelonLogger.Msg("MoveSpeed Now: " + playerMovement.MoveSpeedMultiplier);
                    break;
                case "MoreStackItem":
                    {
                        if (Core.SkillData.MoreStackItem == 0)
                            break;

                        if (registry == null)
                            break;

                        StackCache.FillCache(allItems);
                        foreach (ItemDefinition item in allItems)
                        {
                            string key = item.name;

                            if (StackCache.ItemStack.TryGetValue(key, out int baseMin))
                            {
                                item.StackLimit = baseMin * SkillModifiers.InventoryStackSizeMultiplier;
                                MelonLogger.Msg($"[MoreStackItem] {key}: {baseMin} -> {item.StackLimit}");
                            }
                        }
                        MelonLogger.Msg($"Skill Item Stack x2 Active");
                    }
                    break;
                case "MoreXP":
                    //SkillPatchStats.PlayerXPConfig.XpBase = 100f + (Core.SkillData.MoreXP * 5f);
                    //MelonLogger.Msg($"XP Base updated for: {SkillPatchStats.PlayerXPConfig.XpBase}%");
                    break;
                case "MoreXP2":
                    //SkillPatchStats.PlayerXPConfig.XpBase = 100f + ((Core.SkillData.MoreXP + Core.SkillData.MoreXP2) * 5f);
                    //MelonLogger.Msg($"XP Base updated for: {SkillPatchStats.PlayerXPConfig.XpBase}%");
                    break;
                case "BetterDelivery":
                    //SkillPatchStats.BetterDelivery.Add = (data.BetterDelivery == 1);
                    break;
                case "AllowSleepAthEne":
                    //SkillPatchStats.AllowSleepAthEne.Add = (data.AllowSleepAthEne == 1);
                    break;
                case "AllowSeeCounteroffChance":
                    //SkillPatchStats.CounterofferHelper.Counteroffer = (Core.SkillData.AllowSeeCounteroffChance == 1);
                    break;
                case "SkipSchedule":
                    //SkillPatchStats.SkipSchedule.Add = (Core.SkillData.SkipSchedule == 1);
                    break;
                case "MoreXPWhenEarnMoney":
                    //SkillPatchStats.PlayerXpMoney.XpMoney = (Core.SkillData.MoreXPWhenEarnMoney == 1);
                    break;

                // OPERATIONS
                case "Operations":
                    //SkillPatchOperations.BetterGrowTent.Add = (data.Operations * 0.16f);
                    break;
                case "GrowthSpeed":
                    //SkillPatchOperations.GrowthSpeedUp.Add = (data.GrowthSpeed * 0.025f);
                    break;
                case "GrowthSpeed2":
                    //SkillPatchOperations.GrowthSpeedUp.Add = ((data.GrowthSpeed + data.GrowthSpeed2) * 0.025f);
                    break;
                case "MoreYield":
                    //SkillPatchOperations.YieldAdd.Add = (data.MoreYield);
                    break;
                case "MoreQuality":
                    //SkillPatchOperations.QualityUP.Add = (data.MoreQuality * 0.15f);
                    //SkillPatchOperations.QualityMushroomUP.Add = (data.MoreQuality == 2 ? 0.3f : 0f);
                    break;
                case "MoreQualityMethCoca":
                    //SkillPatchOperations.MethQualityAdd.Add = (data.MoreQualityMethCoca == 1);
                    break;
                case "AbsorbentSoil":
                    //SkillPatchOperations.AbsorbentSoil.Add = (data.AbsorbentSoil == 1);
                    break;
                case "MoreMixAndDryingRackOutput":
                    if (Core.SkillData.MoreMixAndDryingRackOutput == 0)
                        break;

                    DryingRack[] racks = GameObject.FindObjectsOfType<DryingRack>();
                    foreach (DryingRack rack in racks)
                    {
                        DryingRack_Patch.ApplyCapacityUpdate(rack);
                    }
                    MelonLogger.Msg($"[DryingRack] Capacity updated for {racks.Length} active racks.");
                    //SkillPatchOperations.StackItem2xFix.Add = (data.MoreMixAndDryingRackOutput == 1);
                    //SkillPatchOperations.MixOutputAdd.Add = (data.MoreMixAndDryingRackOutput * 2) == 0 ? 1 : (data.MoreMixAndDryingRackOutput * 2);
                    break;
                case "ChemistStationQuick":
                    //SkillPatchOperations.StationTimeLess.TimeAjust = (data.ChemistStationQuick * 1.5f) == 0 ? 1 : (data.ChemistStationQuick * 2);
                    //SkillPatchOperations.MixOutputAdd.TimeAjust = (data.ChemistStationQuick * 2) == 0 ? 1 : (data.ChemistStationQuick * 2);
                    break;
                case "MoreCauldronOutput":
                    //{
                    //    int valueBase = SkillPatchOperations.CauldronOutputAdd.Add;
                    //    int bonus = Mathf.FloorToInt(valueBase * 1f * data.MoreCauldronOutput);
                    //    SkillPatchOperations.CauldronOutputAdd.Add = valueBase + bonus;
                    //}
                    break;

                // SOCIAL
                case "Social":
                    //SkillPatchSocial.CustomerSample.AddSampleChance = (data.Social * 0.05f);
                    break;
                case "CityEvolving":
                    if (Core.SkillData.CityEvolving == 0)
                        break;
                        
                    SkillPatchSocial.CustomerCache.FillCache(customerList.ToList());
                    foreach (Customer customer in customerList)
                    {
                        string key = customer.CustomerData.name;

                        if (SkillPatchSocial.CustomerCache.OriginalMinSpend.TryGetValue(key, out float baseMin) &&
                            SkillPatchSocial.CustomerCache.OriginalMaxSpend.TryGetValue(key, out float baseMax))
                        {
                            customer.CustomerData.MinWeeklySpend = baseMin * SkillModifiers.GetCustomerCashMultiplier();
                            customer.CustomerData.MaxWeeklySpend = baseMax * SkillModifiers.GetCustomerCashMultiplier();

                            MelonLogger.Msg($"[CityEvolving] {key}'s spending range increased from {baseMin}-{baseMax} to {customer.CustomerData.MinWeeklySpend}-{customer.CustomerData.MaxWeeklySpend}");
                        }
                    }
                    MelonLogger.Msg($"Weekly spend increased by {(SkillModifiers.GetCustomerCashMultiplier() % 1) * 100}%");
                    break;
                case "BusinessEvolving":
                    if (Core.SkillData.BusinessEvolving == 0)
                        break;

                    SkillPatchSocial.BusinessCache.FillCache(businessList.ToList());
                    foreach (Business business in businessList)
                    {
                        string key = business.PropertyName;

                        if (SkillPatchSocial.BusinessCache.LaunderCapacity.TryGetValue(key, out float baseMin))
                        {
                            business.LaunderCapacity = baseMin * SkillModifiers.GetLaunderingCapacityMultiplier();
                            MelonLogger.Msg($"[BusinessEvolving] {key}: {baseMin} -> {business.LaunderCapacity}");
                        }
                    }
                    MelonLogger.Msg($"[BusinessEvolving] LaunderCapacity increased by {(SkillModifiers.GetLaunderingCapacityMultiplier() % 1) * 100}%");
                    break;
                case "MoreATMLimit":
                    //SkillPatchSocial.ATMConfig.MaxWeeklyLimit += (data.MoreATMLimit * 2000);
                    //MelonLogger.Msg($"ATM Deposit Weekly Limit: ${SkillPatchSocial.ATMConfig.MaxWeeklyLimit}");
                    break;
                case "DealerCutLess":
                    foreach (Dealer dealer in dealerList)
                    {
                        if (!ValidDealer(dealer))
                            continue;
                        float originalCut = dealer.Cut;
                        dealer.Cut = SkillModifiers.GetDealerCut();
                        MelonLogger.Msg($"Dealer: {dealer.name} decreased cut from {originalCut * 100}% to {dealer.Cut * 100}%");
                    }
                    break;
                case "DealerSpeedUp":
                    foreach (Dealer dealer in dealerList)
                    {
                        if (!ValidDealer(dealer))
                            continue;
                        float originalMoveSpeed = dealer.Movement.MoveSpeedMultiplier;
                        dealer.Movement.MoveSpeedMultiplier += SkillModifiers.GetDealerSpeedBonus();
                        MelonLogger.Msg($"Dealer: {dealer.name} movespeed increased from {originalMoveSpeed * 100}% to {dealer.Movement.MoveSpeedMultiplier * 100}%");
                    }
                    break;
                case "DealerMoreCustomer":
                    //SkillPatchSocial.DealerUpCustomer.MaxCustomer += (data.DealerMoreCustomer * 2);
                    //MelonLogger.Msg($"Dealer MaxCustomer: {SkillPatchSocial.DealerUpCustomer.MaxCustomer}");
                    break;
                case "BetterSupplier":
                    //SkillPatchSocial.SupplierUp.SupplierCashLimitMultiplier = 1f + (data.BetterSupplier * 0.675f);
                    //SkillPatchSocial.SupplierUp.SupplierItemLimitMultiplier = 1f + (data.BetterSupplier * 0.5f);
                    MelonLogger.Msg($"Supplier cash limit multiplier is x{SkillModifiers.GetSupplierCashMultiplier()}");
                    MelonLogger.Msg($"Supplier item limit is {SkillModifiers.GetSupplierItemLimit()}");
                    break;

                //SPECIAL
                case "Special":
                    //SkillEnabled.enabledTrash = (data.Special == 1);
                    break;
                case "Heal":
                    //SkillEnabled.enabledHeal = (data.Heal == 1);
                    break;
                case "GetCashDealer":
                    //SkillEnabled.enabledGetCash = (data.GetCashDealer == 1);
                    break;
                case "BetterBotanists":
                    //BetterBotanist.Add = (data.BetterBotanists == 1);
                    break;
                case "Employees24h":
                    //CanWork.Add = (data.Employees24h == 1);
                    break;
                case "EmployeeMovespeed":
                    //EmployeeMovespeed.Add = (data.EmployeeMovespeed == 1);
                    //ValidEmployees();
                    break;
                case "EmployeeMaxStation":
                    //EmployeeMoreStation.Add = (data.EmployeeMaxStation * 2);
                    //ValidEmployees();
                    break;
            }
        }

        public static void ApplyAll(SkillTreeData data)
        {
            foreach (var field in typeof(SkillTreeData).GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                ApplySkill(field.Name, data);
            }
        }

        private static bool ValidDealer(Dealer dealer)
        {
            if (dealer.name.ToLower().Contains("carteldealer"))
                return false;
            return true;
        }
    }
}
