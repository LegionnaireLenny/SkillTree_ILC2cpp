using Il2CppScheduleOne;
using Il2CppScheduleOne.ItemFramework;
using MelonLoader;

namespace SkillTree.Core.Patches.Stats
{
    public static class MoreStackItem
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

        public static void SetItemStackSize()
        {
            if (Registry.Instance == null || Core.SkillData == null || Core.SkillData.MoreStackItem == 0)
                return;

            Il2CppSystem.Collections.Generic.List<ItemDefinition> allItems = Registry.Instance.GetAllItems();
            StackCache.FillCache(allItems);

            foreach (ItemDefinition item in allItems)
            {
                if (StackCache.ItemStack.TryGetValue(item.name, out int baseStackLimit))
                {
                    item.StackLimit = baseStackLimit * SkillModifiers.GetInventoryStackSizeMultiplier();
                    MelonLogger.Msg($"[MoreStackItem] {item.name}: {baseStackLimit} -> {item.StackLimit}");
                }
            }
            MelonLogger.Msg($"Skill Item Stack x{SkillModifiers.GetInventoryStackSizeMultiplier()} Active");
        }
    }
}
