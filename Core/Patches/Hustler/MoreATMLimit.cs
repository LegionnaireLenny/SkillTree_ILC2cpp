using HarmonyLib;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Money;
using Il2CppScheduleOne.UI.ATM;
using SkillTree.Core.Serialization;
using SkillTree.Core.Skills;
using UnityEngine;

namespace SkillTree.Core.Patches.Hustler
{
    [HarmonyPatch(typeof(ATMInterface))]
    public static class MoreATMLimit
    {
        [HarmonyPatch("get_remainingAllowedDeposit")]
        [HarmonyPrefix]
        public static bool PrefixGetRemaining(ref float __result)
        {
            __result = Mathf.Max(0f, SkillModifiers.GetATMLimit() - ATM.WeeklyDepositSum);
            return false;
        }

        [HarmonyPatch("GetAmountFromIndex")]
        [HarmonyPrefix]
        public static bool PrefixGetAmount(int index, bool depositing, ref float __result)
        {
            if (index == -1 || index >= ATMInterface.amounts.Length)
            {
                __result = 0f;
                return false;
            }

            if (depositing && index == ATMInterface.amounts.Length - 1)
            {
                float remaining = Mathf.Max(0f, SkillModifiers.GetATMLimit() - ATM.WeeklyDepositSum);
                __result = Mathf.Min(NetworkSingleton<MoneyManager>.Instance.cashBalance, remaining);
                return false;
            }

            __result = ATMInterface.amounts[index];
            return false;
        }

        [HarmonyPatch("SetSelectedAmount")]
        [HarmonyPrefix]
        public static bool PrefixSetSelected(ATMInterface __instance, float amount)
        {
            if (__instance == null || SkillTreeData.HoardTheWealth.CurrentLevel == 0) return true;

            float remaining = Mathf.Max(0f, SkillModifiers.GetATMLimit() - ATM.WeeklyDepositSum);

            float onlineBalance = NetworkSingleton<MoneyManager>.Instance.sync___get_value_onlineBalance();

            float limitForOperation = !__instance.depositing
                ? onlineBalance
                : Mathf.Min(NetworkSingleton<MoneyManager>.Instance.cashBalance, remaining);

            // Acessamos os campos diretamente pela instância para evitar o erro do AccessTools
            __instance.selectedAmount = Mathf.Clamp(amount, 0f, limitForOperation);

            if (__instance.amountLabelText != null)
            {
                __instance.amountLabelText.text = MoneyManager.FormatAmount(__instance.selectedAmount);
            }

            return false;
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static void PostfixUpdate(ATMInterface __instance)
        {
            if (__instance == null || !__instance.isOpen || SkillTreeData.HoardTheWealth.CurrentLevel == 0) return;

            bool limitReached = ATM.WeeklyDepositSum >= SkillModifiers.GetATMLimit();

            if (__instance.menu_DepositButton != null)
                __instance.menu_DepositButton.interactable = !limitReached;

            if (__instance.depositLimitText != null)
            {
                __instance.depositLimitText.text = MoneyManager.FormatAmount(ATM.WeeklyDepositSum) + " / " + MoneyManager.FormatAmount(SkillModifiers.GetATMLimit());
                __instance.depositLimitText.color = limitReached ? new Color32(255, 75, 75, 255) : Color.white;
            }
        }

        [HarmonyPatch("UpdateAvailableAmounts")]
        [HarmonyPrefix]
        public static bool PrefixUpdateAmounts(ATMInterface __instance)
        {
            if (__instance == null || SkillTreeData.HoardTheWealth.CurrentLevel == 0) return true;

            if (__instance.depositing)
            {
                float cash = NetworkSingleton<MoneyManager>.Instance.cashBalance;
                float remaining = Mathf.Max(0f, SkillModifiers.GetATMLimit() - ATM.WeeklyDepositSum);
                var buttons = __instance.amountButtons;

                for (int i = 0; i < ATMInterface.amounts.Length; i++)
                {
                    if (i >= buttons.Count) break;

                    float amountVal = ATMInterface.amounts[i];
                    if (i == ATMInterface.amounts.Length - 1)
                        buttons[i].interactable = cash > 0f && remaining > 0f;
                    else
                        buttons[i].interactable = cash >= amountVal && amountVal <= remaining;
                }
                return false;
            }
            return true;
        }
    }
}
