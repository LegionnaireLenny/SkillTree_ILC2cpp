using MelonLoader;
using SkillTree.Core.Patches.Enforcer;
using SkillTree.Core.Patches.Hustler;
using SkillTree.Core.Patches.Logistician;
using SkillTree.Core.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace SkillTree.Core.Serialization
{
    public class SkillTreeData
    {
        public static readonly Skill Hardy = new("Hardy", "You can survive injuries that would kill lesser people.\n\nYour maximum health is increased by 20", SkillCategory.Enforcer, 1, null, [HealthPatches.SetPlayerHealth]);
        public static readonly Skill BattleScarred = new("Battle-scarred", "Not even the most greivous wounds have kept you down for long.\n\nYou regenerate 100% more health per second and the amount of time before your health begins regenerating is reduced by 50%", SkillCategory.Enforcer, 1, Hardy, [HealthPatches.SetPlayerHealth]);
        public static readonly Skill Ghost = new("Ghost", "You always stay low-profile and apply a light touch.\n\nYour visibility is reduced by 25% and pickpocketing items is much easier.", SkillCategory.Enforcer, 1, Hardy, [Effects.Ghost.ApplyToPlayer, PickpocketPatches.SetPickpockDifficulty]);
        public static readonly Skill Slippery = new("Slippery", "You're an expert at keeping out of reach and breaking the tightest grips.\n\nReduces police arrest radius by 25% and increases time until arrested by 100%", SkillCategory.Enforcer, 1, Ghost);
        public static readonly Skill FleetFeet = new("Fleet Feet", "Increase movement speed by 15% per level", SkillCategory.Enforcer, 2, Hardy, [MovementPatches.SetPlayerSpeed]);
        public static readonly Skill SpringHeeled = new("Spring-Heeled", "Increase max stamina by 30% and jump height by 35%", SkillCategory.Enforcer, 1, FleetFeet, [MovementPatches.SetPlayerJumpHeight, MovementPatches.SetPlayerStamina]);
        public static readonly Skill PrisonWallet = new("Prison Wallet", "Increase item stack size by 100% per level", SkillCategory.Enforcer, 3, Hardy, [ItemStackPatches.SetItemStackSize]);
        public static readonly Skill CircadianMastery = new("Circadian Mastery", "You have achieved complete mastery of your sleep cycle.\n\nYou are able to sleep while Athletic or Energizing effects are active and can use a bed to rest until the next time period.\n\nPlants only grow at 33% of their normal speed when time is skipped.", SkillCategory.Enforcer, 1, Hardy);
        public static readonly Skill FastLearner = new("Fast Learner", "Increase XP gain by 5%", SkillCategory.Enforcer, 2, Hardy);
        public static readonly Skill TurboNerdo = new("Turbo Nerdo", "Increase XP gain by an additional 10%", SkillCategory.Enforcer, 2, FastLearner);
        public static readonly Skill Kingpin = new("Kingpin", "Gain 5% of a drug sale's value as bonus XP", SkillCategory.Enforcer, 1, FastLearner);

        public static readonly Skill PitchinATent = new("Pitchin' a Tent", "Increase quality of plants in grow tents by 16%", SkillCategory.Provisioner, 1, null);
        public static readonly Skill AdvancedPotTechniques = new("Advanced Pot Techniques", "Rank 1: Increase plant quality for all pots by 15%.\n\nRank 2: Increase plant quality for air pots by an additional 20%", SkillCategory.Provisioner, 2, PitchinATent);
        public static readonly Skill HarderAndStronger = new("Harder and Stronger", "Meth and cocaine quality increased by 1", SkillCategory.Provisioner, 1, AdvancedPotTechniques);
        public static readonly Skill Mushroomancer = new("Mushroomancer", "Increase mushroom quality by 15%", SkillCategory.Provisioner, 2, AdvancedPotTechniques);
        public static readonly Skill WetAssPlants = new("Wet-Ass Plants", "Moisture drains 50% slower for all grow containers", SkillCategory.Provisioner, 1, PitchinATent);
        public static readonly Skill AbsorbentSoil = new("Absorbent Soil", "Soil additives last until the soil is depleted", SkillCategory.Provisioner, 1, WetAssPlants);
        public static readonly Skill GreenThumb = new("Green Thumb", "Increase plant and mushroom growth speed 2.5%", SkillCategory.Provisioner, 2, PitchinATent);
        public static readonly Skill PlantWhisperer = new("Plant Whisperer", "Increase plant and mushroom growth speed by an additional 2.5%", SkillCategory.Provisioner, 2, GreenThumb);
        public static readonly Skill QuickCrafter = new("Quick Crafter", "Double the speed of cauldrons, chemistry stations, lab ovens, and mixing stations", SkillCategory.Provisioner, 1, GreenThumb);
        public static readonly Skill BountifulHarvest = new("Bountiful Harvest", "Increase base yield of plants by 1", SkillCategory.Provisioner, 1, PitchinATent);
        public static readonly Skill CrankinOneOut = new("Crankin' One Out", "Double the production capacity of mixing stations and drying racks", SkillCategory.Provisioner, 1, BountifulHarvest);
        public static readonly Skill WitchsBrew = new("Witch's Brew", "Double the cauldron's output", SkillCategory.Provisioner, 1, BountifulHarvest);

        public static readonly Skill ReliableBusinessPartner = new("Reliable Business Partner", "Increase dead drop order limit by 67.5% and item limit by 50%", SkillCategory.Logistician, 2, null);
        public static readonly Skill RushDelivery = new("Rush Delivery", "Reduces delivery time\n\nMinimum: 60 minutes -> 30 minutes\n\nMaximum: 6 hours -> 2 hours", SkillCategory.Logistician, 1, ReliableBusinessPartner);
        public static readonly Skill ExpansiveEmpire = new("Expansive Empire", "Increase dealer's customer limit by 2", SkillCategory.Logistician, 1, ReliableBusinessPartner);
        public static readonly Skill WageGarnishment = new("Wage Garnishment", "Decrease dealer's cut by 5%", SkillCategory.Logistician, 2, ExpansiveEmpire, [DealerPatches.SetDealerCut]);
        public static readonly Skill MotivationalLeader = new("Motivational Leader", "Double the movespeed of dealers", SkillCategory.Logistician, 1, ExpansiveEmpire, [DealerPatches.SetDealerMoveSpeed]);
        public static readonly Skill FastFarmers = new("Fast Farmers", "Botanists perform all actions twice as fast", SkillCategory.Logistician, 1, ReliableBusinessPartner);
        public static readonly Skill FastHandlers = new("Fast Handlers", "Packagers perform all actions twice as fast", SkillCategory.Logistician, 1, FastFarmers, [HandlerBehaviorPatches.SetHandlerPackagingSpeed]);
        public static readonly Skill FastChemists = new("Fast Chemists", "Chemists perform all actions twice as fast", SkillCategory.Logistician, 1, FastFarmers);
        public static readonly Skill NightShift = new("Night Shift", "Employees don't stop at 4 AM", SkillCategory.Logistician, 1, ReliableBusinessPartner);
        public static readonly Skill EmployeeMovespeed = new("RUN BITCH RUN!", "Employees move 3 times faster", SkillCategory.Logistician, 1, NightShift);
        public static readonly Skill EmployeeMaxStation = new("Overworked and Underpaid", "Increase station assignment limit for botanists and chemists by 2", SkillCategory.Logistician, 2, NightShift);

        public static readonly Skill SilverTonguedDevil = new("Silver Tongued Devil", "Increase chance a potential customer will accept a free sample by 5%", SkillCategory.Hustler, 1, null);
        public static readonly Skill CommunityService = new("Community Service", "Through your experience in gathering trash, you've become more adept at picking up and storing trash\n\nTrash grabbers now pick up trash in a radius roughly equal to a trash bag and can store twice as many items", SkillCategory.Hustler, 1, SilverTonguedDevil);
        public static readonly Skill SacarLaBasura = new("Sacar La Basura", "You have the uncanny ability to convince people that your trash is their treasure\n\nTrash is worth $1 more per level and pawned items are worth 25% more per level", SkillCategory.Hustler, 2, CommunityService, [TrashPatches.IncreaseTrashValue]);
        public static readonly Skill SpreadTheWealth = new("Spread the Wealth", "Increase citizens' weekly spending limits by 25% per level", SkillCategory.Hustler, 2, SilverTonguedDevil, [CustomerPatches.SetCustomerSpendLimits]);
        public static readonly Skill CaptiveMarket = new("Captive Market", "Increase citizens' weekly order limits by 3 per level and 1 per rank", SkillCategory.Hustler, 2, SpreadTheWealth, [CustomerPatches.SetCustomerOrderLimits]);
        public static readonly Skill HoardTheWealth = new("Hoard the Wealth", "Increase ATM deposit limit by $2500 per level", SkillCategory.Hustler, 2, SilverTonguedDevil);
        public static readonly Skill SqueakyClean = new("Squeaky Clean", "Increase money laundering capacity by 30%", SkillCategory.Hustler, 2, HoardTheWealth, [BusinessPatches.SetLaunderingCapacity]);
        public static readonly Skill CrystalBall = new("Crystal Ball", "See the chance of a customer accepting a counteroffer", SkillCategory.Hustler, 1, SilverTonguedDevil);
        public static readonly Skill Informant = new("Informant", "Police are shown on the map", SkillCategory.Hustler, 1, CrystalBall, [NPCPatches.UpdateVisibility]);
        public static readonly Skill Spymaster = new("Spymaster", "Benzies are shown on the map", SkillCategory.Hustler, 1, CrystalBall, [NPCPatches.UpdateVisibility]);

        public static readonly Skill GoodSamaritan = new("Good Samaritan", "Once per day, destroy all trash on the map and gain 100% of the sell value as online balance", SkillCategory.Special, 1, null);
        public static readonly Skill BloodRush = new("Blood Rush", "Active ability: Once per day, fully restore health and gain Blood Rush for 60 seconds.\n\nPassive ability: Gain 0.1 max health for every police officer or cartel member killed, up to 30 health.\n\nWhile Blood Rush is active, the passive health cap is doubled to 60 health and health regen delay is reduced by 80% (90% with Battle-Scarred)", SkillCategory.Special, 1, GoodSamaritan);
        public static readonly Skill SiphonFunds = new("Siphon Funds", "Once per day, instantly collect your money from all dealers. 10% + 5% per owned business of collected money is converted to online balance.", SkillCategory.Special, 1, GoodSamaritan);
        public static readonly Skill TrickleDown = new("Trick-down Economics", "Once per day, instantly deposit your cash in all owned businesses for laundering while keeping a minimum in reserve.\n\nLaundering operations now pay out in increments of 25% every 6 hours instead of 100% every 24 hours", SkillCategory.Special, 1, GoodSamaritan);

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
