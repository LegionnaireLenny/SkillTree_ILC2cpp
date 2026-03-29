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

        public static void ApplyToPlayer()
        {
            IsBloodRushActive = true;
            MelonLogger.Msg($"Blood Rush effect applied");
            MelonCoroutines.Start(ClearFromPlayer());
        }

        public static IEnumerator ClearFromPlayer()
        {
            yield return new WaitForSeconds(ConfigManager.BloodRushDuration.GetValue());
            IsBloodRushActive = false;
            MelonLogger.Msg($"Blood Rush effect removed");
        }
    }
}
