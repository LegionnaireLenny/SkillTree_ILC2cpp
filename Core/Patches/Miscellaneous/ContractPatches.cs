using HarmonyLib;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.GameTime;
using Il2CppScheduleOne.Quests;
using System;
using UnityEngine;
using UnityEngine.UI;
using static SkillTree.Core.Utilities.ConfigManager;

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
            if (__instance?.Entries[0]?.PoI == null || !EnableContractColors.GetValue()) return;

            try
            {
                string currentWindow = GetTimeWindow(NetworkSingleton<TimeManager>.Instance.CurrentTime);
                string deliveryWindow = GetTimeWindow(__instance.DeliveryWindow.WindowStartTime);
                Color backgroundColor = currentWindow.Equals(deliveryWindow) ? ContractReadyBackgroundColor.GetValue() : ContractNotReadyBackgroundColor.GetValue();
                Color fillColor = currentWindow.Equals(deliveryWindow) ? ContractReadyFillColor.GetValue() : ContractNotReadyFillColor.GetValue();

                __instance.hudUI.transform.FindChild("Title").FindChild("IconContainer").FindChild("ContractIcon(Clone)").FindChild("Background").GetComponent<Image>().color = backgroundColor;
                __instance.Entries[0].compassElement.Rect.FindChild("ContractIcon(Clone)").FindChild("Background").GetComponent<Image>().color = backgroundColor;
                __instance.Entries[0].PoI.IconContainer.FindChild("ContractIcon").FindChild("Background").GetComponent<Image>().color = backgroundColor;

                __instance.hudUI.transform.FindChild("Title").FindChild("IconContainer").FindChild("ContractIcon(Clone)").FindChild("Fill").GetComponent<Image>().color = fillColor;
                __instance.Entries[0].compassElement.Rect.FindChild("ContractIcon(Clone)").FindChild("Fill").GetComponent<Image>().color = fillColor;
                __instance.Entries[0].PoI.IconContainer.FindChild("ContractIcon").FindChild("Fill").GetComponent<Image>().color = fillColor;
            }
            catch (Exception) { }
        }
    }
}
