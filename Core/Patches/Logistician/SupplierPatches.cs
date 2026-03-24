using HarmonyLib;
using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.Money;
using Il2CppScheduleOne.UI.Phone;
using SkillTree.Core.Serialization;
using SkillTree.Core.Skills;
using UnityEngine;

namespace SkillTree.Core.Patches.Logistician
{
    [HarmonyPatch]
    public class SupplierPatches
    {
        [HarmonyPatch(typeof(PhoneShopInterface), "CartChanged")]
        [HarmonyPrefix]
        public static bool Patch_CartChanged(PhoneShopInterface __instance)
        {
            if (__instance == null || SkillTreeData.Logistician.CurrentLevel == 0)
                return true;

            int itemCount;
            int itemMax = SkillModifiers.GetSupplierItemLimit();
            float orderTotal = __instance.GetOrderTotal(out itemCount);

            __instance.OrderTotalLabel.text = MoneyManager.FormatAmount(orderTotal, false, false);
            __instance.OrderTotalLabel.color = orderTotal <= __instance.orderLimit ? __instance.ValidAmountColor : __instance.InvalidAmountColor;
            __instance.ItemLimitLabel.text = itemCount.ToString() + "/" + itemMax.ToString();
            __instance.ItemLimitLabel.color = itemCount <= itemMax ? Color.black : __instance.InvalidAmountColor;

            __instance.ConfirmButton.interactable = orderTotal > 0f && orderTotal <= __instance.orderLimit && itemCount <= SkillModifiers.GetSupplierItemLimit();
            return false;
        }

        [HarmonyPatch(typeof(Supplier), "GetDeadDropLimit")]
        [HarmonyPostfix]
        public static void Patch_GetDeadDropLimit(Supplier __instance, ref float __result)
        {
            if (SkillTreeData.Logistician.CurrentLevel == 0)
                return;

            float originalLimit = __result;
            __result *= SkillModifiers.GetSupplierCashMultiplier();
            //MelonLogger.Msg($"[BetterSupplier] Supplier {__instance.fullName}'s order limit increased from ${(int)originalLimit} to ${(int)__result}");
        }
    }
}
