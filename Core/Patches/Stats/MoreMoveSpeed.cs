using HarmonyLib;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Effects;
using Il2CppScheduleOne.PlayerScripts;
using MelonLoader;

namespace SkillTree.Core.Patches.Stats
{
    [HarmonyPatch]
    public static class MoreMoveSpeed
    {
        public static void SetPlayerSpeed()
        {
            float original = PlayerMovement.Instance.MoveSpeedMultiplier;
            PlayerMovement.Instance.MoveSpeedMultiplier = SkillModifiers.GetPlayerMoveSpeed();
            MelonLogger.Msg($"[MoreMovespeed] Player speed multiplier changed {original} to {PlayerMovement.Instance.MoveSpeedMultiplier}");
        }

        [HarmonyPatch(typeof(Athletic), "ApplyToPlayer")]
        [HarmonyPostfix]
        public static void Athletic_Apply_Postfix()
        {
            PlayerSingleton<PlayerMovement>.Instance.MoveSpeedMultiplier = SkillModifiers.GetPlayerMoveSpeed() + (Athletic.SPEED_MULTIPLIER - 1f);
        }

        [HarmonyPatch(typeof(Athletic), "ClearFromPlayer")]
        [HarmonyPostfix]
        public static void Athletic_Clear_Postfix()
        {
            PlayerSingleton<PlayerMovement>.Instance.MoveSpeedMultiplier = SkillModifiers.GetPlayerMoveSpeed();
        }

        [HarmonyPatch(typeof(Energizing), "ApplyToPlayer")]
        [HarmonyPostfix]
        public static void Energizing_Apply_Postfix()
        {
            PlayerSingleton<PlayerMovement>.Instance.MoveSpeedMultiplier = SkillModifiers.GetPlayerMoveSpeed() + (Energizing.SPEED_MULTIPLIER - 1f);
        }

        [HarmonyPatch(typeof(Energizing), "ClearFromPlayer")]
        [HarmonyPostfix]
        public static void Energizing_Clear_Postfix()
        {
            PlayerSingleton<PlayerMovement>.Instance.MoveSpeedMultiplier = SkillModifiers.GetPlayerMoveSpeed();
        }
    }
}