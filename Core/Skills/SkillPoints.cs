using MelonLoader;
using System.Collections.Generic;
using System.Text.Json;

namespace SkillTree.Core.Skills
{
    public static class SkillPoints
    {
        public static int StatsPoints { get; private set; } = 0;
        public static int OperationsPoints { get; private set; } = 0;
        public static int SocialPoints { get; private set; } = 0;
        public static int SpecialPoints { get; private set; } = 0;
        public static int UsedSkillPoints { get; private set; } = 0;

        public static void ConsumeSkillPoints(SkillCategory category, int amount)
        {
            switch (category)
            {
                case SkillCategory.Stats:
                    StatsPoints -= amount;
                    break;
                case SkillCategory.Operations:
                    OperationsPoints -= amount;
                    break;
                case SkillCategory.Social:
                    SocialPoints -= amount;
                    break;
                case SkillCategory.Special:
                    SpecialPoints -= amount;
                    break;
            }

            UsedSkillPoints += amount;
        }

        public static void AddSkillPoints(int stats, int ops, int social, int special)
        {
            StatsPoints += stats;
            OperationsPoints += ops;
            SocialPoints += social;
            SpecialPoints += special;
        }

        public static bool ArePointsAvailable(SkillCategory category)
        {
            switch (category)
            {
                case SkillCategory.Stats:
                    return StatsPoints > 0;
                case SkillCategory.Operations:
                    return OperationsPoints > 0;
                case SkillCategory.Social:
                    return SocialPoints > 0;
                case SkillCategory.Special:
                    return SpecialPoints > 0;
                default:
                    return false;
            }
        }

        public static Dictionary<string, int> GetSaveData()
        {
            Dictionary<string, int> skillData = new()
            {
                ["StatsPoints"] = StatsPoints,
                ["OperationsPoints"] = OperationsPoints,
                ["SocialPoints"] = SocialPoints,
                ["SpecialPoints"] = SpecialPoints,
                ["UsedSkillPoints"] = UsedSkillPoints
            };

            return skillData;
        }

        public static Dictionary<string, int> GetDefaultSaveData()
        {
            Dictionary<string, int> skillData = new()
            {
                ["StatsPoints"] = 0,
                ["OperationsPoints"] = 0,
                ["SocialPoints"] = 0,
                ["SpecialPoints"] = 0,
                ["UsedSkillPoints"] = 0
            };

            return skillData;
        }

        public static void LoadFromFile(JsonElement data)
        {
            try
            {
                StatsPoints = data.GetProperty(nameof(StatsPoints)).GetInt32();
                OperationsPoints = data.GetProperty(nameof(OperationsPoints)).GetInt32();
                SocialPoints = data.GetProperty(nameof(SocialPoints)).GetInt32();
                SpecialPoints = data.GetProperty(nameof(SpecialPoints)).GetInt32();
                UsedSkillPoints = data.GetProperty(nameof(UsedSkillPoints)).GetInt32();

            }
            catch (KeyNotFoundException e) 
            {
                throw new KeyNotFoundException($"Failed to load skill points from file {e}");
            }
        }
        public static void LoadDefaultValues()
        {
            StatsPoints = 0;
            OperationsPoints = 0;
            SocialPoints = 0;
            SpecialPoints = 0;
            UsedSkillPoints = 0;
        }
    }
}
