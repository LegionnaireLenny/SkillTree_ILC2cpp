using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Effects;
using Il2CppScheduleOne.FX;
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
        private static readonly string EffectName = "BloodMoney";
        private static readonly int EffectTier = 3;
        private static readonly Color EffectTint = new Color(1f, 1f, 0.9f, 0.2f);

        public static void ApplyToPlayer()
        {
            IsBloodMoneyActive = true;
            MelonLogger.Msg($"Blood Money effect applied");
            PlayerSingleton<PlayerCamera>.Instance.FoVChangeSmoother.AddOverride(10f, EffectTier, EffectName);
            PlayerSingleton<PlayerCamera>.Instance.HeartbeatSoundController.VolumeController.AddOverride(0.5f, EffectTier, EffectName);
            PlayerSingleton<PlayerCamera>.Instance.HeartbeatSoundController.PitchController.AddOverride(0.85f, EffectTier, EffectName);
            Singleton<PostProcessingManager>.Instance.ColorFilterController.AddOverride(EffectTint, EffectTier, EffectName);
            MelonCoroutines.Start(ClearFromPlayer());
        }

        public static IEnumerator ClearFromPlayer()
        {
            yield return new WaitForSeconds(ConfigManager.BloodMoneyDuration.GetValue());
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
