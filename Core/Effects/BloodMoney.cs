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

        public static void ApplyToPlayer()
        {
            IsBloodMoneyActive = true;
            MelonLogger.Msg($"Blood Money effect applied");
            MelonCoroutines.Start(ClearFromPlayer());
        }

        public static IEnumerator ClearFromPlayer()
        {
            yield return new WaitForSeconds(ConfigManager.BloodMoneyDuration.GetValue());
            IsBloodMoneyActive = false;
            MelonLogger.Msg($"Blood Money effect removed");
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
