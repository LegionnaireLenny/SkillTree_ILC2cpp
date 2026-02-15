using HarmonyLib;
using Il2CppFishNet;
using Il2CppScheduleOne.Growing;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.ObjectScripts;
using MelonLoader;
using UnityEngine;

namespace SkillTree.Core.Patches.Operations
{
    [HarmonyPatch(typeof(Plant), "GrowthDone")]
    public static class MoreYield
    {
        [HarmonyPrefix]
        public static void Prefix(Plant __instance)
        {
            if (!InstanceFinder.IsServer || Core.SkillData == null || Core.SkillData.MoreYield == 0)
                return;

            var currentMultiplier = __instance.YieldMultiplier;
            var originalBase = __instance.BaseYieldQuantity;

            MelonLogger.Msg($"[GrowthDone_SmartBasePatch] Yield multiplier {__instance.YieldMultiplier}. Base yield: {__instance.BaseYieldQuantity}");
            if (Mathf.Approximately(currentMultiplier, 1.0f) && originalBase == 12)
            {
                int finalBase = originalBase + SkillModifiers.YieldBonusPlants; 

                __instance.BaseYieldQuantity = finalBase; 
                MelonLogger.Msg($"[GrowthDone_SmartBasePatch] No additives detected. Skill applied. New Base: {finalBase}");
            }
        }

        [HarmonyPatch(typeof(Cauldron), "RpcLogic___FinishCookOperation_2166136261")]
        [HarmonyPostfix]
        public static void Postfix(Cauldron __instance)
        {
            if (Core.SkillData == null || Core.SkillData.MoreCauldronOutput == 0)
                return;

            if (InstanceFinder.IsServer)
            {
                QualityItemInstance qualityItemInstance = __instance.CocaineBaseDefinition.GetDefaultInstance(10) as QualityItemInstance;
                qualityItemInstance.SetQuality(__instance.InputQuality);
                __instance.OutputSlot.InsertItem(qualityItemInstance);
            }
        }

        //// TODO: Cauldron_Double_Output_Patch, incorrectly activates on chemistry station
        //[HarmonyPatch(typeof(QualityItemDefinition), "GetDefaultInstance", typeof(int))]
        //public static class Patch_Cauldron_Double_Output
        //{
        //    [HarmonyPrefix]
        //    public static void Prefix(QualityItemDefinition __instance, ref int quantity)
        //    {

        //        if (Core.SkillData.MoreCauldronOutput == 0) 
        //            return;

        //        if (quantity != SkillModifiers.CauldronBaseOutput)
        //            return;

        //        MelonLogger.Msg($"Cauldron_Double_Output_Patch enter: MoreCauldronOutput {Core.SkillData.MoreCauldronOutput} quantity {quantity} base {SkillModifiers.CauldronBaseOutput}");
        //        if (__instance.name.Contains("CocaineBase"))
        //        {
        //            quantity = SkillModifiers.GetCauldronStackSize(); 
        //        }
        //    }
        //}

        [HarmonyPatch(typeof(MixingStation), "GetMixQuantity")]
        [HarmonyPostfix]
        public static void Postfix(MixingStation __instance, ref int __result)
        {

            if (__instance.GetProduct() == null || __instance.GetMixer() == null || Core.SkillData == null || Core.SkillData.MoreMixAndDryingRackOutput == 0)
                return;

            __result = Mathf.Min(Mathf.Min(__instance.ProductSlot.Quantity, __instance.MixerSlot.Quantity) * SkillModifiers.GetMixDryOutputMultiplier(),
                __instance.MaxMixQuantity * SkillModifiers.GetMixDryOutputMultiplier());
        }

        [HarmonyPatch(typeof(DryingRack), "InitializeGridItem")]
        [HarmonyPostfix]
        public static void Postfix(DryingRack __instance)
        {
            //MelonLogger.Msg($"[DryingRack] Updating rack capacity.");
            ApplyCapacityUpdate(__instance);
        }

        public static void ApplyCapacityUpdate(DryingRack __instance)
        {
            if (Core.SkillData == null || Core.SkillData.MoreMixAndDryingRackOutput == 0)
                return;

            __instance.ItemCapacity *= SkillModifiers.MixDryOutputSizeMultiplier;
            __instance.RefreshHangingVisuals();
        }
    }
}

