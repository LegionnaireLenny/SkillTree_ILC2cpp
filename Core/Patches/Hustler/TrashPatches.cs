using HarmonyLib;
using Il2CppScheduleOne;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Equipping;
using Il2CppScheduleOne.Interaction;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.Trash;
using Il2CppScheduleOne.UI;
using Il2CppScheduleOne.UI.Items;
using Il2CppSystem;
using SkillTree.Core.Serialization;
using SkillTree.Core.Skills;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace SkillTree.Core.Patches.Hustler
{
    [HarmonyPatch]
    public class TrashPatches
    {
        public static readonly HashSet<Guid> ProcessedTrash = [];
        private static LayerMask PickupLookMask = 5;
        private static DecalProjector PickupAreaProjector;

        [HarmonyPatch(typeof(TrashItem), "Start")]
        [HarmonyPostfix]
        public static void Patch_Start(TrashItem __instance)
        {
            if (__instance == null || SkillTreeData.SacarLaBasura.CurrentLevel == 0 || ProcessedTrash.Contains(__instance.GUID))
            {
                return;
            }

            //int original = __instance.SellValue;
            __instance.SellValue += SkillModifiers.GetTrashValueBonus();
            ProcessedTrash.Add(__instance.GUID);
            //MelonLogger.Msg($"Start Trash value {__instance.GUID} {original} -> {__instance.SellValue}");
        }

        [HarmonyPatch(typeof(Equippable_TrashGrabber), "GetCapacity")]
        [HarmonyPrefix]
        public static bool Patch_TrashGrabber_GetCapacity(Equippable_TrashGrabber __instance, ref int __result)
        {
            if (__instance == null || SkillTreeData.CommunityService.CurrentLevel == 0)
            {
                return true;
            }
            __result = SkillModifiers.GetGrabberBinSize() - __instance.trashGrabberInstance.GetTotalSize();
            return false;
        }

        [HarmonyPatch(typeof(Equippable_TrashGrabber), "RefreshVisuals")]
        [HarmonyPrefix]
        public static bool Patch_TrashGrabber_RefreshVisuals(Equippable_TrashGrabber __instance)
        {
            if (__instance == null || SkillTreeData.CommunityService.CurrentLevel == 0)
            {
                return true;
            }

            float num = Mathf.Clamp01((float)__instance.trashGrabberInstance.GetTotalSize() / SkillModifiers.GetGrabberBinSize());
            __instance.TrashContent.localPosition = Vector3.Lerp(__instance.TrashContent_Min.localPosition, __instance.TrashContent_Max.localPosition, num);
            __instance.TrashContent.localScale = Vector3.Lerp(__instance.TrashContent_Min.localScale, __instance.TrashContent_Max.localScale, num);
            __instance.TrashContent.gameObject.SetActive(num > 0f);

            return false;
        }

        [HarmonyPatch(typeof(TrashGrabberItemUI), "UpdateUI")]
        [HarmonyPrefix]
        public static bool Patch_TrashGrabberItemUI_UpdateUI(TrashGrabberItemUI __instance)
        {
            if (__instance == null || SkillTreeData.CommunityService.CurrentLevel == 0) return true;

            if (__instance.Destroyed) return false;

            __instance.ValueLabel.text = Mathf.FloorToInt(Mathf.Clamp01((float)__instance.trashGrabberInstance.GetTotalSize() / SkillModifiers.GetGrabberBinSize()) * 100f).ToString() + "%";
            __instance.IconImg.sprite = __instance.itemInstance.Icon;
            __instance.SetDisplayedQuantity(__instance.itemInstance.Quantity);
            return false;
        }


        [HarmonyPatch(typeof(Equippable_TrashGrabber), "Equip")]
        [HarmonyPostfix]
        public static void Patch_TrashGrabber_Equip(Equippable_TrashGrabber __instance, ItemInstance item)
        {
            if (__instance == null || SkillTreeData.CommunityService.CurrentLevel == 0) return;

            if (PickupAreaProjector == null)
            {
                PickupAreaProjector = UnityEngine.Object.Instantiate(Resources.FindObjectsOfTypeAll<TrashBag_Equippable>().First().PickupAreaProjector);
            }

            Singleton<TrashBagCanvas>.Instance.InputPrompt.gameObject.SetActive(false);
            Singleton<TrashBagCanvas>.Instance.Open();
            PickupAreaProjector.transform.SetParent(NetworkSingleton<GameManager>.Instance.Temp);
            PickupAreaProjector.transform.localScale = Vector3.one;
            PickupAreaProjector.transform.forward = -Vector3.up;
            PickupAreaProjector.gameObject.SetActive(false);
        }

        [HarmonyPatch(typeof(Equippable_TrashGrabber), "Unequip")]
        [HarmonyPostfix]
        public static void Patch_TrashGrabber_Unequip(Equippable_TrashGrabber __instance)
        {
            if (__instance == null || SkillTreeData.CommunityService.CurrentLevel == 0) return;

            Singleton<TrashBagCanvas>.Instance?.Close();
            UnityEngine.Object.Destroy(PickupAreaProjector.gameObject);
            PickupAreaProjector = null;
        }

        [HarmonyPatch(typeof(Equippable_TrashGrabber), "Update")]
        [HarmonyPostfix]
        public static void Patch_TrashGrabber_Update(Equippable_TrashGrabber __instance)
        {
            if (__instance == null || SkillTreeData.CommunityService.CurrentLevel == 0 || PickupAreaProjector == null) return;

            Singleton<TrashBagCanvas>.Instance.InputPrompt.gameObject.SetActive(false);
            PickupAreaProjector.gameObject.SetActive(false);

            if (RaycastLook(out RaycastHit raycastHit) && IsPickupLocationValid(raycastHit))
            {
                List<TrashItem> list = GetTrashItemsAtPoint(raycastHit.point);

                PickupAreaProjector.size = new Vector3(SkillModifiers.GetGrabberPickupRadiusMultiplier(), SkillModifiers.GetGrabberPickupRadiusMultiplier(), 0.5f);
                PickupAreaProjector.transform.position = raycastHit.point + Vector3.up * 0.1f;
                PickupAreaProjector.gameObject.SetActive(true);
                if (list.Count > 0)
                {
                    PickupAreaProjector.fadeFactor = 0.5f;
                    if (Equippable_TrashGrabber.Instance.GetCapacity() > 0)
                    {
                        Singleton<TrashBagCanvas>.Instance.InputPrompt.SetLabel("Grab trash");
                    }
                    else
                    {
                        Singleton<TrashBagCanvas>.Instance.InputPrompt.SetLabel("<color=#FF0000>Bin is full</color>");
                    }
                    Singleton<TrashBagCanvas>.Instance.InputPrompt.gameObject.SetActive(true);
                    bool itemGrabbed = false;
                    if (GameInput.GetButtonDown(GameInput.ButtonCode.Interact) && Cursor.lockState != CursorLockMode.None)
                    {
                        HashSet<Guid> processedTrash = [];
                        for (int i = 0; i < list.Count; i++)
                        {
                            if (Equippable_TrashGrabber.IsEquipped &&
                                list[i].CanGoInContainer &&
                                !processedTrash.Contains(list[i].GUID) &&
                                Equippable_TrashGrabber.Instance.GetCapacity() > 0)
                            {
                                //MelonLogger.Msg($"Adding {list[i]} | ID {list[i].ID} | Name {list[i].name} | GUID {list[i].GUID} | Size {list[i].Size} | Value {list[i].SellValue}");
                                __instance.trashGrabberInstance.AddTrash(list[i].ID, 1);
                                list[i].DestroyTrash();
                                itemGrabbed = true;
                                processedTrash.Add(list[i].GUID);
                            }
                        }

                        if (itemGrabbed)
                        {
                            __instance.GrabAnim.Stop();
                            __instance.GrabAnim.Play();
                            __instance.onPickup?.Invoke();
                        }
                    }
                }
                else
                {
                    PickupAreaProjector.fadeFactor = 0.05f;
                }
            }
        }

        public static bool RaycastLook(out RaycastHit hit)
        {
            return PlayerSingleton<PlayerCamera>.Instance.LookRaycast(3f, out hit, PickupLookMask, true, 0f);
        }

        public static List<TrashItem> GetTrashItemsAtPoint(Vector3 pos)
        {
            Collider[] array = Physics.OverlapSphere(pos, SkillModifiers.GetGrabberPickupRadius(), Singleton<InteractionManager>.Instance.Interaction_SearchMask, QueryTriggerInteraction.Collide);
            List<TrashItem> list = new List<TrashItem>();
            for (int i = 0; i < array.Length; i++)
            {
                TrashItem componentInParent = array[i].GetComponentInParent<TrashItem>();
                if (componentInParent != null && componentInParent.CanGoInContainer)
                {
                    list.Add(componentInParent);
                }
            }
            return list;
        }

        public static bool IsPickupLocationValid(RaycastHit hit)
        {
            return Vector3.Angle(hit.normal, Vector3.up) <= 5f;
        }

        public static void IncreaseTrashValue()
        {
            foreach (TrashItem item in NetworkSingleton<TrashManager>.Instance.trashItems)
            {
                if (ProcessedTrash.Contains(item.GUID))
                {
                    continue;
                }

                //int original = item.SellValue;
                item.SellValue += SkillModifiers.GetTrashValueBonus();
                ProcessedTrash.Add(item.GUID);
                //MelonLogger.Msg($"IncreaseTrashValue {item.GUID} {original} -> {item.SellValue}");
            }
        }

        [HarmonyPatch(typeof(TrashItem), "DestroyTrash")]
        [HarmonyPostfix]
        public static void Patch_DestroyTrash(TrashItem __instance)
        {
            //MelonLogger.Msg($"Destroy Trash. Removed {__instance.GUID} from cache");
            ProcessedTrash.Remove(__instance.GUID);
            //MelonLogger.Msg($"Processed IDs left {ProcessedTrash.Count}");
        }
    }
}
