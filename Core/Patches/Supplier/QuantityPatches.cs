using HarmonyLib;
using Il2CppFishNet;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.ObjectScripts;
using SkillTree.Core.Serialization;
using SkillTree.Core.Skills;
using UnityEngine;

namespace SkillTree.Core.Patches.Supplier
{
    [HarmonyPatch]
    public static class QuantityPatches
    {

        [HarmonyPatch(typeof(Cauldron), "OnTimePass")]
        [HarmonyPrefix]
        public static bool Prefix(Cauldron __instance, int minutes)
        {
            if (__instance == null || SkillTreeData.MoreCauldronOutput.CurrentLevel == 0 && SkillTreeData.MoreQualityMethCoca.CurrentLevel == 0)
                return true;

            minutes *= SkillModifiers.GetChemistStationSpeedMultiplier();
            if (__instance.RemainingCookTime > 0)
            {
                __instance.Alarm.SetScreenLit(true);
                __instance.Alarm.DisplayMinutes(__instance.RemainingCookTime);
                __instance.Light.isOn = true;
                __instance.RemainingCookTime -= minutes;
                if (__instance.RemainingCookTime <= 0 && InstanceFinder.IsServer)
                {
                    if (InstanceFinder.IsServer)
                    {
                        QualityItemInstance qualityItemInstance = new QualityItemInstance(
                            __instance.CocaineBaseDefinition,
                            SkillModifiers.GetCauldronOutput(),
                            ItemQuality.ShiftQuality(__instance.InputQuality, SkillModifiers.GetMethCocaProductQualityBonus()));
                        __instance.OutputSlot.InsertItem(qualityItemInstance);
                    }

                    __instance.CauldronFillable.ResetContents();
                    if (__instance.onCookEnd != null)
                    {
                        __instance.onCookEnd?.Invoke();
                    }

                    return false;
                }
            }
            else
            {
                __instance.Alarm.SetScreenLit(false);
                __instance.Alarm.DisplayMinutes(0);
                if (__instance.OutputSlot.Quantity > 0)
                {
                    __instance.Light.isOn = NetworkSingleton<Il2CppScheduleOne.GameTime.TimeManager>.Instance.CurrentTime % 2 == 0;
                    return false;
                }
                __instance.Light.isOn = false;
            }
            return false;
        }

        [HarmonyPatch(typeof(MixingStation), "GetMixQuantity")]
        [HarmonyPostfix]
        public static void Postfix(MixingStation __instance, ref int __result)
        {
            if (__instance?.GetProduct() == null || __instance?.GetMixer() == null || SkillTreeData.MoreMixAndDryingRackOutput.CurrentLevel == 0)
                return;

            __result = Mathf.Min(Mathf.Min(__instance.ProductSlot.Quantity, __instance.MixerSlot.Quantity),
                __instance.MaxMixQuantity * SkillModifiers.GetMixDryOutputMultiplier());
        }

        [HarmonyPatch(typeof(DryingRack), "InitializeGridItem")]
        [HarmonyPostfix]
        public static void Postfix(DryingRack __instance)
        {
            if (__instance == null || SkillTreeData.MoreMixAndDryingRackOutput.CurrentLevel == 0)
                return;
            UpdateDryingRackCapacity(__instance);
        }

        [HarmonyPatch(typeof(DryingRack), "Open")]
        [HarmonyPrefix]
        public static void Prefix(DryingRack __instance)
        {
            if (__instance == null || SkillTreeData.MoreMixAndDryingRackOutput.CurrentLevel == 0)
                return;
            UpdateDryingRackCapacity(__instance);
        }

        private static void UpdateDryingRackCapacity(DryingRack rack)
        {
            int original = rack.ItemCapacity;
            rack.ItemCapacity = SkillModifiers.GetDryingRackCapacity();

            if (original != rack.ItemCapacity)
                rack.RefreshHangingVisuals();
        }
    }
}

