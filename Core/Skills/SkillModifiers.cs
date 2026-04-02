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
            return BaseHealth.GetValue(UseDefaultSkillParameters.GetValue()) + (SkillTreeData.Hardy.CurrentLevel * HealthBonus.GetValue(UseDefaultSkillParameters.GetValue())) + GetBloodRushHealthBonus();
        }

        public static float GetPlayerHealthRegen()
        {
            return BaseHealthRegen.GetValue(UseDefaultSkillParameters.GetValue()) * (1 + SkillTreeData.BattleScarred.CurrentLevel * HealthRegenBonus.GetValue(UseDefaultSkillParameters.GetValue()));
        }

        public static float GetPlayerHealthRegenDelay()
        {
            return BaseHealthRegenDelay.GetValue(UseDefaultSkillParameters.GetValue()) *
                (SkillTreeData.BattleScarred.CurrentLevel == 0 ? 1f : SkillTreeData.BattleScarred.CurrentLevel * HealthRegenDelayMultiplier.GetValue(UseDefaultSkillParameters.GetValue())) *
                (BloodRush.IsBloodRushActive ? BloodRushRegenDelayMultiplier.GetValue(UseDefaultSkillParameters.GetValue()) : 1f);
        }

        public static float GetPlayerMaxStamina()
        {
            return BaseStamina.GetValue(UseDefaultSkillParameters.GetValue()) * (1 + SkillTreeData.SpringHeeled.CurrentLevel * StaminaBonus.GetValue(UseDefaultSkillParameters.GetValue()));
        }

        public static float GetFleetFeetMoveSpeedMultiplier()
        {
            return 1 + SkillTreeData.FleetFeet.CurrentLevel * MoveSpeedBonus.GetValue(UseDefaultSkillParameters.GetValue());
        }

        public static float GetPlayerJumpHeight()
        {
            return BaseJumpHeight.GetValue(UseDefaultSkillParameters.GetValue()) *
                (1 + SkillTreeData.SpringHeeled.CurrentLevel * JumpHeightBonus.GetValue(UseDefaultSkillParameters.GetValue())) *
                (AdrenalineSurge.IsAdrenalineSurgeActive ? AdrenalineSurgeJumpMultiplier.GetValue(UseDefaultSkillParameters.GetValue()) : 1f);
        }

        public static float GetXPGainMultiplier()
        {
            return 1f + (SkillTreeData.FastLearner.CurrentLevel * XPGainBonus.GetValue(UseDefaultSkillParameters.GetValue())) + (SkillTreeData.TurboNerdo.CurrentLevel * XPGainBonus2.GetValue(UseDefaultSkillParameters.GetValue()));
        }

        public static float GetSaleXPBonus()
        {
            return SkillTreeData.Kingpin.CurrentLevel * SaleXPBonus.GetValue(UseDefaultSkillParameters.GetValue());
        }

        public static float GetVisbilityMultiplier()
        {
            return SkillTreeData.Ghost.CurrentLevel == 0 ? 1f : VisibilityMultiplier.GetValue(UseDefaultSkillParameters.GetValue());
        }

        public static float GetPickpocketDifficultyMultiplier()
        {
            return SkillTreeData.Ghost.CurrentLevel == 0 ? 1f : PickpocketDifficultyMultiplier.GetValue(UseDefaultSkillParameters.GetValue());
        }

        public static float GetPickpocketMinimumWidth(float original)
        {
            return SkillTreeData.Ghost.CurrentLevel == 0 ? original : PickpocketMinimumSuccessWidth.GetValue(UseDefaultSkillParameters.GetValue());
        }

        public static int GetInventoryStackSizeMultiplier()
        {
            return 1 + SkillTreeData.PrisonWallet.CurrentLevel * InventoryStackSizeBonus.GetValue(UseDefaultSkillParameters.GetValue());
        }

        public static float GetArrestTime()
        {
            return BaseArrestTime.GetValue(UseDefaultSkillParameters.GetValue()) * (1 + SkillTreeData.Slippery.CurrentLevel * ArrestTimeBonus.GetValue(UseDefaultSkillParameters.GetValue()));
        }

        public static float GetArrestRadius()
        {
            return SkillTreeData.Slippery.CurrentLevel == 0 ? BaseArrestRadius.GetValue(UseDefaultSkillParameters.GetValue()) : BaseArrestRadius.GetValue(UseDefaultSkillParameters.GetValue()) * ArrestRadiusBonus.GetValue(UseDefaultSkillParameters.GetValue());
        }

        public static float GetPoliceSearchTime(float searchTime)
        {
            return SkillTreeData.PileTheBodiesHigh.CurrentLevel == 0 ? searchTime : Mathf.Clamp(searchTime, 0f, 35f) - Mathf.Clamp(KillCounts.PoliceKilled / 20, 0, 10);
        }

        #endregion Stats

        #region Operations
        public static int GetCauldronOutput()
        {
            return BaseCauldronOutput.GetValue(UseDefaultSkillParameters.GetValue()) * (1 + SkillTreeData.WitchsBrew.CurrentLevel * CauldronOutputBonus.GetValue(UseDefaultSkillParameters.GetValue()));
        }

        public static int GetDryingRackCapacity()
        {
            return BaseDryingRackCapacity.GetValue(UseDefaultSkillParameters.GetValue()) * (1 + SkillTreeData.CrankinOneOut.CurrentLevel * MixDryOutputSizeBonus.GetValue(UseDefaultSkillParameters.GetValue()));
        }

        public static int GetChemistStationSpeedMultiplier()
        {
            return 1 + SkillTreeData.QuickCrafter.CurrentLevel * ChemistStationSpeedBonus.GetValue(UseDefaultSkillParameters.GetValue());
        }

        public static int GetMethCocaProductQualityBonus()
        {
            return SkillTreeData.HarderAndStronger.CurrentLevel;
        }

        public static int GetMixDryOutputMultiplier()
        {
            return 1 + SkillTreeData.CrankinOneOut.CurrentLevel * MixDryOutputSizeBonus.GetValue(UseDefaultSkillParameters.GetValue());
        }

        public static float GetGrowthSpeedMultiplier()
        {
            return 1f + (SkillTreeData.GreenThumb.CurrentLevel + SkillTreeData.PlantWhisperer.CurrentLevel) * GrowthSpeedBonusPlants.GetValue(UseDefaultSkillParameters.GetValue());
        }

        public static float GetMoistureDrainMultiplier()
        {
            return SkillTreeData.WetAssPlants.CurrentLevel == 0 ? 1f : MoistureDrainBonus.GetValue(UseDefaultSkillParameters.GetValue());
        }

        public static float GetPlantQualityBonus(string potName)
        {
            float potBonus = 0f;
            if (potName.Equals("Grow Tent"))
            {
                potBonus = SkillTreeData.PitchinATent.CurrentLevel * QualityBonusGrowTent.GetValue(UseDefaultSkillParameters.GetValue());
            }
            else if (potName.Equals("Plastic Pot") || potName.Equals("Moisture-Preserving Pot"))
            {
                potBonus = SkillTreeData.AdvancedPotTechniques.CurrentLevel > 0 ? QualityBonusPlants.GetValue(UseDefaultSkillParameters.GetValue()) : 0;
            }
            else if (potName.Equals("Air Pot"))
            {
                potBonus = SkillTreeData.AdvancedPotTechniques.CurrentLevel * QualityBonusPlants.GetValue(UseDefaultSkillParameters.GetValue());
                potBonus += SkillTreeData.AdvancedPotTechniques.CurrentLevel == 2 ? 0.05f : 0f;
            }

            return potBonus;
        }

        public static float GetShroomQualityBonus()
        {
            return SkillTreeData.Mushroomancer.CurrentLevel * QualityBonusShrooms.GetValue(UseDefaultSkillParameters.GetValue());
        }

        public static int GetPlantYieldBonus()
        {
            return SkillTreeData.BountifulHarvest.CurrentLevel * YieldBonusPlants.GetValue(UseDefaultSkillParameters.GetValue());
        }
        #endregion Operations

        #region Social
        public static float GetATMLimit()
        {
            return BaseWeeklyDepositLimit.GetValue(UseDefaultSkillParameters.GetValue()) + SkillTreeData.HoardTheWealth.CurrentLevel * ATMDepositBonus.GetValue(UseDefaultSkillParameters.GetValue());
        }


        public static float GetCustomerSampleBonus()
        {
            return SkillTreeData.SilverTonguedDevil.CurrentLevel * CustomerSampleAcceptBonus.GetValue(UseDefaultSkillParameters.GetValue());
        }

        public static int GetGrabberBinSize()
        {
            return Mathf.RoundToInt(BaseTrashGrabberBinSize.GetValue(UseDefaultSkillParameters.GetValue()) * (1 + SkillTreeData.CommunityService.CurrentLevel * TrashGrabberBinSizeBonus.GetValue(UseDefaultSkillParameters.GetValue())));
        }

        public static float GetGrabberPickupRadius()
        {
            return TrashPickupRadius.GetValue(UseDefaultSkillParameters.GetValue()) * GetGrabberPickupRadiusMultiplier();
        }

        public static float GetGrabberPickupRadiusMultiplier()
        {
            return (1 + SkillTreeData.CommunityService.CurrentLevel * TrashPickupRadiusBonus.GetValue(UseDefaultSkillParameters.GetValue()));
        }

        public static float GetPawnPriceMultiplier()
        {
            return 1 + (SkillTreeData.SacarLaBasura.CurrentLevel * PawnPriceBonus.GetValue(UseDefaultSkillParameters.GetValue()));
        }

        public static int GetTrashValueBonus()
        {
            return SkillTreeData.SacarLaBasura.CurrentLevel * TrashValueBonus.GetValue(UseDefaultSkillParameters.GetValue());
        }


        public static float GetLaunderingCapacityMultiplier()
        {
            return 1f + SkillTreeData.SqueakyClean.CurrentLevel * LaunderingBonus.GetValue(UseDefaultSkillParameters.GetValue());
        }

        public static float GetCustomerCashMultiplier()
        {
            return 1f + SkillTreeData.SpreadTheWealth.CurrentLevel * CustomerCashBonus.GetValue(UseDefaultSkillParameters.GetValue());
        }

        #endregion Social

        #region Logistician
        public static int GetMaxCustomers()
        {
            return BaseMaxCustomer.GetValue(UseDefaultSkillParameters.GetValue()) + SkillTreeData.ExpansiveEmpire.CurrentLevel * DealerCustomerLimitBonus.GetValue(UseDefaultSkillParameters.GetValue());
        }

        public static float GetDealerCutReduction()
        {
            return SkillTreeData.WageGarnishment.CurrentLevel * DealerCutReduction.GetValue(UseDefaultSkillParameters.GetValue());
        }

        public static float GetSupplierCashMultiplier()
        {
            return 1f + SkillTreeData.ReliableBusinessPartner.CurrentLevel * SupplierCashBonus.GetValue(UseDefaultSkillParameters.GetValue());
        }

        public static int GetCustomerOrderLimitBonus()
        {
            if (SkillTreeData.CaptiveMarket.CurrentLevel == 0) return 0;

            return SkillTreeData.CaptiveMarket.CurrentLevel * CustomerOrderLimitBonus.GetValue(UseDefaultSkillParameters.GetValue()) + ((int)S1API.Leveling.LevelManager.Rank + 1) * CustomerOrderLimitRankBonus.GetValue(UseDefaultSkillParameters.GetValue());
        }

        public static int GetSupplierItemLimit()
        {
            return (int)(BaseDeadDropItemLimit.GetValue(UseDefaultSkillParameters.GetValue()) * (1f + SkillTreeData.ReliableBusinessPartner.CurrentLevel * SupplierItemBonus.GetValue(UseDefaultSkillParameters.GetValue())));
        }

        public static float GetDealerSpeedMultiplier()
        {
            return 1f + SkillTreeData.MotivationalLeader.CurrentLevel * DealerSpeedBonus.GetValue(UseDefaultSkillParameters.GetValue());
        }

        public static float GetEmployeeMoveSpeedScale()
        {
            return SkillTreeData.EmployeeMovespeed.CurrentLevel == 0 ? 1f : Mathf.Clamp(EmployeeMoveSpeedBonus.GetValue(UseDefaultSkillParameters.GetValue()), 0.1f, 10f);
        }

        public static float GetBotanistActionDurationMultiplier()
        {
            return SkillTreeData.FastFarmers.CurrentLevel == 0 ? 1f : Mathf.Clamp(BotanistActionDurationMultiplier.GetValue(UseDefaultSkillParameters.GetValue()), 0.1f, 10f);
        }

        public static float GetHandlerPackagingSpeedMultiplier()
        {
            return SkillTreeData.FastHandlers.CurrentLevel == 0 ? 1f : Mathf.Clamp(HandlerPackagingSpeedMultiplier.GetValue(UseDefaultSkillParameters.GetValue()), 0.1f, 10f);
        }

        public static float GetChemistActionDurationMultiplier()
        {
            return SkillTreeData.FastChemists.CurrentLevel == 0 ? 1f : Mathf.Clamp(ChemistActionDurationMultiplier.GetValue(UseDefaultSkillParameters.GetValue()), 0.1f, 10f);
        }

        public static int GetEmployeeStationBonus()
        {
            return SkillTreeData.EmployeeMaxStation.CurrentLevel * EmployeeStationBonus.GetValue(UseDefaultSkillParameters.GetValue());
        }

        public static (int, int) GetChemistStationBonus()
        {
            return (BaseMaxChemistStations.GetValue(UseDefaultSkillParameters.GetValue()) + GetEmployeeStationBonus(), BaseMaxChemistStations.GetValue(UseDefaultSkillParameters.GetValue()));
        }

        public static (int, int) GetBotanistStationBonus()
        {
            return (BaseMaxBotanistStations.GetValue(UseDefaultSkillParameters.GetValue()) + GetEmployeeStationBonus(), BaseMaxBotanistStations.GetValue(UseDefaultSkillParameters.GetValue()));
        }
        #endregion Logistician


        #region Special
        public static float GetBloodRushHealthBonus()
        {
            if (SkillTreeData.BloodRush.CurrentLevel == 0)
            {
                return 0f;
            }

            float policeBonus = (KillCounts.PoliceKilled / 2) * PoliceKilledBonus.GetValue(UseDefaultSkillParameters.GetValue());
            float cartelBonus = KillCounts.CartelKilled * CartelKilledBonus.GetValue(UseDefaultSkillParameters.GetValue());
            float healthCap = BloodRushHealthBonusCap.GetValue(UseDefaultSkillParameters.GetValue()) * (BloodRush.IsBloodRushActive ? BloodRushHealthBonusMultiplier.GetValue(UseDefaultSkillParameters.GetValue()) : 1f);
            float bonus = Mathf.Clamp(policeBonus + cartelBonus, 0f, healthCap);
            return bonus;
        }

        public static float GetSiphonFundsConversionMultiplier()
        {
            return SkillTreeData.SiphonFunds.CurrentLevel * (SiphonFundsBaseConversionRate.GetValue(UseDefaultSkillParameters.GetValue()) + SiphonFundsOwnedBusinessBonus.GetValue(UseDefaultSkillParameters.GetValue()) * BusinessManager.GetOwnedBusinesses().Count);
        }

        #endregion Special
    }
}