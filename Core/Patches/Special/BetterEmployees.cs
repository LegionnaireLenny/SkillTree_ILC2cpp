using HarmonyLib;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Employees;
using Il2CppScheduleOne.GameTime;
using Il2CppScheduleOne.NPCs.Behaviour;
using MelonLoader;

namespace SkillTree.Core.Patches.Special
{
    [HarmonyPatch]
    public static class BetterEmployees
    {
        [HarmonyPatch(typeof(Employee), "CanWork")]
        [HarmonyPostfix]
        public static void Postfix(Employee __instance, ref bool __result)
        {
            if (__instance == null || Core.SkillData == null || Core.SkillData.Employees24h == 0)
                return;


            Employee.NoWorkReason bogusReason = null;
            foreach (Employee.NoWorkReason reason in __instance.WorkIssues)
            {
                if (reason.Reason.Equals("Sorry boss, my shift ends at 4AM."))
                {
                    bogusReason = reason;
                }
            }

            if (bogusReason != null)
            {
                __instance.WorkIssues.Remove(bogusReason);
            }

            __result = __instance.GetHome() != null &&
                       __instance.PaidForToday &&
                       (!NetworkSingleton<TimeManager>.Instance.IsEndOfDay || Core.SkillData.Employees24h == 1);
        }

        private static readonly HashSet<Il2CppSystem.Guid> processedEmployees = [];

        [HarmonyPatch(typeof(Employee), "UpdateBehaviour")]
        [HarmonyPostfix]
        public static void Postfix(Employee __instance)
        {
            if (__instance == null || Core.SkillData == null || Core.SkillData.EmployeeMovespeed == 0)
                return;

            __instance.Movement.MovementSpeedScale = SkillModifiers.EmployeeMoveSpeedBonus;
            if (!processedEmployees.Contains(__instance.GUID))
            {
                MelonLogger.Msg($"{__instance.EmployeeType} {__instance.fullName}'s movespeed scale set to {__instance.Movement.MovementSpeedScale}");
                processedEmployees.Add(__instance.GUID);
            }
        }

        private static readonly HashSet<Il2CppSystem.Guid> processedBotanists = [];

        [HarmonyPatch(typeof(Botanist), "UpdateBehaviour")]
        [HarmonyPostfix]
        public static void Postfix(Botanist __instance)
        {
            if (__instance == null || Core.SkillData == null || Core.SkillData.EmployeeMaxStation == 0)
                return;

            (int, int) stations = SkillModifiers.GetBotanistStationBonus();
            __instance.configuration.Assigns.MaxItems = stations.Item1;

            if (!processedBotanists.Contains(__instance.GUID))
            {
                MelonLogger.Msg($"Botanist {__instance.fullName}'s max assigns increased from {stations.Item2} to {stations.Item1}");
                processedBotanists.Add(__instance.GUID);
            }
        }

        private static readonly HashSet<Il2CppSystem.Guid> processedChemists = [];

        [HarmonyPatch(typeof(Chemist), "UpdateBehaviour")]
        [HarmonyPostfix]
        public static void Postfix(Chemist __instance)
        {
            if (__instance == null || Core.SkillData == null || Core.SkillData.EmployeeMaxStation == 0)
                return;

            (int, int) stations = SkillModifiers.GetChemistStationBonus();
            __instance.configuration.Stations.MaxItems = stations.Item1;

            if (!processedChemists.Contains(__instance.GUID))
            {
                MelonLogger.Msg($"Chemist {__instance.fullName}'s max stations increased from {stations.Item2} to {stations.Item1}");
                processedChemists.Add(__instance.GUID);
            }
        }

        [HarmonyPatch(typeof(AddSoilToGrowContainerBehaviour), "GetActionDuration")]
        [HarmonyPostfix]
        public static void AddSoilToGrowContainerBehaviour(ref float __result)
        {
            __result *= SkillModifiers.GetBotanistActionSpeedBonus();
        }

        [HarmonyPatch(typeof(ApplyAdditiveToGrowContainerBehaviour), "GetActionDuration")]
        [HarmonyPostfix]
        public static void ApplyAdditiveToGrowContainerBehaviour(ref float __result)
        {
            __result *= SkillModifiers.GetBotanistActionSpeedBonus();
        }

        [HarmonyPatch(typeof(ApplySpawnToMushroomBedBehaviour), "GetActionDuration")]
        [HarmonyPostfix]
        public static void ApplySpawnToMushroomBedBehaviour(ref float __result)
        {
            __result *= SkillModifiers.GetBotanistActionSpeedBonus();
        }

        [HarmonyPatch(typeof(HarvestMushroomBedBehaviour), "GetActionDuration")]
        [HarmonyPostfix]
        public static void HarvestMushroomBedBehaviour(ref float __result)
        {
            __result *= SkillModifiers.GetBotanistActionSpeedBonus();
        }

        [HarmonyPatch(typeof(HarvestPotBehaviour), "GetActionDuration")]
        [HarmonyPostfix]
        public static void HarvestPotBehaviour(ref float __result)
        {
            __result *= SkillModifiers.GetBotanistActionSpeedBonus();
        }

        [HarmonyPatch(typeof(SowSeedInPotBehaviour), "GetActionDuration")]
        [HarmonyPostfix]
        public static void SowSeedInPotBehaviour(ref float __result)
        {
            __result *= SkillModifiers.GetBotanistActionSpeedBonus();
        }

        [HarmonyPatch(typeof(WaterPotBehaviour), "GetActionDuration")]
        [HarmonyPostfix]
        public static void WaterPotBehaviour(ref float __result)
        {
            __result *= SkillModifiers.GetBotanistActionSpeedBonus();
        }
    }
}
