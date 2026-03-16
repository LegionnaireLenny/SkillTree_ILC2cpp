using Il2CppFishNet;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.Money;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.Trash;
using Il2CppScheduleOne.UI;
using MelonLoader;
using SkillTree.Core.Skills;
using SkillTree.Core.Utilities;

namespace SkillTree.Core.Patches.Special
{
    public static class SkillActive
    {
        private static bool GoodSamaritanUsed = false;
        private static bool BloodRushUsed = false;
        private static bool SiphonFundsUsed = false;

        public static void ResetSkillCooldowns()
        {
            GoodSamaritanUsed = false;
            BloodRushUsed = false;
            SiphonFundsUsed = false;
            Singleton<NotificationsManager>.Instance?.SendNotification(
                "A New Day Dawns",
                "Cooldowns Reset",
                IconManager.LoadSprite(IconManager.IconClock));
        }

        public static void GoodSamaritan()
        {
            if (GoodSamaritanUsed)
            {
                Singleton<NotificationsManager>.Instance.SendNotification(
                                "Good Samaritan on Cooldown",
                                $"<color=#FF0000>Wait one day</color>",
                                IconManager.LoadSprite(IconManager.IconTrashcan));
            }
            else
            {
                int total = 0;
                int count = NetworkSingleton<TrashManager>.Instance.trashItems.Count;

                if (count == 0)
                {
                    Singleton<NotificationsManager>.Instance.SendNotification(
                        "Good Samaritan",
                        "No trash found",
                        IconManager.LoadSprite(IconManager.IconTrashcan));

                    return;
                }

                foreach (var item in NetworkSingleton<TrashManager>.Instance.trashItems)
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

                NetworkSingleton<TrashManager>.Instance.DestroyAllTrash();
                //TrashManager.Instance.DestroyAllTrash();
                Singleton<NotificationsManager>.Instance.SendNotification(
                    "Good Samaritan",
                    $"Earned <color=#4CBFFF>{MoneyManager.FormatAmount(total)}</color>",
                    IconManager.LoadSprite(IconManager.IconTrashcan));
                GoodSamaritanUsed = true;
            }

        }

        public static void BloodRush()
        {
            if (BloodRushUsed)
                Singleton<NotificationsManager>.Instance.SendNotification(
                                "Blood Rush on Cooldown",
                                $"<color=#FF0000>Wait one day</color>",
                                IconManager.LoadSprite(IconManager.IconHeart));
            else
            {
                Effects.BloodRush.ApplyToPlayer(Player.Local);
                float oldHp = Player.Local.Health.CurrentHealth;
                Player.Local.Health.RecoverHealth(SkillModifiers.GetPlayerMaxHealth());
                Singleton<NotificationsManager>.Instance.SendNotification(
                                "Blood Rush",
                                $"<color=#FF0000>{oldHp}</color> to <color=#FF0000>{Player.Local.Health.CurrentHealth}</color>",
                                IconManager.LoadSprite(IconManager.IconHeart));
                BloodRushUsed = true;
            }
        }

        public static void SiphonFunds()
        {
            if (SiphonFundsUsed)
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

                SiphonFundsUsed = true;
            }
        }
    }
}
