using HarmonyLib;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.Tools;
using MelonLoader;
using SkillTree.Core.Skills;
using SkillTree.Core.Utilities;
using UnityEngine;
using static SkillTree.Core.Utilities.ConfigManager;

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
            LogManager.LogMessage($"Player speed multiplier changed from x{original} to x{PlayerMovement.Instance.MoveSpeedMultiplier}", LogLevel.Debug);
        }

        public static void SetPlayerJumpHeight()
        {
            float original = PlayerMovement.JumpMultiplier;
            PlayerMovement.JumpMultiplier = SkillModifiers.GetPlayerJumpHeight();
            LogManager.LogMessage($"Player jump multiplier changed from x{BaseJumpHeight.GetValue(UseDefault.GetValue())} to x{PlayerMovement.JumpMultiplier}", LogLevel.Debug);
        }

        public static void SetPlayerStamina()
        {
            PlayerMovement.StaminaReserveMax = SkillModifiers.GetPlayerMaxStamina();
            LogManager.LogMessage($"Player max stamina changed from {BaseStamina.GetValue(UseDefault.GetValue())} to {PlayerMovement.StaminaReserveMax}", LogLevel.Debug);
        }
    }
}