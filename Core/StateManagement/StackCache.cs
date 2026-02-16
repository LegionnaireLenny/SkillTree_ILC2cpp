using Il2CppScheduleOne.ItemFramework;
using MelonLoader;

namespace SkillTree.Core.StateManagement
{
    public static class StackCache
    {
        public static readonly Dictionary<string, int> ItemStack = [];

        public static void FillCache(Il2CppSystem.Collections.Generic.List<ItemDefinition> items)
        {
            foreach (ItemDefinition item in items)
            {
                if (!ItemStack.ContainsKey(item.name))
                {
                    ItemStack.Add(item.name, item.StackLimit);
                }
            }
            MelonLogger.Msg("ItemStack Memory successfully stored!");
        }

        public static void ClearCache()
        {
            ItemStack.Clear();
        }
    }
}
