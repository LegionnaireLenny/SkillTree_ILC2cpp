//using HarmonyLib;
//using Il2CppScheduleOne.PlayerScripts;
//using MelonLoader;
//using System.Collections;
//using UnityEngine;

//namespace SkillTree.Core.Patches.Stats
//{
//    [HarmonyPatch(typeof(Player))]
//    public class PlayerPatches
//    {
//        [HarmonyPatch("RpcLogic___Taze_2166136261")]
//        [HarmonyPrefix]
//        public static bool Tazed_Patch(Player __instance)
//        {
//            __instance.IsTased = true;
//            if (__instance.onTased != null)
//            {
//                __instance.onTased.Invoke();
//            }
//            if (__instance.taseCoroutine != null)
//            {
//                __instance.StopCoroutine(__instance.taseCoroutine);
//            }
//            __instance.Health.TakeDamage(1f, true, false);
//            __instance.taseCoroutine = (Coroutine)MelonCoroutines.Start(TasePlayer(__instance, 2f));

//            return false;
//        }

//        private static IEnumerator TasePlayer(Player instance, float taseDuration)
//		{
//            instance.Avatar.Effects.SetZapped(true, true);
//			yield return new WaitForSeconds(taseDuration);
//            instance.Avatar.Effects.SetZapped(false, true);
//            instance.IsTased = false;
//			if (instance.onTasedEnd != null)
//            {
//                instance.onTasedEnd.Invoke();
//            }
//            yield break;
//        }
//    }
//}