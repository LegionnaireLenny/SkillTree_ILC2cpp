using Il2CppScheduleOne;
using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.Employees;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.Management;
using Il2CppScheduleOne.Money;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.Property;
using Il2CppScheduleOne.Tools;
using Il2CppScheduleOne.UI;
using Il2CppScheduleOne.UI.ATM;
using MelonLoader;
using System.Reflection;
using UnityEngine;

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

            foreach (ItemDefinition item in allItems)
            {
                if (Cache.ItemStack.TryGetValue(item.name, out int baseStackLimit))
                {
                    item.StackLimit = baseStackLimit * SkillModifiers.GetInventoryStackSizeMultiplier();
                    MelonLogger.Msg($"[MoreStackItem] {item.name}: {baseStackLimit} -> {item.StackLimit}");
                }
            }
            MelonLogger.Msg($"Skill Item Stack x{SkillModifiers.GetInventoryStackSizeMultiplier()} Active");
        }
    }
}
