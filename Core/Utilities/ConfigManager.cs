using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.Money;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.PlayerScripts.Health;
using MelonLoader;
using UnityEngine;

namespace SkillTree.Core.Utilities
{
    public class ConfigManager
    {
        public class ConfigEntry<T>
        {
            private readonly MelonPreferences_Entry<T> _entry;

            public ConfigEntry(MelonPreferences_Category category, string identifier, T defaultValue, string displayName = null, string description = null)
            {
                _entry = category.CreateEntry(identifier, defaultValue, displayName, description);
            }

            public T GetValue()
            {
                return (T)_entry.BoxedValue;
            }

            public void SetValue(T value)
            {
                _entry.BoxedValue = value;
            }
        }

        private static MelonPreferences_Category Stats { get; set; }
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
        public static ConfigEntry<float> XPGainBonus { get; set; }
        public static ConfigEntry<float> XPGainBonus2 { get; set; }
        public static ConfigEntry<float> SaleXPBonus { get; set; }
        public static ConfigEntry<int>   InventoryStackSizeBonus { get; set; }
        public static ConfigEntry<float> ArrestTimeBonus { get; set; }
        public static ConfigEntry<float> ArrestRadiusBonus { get; set; }
        //public static readonly float PlayerBaseMoveSpeed = PlayerMovement.StaticMoveSpeedMultiplier;
        //public static readonly float PackagerMoveSpeedMultiplier = 2f;


        private static MelonPreferences_Category Operations { get; set; }
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
        //public static readonly int StackSizeMultiplier = 2;

        private static MelonPreferences_Category Social { get; set; }
        public static ConfigEntry<float> BaseWeeklyDepositLimit { get; set; }
        public static ConfigEntry<int>   BaseMaxCustomer { get; set; }
        public static ConfigEntry<int>   BaseDeadDropItemLimit { get; set; }
        public static ConfigEntry<int>   BaseTrashGrabberBinSize { get; set; }
        public static ConfigEntry<float> TrashGrabberBinSizeBonus { get; set; }
        public static ConfigEntry<float> TrashPickupRadius { get; set; }
        public static ConfigEntry<float> TrashPickupRadiusBonus { get; set; }
        public static ConfigEntry<int>   TrashValueBonus { get; set; }
        public static ConfigEntry<float> PawnPriceBonus { get; set; }
        public static ConfigEntry<float> ATMDepositBonus { get; set; }
        public static ConfigEntry<float> CustomerSampleAcceptBonus { get; set; }
        public static ConfigEntry<float> CustomerCashBonus { get; set; }
        public static ConfigEntry<int>   DealerCustomerLimitBonus { get; set; }
        public static ConfigEntry<float> DealerCutReduction { get; set; }
        public static ConfigEntry<float> DealerSpeedBonus { get; set; }
        public static ConfigEntry<float> SupplierCashBonus { get; set; }
        public static ConfigEntry<float> SupplierItemBonus { get; set; }
        public static ConfigEntry<float> LaunderingBonus { get; set; }

        private static MelonPreferences_Category Special { get; set; }
        public static ConfigEntry<int>   BaseMaxChemistStations { get; set; }
        public static ConfigEntry<int>   BaseMaxBotanistStations { get; set; }
        public static ConfigEntry<float> PoliceKilledBonus { get; set; }
        public static ConfigEntry<float> CartelKilledBonus { get; set; }
        public static ConfigEntry<float> BloodRushRegenDelayMultiplier { get; set; }
        public static ConfigEntry<float> BloodRushHealthBonusMultiplier { get; set; }
        public static ConfigEntry<float> BloodRushHealthBonusCap { get; set; }
        public static ConfigEntry<float> BloodRushDuration { get; set; }
        public static ConfigEntry<float> SiphonFundsBaseConversionRate { get; set; }
        public static ConfigEntry<float> SiphonFundsOwnedBusinessBonus { get; set; }
        public static ConfigEntry<float> BotanistActionSpeedBonus { get; set; }
        public static ConfigEntry<float> EmployeeMoveSpeedBonus { get; set; }
        public static ConfigEntry<int>   EmployeeStationBonus { get; set; }

        private static MelonPreferences_Category UserSettings { get; set; }
        public static ConfigEntry<bool> AutoUnlockPrerequisites { get; set; }

        private static MelonPreferences_Category Keybinds { get; set; }
        public static ConfigEntry<KeyCode> MenuHotkey { get; set; }
        public static ConfigEntry<KeyCode> ActiveSkillOne { get; set; }
        public static ConfigEntry<KeyCode> ActiveSkillTwo { get; set; }
        public static ConfigEntry<KeyCode> ActiveSkillThree { get; set; }

        private static MelonPreferences_Category DebugOptions { get; set; }
        public static ConfigEntry<bool> ResetSkills { get; set; }

        public static void Initialize()
        {
            Stats = MelonPreferences.CreateCategory("SkillTree_05_StatsSettings", "Stats Skills Settings");
            BaseHealth = new ConfigEntry<float>(Stats, "SkillTree_BaseMaxHealth", PlayerHealth.MAX_HEALTH, "Base Maximum Health");
            BaseHealthRegen = new ConfigEntry<float>(Stats, "SkillTree_BaseHealthRegen", 0.5f, "Base Health Restored Per Second");
            BaseHealthRegenDelay = new ConfigEntry<float>(Stats, "SkillTree_BaseHealthRegenDelay", 30f, "Base Time Until Health Regenerates");
            BaseStamina = new ConfigEntry<float>(Stats, "SkillTree_BaseStamina", PlayerMovement.StaminaReserveMax, "Base Maximum Stamina");
            BaseMoveSpeed = new ConfigEntry<float>(Stats, "SkillTree_BaseMoveSpeed", 1f, "Base Movement Speed");
            BaseJumpHeight = new ConfigEntry<float>(Stats, "SkillTree_BaseJumpHeight", PlayerMovement.JumpMultiplier, "Base Jump Height");
            BaseArrestTime = new ConfigEntry<float>(Stats, "SkillTree_BaseArrestTime", 1.75f, "Base Arrest Time");
            BaseArrestRadius = new ConfigEntry<float>(Stats, "SkillTree_BaseArrestRadius", 2.75f, "Base Arrest Radius");
            HealthBonus = new ConfigEntry<float>(Stats, "SkillTree_HealthBonus", 20f, "Hardy - Health Bonus");
            HealthRegenBonus = new ConfigEntry<float>(Stats, "SkillTree_HealthRegenBonus", 1f, "Battle-Scarred - Health Regen Bonus", "Increases the health regeneration amount multiplier");
            HealthRegenDelayMultiplier = new ConfigEntry<float>(Stats, "SkillTree_HealthRegenDelayMultiplier", 0.5f, "Battle-Scarred - Health Regen Delay Multiplier");
            StaminaBonus = new ConfigEntry<float>(Stats, "SkillTree_StaminaBonus", 0.30f, "Spring-Heeled - Stamina Bonus");
            JumpHeightBonus = new ConfigEntry<float>(Stats, "SkillTree_JumpHeightBonus", 0.35f, "Spring-Heeled - Jump Height Bonus");
            MoveSpeedBonus = new ConfigEntry<float>(Stats, "SkillTree_MoveSpeedBonus", 0.15f, "Fleet Feet - Movement Speed Bonus");
            VisibilityMultiplier = new ConfigEntry<float>(Stats, "SkillTree_VisibilityMultiplier", 0.75f, "Ghost - Visibility Multiplier");
            PickpocketDifficultyMultiplier = new ConfigEntry<float>(Stats, "SkillTree_PickpocketDifficultyMultiplier", 0.75f, "Ghost - Pickpocket Difficulty Multiplier");
            PickpocketMinimumSuccessWidth = new ConfigEntry<float>(Stats, "SkillTree_PickpocketMinimumSuccessWidth", 20f, "Ghost - Minimum Width of Pickpocket Green Area");
            XPGainBonus = new ConfigEntry<float>(Stats, "SkillTree_XPGainBonus", 0.05f, "Fast Learner - XP Bonus");
            XPGainBonus2 = new ConfigEntry<float>(Stats, "SkillTree_XPGainBonus2", 0.1f, "Turbo Nerdo - XP Bonus");
            SaleXPBonus = new ConfigEntry<float>(Stats, "SkillTree_SalesXPBonus", 0.05f, "Kingpin - Sales XP Bonus");
            InventoryStackSizeBonus = new ConfigEntry<int>(Stats, "SkillTree_InventoryStackSizeBonus", 1, "Prison Wallet - Inventory Stack Size Bonus");
            ArrestTimeBonus = new ConfigEntry<float>(Stats, "SkillTree_ArrestTimeBonus", 1f, "Slippery - Arrest Time Bonus", "Increases the arrest time multiplier");
            ArrestRadiusBonus = new ConfigEntry<float>(Stats, "SkillTree_ArrestRadiusBonus", 0.75f, "Slippery - Arrest Radius Multiplier");
            Stats.SetFilePath($"UserData/SkillTree_Config.cfg", true, false);

            Operations = MelonPreferences.CreateCategory("SkillTree_06_OperationsSettings", "Operations Skills Settings");
            BaseDryingRackCapacity = new ConfigEntry<int>(Operations, "SkillTree_BaseDryingRackCapacity", 20, "Base Drying Rack Capacity");
            BaseCauldronOutput = new ConfigEntry<int>(Operations, "SkillTree_BaseCauldronOutput", 10, "Base Cauldron Output");
            BaseYieldPlants = new ConfigEntry<int>(Operations, "SkillTree_BaseYieldPlants", 12, "Base Yield of Plants");
            BasePlantQualityLevel = new ConfigEntry<float>(Operations, "SkillTree_BasePlantQualityLevel", 0.5f, "Base Quality Level of Plants");
            CauldronOutputBonus = new ConfigEntry<int>(Operations, "SkillTree_CauldronOutputBonus", 1, "Witch's Brew - Cauldron Output Bonus", "Increases the output multiplier");
            MixDryOutputSizeBonus = new ConfigEntry<int>(Operations, "SkillTree_MixDryOutputSizeBonus", 1, "Crankin' One Out - Mixer/drying rack capacity bonus", "Increases the capacity multiplier");
            ChemistStationSpeedBonus = new ConfigEntry<int>(Operations, "SkillTree_ChemistStationSpeedBonus", 1, "Quick Crafter - Crafting Speed Bonus", "Increases the speed multiplier");
            QualityBonusGrowTent = new ConfigEntry<float>(Operations, "SkillTree_QualityBonusGrowTent", 0.16f, "Pitchin' a Tent - Quality Bonus for Grow Tents");
            QualityBonusPlants = new ConfigEntry<float>(Operations, "SkillTree_QualityBonusPlants", 0.15f, "Advanced Pot Techniques - Quality Bonus for Pots");
            QualityBonusShrooms = new ConfigEntry<float>(Operations, "SkillTree_QualityBonusShrooms",  0.15f, "Mushroomancer - Quality Bonus for Mushrooms");
            YieldBonusPlants = new ConfigEntry<int>(Operations, "SkillTree_YieldBonusPlants", 1, "Bountiful Harvest - Yield Bonus for Plants");
            GrowthSpeedBonusPlants = new ConfigEntry<float>(Operations, "SkillTree_GrowthSpeedBonusPlants",  0.025f, "Green Thumb/Plant Whisperer - Plant Growth Speed Bonus");
            MoistureDrainBonus = new ConfigEntry<float>(Operations, "SkillTree_MoistureDrainBonus", 0.5f, "Wet-Ass Plants - Moisture Drain Multiplier");
            Operations.SetFilePath($"UserData/SkillTree_Config.cfg", true, false);

            Social = MelonPreferences.CreateCategory("SkillTree_07_SocialSettings", "Social Skills Settings");
            BaseWeeklyDepositLimit = new ConfigEntry<float>(Social, "SkillTree_BaseWeeklyDepositLimit", ATM.WEEKLY_DEPOSIT_LIMIT, "Base Weekly ATM Deposit Limit");
            BaseMaxCustomer = new ConfigEntry<int>(Social, "SkillTree_BaseMaxCustomer", Dealer.MAX_CUSTOMERS, "Base Max Customers for Dealers");
            BaseDeadDropItemLimit = new ConfigEntry<int>(Social, "SkillTree_BaseDeadDropItemLimit", Supplier.DEADDROP_ITEM_LIMIT, "Base Deaddrop Item Limit");
            BaseTrashGrabberBinSize = new ConfigEntry<int>(Social, "SkillTree_BaseTrashGrabberBinSize", 20, "Base Trash Grabber Bin Size");
            TrashGrabberBinSizeBonus = new ConfigEntry<float>(Social, "SkillTree_TrashGrabberBinSizeBonus", 1f, "Community Service - Trash Grabber Bin Size Bonus", "Increases the trash grabber's bin size multiplier");
            TrashPickupRadius = new ConfigEntry<float>(Social, "SkillTree_TrashPickupRadius", 0.45f, "Community Service - Trash Grabber Pickup Radius");
            TrashPickupRadiusBonus = new ConfigEntry<float>(Social, "SkillTree_TrashPickupRadiusBonus", 1f, "Community Service - Trash Grabber Pickup Radius Bonus", "Increases the trash grabber's pickup radius multiplier");
            TrashValueBonus = new ConfigEntry<int>(Social, "SkillTree_TrashValueBonus", 1, "Sacar La Basura - Trash Value Bonus");
            PawnPriceBonus = new ConfigEntry<float>(Social, "SkillTree_PawnPriceBonus", 0.25f, "Sacar La Basura - Pawn Price Bonus");
            ATMDepositBonus = new ConfigEntry<float>(Social, "SkillTree_ATMDepositBonus", 2000f, "Hoard the Wealth - ATM Deposit Limit Bonus");
            DealerCustomerLimitBonus = new ConfigEntry<int>(Social, "SkillTree_CustomerLimitBonus", 2, "Expansive Empire - Customer Limit Bonus");
            CustomerSampleAcceptBonus = new ConfigEntry<float>(Social, "SkillTree_CustomerSampleAcceptBonus", 0.05f, "Silver Tongued Devil - Sample Acceptance Chance Bonus");
            CustomerCashBonus = new ConfigEntry<float>(Social, "SkillTree_CustomerCashBonus", 0.10f, "Spread the Wealth - Customer Weekly Spend Limit Bonus");
            DealerCutReduction = new ConfigEntry<float>(Social, "SkillTree_DealerCutReduction", 0.05f, "Wage Garnishment - Dealer Cut Reduction");
            DealerSpeedBonus = new ConfigEntry<float>(Social, "SkillTree_DealerSpeedBonus", 1f, "Motivational Leader - Dealer Speed Bonus", "Increases the speed multiplier");
            SupplierCashBonus = new ConfigEntry<float>(Social, "SkillTree_SupplierCashBonus", 0.675f, "Reliable Bus. Partner - Dead Drop Order Limit Bonus");
            SupplierItemBonus = new ConfigEntry<float>(Social, "SkillTree_SupplierItemBonus", 0.50f, "Reliable Bus. Partner - Dead Drop Item Limit Bonus");
            LaunderingBonus = new ConfigEntry<float>(Social, "SkillTree_LaunderingBonus", 0.30f, "Squeaky Clean - Business Laundering Capacity Bonus");
            Social.SetFilePath($"UserData/SkillTree_Config.cfg", true, false);

            Special = MelonPreferences.CreateCategory("SkillTree_08_SpecialSettings", "Special Skills Settings");
            BaseMaxChemistStations = new ConfigEntry<int>(Special, "SkillTree_BaseMaxChemistStations", 4, "Base Maximum Chemist Stations");
            BaseMaxBotanistStations = new ConfigEntry<int>(Special, "SkillTree_BaseMaxBotanistStations", 8, "Base Maximum Botanist Stations");
            PoliceKilledBonus = new ConfigEntry<float>(Special, "SkillTree_PoliceKilledBonus", 0.1f, "Blood Rush - Bonus Health per Police Killed");
            CartelKilledBonus = new ConfigEntry<float>(Special, "SkillTree_CartelKilledBonus", 0.1f, "Blood Rush - Bonus Health per Cartel Killed");
            BloodRushRegenDelayMultiplier = new ConfigEntry<float>(Special, "SkillTree_BloodRushRegenDelayMultiplier", 0.2f, "Blood Rush - Health Regen Delay Multiplier");
            BloodRushHealthBonusMultiplier = new ConfigEntry<float>(Special, "SkillTree_BloodRushHealthBonusMultiplier", 2f, "Blood Rush - Bonus Health Cap Multiplier");
            BloodRushHealthBonusCap = new ConfigEntry<float>(Special, "SkillTree_BloodRushHealthBonusCap", 30f, "Blood Rush - Bonus Health Cap");
            BloodRushDuration = new ConfigEntry<float>(Special, "SkillTree_BloodRushDuration", 60f, "Blood Rush - Effect Duration");
            SiphonFundsBaseConversionRate = new ConfigEntry<float>(Special, "SkillTree_SiphonFundsBaseConversionRate", 0.1f, "Siphon Funds - Base Online Balance Conversion Rate");
            SiphonFundsOwnedBusinessBonus = new ConfigEntry<float>(Special, "SkillTree_SiphonFundsOwnedBusinessBonus", 0.05f, "Siphon Funds - Bonus Per Owned Business");
            BotanistActionSpeedBonus = new ConfigEntry<float>(Special, "SkillTree_BotanistActionSpeedBonus", 0.5f, "Fast Farmers - Multiplier for Botanist Action Duration");
            EmployeeMoveSpeedBonus = new ConfigEntry<float>(Special, "SkillTree_EmployeeMoveSpeedBonus", 0.33f, "RUN BITCH RUN! - Employee MovespeedScale", "Lower is faster, higher is slower. Value is clamped between 0.1f (10x speed) and 10f (0.1x speed)");
            EmployeeStationBonus = new ConfigEntry<int>(Special, "SkillTree_EmployeeStationBonus", 2, "Overworked/Underpaid - Station bonus for Botanists and Chemists");
            Special.SetFilePath($"UserData/SkillTree_Config.cfg", true, false);

            UserSettings = MelonPreferences.CreateCategory("SkillTree_00_UserSettings", "User Settings");
            AutoUnlockPrerequisites = new ConfigEntry<bool>(UserSettings, "SkillTree_Auto_Unlock_Prerequisites", true, "Auto Unlock Prerequisite Skills", "If enabled, attempting to level a locked skill will automatically unlock all prerequisite skills and level the selected skill once");
            UserSettings.SetFilePath($"UserData/SkillTree_Config.cfg", true, false);

            Keybinds = MelonPreferences.CreateCategory("SkillTree_01_Keybinds", "Keybindings");
            MenuHotkey = new ConfigEntry<KeyCode>(Keybinds, $"SkillTree_01_Menu Hotkey", KeyCode.BackQuote, "Menu Hotkey", "Open the skill tree menu");
            ActiveSkillOne = new ConfigEntry<KeyCode>(Keybinds, "SkillTree_02_Skill One", KeyCode.F1, "Skill: Good Samaritan", "Activate 'Good Samaritan' skill");
            ActiveSkillTwo = new ConfigEntry<KeyCode>(Keybinds, "SkillTree_03_Skill Two", KeyCode.F2, "Skill: Blood Rush", "Activate 'Blood Rush' skill");
            ActiveSkillThree = new ConfigEntry<KeyCode>(Keybinds, "SkillTree_04_Skill Three", KeyCode.F3, "Skill: Siphon Funds", "Activate 'Siphon Funds' skill");
            Keybinds.SetFilePath($"UserData/SkillTree_Config.cfg", true, false);

            DebugOptions = MelonPreferences.CreateCategory($"SkillTree_99_ModInfo", $"Debug Options");
            ResetSkills = new ConfigEntry<bool>(DebugOptions, "SkillTree_02_ResetSkills", true, "Reset skills on next game load", "Debug: Enable this option and reload your save to reset your skills");
            DebugOptions.SetFilePath($"UserData/SkillTree_Config.cfg", true, false);

        }
    }
}
