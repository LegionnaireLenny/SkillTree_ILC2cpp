using HarmonyLib;
using Il2CppScheduleOne.NPCs.Behaviour;
using SkillTree.Core.Serialization;
using SkillTree.Core.Skills;
using System.Collections.Generic;
using System.Reflection;

namespace SkillTree.Core.Patches.Special
{
    [HarmonyPatch]
    public class BetterBotanists
    {
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
            if (__instance == null || SkillTreeData.BetterBotanists.CurrentLevel == 0) return true;

            if (__instance.TryCast<AddSoilToGrowContainerBehaviour>() != null)
            {
                __result = 10f * SkillModifiers.GetBotanistActionSpeedMultiplier();
            }
            else if (__instance.TryCast<ApplyAdditiveToGrowContainerBehaviour>() != null)
            {
                __result = 10f * SkillModifiers.GetBotanistActionSpeedMultiplier();
            }
            else if (__instance.TryCast<ApplySpawnToMushroomBedBehaviour>() != null)
            {
                __result = 15f * SkillModifiers.GetBotanistActionSpeedMultiplier();
            }
            else if (__instance.TryCast<HarvestPotBehaviour>() != null)
            {
                __result = (__instance as HarvestPotBehaviour).GetQuantityToHarvest() * SkillModifiers.GetBotanistActionSpeedMultiplier();
            }
            else if (__instance.TryCast<HarvestMushroomBedBehaviour>() != null)
            {
                __result = (__instance as HarvestMushroomBedBehaviour).GetQuantityToHarvest() * SkillModifiers.GetBotanistActionSpeedMultiplier();
            }
            else if (__instance.TryCast<SowSeedInPotBehaviour>() != null)
            {
                __result = 15f * SkillModifiers.GetBotanistActionSpeedMultiplier();
            }
            else if (__instance.TryCast<WaterPotBehaviour>() != null)
            {
                __result = 10f * SkillModifiers.GetBotanistActionSpeedMultiplier();
            }
            else
            {
                __result = 15f * SkillModifiers.GetBotanistActionSpeedMultiplier();
            }

            return false;
        }
    }
}
