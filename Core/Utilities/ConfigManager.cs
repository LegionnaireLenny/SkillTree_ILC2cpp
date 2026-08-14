using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.Money;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.PlayerScripts.Health;
using MelonLoader;
using MelonLoader.Preferences;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SkillTree.Core.Utilities
{
    public class ConfigManager
    {
        public class ConfigEntry<T>
        {
            public MelonPreferences_Entry<T> Entry { get; private set; }

            public ConfigEntry(MelonPreferences_Entry<T> entry)
            {
                Entry = entry;
            }

            public T GetValue(bool useDefault = false)
            {
                return useDefault ? Entry.DefaultValue : Entry.Value;
            }

            public void SetValue(T value)
            {
                Entry.Value = value;
            }

            public void SetDefault()
            {
                Entry.Value = Entry.DefaultValue;
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
                    (property.GetValue(obj) as ConfigEntry<string>)?.SetDefault();
                    (property.GetValue(obj) as ConfigEntry<List<string>>)?.SetDefault();
                    (property.GetValue(obj) as ConfigEntry<int>)?.SetDefault();
                    (property.GetValue(obj) as ConfigEntry<bool>)?.SetDefault();
                    (property.GetValue(obj) as ConfigEntry<KeyCode>)?.SetDefault();
                    (property.GetValue(obj) as ConfigEntry<LogLevel>)?.SetDefault();
                    (property.GetValue(obj) as ConfigEntry<Color>)?.SetDefault();
                }
                catch (Exception) { }
            }
        }

        public static Action OnLocaleChanged;

        private static MelonPreferences_Category BaseGameSettings { get; set; }
        public static ConfigEntry<float> BaseHealth { get; set; }
        public static ConfigEntry<float> BaseHealthRegen { get; set; }
        public static ConfigEntry<float> BaseHealthRegenDelay { get; set; }
        public static ConfigEntry<float> BaseStamina { get; set; }
        public static ConfigEntry<float> BaseJumpHeight { get; set; }
        public static ConfigEntry<float> BaseArrestTime { get; set; }
        public static ConfigEntry<float> BaseArrestRadius { get; set; }
        public static ConfigEntry<int>   BaseHarvestXPGain { get; set; }
        public static ConfigEntry<int>   BaseNewMixXPGain { get; set; }
        public static ConfigEntry<int>   BaseCounterOfferXPGain { get; set; }
        public static ConfigEntry<int>   BaseDryingRackCapacity { get; set; }
        public static ConfigEntry<int>   BaseCauldronOutput { get; set; }
        public static ConfigEntry<float> BaseWeeklyDepositLimit { get; set; }
        public static ConfigEntry<int>   BaseTrashGrabberBinSize { get; set; }
        public static ConfigEntry<int>   BaseMaxCustomer { get; set; }
        public static ConfigEntry<int>   BaseDeadDropItemLimit { get; set; }
        public static ConfigEntry<int>   BaseMaxChemistStations { get; set; }
        public static ConfigEntry<int>   BaseMaxBotanistStations { get; set; }

        private static MelonPreferences_Category Enforcer { get; set; }
        public static ConfigEntry<float> HealthBonus { get; set; }
        public static ConfigEntry<float> HealthRegenBonus { get; set; }
        public static ConfigEntry<float> HealthRegenDelayMultiplier { get; set; }
        public static ConfigEntry<float> StaminaBonus { get; set; }
        public static ConfigEntry<float> MoveSpeedBonus { get; set; }
        public static ConfigEntry<float> JumpHeightBonus { get; set; }
        public static ConfigEntry<float> VisibilityMultiplier { get; set; }
        public static ConfigEntry<float> PickpocketDifficultyMultiplier { get; set; }
        public static ConfigEntry<float> PickpocketMinimumSuccessWidth { get; set; }
        public static ConfigEntry<float> TimeSkipGrowthMultiplier { get; set; }
        public static ConfigEntry<int>   InventoryStackSizeBonus { get; set; }
        public static ConfigEntry<float> AimTimeMultiplier { get; set; }
        public static ConfigEntry<float> MaxSpreadMultiplier { get; set; }
        public static ConfigEntry<float> MinSpreadMultiplier { get; set; }
        public static ConfigEntry<int>   AmmoCapacityBonus { get; set; }
        public static ConfigEntry<float> ArrestTimeBonus { get; set; }
        public static ConfigEntry<float> ArrestRadiusBonus { get; set; }
        public static ConfigEntry<float> SchoolOfHardKnocksXPBonus { get; set; }
        public static ConfigEntry<int>   PoliceXPBonus { get; set; }
        public static ConfigEntry<int>   CartelGoonXPBonus { get; set; }
        public static ConfigEntry<int>   CartelDealerXPBonus { get; set; }

        private static MelonPreferences_Category Provisioner { get; set; }
        public static ConfigEntry<int>   CauldronOutputBonus { get; set; }
        public static ConfigEntry<int>   MixDryOutputSizeBonus { get; set; }
        public static ConfigEntry<int>   ChemistStationSpeedBonus { get; set; }
        public static ConfigEntry<int>   PlantSeedXP { get; set; }
        public static ConfigEntry<int>   DrugProductionXP { get; set; }
        public static ConfigEntry<int>   DrugPackagingXP { get; set; }
        public static ConfigEntry<int>   DrugMixingXP { get; set; }
        public static ConfigEntry<int>   YieldBonusPot { get; set; }
        public static ConfigEntry<int>   YieldBonusGrowTent { get; set; }
        public static ConfigEntry<float> YieldMultiplierBonusPot { get; set; }
        public static ConfigEntry<float> YieldMultiplierBonusGrowTent { get; set; }
        public static ConfigEntry<float> QualityBonusPot { get; set; }
        public static ConfigEntry<float> QualityBonusGrowTent { get; set; }
        public static ConfigEntry<float> QualityBonusShrooms { get; set; }
        public static ConfigEntry<float> GreenThumbBonus { get; set; }
        public static ConfigEntry<float> MoistureDrainBonus { get; set; }
        public static ConfigEntry<float> MeisterXPBonus { get; set; }
        public static ConfigEntry<int>   HarvestXPBonus { get; set; }
        public static ConfigEntry<float> NewMixXPBonus { get; set; }

        private static MelonPreferences_Category Hustler { get; set; }
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
        public static ConfigEntry<float> MultiLevelMarketeerXPBonus { get; set; }
        public static ConfigEntry<float> SaleValueXPBonus { get; set; }
        public static ConfigEntry<int>   CounterOfferXPBonus { get; private set; }
        public static ConfigEntry<float> ProductShortChanceBonus { get; private set; }
        public static ConfigEntry<int>   ProductExcessCashBonus { get; private set; }

        private static MelonPreferences_Category Logistician { get; set; }
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
        public static ConfigEntry<float> EducatedWorkforceBonus { get; set; }

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
        public static ConfigEntry<string> Locale { get; set; }
        public static ConfigEntry<List<string>> OverwriteLocaleFiles { get; set; }
        public static ConfigEntry<bool> UseDefault { get; set; }
        public static ConfigEntry<LogLevel> LoggingLevel { get; set; }
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
            UserSettings = MelonPreferences.CreateCategory("SkillTree_UserSettings", "User Settings");
            Locale = new ConfigEntry<string>(UserSettings.CreateEntry("SkillTree_Locale", "en_US", "Language Selection"));
            OverwriteLocaleFiles = new ConfigEntry<List<string>>(UserSettings.CreateEntry("SkillTree_OverwriteLocaleFiles", new List<string>() { "en_US" }, "Overwrite Locale Files", "These locales will have their localized string data overwritten by the mod's default data when the game is launched. Remove a locale if you wish to customize it."));
            UseDefault = new ConfigEntry<bool>(UserSettings.CreateEntry("SkillTree_UseDefaultSkillParameters", true, "Use Default Skill Parameters", "If enabled, skills will use their default parameters. Disable this if you want to customize skill parameters. Does not apply to options from User Settings or Keybindings."));
            LoggingLevel = new ConfigEntry<LogLevel>(UserSettings.CreateEntry("SkillTree_LoggingLevel", LogLevel.Info, "Log Level", "Debug - Shows all log messages; Info - Shows informational messages, warnings, and errors; Error - Shows only warnings and errors"));
            AutoUnlockPrerequisites = new ConfigEntry<bool>(UserSettings.CreateEntry("SkillTree_AutoUnlockPrerequisites", true, "Auto Unlock Prerequisite Skills", "If enabled, attempting to level a locked skill will automatically unlock all prerequisite skills and level the selected skill once"));
            EnableCrosshair = new ConfigEntry<bool>(UserSettings.CreateEntry("SkillTree_EnableCrosshair", true, "Enable Crosshair", "If enabled, the crosshair stays enabled while wielding a ranged weapon"));
            EnableContractColors = new ConfigEntry<bool>(UserSettings.CreateEntry("SkillTree_EnableContractColors", true, "Enable Contract Colors", "If enabled, contract icon colors can be customized and will change colors to indicate contracts within their delivery window"));
            ContractReadyBackgroundColor = new ConfigEntry<Color>(UserSettings.CreateEntry("SkillTree_ContractReadyBackgroundColor", new Color(0.2984f, 0.6226f, 0.2673f, 1f), "Contract Ready: Background Color", "Icon background color for contracts that are within their delivery window"));
            ContractReadyFillColor = new ConfigEntry<Color>(UserSettings.CreateEntry("SkillTree_ContractReadyFillColor", new Color(1f, 1f, 1f, 1f), "Contract Ready: Fill Color", "Icon fill color for contracts that are within their delivery window"));
            ContractNotReadyBackgroundColor = new ConfigEntry<Color>(UserSettings.CreateEntry("SkillTree_ContractNotReady_BackgroundColor", new Color(0.6984f, 0.6226f, 0.4673f, 1f), "Contract Not Ready: Background Color", "Icon background color for contracts that are outside their delivery window"));
            ContractNotReadyFillColor = new ConfigEntry<Color>(UserSettings.CreateEntry("SkillTree_ContractNotReady_FillColor", new Color(1f, 1f, 1f, 1f), "Contract Not Ready: Fill Color", "Icon fill color for contracts that are outside their delivery window"));
            UserSettings.SetFilePath(Core.ConfigFile, true, false);

            Keybinds = MelonPreferences.CreateCategory("SkillTree_Keybinds", "Keybindings");
            MenuHotkey = new ConfigEntry<KeyCode>(Keybinds.CreateEntry($"SkillTree_Menu_Hotkey", KeyCode.BackQuote, "Menu Hotkey", "Open the skill tree menu"));
            LevelSkillHotkey = new ConfigEntry<KeyCode>(Keybinds.CreateEntry($"SkillTree_LevelSkill_Hotkey", KeyCode.Space, "Level Skill Hotkey", "While the skill tree is open, levels the currently selected skill"));
            GoodSamaritanHotkey = new ConfigEntry<KeyCode>(Keybinds.CreateEntry("SkillTree_GoodSamaritan_Hotkey", KeyCode.F1, "Skill: Good Samaritan", "Activate 'Good Samaritan' skill"));
            BloodRushHotkey = new ConfigEntry<KeyCode>(Keybinds.CreateEntry("SkillTree_BloodRush_Hotkey", KeyCode.F2, "Skill: Blood Rush", "Activate 'Blood Rush' skill"));
            SiphonFundsHotkey = new ConfigEntry<KeyCode>(Keybinds.CreateEntry("SkillTree_SiphonFundsHotkey", KeyCode.F3, "Skill: Siphon Funds", "Activate 'Siphon Funds' skill"));
            TrickledownHotkey = new ConfigEntry<KeyCode>(Keybinds.CreateEntry("SkillTree_Trickledown_Hotkey", KeyCode.F4, "Skill: Trickle-down Economics", "Activate 'Trickle-down Economics' skill"));
            BloodMoneyHotkey = new ConfigEntry<KeyCode>(Keybinds.CreateEntry("SkillTree_BloodMoney_Hotkey", KeyCode.F5, "Skill: Blood Money", "Activate 'Blood Money' skill"));
            InfectiousPersonalityHotkey = new ConfigEntry<KeyCode>(Keybinds.CreateEntry("SkillTree_InfectiousPersonality_Hotkey", KeyCode.F6, "Skill: Infectious Personality", "Activate 'Infectious Personality' skill"));
            AdrenalineSurgeHotkey = new ConfigEntry<KeyCode>(Keybinds.CreateEntry("SkillTree_AdrenalineSurge_Hotkey", KeyCode.F7, "Skill: Adrenaline Surge", "Activate 'Adrenaline Surge' skill"));
            AntiGravityBongHotkey = new ConfigEntry<KeyCode>(Keybinds.CreateEntry("SkillTree_AntiGravityBong_Hotkey", KeyCode.F8, "Skill: Anti-Gravity Bong", "Activate 'Anti-Gravity Bong' skill"));
            Keybinds.SetFilePath(Core.ConfigFile, true, false);

            BaseGameSettings = MelonPreferences.CreateCategory("SkillTree_BaseGameSettings", "Base Game Settings");
            BaseHealth = new ConfigEntry<float>(BaseGameSettings.CreateEntry("SkillTree_BaseMaxHealth", PlayerHealth.MaxHealth, "Base Maximum Health"));
            BaseHealthRegen = new ConfigEntry<float>(BaseGameSettings.CreateEntry("SkillTree_BaseHealthRegen", 0.5f, "Base Health Restored Per Second"));
            BaseHealthRegenDelay = new ConfigEntry<float>(BaseGameSettings.CreateEntry("SkillTree_BaseHealthRegenDelay", 30f, "Base Time Until Health Regenerates"));
            BaseStamina = new ConfigEntry<float>(BaseGameSettings.CreateEntry("SkillTree_BaseStamina", PlayerMovement.StaminaReserveMax, "Base Maximum Stamina"));
            BaseJumpHeight = new ConfigEntry<float>(BaseGameSettings.CreateEntry("SkillTree_BaseJumpHeight", PlayerMovement.JumpMultiplier, "Base Jump Height"));
            BaseArrestTime = new ConfigEntry<float>(BaseGameSettings.CreateEntry("SkillTree_BaseArrestTime", 1.75f, "Base Arrest Time"));
            BaseArrestRadius = new ConfigEntry<float>(BaseGameSettings.CreateEntry("SkillTree_BaseArrestRadius", 2.75f, "Base Arrest Radius"));
            BaseHarvestXPGain = new ConfigEntry<int>(BaseGameSettings.CreateEntry("SkillTree_BaseHarvestXPGain", 5, "Base Harvest XP Gain"));
            BaseNewMixXPGain = new ConfigEntry<int>(BaseGameSettings.CreateEntry("SkillTree_BaseNewMixXPGain", 80, "Base New Mix XP Gain"));
            BaseCounterOfferXPGain = new ConfigEntry<int>(BaseGameSettings.CreateEntry("SkillTree_BaseCounterOfferXPGain", 5, "Base CounterOffer XP Gain"));
            BaseDryingRackCapacity = new ConfigEntry<int>(BaseGameSettings.CreateEntry("SkillTree_BaseDryingRackCapacity", 20, "Base Drying Rack Capacity"));
            BaseCauldronOutput = new ConfigEntry<int>(BaseGameSettings.CreateEntry("SkillTree_BaseCauldronOutput", 10, "Base Cauldron Output"));
            BaseWeeklyDepositLimit = new ConfigEntry<float>(BaseGameSettings.CreateEntry("SkillTree_BaseWeeklyDepositLimit", ATM.WeeklyDepositLimit, "Base Weekly ATM Deposit Limit"));
            BaseTrashGrabberBinSize = new ConfigEntry<int>(BaseGameSettings.CreateEntry("SkillTree_BaseTrashGrabberBinSize", 20, "Base Trash Grabber Bin Size"));
            BaseDeadDropItemLimit = new ConfigEntry<int>(BaseGameSettings.CreateEntry("SkillTree_BaseDeadDropItemLimit", Supplier.DeaddropItemLimit, "Base Deaddrop Item Limit"));
            BaseMaxCustomer = new ConfigEntry<int>(BaseGameSettings.CreateEntry("SkillTree_BaseMaxCustomer", Dealer.MAX_CUSTOMERS, "Base Max Customers for Dealers"));
            BaseMaxChemistStations = new ConfigEntry<int>(BaseGameSettings.CreateEntry("SkillTree_BaseMaxChemistStations", 4, "Base Maximum Chemist Stations"));
            BaseMaxBotanistStations = new ConfigEntry<int>(BaseGameSettings.CreateEntry("SkillTree_BaseMaxBotanistStations", 8, "Base Maximum Botanist Stations"));
            BaseGameSettings.SetFilePath(Core.ConfigFile, true, false);

            Enforcer = MelonPreferences.CreateCategory("SkillTree_EnforcerSettings", "Enforcer Skills Settings");
            HealthRegenBonus = new ConfigEntry<float>(Enforcer.CreateEntry("SkillTree_HealthRegenBonus", 1f, "Battle-Scarred: Health Regen Bonus", "Increases the health regeneration amount multiplier"));
            HealthRegenDelayMultiplier = new ConfigEntry<float>(Enforcer.CreateEntry("SkillTree_HealthRegenDelayMultiplier", 0.5f, "Battle-Scarred: Health Regen Delay Multiplier"));
            TimeSkipGrowthMultiplier = new ConfigEntry<float>(Enforcer.CreateEntry("SkillTree_TimeSkipGrowthMultiplier", 0.33f, "Circadian Mastery: Plant Growth Progress Modifier", "By default, plants grow at one-third (0.33) of their normal speed when time is skipped.", validator: new ValueRange<float>(0.1f, 2f)));
            AmmoCapacityBonus = new ConfigEntry<int>(Enforcer.CreateEntry("SkillTree_AmmoCapacityBonus", 1, "Double-Stack Mags: Ammo Capacity Bonus", "Increases the ammo capacity multiplier"));
            MoveSpeedBonus = new ConfigEntry<float>(Enforcer.CreateEntry("SkillTree_MoveSpeedBonus", 0.15f, "Fleet Feet: Movement Speed Bonus"));
            VisibilityMultiplier = new ConfigEntry<float>(Enforcer.CreateEntry("SkillTree_VisibilityMultiplier", 0.75f, "Ghost: Visibility Multiplier"));
            PickpocketDifficultyMultiplier = new ConfigEntry<float>(Enforcer.CreateEntry("SkillTree_PickpocketDifficultyMultiplier", 0.75f, "Ghost: Pickpocket Difficulty Multiplier"));
            PickpocketMinimumSuccessWidth = new ConfigEntry<float>(Enforcer.CreateEntry("SkillTree_PickpocketMinimumSuccessWidth", 20f, "Ghost: Minimum Width of Pickpocket Green Area"));
            HealthBonus = new ConfigEntry<float>(Enforcer.CreateEntry("SkillTree_HealthBonus", 20f, "Hardy: Health Bonus"));
            InventoryStackSizeBonus = new ConfigEntry<int>(Enforcer.CreateEntry("SkillTree_InventoryStackSizeBonus", 1, "Prison Wallet: Inventory Stack Size Bonus", validator: new ValueRange<int>(0, 1000)));
            AimTimeMultiplier = new ConfigEntry<float>(Enforcer.CreateEntry("SkillTree_AimTimeMultiplier", 0.5f, "Quick Draw McGraw: Ranged Weapon Aim Time Multiplier"));
            SchoolOfHardKnocksXPBonus = new ConfigEntry<float>(Enforcer.CreateEntry("SkillTree_SchoolOfHardKnocksXPBonus", 0.125f, "School Of Hard Knocks: XP Bonus"));
            MaxSpreadMultiplier = new ConfigEntry<float>(Enforcer.CreateEntry("SkillTree_MaxSpreadMultiplier", 0.35f, "Sharpshooter: Maximum Ranged Weapon Spread Multiplier"));
            MinSpreadMultiplier = new ConfigEntry<float>(Enforcer.CreateEntry("SkillTree_MinSpreadMultiplier", 0.35f, "Sharpshooter: Minimum Ranged Weapon Spread Multiplier"));
            ArrestTimeBonus = new ConfigEntry<float>(Enforcer.CreateEntry("SkillTree_ArrestTimeBonus", 1f, "Slippery: Arrest Time Bonus", "Increases the arrest time multiplier"));
            ArrestRadiusBonus = new ConfigEntry<float>(Enforcer.CreateEntry("SkillTree_ArrestRadiusBonus", 0.75f, "Slippery: Arrest Radius Multiplier"));
            StaminaBonus = new ConfigEntry<float>(Enforcer.CreateEntry("SkillTree_StaminaBonus", 0.30f, "Spring-Heeled: Stamina Bonus"));
            JumpHeightBonus = new ConfigEntry<float>(Enforcer.CreateEntry("SkillTree_JumpHeightBonus", 0.35f, "Spring-Heeled: Jump Height Bonus"));
            PoliceXPBonus = new ConfigEntry<int>(Enforcer.CreateEntry("SkillTree_PoliceXPBonus", 12, "CombatExperience: Police XP Bonus"));
            CartelGoonXPBonus = new ConfigEntry<int>(Enforcer.CreateEntry("SkillTree_CartelGoonXPBonus", 20, "CombatExperience: Cartel Goon XP Bonus"));
            CartelDealerXPBonus = new ConfigEntry<int>(Enforcer.CreateEntry("SkillTree_CartelDealerXPBonus", 30, "CombatExperience: Cartel Dealer Bonus"));
            Enforcer.SetFilePath(Core.ConfigFile, true, false);

            Provisioner = MelonPreferences.CreateCategory("SkillTree_ProvisionerSettings", "Provisioner Skills Settings");
            PlantSeedXP = new ConfigEntry<int>(Provisioner.CreateEntry("SkillTree_PlantSeedXP", 2, "Apprenticeship: Seed/Spore Planting XP"));
            DrugProductionXP = new ConfigEntry<int>(Provisioner.CreateEntry("SkillTree_DrugProductionXP", 5, "Apprenticeship: Drug Production XP"));
            DrugPackagingXP = new ConfigEntry<int>(Provisioner.CreateEntry("SkillTree_DrugPackagingXP", 1, "Apprenticeship: Drug Packaging XP"));
            DrugMixingXP = new ConfigEntry<int>(Provisioner.CreateEntry("SkillTree_DrugMixingXP", 1, "Apprenticeship: Drug Mixing XP"));
            HarvestXPBonus = new ConfigEntry<int>(Provisioner.CreateEntry("SkillTree_HarvestXPBonus", 2, "Apprenticeship: Harvest XP Bonus", "Increases the harvest XP multiplier"));
            NewMixXPBonus = new ConfigEntry<float>(Provisioner.CreateEntry("SkillTree_NewMixXPBonus", 0.25f, "Apprenticeship: New Mix XP Bonus", "Increases the new mix XP multiplier"));
            MixDryOutputSizeBonus = new ConfigEntry<int>(Provisioner.CreateEntry("SkillTree_MixDryOutputSizeBonus", 1, "Crankin' One Out: Mixer/drying rack capacity bonus", "Increases the capacity multiplier"));
            GreenThumbBonus = new ConfigEntry<float>(Provisioner.CreateEntry("SkillTree_GreenThumbBonus", 0.075f, "Green Thumb: Plant Growth Speed Bonus"));
            MeisterXPBonus = new ConfigEntry<float>(Provisioner.CreateEntry("SkillTree_MeisterXPBonus", 0.125f, "Meister: XP Bonus"));
            YieldBonusPot = new ConfigEntry<int>(Provisioner.CreateEntry("SkillTree_YieldBonusPots", 1, "Bountiful Harvest: Base Yield Bonus for Pots"));
            YieldBonusGrowTent = new ConfigEntry<int>(Provisioner.CreateEntry("SkillTree_YieldBonusGrowTents", 0, "Bountiful Harvest: Base Yield Bonus for Grow Tents"));
            YieldMultiplierBonusPot = new ConfigEntry<float>(Provisioner.CreateEntry("SkillTree_YieldMultiplierBonusPot", 0f, "Bountiful Harvest: Yield Multiplier Bonus for Pots"));
            YieldMultiplierBonusGrowTent = new ConfigEntry<float>(Provisioner.CreateEntry("SkillTree_YieldMultiplierBonusGrowTent", 0.17f, "Bountiful Harvest: Yield Multiplier Bonus for Grow Tents"));
            QualityBonusPot = new ConfigEntry<float>(Provisioner.CreateEntry("SkillTree_QualityBonusPlants", 0.12f, "Advanced Pot Techniques: Quality Bonus for Pots"));
            QualityBonusGrowTent = new ConfigEntry<float>(Provisioner.CreateEntry("SkillTree_QualityBonusGrowTent", 0.30f, "Pitchin' a Tent: Quality Bonus for Grow Tents"));
            QualityBonusShrooms = new ConfigEntry<float>(Provisioner.CreateEntry("SkillTree_QualityBonusShrooms", 0.15f, "Mushroomancer: Quality Bonus for Mushrooms"));
            ChemistStationSpeedBonus = new ConfigEntry<int>(Provisioner.CreateEntry("SkillTree_ChemistStationSpeedBonus", 1, "Quick Crafter: Crafting Speed Bonus", "Increases the speed multiplier"));
            MoistureDrainBonus = new ConfigEntry<float>(Provisioner.CreateEntry("SkillTree_MoistureDrainBonus", 0.5f, "Wet-Ass Plants: Moisture Drain Multiplier"));
            CauldronOutputBonus = new ConfigEntry<int>(Provisioner.CreateEntry("SkillTree_CauldronOutputBonus", 1, "Witch's Brew: Cauldron Output Bonus", "Increases the output multiplier"));
            Provisioner.SetFilePath(Core.ConfigFile, true, false);

            Hustler = MelonPreferences.CreateCategory("SkillTree_HustlerSettings", "Hustler Skills Settings");
            CustomerOrderLimitBonus = new ConfigEntry<int>(Hustler.CreateEntry("SkillTree_CustomerOrderLimitBonus", 3, "Captive Market: Customer Order Limit Bonus"));
            CustomerOrderLimitRankBonus = new ConfigEntry<int>(Hustler.CreateEntry("SkillTree_CustomerOrderLimitRankBonus", 1, "Captive Market: Customer Order Limit Rank Bonus"));
            TrashGrabberBinSizeBonus = new ConfigEntry<float>(Hustler.CreateEntry("SkillTree_TrashGrabberBinSizeBonus", 1f, "Community Service: Trash Grabber Bin Size Bonus", "Increases the trash grabber's bin size multiplier"));
            TrashPickupRadius = new ConfigEntry<float>(Hustler.CreateEntry("SkillTree_TrashPickupRadius", 0.45f, "Community Service: Trash Grabber Pickup Radius"));
            TrashPickupRadiusBonus = new ConfigEntry<float>(Hustler.CreateEntry("SkillTree_TrashPickupRadiusBonus", 0f, "Community Service: Trash Grabber Pickup Radius Bonus", "Increases the trash grabber's pickup radius multiplier"));
            SaleValueXPBonus = new ConfigEntry<float>(Hustler.CreateEntry("SkillTree_SaleValueXPBonus", 0.1f, "Haggler: Sale Value XP Bonus"));
            CounterOfferXPBonus = new ConfigEntry<int>(Hustler.CreateEntry("SkillTree_CounterOfferXPBonus", 2, "Haggler: Counter Offer XP Bonus", "Increases counter offer XP multiplier"));
            ProductShortChanceBonus = new ConfigEntry<float>(Hustler.CreateEntry("SkillTree_ProductShortChanceBonus", 0.15f, "Scam Artist: Product Short Accept Chance Bonus"));
            ProductExcessCashBonus = new ConfigEntry<int>(Hustler.CreateEntry("SkillTree_ProductExcessCashBonus", 1, "Munificent: Generosity Cash Bonus", "Increases the multiplier for generosity bonus"));
            ATMDepositBonus = new ConfigEntry<float>(Hustler.CreateEntry("SkillTree_ATMDepositBonus", 2500f, "Hoard the Wealth: ATM Deposit Limit Bonus"));
            MultiLevelMarketeerXPBonus = new ConfigEntry<float>(Hustler.CreateEntry("SkillTree_MultiLevelMarketeerXPBonus", 0.125f, "Multi-level Marketeer: XP Bonus"));
            TrashValueBonus = new ConfigEntry<int>(Hustler.CreateEntry("SkillTree_TrashValueBonus", 1, "Sacar La Basura: Trash Value Bonus"));
            PawnPriceBonus = new ConfigEntry<float>(Hustler.CreateEntry("SkillTree_PawnPriceBonus", 0.25f, "Sacar La Basura: Pawn Price Bonus"));
            CustomerSampleAcceptBonus = new ConfigEntry<float>(Hustler.CreateEntry("SkillTree_CustomerSampleAcceptBonus", 0.05f, "Silver Tongued Devil: Sample Acceptance Chance Bonus"));
            CustomerCashBonus = new ConfigEntry<float>(Hustler.CreateEntry("SkillTree_CustomerCashBonus", 0.25f, "Spread the Wealth: Customer Weekly Spend Limit Bonus"));
            LaunderingBonus = new ConfigEntry<float>(Hustler.CreateEntry("SkillTree_LaunderingBonus", 0.30f, "Squeaky Clean: Business Laundering Capacity Bonus"));
            Hustler.SetFilePath(Core.ConfigFile, true, false);

            Logistician = MelonPreferences.CreateCategory("SkillTree_LogisticianSettings", "Logistician Skills Settings");
            DealerCustomerLimitBonus = new ConfigEntry<int>(Logistician.CreateEntry("SkillTree_CustomerLimitBonus", 2, "Expansive Empire: Customer Limit Bonus"));
            ChemistActionDurationMultiplier = new ConfigEntry<float>(Logistician.CreateEntry("SkillTree_ChemistActionDurationMultiplier", 0.5f, "Fast Chemists: Chemist Action Duration Multiplier", validator: new ValueRange<float>(0.1f, 10f)));
            BotanistActionDurationMultiplier = new ConfigEntry<float>(Logistician.CreateEntry("SkillTree_BotanistActionDurationMultiplier", 0.5f, "Fast Farmers: Botanist Action Duration Multiplier", validator: new ValueRange<float>(0.1f, 10f)));
            HandlerPackagingSpeedMultiplier = new ConfigEntry<float>(Logistician.CreateEntry("SkillTree_HandlerPackagingSpeedMultiplier", 2f, "Fast Handlers: Handler Packaging Speed Multiplier", validator: new ValueRange<float>(0.1f, 10f)));
            DealerSpeedBonus = new ConfigEntry<float>(Logistician.CreateEntry("SkillTree_DealerSpeedBonus", 1f, "Motivational Leader: Dealer Speed Bonus", "Increases the speed multiplier"));
            EmployeeStationBonus = new ConfigEntry<int>(Logistician.CreateEntry("SkillTree_EmployeeStationBonus", 2, "Overworked/Underpaid: Station bonus for Botanists and Chemists"));
            SupplierCashBonus = new ConfigEntry<float>(Logistician.CreateEntry("SkillTree_SupplierCashBonus", 0.675f, "Reliable Bus. Partner: Dead Drop Order Limit Bonus"));
            SupplierItemBonus = new ConfigEntry<float>(Logistician.CreateEntry("SkillTree_SupplierItemBonus", 0.50f, "Reliable Bus. Partner: Dead Drop Item Limit Bonus"));
            EmployeeMoveSpeedBonus = new ConfigEntry<float>(Logistician.CreateEntry("SkillTree_EmployeeMoveSpeedBonus", 3f, "RUN BITCH RUN!: Employee MoveSpeedMultiplier", "Value is clamped between 0.1f (0.1x speed) and 10f (10x speed)"));
            EducatedWorkforceBonus = new ConfigEntry<float>(Enforcer.CreateEntry("SkillTree_EducatedWorkforceBonus", 0.125f, "Educated Workforce: XP Bonus"));
            DealerCutReduction = new ConfigEntry<float>(Logistician.CreateEntry("SkillTree_DealerCutReduction", 0.05f, "Wage Garnishment: Dealer Cut Reduction"));
            Logistician.SetFilePath(Core.ConfigFile, true, false);

            Special = MelonPreferences.CreateCategory("SkillTree_SpecialSettings", "Special Skills Settings");
            AdrenalineSurgeZappedEffect = new ConfigEntry<bool>(Special.CreateEntry("SkillTree_AdrenalineSurgeZappedEffect", true, "Adrenaline Surge: Enabled Zapped Effect"));
            AdrenalineSurgeMaxCharges = new ConfigEntry<int>(Special.CreateEntry("SkillTree_AdrenalineSurgeCharges", 3, "Adrenaline Surge: Max Number of Charges"));
            AdrenalineSurgeDuration = new ConfigEntry<float>(Special.CreateEntry("SkillTree_AdrenalineSurgeDuration", 15f, "Adrenaline Surge: Effect Duration"));
            AdrenalineSurgeSpeedMultiplier = new ConfigEntry<float>(Special.CreateEntry("SkillTree_AdrenalineSurgeSpeedMultiplier", 3f, "Adrenaline Surge: Speed Multiplier"));
            AdrenalineSurgeJumpMultiplier = new ConfigEntry<float>(Special.CreateEntry("SkillTree_AdrenalineSurgeJumpMultiplier", 3f, "Adrenaline Surge: Jump Multiplier"));
            AntiGravityBongDuration = new ConfigEntry<float>(Special.CreateEntry("SkillTree_AntiGravityBongDuration", 15f, "Anti-Gravity Bong: Effect Duration"));
            AntiGravityBongRadius = new ConfigEntry<float>(Special.CreateEntry("SkillTree_AntiGravityBongRadius", 10f, "Anti-Gravity Bong: Effect Radius"));
            AntiGravityBongCooldown = new ConfigEntry<float>(Special.CreateEntry("SkillTree_AntiGravityBongCooldown", 60f, "Anti-Gravity Bong: Skill Cooldown"));
            BloodMoneyDuration = new ConfigEntry<float>(Special.CreateEntry("SkillTree_BloodMoneyDuration", 30f, "Blood Money: Effect Duration"));
            BloodMoneyFOVChange = new ConfigEntry<float>(Special.CreateEntry("SkillTree_BloodMoneyFOVChange", 10f, "Blood Money: FOV Change"));
            BloodMoneyHeartbeatVolume = new ConfigEntry<float>(Special.CreateEntry("SkillTree_BloodMoneyHeartbeatVolume", 0.5f, "Blood Money: Heartbeat Volume"));
            BloodMoneyHeartbeatPitch = new ConfigEntry<float>(Special.CreateEntry("SkillTree_BloodMoneyHeartbeatPitch", 0.85f, "Blood Money: Heartbeat Pitch"));
            BloodMoneyScreenTint = new ConfigEntry<Color>(Special.CreateEntry("SkillTree_BloodMoneyScreenTint", new Color(1f, 1f, 0.9f, 0.2f), "Blood Money: Screen Tint"));
            PoliceKilledBonus = new ConfigEntry<float>(Special.CreateEntry("SkillTree_PoliceKilledBonus", 0.1f, "Blood Rush: Bonus Health per Police Killed"));
            CartelKilledBonus = new ConfigEntry<float>(Special.CreateEntry("SkillTree_CartelKilledBonus", 0.1f, "Blood Rush: Bonus Health per Cartel Killed"));
            BloodRushRegenDelayMultiplier = new ConfigEntry<float>(Special.CreateEntry("SkillTree_BloodRushRegenDelayMultiplier", 0.2f, "Blood Rush: Health Regen Delay Multiplier"));
            BloodRushHealthBonusMultiplier = new ConfigEntry<float>(Special.CreateEntry("SkillTree_BloodRushHealthBonusMultiplier", 2f, "Blood Rush: Bonus Health Cap Multiplier"));
            BloodRushHealthBonusCap = new ConfigEntry<float>(Special.CreateEntry("SkillTree_BloodRushHealthBonusCap", 30f, "Blood Rush: Bonus Health Cap"));
            BloodRushDuration = new ConfigEntry<float>(Special.CreateEntry("SkillTree_BloodRushDuration", 60f, "Blood Rush: Effect Duration"));
            BloodRushFOVChange = new ConfigEntry<float>(Special.CreateEntry("SkillTree_BloodRushFOVChange", 10f, "Blood Rush: FOV Change"));
            BloodRushHeartbeatVolume = new ConfigEntry<float>(Special.CreateEntry("SkillTree_BloodRushHeartbeatVolume", 0.25f, "Blood Rush: Heartbeat Volume"));
            BloodRushHeartbeatPitch = new ConfigEntry<float>(Special.CreateEntry("SkillTree_BloodRushHeartbeatPitch", 1f, "Blood Rush: Heartbeat Pitch"));
            BloodRushScreenTint = new ConfigEntry<Color>(Special.CreateEntry("SkillTree_BloodRushScreenTint", new Color(1f, 0.9f, 0.9f, 0.2f), "Blood Rush: Screen Tint"));
            InfectiousPersonalityRange = new ConfigEntry<float>(Special.CreateEntry("SkillTree_InfectiousPersonalityRange", 15f, "Infectious Personality: Effect Range"));
            SiphonFundsBaseConversionRate = new ConfigEntry<float>(Special.CreateEntry("SkillTree_SiphonFundsBaseConversionRate", 0.1f, "Siphon Funds: Base Online Balance Conversion Rate"));
            SiphonFundsOwnedBusinessBonus = new ConfigEntry<float>(Special.CreateEntry("SkillTree_SiphonFundsOwnedBusinessBonus", 0.05f, "Siphon Funds: Bonus Per Owned Business"));
            TrickleDownCashReserve = new ConfigEntry<float>(Special.CreateEntry("SkillTree_TrickledownCashReserve", 2000f, "Trickle-down Economics: Cash Reserve", "Amount of cash kept when transferring money to businesses if there's not enough to max out their capacity"));
            TrickleDownPayoutInterval = new ConfigEntry<int>(Special.CreateEntry("SkillTree_TrickleDownPayoutInterval", 6, "Trickle-down Economics: Payout Interval", "Number of hours between laundering payouts", validator: new ValueRange<int>(1, 24)));
            Special.SetFilePath(Core.ConfigFile, true, false);

            DebugOptions = MelonPreferences.CreateCategory($"SkillTree_DebugOptions", $"Debug Options");
            ResetSkills = new ConfigEntry<bool>(DebugOptions.CreateEntry("SkillTree_ResetSkills", false, "Reset skills on next game load", "Debug: Enable this option and reload your save to reset your skills"));
            ResetConfiguration = new ConfigEntry<bool>(DebugOptions.CreateEntry("SkillTree_ResetConfiguration", false, "Reset all options", "Debug: Enable this option to reset mod options to default."));
            DebugOptions.SetFilePath(Core.ConfigFile, true, false);

            ResetConfiguration.Entry.OnEntryValueChanged.Subscribe((oldVal, newVal) => ResetConfig(newVal));
            Locale.Entry.OnEntryValueChanged.Subscribe((oldVal, newVal) => OnLocaleChanged?.Invoke());
        }
    }
}
