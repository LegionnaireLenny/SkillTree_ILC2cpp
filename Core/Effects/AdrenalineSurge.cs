using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.Tools;
using MelonLoader;
using SkillTree.Core.Utilities;
using System.Collections;
using UnityEngine;

namespace SkillTree.Core.Effects
{
    public class AdrenalineSurge
    {
        public static bool IsAdrenalineSurgeActive { get; set; } = false;

        public static void ApplyToPlayer()
        {
            IsAdrenalineSurgeActive = true;
            PlayerMovement.Instance.MoveSpeedMultiplierStack.Add(new FloatStack.StackEntry("SkillTree_AdrenalineSurge", ConfigManager.AdrenalineSurgeSpeedMultiplier.GetValue(), FloatStack.EStackMode.Multiplicative, 5));
            Patches.Enforcer.MovementPatches.SetPlayerJumpHeight();
            if (ConfigManager.AdrenalineSurgeZappedEffect.GetValue())
            {
                Player.Local.Avatar.Effects.SetZapped(true, true);
            }
            MelonLogger.Msg($"Adrenaline Surge effect applied");
            MelonCoroutines.Start(ClearFromPlayer());
        }

        public static IEnumerator ClearFromPlayer()
        {
            yield return new WaitForSeconds(ConfigManager.AdrenalineSurgeDuration.GetValue());
            IsAdrenalineSurgeActive = false;
            PlayerMovement.Instance.MoveSpeedMultiplierStack.Remove("SkillTree_AdrenalineSurge");
            Patches.Enforcer.MovementPatches.SetPlayerJumpHeight();
            Player.Local.Avatar.Effects.SetZapped(false, true);
            MelonLogger.Msg($"Adrenaline Surge effect removed");
        }
    }
}
