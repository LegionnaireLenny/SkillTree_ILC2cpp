using MelonLoader;
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
                    int value = data.GetProperty(property.Name).ValueKind == JsonValueKind.String ? int.Parse(data.GetProperty(property.Name).GetString()) : data.GetProperty(property.Name).GetInt32();
                    property.SetValue(new KillCounts(), value);
                }
                catch (KeyNotFoundException e)
                {
                    MelonLogger.Warning($"Failed to load {property.Name} from file {e}");
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
