using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.FX;
using Il2CppScheduleOne.PlayerScripts;
using MelonLoader;
using System.Collections;
using UnityEngine;
using static SkillTree.Core.Utilities.ConfigManager;

namespace SkillTree.Core.Effects
{
    public class BloodRush
    {
        public static bool IsBloodRushActive { get; private set; } = false;
        private static readonly string EffectName = "BloodRush";
        private static readonly int EffectTier = 3;


        public static void ApplyToPlayer()
        {
            IsBloodRushActive = true;
            MelonLogger.Msg($"Blood Rush effect applied");
            PlayerSingleton<PlayerCamera>.Instance.FoVChangeSmoother.AddOverride(BloodRushFOVChange.GetValue(UseDefault.GetValue()), EffectTier, EffectName);
            PlayerSingleton<PlayerCamera>.Instance.HeartbeatSoundController.VolumeController.AddOverride(BloodRushHeartbeatVolume.GetValue(UseDefault.GetValue()), EffectTier, EffectName);
            PlayerSingleton<PlayerCamera>.Instance.HeartbeatSoundController.PitchController.AddOverride(BloodRushHeartbeatPitch.GetValue(UseDefault.GetValue()), EffectTier, EffectName);
            Singleton<PostProcessingManager>.Instance.ColorFilterController.AddOverride(BloodRushScreenTint.GetValue(UseDefault.GetValue()), EffectTier, EffectName);
            MelonCoroutines.Start(ClearFromPlayer());
        }

        public static IEnumerator ClearFromPlayer()
        {
            yield return new WaitForSeconds(BloodRushDuration.GetValue(UseDefault.GetValue()));
            IsBloodRushActive = false;
            PlayerSingleton<PlayerCamera>.Instance.FoVChangeSmoother.RemoveOverride(EffectName);
            PlayerSingleton<PlayerCamera>.Instance.HeartbeatSoundController.VolumeController.RemoveOverride(EffectName);
            PlayerSingleton<PlayerCamera>.Instance.HeartbeatSoundController.PitchController.RemoveOverride(EffectName);
            Singleton<PostProcessingManager>.Instance.ColorFilterController.RemoveOverride(EffectName);
            MelonLogger.Msg($"Blood Rush effect removed");
        }
    }
}
