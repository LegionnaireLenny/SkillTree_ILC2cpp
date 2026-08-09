using HarmonyLib;
using Il2CppFishNet;
using Il2CppScheduleOne.NPCs.Behaviour;
using MelonLoader;
using SkillTree.Core.Serialization;
using SkillTree.Core.Skills;
using System.Collections;
using UnityEngine;

namespace SkillTree.Core.Patches.Logistician
{
    [HarmonyPatch]
    public class BotanistDryingRackPatches
    {
        [HarmonyPatch(typeof(StartDryingRackBehaviour), "RpcLogic___BeginAction_2166136261")]
        [HarmonyPrefix]
        public static bool RpcLogic___BeginAction_2166136261(StartDryingRackBehaviour __instance)
        {
            if (SkillTreeData.FastFarmers.CurrentLevel == 0)
            {
                return true;
            }

            if (__instance.WorkInProgress)
            {
                return false;
            }
            if (__instance.Rack == null)
            {
                return false;
            }
            __instance.WorkInProgress = true;
            __instance.Npc.Movement.FacePoint(__instance.Rack.uiPoint.position, 0.5f);
            __instance.workRoutine = (Coroutine)MelonCoroutines.Start(BeginAction(__instance));
            Core.AddCoroutine(__instance.ObjectId, __instance.workRoutine, __instance.Name);
            return false;
        }

        private static IEnumerator BeginAction(StartDryingRackBehaviour instance)
        {
            yield return new WaitForEndOfFrame();
            instance.Rack.InputSlot.ItemInstance.GetCopy(1);
            int itemCount = 0;
            while (instance.Rack != null && instance.Rack.InputSlot.Quantity > itemCount && instance.Rack.GetTotalDryingItems() + itemCount < instance.Rack.ItemCapacity)
            {
                instance.Npc.Avatar.Animation.SetTrigger("GrabItem");
                yield return new WaitForSeconds(1f * SkillModifiers.GetBotanistActionDurationMultiplier());
                int num = itemCount;
                itemCount = num + 1;
            }
            if (InstanceFinder.IsServer)
            {
                instance.Rack.StartOperation();
            }
            instance.WorkInProgress = false;
            instance.workRoutine = null;
            Core.RemoveCoroutine(instance.ObjectId, instance.Name);
            yield break;
        }
    }
}
