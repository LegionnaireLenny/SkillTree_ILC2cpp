using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.Money;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.PlayerScripts.Health;
using MelonLoader;
using MelonLoader.Preferences;
using System;
using System.Linq;
using UnityEngine;

namespace SkillTree.Core.Utilities
{
    public class ConfigManager
    {
        public class ConfigEntry<T>
        {
            public MelonPreferences_Entry<T> Entry { get; private set; }

            public ConfigEntry(MelonPreferences_Category category,
                               string identifier,
                               T defaultValue,
                               string displayName = null,
                               string description = null,
                               ValueValidator validator = null)
            {
                Entry = category.CreateEntry(identifier, defaultValue, displayName, description, validator: validator);
            }

            public T GetValue(bool useDefault = false)
            {
                return useDefault ? Entry.DefaultValue : (T)Entry.BoxedValue;
            }

            public void SetValue(T value)
            {
                Entry.BoxedValue = value;
            }

            public void SetDefault()
            {
                Entry.BoxedValue = Entry.DefaultValue;
            }
        }

        private static void ResetConfig(bool reset)
        {
            if (!reset) return;

            ConfigManager obj = new();
            foreach (var property in typeof(ConfigManager).GetProperties())
            {
                try
                {
                    (property.GetValue(obj) as ConfigEntry<float>)?.SetDefault();
                    (property.GetValue(obj) as ConfigEntry<int>)?.SetDefault();
                    (property.GetValue(obj) as ConfigEntry<bool>)?.SetDefault();
                    (property.GetValue(obj) as ConfigEntry<KeyCode>)?.SetDefault();
                    (property.GetValue(obj) as ConfigEntry<Color>)?.SetDefault();
                }
                catch (Exception) { }
            }
        }

        private static MelonPreferences_Category Enforcer { get; set; }
        public static ConfigEntry<float> BaseHealth { get; set; }
        public static ConfigEntry<float> HealthBonus { get; set; }
        public static ConfigEntry<float> BaseHealthRegen { get; set; }
        public static ConfigEntry<float> BaseStamina { get; set; }
        public static ConfigEntry<float> BaseMoveSpeed { get; set; }
        public static ConfigEntry<float> BaseJumpHeight { get; set; }
        public static ConfigEntry<float> BaseArrestTime { get; set; }
        public static ConfigEntry<float> BaseArrestRadius { get; set; }
        public static ConfigEntry<float> HealthRegenBonus { get; set; }
        public static ConfigEntry<float> BaseHealthRegenDelay { get; set; }
        public static ConfigEntry<float> HealthRegenDelayMultiplier { get; set; }
        public static ConfigEntry<float> StaminaBonus { get; set; }
        public static ConfigEntry<float> MoveSpeedBonus { get; set; }
        public static ConfigEntry<float> JumpHeightBonus { get; set; }
        public static ConfigEntry<float> VisibilityMultiplier { get; set; }
        public static ConfigEntry<float> PickpocketDifficultyMultiplier { get; set; }
        public static ConfigEntry<float> PickpocketMinimumSuccessWidth { get; set; }
        public static ConfigEntry<float> TimeSkipGrowthMultiplier { get; set; }
        public static ConfigEntry<float> XPGainBonus { get; set; }
        public static ConfigEntry<float> XPGainBonus2 { get; set; }
        public static ConfigEntry<float> SaleXPBonus { get; set; }
        public static ConfigEntry<int>   InventoryStackSizeBonus { get; set; }
        public static ConfigEntry<float> AimTimeMultiplier { get; set; }
        public static ConfigEntry<float> MaxSpreadMultiplier { get; set; }
        public static ConfigEntry<float> MinSpreadMultiplier { get; set; }
        public static ConfigEntry<int>   AmmoCapacityBonus { get; set; }
        public static ConfigEntry<float> ArrestTimeBonus { get; set; }
        public static ConfigEntry<float> ArrestRadiusBonus { get; set; }

        private static MelonPreferences_Category Provisioner { get; set; }
        public static ConfigEntry<int>   BaseDryingRackCapacity { get; set; }
        public static ConfigEntry<int>   BaseCauldronOutput { get; set; }
        public static ConfigEntry<int>   CauldronOutputBonus { get; set; }
        public static ConfigEntry<int>   MixDryOutputSizeBonus { get; set; }
        public static ConfigEntry<int>   ChemistStationSpeedBonus { get; set; }
        public static ConfigEntry<float> BasePlantQualityLevel { get; set; }
        public static ConfigEntry<float> QualityBonusGrowTent { get; set; }
        public static ConfigEntry<float> QualityBonusPlants { get; set; }
        public static ConfigEntry<float> QualityBonusShrooms { get; set; }
        public static ConfigEntry<int>   BaseYieldPlants { get; set; }
        public static ConfigEntry<int>   YieldBonusPlants { get; set; }
        public static ConfigEntry<float> GrowthSpeedBonusPlants { get; set; }
        public static ConfigEntry<float> MoistureDrainBonus { get; set; }

        private static MelonPreferences_Category Hustler { get; set; }
        public static ConfigEntry<float> BaseWeeklyDepositLimit { get; set; }
        public static ConfigEntry<int>   BaseTrashGrabberBinSize { get; set; }
        public static ConfigEntry<float> TrashGrabberBinSizeBonus { get; set; }
        public static ConfigEntry<float> TrashPickupRadius { get; set; }
        public static ConfigEntry<float> TrashPickupRadiusBonus { get; set; }
        public static ConfigEntry<int>   TrashValueBonus { get; set; }
        public static ConfigEntry<float> PawnPriceBonus { get; set; }
        public static ConfigEntry<float> ATMDepositBonus { get; set; }
        public static ConfigEntry<float> CustomerSampleAcceptBonus { get; set; }
        public static ConfigEntry<float> CustomerCashBonus { get; set; }
        public static ConfigEntry<int>   CustomerOrderLimitBonus { get; set; }
        public static ConfigEntry<int>   CustomerOrderLimitRankBonus { get; set; }
        public static ConfigEntry<float> LaunderingBonus { get; set; }

        private static MelonPreferences_Category Logistician { get; set; }
        public static ConfigEntry<int>   BaseMaxCustomer { get; set; }
        public static ConfigEntry<int>   BaseDeadDropItemLimit { get; set; }
        public static ConfigEntry<int>   BaseMaxChemistStations { get; set; }
        public static ConfigEntry<int>   BaseMaxBotanistStations { get; set; }
        public static ConfigEntry<float> BotanistActionDurationMultiplier { get; set; }
        public static ConfigEntry<float> HandlerPackagingSpeedMultiplier { get; set; }
        public static ConfigEntry<float> ChemistActionDurationMultiplier { get; set; }
        public static ConfigEntry<float> EmployeeMoveSpeedBonus { get; set; }
        public static ConfigEntry<int>   EmployeeStationBonus { get; set; }
        public static ConfigEntry<int>   DealerCustomerLimitBonus { get; set; }
        public static ConfigEntry<float> DealerCutReduction { get; set; }
        public static ConfigEntry<float> DealerSpeedBonus { get; set; }
        public static ConfigEntry<float> SupplierCashBonus { get; set; }
        public static ConfigEntry<float> SupplierItemBonus { get; set; }

        private static MelonPreferences_Category Special { get; set; }
        public static ConfigEntry<float> PoliceKilledBonus { get; set; }
        public static ConfigEntry<float> CartelKilledBonus { get; set; }
        public static ConfigEntry<float> BloodRushRegenDelayMultiplier { get; set; }
        public static ConfigEntry<float> BloodRushHealthBonusMultiplier { get; set; }
        public static ConfigEntry<float> BloodRushHealthBonusCap { get; set; }
        public static ConfigEntry<float> BloodRushDuration { get; set; }
        public static ConfigEntry<float> BloodRushFOVChange { get; set; }
        public static ConfigEntry<float> BloodRushHeartbeatVolume { get; set; }
        public static ConfigEntry<float> BloodRushHeartbeatPitch { get; set; }
        public static ConfigEntry<Color> BloodRushScreenTint { get; set; }
        public static ConfigEntry<float> SiphonFundsBaseConversionRate { get; set; }
        public static ConfigEntry<float> SiphonFundsOwnedBusinessBonus { get; set; }
        public static ConfigEntry<float> TrickleDownCashReserve { get; set; }
        public static ConfigEntry<int>   TrickleDownPayoutInterval { get; set; }
        public static ConfigEntry<float> BloodMoneyDuration { get; set; }
        public static ConfigEntry<float> BloodMoneyFOVChange { get; set; }
        public static ConfigEntry<float> BloodMoneyHeartbeatVolume { get; set; }
        public static ConfigEntry<float> BloodMoneyHeartbeatPitch { get; set; }
        public static ConfigEntry<Color> BloodMoneyScreenTint { get; set; }
        public static ConfigEntry<float> InfectiousPersonalityRange { get; set; }
        public static ConfigEntry<int>   AdrenalineSurgeMaxCharges { get; set; }
        public static ConfigEntry<float> AdrenalineSurgeDuration { get; set; }
        public static ConfigEntry<float> AdrenalineSurgeSpeedMultiplier { get; set; }
        public static ConfigEntry<float> AdrenalineSurgeJumpMultiplier { get; set; }
        public static ConfigEntry<bool>  AdrenalineSurgeZappedEffect { get; set; }
        public static ConfigEntry<float> AntiGravityBongDuration { get; set; }
        public static ConfigEntry<float> AntiGravityBongRadius { get; set; }
        public static ConfigEntry<float> AntiGravityBongCooldown { get; set; }

        private static MelonPreferences_Category UserSettings { get; set; }
        public static ConfigEntry<bool> UseDefaultSkillParameters { get; set; }
        public static ConfigEntry<bool> AutoUnlockPrerequisites { get; set; }
        public static ConfigEntry<bool> EnableContractColors { get; set; }
        public static ConfigEntry<bool> EnableCrosshair { get; set; }
        public static ConfigEntry<Color> ContractReadyBackgroundColor { get; set; }
        public static ConfigEntry<Color> ContractReadyFillColor { get; set; }
        public static ConfigEntry<Color> ContractNotReadyBackgroundColor { get; set; }
        public static ConfigEntry<Color> ContractNotReadyFillColor { get; set; }

        private static MelonPreferences_Category Keybinds { get; set; }
        public static ConfigEntry<KeyCode> MenuHotkey { get; set; }
        public static ConfigEntry<KeyCode> LevelSkillHotkey { get; set; }
        public static ConfigEntry<KeyCode> GoodSamaritanHotkey { get; set; }
        public static ConfigEntry<KeyCode> BloodRushHotkey { get; set; }
        public static ConfigEntry<KeyCode> SiphonFundsHotkey { get; set; }
        public static ConfigEntry<KeyCode> TrickledownHotkey { get; set; }
        public static ConfigEntry<KeyCode> BloodMoneyHotkey { get; set; }
        public static ConfigEntry<KeyCode> InfectiousPersonalityHotkey { get; set; }
        public static ConfigEntry<KeyCode> AdrenalineSurgeHotkey { get; set; }
        public static ConfigEntry<KeyCode> AntiGravityBongHotkey { get; set; }

        private static MelonPreferences_Category DebugOptions { get; set; }
        public static ConfigEntry<bool> ResetSkills { get; set; }
        public static ConfigEntry<bool> ResetConfiguration { get; set; }

        public static void Initialize()
        {
            Enforcer = MelonPreferences.CreateCategory("SkillTree_EnforcerSettings", "Enforcer Skills Settings");
            BaseHealth = new ConfigEntry<float>(Enforcer, "SkillTree_BaseMaxHealth", PlayerHealth.MAX_HEALTH, "Base Maximum Health");
            BaseHealthRegen = new ConfigEntry<float>(Enforcer, "SkillTree_BaseHealthRegen", 0.5f, "Base Health Restored Per Second");
            BaseHealthRegenDelay = new ConfigEntry<float>(Enforcer, "SkillTree_BaseHealthRegenDelay", 30f, "Base Time Until Health Regenerates");
            BaseStamina = new ConfigEntry<float>(Enforcer, "SkillTree_BaseStamina", PlayerMovement.StaminaReserveMax, "Base Maximum Stamina");
            BaseMoveSpeed = new ConfigEntry<float>(Enforcer, "SkillTree_BaseMoveSpeed", 1f, "Base Movement Speed");
            BaseJumpHeight = new ConfigEntry<float>(Enforcer, "SkillTree_BaseJumpHeight", PlayerMovement.JumpMultiplier, "Base Jump Height");
            BaseArrestTime = new ConfigEntry<float>(Enforcer, "SkillTree_BaseArrestTime", 1.75f, "Base Arrest Time");
            BaseArrestRadius = new ConfigEntry<float>(Enforcer, "SkillTree_BaseArrestRadius", 2.75f, "Base Arrest Radius");
            HealthBonus = new ConfigEntry<float>(Enforcer, "SkillTree_HealthBonus", 20f, "Hardy: Health Bonus");
            HealthRegenBonus = new ConfigEntry<float>(Enforcer, "SkillTree_HealthRegenBonus", 1f, "Battle-Scarred: Health Regen Bonus", "Increases the health regeneration amount multiplier");
            HealthRegenDelayMultiplier = new ConfigEntry<float>(Enforcer, "SkillTree_HealthRegenDelayMultiplier", 0.5f, "Battle-Scarred: Health Regen Delay Multiplier");
            StaminaBonus = new ConfigEntry<float>(Enforcer, "SkillTree_StaminaBonus", 0.30f, "Spring-Heeled: Stamina Bonus");
            JumpHeightBonus = new ConfigEntry<float>(Enforcer, "SkillTree_JumpHeightBonus", 0.35f, "Spring-Heeled: Jump Height Bonus");
            MoveSpeedBonus = new ConfigEntry<float>(Enforcer, "SkillTree_MoveSpeedBonus", 0.15f, "Fleet Feet: Movement Speed Bonus");
            VisibilityMultiplier = new ConfigEntry<float>(Enforcer, "SkillTree_VisibilityMultiplier", 0.75f, "Ghost: Visibility Multiplier");
            PickpocketDifficultyMultiplier = new ConfigEntry<float>(Enforcer, "SkillTree_PickpocketDifficultyMultiplier", 0.75f, "Ghost: Pickpocket Difficulty Multiplier");
            PickpocketMinimumSuccessWidth = new ConfigEntry<float>(Enforcer, "SkillTree_PickpocketMinimumSuccessWidth", 20f, "Ghost: Minimum Width of Pickpocket Green Area");
            TimeSkipGrowthMultiplier = new ConfigEntry<float>(Enforcer, "SkillTree_TimeSkipGrowthMultiplier", 0.33f, "Circadian Mastery: Time Skip Plant Growth Progress Multiplier", "By default, plants grow at one-third (0.33) of their normal speed when time is skipped.", new ValueRange<float>(0.1f, 2f));
            XPGainBonus = new ConfigEntry<float>(Enforcer, "SkillTree_XPGainBonus", 0.05f, "Fast Learner: XP Bonus");
            XPGainBonus2 = new ConfigEntry<float>(Enforcer, "SkillTree_XPGainBonus2", 0.1f, "Turbo Nerdo: XP Bonus");
            SaleXPBonus = new ConfigEntry<float>(Enforcer, "SkillTree_SalesXPBonus", 0.05f, "Kingpin: Sales XP Bonus");
            InventoryStackSizeBonus = new ConfigEntry<int>(Enforcer, "SkillTree_InventoryStackSizeBonus", 1, "Prison Wallet: Inventory Stack Size Bonus", validator: new ValueRange<int>(0, 1000));
            AimTimeMultiplier = new ConfigEntry<float>(Enforcer, "SkillTree_AimTimeMultiplier", 0.5f, "Quick Draw McGraw: Ranged Weapon Aim Time Multiplier");
            MaxSpreadMultiplier = new ConfigEntry<float>(Enforcer, "SkillTree_MaxSpreadMultiplier", 0.35f, "Sharpshooter: Maximum Ranged Weapon Spread Multiplier");
            MinSpreadMultiplier = new ConfigEntry<float>(Enforcer, "SkillTree_MinSpreadMultiplier", 0.35f, "Sharpshooter: Minimum Ranged Weapon Spread Multiplier");
            AmmoCapacityBonus = new ConfigEntry<int>(Enforcer, "SkillTree_AmmoCapacityBonus", 1, "Double-Stack Mags: Ammo Capacity Bonus", "Increases the ammo capacity multiplier");
            ArrestTimeBonus = new ConfigEntry<float>(Enforcer, "SkillTree_ArrestTimeBonus", 1f, "Slippery: Arrest Time Bonus", "Increases the arrest time multiplier");
            ArrestRadiusBonus = new ConfigEntry<float>(Enforcer, "SkillTree_ArrestRadiusBonus", 0.75f, "Slippery: Arrest Radius Multiplier");
            Enforcer.SetFilePath($"UserData/SkillTree_Config.cfg", true, false);

            Provisioner = MelonPreferences.CreateCategory("SkillTree_ProvisionerSettings", "Provisioner Skills Settings");
            BaseDryingRackCapacity = new ConfigEntry<int>(Provisioner, "SkillTree_BaseDryingRackCapacity", 20, "Base Drying Rack Capacity");
            BaseCauldronOutput = new ConfigEntry<int>(Provisioner, "SkillTree_BaseCauldronOutput", 10, "Base Cauldron Output");
            BaseYieldPlants = new ConfigEntry<int>(Provisioner, "SkillTree_BaseYieldPlants", 12, "Base Yield of Plants");
            BasePlantQualityLevel = new ConfigEntry<float>(Provisioner, "SkillTree_BasePlantQualityLevel", 0.5f, "Base Quality Level of Plants");
            CauldronOutputBonus = new ConfigEntry<int>(Provisioner, "SkillTree_CauldronOutputBonus", 1, "Witch's Brew: Cauldron Output Bonus", "Increases the output multiplier");
            MixDryOutputSizeBonus = new ConfigEntry<int>(Provisioner, "SkillTree_MixDryOutputSizeBonus", 1, "Crankin' One Out: Mixer/drying rack capacity bonus", "Increases the capacity multiplier");
            ChemistStationSpeedBonus = new ConfigEntry<int>(Provisioner, "SkillTree_ChemistStationSpeedBonus", 1, "Quick Crafter: Crafting Speed Bonus", "Increases the speed multiplier");
            QualityBonusGrowTent = new ConfigEntry<float>(Provisioner, "SkillTree_QualityBonusGrowTent", 0.26f, "Pitchin' a Tent: Quality Bonus for Grow Tents");
            QualityBonusPlants = new ConfigEntry<float>(Provisioner, "SkillTree_QualityBonusPlants", 0.15f, "Advanced Pot Techniques: Quality Bonus for Pots");
            QualityBonusShrooms = new ConfigEntry<float>(Provisioner, "SkillTree_QualityBonusShrooms", 0.15f, "Mushroomancer: Quality Bonus for Mushrooms");
            YieldBonusPlants = new ConfigEntry<int>(Provisioner, "SkillTree_YieldBonusPlants", 1, "Bountiful Harvest: Yield Bonus for Plants");
            GrowthSpeedBonusPlants = new ConfigEntry<float>(Provisioner, "SkillTree_GrowthSpeedBonusPlants", 0.025f, "Green Thumb/Plant Whisperer: Plant Growth Speed Bonus");
            MoistureDrainBonus = new ConfigEntry<float>(Provisioner, "SkillTree_MoistureDrainBonus", 0.5f, "Wet-Ass Plants: Moisture Drain Multiplier");
            Provisioner.SetFilePath($"UserData/SkillTree_Config.cfg", true, false);

            Hustler = MelonPreferences.CreateCategory("SkillTree_HustlerSettings", "Hustler Skills Settings");
            BaseWeeklyDepositLimit = new ConfigEntry<float>(Hustler, "SkillTree_BaseWeeklyDepositLimit", ATM.WEEKLY_DEPOSIT_LIMIT, "Base Weekly ATM Deposit Limit");
            BaseTrashGrabberBinSize = new ConfigEntry<int>(Hustler, "SkillTree_BaseTrashGrabberBinSize", 20, "Base Trash Grabber Bin Size");
            TrashGrabberBinSizeBonus = new ConfigEntry<float>(Hustler, "SkillTree_TrashGrabberBinSizeBonus", 1f, "Community Service: Trash Grabber Bin Size Bonus", "Increases the trash grabber's bin size multiplier");
            TrashPickupRadius = new ConfigEntry<float>(Hustler, "SkillTree_TrashPickupRadius", 0.45f, "Community Service: Trash Grabber Pickup Radius");
            TrashPickupRadiusBonus = new ConfigEntry<float>(Hustler, "SkillTree_TrashPickupRadiusBonus", 0f, "Community Service: Trash Grabber Pickup Radius Bonus", "Increases the trash grabber's pickup radius multiplier");
            TrashValueBonus = new ConfigEntry<int>(Hustler, "SkillTree_TrashValueBonus", 1, "Sacar La Basura: Trash Value Bonus");
            PawnPriceBonus = new ConfigEntry<float>(Hustler, "SkillTree_PawnPriceBonus", 0.25f, "Sacar La Basura: Pawn Price Bonus");
            ATMDepositBonus = new ConfigEntry<float>(Hustler, "SkillTree_ATMDepositBonus", 2500f, "Hoard the Wealth: ATM Deposit Limit Bonus");
            CustomerSampleAcceptBonus = new ConfigEntry<float>(Hustler, "SkillTree_CustomerSampleAcceptBonus", 0.05f, "Silver Tongued Devil: Sample Acceptance Chance Bonus");
            CustomerCashBonus = new ConfigEntry<float>(Hustler, "SkillTree_CustomerCashBonus", 0.25f, "Spread the Wealth: Customer Weekly Spend Limit Bonus");
            CustomerOrderLimitBonus = new ConfigEntry<int>(Hustler, "SkillTree_CustomerOrderLimitBonus", 3, "Captive Market: Customer Order Limit Bonus");
            CustomerOrderLimitRankBonus = new ConfigEntry<int>(Hustler, "SkillTree_CustomerOrderLimitRankBonus", 1, "Captive Market: Customer Order Limit Rank Bonus");
            LaunderingBonus = new ConfigEntry<float>(Hustler, "SkillTree_LaunderingBonus", 0.30f, "Squeaky Clean: Business Laundering Capacity Bonus");
            Hustler.SetFilePath($"UserData/SkillTree_Config.cfg", true, false);

            Logistician = MelonPreferences.CreateCategory("SkillTree_LogisticianSettings", "Logistician Skills Settings");
            BaseDeadDropItemLimit = new ConfigEntry<int>(Logistician, "SkillTree_BaseDeadDropItemLimit", Supplier.DEADDROP_ITEM_LIMIT, "Base Deaddrop Item Limit");
            BaseMaxCustomer = new ConfigEntry<int>(Logistician, "SkillTree_BaseMaxCustomer", Dealer.MAX_CUSTOMERS, "Base Max Customers for Dealers");
            BaseMaxChemistStations = new ConfigEntry<int>(Logistician, "SkillTree_BaseMaxChemistStations", 4, "Base Maximum Chemist Stations");
            BaseMaxBotanistStations = new ConfigEntry<int>(Logistician, "SkillTree_BaseMaxBotanistStations", 8, "Base Maximum Botanist Stations");
            SupplierCashBonus = new ConfigEntry<float>(Logistician, "SkillTree_SupplierCashBonus", 0.675f, "Reliable Bus. Partner: Dead Drop Order Limit Bonus");
            SupplierItemBonus = new ConfigEntry<float>(Logistician, "SkillTree_SupplierItemBonus", 0.50f, "Reliable Bus. Partner: Dead Drop Item Limit Bonus");
            DealerCustomerLimitBonus = new ConfigEntry<int>(Logistician, "SkillTree_CustomerLimitBonus", 2, "Expansive Empire: Customer Limit Bonus");
            DealerCutReduction = new ConfigEntry<float>(Logistician, "SkillTree_DealerCutReduction", 0.05f, "Wage Garnishment: Dealer Cut Reduction");
            DealerSpeedBonus = new ConfigEntry<float>(Logistician, "SkillTree_DealerSpeedBonus", 1f, "Motivational Leader: Dealer Speed Bonus", "Increases the speed multiplier");
            BotanistActionDurationMultiplier = new ConfigEntry<float>(Logistician, "SkillTree_BotanistActionDurationMultiplier", 0.5f, "Fast Farmers: Botanist Action Duration Multiplier", validator: new ValueRange<float>(0.1f, 10f));
            HandlerPackagingSpeedMultiplier = new ConfigEntry<float>(Logistician, "SkillTree_HandlerPackagingSpeedMultiplier", 2f, "Fast Handlers: Handler Packaging Speed Multiplier", validator: new ValueRange<float>(0.1f, 10f));
            ChemistActionDurationMultiplier = new ConfigEntry<float>(Logistician, "SkillTree_ChemistActionDurationMultiplier", 0.5f, "Fast Chemists: Chemist Action Duration Multiplier", validator: new ValueRange<float>(0.1f, 10f));
            EmployeeMoveSpeedBonus = new ConfigEntry<float>(Logistician, "SkillTree_EmployeeMoveSpeedBonus", 0.33f, "RUN BITCH RUN!: Employee MovespeedScale", "Lower is faster, higher is slower. Value is clamped between 0.1f (10x speed) and 10f (0.1x speed)");
            EmployeeStationBonus = new ConfigEntry<int>(Logistician, "SkillTree_EmployeeStationBonus", 2, "Overworked/Underpaid: Station bonus for Botanists and Chemists");
            Logistician.SetFilePath($"UserData/SkillTree_Config.cfg", true, false);

            Special = MelonPreferences.CreateCategory("SkillTree_SpecialSettings", "Special Skills Settings");
            PoliceKilledBonus = new ConfigEntry<float>(Special, "SkillTree_PoliceKilledBonus", 0.1f, "Blood Rush: Bonus Health per Police Killed");
            CartelKilledBonus = new ConfigEntry<float>(Special, "SkillTree_CartelKilledBonus", 0.1f, "Blood Rush: Bonus Health per Cartel Killed");
            BloodRushRegenDelayMultiplier = new ConfigEntry<float>(Special, "SkillTree_BloodRushRegenDelayMultiplier", 0.2f, "Blood Rush: Health Regen Delay Multiplier");
            BloodRushHealthBonusMultiplier = new ConfigEntry<float>(Special, "SkillTree_BloodRushHealthBonusMultiplier", 2f, "Blood Rush: Bonus Health Cap Multiplier");
            BloodRushHealthBonusCap = new ConfigEntry<float>(Special, "SkillTree_BloodRushHealthBonusCap", 30f, "Blood Rush: Bonus Health Cap");
            BloodRushDuration = new ConfigEntry<float>(Special, "SkillTree_BloodRushDuration", 60f, "Blood Rush: Effect Duration");
            BloodRushFOVChange = new ConfigEntry<float>(Special, "SkillTree_BloodRushFOVChange", 10f, "Blood Rush: FOV Change");
            BloodRushHeartbeatVolume = new ConfigEntry<float>(Special, "SkillTree_BloodRushHeartbeatVolume", 0.25f, "Blood Rush: Heartbeat Volume");
            BloodRushHeartbeatPitch = new ConfigEntry<float>(Special, "SkillTree_BloodRushHeartbeatPitch", 1f, "Blood Rush: Heartbeat Pitch");
            BloodRushScreenTint = new ConfigEntry<Color>(Special, "SkillTree_BloodRushScreenTint", new Color(1f, 0.9f, 0.9f, 0.2f), "Blood Rush: Screen Tint");
            SiphonFundsBaseConversionRate = new ConfigEntry<float>(Special, "SkillTree_SiphonFundsBaseConversionRate", 0.1f, "Siphon Funds: Base Online Balance Conversion Rate");
            SiphonFundsOwnedBusinessBonus = new ConfigEntry<float>(Special, "SkillTree_SiphonFundsOwnedBusinessBonus", 0.05f, "Siphon Funds: Bonus Per Owned Business");
            TrickleDownCashReserve = new ConfigEntry<float>(Special, "SkillTree_TrickledownCashReserve", 2000f, "Trickle-down Economics: Cash Reserve", "Amount of cash kept when transferring money to businesses if there's not enough to max out their capacity");
            TrickleDownPayoutInterval = new ConfigEntry<int>(Special, "SkillTree_TrickleDownPayoutInterval", 6, "Trickle-down Economics: Payout Interval", "Number of hours between laundering payouts", validator: new ValueRange<int>(1, 24));
            BloodMoneyDuration = new ConfigEntry<float>(Special, "SkillTree_BloodMoneyDuration", 30f, "Blood Money: Effect Duration");
            BloodMoneyFOVChange = new ConfigEntry<float>(Special, "SkillTree_BloodMoneyFOVChange", 10f, "Blood Money: FOV Change");
            BloodMoneyHeartbeatVolume = new ConfigEntry<float>(Special, "SkillTree_BloodMoneyHeartbeatVolume", 0.5f, "Blood Money: Heartbeat Volume");
            BloodMoneyHeartbeatPitch = new ConfigEntry<float>(Special, "SkillTree_BloodMoneyHeartbeatPitch", 0.85f, "Blood Money: Heartbeat Pitch");
            BloodMoneyScreenTint = new ConfigEntry<Color>(Special, "SkillTree_BloodMoneyScreenTint", new Color(1f, 1f, 0.9f, 0.2f), "Blood Money: Screen Tint");
            InfectiousPersonalityRange = new ConfigEntry<float>(Special, "SkillTree_InfectiousPersonalityRange", 15f, "Infectious Personality: Effect Range");
            AdrenalineSurgeMaxCharges = new ConfigEntry<int>(Special, "SkillTree_AdrenalineSurgeCharges", 3, "Adrenaline Surge: Max Number of Charges");
            AdrenalineSurgeDuration = new ConfigEntry<float>(Special, "SkillTree_AdrenalineSurgeDuration", 15f, "Adrenaline Surge: Effect Duration");
            AdrenalineSurgeSpeedMultiplier = new ConfigEntry<float>(Special, "SkillTree_AdrenalineSurgeSpeedMultiplier", 3f, "Adrenaline Surge: Speed Multiplier");
            AdrenalineSurgeJumpMultiplier = new ConfigEntry<float>(Special, "SkillTree_AdrenalineSurgeJumpMultiplier", 3f, "Adrenaline Surge: Jump Multiplier");
            AdrenalineSurgeZappedEffect = new ConfigEntry<bool>(Special, "SkillTree_AdrenalineSurgeZappedEffect", true, "Adrenaline Surge: Enabled Zapped Effect");
            AntiGravityBongDuration = new ConfigEntry<float>(Special, "SkillTree_AntiGravityBongDuration", 15f, "Anti-Gravity Bong: Effect Duration");
            AntiGravityBongRadius = new ConfigEntry<float>(Special, "SkillTree_AntiGravityBongRadius", 10f, "Anti-Gravity Bong: Effect Radius");
            AntiGravityBongCooldown = new ConfigEntry<float>(Special, "SkillTree_AntiGravityBongCooldown", 60f, "Anti-Gravity Bong: Skill Cooldown");
            Special.SetFilePath($"UserData/SkillTree_Config.cfg", true, false);

            UserSettings = MelonPreferences.CreateCategory("SkillTree_UserSettings", "User Settings");
            UseDefaultSkillParameters = new ConfigEntry<bool>(UserSettings, "SkillTree_UseDefaultSkillParameters", true, "Use Default Skill Parameters", "If enabled, skills will use their default parameters. Disable this if you want to customize skill parameters.");
            AutoUnlockPrerequisites = new ConfigEntry<bool>(UserSettings, "SkillTree_AutoUnlockPrerequisites", true, "Auto Unlock Prerequisite Skills", "If enabled, attempting to level a locked skill will automatically unlock all prerequisite skills and level the selected skill once");
            EnableContractColors = new ConfigEntry<bool>(UserSettings, "SkillTree_EnableContractColors", true, "Enable Contract Colors", "If enabled, contract icon colors can be customized and will change colors to indicate contracts within their delivery window");
            EnableCrosshair = new ConfigEntry<bool>(UserSettings, "SkillTree_EnableCrosshair", true, "Enable Crosshair", "If enabled, the crosshair stays enabled while wielding a ranged weapon");
            ContractReadyBackgroundColor = new ConfigEntry<Color>(UserSettings, "SkillTree_ContractReadyBackgroundColor", new(0.2984f, 0.6226f, 0.2673f, 1f), "Contract Ready: Background Color", "Icon background color for contracts that are within their delivery window");
            ContractReadyFillColor = new ConfigEntry<Color>(UserSettings, "SkillTree_ContractReadyFillColor", new(1f, 1f, 1f, 1f), "Contract Ready: Fill Color", "Icon fill color for contracts that are within their delivery window");
            ContractNotReadyBackgroundColor = new ConfigEntry<Color>(UserSettings, "SkillTree_ContractNotReady_BackgroundColor", new(0.6984f, 0.6226f, 0.4673f, 1f), "Contract Not Ready: Background Color", "Icon background color for contracts that are outside their delivery window");
            ContractNotReadyFillColor = new ConfigEntry<Color>(UserSettings, "SkillTree_ContractNotReady_FillColor", new(1f, 1f, 1f, 1f), "Contract Not Ready: Fill Color", "Icon fill color for contracts that are outside their delivery window");
            UserSettings.SetFilePath($"UserData/SkillTree_Config.cfg", true, false);

            Keybinds = MelonPreferences.CreateCategory("SkillTree_Keybinds", "Keybindings");
            MenuHotkey = new ConfigEntry<KeyCode>(Keybinds, $"SkillTree_Menu_Hotkey", KeyCode.BackQuote, "Menu Hotkey", "Open the skill tree menu");
            LevelSkillHotkey = new ConfigEntry<KeyCode>(Keybinds, $"SkillTree_LevelSkill_Hotkey", KeyCode.Space, "Level Skill Hotkey", "While the skill tree is open, levels the currently selected skill");
            GoodSamaritanHotkey = new ConfigEntry<KeyCode>(Keybinds, "SkillTree_GoodSamaritan_Hotkey", KeyCode.F1, "Skill: Good Samaritan", "Activate 'Good Samaritan' skill");
            BloodRushHotkey = new ConfigEntry<KeyCode>(Keybinds, "SkillTree_BloodRush_Hotkey", KeyCode.F2, "Skill: Blood Rush", "Activate 'Blood Rush' skill");
            SiphonFundsHotkey = new ConfigEntry<KeyCode>(Keybinds, "SkillTree_SiphonFundsHotkey", KeyCode.F3, "Skill: Siphon Funds", "Activate 'Siphon Funds' skill");
            TrickledownHotkey = new ConfigEntry<KeyCode>(Keybinds, "SkillTree_Trickledown_Hotkey", KeyCode.F4, "Skill: Trickle-down Economics", "Activate 'Trickle-down Economics' skill");
            BloodMoneyHotkey = new ConfigEntry<KeyCode>(Keybinds, "SkillTree_BloodMoney_Hotkey", KeyCode.F5, "Skill: Blood Money", "Activate 'Blood Money' skill");
            InfectiousPersonalityHotkey = new ConfigEntry<KeyCode>(Keybinds, "SkillTree_InfectiousPersonality_Hotkey", KeyCode.F6, "Skill: Infectious Personality", "Activate 'Infectious Personality' skill");
            AdrenalineSurgeHotkey = new ConfigEntry<KeyCode>(Keybinds, "SkillTree_AdrenalineSurge_Hotkey", KeyCode.F7, "Skill: Adrenaline Surge", "Activate 'Adrenaline Surge' skill");
            AntiGravityBongHotkey = new ConfigEntry<KeyCode>(Keybinds, "SkillTree_AntiGravityBong_Hotkey", KeyCode.F8, "Skill: Anti-Gravity Bong", "Activate 'Anti-Gravity Bong' skill");
            Keybinds.SetFilePath($"UserData/SkillTree_Config.cfg", true, false);

            DebugOptions = MelonPreferences.CreateCategory($"SkillTree_DebugOptions", $"Debug Options");
            ResetSkills = new ConfigEntry<bool>(DebugOptions, "SkillTree_ResetSkills", false, "Reset skills on next game load", "Debug: Enable this option and reload your save to reset your skills");
            ResetConfiguration = new ConfigEntry<bool>(DebugOptions, "SkillTree_ResetConfiguration", false, "Reset all options", "Debug: Enable this option to reset mod options to default.");
            DebugOptions.SetFilePath($"UserData/SkillTree_Config.cfg", true, false);

            ResetConfiguration.Entry.OnEntryValueChanged.Subscribe((oldVal, newVal) => ResetConfig(newVal));
        }
    }
}
