using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.FX;
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
        private static readonly string EffectName = "BloodRush";
        private static readonly int EffectTier = 3;
        private static readonly Color EffectTint = new Color(1f, 0.9f, 0.9f, 0.2f);


        public static void ApplyToPlayer()
        {
            IsBloodRushActive = true;
            MelonLogger.Msg($"Blood Rush effect applied");
            PlayerSingleton<PlayerCamera>.Instance.FoVChangeSmoother.AddOverride(10f, EffectTier, EffectName);
            PlayerSingleton<PlayerCamera>.Instance.HeartbeatSoundController.VolumeController.AddOverride(0.25f, EffectTier, EffectName);
            PlayerSingleton<PlayerCamera>.Instance.HeartbeatSoundController.PitchController.AddOverride(1f, EffectTier, EffectName);
            Singleton<PostProcessingManager>.Instance.ColorFilterController.AddOverride(EffectTint, EffectTier, EffectName);
            MelonCoroutines.Start(ClearFromPlayer());
        }

        public static IEnumerator ClearFromPlayer()
        {
            yield return new WaitForSeconds(ConfigManager.BloodRushDuration.GetValue());
            IsBloodRushActive = false;
            PlayerSingleton<PlayerCamera>.Instance.FoVChangeSmoother.RemoveOverride(EffectName);
            PlayerSingleton<PlayerCamera>.Instance.HeartbeatSoundController.VolumeController.RemoveOverride(EffectName);
            PlayerSingleton<PlayerCamera>.Instance.HeartbeatSoundController.PitchController.RemoveOverride(EffectName);
            Singleton<PostProcessingManager>.Instance.ColorFilterController.RemoveOverride(EffectName);
            MelonLogger.Msg($"Blood Rush effect removed");
        }
    }
}
