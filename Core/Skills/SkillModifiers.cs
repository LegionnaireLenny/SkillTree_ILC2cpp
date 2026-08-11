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
            return BaseHealth.GetValue(UseDefault.GetValue()) + (SkillTreeData.Hardy.CurrentLevel * HealthBonus.GetValue(UseDefault.GetValue())) + GetBloodRushHealthBonus();
        }

        public static float GetPlayerHealthRegen()
        {
            return BaseHealthRegen.GetValue(UseDefault.GetValue()) * (1 + SkillTreeData.BattleScarred.CurrentLevel * HealthRegenBonus.GetValue(UseDefault.GetValue()));
        }

        public static float GetPlayerHealthRegenDelay()
        {
            return BaseHealthRegenDelay.GetValue(UseDefault.GetValue()) *
                (SkillTreeData.BattleScarred.CurrentLevel == 0 ? 1f : SkillTreeData.BattleScarred.CurrentLevel * HealthRegenDelayMultiplier.GetValue(UseDefault.GetValue())) *
                (BloodRush.IsBloodRushActive ? BloodRushRegenDelayMultiplier.GetValue(UseDefault.GetValue()) : 1f);
        }

        public static float GetPlayerMaxStamina()
        {
            return BaseStamina.GetValue(UseDefault.GetValue()) * (1 + SkillTreeData.SpringHeeled.CurrentLevel * StaminaBonus.GetValue(UseDefault.GetValue()));
        }

        public static float GetFleetFeetMoveSpeedMultiplier()
        {
            return 1 + SkillTreeData.FleetFeet.CurrentLevel * MoveSpeedBonus.GetValue(UseDefault.GetValue());
        }

        public static float GetPlayerJumpHeight()
        {
            return BaseJumpHeight.GetValue(UseDefault.GetValue()) *
                (1 + SkillTreeData.SpringHeeled.CurrentLevel * JumpHeightBonus.GetValue(UseDefault.GetValue())) *
                (AdrenalineSurge.IsAdrenalineSurgeActive ? AdrenalineSurgeJumpMultiplier.GetValue(UseDefault.GetValue()) : 1f);
        }

        public static float GetXPGainMultiplier()
        {
            float enforcerBonus = SkillTreeData.SchoolOfHardKnocks.CurrentLevel * SchoolOfHardKnocksXPBonus.GetValue(UseDefault.GetValue());
            float provisionerBonus = SkillTreeData.Meister.CurrentLevel * MeisterXPBonus.GetValue(UseDefault.GetValue());
            float hustlerBonus = SkillTreeData.MultiLevelMarketeer.CurrentLevel * MultiLevelMarketeerXPBonus.GetValue(UseDefault.GetValue());
            float logisticianBonus = SkillTreeData.EducatedWorkforce.CurrentLevel * EducatedWorkforceBonus.GetValue(UseDefault.GetValue());
            return 1f + enforcerBonus + provisionerBonus + hustlerBonus + logisticianBonus;
        }


        public static float GetVisbilityMultiplier()
        {
            return SkillTreeData.Ghost.CurrentLevel == 0 ? 1f : VisibilityMultiplier.GetValue(UseDefault.GetValue());
        }

        public static float GetPickpocketDifficultyMultiplier()
        {
            return SkillTreeData.Ghost.CurrentLevel == 0 ? 1f : PickpocketDifficultyMultiplier.GetValue(UseDefault.GetValue());
        }

        public static float GetPickpocketMinimumWidth(float original)
        {
            return SkillTreeData.Ghost.CurrentLevel == 0 ? original : PickpocketMinimumSuccessWidth.GetValue(UseDefault.GetValue());
        }

        public static int GetInventoryStackSizeMultiplier()
        {
            return 1 + (SkillTreeData.PrisonWallet.CurrentLevel + SkillTreeData.QuantumStockpile.CurrentLevel) * InventoryStackSizeBonus.GetValue(UseDefault.GetValue()) + ;
        }

        public static float GetAimTimeMultiplier()
        {
            return SkillTreeData.QuickDraw.CurrentLevel == 0 ? 1f : AimTimeMultiplier.GetValue(UseDefault.GetValue());
        }

        public static float GetMaxSpreadMultiplier()
        {
            return SkillTreeData.Sharpshooter.CurrentLevel == 0 ? 1f : MaxSpreadMultiplier.GetValue(UseDefault.GetValue());
        }

        public static float GetMinSpreadMultiplier()
        {
            return SkillTreeData.Sharpshooter.CurrentLevel == 0 ? 1f : MinSpreadMultiplier.GetValue(UseDefault.GetValue());
        }

        public static int GetAmmoCapacityMultiplier()
        {
            return 1 + (SkillTreeData.DoubleStackMags.CurrentLevel * AmmoCapacityBonus.GetValue(UseDefault.GetValue()));
        }

        public static float GetArrestTime()
        {
            return BaseArrestTime.GetValue(UseDefault.GetValue()) * (1 + SkillTreeData.Slippery.CurrentLevel * ArrestTimeBonus.GetValue(UseDefault.GetValue()));
        }

        public static float GetArrestRadius()
        {
            return SkillTreeData.Slippery.CurrentLevel == 0 ? BaseArrestRadius.GetValue(UseDefault.GetValue()) : BaseArrestRadius.GetValue(UseDefault.GetValue()) * ArrestRadiusBonus.GetValue(UseDefault.GetValue());
        }

        public static float GetPoliceSearchTime(float searchTime)
        {
            return SkillTreeData.PileTheBodiesHigh.CurrentLevel == 0 ? searchTime : Mathf.Clamp(searchTime, 0f, 35f) - Mathf.Clamp(KillCounts.PoliceKilled / 20, 0, 10);
        }

        public static int GetPoliceXPBonus()
        {
            return SkillTreeData.CombatExperience.CurrentLevel * PoliceXPBonus.GetValue(UseDefault.GetValue());
        }

        public static int GetCartelGoonXPBonus()
        {
            return SkillTreeData.CombatExperience.CurrentLevel * CartelGoonXPBonus.GetValue(UseDefault.GetValue());
        }

        public static int GetCartelDealerXPBonus()
        {
            return SkillTreeData.CombatExperience.CurrentLevel * CartelDealerXPBonus.GetValue(UseDefault.GetValue());
        }

        #endregion Stats

        #region Operations
        public static int GetCauldronOutput()
        {
            return BaseCauldronOutput.GetValue(UseDefault.GetValue()) * (1 + SkillTreeData.WitchsBrew.CurrentLevel * CauldronOutputBonus.GetValue(UseDefault.GetValue()));
        }

        public static int GetDryingRackCapacity()
        {
            return BaseDryingRackCapacity.GetValue(UseDefault.GetValue()) * (1 + SkillTreeData.CrankinOneOut.CurrentLevel * MixDryOutputSizeBonus.GetValue(UseDefault.GetValue()));
        }

        public static int GetChemistStationSpeedMultiplier()
        {
            return 1 + SkillTreeData.QuickCrafter.CurrentLevel * ChemistStationSpeedBonus.GetValue(UseDefault.GetValue());
        }

        public static int GetMethCocaProductQualityBonus()
        {
            return SkillTreeData.HarderAndStronger.CurrentLevel;
        }

        public static int GetMixDryOutputMultiplier()
        {
            return 1 + SkillTreeData.CrankinOneOut.CurrentLevel * MixDryOutputSizeBonus.GetValue(UseDefault.GetValue());
        }

        public static float GetGrowthSpeedMultiplier()
        {
            return 1f + (SkillTreeData.GreenThumb.CurrentLevel * GreenThumbBonus.GetValue(UseDefault.GetValue()));
        }

        public static float GetMoistureDrainMultiplier()
        {
            return SkillTreeData.WetAssPlants.CurrentLevel == 0 ? 1f : MoistureDrainBonus.GetValue(UseDefault.GetValue());
        }

        public static float GetPlantQualityBonus(string potName)
        {
            float potBonus = 0f;
            if (potName.Equals("Grow Tent"))
            {
                potBonus = SkillTreeData.PitchinATent.CurrentLevel * QualityBonusGrowTent.GetValue(UseDefault.GetValue());
            }
            else if (potName.Equals("Plastic Pot") || potName.Equals("Moisture-Preserving Pot"))
            {
                potBonus = SkillTreeData.AdvancedPotTechniques.CurrentLevel > 0 ? QualityBonusPlants.GetValue(UseDefault.GetValue()) : 0;
            }
            else if (potName.Equals("Air Pot"))
            {
                potBonus = SkillTreeData.AdvancedPotTechniques.CurrentLevel * QualityBonusPlants.GetValue(UseDefault.GetValue());
                potBonus += SkillTreeData.AdvancedPotTechniques.CurrentLevel == 2 ? 0.05f : 0f;
            }

            return potBonus;
        }

        public static float GetShroomQualityBonus()
        {
            return SkillTreeData.Mushroomancer.CurrentLevel * QualityBonusShrooms.GetValue(UseDefault.GetValue());
        }

        public static int GetPlantYieldBonus()
        {
            return SkillTreeData.BountifulHarvest.CurrentLevel * YieldBonusPlants.GetValue(UseDefault.GetValue());
        }

        public static int GetHarvestXPMultiplier()
        {
            return 1 + SkillTreeData.Apprenticeship.CurrentLevel * HarvestXPBonus.GetValue(UseDefault.GetValue());
        }

        public static float GetNewMixXPMultiplier()
        {
            return 1 + SkillTreeData.Apprenticeship.CurrentLevel * NewMixXPBonus.GetValue(UseDefault.GetValue());
        }

        #endregion Operations

        #region Social
        public static float GetATMLimit()
        {
            return BaseWeeklyDepositLimit.GetValue(UseDefault.GetValue()) + SkillTreeData.HoardTheWealth.CurrentLevel * ATMDepositBonus.GetValue(UseDefault.GetValue());
        }

        public static float GetCustomerSampleBonus()
        {
            return SkillTreeData.SilverTonguedDevil.CurrentLevel * CustomerSampleAcceptBonus.GetValue(UseDefault.GetValue());
        }

        public static int GetGrabberBinSize()
        {
            return Mathf.RoundToInt(BaseTrashGrabberBinSize.GetValue(UseDefault.GetValue()) * (1 + SkillTreeData.CommunityService.CurrentLevel * TrashGrabberBinSizeBonus.GetValue(UseDefault.GetValue())));
        }

        public static float GetGrabberPickupRadius()
        {
            return TrashPickupRadius.GetValue(UseDefault.GetValue()) * GetGrabberPickupRadiusMultiplier();
        }

        public static float GetGrabberPickupRadiusMultiplier()
        {
            return (1 + SkillTreeData.CommunityService.CurrentLevel * TrashPickupRadiusBonus.GetValue(UseDefault.GetValue()));
        }

        public static float GetPawnPriceMultiplier()
        {
            return 1 + (SkillTreeData.SacarLaBasura.CurrentLevel * PawnPriceBonus.GetValue(UseDefault.GetValue()));
        }

        public static int GetTrashValueBonus()
        {
            return SkillTreeData.SacarLaBasura.CurrentLevel * TrashValueBonus.GetValue(UseDefault.GetValue());
        }

        public static float GetLaunderingCapacityMultiplier()
        {
            return 1f + SkillTreeData.SqueakyClean.CurrentLevel * LaunderingBonus.GetValue(UseDefault.GetValue());
        }

        public static float GetCustomerCashMultiplier()
        {
            return 1f + SkillTreeData.SpreadTheWealth.CurrentLevel * CustomerCashBonus.GetValue(UseDefault.GetValue());
        }

        public static float GetSaleValueXPBonus()
        {
            return SkillTreeData.Grifter.CurrentLevel * SaleValueXPBonus.GetValue(UseDefault.GetValue());
        }

        public static int GetCounterOfferXPMultiplier()
        {
            return 1 + SkillTreeData.Grifter.CurrentLevel * CounterOfferXPBonus.GetValue(UseDefault.GetValue());
        }

        #endregion Social

        #region Logistician
        public static int GetMaxCustomers()
        {
            return BaseMaxCustomer.GetValue(UseDefault.GetValue()) + SkillTreeData.ExpansiveEmpire.CurrentLevel * DealerCustomerLimitBonus.GetValue(UseDefault.GetValue());
        }

        public static float GetDealerCutReduction()
        {
            return SkillTreeData.WageGarnishment.CurrentLevel * DealerCutReduction.GetValue(UseDefault.GetValue());
        }

        public static float GetSupplierCashMultiplier()
        {
            return 1f + SkillTreeData.ReliableBusinessPartner.CurrentLevel * SupplierCashBonus.GetValue(UseDefault.GetValue());
        }

        public static int GetCustomerOrderLimitBonus()
        {
            if (SkillTreeData.CaptiveMarket.CurrentLevel == 0) return 0;

            return SkillTreeData.CaptiveMarket.CurrentLevel * CustomerOrderLimitBonus.GetValue(UseDefault.GetValue()) + ((int)S1API.Leveling.LevelManager.Rank + 1) * CustomerOrderLimitRankBonus.GetValue(UseDefault.GetValue());
        }

        public static int GetSupplierItemLimit()
        {
            return (int)(BaseDeadDropItemLimit.GetValue(UseDefault.GetValue()) * (1f + SkillTreeData.ReliableBusinessPartner.CurrentLevel * SupplierItemBonus.GetValue(UseDefault.GetValue())));
        }

        public static float GetDealerSpeedMultiplier()
        {
            return 1f + SkillTreeData.MotivationalLeader.CurrentLevel * DealerSpeedBonus.GetValue(UseDefault.GetValue());
        }

        public static float GetEmployeeMoveSpeedMultiplier()
        {
            return SkillTreeData.EmployeeMovespeed.CurrentLevel == 0 ? 1f : Mathf.Clamp(EmployeeMoveSpeedBonus.GetValue(UseDefault.GetValue()), 0.1f, 10f);
        }

        public static float GetBotanistActionDurationMultiplier()
        {
            return SkillTreeData.FastFarmers.CurrentLevel == 0 ? 1f : Mathf.Clamp(BotanistActionDurationMultiplier.GetValue(UseDefault.GetValue()), 0.1f, 10f);
        }

        public static float GetHandlerPackagingSpeedMultiplier()
        {
            return SkillTreeData.FastHandlers.CurrentLevel == 0 ? 1f : Mathf.Clamp(HandlerPackagingSpeedMultiplier.GetValue(UseDefault.GetValue()), 0.1f, 10f);
        }

        public static float GetChemistActionDurationMultiplier()
        {
            return SkillTreeData.FastChemists.CurrentLevel == 0 ? 1f : Mathf.Clamp(ChemistActionDurationMultiplier.GetValue(UseDefault.GetValue()), 0.1f, 10f);
        }

        public static int GetEmployeeStationBonus()
        {
            return SkillTreeData.EmployeeMaxStation.CurrentLevel * EmployeeStationBonus.GetValue(UseDefault.GetValue());
        }

        public static (int, int) GetChemistStationBonus()
        {
            return (BaseMaxChemistStations.GetValue(UseDefault.GetValue()) + GetEmployeeStationBonus(), BaseMaxChemistStations.GetValue(UseDefault.GetValue()));
        }

        public static (int, int) GetBotanistStationBonus()
        {
            return (BaseMaxBotanistStations.GetValue(UseDefault.GetValue()) + GetEmployeeStationBonus(), BaseMaxBotanistStations.GetValue(UseDefault.GetValue()));
        }
        #endregion Logistician

        #region Special
        public static float GetBloodRushHealthBonus()
        {
            if (SkillTreeData.BloodRush.CurrentLevel == 0)
            {
                return 0f;
            }

            float policeBonus = (KillCounts.PoliceKilled / 2) * PoliceKilledBonus.GetValue(UseDefault.GetValue());
            float cartelBonus = KillCounts.CartelKilled * CartelKilledBonus.GetValue(UseDefault.GetValue());
            float healthCap = BloodRushHealthBonusCap.GetValue(UseDefault.GetValue()) * (BloodRush.IsBloodRushActive ? BloodRushHealthBonusMultiplier.GetValue(UseDefault.GetValue()) : 1f);
            float bonus = Mathf.Clamp(policeBonus + cartelBonus, 0f, healthCap);
            return bonus;
        }

        public static float GetSiphonFundsConversionMultiplier()
        {
            return SkillTreeData.SiphonFunds.CurrentLevel * (SiphonFundsBaseConversionRate.GetValue(UseDefault.GetValue()) + SiphonFundsOwnedBusinessBonus.GetValue(UseDefault.GetValue()) * BusinessManager.GetOwnedBusinesses().Count);
        }

        #endregion Special
    }
}