using HarmonyLib;
using MelonLoader;
using Il2CppScheduleOne.NPCs.Behaviour;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SkillTree.SkillSpecial.SkillEmployee
{

    public static class BetterBotanist
    {
        public static bool Add = false;
    }

    [HarmonyPatch]
    public static class Patch_Botanist_Fix
    {
        // LOG: Building TargetMethods for IL2CPP Virtual Methods
        static IEnumerable<MethodBase> TargetMethods()
        {
            return new List<MethodBase>
        {
            AccessTools.Method(typeof(SowSeedInPotBehaviour), nameof(SowSeedInPotBehaviour.GetActionDuration)),
            AccessTools.Method(typeof(WaterPotBehaviour), nameof(WaterPotBehaviour.GetActionDuration)),
            AccessTools.Method(typeof(HarvestPotBehaviour), nameof(HarvestPotBehaviour.GetActionDuration)),
            AccessTools.Method(typeof(AddSoilToGrowContainerBehaviour), nameof(AddSoilToGrowContainerBehaviour.GetActionDuration)),
            AccessTools.Method(typeof(ApplyAdditiveToGrowContainerBehaviour), nameof(ApplyAdditiveToGrowContainerBehaviour.GetActionDuration)),
            AccessTools.Method(typeof(HarvestMushroomBedBehaviour), nameof(HarvestMushroomBedBehaviour.GetActionDuration)),
            AccessTools.Method(typeof(ApplySpawnToMushroomBedBehaviour), nameof(ApplySpawnToMushroomBedBehaviour.GetActionDuration))
        };
        }

        [HarmonyPrefix]
        public static bool Prefix(ref float __result, Il2CppSystem.Object __instance)
        {
            if (!BetterBotanist.Add) return true;

            if (__instance.TryCast<SowSeedInPotBehaviour>() != null)
            {
                __result = 7.5f;
            }
            else if (__instance.TryCast<WaterPotBehaviour>() != null)
            {
                __result = 5f;
            }
            else if (__instance.TryCast<HarvestPotBehaviour>() != null)
            {
                __result = 7.5f;
            }
            else if (__instance.TryCast<HarvestMushroomBedBehaviour>() != null)
            {
                __result = 8f;
            }
            else if (__instance.TryCast<AddSoilToGrowContainerBehaviour>() != null)
            {
                __result = 5f;
            }
            else if (__instance.TryCast<ApplySpawnToMushroomBedBehaviour>() != null)
            {
                __result = 5f;
            }
            else
            {
                __result = 5f;
            }

            return false; 
        }
    }
}
