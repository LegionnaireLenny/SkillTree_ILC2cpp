using Il2CppScheduleOne;
using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.Employees;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.Management;
using Il2CppScheduleOne.Money;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.Property;
using Il2CppScheduleOne.Tools;
using Il2CppScheduleOne.UI;
using Il2CppScheduleOne.UI.ATM;
using MelonLoader;
using SkillTree.Json;
using SkillTree.SkillPatchSocial;
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

        private static Packager[] packagerList;
        private static Chemist[] chemistList;
        private static Botanist[] botanistList;
        private static Cleaner[] cleanerList;

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
                    { 
                        MelonLogger.Msg("Player Health Before: " + localPlayer.Health.CurrentHealth);
                        SkillPatchStats.PlayerHealthConfig.MaxHealth = 100 + (data.Stats * 20f);
                        localPlayer.Health.SetHealth(SkillPatchStats.PlayerHealthConfig.MaxHealth);
                        localPlayer.Health.RecoverHealth(SkillPatchStats.PlayerHealthConfig.MaxHealth);
                        MelonLogger.Msg("Player Health Now: " + localPlayer.Health.CurrentHealth);
                        break;
                    }
                case "MoreMovespeed":
                    {
                        MelonLogger.Msg("MoveSpeed Before: " + playerMovement.MoveSpeedMultiplier);
                        SkillPatchStats.PlayerMovespeed.MovespeedBase = 1f + (data.MoreMovespeed * 0.10f);
                        playerMovement.MoveSpeedMultiplier = SkillPatchStats.PlayerMovespeed.MovespeedBase;
                        MelonLogger.Msg("MoveSpeed Now: " + playerMovement.MoveSpeedMultiplier);
                        break;
                    }

                case "MoreStackItem":
                    {
                        QuickPackagers.Add = (data.MoreStackItem == 1);
                        if (registry == null) return;

                        if (!(data.MoreStackItem == 1))
                            return;

                        StackCache.FillCache(allItems);

                        int multiplier = 1 + (data.MoreStackItem);

                        if (multiplier > 1)
                        {
                            foreach (ItemDefinition item in allItems)
                            {
                                string key = item.name;

                                if (StackCache.ItemStack.TryGetValue(key, out int baseMin))
                                {
                                    int stackLimit = item.StackLimit;
                                    item.StackLimit = baseMin * multiplier;
                                    MelonLogger.Msg($"[MoreStackItem] {key}: {baseMin} -> {item.StackLimit}");
                                }
                            }
                            MelonLogger.Msg($"Skill Item Stack x2 Active");
                        }
                        break;
                    }
                case "MoreXP":
                    {
                        SkillPatchStats.PlayerXPConfig.XpBase = 100f + (data.MoreXP * 5f);
                        MelonLogger.Msg($"XP Base updated for: {SkillPatchStats.PlayerXPConfig.XpBase}%");
                        break;
                    }
                case "MoreXP2":
                    {
                        SkillPatchStats.PlayerXPConfig.XpBase = 100f + ((data.MoreXP + data.MoreXP2) * 5f);
                        MelonLogger.Msg($"XP Base updated for: {SkillPatchStats.PlayerXPConfig.XpBase}%");
                        break;
                    }
                case "BetterDelivery":
                    {
                        SkillPatchStats.BetterDelivery.Add = (data.BetterDelivery == 1);
                        break;
                    }
                case "AllowSleepAthEne":
                    {
                        SkillPatchStats.AllowSleepAthEne.Add = (data.AllowSleepAthEne == 1);
                        break;
                    }
                case "AllowSeeCounteroffChance":
                    {
                        SkillPatchStats.CounterofferHelper.Counteroffer = (data.AllowSeeCounteroffChance == 1);
                        break;
                    }
                case "SkipSchedule":
                    {
                        SkillPatchStats.SkipSchedule.Add = (data.SkipSchedule == 1);
                        break;
                    }
                case "MoreXPWhenEarnMoney":
                    {
                        SkillPatchStats.PlayerXpMoney.XpMoney = (data.MoreXPWhenEarnMoney == 1);
                        MelonLogger.Msg($"More XP When Earn Money {SkillPatchStats.PlayerXpMoney.XpMoney}");
                        break;
                    }
                // OPERATIONS
                case "Operations":
                    SkillPatchOperations.BetterGrowTent.Add = (data.Operations * 0.16f);
                    break;

                case "GrowthSpeed":
                    SkillPatchOperations.GrowthSpeedUp.Add = (data.GrowthSpeed * 0.025f);
                    break;

                case "GrowthSpeed2":
                    SkillPatchOperations.GrowthSpeedUp.Add = ((data.GrowthSpeed + data.GrowthSpeed2) * 0.025f);
                    break;

                case "MoreYield":
                    SkillPatchOperations.YieldAdd.Add = (data.MoreYield);
                    break;

                case "MoreQuality":
                    SkillPatchOperations.QualityUP.Add = (data.MoreQuality * 0.15f);
                    SkillPatchOperations.QualityMushroomUP.Add = (data.MoreQuality == 2 ? 0.3f : 0f);
                    break;

                case "MoreQualityMethCoca":
                    SkillPatchOperations.MethQualityAdd.Add = (data.MoreQualityMethCoca == 1);
                    break;

                case "AbsorbentSoil":
                    SkillPatchOperations.AbsorbentSoil.Add = (data.AbsorbentSoil == 1);
                    break;

                case "MoreMixAndDryingRackOutput":
                    SkillPatchOperations.StackItem2xFix.Add = (data.MoreMixAndDryingRackOutput == 1);
                    SkillPatchOperations.MixOutputAdd.Add = (data.MoreMixAndDryingRackOutput * 2) == 0 ? 1 : (data.MoreMixAndDryingRackOutput * 2);
                    break;

                case "ChemistStationQuick":
                    SkillPatchOperations.StationTimeLess.TimeAjust = (data.ChemistStationQuick * 1.5f) == 0 ? 1 : (data.ChemistStationQuick * 2);
                    SkillPatchOperations.MixOutputAdd.TimeAjust = (data.ChemistStationQuick * 2) == 0 ? 1 : (data.ChemistStationQuick * 2);
                    break;

                case "MoreCauldronOutput":
                    {
                        int valueBase = SkillPatchOperations.CauldronOutputAdd.Add;
                        int bonus = Mathf.FloorToInt(valueBase * 1f * data.MoreCauldronOutput);
                        SkillPatchOperations.CauldronOutputAdd.Add = valueBase + bonus;
                    }
                    break;

                // SOCIAL
                case "Social":
                    SkillPatchSocial.CustomerSample.AddSampleChance = (data.Social * 0.05f);
                    break;

                case "CityEvolving":
                    {
                        CustomerCache.FillCache(customerList.ToList());
                        float multiplier = 1.0f + (data.CityEvolving * 0.10f);

                        if (multiplier > 1.0f)
                        {
                            foreach (Customer customer in customerList)
                            {
                                string key = customer.CustomerData.name;

                                if (CustomerCache.OriginalMinSpend.TryGetValue(key, out float baseMin) &&
                                    CustomerCache.OriginalMaxSpend.TryGetValue(key, out float baseMax))
                                {
                                    customer.CustomerData.MinWeeklySpend = baseMin * multiplier;
                                    customer.CustomerData.MaxWeeklySpend = baseMax * multiplier;

                                    //MelonLogger.Msg($"[CityEvolving] {key}: {baseMin} -> {customer.CustomerData.MinWeeklySpend}");
                                }
                            }
                            MelonLogger.Msg($"Weekly spend incresed by {1.0f + (data.CityEvolving * 0.15f)}%");
                        }
                    }
                    break;

                case "BusinessEvolving":
                    {
                        BusinessCache.FillCache(businessList.ToList());
                        float multiplier = 1.0f + (data.BusinessEvolving * 0.20f);

                        if (multiplier > 1.0f)
                        {
                            foreach (Business business in businessList)
                            {
                                string key = business.PropertyName;

                                if (BusinessCache.LaunderCapacity.TryGetValue(key, out float baseMin))
                                {
                                    float oldCapacity = business.LaunderCapacity;
                                    business.LaunderCapacity = baseMin * multiplier;
                                    MelonLogger.Msg($"[BusinessEvolving] {key}: {baseMin} -> {business.LaunderCapacity}");
                                }
                            }
                            MelonLogger.Msg($"LaunderCapacity incresed by {1.0f + (data.BusinessEvolving * 0.20f)}%");
                        }
                    }
                    break;
                case "MoreATMLimit":
                    {
                        SkillPatchSocial.ATMConfig.MaxWeeklyLimit = 10000f + (data.MoreATMLimit * 2000);
                        MelonLogger.Msg($"ATM Deposit Weekly Limit: ${SkillPatchSocial.ATMConfig.MaxWeeklyLimit}");
                        break;
                    }
                case "DealerCutLess":
                    {
                        foreach (Dealer dealer in dealerList)
                        {
                            if (!ValidDealer(dealer))
                                continue;
                            float origin = dealer.Cut;
                            dealer.Cut = 0.2f - (data.DealerCutLess * 0.05f);
                            MelonLogger.Msg($"Dealer: {dealer.name} decrease cut from {origin}% to {dealer.Cut}");
                        }
                        break;
                    }
                case "DealerSpeedUp":
                    {
                        foreach (Dealer dealer in dealerList)
                        {
                            if (!ValidDealer(dealer))
                                continue;
                            float origin = dealer.Movement.MoveSpeedMultiplier;
                            dealer.Movement.MoveSpeedMultiplier = 1f + (data.DealerSpeedUp);
                            MelonLogger.Msg($"Dealer: {dealer.name} movespeed increase from {origin}% to {dealer.Movement.MoveSpeedMultiplier}");
                        }
                        break;
                    }
                case "DealerMoreCustomer":
                    {
                        SkillPatchSocial.DealerUpCustomer.MaxCustomer += (data.DealerMoreCustomer * 2);
                        MelonLogger.Msg($"Dealer MaxCustomer: {SkillPatchSocial.DealerUpCustomer.MaxCustomer}");
                        break;
                    }
                case "BetterSupplier":
                    {
                        SkillPatchSocial.SupplierUp.SupplierInc = 1f + (1f * (data.BetterSupplier * 0.675f));
                        SkillPatchSocial.SupplierUp.SupplierLimit = (int)(10 + (10 * (data.BetterSupplier * 0.5f)));
                        break;
                    }

                //SPECIAL
                case "Special":
                    {
                        SkillEnabled.enabledTrash = (data.Special == 1);
                        break;
                    }
                case "Heal":
                    {
                        SkillEnabled.enabledHeal = (data.Heal == 1);
                        break;
                    }
                case "GetCashDealer":
                    {
                        SkillEnabled.enabledGetCash = (data.GetCashDealer == 1);
                        break;
                    }
                case "BetterBotanists":
                    {
                        BetterBotanist.Add = (data.BetterBotanists == 1);
                        break;
                    }
                case "Employees24h":
                    {
                        CanWork.Add = (data.Employees24h == 1);
                        break;
                    }
                case "EmployeeMovespeed":
                    {
                        EmployeeMovespeed.Add = (data.EmployeeMovespeed == 1);
                        ValidEmployees();
                        break;
                    }
                case "EmployeeMaxStation":
                    {
                        EmployeeMoreStation.Add = (data.EmployeeMaxStation * 2);
                        ValidEmployees();
                        break;
                    }
            }
        }

        public static void ApplyAll(SkillTreeData data)
        {
            foreach (var field in typeof(SkillTreeData).GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                SkillSystem.ApplySkill(field.Name, data);
            }
        }

        private static bool ValidDealer(Dealer dealer)
        {
            if (dealer.name.ToLower().Contains("carteldealer"))
                return false;
            return true;
        }

        private static void ValidEmployees()
        {

            if (!BetterBotanist.Add) return;

            packagerList = UnityEngine.Object.FindObjectsOfType<Packager>();
            chemistList = UnityEngine.Object.FindObjectsOfType<Chemist>();
            botanistList = UnityEngine.Object.FindObjectsOfType<Botanist>();
            cleanerList = UnityEngine.Object.FindObjectsOfType<Cleaner>();

            foreach (Packager packager in packagerList)
            {
                if (EmployeeMovespeed.Add)
                    packager.Movement.MovementSpeedScale = 0.33f;
            }

            foreach (Chemist chemist in chemistList)
            {
                if (EmployeeMovespeed.Add)
                    chemist.Movement.MovementSpeedScale = 0.33f;

                if (EmployeeMoreStation.Add > 0)
                {
                    chemist.configuration.Stations.MaxItems = 4 + EmployeeMoreStation.Add;
                }
            }

            foreach (Botanist botanist in botanistList)
            {
                if (EmployeeMovespeed.Add)
                    botanist.Movement.MovementSpeedScale = 0.33f;

                if (EmployeeMoreStation.Add > 0)
                {
                    botanist.configuration.Assigns.MaxItems = 8 + (EmployeeMoreStation.Add);
                }
            }
            foreach (Cleaner cleaner in cleanerList)
            {
                if (!EmployeeMovespeed.Add) continue;
                cleaner.Movement.MovementSpeedScale = 0.33f;
            }
        }
    }
}
