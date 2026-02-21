using HarmonyLib;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Effects;
using Il2CppScheduleOne.PlayerScripts;
using MelonLoader;
using UnityEngine;

namespace SkillTree.Core.Patches.Stats
{
    [HarmonyPatch]
    public static class MovementPatches
    {
        public static void SetPlayerSpeed()
        {
            float original = PlayerMovement.Instance.MoveSpeedMultiplier;
            PlayerMovement.Instance.MoveSpeedMultiplier = SkillModifiers.GetPlayerMoveSpeed();
            MelonLogger.Msg($"[MoreMovespeed] Player speed multiplier changed {original} to {PlayerMovement.Instance.MoveSpeedMultiplier}");
        }

        public static void SetPlayerJumpHeight()
        {
            PlayerMovement.JumpMultiplier = SkillModifiers.GetPlayerJumpHeight();
            MelonLogger.Msg($"Player jump multiplier changed from x{SkillModifiers.PlayerBaseJumpHeight} to x{PlayerMovement.JumpMultiplier}");
        }

        public static void SetPlayerStamina()
        {
            PlayerMovement.StaminaReserveMax = SkillModifiers.GetPlayerMaxStamina();
            MelonLogger.Msg($"Player max stamina changed from {SkillModifiers.PlayerBaseStamina} to {PlayerMovement.StaminaReserveMax}");
        }

        //WIP stamina regen patch
        //[HarmonyPatch(typeof(PlayerMovement), "Update")]
        //[HarmonyPostfix]
        //public static void Update_Postfix(PlayerMovement __instance)
        //{
        //    if (Core.SkillData == null || Core.SkillData.Stats == 0)
        //    {
        //        return;
        //    }

        //    if (__instance.timeSinceStaminaDrain > 1f && __instance.CurrentStaminaReserve < PlayerMovement.StaminaReserveMax)
        //    {
        //        //MelonLogger.Msg($"Regenerating stamina by an additional {25f * Time.deltaTime}");
        //        __instance.ChangeStamina(25f * Time.deltaTime, true);
        //    }
        //}

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