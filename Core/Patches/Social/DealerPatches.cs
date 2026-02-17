using HarmonyLib;
using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.UI.Phone.Messages;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SkillTree.Core.Patches.Social
{
    [HarmonyPatch(typeof(DealerManagementApp))]
    public class DealerPatches
    {
        private static void CheckAndExpandUI(DealerManagementApp __instance)
        {
            if (Core.SkillData == null)
                return;

            if (__instance.CustomerEntries.Length < SkillModifiers.GetMaxCustomers())
            {
                List<RectTransform> entriesList = __instance.CustomerEntries.ToList();
                RectTransform template = entriesList[0];
                Transform listParent = template.parent;

                while (entriesList.Count < SkillModifiers.GetMaxCustomers())
                {
                    RectTransform newSlot = UnityEngine.Object.Instantiate(template, listParent);
                    newSlot.name = "CustomerEntry_Mod_Slot_" + entriesList.Count;
                    entriesList.Add(newSlot);
                }

                __instance.CustomerEntries = entriesList.ToArray();
                LayoutRebuilder.ForceRebuildLayoutImmediate(__instance.Content);
            }
        }

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void Awake_Postfix(DealerManagementApp __instance)
        {
            if (Core.SkillData == null)
                return;

            CheckAndExpandUI(__instance);

            if (__instance.AssignCustomerButton != null)
            {
                __instance.AssignCustomerButton.transform.SetSiblingIndex(1);
            }

            if (__instance.CustomerTitleLabel != null)
            {
                __instance.CustomerTitleLabel.transform.SetAsFirstSibling();
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(__instance.Content);
        }

        [HarmonyPatch("SetDisplayedDealer")]
        [HarmonyPostfix]
        public static void SetDisplayedDealer_Postfix(DealerManagementApp __instance, Dealer dealer)
        {
            if (Core.SkillData == null)
                return;
            
            CheckAndExpandUI(__instance);

            if (__instance.CustomerTitleLabel != null)
            {
                __instance.CustomerTitleLabel.text = $"Assigned Customers ({dealer.AssignedCustomers.Count}/{SkillModifiers.GetMaxCustomers()})";
            }

            if (__instance.AssignCustomerButton != null)
            {
                __instance.AssignCustomerButton.gameObject.SetActive(dealer.AssignedCustomers.Count < SkillModifiers.GetMaxCustomers());
                __instance.AssignCustomerButton.transform.SetSiblingIndex(1);
            }

            for (int j = 0; j < __instance.CustomerEntries.Length; j++)
            {
                if (dealer.AssignedCustomers.Count > j)
                {
                    Customer customer = dealer.AssignedCustomers[j];
                    RectTransform entry = __instance.CustomerEntries[j];

                    entry.Find("Mugshot").GetComponent<Image>().sprite = customer.NPC.MugshotSprite;
                    entry.Find("Name").GetComponent<Text>().text = customer.NPC.fullName;

                    Button removeBtn = entry.Find("Remove").GetComponent<Button>();
                    removeBtn.onClick.RemoveAllListeners();
                    removeBtn.onClick.AddListener((UnityAction)(() => {
                        dealer.SendRemoveCustomer(customer.NPC.ID);
                        __instance.SetDisplayedDealer(dealer);
                    }));

                    entry.gameObject.SetActive(true);
                }
                else
                {
                    __instance.CustomerEntries[j].gameObject.SetActive(false);
                }
            }
        }
    }
}
