using HarmonyLib;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Trash;
using Il2CppSystem;
using SkillTree.Core.Skills;
using System.Collections.Generic;
using UnityEngine;

namespace SkillTree.Core.Patches.Operations
{
    [HarmonyPatch]
    public class TrashPatches
    {
        public static readonly HashSet<Guid> ProcessedTrash = [];

        [HarmonyPatch(typeof(TrashItem), "Start")]
        [HarmonyPostfix]
        public static void Patch_Start(TrashItem __instance)
        {
            if (SkillTreeData.SacarLaBasura.CurrentLevel == 0 || ProcessedTrash.Contains(__instance.GUID))
            {
                return;
            }

            //int original = __instance.SellValue;
            __instance.SellValue += SkillModifiers.GetTrashValueBonus();
            ProcessedTrash.Add(__instance.GUID);
            //MelonLogger.Msg($"Start Trash value {__instance.GUID} {original} -> {__instance.SellValue}");
        }

        public static void IncreaseTrashValue()
        {
            foreach (TrashItem item in NetworkSingleton<TrashManager>.Instance.trashItems)
            {
                if (ProcessedTrash.Contains(item.GUID))
                {
                    continue;
                }

                //int original = item.SellValue;
                item.SellValue += SkillModifiers.GetTrashValueBonus();
                ProcessedTrash.Add(item.GUID);
                //MelonLogger.Msg($"IncreaseTrashValue {item.GUID} {original} -> {item.SellValue}");
            }
        }

        [HarmonyPatch(typeof(TrashItem), "DestroyTrash")]
        [HarmonyPostfix]
        public static void Patch_DestroyTrash(TrashItem __instance)
        {
            //MelonLogger.Msg($"Destroy Trash. Removed {__instance.GUID} from cache");
            ProcessedTrash.Remove(__instance.GUID);
            //MelonLogger.Msg($"Processed IDs left {ProcessedTrash.Count}");
        }
    }
}
