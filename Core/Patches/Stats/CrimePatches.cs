using HarmonyLib;
using Il2CppScheduleOne.NPCs.Behaviour;
using Il2CppScheduleOne.NPCs.CharacterClasses;
using Il2CppScheduleOne.PlayerScripts;
using MelonLoader;
using System.Collections;
using UnityEngine;

namespace SkillTree.Core.Patches.Stats
{
    [HarmonyPatch(typeof(PursuitBehaviour))]
    public class CrimePatches
    {
        [HarmonyPatch("UpdateArrest")]
        [HarmonyPrefix]
        public static bool Patch_UpdateArrest(PursuitBehaviour __instance, float tick)
        {
            if (Core.SkillData == null || Core.SkillData.Slippery == 0 || __instance.TargetPlayer == null)
            {
                return true;
            }

            if (Vector3.Distance(__instance.transform.position, __instance.TargetPlayer.Avatar.CenterPoint) < SkillModifiers.GetArrestRadius() && __instance.arrestingEnabled && __instance.IsTargetRecentlyVisible)
            {
                __instance.timeWithinArrestRange += tick;
                if (__instance.timeWithinArrestRange > 0.5f)
                {
                    __instance.wasInArrestCircleLastFrame = true;
                }
            }
            else
            {
                if (__instance.wasInArrestCircleLastFrame)
                {
                    __instance.leaveArrestCircleCount++;
                    __instance.wasInArrestCircleLastFrame = false;
                }
                __instance.timeWithinArrestRange = Mathf.Clamp(__instance.timeWithinArrestRange - tick, 0f, float.MaxValue);
            }

            if (__instance.TargetPlayer.IsOwner && __instance.timeWithinArrestRange / SkillModifiers.GetArrestTime() > __instance.TargetPlayer.CrimeData.CurrentArrestProgress)
            {
                __instance.TargetPlayer.CrimeData.SetArrestProgress(__instance.timeWithinArrestRange / SkillModifiers.GetArrestTime());
            }

            return false;
        }
    }
}