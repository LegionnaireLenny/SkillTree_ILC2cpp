using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Money;
using Il2CppScheduleOne.UI;
using MelonLoader;
using S1API.Leveling;
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
        }

        public static void AddSkillPoints(int stats, int ops, int social, int special)
        {
            StatsPoints += stats;
            OperationsPoints += ops;
            SocialPoints += social;
            SpecialPoints += special;
        }

        public static void AddSkillPoint(SkillCategory category)
        {
            switch (category)
            {
                case SkillCategory.Stats:
                    StatsPoints++;
                    MelonLogger.Msg("Gained 1 Stats point");
                    Singleton<NotificationsManager>.Instance.SendNotification(
                    "Level Up",
                    $"<color=#16F01C>Gained 1 Stats point</color>", NetworkSingleton<MoneyManager>.Instance.LaunderingNotificationIcon);

                    break;
                case SkillCategory.Operations:
                    OperationsPoints++;
                    MelonLogger.Msg("Gained 1 Operations point");
                    Singleton<NotificationsManager>.Instance.SendNotification(
                    "Level Up",
                    $"<color=#16F01C>Gained 1 Operations point</color>", NetworkSingleton<MoneyManager>.Instance.LaunderingNotificationIcon);
                    break;
                case SkillCategory.Social:
                    SocialPoints++;
                    MelonLogger.Msg("Gained 1 Social point");
                    Singleton<NotificationsManager>.Instance.SendNotification(
                    "Level Up",
                    $"<color=#16F01C>Gained 1 Social point</color>", NetworkSingleton<MoneyManager>.Instance.LaunderingNotificationIcon);
                    break;
                case SkillCategory.Special:
                    SpecialPoints++;
                    MelonLogger.Msg("Gained 1 Special point");
                    Singleton<NotificationsManager>.Instance.SendNotification(
                    "Level Up",
                    $"<color=#16F01C>Gained 1 Special point</color>", NetworkSingleton<MoneyManager>.Instance.LaunderingNotificationIcon);
                    break;
            }
        }

        public static void ProcessLevelUp(FullRank previousRank, FullRank currentRank)
        {
            if (previousRank == currentRank || previousRank > currentRank) return;

            int previousMaxPoints = (int)previousRank.Rank * 7 + (previousRank.Tier - 1);
            int previousNonSpecialPoints = previousMaxPoints - (int)previousRank.Rank;

            int currentMaxPoints = (int)currentRank.Rank * 7 + (currentRank.Tier - 1);
            int currentNonSpecialPoints = currentMaxPoints - (int)currentRank.Rank;

            int nonSpecialPointsGained = currentNonSpecialPoints - previousNonSpecialPoints;
            int specialPointsGained = (int)currentRank.Rank - (int)previousRank.Rank;

            //MelonLogger.Msg($"Previous Max {previousMaxPoints} | Current Max {currentMaxPoints} | Previous NonSpecial {previousNonSpecialPoints} | Current NonSpecial {currentNonSpecialPoints} | Points Gain {nonSpecialPointsGained} | Special Gained {specialPointsGained}");

            for (int i = 0; i < nonSpecialPointsGained; i++)
            {
                int mod = (previousNonSpecialPoints + i) % 3;
                //MelonLogger.Msg($"Mod {mod}");
                switch (mod)
                {
                    case 0:
                        AddSkillPoint(SkillCategory.Stats);
                        break;
                    case 1:
                        AddSkillPoint(SkillCategory.Operations);
                        break;
                    case 2:
                        AddSkillPoint(SkillCategory.Social);
                        break;
                }
            }

            for (int i = 0; i < specialPointsGained; i++)
            {
                AddSkillPoint(SkillCategory.Special);
            }
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

        public static int GetPointsAvailable(SkillCategory category)
        {
            switch (category)
            {
                case SkillCategory.Stats:
                    return StatsPoints;
                case SkillCategory.Operations:
                    return OperationsPoints;
                case SkillCategory.Social:
                    return SocialPoints;
                case SkillCategory.Special:
                    return SpecialPoints;
                default:
                    return 0;
            }
        }

        public static Dictionary<string, int> GetSaveData()
        {
            Dictionary<string, int> skillData = new()
            {
                ["StatsPoints"] = StatsPoints,
                ["OperationsPoints"] = OperationsPoints,
                ["SocialPoints"] = SocialPoints,
                ["SpecialPoints"] = SpecialPoints
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
        }
    }
}
