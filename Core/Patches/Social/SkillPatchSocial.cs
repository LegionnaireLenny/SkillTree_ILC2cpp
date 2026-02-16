using HarmonyLib;
using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.UI.Shop;
using MelonLoader;
using System.Reflection;
using UnityEngine;

namespace SkillTree.Core.Patches.Social
{

    /// <summary>
    /// UP CUSTOMER SAMPLE
    /// </summary>

    [HarmonyPatch(typeof(Customer), "GetSampleSuccess")]
    public class PatchSampleSuccessUI
    {
        private static int _depth = 0;
        [HarmonyPrefix]
        public static void Prefix()
        {
            _depth++;
        }

        [HarmonyPostfix]
        public static void Postfix(ref float __result, float __state)
        {
            if (_depth == 1)
            {
                if (Core.SkillData == null || Core.SkillData.Social == 0) 
                    return;

                float origin = __result;

                __result = Mathf.Clamp(__result + SkillModifiers.GetCustomerSampleBonus(), 0f, 1f);
                MelonLogger.Msg($"[Skill] Changed sample chance from {origin:P0} to {__result:P0}");
            }
            _depth--;
        }
    }
}
