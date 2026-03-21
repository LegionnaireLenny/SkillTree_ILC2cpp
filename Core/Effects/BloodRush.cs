using Il2CppScheduleOne.PlayerScripts;
using MelonLoader;
using SkillTree.Core.Utilities;
using System.Collections;
using UnityEngine;

namespace SkillTree.Core.Effects
{
    public class BloodRush
    {
        public static bool IsBloodRushActive { get; private set; } = false;

        public static void ApplyToPlayer(Player player)
        {
            IsBloodRushActive = true;
            MelonLogger.Msg($"Blood Rush effect applied");
            MelonCoroutines.Start(RemoveBloodRush(player));
        }

        public static void ClearFromPlayer(Player player)
        {
            IsBloodRushActive = false;
            MelonLogger.Msg($"Blood Rush effect removed");
        }

        private static IEnumerator RemoveBloodRush(Player player)
        {
            yield return new WaitForSeconds(ConfigManager.BloodRushDuration.GetValue());
            ClearFromPlayer(player);
        }
    }
}
