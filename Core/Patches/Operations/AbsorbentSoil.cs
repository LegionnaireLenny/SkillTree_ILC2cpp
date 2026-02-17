using HarmonyLib;
using Il2CppFishNet;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Levelling;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.Variables;
using static Il2CppScheduleOne.ObjectScripts.Pot;

namespace SkillTree.Core.Patches.Operations
{
    [HarmonyPatch]
    public static class AbsorbentSoil
    {
        [HarmonyPatch(typeof(Pot), "OnPlantFullyHarvested")]
        [HarmonyPrefix]
        public static bool Prefix(Pot __instance)
        {
            if (Core.SkillData == null || Core.SkillData.AbsorbentSoil == 0)
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
