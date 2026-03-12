using HarmonyLib;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.UI;
using MelonLoader;
using SkillTree.Core.Skills;
using UnityEngine;

namespace SkillTree.Core.Patches.Stats
{
    [HarmonyPatch]
    public class PickpocketPatches
    {
        [HarmonyPatch(typeof(PickpocketScreen), "Open")]
        [HarmonyPostfix]
        public static void Patch_Open(PickpocketScreen __instance, NPC _npc)
        {
            if (SkillTreeData.Ghost.CurrentLevel == 0)
            {
                return;
            }

            for (int j = 0; j < __instance.Slots.Length; j++)
            {
                ItemSlotUI itemSlotUI = __instance.Slots[j];
                __instance.SetSlotLocked(j, true);
                if (itemSlotUI.assignedSlot == null || itemSlotUI.assignedSlot.Quantity == 0)
                {
                    __instance.GreenAreas[j].gameObject.SetActive(false);
                }
                else
                {
                    float num = itemSlotUI.assignedSlot.ItemInstance.GetMonetaryValue() * itemSlotUI.assignedSlot.ItemInstance.Definition.TryCast<StorableItemDefinition>().PickpocketDifficultyMultiplier * SkillModifiers.GetPickpocketDifficultyMultiplier();
                    float num2 = Mathf.Lerp(__instance.GreenAreaMaxWidth, __instance.GreenAreaMinWidth, Mathf.Pow(Mathf.Clamp01(num / __instance.ValueDivisor), 0.3f)) / (__instance.npc.Inventory.PickpocketDifficultyMultiplier * SkillModifiers.GetPickpocketDifficultyMultiplier());
                    RectTransform rectTransform = __instance.GreenAreas[j];
                    rectTransform.sizeDelta = new Vector2(num2, rectTransform.sizeDelta.y);
                    rectTransform.gameObject.SetActive(true);
                    rectTransform.anchoredPosition = new Vector2(37.5f + 90f * (float)j, rectTransform.anchoredPosition.y);
                }
            }
        }
    }
}
