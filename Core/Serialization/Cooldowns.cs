using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.UI;
using MelonLoader;
using SkillTree.Core.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using UnityEngine;
using static SkillTree.Core.Utilities.ConfigManager;
using static SkillTree.Core.Utilities.LocalizationManager;

namespace SkillTree.Core.Serialization
{
    public class Cooldowns
    {
        public static bool GoodSamaritanUsed { get; set; } = false;
        public static bool BloodRushUsed { get; set; } = false;
        public static bool BloodMoneyUsed { get; set; } = false;
        public static bool SiphonFundsUsed { get; set; } = false;
        public static bool TrickleDownUsed { get; set; } = false;
        public static bool InfectiousPersonalityUsed { get; set; } = false;
        public static int AdrenalineSurgeRemainingCharges { get; set; } = AdrenalineSurgeMaxCharges.GetValue(UseDefault.GetValue());
        public static bool AntiGravityBongUsed { get; set; } = false;
        public static bool CircadianMasteryUsed { get; set; } = false;

        public static void ResetDailySkills()
        {
            GoodSamaritanUsed = false;
            BloodRushUsed = false;
            BloodMoneyUsed = false;
            SiphonFundsUsed = false;
            TrickleDownUsed = false;
            InfectiousPersonalityUsed = false;
            CircadianMasteryUsed = false;
            AdrenalineSurgeRemainingCharges = AdrenalineSurgeMaxCharges.GetValue(UseDefault.GetValue());
            Singleton<NotificationsManager>.Instance?.SendNotification(
                GetNotificationTitle("CooldownReset", "DailySkills"),
                GetNotificationSubtitle("CooldownReset", "DailySkills"),
                IconManager.LoadSprite(IconManager.IconClock));
        }

        public static IEnumerator ResetAntiGravityBong()
        {
            yield return new WaitForSeconds(AntiGravityBongCooldown.GetValue(UseDefault.GetValue()));
            AntiGravityBongUsed = false;
            Singleton<NotificationsManager>.Instance.SendNotification(
                GetNotificationTitle("CooldownReset", "AntiGravityBong"),
                GetNotificationSubtitle("CooldownReset", "AntiGravityBong"),
                IconManager.LoadSprite(IconManager.IconClock));
            Core.RemoveCoroutine("ResetAntiGravityBong");
        }

        public static void LoadFromFile(JsonElement data)
        {
            var properties = typeof(Cooldowns).GetProperties();

            foreach (var property in properties)
            {
                try
                {
                    if (bool.TryParse(data.GetProperty(property.Name).GetString(), out bool resultBool))
                    {
                        property.SetValue(new Cooldowns(), resultBool);
                    }
                    else if (int.TryParse(data.GetProperty(property.Name).GetString(), out int resultInt))
                    {
                        property.SetValue(new Cooldowns(), resultInt);
                    }
                }
                catch (KeyNotFoundException e)
                {
                    LogManager.LogMessage($"Failed to load {property.Name} from file {e}", LogLevel.Warning);
                    property.SetValue(new Cooldowns(), false);
                }
            }
        }

        public static void LoadDefaultValues()
        {
            foreach (var property in typeof(Cooldowns).GetProperties())
            {
                if (property.PropertyType.GetType() == typeof(bool))
                {
                    property.SetValue(new Cooldowns(), false);
                }
                else if (property.PropertyType.GetType() == typeof(int))
                {
                    property.SetValue(new Cooldowns(), AdrenalineSurgeMaxCharges.GetValue(UseDefault.GetValue()));
                }
            }
        }
    }
}
