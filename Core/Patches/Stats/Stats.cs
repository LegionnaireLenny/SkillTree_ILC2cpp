using HarmonyLib;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.PlayerScripts.Health;
using MelonLoader;
using UnityEngine;
using static UnityEngine.UI.Image;

namespace SkillTree.Core.Patches.Stats
{
    [HarmonyPatch(typeof(PlayerHealth))]
    public class Stats
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
            if (__instance.IsAlive && __instance.CurrentHealth < SkillModifiers.GetPlayerMaxHealth() && __instance.TimeSinceLastDamage > 30f)
            {
                __instance.RecoverHealth(0.5f);
                MelonLogger.Msg($"Recovered {0.5f} health. Current health {__instance.CurrentHealth}. Max health {SkillModifiers.GetPlayerMaxHealth()}");

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
            MelonLogger.Msg($"Recovered {recovery} health. Current health {__instance.CurrentHealth}. Max health {SkillModifiers.GetPlayerMaxHealth()}");
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
            __instance.CurrentHealth = Mathf.Clamp(health, 0f, SkillModifiers.GetPlayerMaxHealth());
            __instance.onHealthChanged?.Invoke(__instance.CurrentHealth);
            MelonLogger.Msg($"[Stats] Trying to set health to {health}. Health after {__instance.CurrentHealth}.");
            if (__instance.CurrentHealth <= 0f)
            {
                __instance.SendDie();
            }
            return false;
        }
    }
}