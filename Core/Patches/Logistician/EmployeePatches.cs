using HarmonyLib;
using Il2CppFishNet;
using Il2CppScheduleOne.Employees;
using Il2CppSystem;
using SkillTree.Core.Serialization;
using SkillTree.Core.Skills;
using SkillTree.Core.Utilities;
using System.Collections.Generic;

namespace SkillTree.Core.Patches.Logistician
{
    [HarmonyPatch]
    public class EmployeePatches
    {
        private static readonly HashSet<Guid> processedEmployees = [];
        private static readonly HashSet<Guid> processedBotanists = [];
        private static readonly HashSet<Guid> processedChemists = [];

        [HarmonyPatch(typeof(Employee), "CanWork")]
        [HarmonyPostfix]
        public static void Postfix(Employee __instance, ref bool __result)
        {
            if (__instance == null || Core.ApplyeMployeePatch || SkillTreeData.NightShift.CurrentLevel == 0) 
                return;

            List<Employee.NoWorkReason> bogusReasons = [];
            foreach (var reason in __instance.WorkIssues)
            {
                if (reason.Reason.Equals("Sorry boss, my shift ends at 4AM."))
                {
                    bogusReasons.Add(reason);
                }
            }

            foreach (var reason in bogusReasons)
            {
                __instance.WorkIssues.Remove(reason);
            }
            __result = __instance.GetHome() != null && __instance.PaidForToday;
        }

        [HarmonyPatch(typeof(Employee), "UpdateBehaviour")]
        [HarmonyPostfix]
        public static void Patch_Employee_UpdateBehaviour(Employee __instance)
        {
            if (__instance == null || __instance.Fired)
                return;

            if (!Core.ApplyeMployeePatch &&
                SkillTreeData.NightShift.CurrentLevel > 0 &&
                InstanceFinder.IsServer &&
                (__instance.Behaviour.activeBehaviour == null || __instance.Behaviour.activeBehaviour == __instance.WaitOutside) &&
                __instance.GetHome() != null &&
                !__instance.PaidForToday &&
                __instance.IsPayAvailable())
            {
                __instance.SetWaitOutside(false);
                __instance.RemoveDailyWage();
                __instance.SetIsPaid();

                List<Employee.NoWorkReason> bogusReasons = [];
                foreach (var reason in __instance.WorkIssues)
                {
                    if (reason.Reason.Equals("Sorry boss, my shift ends at 4AM."))
                    {
                        bogusReasons.Add(reason);
                    }
                    if (reason.Reason.Equals("I haven't been paid yet"))
                    {
                        bogusReasons.Add(reason);
                    }
                }

                foreach (var reason in bogusReasons)
                {
                    __instance.WorkIssues.Remove(reason);
                }
            }

            if (SkillTreeData.EmployeeMovespeed.CurrentLevel > 0)
            {
                __instance.Movement.MovementSpeedScale = SkillModifiers.GetEmployeeMoveSpeedScale();
                if (!processedEmployees.Contains(__instance.GUID))
                {
                    LogManager.LogMessage($"{__instance.EmployeeType} {__instance.fullName}'s movespeed scale set to {__instance.Movement.MovementSpeedScale}", LogLevel.Debug);
                    processedEmployees.Add(__instance.GUID);
                }
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
                LogManager.LogMessage($"[EmployeeMaxStation] Botanist {__instance.fullName}'s max assigns increased from {stations.Item2} to {stations.Item1}", LogLevel.Debug);
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
                LogManager.LogMessage($"[EmployeeMaxStation] Chemist {__instance.fullName}'s max stations increased from {stations.Item2} to {stations.Item1}", LogLevel.Debug);
                processedChemists.Add(__instance.GUID);
            }
        }

        [HarmonyPatch(typeof(Employee), "OnDestroy")]
        [HarmonyPrefix]
        public static void Patch_Employee_OnDestroy(Employee __instance)
        {
            if (__instance?.GUID == null) return;
            try
            {
                if (processedEmployees.Contains(__instance.GUID) ||
                    processedBotanists.Contains(__instance.GUID) ||
                    processedChemists.Contains(__instance.GUID))
                {
                    processedEmployees.Remove(__instance.GUID);
                    processedBotanists.Remove(__instance.GUID);
                    processedChemists.Remove(__instance.GUID);
                    LogManager.LogMessage($"{__instance.EmployeeType} {__instance.fullName} removed from cache", LogLevel.Debug);
                }
            }
            catch (System.Exception) { }
        }
    }
}
