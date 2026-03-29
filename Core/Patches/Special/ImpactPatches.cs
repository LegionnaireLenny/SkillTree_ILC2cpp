using HarmonyLib;
using Il2CppScheduleOne.Combat;
using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.PlayerScripts;

namespace SkillTree.Core.Patches.Special
{
    [HarmonyPatch]
    public class ImpactPatches
    {
        [HarmonyPatch(typeof(NPC), "RpcLogic___ReceiveImpact_427288424")]
        [HarmonyPostfix]
        public static void RpcLogic___ReceiveImpact_427288424(NPC __instance, Impact impact)
        {
            if (!__instance.Health.IsDead && !__instance.Health.IsKnockedOut && impact.IsPlayerImpact(out Player player))
            {
                if (player == Player.Local)
                {
                    Effects.BloodMoney.GetBloodMoney(impact.ImpactDamage);
                }
            }
        }
    }
}
