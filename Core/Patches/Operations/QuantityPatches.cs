using HarmonyLib;
using Il2CppFishNet;
using Il2CppScheduleOne.Growing;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.ObjectScripts;
using UnityEngine;

namespace SkillTree.Core.Patches.Operations
{
    [HarmonyPatch]
    public static class QuantityPatches
    {
        [HarmonyPatch(typeof(Cauldron), "RpcLogic___FinishCookOperation_2166136261")]
        [HarmonyPrefix]
        public static bool Prefix(Cauldron __instance)
        {
            if (Core.SkillData == null || (Core.SkillData.MoreCauldronOutput == 0 && Core.SkillData.MoreQualityMethCoca == 0))
                return true;

            if (InstanceFinder.IsServer)
            {
                QualityItemInstance qualityItemInstance = __instance.CocaineBaseDefinition.GetDefaultInstance(SkillModifiers.GetCauldronOutputBonus()) as QualityItemInstance;
                qualityItemInstance.SetQuality(ItemQuality.ShiftQuality(__instance.InputQuality, SkillModifiers.GetMethCocaProductQualityBonus()));
                __instance.OutputSlot.InsertItem(qualityItemInstance);
            }

            __instance.CauldronFillable.ResetContents();
            if (__instance.onCookEnd != null)
            {
                __instance.onCookEnd.Invoke();
            }

            return false;
        }

        [HarmonyPatch(typeof(MixingStation), "GetMixQuantity")]
        [HarmonyPostfix]
        public static void Postfix(MixingStation __instance, ref int __result)
        {
            if (__instance.GetProduct() == null || __instance.GetMixer() == null || Core.SkillData == null || Core.SkillData.MoreMixAndDryingRackOutput == 0)
                return;

            __result = Mathf.Min(Mathf.Min(__instance.ProductSlot.Quantity, __instance.MixerSlot.Quantity),
                __instance.MaxMixQuantity * SkillModifiers.GetMixDryOutputMultiplier());
        }

        [HarmonyPatch(typeof(DryingRack), "InitializeGridItem")]
        [HarmonyPostfix]
        public static void Postfix(DryingRack __instance)
        {
            if (Core.SkillData == null || Core.SkillData.MoreMixAndDryingRackOutput == 0)
                return;
            UpdateDryingRackCapacity(__instance);
        }

        [HarmonyPatch(typeof(DryingRack), "Open")]
        [HarmonyPrefix]
        public static void Prefix(DryingRack __instance)
        {
            if (Core.SkillData == null || Core.SkillData.MoreMixAndDryingRackOutput == 0)
                return;
            UpdateDryingRackCapacity(__instance);
        }

        private static void UpdateDryingRackCapacity(DryingRack rack)
        {
            int original = rack.ItemCapacity;
            rack.ItemCapacity = SkillModifiers.BaseDryingRackCapacity * SkillModifiers.MixDryOutputSizeMultiplier;

            if (original != rack.ItemCapacity)
                rack.RefreshHangingVisuals();
        }
    }
}

