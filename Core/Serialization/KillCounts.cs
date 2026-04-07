using MelonLoader;
using SkillTree.Core.Utilities;
using System.Collections.Generic;
using System.Text.Json;

namespace SkillTree.Core.Serialization
{
    public class KillCounts
    {
        public static int PoliceKilled { get; set; } = 0;
        public static int CartelKilled { get; set; } = 0;
        public static int CivilianKilled { get; set; } = 0;

        public static void LoadFromFile(JsonElement data)
        {
            var properties = typeof(KillCounts).GetProperties();

            foreach (var property in properties)
            {
                try
                {
                    if (int.TryParse(data.GetProperty(property.Name).GetString(), out int result))
                    {
                        property.SetValue(new KillCounts(), result);
                    }
                }
                catch (KeyNotFoundException e)
                {
                    LogManager.LogMessage($"Failed to load {property.Name} from file {e}", LogLevel.Warning);
                    property.SetValue(new KillCounts(), 0);
                }
            }
        }

        public static void LoadDefaultValues()
        {
            foreach (var property in typeof(KillCounts).GetProperties())
            {
                property.SetValue(new KillCounts(), 0);
            }
        }
    }
}
