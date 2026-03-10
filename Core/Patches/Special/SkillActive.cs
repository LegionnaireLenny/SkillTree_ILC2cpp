using Il2CppFishNet;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.GameTime;
using Il2CppScheduleOne.Money;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.Trash;
using Il2CppScheduleOne.UI;
using MelonLoader;
using SkillTree.Core.Effects;
using SkillTree.Core.Skills;
using SkillTree.Core.Utilities;
using System.Collections;
using UnityEngine;

namespace SkillTree.Core.Patches.Special
{
    public static class SkillActive
    {
        private static int currentDay = -1;

        private static bool clearTrashUsed = false;
        private static bool healUsed = false;
        private static bool getCashUsed = false;

        public static void Reset()
        {
            clearTrashUsed = false;
            healUsed = false;
            getCashUsed = false;
            currentDay = -1;
        }

        private static IEnumerator RemoveBloodRush(float duration, BloodRush effect)
        {
            yield return new WaitForSeconds(duration);
            MelonLogger.Msg($"Removing effect {effect}");
            effect.ClearFromPlayer(Player.Local);
        }

        public static void ResetSkillsIfNewDay()
        {
            if (currentDay != (int)TimeManager.Instance.CurrentDay)
            {
                clearTrashUsed = false;
                healUsed = false;
                getCashUsed = false;
                currentDay = (int)TimeManager.Instance.CurrentDay;
            }
        }

        public static void GoodSamaritan()
        {
            if (clearTrashUsed)
            {
                Singleton<NotificationsManager>.Instance.SendNotification(
                                "Good Samaritan on Cooldown",
                                $"<color=#FF0000>Wait one day</color>",
                                IconManager.LoadSprite(IconManager.IconTrashcan));
            }
            else
            {
                int total = 0;
                int count = TrashManager.Instance.trashItems.Count;

                if (count == 0)
                {
                    Singleton<NotificationsManager>.Instance.SendNotification(
                        "Good Samaritan",
                        "No trash found",
                        IconManager.LoadSprite(IconManager.IconTrashcan));

                    return;
                }

                foreach (var item in TrashManager.Instance.trashItems)
                {
                    total += item.SellValue;
                }

                if (InstanceFinder.IsServer)
                {
                    NetworkSingleton<MoneyManager>.Instance.CreateOnlineTransaction(
                        $"Payment for {count} pieces of trash destroyed",
                        total, 1f, string.Empty);

                    MelonLogger.Msg($"[Special] Payment of ${total} processed for destroying {count} pieces of trash");
                }

                TrashManager.Instance.DestroyAllTrash();
                Singleton<NotificationsManager>.Instance.SendNotification(
                    "Good Samaritan",
                    $"Earned <color=#4CBFFF>{MoneyManager.FormatAmount(total)}</color>",
                    IconManager.LoadSprite(IconManager.IconTrashcan));
                clearTrashUsed = true;
            }

        }

        public static void BloodRush()
        {
            if (healUsed)
                Singleton<NotificationsManager>.Instance.SendNotification(
                                "Blood Rush on Cooldown",
                                $"<color=#FF0000>Wait one day</color>",
                                IconManager.LoadSprite(IconManager.IconHeart));
            else
            {
                BloodRush bloodrush = new();
                bloodrush.ApplyToPlayer(Player.Local);

                float oldHp = Player.Local.Health.CurrentHealth;
                Player.Local.Health.RecoverHealth(SkillModifiers.GetPlayerMaxHealth());
                Singleton<NotificationsManager>.Instance.SendNotification(
                                "Blood Rush",
                                $"<color=#FF0000>{oldHp}</color> to <color=#FF0000>{Player.Local.Health.CurrentHealth}</color>",
                                IconManager.LoadSprite(IconManager.IconHeart));

                MelonCoroutines.Start(RemoveBloodRush(SkillModifiers.BloodRushDuration, bloodrush));
                healUsed = true;
            }
        }

        public static void SiphonFunds()
        {
            if (getCashUsed)
                Singleton<NotificationsManager>.Instance.SendNotification(
                                "Siphon Funds on Cooldown",
                                "<color=#FF0000>Wait one day</color>",
                                IconManager.LoadSprite(IconManager.IconCash));
            else
            {
                float totalCash = 0f;
                float totalOnlineBalance = 0f;

                foreach (Dealer dealer in Dealer.AllPlayerDealers)
                {
                    float amountConverted = dealer.Cash * SkillModifiers.GetSiphonFundsConversionMultiplier();
                    totalCash += dealer.Cash - amountConverted;
                    totalOnlineBalance += amountConverted;
                    //MelonLogger.Msg($"Dealer Cash: {dealer.Cash} | Unconverted Amount: {dealer.Cash - amountConverted} | Converted Amount: {amountConverted} | Conversion Rate: {SkillModifiers.GetSiphonFundsConversionMultiplier()}");

                    dealer.SetCash(0f);
                }

                if (!(totalCash > 0) && !(totalOnlineBalance > 0))
                {
                    Singleton<NotificationsManager>.Instance.SendNotification(
                                   "Siphon Funds",
                                   $"Dealers had no funds",
                                   IconManager.LoadSprite(IconManager.IconCash));
                    return;
                }

                NetworkSingleton<MoneyManager>.Instance.ChangeCashBalance(totalCash, true, true);

                //if (InstanceFinder.IsServer)
                //{
                NetworkSingleton<MoneyManager>.Instance.CreateOnlineTransaction(
                    $"Siphon Funds",
                    totalOnlineBalance, 1f, string.Empty);
                //}

                Singleton<NotificationsManager>.Instance.SendNotification(
                                "Siphon Funds",
                                $"<color=#54E717>{MoneyManager.FormatAmount(totalCash)}</color> and <color=#4CBFFF>{MoneyManager.FormatAmount(totalOnlineBalance)}</color>",
                                IconManager.LoadSprite(IconManager.IconCash));

                getCashUsed = true;
            }
        }
    }
}
