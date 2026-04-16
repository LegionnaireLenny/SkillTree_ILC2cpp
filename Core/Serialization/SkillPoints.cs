using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.UI;
using S1API.Leveling;
using SkillTree.Core.Skills;
using SkillTree.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Text.Json;
using static SkillTree.Core.Utilities.LocalizationManager;

namespace SkillTree.Core.Serialization
{
    public class SkillPoints
    {
        public static int EnforcerPoints { get; private set; } = 0;
        public static int ProvisionerPoints { get; private set; } = 0;
        public static int HustlerPoints { get; private set; } = 0;
        public static int LogisticianPoints { get; private set; } = 0;
        public static int SpecialPoints { get; private set; } = 0;
        public static Action OnSkillPointsChanged;

        public static void ConsumeSkillPoints(SkillCategory category, int amount)
        {
            switch (category)
            {
                case SkillCategory.Enforcer:
                    EnforcerPoints -= amount;
                    break;
                case SkillCategory.Provisioner:
                    ProvisionerPoints -= amount;
                    break;
                case SkillCategory.Hustler:
                    HustlerPoints -= amount;
                    break;
                case SkillCategory.Logistician:
                    LogisticianPoints -= amount;
                    break;
                case SkillCategory.Special:
                    SpecialPoints -= amount;
                    break;
            }
            OnSkillPointsChanged?.Invoke();
        }

        public static void AddSkillPoints(int stats, int ops, int social, int logistician, int special)
        {
            EnforcerPoints += stats;
            ProvisionerPoints += ops;
            HustlerPoints += social;
            LogisticianPoints += logistician;
            SpecialPoints += special;
            OnSkillPointsChanged?.Invoke();
        }

        public static void AddSkillPoint(SkillCategory category)
        {
            switch (category)
            {
                case SkillCategory.Enforcer:
                    EnforcerPoints++;
                    LogManager.LogMessage("+1 Enforcer point", LogLevel.Info);
                    Singleton<NotificationsManager>.Instance.SendNotification(
                        GetNotificationTitle("GainSkillPoint", "Enforcer"),
                        string.Format(GetNotificationSubtitle("GainSkillPoint", "Enforcer"), "Enforcer"),
                    IconManager.LoadSprite(IconManager.IconEnforcer));

                    break;
                case SkillCategory.Provisioner:
                    ProvisionerPoints++;
                    LogManager.LogMessage("+1 Provisioner point", LogLevel.Info);
                    Singleton<NotificationsManager>.Instance.SendNotification(
                        GetNotificationTitle("GainSkillPoint", "Provisioner"),
                        string.Format(GetNotificationSubtitle("GainSkillPoint", "Provisioner"), "Provisioner"),
                    IconManager.LoadSprite(IconManager.IconSupplier));
                    break;
                case SkillCategory.Hustler:
                    HustlerPoints++;
                    LogManager.LogMessage("+1 Hustler point", LogLevel.Info);
                    Singleton<NotificationsManager>.Instance.SendNotification(
                        GetNotificationTitle("GainSkillPoint", "Hustler"),
                        string.Format(GetNotificationSubtitle("GainSkillPoint", "Hustler"), "Hustler"),
                    IconManager.LoadSprite(IconManager.IconHustler));
                    break;
                case SkillCategory.Logistician:
                    LogisticianPoints++;
                    LogManager.LogMessage("+1 Logistician point", LogLevel.Info);
                    Singleton<NotificationsManager>.Instance.SendNotification(
                        GetNotificationTitle("GainSkillPoint", "Logistician"),
                        string.Format(GetNotificationSubtitle("GainSkillPoint", "Logistician"), "Logistician"),
                    IconManager.LoadSprite(IconManager.IconLogistician));
                    break;
                case SkillCategory.Special:
                    SpecialPoints++;
                    LogManager.LogMessage("+1 Special point", LogLevel.Info);
                    Singleton<NotificationsManager>.Instance.SendNotification(
                        GetNotificationTitle("GainSkillPoint", "Special"),
                        string.Format(GetNotificationSubtitle("GainSkillPoint", "Special"), "Special"),
                    IconManager.LoadSprite(IconManager.IconSpecial));
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

            LogManager.LogMessage($"Previous Max {previousMaxPoints} | Current Max {currentMaxPoints} | Previous NonSpecial {previousNonSpecialPoints} | Current NonSpecial {currentNonSpecialPoints} | Points Gain {nonSpecialPointsGained} | Special Gained {specialPointsGained}", LogLevel.Debug);

            for (int i = 0; i < nonSpecialPointsGained; i++)
            {
                int mod = (previousNonSpecialPoints + i) % 4;
                switch (mod)
                {
                    case 0:
                        AddSkillPoint(SkillCategory.Enforcer);
                        break;
                    case 1:
                        AddSkillPoint(SkillCategory.Provisioner);
                        break;
                    case 2:
                        AddSkillPoint(SkillCategory.Hustler);
                        break;
                    case 3:
                        AddSkillPoint(SkillCategory.Logistician);
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
                case SkillCategory.Enforcer:
                    return EnforcerPoints > 0;
                case SkillCategory.Provisioner:
                    return ProvisionerPoints > 0;
                case SkillCategory.Hustler:
                    return HustlerPoints > 0;
                case SkillCategory.Logistician:
                    return LogisticianPoints > 0;
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
                case SkillCategory.Enforcer:
                    return EnforcerPoints;
                case SkillCategory.Provisioner:
                    return ProvisionerPoints;
                case SkillCategory.Hustler:
                    return HustlerPoints;
                case SkillCategory.Logistician:
                    return LogisticianPoints;
                case SkillCategory.Special:
                    return SpecialPoints;
                default:
                    return 0;
            }
        }

        public static (int, int, int, int, int) GetExpectedPointTotals()
        {
            int currentRank = (int)LevelManager.Rank;
            int currentTier = LevelManager.Tier - 1;

            int maxPointsPossible = currentRank * 7 + currentTier;
            int nonSpecialPoints = maxPointsPossible - currentRank;

            int stats = 0;
            int operations = 0;
            int social = 0;
            int logistician = 0;
            int special = currentRank;

            for (int i = 0; i < nonSpecialPoints; i++)
            {
                int mod = i % 4;
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
                    case 3:
                        logistician++;
                        break;
                }
            }
            return (stats, operations, social, logistician, special);
        }

        public static void ValidateTotalSkillPoints()
        {
            var (enforcerExpected, provisionerExpected, hustlerExpected, logisticianExpected, specialExpected) = GetExpectedPointTotals();
            var points = SkillTreeData.GetCategoryPointsSpent();
            int enforcerSpent = points[SkillCategory.Enforcer];
            int provisionerSpent = points[SkillCategory.Provisioner];
            int hustlerSpent = points[SkillCategory.Hustler];
            int logisticianSpent = points[SkillCategory.Logistician];
            int specialSpent = points[SkillCategory.Special];

            int expectedTotal = enforcerExpected + provisionerExpected + hustlerExpected + logisticianExpected + specialExpected;
            int pointsSpent = enforcerSpent + provisionerSpent + hustlerSpent + logisticianSpent + specialSpent;
            int pointsRemaining = EnforcerPoints + ProvisionerPoints + HustlerPoints + LogisticianPoints + SpecialPoints;

            LogManager.LogMessage($"Expected: Enforcer {enforcerExpected} | Provisioner {provisionerExpected} | Hustler {hustlerExpected} | Logistician {logisticianExpected} | Special {specialExpected} | Total {expectedTotal}", LogLevel.Debug);
            LogManager.LogMessage($"Spent:    Enforcer {enforcerSpent} | Provisioner {provisionerSpent} | Hustler {hustlerSpent} | Logistician {logisticianSpent} | Special {specialSpent} | Total {pointsSpent}", LogLevel.Debug);
            LogManager.LogMessage($"Left:     Enforcer {EnforcerPoints} | Provisioner {ProvisionerPoints} | Hustler {HustlerPoints} | Logistician {LogisticianPoints} | Special {SpecialPoints} | Total {pointsRemaining}", LogLevel.Debug);

            if (expectedTotal < pointsSpent + pointsRemaining)
            {
                LogManager.LogMessage($"Current character is below the expected level for this save file or save file is corrupt, resetting save data. Expected Total: {expectedTotal} | Actual Total: {pointsSpent + pointsRemaining}.", LogLevel.Warning);
                SaveManager.LoadDefaultValues();
                ValidateTotalSkillPoints();
                return;
            }

            int missingEnforcer = enforcerExpected - (enforcerSpent + EnforcerPoints);
            int missingProvisioner = provisionerExpected - (provisionerSpent + ProvisionerPoints);
            int missingHustler = hustlerExpected - (hustlerSpent + HustlerPoints);
            int missingLogistician = logisticianExpected - (logisticianSpent + LogisticianPoints);
            int missingSpecial = specialExpected - (specialSpent + SpecialPoints);

            if (missingEnforcer != 0)
            {
                LogManager.LogMessage($"Adjusting Enforcer points by {missingEnforcer}", LogLevel.Warning);
            }
            if (missingProvisioner != 0)
            {
                LogManager.LogMessage($"Adjusting Supplier points by {missingProvisioner}", LogLevel.Warning);
            }
            if (missingHustler != 0)
            {
                LogManager.LogMessage($"Adjusting Hustler points by {missingHustler}", LogLevel.Warning);
            }
            if (missingLogistician != 0)
            {
                LogManager.LogMessage($"Adjusting Logistician points by {missingLogistician}", LogLevel.Warning);
            }
            if (missingSpecial != 0)
            {
                LogManager.LogMessage($"Adjusting Special points by {missingSpecial}", LogLevel.Warning);
            }
            AddSkillPoints(missingEnforcer, missingProvisioner, missingHustler, missingLogistician, missingSpecial);
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
                    LogManager.LogMessage($"Failed to load {property.Name} from file {e}", LogLevel.Warning);
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
