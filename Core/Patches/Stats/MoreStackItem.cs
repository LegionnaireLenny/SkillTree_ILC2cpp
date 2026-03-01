using Il2CppScheduleOne;
using Il2CppScheduleOne.ItemFramework;
using MelonLoader;
using SkillTree.Core.FileManagement;

namespace SkillTree.Core.Patches.Stats
{
    public static class MoreStackItem
    {
        public static void SetItemStackSize()
        {
            if (Registry.Instance == null || SkillTreeData.MoreStackItem.CurrentLevel == 0)
                return;

            Il2CppSystem.Collections.Generic.List<ItemDefinition> allItems = Registry.Instance.GetAllItems();
            Cache.FillCache(allItems);

            MelonLogger.Msg($"[MoreStackItem] Increasing item stack by x{SkillModifiers.GetInventoryStackSizeMultiplier()}");
            foreach (ItemDefinition item in allItems)
            {
                if (Cache.OriginalItemStackSize.TryGetValue(item.name, out int baseStackLimit))
                {
                    item.StackLimit = baseStackLimit * SkillModifiers.GetInventoryStackSizeMultiplier();
                    //MelonLogger.Msg($"[MoreStackItem] {item.name}: {baseStackLimit} -> {item.StackLimit}");
                }
            }
        }
    }
}
