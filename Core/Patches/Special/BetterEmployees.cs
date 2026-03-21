using HarmonyLib;
using Il2CppFishNet;
using Il2CppScheduleOne.Employees;
using Il2CppSystem;
using MelonLoader;
using SkillTree.Core.Skills;
using System.Collections.Generic;

namespace SkillTree.Core.Patches.Special
{
    [HarmonyPatch]
    public class BetterEmployees
    {
        private static readonly HashSet<Guid> processedEmployees = [];
        private static readonly HashSet<Guid> processedBotanists = [];
        private static readonly HashSet<Guid> processedChemists = [];

        [HarmonyPatch(typeof(Employee), "CanWork")]
        [HarmonyPostfix]
        public static void Postfix(Employee __instance, ref bool __result)
        {
            if (__instance == null || SkillTreeData.Employees24h.CurrentLevel == 0) return;

            foreach (Employee.NoWorkReason reason in __instance.WorkIssues)
            {
                if (reason.Reason.Equals("Sorry boss, my shift ends at 4AM."))
                {
                    __instance.WorkIssues.Remove(reason);
                }
            }
            __result = __instance.GetHome() != null && __instance.PaidForToday;
        }

        [HarmonyPatch(typeof(Employee), "UpdateBehaviour")]
        [HarmonyPostfix]
        public static void Patch_Employee_UpdateBehaviour(Employee __instance)
        {
            if (__instance == null || __instance.Fired || SkillTreeData.EmployeeMovespeed.CurrentLevel == 0)
                return;

            if (InstanceFinder.IsServer && (__instance.Behaviour.activeBehaviour == null || __instance.Behaviour.activeBehaviour == __instance.WaitOutside))
            {
                if (__instance.GetHome() != null && !__instance.PaidForToday && __instance.IsPayAvailable())
                {
                    __instance.SetWaitOutside(false);
                    __instance.RemoveDailyWage();
                    __instance.SetIsPaid();

                    foreach (Employee.NoWorkReason reason in __instance.WorkIssues)
                    {
                        if (reason.Reason.Equals("Sorry boss, my shift ends at 4AM."))
                        {
                            __instance.WorkIssues.Remove(reason);
                        }
                        if (reason.Reason.Equals("I haven't been paid yet"))
                        {
                            __instance.WorkIssues.Remove(reason);
                        }
                    }
                }
            }

            __instance.Movement.MovementSpeedScale = SkillModifiers.GetEmployeeMoveSpeedScale();
            if (!processedEmployees.Contains(__instance.GUID))
            {
                MelonLogger.MsgPastel($"{__instance.EmployeeType} {__instance.fullName}'s movespeed scale set to {__instance.Movement.MovementSpeedScale}");
                processedEmployees.Add(__instance.GUID);
            }
        }

        [HarmonyPatch(typeof(Botanist), "UpdateBehaviour")]
        [HarmonyPostfix]
        public static void Postfix(Botanist __instance)
        {
            if (__instance == null || SkillTreeData.EmployeeMaxStation.CurrentLevel == 0)
                return;

            (int, int) stations = SkillModifiers.GetBotanistStationBonus();
            __instance.configuration.Assigns.MaxItems = stations.Item1;

            if (!processedBotanists.Contains(__instance.GUID))
            {
                MelonLogger.Msg($"[EmployeeMaxStation] Botanist {__instance.fullName}'s max assigns increased from {stations.Item2} to {stations.Item1}");
                processedBotanists.Add(__instance.GUID);
            }
        }

        [HarmonyPatch(typeof(Chemist), "UpdateBehaviour")]
        [HarmonyPostfix]
        public static void Postfix(Chemist __instance)
        {
            if (__instance == null || SkillTreeData.EmployeeMaxStation.CurrentLevel == 0)
                return;

            (int, int) stations = SkillModifiers.GetChemistStationBonus();
            __instance.configuration.Stations.MaxItems = stations.Item1;

            if (!processedChemists.Contains(__instance.GUID))
            {
                MelonLogger.Msg($"[EmployeeMaxStation] Chemist {__instance.fullName}'s max stations increased from {stations.Item2} to {stations.Item1}");
                processedChemists.Add(__instance.GUID);
            }
        }

        [HarmonyPatch(typeof(Employee), "OnDestroy")]
        [HarmonyPrefix]
        public static void Patch_Employee_OnDestroy(Employee __instance)
        {
            if (processedEmployees.Contains(__instance.GUID) || 
                processedBotanists.Contains(__instance.GUID) ||
                processedChemists.Contains(__instance.GUID))
            {
                processedEmployees.Remove(__instance.GUID);
                processedBotanists.Remove(__instance.GUID);
                processedChemists.Remove(__instance.GUID);
                MelonLogger.MsgPastel($"{__instance.EmployeeType} {__instance.fullName} removed from cache");
            }
        }
    }
}
