using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.FX;
using Il2CppScheduleOne.PlayerScripts;
using MelonLoader;
using SkillTree.Core.Utilities;
using System.Collections;
using UnityEngine;
using static SkillTree.Core.Utilities.ConfigManager;

namespace SkillTree.Core.Effects
{
    public class BloodRush
    {
        private static readonly string EffectName = "BloodRush";
        private static readonly int EffectTier = 3;
        public static bool IsBloodRushActive { get; private set; } = false;

        public static void ApplyToPlayer()
        {
            IsBloodRushActive = true;
            LogManager.LogMessage($"Blood Rush effect applied", LogLevel.Debug);
            PlayerSingleton<PlayerCamera>.Instance.FoVChangeSmoother.AddOverride(BloodRushFOVChange.GetValue(UseDefault.GetValue()), EffectTier, EffectName);
            PlayerSingleton<PlayerCamera>.Instance.HeartbeatSoundController.VolumeController.AddOverride(BloodRushHeartbeatVolume.GetValue(UseDefault.GetValue()), EffectTier, EffectName);
            PlayerSingleton<PlayerCamera>.Instance.HeartbeatSoundController.PitchController.AddOverride(BloodRushHeartbeatPitch.GetValue(UseDefault.GetValue()), EffectTier, EffectName);
            Singleton<PostProcessingManager>.Instance.ColorFilterController.AddOverride(BloodRushScreenTint.GetValue(UseDefault.GetValue()), EffectTier, EffectName);
            Core.AddCoroutine(EffectName, MelonCoroutines.Start(ClearFromPlayer()));
        }

        public static IEnumerator ClearFromPlayer()
        {
            yield return new WaitForSeconds(BloodRushDuration.GetValue(UseDefault.GetValue()));
            ClearEffect();
            Core.RemoveCoroutine(EffectName);
        }

        public static void ClearEffect()
        {
            IsBloodRushActive = false;
            PlayerSingleton<PlayerCamera>.Instance.FoVChangeSmoother.RemoveOverride(EffectName);
            PlayerSingleton<PlayerCamera>.Instance.HeartbeatSoundController.VolumeController.RemoveOverride(EffectName);
            PlayerSingleton<PlayerCamera>.Instance.HeartbeatSoundController.PitchController.RemoveOverride(EffectName);
            Singleton<PostProcessingManager>.Instance.ColorFilterController.RemoveOverride(EffectName);
            LogManager.LogMessage($"Blood Rush effect removed", LogLevel.Debug);
        }
    }
}
