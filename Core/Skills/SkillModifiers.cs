using MelonLoader;
using S1API.Property;
using SkillTree.Core.Effects;
using SkillTree.Core.Serialization;
using UnityEngine;
using static SkillTree.Core.Utilities.ConfigManager;

namespace SkillTree.Core.Skills
{
    public static class SkillModifiers
    {
        #region Stats
        public static float GetPlayerMaxHealth()
        {
            return BaseHealth.GetValue() + (SkillTreeData.Enforcer.CurrentLevel * HealthBonus.GetValue()) + GetBloodRushHealthBonus();
        }

        public static float GetPlayerHealthRegen()
        {
            return BaseHealthRegen.GetValue() * (1 + SkillTreeData.BattleScarred.CurrentLevel * HealthRegenBonus.GetValue());
        }

        public static float GetPlayerHealthRegenDelay()
        {
            float battleScarred = SkillTreeData.BattleScarred.CurrentLevel == 0 ? 1f : SkillTreeData.BattleScarred.CurrentLevel * HealthRegenDelayMultiplier.GetValue();
            float bloodRush = BloodRush.IsBloodRushActive ? BloodRushRegenDelayMultiplier.GetValue() : 1f;
            float delay = BaseHealthRegenDelay.GetValue() * battleScarred * bloodRush;
            return delay;
        }

        public static float GetPlayerMaxStamina()
        {
            return BaseStamina.GetValue() * (1 + SkillTreeData.SpringHeeled.CurrentLevel * StaminaBonus.GetValue());
        }

        public static float GetPlayerMoveSpeedMultiplier()
        {
            return 1 + SkillTreeData.MoreMovespeed.CurrentLevel * MoveSpeedBonus.GetValue();
        }

        public static float GetPlayerJumpHeight()
        {
            return BaseJumpHeight.GetValue() * (1 + SkillTreeData.SpringHeeled.CurrentLevel * JumpHeightBonus.GetValue());
        }

        public static float GetXPGainMultiplier()
        {
            return 1f + (SkillTreeData.MoreXP.CurrentLevel * XPGainBonus.GetValue()) + (SkillTreeData.MoreXP2.CurrentLevel * XPGainBonus2.GetValue());
        }

        public static float GetSaleXPBonus()
        {
            return SkillTreeData.MoreXPWhenEarnMoney.CurrentLevel * SaleXPBonus.GetValue();
        }

        public static float GetVisbilityMultiplier()
        {
            return SkillTreeData.Ghost.CurrentLevel == 0 ? 1f : VisibilityMultiplier.GetValue();
        }

        public static float GetPickpocketDifficultyMultiplier()
        {
            return SkillTreeData.Ghost.CurrentLevel == 0 ? 1f : PickpocketDifficultyMultiplier.GetValue();
        }

        public static float GetPickpocketMinimumWidth(float original)
        {
            return SkillTreeData.Ghost.CurrentLevel == 0 ? original : PickpocketMinimumSuccessWidth.GetValue();
        }

        public static int GetInventoryStackSizeMultiplier()
        {
            return 1 + SkillTreeData.MoreStackItem.CurrentLevel * InventoryStackSizeBonus.GetValue();
        }

        public static float GetArrestTime()
        {
            return BaseArrestTime.GetValue() * (1 + SkillTreeData.Slippery.CurrentLevel * ArrestTimeBonus.GetValue());
        }

        public static float GetArrestRadius()
        {
            return SkillTreeData.Slippery.CurrentLevel == 0 ? BaseArrestRadius.GetValue() : BaseArrestRadius.GetValue() * ArrestRadiusBonus.GetValue();
        }

        #endregion Stats

        #region Operations
        public static int GetCauldronOutput()
        {
            return BaseCauldronOutput.GetValue() * (1 + SkillTreeData.MoreCauldronOutput.CurrentLevel * CauldronOutputBonus.GetValue());
        }

        public static int GetDryingRackCapacity()
        {
            return BaseDryingRackCapacity.GetValue() * (1 + SkillTreeData.MoreMixAndDryingRackOutput.CurrentLevel * MixDryOutputSizeBonus.GetValue());
        }

        public static int GetChemistStationSpeedMultiplier()
        {
            return 1 + SkillTreeData.ChemistStationQuick.CurrentLevel * ChemistStationSpeedBonus.GetValue();
        }

        public static int GetMethCocaProductQualityBonus()
        {
            return SkillTreeData.MoreQualityMethCoca.CurrentLevel;
        }

        public static int GetMixDryOutputMultiplier()
        {
            return 1 + SkillTreeData.MoreMixAndDryingRackOutput.CurrentLevel * MixDryOutputSizeBonus.GetValue();
        }

        public static float GetGrowthSpeedMultiplier()
        {
            return 1f + (SkillTreeData.GrowthSpeed.CurrentLevel + SkillTreeData.GrowthSpeed2.CurrentLevel) * GrowthSpeedBonusPlants.GetValue();
        }

        public static float GetMoistureDrainMultiplier()
        {
            return SkillTreeData.WetAssPlants.CurrentLevel == 0 ? 1f : MoistureDrainBonus.GetValue();
        }

        public static float GetPlantQualityBonus(string potName)
        {
            float potBonus = 0f;
            if (potName.Equals("Grow Tent"))
            {
                potBonus = SkillTreeData.Supplier.CurrentLevel * QualityBonusGrowTent.GetValue();
            }
            else if (potName.Equals("Plastic Pot") || potName.Equals("Moisture-Preserving Pot"))
            {
                potBonus = SkillTreeData.MoreQuality.CurrentLevel > 0 ? QualityBonusPlants.GetValue() : 0;
            }
            else if (potName.Equals("Air Pot"))
            {
                potBonus = SkillTreeData.MoreQuality.CurrentLevel * QualityBonusPlants.GetValue();
                potBonus += SkillTreeData.MoreQuality.CurrentLevel == 2 ? 0.05f : 0f;
            }

            return potBonus;
        }

        public static float GetShroomQualityBonus()
        {
            return SkillTreeData.Mushroomancer.CurrentLevel * QualityBonusShrooms.GetValue();
        }

        public static int GetPlantYieldBonus()
        {
            return SkillTreeData.MoreYield.CurrentLevel * YieldBonusPlants.GetValue();
        }
        #endregion Operations

        #region Social
        public static float GetATMLimit()
        {
            return BaseWeeklyDepositLimit.GetValue() + SkillTreeData.HoardTheWealth.CurrentLevel * ATMDepositBonus.GetValue();
        }


        public static float GetCustomerSampleBonus()
        {
            return SkillTreeData.Hustler.CurrentLevel * CustomerSampleAcceptBonus.GetValue();
        }

        public static int GetGrabberBinSize()
        {
            return Mathf.RoundToInt(BaseTrashGrabberBinSize.GetValue() * (1 + SkillTreeData.CommunityService.CurrentLevel * TrashGrabberBinSizeBonus.GetValue()));
        }

        public static float GetGrabberPickupRadius()
        {
            return TrashPickupRadius.GetValue() * GetGrabberPickupRadiusMultiplier();
        }

        public static float GetGrabberPickupRadiusMultiplier()
        {
            return (1 + SkillTreeData.CommunityService.CurrentLevel * TrashPickupRadiusBonus.GetValue());
        }

        public static float GetPawnPriceMultiplier()
        {
            return 1 + (SkillTreeData.SacarLaBasura.CurrentLevel * PawnPriceBonus.GetValue());
        }

        public static int GetTrashValueBonus()
        {
            return SkillTreeData.SacarLaBasura.CurrentLevel * TrashValueBonus.GetValue();
        }


        public static float GetLaunderingCapacityMultiplier()
        {
            return 1f + SkillTreeData.SqueakyClean.CurrentLevel * LaunderingBonus.GetValue();
        }

        public static float GetCustomerCashMultiplier()
        {
            return 1f + SkillTreeData.SpreadTheWealth.CurrentLevel * CustomerCashBonus.GetValue();
        }

        #endregion Social

        #region Logistician
        public static int GetMaxCustomers()
        {
            return BaseMaxCustomer.GetValue() + SkillTreeData.ExpansiveEmpire.CurrentLevel * DealerCustomerLimitBonus.GetValue();
        }

        public static float GetDealerCutReduction()
        {
            return SkillTreeData.WageGarnishment.CurrentLevel * DealerCutReduction.GetValue();
        }

        public static float GetSupplierCashMultiplier()
        {
            return 1f + SkillTreeData.Logistician.CurrentLevel * SupplierCashBonus.GetValue();
        }

        public static int GetCustomerOrderLimitBonus()
        {
            if (SkillTreeData.CaptiveMarket.CurrentLevel == 0) return 0;

            return SkillTreeData.CaptiveMarket.CurrentLevel * CustomerOrderLimitBonus.GetValue() + ((int)S1API.Leveling.LevelManager.Rank + 1) * CustomerOrderLimitRankBonus.GetValue();
        }

        public static int GetSupplierItemLimit()
        {
            return (int)(BaseDeadDropItemLimit.GetValue() * (1f + SkillTreeData.Logistician.CurrentLevel * SupplierItemBonus.GetValue()));
        }

        public static float GetDealerSpeedMultiplier()
        {
            return 1f + SkillTreeData.MotivationalLeader.CurrentLevel * DealerSpeedBonus.GetValue();
        }

        public static float GetEmployeeMoveSpeedScale()
        {
            return SkillTreeData.EmployeeMovespeed.CurrentLevel == 0 ? 1f : Mathf.Clamp(EmployeeMoveSpeedBonus.GetValue(), 0.1f, 10f);
        }

        public static float GetBotanistActionSpeedMultiplier()
        {
            return Mathf.Clamp(1f - SkillTreeData.BetterBotanists.CurrentLevel * BotanistActionSpeedBonus.GetValue(), 0.1f, 10f);
        }

        public static int GetEmployeeStationBonus()
        {
            return SkillTreeData.EmployeeMaxStation.CurrentLevel * EmployeeStationBonus.GetValue();
        }

        public static (int, int) GetChemistStationBonus()
        {
            return (BaseMaxChemistStations.GetValue() + GetEmployeeStationBonus(), BaseMaxChemistStations.GetValue());
        }

        public static (int, int) GetBotanistStationBonus()
        {
            return (BaseMaxBotanistStations.GetValue() + GetEmployeeStationBonus(), BaseMaxBotanistStations.GetValue());
        }
        #endregion Logistician


        #region Special
        public static float GetBloodRushHealthBonus()
        {
            if (SkillTreeData.Heal.CurrentLevel == 0)
            {
                return 0f;
            }

            float policeBonus = (KillCounts.PoliceKilled / 2) * PoliceKilledBonus.GetValue();
            float cartelBonus = KillCounts.CartelKilled * CartelKilledBonus.GetValue();
            float healthCap = BloodRushHealthBonusCap.GetValue() * (BloodRush.IsBloodRushActive ? BloodRushHealthBonusMultiplier.GetValue() : 1f);
            float bonus = Mathf.Clamp(policeBonus + cartelBonus, 0f, healthCap);
            return bonus;
        }

        public static float GetSiphonFundsConversionMultiplier()
        {
            return SkillTreeData.GetCashDealer.CurrentLevel * (SiphonFundsBaseConversionRate.GetValue() + SiphonFundsOwnedBusinessBonus.GetValue() * BusinessManager.GetOwnedBusinesses().Count);
        }

        #endregion Special
    }
}