using HarmonyLib;
using Il2CppScheduleOne.UI;
using MelonLoader;
using SkillTree.Core.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillTree.Core.Patches.Social
{
    [HarmonyPatch]
    public class PawnShopPatches
    {
        [HarmonyPatch(typeof(PawnShopInterface), "GetItemValue")]
        [HarmonyPostfix]
        public static void Patch_PawnValue(ref float __result)
        {
            //float original = __result;
            __result *= SkillModifiers.GetPawnPriceMultiplier();
            //MelonLogger.Msg($"GetItemValue {original} -> {__result}");
        }
    }
}
