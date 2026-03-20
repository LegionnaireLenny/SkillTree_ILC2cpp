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
        //public static readonly string ColorContractReady = "ContractReady";
        //public static readonly string ColorContractNotReady = "ContractNotReady";

        //public static readonly Dictionary<string, Color> Colors = new()
        //{
        //    {ColorContractReady, new(0.2984f, 0.6226f, 0.2673f, 1f)},
        //    {ColorContractNotReady, new(0.6984f, 0.6226f, 0.4673f, 1f)}
        //};

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

        //[HarmonyPatch(typeof(ColorFont), "GetColour")]
        //[HarmonyPostfix]
        //public static void Patch_ColorFont_GetColor(string name, ref Color __result)
        //{
        //    if (Colors.ContainsKey(name))
        //    {
        //        __result = Colors[name];
        //    }
        //}

        //[HarmonyPatch(typeof(FontSetter), "SetColour")]
        //[HarmonyPrefix]
        //public static bool SetColour(FontSetter __instance, string componentName, string ColourName)
        //{
        //    FontSetter.ImageItem imageItem = new();
        //    foreach (var item in __instance._imageItems)
        //    {
        //        if (item.Name == componentName)
        //        {
        //            imageItem = item;
        //            break;
        //        }
        //    }
        //    if (__instance._colourFont == null || imageItem == null)
        //    {
        //        return false;
        //    }
        //    Color colour = __instance._colourFont.GetColour(ColourName);
        //    if (imageItem.Image != null)
        //    {
        //        imageItem.Image.color = colour;
        //    }
        //    return false;
        //}

        [HarmonyPatch(typeof(Contract), "UpdatePoI")]
        [HarmonyPostfix]
        public static void Patch_Contract_UpdatePoI(Contract __instance)
        {
            string currentWindow = GetTimeWindow(NetworkSingleton<TimeManager>.Instance.CurrentTime);
            string deliveryWindow = GetTimeWindow(__instance.DeliveryWindow.WindowStartTime);
            Color color = currentWindow.Equals(deliveryWindow) ? ConfigManager.ContractColorReady.GetValue() : ConfigManager.ContractColorNotReady.GetValue();

            //var entry = __instance.Entries[0];
            if (__instance.Entries[0].PoI != null)
            {
                try
                {
                    __instance.hudUI.transform.FindChild("Title").FindChild("IconContainer").FindChild("ContractIcon(Clone)").FindChild("Background").GetComponent<Image>().color = color;
                    __instance.Entries[0].compassElement.Rect.FindChild("ContractIcon(Clone)").FindChild("Background").GetComponent<Image>().color = color;
                    __instance.Entries[0].PoI.IconContainer.FindChild("ContractIcon").FindChild("Background").GetComponent<Image>().color = color;
                    //entry.compassElement.Rect.FindChild("ContractIcon(Clone)").FindChild("Background").GetComponent<Image>().color = Colors[color];
                    //__instance.hudUI.transform.FindChild("Title").FindChild("IconContainer").FindChild("ContractIcon(Clone)").FindChild("Background").GetComponent<Image>().color = Colors[color];
                    //entry.PoI.IconContainer.FindChild("ContractIcon").FindChild("Background").GetComponent<Image>().color = Colors[color];
                    //__instance.Entries[0].SetPoIColor("Background", color);
                    //MelonLogger.Msg(entry.PoI.transform.name);
                }
                catch { }
            }
        }
    }
}
