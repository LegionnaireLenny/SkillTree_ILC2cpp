using HarmonyLib;
using Il2CppFishNet;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Levelling;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.Variables;
using MelonLoader;
using UnityEngine;
using static Il2CppScheduleOne.ObjectScripts.Pot;

namespace SkillTree.Core.Patches.Operations
{
    //Trying to update the racks when they're placed
    //[HarmonyPatch(typeof(DryingRack), "Initialize")]
    //public static class DryingRack_Placement_Patch
    //{
    //    [HarmonyPostfix]
    //    public static void Postfix(DryingRack __instance)
    //    {
    //        MelonLogger.Msg($"[DryingRack] Updating rack capacity.");
    //        ApplyCapacityUpdate(__instance);
    //    }

    //    public static void ApplyCapacityUpdate(DryingRack __instance)
    //    {
    //        if (Core.SkillData.MoreMixAndDryingRackOutput == 0)
    //            return;

    //        __instance.ItemCapacity *= SkillModifiers.MixDryOutputSizeMultiplier;
    //        __instance.RefreshHangingVisuals();
    //    }
    //}

    /// <summary>
    /// ABSORBENT SOIL
    /// </summary>

    [HarmonyPatch(typeof(Pot), "OnPlantFullyHarvested")]
    public static class AbsorbentSoil
    {
        private static readonly HashSet<int> processedIds = new HashSet<int>();

        [HarmonyPrefix]
        public static bool Prefix(Pot __instance)
        {
            MelonLogger.Msg($"Pot_OnPlantFullyHarvested_Patch enter: AbsorbentSoil {Core.SkillData.AbsorbentSoil}");

            if (Core.SkillData == null || Core.SkillData.AbsorbentSoil == 0)
                return true;

            if (__instance.Plant == null)
                return false;

            try
            {
                //var traverse = Traverse.Create(__instance);
                if (InstanceFinder.IsServer)
                {
                    float value = NetworkSingleton<VariableDatabase>.Instance
                        .GetValue<float>("HarvestedPlantCount");

                    NetworkSingleton<VariableDatabase>.Instance
                        .SetVariableValue("HarvestedPlantCount", (value + 1f).ToString());

                    NetworkSingleton<LevelManager>.Instance.AddXP(5);

                    MelonLogger.Msg("Server harvest processed");
                }

                //traverse.Property("Plant")?.SetValue(null);

                int id = __instance.GetInstanceID();
                if (processedIds.Contains(id)) return false;

                MelonLogger.Msg($"RemainingUses before remainingUses {__instance._remainingSoilUses}");
                int remainingUses = __instance._remainingSoilUses - 1;
                MelonLogger.Msg($"RemainingUses after remainingUses {remainingUses}");
                __instance.SetRemainingSoilUses(remainingUses);

                __instance.SetSoilState(ESoilState.Flat);

                processedIds.Add(id);

                if (remainingUses <= 0)
                {
                    MelonLogger.Msg("Soil depleted: clearing soil and additives");

                    __instance.ClearAdditives();
                    __instance.ClearSoil();
                }
                else
                {
                    MelonLogger.Msg("Soil still usable: additives preserved");
                }
                MelonCoroutines.Start(CleanUp(id));
                return false;
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"OnPlantFullyHarvested patch failed: {ex}");
                return true;
            }
        }
        private static System.Collections.IEnumerator CleanUp(int id)
        {
            yield return new WaitForSeconds(2f);
            processedIds.Remove(id);
        }
    }



}
