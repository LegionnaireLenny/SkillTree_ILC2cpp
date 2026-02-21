using SkillTree.Core.FileManagement;
using System.Reflection;

namespace SkillTree.Core
{
    public static class SkillSystem
    {
        public static void ApplySkill(string skillId)
        {
            switch (skillId)
            {
                // Stats
                case "Stats":
                case "BattleScarred":
                    Patches.Stats.HealthPatches.SetPlayerHealth();
                    break;
                case "Slippery":
                    Patches.Stats.MovementPatches.SetPlayerJumpHeight();
                    Patches.Stats.MovementPatches.SetPlayerStamina();
                    break;
                case "MoreMovespeed":
                    Patches.Stats.MovementPatches.SetPlayerSpeed();
                    break;
                case "MoreStackItem":
                    Patches.Stats.MoreStackItem.SetItemStackSize();
                    break;
                case "MoreXP":
                case "MoreXP2":
                case "AllowSleepAthEne":
                case "AllowSeeCounteroffChance":
                case "SkipSchedule":
                case "MoreXPWhenEarnMoney":
                    break;

                // OPERATIONS
                case "Operations":
                case "GrowthSpeed":
                case "GrowthSpeed2":
                case "MoreYield":
                case "MoreQuality":
                case "MoreQualityMethCoca":
                case "AbsorbentSoil":
                case "MoreMixAndDryingRackOutput":
                case "ChemistStationQuick":
                case "MoreCauldronOutput":
                    break;

                // SOCIAL
                case "Social":
                    break;
                case "CityEvolving":
                    Patches.Social.CustomerPatches.SetCustomerSpendLimits();
                    break;
                case "BusinessEvolving":
                    Patches.Social.BusinessPatches.SetLaunderingCapacity();
                    break;
                case "MoreATMLimit":
                    break;
                case "DealerCutLess":
                    Patches.Social.DealerPatches.SetDealerCut();
                    break;
                case "DealerSpeedUp":
                    Patches.Social.DealerPatches.SetDealerMoveSpeed();
                    break;
                case "DealerMoreCustomer":
                case "BetterSupplier":
                case "BetterDelivery":
                    break;

                //SPECIAL
                case "Special":
                case "Heal":
                case "GetCashDealer":
                case "BetterBotanists":
                case "Employees24h":
                case "EmployeeMovespeed":
                case "EmployeeMaxStation":
                    break;
            }
        }

        public static void ApplyAll()
        {
            foreach (var field in typeof(SkillTreeData).GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                ApplySkill(field.Name);
            }
        }
    }
}
