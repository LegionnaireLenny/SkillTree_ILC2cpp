using HarmonyLib;
using Il2CppScheduleOne.Trash;
using SkillTree.Core.Skills;
using UnityEngine;

namespace SkillTree.Core.Patches.Operations
{
    [HarmonyPatch]
    public class TrashPatches
    {
        [HarmonyPatch(typeof(TrashItem), "Start")]
        [HarmonyPostfix]
        public static void Patch_Start(TrashItem __instance) {

            __instance.SellValue = Mathf.RoundToInt(__instance.SellValue * SkillModifiers.GetTrashValueMultiplier());
        }
    }
}
