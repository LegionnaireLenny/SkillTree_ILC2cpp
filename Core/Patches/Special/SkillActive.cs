using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.GameTime;
using Il2CppScheduleOne.Money;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.Trash;
using Il2CppScheduleOne.UI;

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

        public static void ClearTrash()
        {
            if(clearTrashUsed)
                Singleton<NotificationsManager>.Instance.SendNotification(
                                "ClearTrash in Cooldown",
                                $"<color=#FF0000>Wait one day</color>",
                                NetworkSingleton<MoneyManager>.Instance.LaunderingNotificationIcon);
            else
            {
                TrashManager.Instance.DestroyAllTrash();
                Singleton<NotificationsManager>.Instance.SendNotification(
                                "ClearTrash",
                                $"All trash clear",
                                NetworkSingleton<MoneyManager>.Instance.LaunderingNotificationIcon);
                clearTrashUsed = true;
            }

        }

        public static void Heal()
        {
            if(healUsed)
                Singleton<NotificationsManager>.Instance.SendNotification(
                                "Heal in Cooldown",
                                $"<color=#FF0000>Wait one day</color>",
                                NetworkSingleton<MoneyManager>.Instance.LaunderingNotificationIcon);
            else
            {
                float oldHp = Player.Local.Health.CurrentHealth;
                Player.Local.Health.RecoverHealth(1000);
                //Player.Local.Health.RecoverHealth(SkillModifiers.GetPlayerMaxHealth());
                Singleton<NotificationsManager>.Instance.SendNotification(
                                "Heal",
                                $"{oldHp} to {Player.Local.Health.CurrentHealth}",
                                NetworkSingleton<MoneyManager>.Instance.LaunderingNotificationIcon);
                healUsed = true;
            }

        }

        public static void GetCashDealer()
        {
            if(getCashUsed)
                Singleton<NotificationsManager>.Instance.SendNotification(
                                "Get Cash Dealer in Cooldown",
                                $"<color=#FF0000>Wait one day</color>",
                                NetworkSingleton<MoneyManager>.Instance.LaunderingNotificationIcon);
            else
            {
                float totalCash = 0;

                foreach (Dealer dealer in Dealer.AllPlayerDealers)
                {             
                    totalCash += dealer.Cash;
                    MoneyManager.Instance.ChangeCashBalance(dealer.Cash, true, true);

                    dealer.SetCash(0f);
                }
                Singleton<NotificationsManager>.Instance.SendNotification(
                                "Get Cash from Dealer",
                                $"<color=#16F01C>{MoneyManager.FormatAmount(totalCash)}</color> cash earned",
                                NetworkSingleton<MoneyManager>.Instance.LaunderingNotificationIcon);
                getCashUsed = true;
            }
        }
    }
}
