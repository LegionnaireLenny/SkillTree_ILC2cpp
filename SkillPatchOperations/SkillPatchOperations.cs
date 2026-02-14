using HarmonyLib;
using Il2CppFishNet;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.GameTime;
using Il2CppScheduleOne.Growing;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.Levelling;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.Product;
using Il2CppScheduleOne.StationFramework;
using Il2CppScheduleOne.Variables;
using MelonLoader;
using SkillTree.Json;
using System.Reflection;
using UnityEngine;
using static Il2CppScheduleOne.ObjectScripts.Pot;

namespace SkillTree.SkillPatchOperations
{
    /// <summary>
    /// INCREASE QUALITY METH
    /// </summary>
    /// 

    [HarmonyPatch(typeof(LabOven), "Shatter")]
    public static class LabOven_QualityPatch
    {
        private static readonly HashSet<object> processedOperations = [];

        [HarmonyPrefix]
        public static void Prefix(LabOven __instance)
        {
            if (__instance.CurrentOperation == null || Core.SkillData == null || Core.SkillData.MoreQualityMethCoca == 0)
                return;

            if (processedOperations.Contains(__instance.CurrentOperation))
                return;

            MelonLogger.Msg($"LabOven_QualityPatch meth quality: {__instance.CurrentOperation.IngredientQuality}");
            if (__instance.CurrentOperation.IngredientQuality < EQuality.Heavenly)
            {
                //MelonLogger.Msg($"__instance.CurrentOperation.IngredientQuality {__instance.CurrentOperation.IngredientQuality}");
                __instance.CurrentOperation.IngredientQuality += 1;
                processedOperations.Add(__instance.CurrentOperation);
                //MelonLogger.Msg($"__instance.CurrentOperation.IngredientQuality {__instance.CurrentOperation.IngredientQuality}");
                MelonCoroutines.Start(CleanUp(__instance.CurrentOperation));
            }
        }

        private static System.Collections.IEnumerator CleanUp(object id)
        {
            yield return new WaitForSeconds(1f);
            processedOperations.Remove(id);
        }
    }


    [HarmonyPatch]
    public static class CraftingStationSpeedPatches
    {
        // TODO: test
        [HarmonyPatch(typeof(Cauldron), "OnTimePass")]
        [HarmonyPrefix]
        public static void Prefix(Cauldron __instance, ref int minutes)
        {
            if (__instance.RemainingCookTime <= 0 || Core.SkillData == null || Core.SkillData.ChemistStationQuick == 0)
                return;

            minutes *= SkillModifiers.GetChemistStationSpeedMultiplier();
            MelonLogger.Msg($"Patch_Cauldron_OnTimePass progress {minutes} minutes");
        }

        private static bool blockChemistryOnTimePassSecondExecution = false;
        [HarmonyPatch(typeof(ChemistryStation), "OnTimePass")]
        [HarmonyPostfix]
        public static void Postfix(ChemistryStation __instance, int minutes)
        {
            if (__instance.CurrentCookOperation == null || Core.SkillData == null || Core.SkillData.ChemistStationQuick == 0)
                return;

            if (blockChemistryOnTimePassSecondExecution)
            {
                blockChemistryOnTimePassSecondExecution = false;
                return;
            }

            // Reduce the multiplier by one to account for Progress being called in the original function
            __instance.CurrentCookOperation.Progress(minutes * (SkillModifiers.GetChemistStationSpeedMultiplier() - 1));
            blockChemistryOnTimePassSecondExecution = true;
        }

        [HarmonyPatch(typeof(OvenCookOperation), "GetCookDuration")]
        [HarmonyPostfix]
        public static void Postfix(OvenCookOperation __instance, ref int __result)
        {
            if (Core.SkillData == null || Core.SkillData.ChemistStationQuick == 0)
                return;

            __result = __instance.Ingredient.StationItem.GetModule<CookableModule>().CookTime / SkillModifiers.GetChemistStationSpeedMultiplier();
        }

        // TODO: fix. Doesn't work. Mix timer goes into negative and completes at the normal time
        private static bool blockMixingOnTimePassSecondExecution = false;
        [HarmonyPatch(typeof(MixingStation), "GetMixTimeForCurrentOperation")]
        [HarmonyPostfix]
        public static void Postfix(MixingStation __instance, ref int __result)
        {
            if (__instance.CurrentMixOperation == null || Core.SkillData == null || Core.SkillData.ChemistStationQuick == 0)
                return;

            if (blockMixingOnTimePassSecondExecution)
            {
                blockMixingOnTimePassSecondExecution = false;
                return;
            }

            __result = (__instance.MixTimePerItem * __instance.CurrentMixOperation.Quantity) / SkillModifiers.GetChemistStationSpeedMultiplier();
            //blockMixingOnTimePassSecondExecution = true;
        }
    }


    [HarmonyPatch]
    public static class CraftingStationOutputPatches
    {
        /// <summary>
        /// INCREASE CAULDRON OUTPUT
        /// </summary>

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
    }

    [HarmonyPatch(typeof(DryingRack), "InitializeGridItem")]
    public static class DryingRack_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(DryingRack __instance)
        {
            MelonLogger.Msg($"[DryingRack] Updating rack capacity.");
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
    public static class Pot_OnPlantFullyHarvested_Patch
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
            catch (System.Exception ex)
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

    /// <summary>
    /// CHANGE GROW SPEED
    /// </summary>

    [HarmonyPatch(typeof(Plant), "MinPass")]
    public static class Plant_MinPass_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(Plant __instance, int mins)
        {
            MelonLogger.Msg($"Plant_MinPass_Patch enter: growth progress {__instance.NormalizedGrowthProgress}");

            if (__instance.NormalizedGrowthProgress >= 1f || NetworkSingleton<TimeManager>.Instance.IsEndOfDay)
                return true; 

            float num = 1f / ((float)__instance.GrowthTime * 60f) * (float)mins;
            num *= __instance.Pot.GetTemperatureGrowthMultiplier();
            num *= __instance.Pot.GetAverageLightExposure(out var growSpeedMultiplier);
            num *= __instance.Pot.GrowSpeedMultiplier;
            num *= growSpeedMultiplier;
            num *= SkillModifiers.GetGrowthSpeedMultiplier();

            if (GameManager.IS_TUTORIAL)
                num *= 0.3f;

            if (__instance.Pot.NormalizedMoistureAmount <= 0f)
                num *= 0f;

            //MelonLogger.Msg($" Before Growth Plant  {__instance.NormalizedGrowthProgress}");
            //MelonLogger.Msg($" Add Growth Plant  {__instance.NormalizedGrowthProgress}");
            __instance.SetNormalizedGrowthProgress(__instance.NormalizedGrowthProgress + num);
            //MelonLogger.Msg($" After Growth Plant {__instance.NormalizedGrowthProgress}");

            MelonLogger.Msg($"Plant_MinPass_Patch growth progress boosted {__instance.NormalizedGrowthProgress + num}");
            return false;
        }
    }

    [HarmonyPatch(typeof(ShroomColony), "ChangeGrowthPercentage")]
    public static class MushroomGrowthSpeedPatch
    {
        [HarmonyPrefix]
        public static void Prefix(ShroomColony __instance, ref float change)
        {
            if (Core.SkillData.GrowthSpeed == 0 && Core.SkillData.GrowthSpeed2 == 0)
                return;
            
            if (change > 0f)
            {
                MelonLogger.Msg($" Growth Shroom {__instance.GrowthProgress}");
                MelonLogger.Msg($" Before Shroom change {change}");
                change *= SkillModifiers.GetGrowthSpeedMultiplier();
                MelonLogger.Msg($" After Shroom change {change}");
            }
        }
    }

    /// <summary>
    /// CHANGE QUALITY SYSTEM BY POT TYPE -- BETTER POT = BETTER QUALITY
    /// </summary>

    [HarmonyPatch(typeof(ShroomColony), "GetHarvestedShroom")]
    public static class MushroomQualityPatch
    {
        private static readonly HashSet<int> processedIds = new HashSet<int>();

        [HarmonyPostfix]
        public static void Postfix(ShroomColony __instance, ref ShroomInstance __result)
        {
            MelonLogger.Msg($"MushroomQualityPatch enter: MoreQuality {Core.SkillData.MoreQuality}");

            if (Core.SkillData.MoreQuality < 2 || __result == null) 
                return;

            int id = __instance.GetInstanceID();
            if (processedIds.Contains(id)) 
                return;

            MelonLogger.Msg($"MushroomQualityPatch doing something");
            float baseQuality = __instance.NormalizedQuality;
            //MelonLogger.Msg($"Base: {baseQuality}");

            __instance.ChangeQuality(SkillModifiers.QualityBonusShrooms);

            processedIds.Add(id);

            float boostedQuality = __instance.NormalizedQuality;
            //MelonLogger.Msg($"Boosted: {boostedQuality}");

            MelonCoroutines.Start(CleanUp(id));
        }

        private static System.Collections.IEnumerator CleanUp(int id)
        {
            yield return new WaitForSeconds(120f);
            processedIds.Remove(id);
        }
    }

    [HarmonyPatch(typeof(Plant), "Initialize")]
    public static class PlantQualityPatch
    {
        [HarmonyPostfix]
        public static void Postfix(Plant __instance)
        {
            if (__instance.Pot == null || Core.SkillData == null)
                return;

            string potName = __instance.Pot.Name.ToString();
            float baseQuality = 0.5f;

            if (potName.Equals("Grow Tent")) 
                baseQuality = 0.1f + SkillModifiers.GetGrowTentBonus();
            else if (potName.Equals("Plastic Pot")) 
                baseQuality = 0.36f;
            else if (potName.Equals("Moisture-Preserving Pot")) 
                baseQuality = 0.36f;
            else if (potName.Equals("Air Pot")) 
                baseQuality = 0.5f;
            else baseQuality = 0.1f; 

            float finalQuality = baseQuality + SkillModifiers.GetPlantBonus();

            if (Core.SkillData.AbsorbentSoil == 1)
            {
                MelonLogger.Msg("AbsobentSoil skill detected");
                var additives = __instance.Pot.AppliedAdditives;
                if (additives == null || additives.Count == 0)
                    MelonLogger.Msg("No initial additives found for instant growth");
                else
                {
                    float delta = 0f;
                    foreach (var additive in additives)
                    {
                        if (additive == null)
                            continue;

                        MelonLogger.Msg("Additive Name: " + additive.Name.ToString().ToLower());

                        /*switch (additive.Name.ToString().ToLower().Trim())
                        {
                            case "fertilizer":
                                delta = +0.3f;
                                break;

                            case "pgr":
                                delta = -0.3f;
                                break;

                            case "speedgrow":
                                delta = -0.3f;
                                break;
                        }*/


                        //finalQuality += delta;
                        //MelonLogger.Msg($"[SkillTree] Change Quality {finalQuality} | Additive: {additive.Name.ToString().ToLower().Trim()}");

                        if (additive.InstantGrowth > 0f && __instance.NormalizedGrowthProgress < 0.5f)
                        {
                            float before = __instance.NormalizedGrowthProgress;

                            __instance.SetNormalizedGrowthProgress(
                                before + additive.InstantGrowth
                            );

                            MelonLogger.Msg(
                                $"Instant growth applied: +{additive.InstantGrowth} (from {before} to {__instance.NormalizedGrowthProgress})"
                            );
                        }

                        if (finalQuality < 0.27f && finalQuality > 0.17f)
                            finalQuality = 0.27f;
                    }
                }
            }

            __instance.QualityLevel = finalQuality;

            /*  var traverse = Traverse.Create(__instance);
            traverse.Field("QualityLevel").SetValue(finalQuality);

            traverse.Field("<QualityLevel>k__BackingField").SetValue(finalQuality);

            traverse.Field("_qualityLevel").SetValue(finalQuality);*/

            MelonLogger.Msg($"[SkillTree] Plant Init: {potName} | Final: {finalQuality} | Skill: {SkillModifiers.QualityBonusPlants * Core.SkillData.MoreQuality} | Total: {__instance.QualityLevel}");
        }
    }

    /// <summary>
    /// ADD YIELD FROM PLANTS
    /// </summary>

    [HarmonyPatch(typeof(Plant), "GrowthDone")]
    public static class GrowthDone_SmartBasePatch
    {
        [HarmonyPrefix]
        public static void Prefix(Plant __instance)
        {
            MelonLogger.Msg($"[GrowthDone_SmartBasePatch]  - MoreYield {Core.SkillData.MoreYield}");

            if (!Il2CppFishNet.InstanceFinder.IsServer)
                return;

            if (Core.SkillData.MoreYield == 0)
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
    }



}
