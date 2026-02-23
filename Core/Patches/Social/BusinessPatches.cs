using HarmonyLib;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Money;
using Il2CppScheduleOne.Property;
using Il2CppScheduleOne.UI;
using MelonLoader;
using System.Linq;
using UnityEngine;

namespace SkillTree.Core.Patches.Social
{
    [HarmonyPatch]
    public static class BusinessPatches
    {
        public static void SetLaunderingCapacity()
        {
            if (Core.SkillData.BusinessEvolving != 0)
            {
                MelonLogger.Msg($"[BusinessEvolving] Increasing business laundering capacity by {(int)(SkillModifiers.GetLaunderingCapacityMultiplier() % 1 * 100)}%");
            }

            Business[] businessList = UnityEngine.Object.FindObjectsOfType<Business>();
            Cache.FillCache(businessList.ToList());
            foreach (Business business in businessList)
            {
                if (Cache.OriginalLaunderCapacity.TryGetValue(business.PropertyName, out float original))
                {
                    business.LaunderCapacity = original * SkillModifiers.GetLaunderingCapacityMultiplier();
                    if (!Mathf.Approximately(original, business.LaunderCapacity))
                    {
                        MelonLogger.Msg($"[BusinessEvolving] {business.PropertyName}: ${original} -> ${business.LaunderCapacity}");
                    }
                }
            }
        }


        // Handles the progression of minutes and partial payments every 4 hours (240 mins)
        [HarmonyPatch(typeof(Business), "MinsPass")]
        [HarmonyPrefix]
        public static bool Prefix_MinsPass(Business __instance, int mins)
        {
            string pName = __instance.propertyName;

            for (int i = 0; i < __instance.LaunderingOperations.Count; i++)
            {
                var op = __instance.LaunderingOperations[i];
                int oldMins = op.minutesSinceStarted;
                op.minutesSinceStarted += mins;

                if (op.minutesSinceStarted < op.completionTime_Minutes)
                {
                    int oldInterval = oldMins / 240;
                    int newInterval = op.minutesSinceStarted / 240;

                    if (newInterval > oldInterval)
                    {
                        float installment = Mathf.Ceil(op.amount / 6f);

                        if (Il2CppFishNet.InstanceFinder.IsServer)
                        {
                            NetworkSingleton<MoneyManager>.Instance.CreateOnlineTransaction(
                                $"Partial Laundering ({pName})",
                                installment, 1f, string.Empty);

                            MelonLogger.Msg($"[LaunderingMod] Partial payout of {installment} processed for {pName}");
                        }

                        Singleton<NotificationsManager>.Instance.SendNotification(
                            pName,
                            $"<color=#16F01C>{MoneyManager.FormatAmount(installment)}</color> Laundered (Partial)",
                            NetworkSingleton<MoneyManager>.Instance.LaunderingNotificationIcon);
                    }
                }

                if (op.minutesSinceStarted >= op.completionTime_Minutes)
                {
                    op.amount = op.amount / 6f;

                    __instance.CompleteOperation(op);

                    MelonLogger.Msg($"[LaunderingMod] Operation completed for {pName}. Final installment paid.");
                    i--;
                }
            }

            return false;
        }
    }
}
