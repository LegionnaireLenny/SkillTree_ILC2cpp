using HarmonyLib;
using Il2CppScheduleOne;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.PlayerScripts.Health;
using SkillTree.Core.Skills;
using SkillTree.Core.Utilities;
using UnityEngine;

namespace SkillTree.Core.Patches.Enforcer
{
    [HarmonyPatch(typeof(PlayerHealth))]
    public class HealthPatches
    {
        public static void SetPlayerHealth()
        {
            float original = Player.Local.Health.CurrentHealth;
            Player.Local.Health.SetHealth(SkillModifiers.GetPlayerMaxHealth());
            LogManager.LogMessage($"[Stats] Player max health changed from {original} to {Player.Local.Health.CurrentHealth}", LogLevel.Info);
        }

        [HarmonyPatch("MinPass")]
        [HarmonyPrefix]
        public static bool Prefix_MinPass(PlayerHealth __instance)
        {
            if (__instance.IsAlive &&
                __instance.CurrentHealth < SkillModifiers.GetPlayerMaxHealth() &&
                __instance.TimeSinceLastDamage > SkillModifiers.GetPlayerHealthRegenDelay())
            {
                float recoveredHealth = SkillModifiers.GetPlayerHealthRegen();
                __instance.RecoverHealth(recoveredHealth);
            }
            return false;
        }

        [HarmonyPatch("RecoverHealth")]
        [HarmonyPrefix]
        public static bool Prefix_RecoverHealth(PlayerHealth __instance, float recovery)
        {
            if (__instance.CurrentHealth <= 0f)
            {
                Console.LogWarning("RecoverHealth called on dead player. Use Revive() instead.", null);
                return false;
            }
            __instance.CurrentHealth = Mathf.Clamp(__instance.CurrentHealth + recovery, 0f, SkillModifiers.GetPlayerMaxHealth());
            __instance.onHealthChanged?.Invoke(__instance.CurrentHealth);
            LogManager.LogMessage($"Recovered {recovery} | Current {__instance.CurrentHealth} | Max {SkillModifiers.GetPlayerMaxHealth()}", LogLevel.DebugVerbose);
            return false;
        }

        [HarmonyPatch("RpcLogic___TakeDamage_3505310624")]
        [HarmonyPrefix]
        public static bool Prefix_RpcLogic_TakeDamage(PlayerHealth __instance, float damage, bool flinch = true, bool playBloodMist = true)
        {
            if (!__instance.IsAlive)
            {
                return false;
            }

            if (!__instance.CanTakeDamage)
            {
                Console.LogWarning("Player cannot take damage right now.", null);
                return false;
            }

            float original = __instance.CurrentHealth;
            float minHealth = Effects.BloodMoney.IsBloodMoneyActive ? 1f : 0f;
            __instance.CurrentHealth = Mathf.Clamp(__instance.CurrentHealth - damage, minHealth, SkillModifiers.GetPlayerMaxHealth());
            __instance.TimeSinceLastDamage = 0f;
            __instance.onHealthChanged?.Invoke(__instance.CurrentHealth);
            Effects.BloodMoney.GetBloodMoney(damage);

            if (__instance.Player.IsOwner)
            {
                if (flinch && PlayerSingleton<PlayerCamera>.InstanceExists)
                {
                    PlayerSingleton<PlayerCamera>.Instance.JoltCamera();
                }

                if (__instance.CurrentHealth <= 0f)
                {
                    __instance.SendDie();
                }
            }

            if (playBloodMist)
            {
                __instance.PlayBloodMist();
            }

            LogManager.LogMessage($"[Stats] Player health: {original} - {damage} = {__instance.CurrentHealth}", LogLevel.Info);
            return false;
        }

        [HarmonyPatch("SetHealth")]
        [HarmonyPrefix]
        public static bool Prefix_SetHealth(PlayerHealth __instance, float health)
        {
            if (health >= 100f)
            {
                health = SkillModifiers.GetPlayerMaxHealth();
            }

            __instance.CurrentHealth = Mathf.Clamp(health, 0f, SkillModifiers.GetPlayerMaxHealth());
            __instance.onHealthChanged?.Invoke(__instance.CurrentHealth);
            LogManager.LogMessage($"[Stats] Player health set to {__instance.CurrentHealth}. Maximum health {SkillModifiers.GetPlayerMaxHealth()}", LogLevel.Info);
            if (__instance.CurrentHealth <= 0f)
            {
                __instance.SendDie();
            }
            return false;
        }
    }
}