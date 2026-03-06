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

        public static void ConsumeSkillPoint(SkillCategory category)
        {
            switch (category)
            {
                case SkillCategory.Stats:
                    StatsPoints--;
                    break;
                case SkillCategory.Operations:
                    OperationsPoints--;
                    break;
                case SkillCategory.Social:
                    SocialPoints--;
                    break;
                case SkillCategory.Special:
                    SpecialPoints--;
                    break;
            }

            UsedSkillPoints++;
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
                StatsPoints = data.GetProperty("StatsPoints").GetInt32();
                OperationsPoints = data.GetProperty("OperationsPoints").GetInt32();
                SocialPoints = data.GetProperty("SocialPoints").GetInt32();
                SpecialPoints = data.GetProperty("SpecialPoints").GetInt32();
                UsedSkillPoints = data.GetProperty("UsedSkillPoints").GetInt32();

            }
            catch (KeyNotFoundException e) 
            {
                MelonLogger.Warning($"Failed loading skill points from file {e}");
            }
        }
    }
}
