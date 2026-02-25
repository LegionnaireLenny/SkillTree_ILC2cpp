using MelonLoader;
using SkillTree.Core.App;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SkillTree.Core.FileManagement
{
    public enum SkillCategory
    {
        Stats,
        Operations,
        Social,
        Special
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class SkillAttribute : Attribute
    {
        public string Name;
        public string Description;
        public string Parent; 
        public SkillCategory Category;
        public int MaxLevel;

        public SkillAttribute(
            string name,
            string description,
            SkillCategory category,
            string parent = null,
            int maxLevel = 2)
        {
            Name = name;
            Description = description;
            Category = category;
            Parent = parent;
            MaxLevel = maxLevel;
        }
    }

    public class Skill(
        string name,
        string description,
        SkillCategory category,
        int maxLevel,
        int currentLevel,
        List<Skill> parents,
        List<Skill> children)
    {
        public string Name = name;
        public string Description = description;
        public SkillCategory Category = category;
        public int MaxLevel = maxLevel;
        public int CurrentLevel = currentLevel;
        public List<Skill> Parents = parents;
        public List<Skill> Children = children;

        public override string ToString()
        {
            string parents = "";
            foreach (var parent in Parents)
            {
                parents += $"{parent.Name}, ";
            }

            string children = "";
            foreach (var child in Children)
            {
                children += $"{child.Name}, ";
            }

            return $"{Name} | {Description} | {Category} | {MaxLevel} | {CurrentLevel} | {parents} | {children}";
        }
    }

    public class SkillTree_Test
    {
        public static Skill Stats = new("Hardy", "Increase max health by 20", SkillCategory.Stats, 1, 0, [], []);
        public static Skill BattleScarred = new("Battle-scarred", "Increase health regen by 100% and decrease health regen delay by 50%", SkillCategory.Stats, 1, 0, [Stats], []);
        public static Skill Slippery = new("Slippery", "Reduces police arrest radius by 25% and increases time until arrested by 100%", SkillCategory.Stats, 1, 0, [BattleScarred], []);
        public static Skill MoreMovespeed = new("Fleet Feet", "Increase movespeed by 15%", SkillCategory.Stats, 2, 0, [Stats], []);
        public static Skill SpringHeeled = new("Spring-Heeled", "Increase max stamina by 30% and jump height by 35%", SkillCategory.Stats, 1, 0, [MoreMovespeed], []);
        public static Skill MoreStackItem = new("Prison Wallet", "Double item stack size", SkillCategory.Stats, 1, 0, [Stats], []);
        public static Skill AllowSeeCounteroffChance = new("Crystal Ball", "See the chance of a customer accepting a counteroffer", SkillCategory.Stats, 1, 0, [Stats], []);
        public static Skill AllowSleepAthEne = new("Master Sleeper", "Allow sleeping while Athletic or Energizing effects are active", SkillCategory.Stats, 1, 0, [Stats], []);
        public static Skill SkipSchedule = new("Napping on the Job", "Can use a bed to skip to the next time period. \nPlants only grow at 33% of their normal speed when time is skipped.", SkillCategory.Stats, 1, 0, [AllowSleepAthEne], []);
        public static Skill MoreXP = new("Fast Learner", "Increase XP gain by 5%", SkillCategory.Stats, 2, 0, [Stats], []);
        public static Skill MoreXP2 = new("Turbo Nerdo", "Increase XP gain by an additional 5%", SkillCategory.Stats, 4, 0, [MoreXP], []);
        public static Skill MoreXPWhenEarnMoney = new("Kingpin", "Gain 5% of a drug sale's value as bonus XP", SkillCategory.Stats, 1, 0, [MoreXP], []);

        public static Skill Operations = new("Pitchin' a Tent", "Increase quality of plants in grow tents by 16%", SkillCategory.Operations, 1, 0, [], []);
        public static Skill MoreQuality = new("Advanced Pot Techniques", "Increase potted plant and mushroom quality by 15%. Bonus for plants in plastic pots \nand moisture pots capped at 15%. Mushrooms only affected at rank 2.", SkillCategory.Operations, 2, 0, [Operations], []);
        public static Skill MoreQualityMethCoca = new("Harder and Stronger", "Meth and cocaine quality increased by 1", SkillCategory.Operations, 1, 0, [MoreQuality], []);
        public static Skill AbsorbentSoil = new("Absorbent Soil", "Soil additives last until the soil is depleted", SkillCategory.Operations, 1, 0, [Operations], []);
        public static Skill GrowthSpeed = new("Green Thumb", "Increase plant and mushroom growth speed 2.5%", SkillCategory.Operations, 2, 0, [Operations], []);
        public static Skill GrowthSpeed2 = new("Plant Whisperer", "Increase plant and mushroom growth speed by an additional 2.5%", SkillCategory.Operations, 2, 0, [GrowthSpeed], []);
        public static Skill ChemistStationQuick = new("Quick Crafter", "Double the speed of cauldrons, chemistry stations, lab ovens, and mixing stations", SkillCategory.Operations, 1, 0, [GrowthSpeed], []);
        public static Skill MoreYield = new("Bountiful Harvest", "Increase base yield of plants by 1", SkillCategory.Operations, 1, 0, [Operations], []);
        public static Skill MoreMixAndDryingRackOutput = new("Crankin' One Out", "Double the production capacity of mixing stations and drying racks", SkillCategory.Operations, 1, 0, [MoreYield], []);
        public static Skill MoreCauldronOutput = new("Witch's Brew", "Double the cauldron's output", SkillCategory.Operations, 1, 0, [MoreYield], []);

        public static Skill Social = new("Silver Tongued Devil", "Increase chance a potential customer will accept a free sample by 5%", SkillCategory.Social, 1, 0, [], []);
        public static Skill CityEvolving = new("Spread the Wealth", "Increase citizens' weekly spending limits by 10%", SkillCategory.Social, 2, 0, [Social], []);
        public static Skill MoreATMLimit = new("Hoard the Wealth", "Increase ATM deposit limit by $2000", SkillCategory.Social, 2, 0, [Social], []);
        public static Skill BusinessEvolving = new("Squeaky Clean", "Increase money laundering capacity by 20%", SkillCategory.Social, 3, 0, [MoreATMLimit], []);
        public static Skill BetterSupplier = new("Reliable Business Partner", "Increase dead drop order limit by 67.5% and item limit by 50%", SkillCategory.Social, 2, 0, [Social], []);
        public static Skill BetterDelivery = new("Speed Dial", "Reduces delivery time. Minimum: 60 minutes -> 30 minutes | Maximum: 6 hours -> 2 hours", SkillCategory.Social, 1, 0, [BetterSupplier], []);
        public static Skill DealerMoreCustomer = new("Expansive Empire", "Increase dealer's customer limit by 2", SkillCategory.Social, 1, 0, [Social], []);
        public static Skill DealerCutLess = new("Wage Garnishment", "Decrease dealer's cut by 5%", SkillCategory.Social, 2, 0, [DealerMoreCustomer], []);
        public static Skill DealerSpeedUp = new("Motivational Leader", "Double the movespeed of dealers", SkillCategory.Social, 1, 0, [DealerMoreCustomer], []);

        public static Skill Special = new("Good Samaritan", "Once per day, destroy all trash on the map and gain 100% of the sell value as online balance", SkillCategory.Special, 1, 0, [], []);
        public static Skill Heal = new("Fit as a Fiddle", "Once per day, heal to max health", SkillCategory.Special, 1, 0, [Special], []);
        public static Skill GetCashDealer = new("Siphon Funds", "Once per day, instantly collect your cash from all dealers", SkillCategory.Special, 1, 0, [Special], []);
        public static Skill BetterBotanists = new("Fast Farmers", "Botanists perform all actions twice as fast", SkillCategory.Special, 1, 0, [Special], []);
        public static Skill Employees24h = new("Sweatshop", "Employees don't stop at 4 AM", SkillCategory.Special, 1, 0, [Special], []);
        public static Skill EmployeeMovespeed = new("RUN BITCH RUN!", "Employees move 3 times faster", SkillCategory.Special, 1, 0, [BetterBotanists], []);
        public static Skill EmployeeMaxStation = new("Over Worked and Underpaid", "Increase station assignment limit for botanists and chemists by 2", SkillCategory.Special, 2, 0, [BetterBotanists], []);


        public static HashSet<Skill> StatsTree = [
            Stats, BattleScarred, Slippery, MoreMovespeed, SpringHeeled, MoreStackItem, AllowSeeCounteroffChance, AllowSleepAthEne, SkipSchedule, MoreXP, MoreXP2, MoreXPWhenEarnMoney
        ];

        public static HashSet<Skill> OperationsTree = [
            Operations, MoreQuality, MoreQualityMethCoca, AbsorbentSoil, GrowthSpeed, GrowthSpeed2, ChemistStationQuick, MoreYield, MoreMixAndDryingRackOutput, MoreCauldronOutput
        ];

        public static HashSet<Skill> SocialTree = [
            Social, CityEvolving, MoreATMLimit, BusinessEvolving, BetterSupplier, BetterDelivery, DealerMoreCustomer, DealerCutLess, DealerSpeedUp
        ];

        public static HashSet<Skill> SpecialTree = [
            Special, Heal, GetCashDealer, BetterBotanists, Employees24h, EmployeeMovespeed, EmployeeMaxStation
        ];

        public static void AddChildren(HashSet<Skill> tree)
        {
            foreach (var node in tree)
            {
                foreach (var parent in node.Parents)
                {
                    parent.Children.Add(node);
                }
            }
        }

        public static void PrintTree(HashSet<Skill> tree)
        {
            foreach (var node in tree)
            {
                MelonLogger.Msg(node.ToString());
            }
        }
    }
}
