//using HarmonyLib;
//using Il2CppScheduleOne.Storage;
//using MelonLoader;
//using UnityEngine;

//namespace SkillTree.Core.Patches.Miscellaneous
//{
//    [HarmonyPatch]
//    public class StoragePatches
//    {
//        [HarmonyPatch(typeof(StorageVisualizationUtility), "GetVisualRepresentation")]
//        [HarmonyPrefix]
//        public static void Prefix(ref int TotalFootprintSize)
//        {
//            MelonLogger.Msg("GetVisualRepresentation");
//            TotalFootprintSize = Mathf.Clamp(TotalFootprintSize, 0, 20);
//        }

//        [HarmonyPatch(typeof(StorageVisualizer), "RefreshVisuals")]
//        [HarmonyPrefix]
//        public static void Prefix()
//        {
//            MelonLogger.Msg("RefreshVisuals");
//        }
//    }
//}
