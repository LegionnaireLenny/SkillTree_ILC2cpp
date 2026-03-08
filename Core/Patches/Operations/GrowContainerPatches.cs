using HarmonyLib;
using Il2CppFishNet;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.GameTime;
using Il2CppScheduleOne.Growing;
using Il2CppScheduleOne.Levelling;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.Variables;
using MelonLoader;
using SkillTree.Core.Skills;
using static Il2CppScheduleOne.ObjectScripts.Pot;

namespace SkillTree.Core.Patches.Operations
{
    [HarmonyPatch]
    public static class GrowContainerPatches
    {
        [HarmonyPatch(typeof(GrowContainer), "OnMinPass")]
        [HarmonyPrefix]

        public static bool Patch_OnMinPass(GrowContainer __instance)
        {
            if (SkillTreeData.WetAssPlants.CurrentLevel == 0)
                return true;


            __instance.onMinPass?.Invoke();
            if (NetworkSingleton<TimeManager>.Instance.IsEndOfDay)
            {
                return false;
            }

            __instance.ChangeMoistureAmount(-((__instance._moistureDrainPerHour * SkillModifiers.GetMoistureDrainMultiplier()) / 60f) * 1f);

            return false;
        }

        [HarmonyPatch(typeof(GrowContainer), "OnTimeSkipped")]
        [HarmonyPrefix]
        public static bool Patch_OnTimeSkipped(GrowContainer __instance, int minsSkipped)
        {
            if (SkillTreeData.WetAssPlants.CurrentLevel == 0)
                return true;

            if (!InstanceFinder.IsServer)
            {
                return false;
            }
            __instance.onTimeSkip?.Invoke(minsSkipped);
            __instance.ChangeMoistureAmount(-((__instance._moistureDrainPerHour * SkillModifiers.GetMoistureDrainMultiplier()) / 60f) * (float)minsSkipped);
            return false;
        }

        [HarmonyPatch(typeof(Pot), "OnPlantFullyHarvested")]
        [HarmonyPrefix]
        public static bool Prefix(Pot __instance)
        {
            if (SkillTreeData.AbsorbentSoil.CurrentLevel == 0)
                return true;

            if (__instance.Plant == null)
                return false;

            if (InstanceFinder.IsServer)
            {
                float value = NetworkSingleton<VariableDatabase>.Instance.GetValue<float>("HarvestedPlantCount");
                NetworkSingleton<VariableDatabase>.Instance.SetVariableValue("HarvestedPlantCount", (value + 1f).ToString());
                NetworkSingleton<LevelManager>.Instance.AddXP(5);
            }

            __instance.Plant = null;
            __instance.SetRemainingSoilUses(__instance._remainingSoilUses - 1);
            __instance.SetSoilState(ESoilState.Flat);

            //MelonLogger.Msg($"[AbsorbentSoil] Soil and additives have {__instance._remainingSoilUses} remaining uses");
            if (__instance._remainingSoilUses <= 0)
            {
                __instance.ClearAdditives();
                __instance.ClearSoil();
            }
            return false;
        }
    }
}
