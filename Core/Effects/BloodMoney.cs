using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.FX;
using Il2CppScheduleOne.PlayerScripts;
using MelonLoader;
using S1API.Money;
using SkillTree.Core.Utilities;
using System.Collections;
using UnityEngine;
using static SkillTree.Core.Utilities.ConfigManager;

namespace SkillTree.Core.Effects
{
    public class BloodMoney
    {
        private static readonly string EffectName = "BloodMoney";
        private static readonly int EffectTier = 3;
        public static bool IsBloodMoneyActive { get; private set; } = false;

        public static void ApplyToPlayer()
        {
            IsBloodMoneyActive = true;
            LogManager.LogMessage($"Blood Money effect applied", LogLevel.Debug);
            PlayerSingleton<PlayerCamera>.Instance.FoVChangeSmoother.AddOverride(BloodMoneyFOVChange.GetValue(UseDefault.GetValue()), EffectTier, EffectName);
            PlayerSingleton<PlayerCamera>.Instance.HeartbeatSoundController.VolumeController.AddOverride(BloodMoneyHeartbeatVolume.GetValue(UseDefault.GetValue()), EffectTier, EffectName);
            PlayerSingleton<PlayerCamera>.Instance.HeartbeatSoundController.PitchController.AddOverride(BloodMoneyHeartbeatPitch.GetValue(UseDefault.GetValue()), EffectTier, EffectName);
            Singleton<PostProcessingManager>.Instance.ColorFilterController.AddOverride(BloodMoneyScreenTint.GetValue(UseDefault.GetValue()), EffectTier, EffectName);
            Core.AddCoroutine(EffectName, MelonCoroutines.Start(ClearFromPlayer()));
        }

        public static IEnumerator ClearFromPlayer()
        {
            yield return new WaitForSeconds(BloodMoneyDuration.GetValue(UseDefault.GetValue()));
            ClearEffect();
            Core.RemoveCoroutine(EffectName);
        }

        public static void ClearEffect()
        {
            IsBloodMoneyActive = false;
            PlayerSingleton<PlayerCamera>.Instance.FoVChangeSmoother.RemoveOverride(EffectName);
            PlayerSingleton<PlayerCamera>.Instance.HeartbeatSoundController.VolumeController.RemoveOverride(EffectName);
            PlayerSingleton<PlayerCamera>.Instance.HeartbeatSoundController.PitchController.RemoveOverride(EffectName);
            Singleton<PostProcessingManager>.Instance.ColorFilterController.RemoveOverride(EffectName);
            LogManager.LogMessage($"Blood Money effect removed", LogLevel.Debug);
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
