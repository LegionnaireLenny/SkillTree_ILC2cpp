using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.FX;
using Il2CppScheduleOne.PlayerScripts;
using MelonLoader;
using S1API.Money;
using System.Collections;
using UnityEngine;
using static SkillTree.Core.Utilities.ConfigManager;

namespace SkillTree.Core.Effects
{
    public class BloodMoney
    {
        public static bool IsBloodMoneyActive { get; private set; } = false;
        private static readonly string EffectName = "BloodMoney";
        private static readonly int EffectTier = 3;

        public static void ApplyToPlayer()
        {
            IsBloodMoneyActive = true;
            MelonLogger.Msg($"Blood Money effect applied");
            PlayerSingleton<PlayerCamera>.Instance.FoVChangeSmoother.AddOverride(BloodMoneyFOVChange.GetValue(UseDefault.GetValue()), EffectTier, EffectName);
            PlayerSingleton<PlayerCamera>.Instance.HeartbeatSoundController.VolumeController.AddOverride(BloodMoneyHeartbeatVolume.GetValue(UseDefault.GetValue()), EffectTier, EffectName);
            PlayerSingleton<PlayerCamera>.Instance.HeartbeatSoundController.PitchController.AddOverride(BloodMoneyHeartbeatPitch.GetValue(UseDefault.GetValue()), EffectTier, EffectName);
            Singleton<PostProcessingManager>.Instance.ColorFilterController.AddOverride(BloodMoneyScreenTint.GetValue(UseDefault.GetValue()), EffectTier, EffectName);
            MelonCoroutines.Start(ClearFromPlayer());
        }

        public static IEnumerator ClearFromPlayer()
        {
            yield return new WaitForSeconds(BloodMoneyDuration.GetValue(UseDefault.GetValue()));
            IsBloodMoneyActive = false;
            PlayerSingleton<PlayerCamera>.Instance.FoVChangeSmoother.RemoveOverride(EffectName);
            PlayerSingleton<PlayerCamera>.Instance.HeartbeatSoundController.VolumeController.RemoveOverride(EffectName);
            PlayerSingleton<PlayerCamera>.Instance.HeartbeatSoundController.PitchController.RemoveOverride(EffectName);
            Singleton<PostProcessingManager>.Instance.ColorFilterController.RemoveOverride(EffectName);

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
