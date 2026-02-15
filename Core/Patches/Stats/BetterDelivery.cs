using HarmonyLib;
using MelonLoader;
using Il2CppScheduleOne.Delivery;
using UnityEngine;

namespace SkillTree.Core.Patches.Stats
{
    [HarmonyPatch(typeof(DeliveryManager), "SendDelivery")]
    public class BetterDelivery
    {
        [HarmonyPrefix]
        public static void Prefix(ref DeliveryInstance delivery)
        {
            if (delivery == null || delivery.TimeUntilArrival <= 120 || Core.SkillData == null || Core.SkillData.BetterDelivery == 0)
                return;

            int originalTime = delivery.TimeUntilArrival;
            float ratio = Mathf.InverseLerp(60f, 360f, originalTime);
            int newTime = Mathf.RoundToInt(Mathf.Lerp(30f, 120f, ratio));

            delivery.TimeUntilArrival = newTime;
            MelonLogger.Msg($"[DeliverySkill] Delivery time adjusted from {originalTime}m to {newTime}m");
        }
    }
}