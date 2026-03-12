using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.Money;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.PlayerScripts.Health;
using MelonLoader;
using S1API.Property;
using SkillTree.Core.Effects;
using SkillTree.Core.Patches.Special;
using UnityEngine;

namespace SkillTree.Core.Skills
{
    public static class SkillModifiers
    {
        #region Stats
        public static readonly float PlayerBaseHealth = PlayerHealth.MAX_HEALTH;
        public static readonly float HealthBonus = 20f;
        public static readonly float PlayerBaseHealthRegen = 0.5f;
        public static readonly int HealthRegenBonus = 1;
        public static readonly float PlayerBaseHealthRegenDelay = 30f;
        public static readonly float BattleScarredRegenDelayMultiplier = 0.5f;
        public static readonly float PlayerBaseStamina = PlayerMovement.StaminaReserveMax;
        public static readonly float StaminaBonus = 0.30f;
        public static readonly float PlayerBaseMoveSpeed = 1f;
        //public static readonly float PlayerBaseMoveSpeed = PlayerMovement.StaticMoveSpeedMultiplier;
        public static readonly float PlayerBaseJumpHeight = PlayerMovement.JumpMultiplier;
        public static readonly float MoveSpeedBonus = 0.15f;
        public static readonly float JumpHeightBonus = 0.35f;
        public static readonly float VisibilityMultiplier = 0.75f;
        public static readonly float PickpocketDifficultyMultiplier = 0.75f;
        public static readonly float XPGainBonus = 0.05f;
        public static readonly float SaleXPBonus = 0.05f;
        public static readonly int InventoryStackSizeBonus = 1;
        public static readonly float BaseArrestTime = 1.75f;
        public static readonly float ArrestTimeIncreaseBonus = 1f;
        public static readonly float BaseArrestRadius = 2.75f;
        public static readonly float ArrestRadiusReductionBonus = 0.25f;
        //public static readonly float PackagerMoveSpeedMultiplier = 2f;

        public static float GetPlayerMaxHealth()
        {
            return PlayerBaseHealth + (SkillTreeData.Stats.CurrentLevel * HealthBonus) + GetBloodRushHealthBonus();
        }

        public static float GetPlayerHealthRegen()
        {
            return PlayerBaseHealthRegen * (1 + SkillTreeData.BattleScarred.CurrentLevel * HealthRegenBonus);
        }

        public static float GetPlayerHealthRegenDelay()
        {
            float battleScarred = SkillTreeData.BattleScarred.CurrentLevel == 0 ? 1f : SkillTreeData.BattleScarred.CurrentLevel * BattleScarredRegenDelayMultiplier;
            float bloodRush = BloodRush.IsBloodRushActive ? BloodRushRegenDelayMultiplier : 1f;
            float delay = PlayerBaseHealthRegenDelay * battleScarred * bloodRush;
            return delay;
        }

        public static float GetPlayerMaxStamina()
        {
            return PlayerBaseStamina * (1 + SkillTreeData.SpringHeeled.CurrentLevel * StaminaBonus);
        }

        public static float GetPlayerMoveSpeed()
        {
            return PlayerBaseMoveSpeed * (1 + SkillTreeData.MoreMovespeed.CurrentLevel * MoveSpeedBonus);
        }

        public static float GetPlayerJumpHeight()
        {
            return PlayerBaseJumpHeight * (1 + SkillTreeData.SpringHeeled.CurrentLevel * JumpHeightBonus);
        }

        public static float GetXPGainMultiplier()
        {
            return 1f + (SkillTreeData.MoreXP.CurrentLevel + SkillTreeData.MoreXP2.CurrentLevel) * XPGainBonus;
        }

        public static float GetSaleXPBonus()
        {
            return SkillTreeData.MoreXPWhenEarnMoney.CurrentLevel * SaleXPBonus;
        }

        public static float GetVisbilityMultiplier()
        {
            return SkillTreeData.Ghost.CurrentLevel == 0 ? 1f : VisibilityMultiplier;
        }

        public static float GetPickpocketDifficultyMultiplier()
        {
            return SkillTreeData.Ghost.CurrentLevel == 0 ? 1f : PickpocketDifficultyMultiplier;
        }

        public static int GetInventoryStackSizeMultiplier()
        {
            return 1 + SkillTreeData.MoreStackItem.CurrentLevel * InventoryStackSizeBonus;
        }

        public static float GetArrestTime()
        {
            return BaseArrestTime * (1 + SkillTreeData.Slippery.CurrentLevel * ArrestTimeIncreaseBonus);
        }

        public static float GetArrestRadius()
        {
            return BaseArrestRadius * (1 - SkillTreeData.Slippery.CurrentLevel * ArrestRadiusReductionBonus);
        }

        #endregion Stats

        #region Operations
        public static readonly int BaseDryingRackCapacity = 20;
        public static readonly int CauldronBaseOutput = 10;
        public static readonly int CauldronOutputBonus = 1;
        //public static readonly int StackSizeMultiplier = 2;
        public static readonly int MixDryOutputSizeBonus = 1;
        public static readonly int ChemistStationSpeedBonus = 1;
        public static readonly float BasePlantQualityLevel = 0.5f;
        public static readonly float QualityBonusGrowTent = 0.16f;
        public static readonly float QualityBonusPlants = 0.15f;
        public static readonly float QualityBonusShrooms = 0.15f;
        public static readonly int BaseYieldPlants = 12;
        public static readonly int YieldBonusPlants = 1;
        public static readonly float GrowthSpeedBonusPlants = 0.025f;
        public static readonly float MoistureDrainBonus = 0.5f;

        public static int GetCauldronOutput()
        {
            return CauldronBaseOutput * (1 + SkillTreeData.MoreCauldronOutput.CurrentLevel * CauldronOutputBonus);
        }

        public static int GetDryingRackCapacity()
        {
            return BaseDryingRackCapacity * (1 + SkillTreeData.MoreMixAndDryingRackOutput.CurrentLevel * MixDryOutputSizeBonus);
        }

        public static int GetChemistStationSpeedMultiplier()
        {
            return 1 + SkillTreeData.ChemistStationQuick.CurrentLevel * ChemistStationSpeedBonus;
        }

        public static int GetMethCocaProductQualityBonus()
        {
            return SkillTreeData.MoreQualityMethCoca.CurrentLevel;
        }

        public static int GetMixDryOutputMultiplier()
        {
            return 1 + SkillTreeData.MoreMixAndDryingRackOutput.CurrentLevel * MixDryOutputSizeBonus;
        }

        public static float GetGrowthSpeedMultiplier()
        {
            return 1f + (SkillTreeData.GrowthSpeed.CurrentLevel + SkillTreeData.GrowthSpeed2.CurrentLevel) * GrowthSpeedBonusPlants;
        }

        public static float GetMoistureDrainMultiplier()
        {
            return SkillTreeData.WetAssPlants.CurrentLevel == 0 ? 1f : MoistureDrainBonus;
        }

        public static float GetPlantQualityBonus(string potName)
        {
            float potBonus = 0f;
            if (potName.Equals("Grow Tent"))
            {
                potBonus = SkillTreeData.Operations.CurrentLevel * QualityBonusGrowTent;
            }
            else if (potName.Equals("Plastic Pot") || potName.Equals("Moisture-Preserving Pot"))
            {
                potBonus = SkillTreeData.MoreQuality.CurrentLevel > 0 ? QualityBonusPlants : 0;
            }
            else if (potName.Equals("Air Pot"))
            {
                potBonus = SkillTreeData.MoreQuality.CurrentLevel * QualityBonusPlants;
                potBonus += SkillTreeData.MoreQuality.CurrentLevel == 2 ? 0.05f : 0f;
            }

            return potBonus;
        }

        public static float GetShroomQualityBonus()
        {
            return SkillTreeData.Mushroomancer.CurrentLevel * QualityBonusShrooms;
        }

        public static int GetPlantYieldBonus()
        {
            return SkillTreeData.MoreYield.CurrentLevel * YieldBonusPlants;
        }
        #endregion Operations

        #region Social
        public static readonly float BaseWeeklyDepositLimit = ATM.WEEKLY_DEPOSIT_LIMIT;
        public static readonly int TrashValueBonus = 1;
        public static readonly float PawnPriceBonus = 0.25f;
        public static readonly int BaseMaxCustomer = Dealer.MAX_CUSTOMERS;
        public static readonly float BaseDealerCut = 0.20f;
        public static readonly int BaseDeadDropItemLimit = Supplier.DEADDROP_ITEM_LIMIT;
        public static readonly float ATMDepositBonus = 2000f;
        public static readonly int CustomerLimitBonus = 2;
        public static readonly float CustomerSampleAcceptBonus = 0.05f;
        public static readonly float CustomerCashBonus = 0.10f;
        public static readonly float DealerCutReduction = 0.05f;
        public static readonly float DealerSpeedBonus = 1f;
        public static readonly float SupplierCashBonus = 0.675f;
        public static readonly float SupplierItemBonus = 0.50f;
        public static readonly float LaunderingBonus = 0.30f;

        public static float GetATMLimit()
        {
            return BaseWeeklyDepositLimit + SkillTreeData.MoreATMLimit.CurrentLevel * ATMDepositBonus;
        }

        public static int GetMaxCustomers()
        {
            return BaseMaxCustomer + SkillTreeData.DealerMoreCustomer.CurrentLevel * CustomerLimitBonus;
        }

        public static float GetCustomerSampleBonus()
        {
            return SkillTreeData.Social.CurrentLevel * CustomerSampleAcceptBonus;
        }

        public static float GetDealerCutReduction()
        {
            return SkillTreeData.DealerCutLess.CurrentLevel * DealerCutReduction;
        }

        public static float GetPawnPriceMultiplier()
        {
            return 1 + (SkillTreeData.SacarLaBasura.CurrentLevel * PawnPriceBonus);
        }

        public static int GetTrashValueBonus()
        {
            return SkillTreeData.SacarLaBasura.CurrentLevel * TrashValueBonus;
        }

        public static float GetSupplierCashMultiplier()
        {
            return 1f + SkillTreeData.BetterSupplier.CurrentLevel * SupplierCashBonus;
        }

        public static int GetSupplierItemLimit()
        {
            return (int)(BaseDeadDropItemLimit * (1f + SkillTreeData.BetterSupplier.CurrentLevel * SupplierItemBonus));
        }

        public static float GetLaunderingCapacityMultiplier()
        {
            return 1f + SkillTreeData.BusinessEvolving.CurrentLevel * LaunderingBonus;
        }

        public static float GetCustomerCashMultiplier()
        {
            return 1f + SkillTreeData.CityEvolving.CurrentLevel * CustomerCashBonus;
        }

        public static float GetDealerSpeedMultiplier()
        {
            return 1f + SkillTreeData.DealerSpeedUp.CurrentLevel * DealerSpeedBonus;
        }
        #endregion Social

        #region Special
        public static readonly float PoliceKilledBonus = 0.1f;
        public static readonly float CartelKilledBonus = 0.1f;
        public static readonly float BloodRushRegenDelayMultiplier = 0.2f;
        public static readonly float BloodRushHealthBonusMultiplier = 2f;
        public static readonly float BloodRushHealthBonusCap = 30f;
        public static readonly float BloodRushDuration = 60f;
        public static readonly float SiphonFundsBaseConversionRate = 0.1f;
        public static readonly float SiphonFundsOwnedBusinessBonus = 0.05f;
        public static readonly float BotanistActionSpeedBonus = 0.5f;
        public static readonly float EmployeeMoveSpeedBonus = 0.33f;
        public static readonly int EmployeeStationBonus = 2;
        public static readonly int MaxChemistStations = 4;
        public static readonly int MaxBotanistStations = 8;

        public static float GetBloodRushHealthBonus()
        {
            if (SkillTreeData.Heal.CurrentLevel == 0)
            {
                return 0f;
            }

            float policeBonus = (NPCPatches.PoliceKilled / 2) * PoliceKilledBonus;
            float cartelBonus = NPCPatches.CartelKilled * CartelKilledBonus;
            float healthCap = BloodRushHealthBonusCap * (BloodRush.IsBloodRushActive ? BloodRushHealthBonusMultiplier : 1f);
            float bonus = Mathf.Clamp(policeBonus + cartelBonus, 0f, healthCap);
            return bonus;
        }

        public static float GetSiphonFundsConversionMultiplier()
        {
            return SkillTreeData.GetCashDealer.CurrentLevel * (SiphonFundsBaseConversionRate + SiphonFundsOwnedBusinessBonus * BusinessManager.GetOwnedBusinesses().Count);
        }

        public static float GetEmployeeMoveSpeedScale()
        {
            return SkillTreeData.EmployeeMovespeed.CurrentLevel == 0 ? 1f : EmployeeMoveSpeedBonus;
        }

        public static float GetBotanistActionSpeedMultiplier()
        {
            return Mathf.Clamp(1f - SkillTreeData.BetterBotanists.CurrentLevel * BotanistActionSpeedBonus, 0.1f, 1f);
        }

        public static int GetEmployeeStationBonus()
        {
            return SkillTreeData.EmployeeMaxStation.CurrentLevel * EmployeeStationBonus;
        }

        public static (int, int) GetChemistStationBonus()
        {
            return (MaxChemistStations + GetEmployeeStationBonus(), MaxChemistStations);
        }

        public static (int, int) GetBotanistStationBonus()
        {
            return (MaxBotanistStations + GetEmployeeStationBonus(), MaxBotanistStations);
        }

        #endregion Special

    }
}