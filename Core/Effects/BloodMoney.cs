using Il2CppScheduleOne.PlayerScripts;
using MelonLoader;
using S1API.Money;
using SkillTree.Core.Utilities;
using System.Collections;
using UnityEngine;

namespace SkillTree.Core.Effects
{
    public class BloodMoney
    {
        public static bool IsBloodMoneyActive { get; private set; } = false;

        public static void ApplyToPlayer(Player player)
        {
            IsBloodMoneyActive = true;
            MelonLogger.Msg($"Blood Money effect applied");
            MelonCoroutines.Start(RemoveBloodMoney(player));
        }

        public static void ClearFromPlayer(Player player)
        {
            IsBloodMoneyActive = false;
            MelonLogger.Msg($"Blood Money effect removed");
        }

        private static IEnumerator RemoveBloodMoney(Player player)
        {
            yield return new WaitForSeconds(ConfigManager.BloodMoneyDuration.GetValue());
            ClearFromPlayer(player);
        }

        public static void GetBloodMoney(float damage)
        {
            if (IsBloodMoneyActive)
            {
                Money.ChangeCashBalance(damage);
            }
        }
    }
}
