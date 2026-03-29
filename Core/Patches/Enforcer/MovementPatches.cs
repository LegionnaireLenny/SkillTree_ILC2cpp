using HarmonyLib;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.Tools;
using MelonLoader;
using SkillTree.Core.Skills;
using SkillTree.Core.Utilities;
using UnityEngine;

namespace SkillTree.Core.Patches.Enforcer
{
    [HarmonyPatch]
    public static class MovementPatches
    {
        public static void SetPlayerSpeed()
        {
            float original = PlayerMovement.Instance.MoveSpeedMultiplier;
            PlayerMovement.Instance.MoveSpeedMultiplierStack.Remove("SkillTree_FleetFeet");
            PlayerMovement.Instance.MoveSpeedMultiplierStack.Add(new FloatStack.StackEntry("SkillTree_FleetFeet", SkillModifiers.GetFleetFeetMoveSpeedMultiplier(), FloatStack.EStackMode.Multiplicative, 5));

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
    }
}