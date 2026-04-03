using Il2CppScheduleOne;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.ItemFramework;
using MelonLoader;
using SkillTree.Core.Skills;
using System.Collections.Generic;
using System.Linq;

namespace SkillTree.Core.Patches.Enforcer
{
    public static class ItemStackPatches
    {
        public static void SetItemStackSize()
        {
            if (Registry.Instance == null)
                return;

            List<ItemDefinition> allItems = Singleton<Registry>.Instance.GetAllItems()._items.ToList();
            Cache.FillCache(allItems);

            MelonLogger.Msg($"[MoreStackItem] Stack limit multiplier x{SkillModifiers.GetInventoryStackSizeMultiplier()}");
            foreach (ItemDefinition item in allItems)
            {
                if (item.StackLimit <= 1) continue;

                if (Cache.OriginalItemStackSize.TryGetValue(item.name, out int baseStackLimit))
                {
                    item.StackLimit = baseStackLimit * SkillModifiers.GetInventoryStackSizeMultiplier();
                    //MelonLogger.Msg($"[MoreStackItem] {item.name}: {baseStackLimit} -> {item.StackLimit}");
                }
            }
        }
    }
}
