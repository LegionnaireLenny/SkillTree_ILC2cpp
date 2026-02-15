using HarmonyLib;
using Il2CppScheduleOne.Growing;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.Product;
using MelonLoader;
using UnityEngine;

namespace SkillTree.Core.Patches.Operations
{
    [HarmonyPatch]
    public class MoreQuality
    {
        private static readonly HashSet<object> processedOperations = [];

        [HarmonyPatch(typeof(LabOven), "Shatter")]
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


        private static readonly HashSet<int> processedIds = new HashSet<int>();

        [HarmonyPatch(typeof(ShroomColony), "GetHarvestedShroom")]
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

        [HarmonyPatch(typeof(Plant), "Initialize")]
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
}
