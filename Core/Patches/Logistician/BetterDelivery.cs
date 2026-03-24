using HarmonyLib;
using Il2CppScheduleOne.Delivery;
using MelonLoader;
using SkillTree.Core.Serialization;
using UnityEngine;

namespace SkillTree.Core.Patches.Logistician
{
    [HarmonyPatch(typeof(DeliveryManager), "SendDelivery")]
    public class BetterDelivery
    {
        [HarmonyPrefix]
        public static void Prefix(ref DeliveryInstance delivery)
        {
            if (delivery == null || delivery.TimeUntilArrival <= 120 || SkillTreeData.RushDelivery.CurrentLevel == 0)
                return;

            int originalTime = delivery.TimeUntilArrival;
            float ratio = Mathf.InverseLerp(60f, 360f, originalTime);
            int newTime = Mathf.RoundToInt(Mathf.Lerp(30f, 120f, ratio));

            delivery.TimeUntilArrival = newTime;
            MelonLogger.Msg($"[DeliverySkill] Delivery time adjusted from {originalTime}m to {newTime}m");
        }
    }
}