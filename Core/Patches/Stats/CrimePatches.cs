using HarmonyLib;
using Il2CppScheduleOne.FX;
using Il2CppScheduleOne.NPCs.Behaviour;
using Il2CppScheduleOne.NPCs.CharacterClasses;
using Il2CppScheduleOne.PlayerScripts;
using MelonLoader;
using SkillTree.Core.FileManagement;
using System.Collections;
using UnityEngine;

namespace SkillTree.Core.Patches.Stats
{
    [HarmonyPatch(typeof(PursuitBehaviour))]
    public class CrimePatches
    {
        [HarmonyPatch("Activate")]
        [HarmonyPostfix]
        public static void Patch_Activate(PursuitBehaviour __instance)
        {
            if (SkillTreeData.Slippery.CurrentLevel == 0)
            {
                return;
            }

            __instance.officer.ProxCircle.SetRadius(SkillModifiers.GetArrestRadius());
        }

        [HarmonyPatch("Resume")]
        [HarmonyPostfix]
        public static void Patch_Resume(PursuitBehaviour __instance)
        {
            if (SkillTreeData.Slippery.CurrentLevel == 0)
            {
                return;
            }

            __instance.officer.ProxCircle.SetRadius(SkillModifiers.GetArrestRadius());
        }

        [HarmonyPatch("UpdateArrest")]
        [HarmonyPrefix]
        public static bool Patch_UpdateArrest(PursuitBehaviour __instance, float tick)
        {
            if (SkillTreeData.Slippery.CurrentLevel == 0 || __instance.TargetPlayer == null)
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

        [HarmonyPatch("UpdateArrestCircle")]
        [HarmonyPostfix]
        public static void Patch_UpdateArrestCircle(PursuitBehaviour __instance)
        {
            if (SkillTreeData.Slippery.CurrentLevel == 0 || __instance.TargetPlayer == null)
            {
                return;
            }

            float num = Vector3.Distance(__instance.TargetPlayer.Avatar.CenterPoint, __instance.transform.position);
            if (num < SkillModifiers.GetArrestRadius())
            {
                __instance.SetArrestCircleAlpha(__instance.ArrestCircle_MaxOpacity);
                __instance.SetArrestCircleColor(new Color32(byte.MaxValue, 50, 50, byte.MaxValue));
                return;
            }
            if (num < __instance.ArrestCircle_MaxVisibleDistance)
            {
                float num2 = Mathf.Lerp(__instance.ArrestCircle_MaxOpacity, 
                    0f, 
                    (num - SkillModifiers.GetArrestRadius()) / (__instance.ArrestCircle_MaxVisibleDistance - SkillModifiers.GetArrestRadius()));
                __instance.SetArrestCircleAlpha(num2);
                __instance.SetArrestCircleColor(Color.white);
                return;
            }
        }
    }
}