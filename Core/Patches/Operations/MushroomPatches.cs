using HarmonyLib;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.GameTime;
using Il2CppScheduleOne.Growing;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.Product;
using MelonLoader;

namespace SkillTree.Core.Patches.Operations
{
    [HarmonyPatch]
    public class MushroomPatches
    {
        [HarmonyPatch(typeof(ShroomColony), "ChangeGrowthPercentage")]
        [HarmonyPrefix]
        public static void Prefix(ShroomColony __instance, ref float change)
        {
            if (Core.SkillData.GrowthSpeed == 0 && Core.SkillData.GrowthSpeed2 == 0)
                return;

            change *= SkillModifiers.GetGrowthSpeedMultiplier();
        }

        [HarmonyPatch(typeof(ShroomColony), "GetHarvestedShroom")]
        [HarmonyPostfix]
        public static void Postfix(ShroomColony __instance, ref ShroomInstance __result)
        {
            if (__result == null || Core.SkillData == null || Core.SkillData.MoreQuality < 2)
                return;

            EQuality original = __result.Quality;
            __instance.ChangeQuality(SkillModifiers.GetShroomQualityBonus());
            __result.SetQuality(ItemQuality.GetQuality(__instance.NormalizedQuality));
            MelonLogger.Msg($"Mushroom quality increased from {original} to {__result.Quality}");
        }
    }
}
