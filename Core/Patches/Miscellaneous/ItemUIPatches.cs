using HarmonyLib;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Equipping;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.UI.Items;
using MelonLoader;
using UnityEngine;

namespace SkillTree.Core.Patches.Miscellaneous
{
    [HarmonyPatch]
    public class ItemUIPatches
    {
        [HarmonyPatch(typeof(ItemUIManager), "EndDrag")]
        [HarmonyPrefix]
        public static bool Patch_ItemSlot_CanStackWith(ItemUIManager __instance)
        {
            if (__instance.isDraggingCash)
            {
                __instance.EndCashDrag();
                return false;
            }

            if (__instance.CanDragFromSlot(__instance.draggedSlot) &&
                __instance.HoveredSlot != null &&
                __instance.HoveredSlot != __instance.draggedSlot &&
                __instance.HoveredSlot.assignedSlot != null &&
                !__instance.HoveredSlot.assignedSlot.IsLocked &&
                !__instance.HoveredSlot.assignedSlot.IsAddLocked &&
                __instance.HoveredSlot.assignedSlot.DoesItemMatchHardFilters(__instance.draggedSlot.assignedSlot.ItemInstance))
            {
                if (__instance.HoveredSlot.assignedSlot.ItemInstance == null)
                {
                    __instance.HoveredSlot.assignedSlot.SetStoredItem(__instance.draggedSlot.assignedSlot.ItemInstance.GetCopy(__instance.draggedAmount), false);
                    __instance.draggedSlot.assignedSlot.ChangeQuantity(-__instance.draggedAmount, false);
                }
                else if (__instance.HoveredSlot.assignedSlot.ItemInstance.CanStackWith(__instance.draggedSlot.assignedSlot.ItemInstance, false))
                {
                    if (__instance.HoveredSlot.assignedSlot.ItemInstance.StackLimit == 1)
                    {
                        try
                        {
                            IntegerItemInstance hoveredItem = __instance.HoveredSlot.assignedSlot.ItemInstance.Cast<IntegerItemInstance>();
                            IntegerItemInstance draggedItem = __instance.draggedSlot.assignedSlot.ItemInstance.Cast<IntegerItemInstance>();

                            int missingValue = hoveredItem._definition.Cast<IntegerItemDefinition>().DefaultValue - hoveredItem.Value;
                            if (missingValue > 0)
                            {
                                int amountToChange = Mathf.Min(missingValue, draggedItem.Value);

                                __instance.HoveredSlot.assignedSlot.ItemInstance.Cast<IntegerItemInstance>().ChangeValue(amountToChange);
                                if (amountToChange >= draggedItem.Value && __instance.draggedSlot.assignedSlot.ItemInstance.Equippable.TryCast<Equippable_RangedWeapon>() == null)
                                {
                                    __instance.draggedSlot.assignedSlot.ClearStoredInstance(false);
                                }
                                else
                                {
                                    __instance.draggedSlot.assignedSlot.ItemInstance.Cast<IntegerItemInstance>().ChangeValue(-amountToChange);
                                }
                            }
                        }
                        catch (System.Exception)
                        {
                            MelonLogger.Msg("Failed to stack items with same ID");
                        }
                    }
                    else
                    {
                        int amountToMove = Mathf.Min(__instance.HoveredSlot.assignedSlot.ItemInstance.StackLimit - __instance.HoveredSlot.assignedSlot.Quantity, __instance.draggedAmount);
                        if (amountToMove > 0)
                        {
                            __instance.HoveredSlot.assignedSlot.ChangeQuantity(amountToMove, false);
                            __instance.draggedSlot.assignedSlot.ChangeQuantity(-amountToMove, false);
                            __instance.draggedAmount -= amountToMove;
                        }
                    }
                }
                else if (__instance.draggedSlot.assignedSlot.DoesItemMatchHardFilters(__instance.HoveredSlot.assignedSlot.ItemInstance))
                {
                    if (__instance.draggedAmount == __instance.draggedSlot.assignedSlot.Quantity)
                    {
                        ItemInstance itemInstance = __instance.draggedSlot.assignedSlot.ItemInstance;
                        ItemInstance itemInstance2 = __instance.HoveredSlot.assignedSlot.ItemInstance;
                        __instance.draggedSlot.assignedSlot.SetStoredItem(itemInstance2, false);
                        __instance.HoveredSlot.assignedSlot.SetStoredItem(itemInstance, false);
                    }
                    else if (__instance.HoveredSlot.assignedSlot.ItemInstance == null)
                    {
                        __instance.HoveredSlot.assignedSlot.SetStoredItem(__instance.draggedSlot.assignedSlot.ItemInstance, false);
                        __instance.draggedSlot.assignedSlot.ClearStoredInstance(false);
                    }
                }
                __instance.onItemMoved?.Invoke();
            }

            if (__instance.draggedSlot != null)
            {
                __instance.draggedSlot.SetVisible(true);
                __instance.draggedSlot.UpdateUI();
                __instance.draggedSlot.IsBeingDragged = false;
                __instance.draggedSlot = null;
            }

            if (__instance.tempIcon != null)
            {
                Object.Destroy(__instance.tempIcon.gameObject);
                __instance.tempIcon = null;
            }

            __instance.ItemQuantityPrompt.gameObject.SetActive(false);
            Singleton<CursorManager>.Instance.SetCursorAppearance(CursorManager.ECursorType.Default);
            return false;
        }
    }
}
