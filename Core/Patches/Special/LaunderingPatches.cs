using HarmonyLib;
using Il2CppFishNet;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Money;
using Il2CppScheduleOne.Property;
using Il2CppScheduleOne.UI;
using MelonLoader;
using SkillTree.Core.Skills;
using SkillTree.Core.Utilities;
using UnityEngine;

namespace SkillTree.Core.Patches.Special
{
    [HarmonyPatch]
    public static class BusinessPatchesBase
    {
        // Handles the progression of minutes and partial payments every user-defined interval (default 6 hours)
        [HarmonyPatch(typeof(Business), "MinsPass")]
        [HarmonyPrefix]
        public static bool Prefix_MinsPass(Business __instance, int mins)
        {
            if (SkillTreeData.TrickleDown.CurrentLevel == 0)
            {
                return true;
            }

            int payoutInterval = ConfigManager.TrickleDownPayoutInterval.GetValue() * 60;
            float payoutPercentage = ConfigManager.TrickleDownPayoutInterval.GetValue() / 24f;

            for (int i = 0; i < __instance.LaunderingOperations.Count; i++)
            {
                var operation = __instance.LaunderingOperations[i];
                int oldMins = operation.minutesSinceStarted;
                operation.minutesSinceStarted += mins;

                if (operation.minutesSinceStarted < operation.completionTime_Minutes)
                {
                    int oldInterval = oldMins / payoutInterval;
                    int newInterval = operation.minutesSinceStarted / payoutInterval;

                    if (newInterval > oldInterval)
                    {
                        float installment = Mathf.Ceil(operation.amount * payoutPercentage);

                        if (InstanceFinder.IsServer)
                        {
                            NetworkSingleton<MoneyManager>.Instance.CreateOnlineTransaction(
                                $"Partial Laundering ({__instance.propertyName})",
                                installment, 1f, string.Empty);

                            MelonLogger.Msg($"[LaunderingMod] Partial payout of {installment} processed for {__instance.propertyName}");
                        }

                        Singleton<NotificationsManager>.Instance.SendNotification(
                            __instance.propertyName,
                            $"<color=#16F01C>{MoneyManager.FormatAmount(installment)}</color> Laundered (Partial)",
                            NetworkSingleton<MoneyManager>.Instance.LaunderingNotificationIcon);
                    }
                }

                if (operation.minutesSinceStarted >= operation.completionTime_Minutes)
                {
                    operation.amount *= payoutPercentage;

                    __instance.CompleteOperation(operation);

                    MelonLogger.Msg($"[LaunderingMod] Operation completed for {__instance.propertyName}. Final installment paid.");
                    i--;
                }
            }

            return false;
        }
    }
}