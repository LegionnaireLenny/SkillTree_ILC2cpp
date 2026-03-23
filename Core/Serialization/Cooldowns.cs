using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.UI;
using MelonLoader;
using SkillTree.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SkillTree.Core.Serialization
{
    public class Cooldowns
    {
        public static bool GoodSamaritanUsed { get; set; } = false;
        public static bool BloodRushUsed { get; set; } = false;
        public static bool SiphonFundsUsed { get; set; } = false;
        public static bool TrickleDownUsed { get; set; } = false;
        public static bool CircadianMasteryUsed { get; set; } = false;

        public static void ResetSkillCooldowns()
        {
            GoodSamaritanUsed = false;
            BloodRushUsed = false;
            SiphonFundsUsed = false;
            TrickleDownUsed = false;
            CircadianMasteryUsed = false;
            Singleton<NotificationsManager>.Instance?.SendNotification(
                "A New Day Dawns",
                "Cooldowns Reset",
                IconManager.LoadSprite(IconManager.IconClock));
        }

        public static void LoadFromFile(JsonElement data)
        {
            var properties = typeof(Cooldowns).GetProperties();

            foreach (var property in properties)
            {
                try
                {
                    bool value = data.GetProperty(property.Name).ValueKind == JsonValueKind.String ? bool.Parse(data.GetProperty(property.Name).GetString()) : data.GetProperty(property.Name).GetBoolean();
                    property.SetValue(new Cooldowns(), value);
                }
                catch (KeyNotFoundException e)
                {
                    MelonLogger.Warning($"Failed to load {property.Name} from file {e}");
                    property.SetValue(new Cooldowns(), false);
                }
            }
        }

        public static void LoadDefaultValues()
        {
            foreach (var property in typeof(Cooldowns).GetProperties())
            {
                property.SetValue(new Cooldowns(), false);
            }
        }
    }
}
