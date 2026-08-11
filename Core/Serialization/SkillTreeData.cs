using MelonLoader;
using SkillTree.Core.Patches.Enforcer;
using SkillTree.Core.Patches.Hustler;
using SkillTree.Core.Patches.Logistician;
using SkillTree.Core.Skills;
using SkillTree.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace SkillTree.Core.Serialization
{
    public class SkillTreeData
    {
        public static readonly Skill Hardy = new("Hardy", SkillCategory.Enforcer, 1, null, [HealthPatches.SetPlayerHealth]);
        public static readonly Skill BattleScarred = new("Battle-scarred", SkillCategory.Enforcer, 1, Hardy, [HealthPatches.SetPlayerHealth]);
        public static readonly Skill PileTheBodiesHigh = new("Pile the Bodies High", SkillCategory.Enforcer, 1, BattleScarred);
        public static readonly Skill QuickDraw = new("Quick Draw McGraw", SkillCategory.Enforcer, 1, Hardy, [RangedWeaponPatches.SetWeaponStats]);
        public static readonly Skill Sharpshooter = new("Sharpshooter", SkillCategory.Enforcer, 1, QuickDraw, [RangedWeaponPatches.SetWeaponStats]);
        public static readonly Skill Ghost = new("Ghost", SkillCategory.Enforcer, 1, Hardy, [Effects.Ghost.ApplyToPlayer, PickpocketPatches.SetPickpockDifficulty]);
        public static readonly Skill Slippery = new("Slippery", SkillCategory.Enforcer, 1, Ghost);
        public static readonly Skill FleetFeet = new("Fleet Feet", SkillCategory.Enforcer, 2, Hardy, [MovementPatches.SetPlayerSpeed]);
        public static readonly Skill SpringHeeled = new("Spring-Heeled", SkillCategory.Enforcer, 1, FleetFeet, [MovementPatches.SetPlayerJumpHeight, MovementPatches.SetPlayerStamina]);
        public static readonly Skill PrisonWallet = new("Prison Wallet", SkillCategory.Enforcer, 1, Hardy, [ItemStackPatches.SetItemStackSize]);
        public static readonly Skill QuantumStockpile = new("Quantum Stockpile", SkillCategory.Enforcer, 2, PrisonWallet, [ItemStackPatches.SetItemStackSize]);
        public static readonly Skill DoubleStackMags = new("Double-Stack Mags", SkillCategory.Enforcer, 1, PrisonWallet, [RangedWeaponPatches.SetWeaponStats]);
        public static readonly Skill CircadianMastery = new("Circadian Mastery", SkillCategory.Enforcer, 1, Hardy);
        public static readonly Skill CombatExperience = new("Combat Experience", SkillCategory.Enforcer, 1, Hardy);
        public static readonly Skill SchoolOfHardKnocks = new("School of Hard Knocks", SkillCategory.Enforcer, 1, CombatExperience);

        public static readonly Skill Apprenticeship = new("Apprenticeship", SkillCategory.Provisioner, 1, null);
        public static readonly Skill PitchinATent = new("Pitchin' a Tent", SkillCategory.Provisioner, 1, Apprenticeship);
        public static readonly Skill AdvancedPotTechniques = new("Advanced Pot Techniques", SkillCategory.Provisioner, 2, PitchinATent);
        public static readonly Skill HarderAndStronger = new("Harder and Stronger", SkillCategory.Provisioner, 1, AdvancedPotTechniques);
        public static readonly Skill Mushroomancer = new("Mushroomancer", SkillCategory.Provisioner, 2, AdvancedPotTechniques);
        public static readonly Skill WetAssPlants = new("Wet-Ass Plants", SkillCategory.Provisioner, 1, PitchinATent);
        public static readonly Skill AbsorbentSoil = new("Absorbent Soil", SkillCategory.Provisioner, 1, WetAssPlants);
        public static readonly Skill GreenThumb = new("Green Thumb", SkillCategory.Provisioner, 2, PitchinATent);
        public static readonly Skill QuickCrafter = new("Quick Crafter", SkillCategory.Provisioner, 1, GreenThumb);
        public static readonly Skill BountifulHarvest = new("Bountiful Harvest", SkillCategory.Provisioner, 1, PitchinATent);
        public static readonly Skill CrankinOneOut = new("Crankin' One Out", SkillCategory.Provisioner, 1, BountifulHarvest);
        public static readonly Skill WitchsBrew = new("Witch's Brew", SkillCategory.Provisioner, 1, BountifulHarvest);
        public static readonly Skill Meister = new("Meister", SkillCategory.Provisioner, 1, Apprenticeship);

        public static readonly Skill SilverTonguedDevil = new("Silver Tongued Devil", SkillCategory.Hustler, 1, null);
        public static readonly Skill CommunityService = new("Community Service", SkillCategory.Hustler, 1, SilverTonguedDevil);
        public static readonly Skill SacarLaBasura = new("Sacar La Basura", SkillCategory.Hustler, 2, CommunityService, [TrashPatches.IncreaseTrashValue]);
        public static readonly Skill SpreadTheWealth = new("Spread the Wealth", SkillCategory.Hustler, 2, SilverTonguedDevil, [CustomerPatches.SetCustomerSpendLimits]);
        public static readonly Skill CaptiveMarket = new("Captive Market", SkillCategory.Hustler, 2, SpreadTheWealth, [CustomerPatches.SetCustomerOrderLimits]);
        public static readonly Skill HoardTheWealth = new("Hoard the Wealth", SkillCategory.Hustler, 2, SilverTonguedDevil);
        public static readonly Skill SqueakyClean = new("Squeaky Clean", SkillCategory.Hustler, 2, HoardTheWealth, [BusinessPatches.SetLaunderingCapacity]);
        public static readonly Skill CrystalBall = new("Crystal Ball", SkillCategory.Hustler, 1, SilverTonguedDevil);
        public static readonly Skill Informant = new("Informant", SkillCategory.Hustler, 1, CrystalBall, [NPCPatches.UpdateVisibility]);
        public static readonly Skill Spymaster = new("Spymaster", SkillCategory.Hustler, 1, CrystalBall, [NPCPatches.UpdateVisibility]);
        public static readonly Skill Grifter = new("Grifter", SkillCategory.Hustler, 1, SilverTonguedDevil);
        public static readonly Skill MultiLevelMarketeer = new("Multi-level Marketeer", SkillCategory.Hustler, 1, Grifter);

        public static readonly Skill ReliableBusinessPartner = new("Reliable Business Partner", SkillCategory.Logistician, 2, null);
        public static readonly Skill RushDelivery = new("Rush Delivery", SkillCategory.Logistician, 1, ReliableBusinessPartner);
        public static readonly Skill ExpansiveEmpire = new("Expansive Empire", SkillCategory.Logistician, 1, ReliableBusinessPartner);
        public static readonly Skill WageGarnishment = new("Wage Garnishment", SkillCategory.Logistician, 2, ExpansiveEmpire, [DealerPatches.SetDealerCut]);
        public static readonly Skill MotivationalLeader = new("Motivational Leader", SkillCategory.Logistician, 1, ExpansiveEmpire, [DealerPatches.SetDealerMoveSpeed]);
        public static readonly Skill FastFarmers = new("Fast Farmers", SkillCategory.Logistician, 1, ReliableBusinessPartner);
        public static readonly Skill FastHandlers = new("Fast Handlers", SkillCategory.Logistician, 1, FastFarmers, [HandlerBehaviorPatches.SetHandlerPackagingSpeed]);
        public static readonly Skill FastChemists = new("Fast Chemists", SkillCategory.Logistician, 1, FastFarmers);
        public static readonly Skill NightShift = new("Night Shift", SkillCategory.Logistician, 1, ReliableBusinessPartner);
        public static readonly Skill EmployeeMovespeed = new("RUN BITCH RUN!", SkillCategory.Logistician, 1, NightShift);
        public static readonly Skill EmployeeMaxStation = new("Overworked and Underpaid", SkillCategory.Logistician, 2, NightShift);
        public static readonly Skill EducatedWorkforce = new("Educated Workforce", SkillCategory.Logistician, 1, ReliableBusinessPartner);

        public static readonly Skill GoodSamaritan = new("Good Samaritan", SkillCategory.Special, 1, null);
        public static readonly Skill BloodRush = new("Blood Rush", SkillCategory.Special, 1, GoodSamaritan);
        public static readonly Skill BloodMoney = new("Blood Money", SkillCategory.Special, 1, BloodRush);
        public static readonly Skill SiphonFunds = new("Siphon Funds", SkillCategory.Special, 1, GoodSamaritan);
        public static readonly Skill TrickleDown = new("Trickle-down Economics", SkillCategory.Special, 1, GoodSamaritan);
        public static readonly Skill InfectiousPersonality = new("Infectious Personality", SkillCategory.Special, 1, GoodSamaritan);
        public static readonly Skill AdrenalineSurge = new("Adrenaline Surge", SkillCategory.Special, 1, GoodSamaritan);
        public static readonly Skill AntiGravityBong = new("Anti-Gravity Bong", SkillCategory.Special, 1, GoodSamaritan);

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
                    LogManager.LogMessage($"Failed to load {field} from file {e}", LogLevel.Warning);
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
