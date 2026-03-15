using HarmonyLib;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Effects;
using Il2CppScheduleOne.PlayerScripts;
using MelonLoader;
using SkillTree.Core.Skills;
using SkillTree.Core.Utilities;
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

            if (!Mathf.Approximately(original, PlayerMovement.Instance.MoveSpeedMultiplier))
            {
                MelonLogger.Msg($"Player speed multiplier changed from x{original} to x{PlayerMovement.Instance.MoveSpeedMultiplier}");
            }
        }

        public static void SetPlayerJumpHeight()
        {
            float original = PlayerMovement.JumpMultiplier;
            PlayerMovement.JumpMultiplier = SkillModifiers.GetPlayerJumpHeight();
            if (!Mathf.Approximately(original, PlayerMovement.JumpMultiplier))
            {
                MelonLogger.Msg($"Player jump multiplier changed from x{ConfigManager.BaseJumpHeight.GetValue()} to x{PlayerMovement.JumpMultiplier}");
            }
        }

        public static void SetPlayerStamina()
        {
            PlayerMovement.StaminaReserveMax = SkillModifiers.GetPlayerMaxStamina();
            if (!Mathf.Approximately(ConfigManager.BaseStamina.GetValue(), PlayerMovement.StaminaReserveMax))
            {
                MelonLogger.Msg($"Player max stamina changed from {ConfigManager.BaseStamina.GetValue()} to {PlayerMovement.StaminaReserveMax}");
            }

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

        [HarmonyPatch(typeof(Sneaky), "ApplyToPlayer")]
        [HarmonyPostfix]
        public static void Sneaky_Apply_Postfix()
        {
            PlayerSingleton<PlayerMovement>.Instance.MoveSpeedMultiplier = SkillModifiers.GetPlayerMoveSpeed() + (Sneaky.SPEED_MULTIPLIER - 1f);
        }

        [HarmonyPatch(typeof(Sneaky), "ClearFromPlayer")]
        [HarmonyPostfix]
        public static void Sneaky_Clear_Postfix()
        {
            PlayerSingleton<PlayerMovement>.Instance.MoveSpeedMultiplier = SkillModifiers.GetPlayerMoveSpeed();
        }
    }
}