using HarmonyLib;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Law;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.UI;
using MelonLoader;
using SkillTree.Core.Serialization;
using SkillTree.Core.Skills;
using UnityEngine;

namespace SkillTree.Core.Patches.Enforcer
{
    [HarmonyPatch]
    public class PlayerCrimeDataPatches
    {
        [HarmonyPatch(typeof(PlayerCrimeData), "GetSearchTime")]
        [HarmonyPostfix]
        public static void Patch_GetSearchTime(ref float __result)
        {
            if (SkillTreeData.PileTheBodiesHigh.CurrentLevel == 0) return;

            __result = SkillModifiers.GetPoliceSearchTime(__result);
        }

        [HarmonyPatch(typeof(PlayerCrimeData), "UpdateTimeout")]
        [HarmonyPrefix]
        public static bool Patch_UpdateTimeout(PlayerCrimeData __instance)
        {
            if (!__instance.Player.IsOwner)
            {
                return false;
            }
            if (__instance.TimeSinceSighted > __instance.GetSearchTime() + 3f)
            {
                __instance.TimeoutPursuit();
            }
            return false;
        }

        [HarmonyPatch(typeof(PlayerCrimeData), "Update")]
        [HarmonyPrefix]
        public static bool Patch_Update(PlayerCrimeData __instance)
        {
            __instance.CurrentPursuitLevelDuration += Time.deltaTime;
            __instance.TimeSincePursuitStart += Time.deltaTime;
            __instance.TimeSinceSighted += Time.deltaTime;
            __instance.TimeSinceLastBodySearch += Time.deltaTime;
            if (!__instance.Player.IsOwner)
            {
                return false;
            }
            if (__instance.CurrentPursuitLevel != PlayerCrimeData.EPursuitLevel.None && __instance.CurrentPursuitLevel != PlayerCrimeData.EPursuitLevel.Lethal)
            {
                __instance.UpdateEscalation();
            }
            if (__instance.CurrentPursuitLevel != PlayerCrimeData.EPursuitLevel.None)
            {
                __instance.UpdateTimeout();
            }
            for (int i = 0; i < __instance.Collisions.Count; i++)
            {
                __instance.Collisions[i].TimeSince += Time.deltaTime;
                if (__instance.Collisions[i].TimeSince > 30f)
                {
                    __instance.Collisions.RemoveAt(i);
                    i--;
                }
            }
            Singleton<HUD>.Instance.CrimeStatusUI.UpdateStatus();
            if ((float)__instance.Collisions.Count >= 3f)
            {
                __instance.RecordLastKnownPosition(true);
                __instance.SetPursuitLevel(PlayerCrimeData.EPursuitLevel.Investigating);
                __instance.AddCrime(new VehicularAssault(), __instance.Collisions.Count - 1);
                Singleton<LawManager>.Instance.PoliceCalled(__instance.Player, new VehicularAssault());
                __instance.Collisions.Clear();
            }
            return false;
        }
    }
}
