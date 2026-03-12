using Il2CppScheduleOne.PlayerScripts;
using MelonLoader;
using System.Collections;
using UnityEngine;

namespace SkillTree.Core.Effects
{
    public class BloodRush
    {
        public static bool IsBloodRushActive { get; private set; } = false;
        public static readonly float Duration = 60f;

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
            yield return new WaitForSeconds(Duration);
            ClearFromPlayer(player);
        }
    }
}
