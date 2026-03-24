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
                    MelonLogger.Msg("+1 Enforcer point");
                    Singleton<NotificationsManager>.Instance.SendNotification(
                    "Level Up",
                    $"<color=#16F01C>+1 Enforcer point</color>", IconManager.LoadSprite(IconManager.IconEnforcer));

                    break;
                case SkillCategory.Provisioner:
                    ProvisionerPoints++;
                    MelonLogger.Msg("+1 Supplier point");
                    Singleton<NotificationsManager>.Instance.SendNotification(
                    "Level Up",
                    $"<color=#16F01C>+1 Supplier point</color>", IconManager.LoadSprite(IconManager.IconSupplier));
                    break;
                case SkillCategory.Hustler:
                    HustlerPoints++;
                    MelonLogger.Msg("+1 Hustler point");
                    Singleton<NotificationsManager>.Instance.SendNotification(
                    "Level Up",
                    $"<color=#16F01C>+1 Hustler point</color>", IconManager.LoadSprite(IconManager.IconHustler));
                    break;
                case SkillCategory.Logistician:
                    LogisticianPoints++;
                    MelonLogger.Msg("+1 Logistician point");
                    Singleton<NotificationsManager>.Instance.SendNotification(
                    "Level Up",
                    $"<color=#16F01C>+1 Logistician point</color>", IconManager.LoadSprite(IconManager.IconLogistician));
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
                int mod = (previousNonSpecialPoints + i) % 4;
                //MelonLogger.Msg($"Mod {mod}");
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

            MelonLogger.Msg($"Expected: Enforcer {enforcerExpected} | Provisioner {provisionerExpected} | Hustler {hustlerExpected} | Logistician {logisticianExpected} | Special {specialExpected} | Total {expectedTotal}");
            MelonLogger.Msg($"Spent:    Enforcer {enforcerSpent} | Provisioner {provisionerSpent} | Hustler {hustlerSpent} | Logistician {logisticianSpent} | Special {specialSpent} | Total {pointsSpent}");
            MelonLogger.Msg($"Left:     Enforcer {EnforcerPoints} | Provisioner {ProvisionerPoints} | Hustler {HustlerPoints} | Logistician {LogisticianPoints} | Special {SpecialPoints} | Total {pointsRemaining}");

            if (expectedTotal < pointsSpent + pointsRemaining)
            {
                MelonLogger.Warning($"Current character is below the expected level for this save file or save file is corrupt, resetting save data. Expected Total: {expectedTotal} | Actual Total: {pointsSpent + pointsRemaining}.");
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
                MelonLogger.Warning($"Adjusting Enforcer points by {missingEnforcer}");
            }
            if (missingProvisioner != 0)
            {
                MelonLogger.Warning($"Adjusting Supplier points by {missingProvisioner}");
            }
            if (missingHustler != 0)
            {
                MelonLogger.Warning($"Adjusting Hustler points by {missingHustler}");
            }
            if (missingLogistician != 0)
            {
                MelonLogger.Warning($"Adjusting Logistician points by {missingLogistician}");
            }
            if (missingSpecial != 0)
            {
                MelonLogger.Warning($"Adjusting Special points by {missingSpecial}");
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
