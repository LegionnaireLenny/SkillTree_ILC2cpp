using Il2CppScheduleOne.Economy;
using MelonLoader;

namespace SkillTree.Core.StateManagement
{
    public static class CustomerCache
    {
        private static bool IsLoaded = false;
        public static readonly Dictionary<string, float> OriginalMinSpend = [];
        public static readonly Dictionary<string, float> OriginalMaxSpend = [];

        public static void FillCache(List<Customer> customers)
        {
            if (IsLoaded) 
                return; 

            foreach (var c in customers)
            {
                string key = c.CustomerData.name;
                if (!OriginalMinSpend.ContainsKey(key))
                {
                    OriginalMinSpend.Add(key, c.CustomerData.MinWeeklySpend);
                    OriginalMaxSpend.Add(key, c.CustomerData.MaxWeeklySpend);
                }
            }
            IsLoaded = true;
            MelonLogger.Msg("Customer spending history successfully stored!");
        }

        public static void ClearCache()
        {
            IsLoaded = false;
            OriginalMinSpend.Clear();
            OriginalMaxSpend.Clear();
        }
    }
}
