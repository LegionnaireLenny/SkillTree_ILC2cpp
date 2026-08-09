using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.Tools;
using MelonLoader;
using SkillTree.Core.Utilities;
using System.Collections;
using UnityEngine;
using static SkillTree.Core.Utilities.ConfigManager;

namespace SkillTree.Core.Effects
{
    public class AdrenalineSurge
    {
        private static readonly string EffectName = "AdrenalineSurge";
        public static bool IsAdrenalineSurgeActive { get; private set; } = false;

        public static void ApplyToPlayer()
        {
            IsAdrenalineSurgeActive = true;
            PlayerMovement.Instance.MoveSpeedMultiplierStack.Add(new FloatStack.StackEntry("SkillTree_AdrenalineSurge", AdrenalineSurgeSpeedMultiplier.GetValue(UseDefault.GetValue()), FloatStack.EStackMode.Multiplicative, 5));
            Patches.Enforcer.MovementPatches.SetPlayerJumpHeight();
            if (AdrenalineSurgeZappedEffect.GetValue(UseDefault.GetValue()))
            {
                Player.Local.Avatar.Effects.SetZapped(true, true);
            }
            LogManager.LogMessage($"Adrenaline Surge effect applied", LogLevel.Debug);
            Core.AddCoroutine(EffectName, MelonCoroutines.Start(ClearFromPlayer()));
        }

        public static IEnumerator ClearFromPlayer()
        {
            yield return new WaitForSeconds(AdrenalineSurgeDuration.GetValue(UseDefault.GetValue()));
            ClearEffect();
            Core.RemoveCoroutine(EffectName);
        }

        public static void ClearEffect()
        {
            IsAdrenalineSurgeActive = false;
            PlayerMovement.Instance.MoveSpeedMultiplierStack.Remove("SkillTree_AdrenalineSurge");
            Patches.Enforcer.MovementPatches.SetPlayerJumpHeight();
            Player.Local.Avatar.Effects.SetZapped(false, true);
            LogManager.LogMessage($"Adrenaline Surge effect removed", LogLevel.Debug);
        }
    }
}
