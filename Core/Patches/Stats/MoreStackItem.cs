using Il2CppScheduleOne;
using Il2CppScheduleOne.ItemFramework;
using MelonLoader;

namespace SkillTree.Core.Patches.Stats
{
    public static class MoreStackItem
    {
        public static void SetItemStackSize()
        {
            if (Registry.Instance == null || Core.SkillData == null || Core.SkillData.MoreStackItem == 0)
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
