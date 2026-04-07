using HarmonyLib;
using Il2CppScheduleOne.UI;
using SkillTree.Core.Skills;
using SkillTree.Core.Utilities;

namespace SkillTree.Core.Patches.Hustler
{
    [HarmonyPatch]
    public class PawnShopPatches
    {
        [HarmonyPatch(typeof(PawnShopInterface), "GetItemValue")]
        [HarmonyPostfix]
        public static void Patch_PawnValue(ref float __result)
        {
            float original = __result;
            __result *= SkillModifiers.GetPawnPriceMultiplier();
            LogManager.LogMessage($"GetItemValue {original} -> {__result}", LogLevel.Debug);
        }
    }
}
