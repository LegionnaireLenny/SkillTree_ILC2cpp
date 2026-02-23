using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.Money;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.PlayerScripts.Health;
using System;
using UnityEngine;

namespace SkillTree.Core
{
    public static class SkillModifiers
    {
        #region Stats
        public static readonly float PlayerBaseHealth = PlayerHealth.MAX_HEALTH;
        public static readonly float HealthBonus = 20f;
        public static readonly float PlayerBaseHealthRegen = 0.5f;
        public static readonly int HealthRegenBonus = 1;
        public static readonly float PlayerBaseHealthRegenDelay = 30f;
        public static readonly float PlayerBaseStamina = PlayerMovement.StaminaReserveMax;
        public static readonly float StaminaBonus = 0.30f;
        public static readonly float PlayerBaseMoveSpeed = 1f;
        //public static readonly float PlayerBaseMoveSpeed = PlayerMovement.StaticMoveSpeedMultiplier;
        public static readonly float PlayerBaseJumpHeight = PlayerMovement.JumpMultiplier;
        public static readonly float MoveSpeedBonus = 0.15f;
        public static readonly float JumpHeightBonus = 0.35f;
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
            return PlayerBaseHealth + (Core.SkillData.Stats * HealthBonus);
        }

        public static float GetPlayerHealthRegen()
        {
            return PlayerBaseHealthRegen * (1 + (Core.SkillData.BattleScarred * HealthRegenBonus));
        }

        public static float GetPlayerHealthRegenDelay()
        {
            return PlayerBaseHealthRegenDelay / (Math.Abs(Core.SkillData.BattleScarred) + 1);
        }

        public static float GetPlayerMaxStamina()
        {
            return PlayerBaseStamina * (1 + (Core.SkillData.SpringHeeled * StaminaBonus));
        }

        public static float GetPlayerMoveSpeed()
        {
            return PlayerBaseMoveSpeed * (1 + (Core.SkillData.MoreMovespeed * MoveSpeedBonus));
        }

        public static float GetPlayerJumpHeight()
        {
            return PlayerBaseJumpHeight * (1 + (Core.SkillData.SpringHeeled * JumpHeightBonus));
        }

        public static float GetXPGainMultiplier()
        {
            return 1f + ((Core.SkillData.MoreXP + Core.SkillData.MoreXP2) * XPGainBonus);
        }

        public static float GetSaleXPBonus()
        {
            return Core.SkillData.MoreXPWhenEarnMoney * SaleXPBonus;
        }

        public static int GetInventoryStackSizeMultiplier()
        {
            return 1 + (Core.SkillData.MoreStackItem * InventoryStackSizeBonus);
        }

        public static float GetArrestTime()
        {
            return BaseArrestTime * (1 + (Core.SkillData.Slippery * ArrestTimeIncreaseBonus));
        }

        public static float GetArrestRadius()
        {
            return BaseArrestRadius * (1 - (Core.SkillData.Slippery * ArrestRadiusReductionBonus));
        }

        #endregion Stats

        #region Operations
        public static readonly int BaseDryingRackCapacity = 20;
        public static readonly int CauldronBaseOutput = 10;
        public static readonly int CauldronOutputMultiplier = 2;
        public static readonly int StackSizeMultiplier = 2;
        public static readonly int MixDryOutputSizeMultiplier = 2;
        public static readonly int ChemistStationSpeedMultiplier = 2;
        public static readonly float BasePlantQualityLevel = 0.5f;
        public static readonly float QualityBonusGrowTent = 0.16f;
        public static readonly float QualityBonusPlants = 0.15f;
        public static readonly float QualityBonusShrooms = 0.15f;
        public static readonly int BaseYieldPlants = 12;
        public static readonly int YieldBonusPlants = 1;
        public static readonly float GrowthSpeedBonusPlants = 0.025f;

        public static int GetCauldronOutputBonus()
        {
            if (Core.SkillData.MoreCauldronOutput == 0)
                return CauldronBaseOutput;
            else
                return CauldronBaseOutput * (Core.SkillData.MoreCauldronOutput * CauldronOutputMultiplier);
        }

        public static int GetChemistStationSpeedMultiplier()
        {
            return Core.SkillData.ChemistStationQuick * ChemistStationSpeedMultiplier;
        }

        public static int GetMethCocaProductQualityBonus()
        {
            return Core.SkillData.MoreQualityMethCoca;
        }

        public static int GetMixDryOutputMultiplier()
        {
            return Core.SkillData.MoreMixAndDryingRackOutput * MixDryOutputSizeMultiplier;
        }

        public static float GetGrowthSpeedMultiplier()
        {
            return 1f + ((Core.SkillData.GrowthSpeed + Core.SkillData.GrowthSpeed2) * GrowthSpeedBonusPlants);
        }

        public static float GetGrowTentQualityBonus()
        {
            return Core.SkillData.Operations * QualityBonusGrowTent;
        }

        public static float GetPlantQualityBonus(int maxSkillBonus = 2)
        {
            return Math.Clamp(Core.SkillData.MoreQuality, 0, maxSkillBonus) * QualityBonusPlants;
        }

        public static float GetShroomQualityBonus()
        {
            return Core.SkillData.MoreQuality * QualityBonusShrooms;
        }

        public static int GetPlantYieldBonus()
        {
            return Core.SkillData.MoreYield * YieldBonusPlants;
        }
        #endregion Operations

        #region Social
        public static readonly float BaseWeeklyDepositLimit = ATM.WEEKLY_DEPOSIT_LIMIT;
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
        public static readonly float LaunderingBonus = 0.20f;

        public static float GetATMLimit()
        {
            return BaseWeeklyDepositLimit + (Core.SkillData.MoreATMLimit * ATMDepositBonus);
        }

        public static int GetMaxCustomers()
        {
            return BaseMaxCustomer + (Core.SkillData.DealerMoreCustomer * CustomerLimitBonus);
        }

        public static float GetCustomerSampleBonus()
        {
            return Core.SkillData.Social * CustomerSampleAcceptBonus;
        }

        public static float GetDealerCutReduction()
        {
            return Core.SkillData.DealerCutLess * DealerCutReduction;
        }

        //public static float GetDealerCut()
        //{
        //    return BaseDealerCut - Core.SkillData.DealerCutLess * DealerCutReduction;
        //}

        public static float GetSupplierCashMultiplier()
        {
            return 1f + (Core.SkillData.BetterSupplier * SupplierCashBonus);
        }

        public static int GetSupplierItemLimit()
        {
            return (int)(BaseDeadDropItemLimit * (1f + (Core.SkillData.BetterSupplier * SupplierItemBonus)));
        }

        public static float GetLaunderingCapacityMultiplier()
        {
            return 1f + (Core.SkillData.BusinessEvolving * LaunderingBonus);
        }

        public static float GetCustomerCashMultiplier()
        {
            return 1f + (Core.SkillData.CityEvolving * CustomerCashBonus);
        }

        public static float GetDealerSpeedMultiplier()
        {
            return 1f + (Core.SkillData.DealerSpeedUp * DealerSpeedBonus);
        }
        #endregion Social

        #region Special
        public static readonly float BotanistActionSpeedBonus = 0.5f;
        public static readonly float EmployeeMoveSpeedBonus = 0.33f;
        public static readonly int EmployeeStationBonus = 2;
        public static readonly int MaxChemistStations = 4;
        public static readonly int MaxBotanistStations = 8;

        public static float GetEmployeeMoveSpeedScale()
        {
            return Core.SkillData.EmployeeMovespeed == 0 ? 1f : EmployeeMoveSpeedBonus;
        }

        public static float GetBotanistActionSpeedMultiplier()
        {
            return Mathf.Clamp(1f - (Core.SkillData.BetterBotanists * BotanistActionSpeedBonus), 0.1f, 1f);
        }

        public static int GetEmployeeStationBonus()
        {
            return Core.SkillData.EmployeeMaxStation * EmployeeStationBonus;
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