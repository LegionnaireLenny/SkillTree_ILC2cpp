using Il2CppScheduleOne.Property;
using MelonLoader;

namespace SkillTree.Core.StateManagement
{
    public static class BusinessCache
    {
        public static Dictionary<string, float> LaunderCapacity = new Dictionary<string, float>();
        public static bool IsLoaded = false;

        public static void FillCache(List<Business> business)
        {
            if (IsLoaded) return; 

            foreach (var c in business)
            {
                string key = c.PropertyName;
                if (!LaunderCapacity.ContainsKey(key))
                    LaunderCapacity.Add(key, c.LaunderCapacity);
            }
            IsLoaded = true;
            MelonLogger.Msg("Business Laundering Memory successfully stored!");
        }
    }
}
