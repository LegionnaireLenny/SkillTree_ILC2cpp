using MelonLoader;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace SkillTree.Core.Skills
{
    public class SkillTreeData
    {
        public static Skill Stats = new("Hardy", "Increase max health by 20", SkillCategory.Stats, 1, 0, null, [], [Patches.Stats.HealthPatches.SetPlayerHealth]);
        public static Skill BattleScarred = new("Battle-scarred", "Increase health regen by 100% and decrease health regen delay by 50%", SkillCategory.Stats, 1, 0, Stats, [], [Patches.Stats.HealthPatches.SetPlayerHealth]);
        public static Skill Slippery = new("Slippery", "Reduces police arrest radius by 25% and increases time until arrested by 100%", SkillCategory.Stats, 1, 0, BattleScarred, []);
        public static Skill MoreMovespeed = new("Fleet Feet", "Increase movespeed by 15%", SkillCategory.Stats, 2, 0, Stats, [], [Patches.Stats.MovementPatches.SetPlayerSpeed]);
        public static Skill SpringHeeled = new("Spring-Heeled", "Increase max stamina by 30% and jump height by 35%", SkillCategory.Stats, 1, 0, MoreMovespeed, [], [Patches.Stats.MovementPatches.SetPlayerJumpHeight, Patches.Stats.MovementPatches.SetPlayerStamina]);
        public static Skill MoreStackItem = new("Prison Wallet", "Double item stack size", SkillCategory.Stats, 1, 0, Stats, [], [Patches.Stats.MoreStackItem.SetItemStackSize]);
        public static Skill AllowSeeCounteroffChance = new("Crystal Ball", "See the chance of a customer accepting a counteroffer", SkillCategory.Stats, 1, 0, Stats, []);
        public static Skill AllowSleepAthEne = new("Master Sleeper", "Allow sleeping while Athletic or Energizing effects are active", SkillCategory.Stats, 1, 0, Stats, []);
        public static Skill SkipSchedule = new("Napping on the Job", "Can use a bed to skip to the next time period. Plants only grow at 33% of their normal speed when time is skipped.", SkillCategory.Stats, 1, 0, AllowSleepAthEne, []);
        public static Skill MoreXP = new("Fast Learner", "Increase XP gain by 5%", SkillCategory.Stats, 2, 0, Stats, []);
        public static Skill MoreXP2 = new("Turbo Nerdo", "Increase XP gain by an additional 5%", SkillCategory.Stats, 4, 0, MoreXP, []);
        public static Skill MoreXPWhenEarnMoney = new("Kingpin", "Gain 5% of a drug sale's value as bonus XP", SkillCategory.Stats, 1, 0, MoreXP, []);

        public static Skill Operations = new("Pitchin' a Tent", "Increase quality of plants in grow tents by 16%", SkillCategory.Operations, 1, 0, null, []);
        public static Skill MoreQuality = new("Advanced Pot Techniques", "Rank 1: Increase plant quality for all pots by 15%.\n\nRank 2: Increase plant quality for air pots by an additional 20%", SkillCategory.Operations, 2, 0, Operations, []);
        public static Skill MoreQualityMethCoca = new("Harder and Stronger", "Meth and cocaine quality increased by 1", SkillCategory.Operations, 1, 0, MoreQuality, []);
        public static Skill Mushroomancer = new("Mushroomancer", "Increase mushroom quality by 15%", SkillCategory.Operations, 2, 0, MoreQuality, [], []);
        public static Skill WetAssPlants = new("Wet-Ass Plants", "Moisture drains 50% slower for all grow containers", SkillCategory.Operations, 1, 0, Operations, [], []);
        public static Skill AbsorbentSoil = new("Absorbent Soil", "Soil additives last until the soil is depleted", SkillCategory.Operations, 1, 0, WetAssPlants, []);
        public static Skill GrowthSpeed = new("Green Thumb", "Increase plant and mushroom growth speed 2.5%", SkillCategory.Operations, 2, 0, Operations, []);
        public static Skill GrowthSpeed2 = new("Plant Whisperer", "Increase plant and mushroom growth speed by an additional 2.5%", SkillCategory.Operations, 2, 0, GrowthSpeed, []);
        public static Skill ChemistStationQuick = new("Quick Crafter", "Double the speed of cauldrons, chemistry stations, lab ovens, and mixing stations", SkillCategory.Operations, 1, 0, GrowthSpeed, []);
        public static Skill MoreYield = new("Bountiful Harvest", "Increase base yield of plants by 1", SkillCategory.Operations, 1, 0, Operations, []);
        public static Skill MoreMixAndDryingRackOutput = new("Crankin' One Out", "Double the production capacity of mixing stations and drying racks", SkillCategory.Operations, 1, 0, MoreYield, []);
        public static Skill MoreCauldronOutput = new("Witch's Brew", "Double the cauldron's output", SkillCategory.Operations, 1, 0, MoreYield, []);

        public static Skill Social = new("Silver Tongued Devil", "Increase chance a potential customer will accept a free sample by 5%", SkillCategory.Social, 1, 0, null, []);
        public static Skill CityEvolving = new("Spread the Wealth", "Increase citizens' weekly spending limits by 10%", SkillCategory.Social, 2, 0, Social, [], [Patches.Social.CustomerPatches.SetCustomerSpendLimits]);
        public static Skill MoreATMLimit = new("Hoard the Wealth", "Increase ATM deposit limit by $2000", SkillCategory.Social, 2, 0, Social, []);
        public static Skill BusinessEvolving = new("Squeaky Clean", "Increase money laundering capacity by 30%", SkillCategory.Social, 2, 0, MoreATMLimit, [], [Patches.Social.BusinessPatches.SetLaunderingCapacity]);
        public static Skill Informant = new("Informant", "Police are shown on the map", SkillCategory.Social, 1, 0, Social, [], [Patches.Special.NPCPatches.UpdateVisibility]); 
        public static Skill Spymaster = new("Spymaster", "Benzies are shown on the map", SkillCategory.Social, 1, 0, Informant, [], [Patches.Special.NPCPatches.UpdateVisibility]); 
        public static Skill BetterSupplier = new("Reliable Business Partner", "Increase dead drop order limit by 67.5% and item limit by 50%", SkillCategory.Social, 2, 0, Social, []);
        public static Skill BetterDelivery = new("Speed Dial", "Reduces delivery time\n\nMinimum: 60 minutes -> 30 minutes\n\nMaximum: 6 hours -> 2 hours", SkillCategory.Social, 1, 0, BetterSupplier, []);
        public static Skill DealerMoreCustomer = new("Expansive Empire", "Increase dealer's customer limit by 2", SkillCategory.Social, 1, 0, Social, []);
        public static Skill DealerCutLess = new("Wage Garnishment", "Decrease dealer's cut by 5%", SkillCategory.Social, 2, 0, DealerMoreCustomer, [], [Patches.Social.DealerPatches.SetDealerCut]);
        public static Skill DealerSpeedUp = new("Motivational Leader", "Double the movespeed of dealers", SkillCategory.Social, 1, 0, DealerMoreCustomer, [], [Patches.Social.DealerPatches.SetDealerMoveSpeed]);

        public static Skill Special = new("Good Samaritan", "Once per day, destroy all trash on the map and gain 100% of the sell value as online balance", SkillCategory.Special, 1, 0, null, []);
        public static Skill Heal = new("Blood Rush", "Active ability: Once per day, fully restore health and gain Blood Rush for 60 seconds.\n\nPassive ability: Gain 0.1 max health for every police officer or cartel member killed, up to 30 health.\n\n While Blood Rush is active, the passive health cap is doubled to 60 health and health regen delay is reduced by 80% (90% with Battle-Scarred)", SkillCategory.Special, 1, 0, Special, []);
        public static Skill GetCashDealer = new("Siphon Funds", "Once per day, instantly collect your money from all dealers. 50% of collected money is converted to online balance.", SkillCategory.Special, 1, 0, Special, []);
        public static Skill Employees24h = new("Sweatshop", "Employees don't stop at 4 AM", SkillCategory.Special, 1, 0, Special, []);
        public static Skill BetterBotanists = new("Fast Farmers", "Botanists perform all actions twice as fast", SkillCategory.Special, 1, 0, Special, []);
        public static Skill EmployeeMovespeed = new("RUN BITCH RUN!", "Employees move 3 times faster", SkillCategory.Special, 1, 0, BetterBotanists, []);
        public static Skill EmployeeMaxStation = new("Over Worked and Underpaid", "Increase station assignment limit for botanists and chemists by 2", SkillCategory.Special, 2, 0, BetterBotanists, []);


        public static readonly HashSet<Skill> StatsTree = [
            Stats, 
            BattleScarred,
            Slippery, 
            MoreMovespeed, 
            SpringHeeled, 
            MoreStackItem, 
            AllowSeeCounteroffChance, 
            AllowSleepAthEne, 
            SkipSchedule, 
            MoreXP, 
            MoreXP2, 
            MoreXPWhenEarnMoney
        ];

        public static readonly HashSet<Skill> OperationsTree = [
            Operations, 
            MoreQuality, 
            MoreQualityMethCoca,
            Mushroomancer,
            WetAssPlants,
            AbsorbentSoil, 
            GrowthSpeed, 
            GrowthSpeed2, 
            ChemistStationQuick, 
            MoreYield, 
            MoreMixAndDryingRackOutput, 
            MoreCauldronOutput
        ];

        public static readonly HashSet<Skill> SocialTree = [
            Social, 
            CityEvolving, 
            MoreATMLimit, 
            BusinessEvolving, 
            BetterSupplier, 
            BetterDelivery,
            Informant,
            Spymaster,
            DealerMoreCustomer, 
            DealerCutLess, 
            DealerSpeedUp
        ];

        public static readonly HashSet<Skill> SpecialTree = [
            Special, 
            Heal, 
            GetCashDealer, 
            Employees24h, 
            BetterBotanists, 
            EmployeeMovespeed, 
            EmployeeMaxStation
        ];

        public static void AddChildren(HashSet<Skill> tree)
        {
            foreach (var node in tree)
            {
                node.Parent?.Children.Add(node);
            }
        }

        public static void PrintTree(HashSet<Skill> tree)
        {
            foreach (var node in tree)
            {
                MelonLogger.Msg(node.ToString());
            }
        }

        public static void ApplyAllSkills()
        {
            SkillTreeData obj = new SkillTreeData();
            foreach (var field in typeof(SkillTreeData).GetFields().Where(x => x.FieldType == typeof(Skill)))
            {
                (field.GetValue(obj) as Skill).ApplySkillEffect();
            }
        }

        public static void ValidateSkillTrees()
        {
            Stats.FixSkills();
            Operations.FixSkills();
            Social.FixSkills();
            Special.FixSkills();
        }

        public static Dictionary<string, int> GetSaveData()
        {
            Dictionary<string, int> skillData = [];

            SkillTreeData obj = new SkillTreeData();
            var fields = typeof(SkillTreeData).GetFields().Where(x => x.FieldType == typeof(Skill));

            foreach (var field in fields)
            {
                skillData[field.Name] = (field.GetValue(obj) as Skill).CurrentLevel;
            }

            return skillData;
        }

        public static Dictionary<string, int> GetDefaultSaveData()
        {
            Dictionary<string, int> skillData = [];

            SkillTreeData obj = new SkillTreeData();
            var fields = typeof(SkillTreeData).GetFields().Where(x => x.FieldType == typeof(Skill));

            foreach (var field in fields)
            {
                skillData[field.Name] = 0;
            }

            return skillData;
        }

        public static void LoadFromFile(JsonElement data)
        {
            SkillTreeData obj = new SkillTreeData();
            var fields = typeof(SkillTreeData).GetFields().Where(x => x.FieldType == typeof(Skill));

            foreach (var field in fields)
            {
                try
                {
                    (field.GetValue(obj) as Skill).CurrentLevel = data.GetProperty(field.Name).GetInt32();
                }
                catch (KeyNotFoundException e)
                {
                    throw new KeyNotFoundException($"Failed to load skills from file {e}");
                }
            }
        }

        public static void LoadDefaultValues()
        {
            SkillTreeData obj = new SkillTreeData();
            foreach (var field in typeof(SkillTreeData).GetFields().Where(x => x.FieldType == typeof(Skill)))
            {
                (field.GetValue(obj) as Skill).CurrentLevel = 0;
            }
        }
    }
}
