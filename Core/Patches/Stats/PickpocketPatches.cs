using HarmonyLib;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.UI;
using MelonLoader;
using SkillTree.Core.Serialization;
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
            if (__instance == null || SkillTreeData.Ghost.CurrentLevel == 0)
            {
                return;
            }

            for (int j = 0; j < __instance.Slots.Length; j++)
            {
                ItemSlotUI itemSlotUI = __instance.Slots[j];
                __instance.SetSlotLocked(j, __instance.npc.IsConscious);
                if (itemSlotUI.assignedSlot == null || itemSlotUI.assignedSlot.Quantity == 0)
                {
                    __instance.GreenAreas[j].gameObject.SetActive(false);
                }
                else
                {
                    //MelonLogger.Msg($"Inventory Difficulty: {Mathf.Clamp(__instance.npc.Inventory.PickpocketDifficultyMultiplier, 0, 2)} | Item Difficulty: {itemSlotUI.assignedSlot.ItemInstance.Definition.TryCast<StorableItemDefinition>().PickpocketDifficultyMultiplier}");
                    float num = itemSlotUI.assignedSlot.ItemInstance.GetMonetaryValue() * itemSlotUI.assignedSlot.ItemInstance.Definition.TryCast<StorableItemDefinition>().PickpocketDifficultyMultiplier;
                    float num2 = Mathf.Lerp(__instance.GreenAreaMaxWidth, 
                                            __instance.GreenAreaMinWidth, 
                                            Mathf.Pow(Mathf.Clamp01(num / __instance.ValueDivisor), 0.3f)) / 
                                        (Mathf.Clamp(__instance.npc.Inventory.PickpocketDifficultyMultiplier, 0, 2) * SkillModifiers.GetPickpocketDifficultyMultiplier());
                    RectTransform rectTransform = __instance.GreenAreas[j];
                    rectTransform.sizeDelta = new Vector2(num2, rectTransform.sizeDelta.y);
                    rectTransform.gameObject.SetActive(true);
                    rectTransform.anchoredPosition = new Vector2(37.5f + 90f * (float)j, rectTransform.anchoredPosition.y);
                }
            }
        }

        public static void SetPickpockDifficulty()
        {
            float originalSlide = Singleton<PickpocketScreen>.instance.SlideTimeMaxMultiplier;
            float originalMin = Singleton<PickpocketScreen>.instance.GreenAreaMinWidth;
            Singleton<PickpocketScreen>.instance.SlideTimeMaxMultiplier *= SkillModifiers.GetPickpocketDifficultyMultiplier();
            Singleton<PickpocketScreen>.instance.GreenAreaMinWidth = SkillModifiers.GetPickpocketMinimumWidth(Singleton<PickpocketScreen>.instance.GreenAreaMinWidth);
            MelonLogger.Msg($"SlideTimeMaxMultiplier {originalSlide} -> {Singleton<PickpocketScreen>.instance.SlideTimeMaxMultiplier} | GreenMin {originalMin} -> {Singleton<PickpocketScreen>.instance.GreenAreaMinWidth}");
        }
    }
}
