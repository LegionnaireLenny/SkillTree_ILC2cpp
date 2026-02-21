using HarmonyLib;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.PlayerScripts.Health;
using MelonLoader;
using UnityEngine;

namespace SkillTree.Core.Patches.Stats
{
    [HarmonyPatch(typeof(PlayerHealth))]
    public class HealthPatches
    {
        public static void SetPlayerHealth()
        {
            if (Core.SkillData.Stats == 0)
            {
                return;
            }

            float original = Player.Local.Health.CurrentHealth;
            Player.Local.Health.SetHealth(SkillModifiers.GetPlayerMaxHealth());
            MelonLogger.Msg($"[Stats] Player max health changed from {original} to {Player.Local.Health.CurrentHealth} ");
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
                Il2CppScheduleOne.Console.LogWarning("RecoverHealth called on dead player. Use Revive() instead.", null);
                return false;
            }
            __instance.CurrentHealth = Mathf.Clamp(__instance.CurrentHealth + recovery, 0f, SkillModifiers.GetPlayerMaxHealth());
            __instance.onHealthChanged?.Invoke(__instance.CurrentHealth);
            MelonLogger.Msg($"Health => Recovered {recovery} | Current {__instance.CurrentHealth} | Max {SkillModifiers.GetPlayerMaxHealth()}");
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
                Il2CppScheduleOne.Console.LogWarning("Player cannot take damage right now.", null);
                return false;
            }

            MelonLogger.MsgPastel($"[Stats] Player health: {__instance.CurrentHealth} - {damage} = {__instance.CurrentHealth - damage}");
            __instance.CurrentHealth = Mathf.Clamp(__instance.CurrentHealth - damage, 0f, SkillModifiers.GetPlayerMaxHealth());
            __instance.TimeSinceLastDamage = 0f;
            __instance.onHealthChanged?.Invoke(__instance.CurrentHealth);

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

            return false;
        }

        [HarmonyPatch("SetHealth")]
        [HarmonyPrefix]
        public static bool Prefix_SetHealth(PlayerHealth __instance, float health)
        {
            if (Mathf.Approximately(health, SkillModifiers.PlayerBaseHealth))
            {
                health = SkillModifiers.GetPlayerMaxHealth();
            }

            __instance.CurrentHealth = Mathf.Clamp(health, 0f, SkillModifiers.GetPlayerMaxHealth());
            __instance.onHealthChanged?.Invoke(__instance.CurrentHealth);
            MelonLogger.Msg($"[Stats] Trying to set health to {health}. Health after {__instance.CurrentHealth}. Max health {SkillModifiers.GetPlayerMaxHealth()}");
            if (__instance.CurrentHealth <= 0f)
            {
                __instance.SendDie();
            }
            return false;
        }
    }
}