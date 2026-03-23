using MelonLoader;
using SkillTree.Core.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace SkillTree.Core.Serialization
{
    public class SkillTreeData
    {
        public static readonly Skill Stats = new("Hardy", "You can survive injuries that would kill lesser people.\n\nYour maximum health is increased by 20", SkillCategory.Stats, 1, null, [Patches.Stats.HealthPatches.SetPlayerHealth]);
        public static readonly Skill BattleScarred = new("Battle-scarred", "Not even the most greivous wounds have kept you down for long.\n\nYou regenerate 100% more health per second and the amount of time before your health begins regenerating is reduced by 50%", SkillCategory.Stats, 1, Stats, [Patches.Stats.HealthPatches.SetPlayerHealth]);
        public static readonly Skill Ghost = new("Ghost", "You always stay low-profile and apply a light touch.\n\nYour visibility is reduced by 25% and pickpocketing items is much easier.", SkillCategory.Stats, 1, Stats, [Effects.Ghost.ApplyToPlayer, Patches.Stats.PickpocketPatches.SetPickpockDifficulty]);
        public static readonly Skill Slippery = new("Slippery", "You're an expert at keeping out of reach and breaking the tightest grips.\n\nReduces police arrest radius by 25% and increases time until arrested by 100%", SkillCategory.Stats, 1, Ghost);
        public static readonly Skill MoreMovespeed = new("Fleet Feet", "Increase movement speed by 15% per level", SkillCategory.Stats, 2, Stats, [Patches.Stats.MovementPatches.SetPlayerSpeed]);
        public static readonly Skill SpringHeeled = new("Spring-Heeled", "Increase max stamina by 30% and jump height by 35%", SkillCategory.Stats, 1, MoreMovespeed, [Patches.Stats.MovementPatches.SetPlayerJumpHeight, Patches.Stats.MovementPatches.SetPlayerStamina]);
        public static readonly Skill MoreStackItem = new("Prison Wallet", "Increase item stack size by 100% per level", SkillCategory.Stats, 3, Stats, [Patches.Stats.MoreStackItem.SetItemStackSize]);
        public static readonly Skill AllowSeeCounteroffChance = new("Crystal Ball", "See the chance of a customer accepting a counteroffer", SkillCategory.Stats, 1, Stats);
        public static readonly Skill CircadianMastery = new("Circadian Mastery", "You have achieved complete mastery of your sleep cycle.\n\nYou are able to sleep while Athletic or Energizing effects are active and can use a bed to rest until the next time period.\n\nPlants only grow at 33% of their normal speed when time is skipped.", SkillCategory.Stats, 1, Stats);
        public static readonly Skill MoreXP = new("Fast Learner", "Increase XP gain by 5%", SkillCategory.Stats, 2, Stats);
        public static readonly Skill MoreXP2 = new("Turbo Nerdo", "Increase XP gain by an additional 10%", SkillCategory.Stats, 2, MoreXP);
        public static readonly Skill MoreXPWhenEarnMoney = new("Kingpin", "Gain 5% of a drug sale's value as bonus XP", SkillCategory.Stats, 1, MoreXP);

        public static readonly Skill Operations = new("Pitchin' a Tent", "Increase quality of plants in grow tents by 16%", SkillCategory.Operations, 1, null);
        public static readonly Skill MoreQuality = new("Advanced Pot Techniques", "Rank 1: Increase plant quality for all pots by 15%.\n\nRank 2: Increase plant quality for air pots by an additional 20%", SkillCategory.Operations, 2, Operations);
        public static readonly Skill MoreQualityMethCoca = new("Harder and Stronger", "Meth and cocaine quality increased by 1", SkillCategory.Operations, 1, MoreQuality);
        public static readonly Skill Mushroomancer = new("Mushroomancer", "Increase mushroom quality by 15%", SkillCategory.Operations, 2, MoreQuality);
        public static readonly Skill WetAssPlants = new("Wet-Ass Plants", "Moisture drains 50% slower for all grow containers", SkillCategory.Operations, 1, Operations);
        public static readonly Skill AbsorbentSoil = new("Absorbent Soil", "Soil additives last until the soil is depleted", SkillCategory.Operations, 1, WetAssPlants);
        public static readonly Skill GrowthSpeed = new("Green Thumb", "Increase plant and mushroom growth speed 2.5%", SkillCategory.Operations, 2, Operations);
        public static readonly Skill GrowthSpeed2 = new("Plant Whisperer", "Increase plant and mushroom growth speed by an additional 2.5%", SkillCategory.Operations, 2, GrowthSpeed);
        public static readonly Skill ChemistStationQuick = new("Quick Crafter", "Double the speed of cauldrons, chemistry stations, lab ovens, and mixing stations", SkillCategory.Operations, 1, GrowthSpeed);
        public static readonly Skill MoreYield = new("Bountiful Harvest", "Increase base yield of plants by 1", SkillCategory.Operations, 1, Operations);
        public static readonly Skill MoreMixAndDryingRackOutput = new("Crankin' One Out", "Double the production capacity of mixing stations and drying racks", SkillCategory.Operations, 1, MoreYield);
        public static readonly Skill MoreCauldronOutput = new("Witch's Brew", "Double the cauldron's output", SkillCategory.Operations, 1, MoreYield);

        public static readonly Skill Social = new("Silver Tongued Devil", "Increase chance a potential customer will accept a free sample by 5%", SkillCategory.Social, 1, null);
        public static readonly Skill CommunityService = new("Community Service", "Through your experience in gathering trash, you've become more adept at picking up and storing trash\n\nTrash grabbers now pick up trash in a radius roughly equal to a trash bag and can store twice as many items", SkillCategory.Social, 1, Social);
        public static readonly Skill SacarLaBasura = new("Sacar La Basura", "You have the uncanny ability to convince people that your trash is their treasure\n\nTrash is worth $1 more per level and pawned items are worth 25% more per level", SkillCategory.Social, 2, CommunityService, [Patches.Social.TrashPatches.IncreaseTrashValue]);
        public static readonly Skill CityEvolving = new("Spread the Wealth", "Increase citizens' weekly spending limits by 10% per level", SkillCategory.Social, 2, Social, [Patches.Social.CustomerPatches.SetCustomerSpendLimits]);
        public static readonly Skill MoreATMLimit = new("Hoard the Wealth", "Increase ATM deposit limit by $2000 per level", SkillCategory.Social, 2, Social);
        public static readonly Skill BusinessEvolving = new("Squeaky Clean", "Increase money laundering capacity by 30%", SkillCategory.Social, 2, MoreATMLimit, [Patches.Social.BusinessPatches.SetLaunderingCapacity]);
        public static readonly Skill Informant = new("Informant", "Police are shown on the map", SkillCategory.Social, 1, Social, [Patches.Special.NPCPatches.UpdateVisibility]); 
        public static readonly Skill Spymaster = new("Spymaster", "Benzies are shown on the map", SkillCategory.Social, 1, Informant, [Patches.Special.NPCPatches.UpdateVisibility]); 
        public static readonly Skill BetterSupplier = new("Reliable Business Partner", "Increase dead drop order limit by 67.5% and item limit by 50%", SkillCategory.Social, 2, Social);
        public static readonly Skill BetterDelivery = new("Speed Dial", "Reduces delivery time\n\nMinimum: 60 minutes -> 30 minutes\n\nMaximum: 6 hours -> 2 hours", SkillCategory.Social, 1, BetterSupplier);
        public static readonly Skill DealerMoreCustomer = new("Expansive Empire", "Increase dealer's customer limit by 2", SkillCategory.Social, 1, Social);
        public static readonly Skill DealerCutLess = new("Wage Garnishment", "Decrease dealer's cut by 5%", SkillCategory.Social, 2, DealerMoreCustomer, [Patches.Social.DealerPatches.SetDealerCut]);
        public static readonly Skill DealerSpeedUp = new("Motivational Leader", "Double the movespeed of dealers", SkillCategory.Social, 1, DealerMoreCustomer, [Patches.Social.DealerPatches.SetDealerMoveSpeed]);

        public static readonly Skill Special = new("Good Samaritan", "Once per day, destroy all trash on the map and gain 100% of the sell value as online balance", SkillCategory.Special, 1, null);
        public static readonly Skill Heal = new("Blood Rush", "Active ability: Once per day, fully restore health and gain Blood Rush for 60 seconds.\n\nPassive ability: Gain 0.1 max health for every police officer or cartel member killed, up to 30 health.\n\nWhile Blood Rush is active, the passive health cap is doubled to 60 health and health regen delay is reduced by 80% (90% with Battle-Scarred)", SkillCategory.Special, 1, Special);
        public static readonly Skill GetCashDealer = new("Siphon Funds", "Once per day, instantly collect your money from all dealers. 10% + 5% per owned business of collected money is converted to online balance.", SkillCategory.Special, 1, Special);
        public static readonly Skill TrickleDown = new("Trick-down Economics", "Once per day, instantly deposit your cash in all owned businesses for laundering while keeping a minimum in reserve.\n\nLaundering operations now pay out in increments of 25% every 6 hours instead of 100% every 24 hours", SkillCategory.Special, 1, Special);
        public static readonly Skill Employees24h = new("Sweatshop", "Employees don't stop at 4 AM", SkillCategory.Special, 1, Special);
        public static readonly Skill BetterBotanists = new("Fast Farmers", "Botanists perform all actions twice as fast", SkillCategory.Special, 1, Special);
        public static readonly Skill EmployeeMovespeed = new("RUN BITCH RUN!", "Employees move 3 times faster", SkillCategory.Special, 1, BetterBotanists);
        public static readonly Skill EmployeeMaxStation = new("Overworked and Underpaid", "Increase station assignment limit for botanists and chemists by 2", SkillCategory.Special, 2, BetterBotanists);

        public static readonly Dictionary<SkillCategory, HashSet<Skill>> SkillTrees = [];

        public static void CreateTrees()
        {
            foreach (SkillCategory category in Enum.GetValues(typeof(SkillCategory)))
            {
                CreateTree(category);
            }

            void AddChildren(HashSet<Skill> tree)
            {
                foreach (var node in tree)
                {
                    node.Parent?.Children.Add(node);
                }
            }

            void CreateTree(SkillCategory category)
            {
                HashSet<Skill> tree = [];

                var fields = typeof(SkillTreeData).GetFields().Where(x => x.FieldType == typeof(Skill) && (x.GetValue(new SkillTreeData()) as Skill).Category == category);
                foreach (var field in fields)
                {
                    tree.Add(field.GetValue(new SkillTreeData()) as Skill);
                }

                AddChildren(tree);
                SkillTrees.Add(category, tree);
            }
        }

        public static void ApplyAllSkills()
        {
            foreach (var field in typeof(SkillTreeData).GetFields().Where(x => x.FieldType == typeof(Skill)))
            {
                (field.GetValue(new SkillTreeData()) as Skill).ApplySkillEffect();
            }
        }

        public static void ValidateSkillTrees()
        {
            foreach (HashSet<Skill> tree in SkillTrees.Values)
            {
                tree.First(x => x.Parent == null).FixSkills();
            }
        }

        public static int GetPointsSpent(HashSet<Skill> tree)
        {
            int points = 0;
            foreach (Skill skill in tree)
            {
                points += skill.CurrentLevel;
            }
            return points;
        }

        public static Dictionary<SkillCategory, int> GetCategoryPointsSpent()
        {
            Dictionary<SkillCategory, int> points = [];
            foreach (SkillCategory category in Enum.GetValues(typeof(SkillCategory)))
            {
                points.Add(category, GetPointsSpent(SkillTrees[category]));
            }
            return points;
        }

        public static Dictionary<string, string> GetSaveData()
        {
            Dictionary<string, string> skillData = [];

            var fields = typeof(SkillTreeData).GetFields().Where(x => x.FieldType == typeof(Skill));
            foreach (var field in fields)
            {
                skillData[field.Name] = (field.GetValue(new SkillTreeData()) as Skill).CurrentLevel.ToString();
            }

            return skillData;
        }

        public static void LoadFromFile(JsonElement data)
        {
            var fields = typeof(SkillTreeData).GetFields().Where(x => x.FieldType == typeof(Skill));
            foreach (var field in fields)
            {
                try
                {
                    int value = data.GetProperty(field.Name).ValueKind == JsonValueKind.String ? int.Parse(data.GetProperty(field.Name).GetString()) : data.GetProperty(field.Name).GetInt32();
                    (field.GetValue(new SkillTreeData()) as Skill).CurrentLevel = value;
                }
                catch (KeyNotFoundException e)
                {
                    MelonLogger.Warning($"Failed to load {field} from file {e}");
                }
            }
        }

        public static void LoadDefaultValues()
        {
            foreach (var field in typeof(SkillTreeData).GetFields().Where(x => x.FieldType == typeof(Skill)))
            {
                (field.GetValue(new SkillTreeData()) as Skill).CurrentLevel = 0;
            }
        }
    }
}
