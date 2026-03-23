using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.UI;
using MelonLoader;
using S1API.Leveling;
using SkillTree.Core.Skills;
using SkillTree.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace SkillTree.Core.Serialization
{
    public class SkillPoints
    {
        public static int StatsPoints { get; private set; } = 0;
        public static int OperationsPoints { get; private set; } = 0;
        public static int SocialPoints { get; private set; } = 0;
        public static int SpecialPoints { get; private set; } = 0;
        public static Action OnSkillPointsChanged;

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
            OnSkillPointsChanged?.Invoke();
        }

        public static void AddSkillPoints(int stats, int ops, int social, int special)
        {
            StatsPoints += stats;
            OperationsPoints += ops;
            SocialPoints += social;
            SpecialPoints += special;
            OnSkillPointsChanged?.Invoke();
        }

        public static void AddSkillPoint(SkillCategory category)
        {
            switch (category)
            {
                case SkillCategory.Stats:
                    StatsPoints++;
                    MelonLogger.Msg("+1 Stats point");
                    Singleton<NotificationsManager>.Instance.SendNotification(
                    "Level Up",
                    $"<color=#16F01C>+1 Stats point</color>", IconManager.LoadSprite(IconManager.IconStats));

                    break;
                case SkillCategory.Operations:
                    OperationsPoints++;
                    MelonLogger.Msg("+1 Operations point");
                    Singleton<NotificationsManager>.Instance.SendNotification(
                    "Level Up",
                    $"<color=#16F01C>+1 Operations point</color>", IconManager.LoadSprite(IconManager.IconOperations));
                    break;
                case SkillCategory.Social:
                    SocialPoints++;
                    MelonLogger.Msg("+1 Social point");
                    Singleton<NotificationsManager>.Instance.SendNotification(
                    "Level Up",
                    $"<color=#16F01C>+1 Social point</color>", IconManager.LoadSprite(IconManager.IconSocial));
                    break;
                case SkillCategory.Special:
                    SpecialPoints++;
                    MelonLogger.Msg("+1 Special point");
                    Singleton<NotificationsManager>.Instance.SendNotification(
                    "Level Up",
                    $"<color=#16F01C>+1 Special point</color>", IconManager.LoadSprite(IconManager.IconSpecial));
                    break;
            }
            OnSkillPointsChanged?.Invoke();
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

        public static (int, int, int, int) GetExpectedPointTotals()
        {
            int currentRank = (int)LevelManager.Rank;
            int currentTier = LevelManager.Tier - 1;

            int maxPointsPossible = currentRank * 7 + currentTier;
            int nonSpecialPoints = maxPointsPossible - currentRank;

            int stats = 0;
            int operations = 0;
            int social = 0;
            int special = currentRank;

            for (int i = 0; i < nonSpecialPoints; i++)
            {
                int mod = i % 3;
                switch (mod)
                {
                    case 0:
                        stats++;
                        break;
                    case 1:
                        operations++;
                        break;
                    case 2:
                        social++;
                        break;
                }
            }
            return (stats, operations, social, special);
        }

        public static void ValidateTotalSkillPoints()
        {
            var (statsExpected, operationsExpected, socialExpected, specialExpected) = GetExpectedPointTotals();
            var points = SkillTreeData.GetCategoryPointsSpent();
            int statsSpent = points[SkillCategory.Stats];
            int operationsSpent = points[SkillCategory.Operations];
            int socialSpent = points[SkillCategory.Social];
            int specialSpent = points[SkillCategory.Special];

            int expectedTotal = statsExpected + operationsExpected + socialExpected + specialExpected;
            int pointsSpent = statsSpent + operationsSpent + socialSpent + specialSpent;
            int pointsRemaining = StatsPoints + OperationsPoints + SocialPoints + SpecialPoints;

            MelonLogger.Msg($"Expected: Stats {statsExpected} | Operations {operationsExpected} | Social {socialExpected} | Special {specialExpected} | Total {expectedTotal}");
            MelonLogger.Msg($"Spent:    Stats {statsSpent} | Operations {operationsSpent} | Social {socialSpent} | Special {specialSpent} | Total {pointsSpent}");
            MelonLogger.Msg($"Left:     Stats {StatsPoints} | Operations {OperationsPoints} | Social {SocialPoints} | Special {SpecialPoints} | Total {pointsRemaining}");

            if (expectedTotal < pointsSpent + pointsRemaining)
            {
                MelonLogger.Warning($"Current character is below the expected level for this save file or save file is corrupt, resetting save data. Expected Total: {expectedTotal} | Actual Total: {pointsSpent + pointsRemaining}.");
                SaveManager.LoadDefaultValues();
                ValidateTotalSkillPoints();
                return;
            }

            int missingStats = statsExpected - (statsSpent + StatsPoints);
            int missingOperations = operationsExpected - (operationsSpent + OperationsPoints);
            int missingSocial = socialExpected - (socialSpent + SocialPoints);
            int missingSpecial = specialExpected - (specialSpent + SpecialPoints);

            if (missingStats != 0)
            {
                MelonLogger.Warning($"Adjusting Stats points by {missingStats}");
            }
            if (missingOperations != 0)
            {
                MelonLogger.Warning($"Adjusting Operations points by {missingOperations}");
            }
            if (missingSocial != 0)
            {
                MelonLogger.Warning($"Adjusting Social points by {missingSocial}");
            }
            if (missingSpecial != 0)
            {
                MelonLogger.Warning($"Adjusting Special points by {missingSpecial}");
            }
            AddSkillPoints(missingStats, missingOperations, missingSocial, missingSpecial);
        }

        public static void LoadFromFile(JsonElement data)
        {
            var properties = typeof(SkillPoints).GetProperties();

            foreach (var property in properties)
            {
                try
                {
                    int value = data.GetProperty(property.Name).ValueKind == JsonValueKind.String ? int.Parse(data.GetProperty(property.Name).GetString()) : data.GetProperty(property.Name).GetInt32();
                    property.SetValue(new SkillPoints(), value);
                }
                catch (KeyNotFoundException e)
                {
                    MelonLogger.Warning($"Failed to load {property.Name} from file {e}");
                    property.SetValue(new SkillPoints(), 0);
                }
            }
        }

        public static void LoadDefaultValues()
        {
            foreach (var property in typeof(SkillPoints).GetProperties())
            {
                property.SetValue(new SkillPoints(), 0);
            }

        }
    }
}
