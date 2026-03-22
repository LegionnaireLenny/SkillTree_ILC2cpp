using HarmonyLib;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.GameTime;
using Il2CppScheduleOne.Quests;
using MelonLoader;
using SkillTree.Core.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SkillTree.Core.Patches.Miscellaneous
{
    [HarmonyPatch]
    public class ContractPatches
    {
        public static string GetTimeWindow(float time)
        {
            string window = "";
            if (time >= 700 && time < 1200)
                window = "morning";
            else if (time >= 1200 && time < 1800)
                window = "afternoon";
            else if (time >= 1800 && time <= 2359)
                window = "night";
            else
                window = "latenight";
            return window;
        }

        [HarmonyPatch(typeof(Contract), "UpdatePoI")]
        [HarmonyPostfix]
        public static void Patch_Contract_UpdatePoI(Contract __instance)
        {
            if (__instance == null || !ConfigManager.EnableContractColors.GetValue()) return;

            string currentWindow = GetTimeWindow(NetworkSingleton<TimeManager>.Instance.CurrentTime);
            string deliveryWindow = GetTimeWindow(__instance.DeliveryWindow.WindowStartTime);
            Color backgroundColor = currentWindow.Equals(deliveryWindow) ? ConfigManager.ContractReadyBackgroundColor.GetValue() : ConfigManager.ContractNotReadyBackgroundColor.GetValue();
            Color fillColor = currentWindow.Equals(deliveryWindow) ? ConfigManager.ContractReadyFillColor.GetValue() : ConfigManager.ContractNotReadyFillColor.GetValue();

            if (__instance.Entries[0].PoI != null)
            {
                try
                {
                    __instance.hudUI.transform.FindChild("Title").FindChild("IconContainer").FindChild("ContractIcon(Clone)").FindChild("Background").GetComponent<Image>().color = backgroundColor;
                    __instance.Entries[0].compassElement.Rect.FindChild("ContractIcon(Clone)").FindChild("Background").GetComponent<Image>().color = backgroundColor;
                    __instance.Entries[0].PoI.IconContainer.FindChild("ContractIcon").FindChild("Background").GetComponent<Image>().color = backgroundColor;

                    __instance.hudUI.transform.FindChild("Title").FindChild("IconContainer").FindChild("ContractIcon(Clone)").FindChild("Fill").GetComponent<Image>().color = fillColor;
                    __instance.Entries[0].compassElement.Rect.FindChild("ContractIcon(Clone)").FindChild("Fill").GetComponent<Image>().color = fillColor;
                    __instance.Entries[0].PoI.IconContainer.FindChild("ContractIcon").FindChild("Fill").GetComponent<Image>().color = fillColor;
                }
                catch { }
            }
        }
    }
}
