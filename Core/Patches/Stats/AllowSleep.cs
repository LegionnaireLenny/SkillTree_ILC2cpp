using HarmonyLib;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.GameTime;
using Il2CppScheduleOne.Growing;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.Tools;
using MelonLoader;

namespace SkillTree.Core.Patches.Stats
{
    public static class AllowSleep
    {
        private static int lastDayUsed = -1;

        public static bool CanUseBedSkill()
        {
            int currentDay = (int)NetworkSingleton<TimeManager>.Instance.CurrentDay;
            return currentDay != lastDayUsed;
        }

        public static string GetTimeRemaining(float currentTime)
        {
            int next = GetNextSchedule();
            if (next == 0) next = 2400;

            int currentTotalMin = (int)currentTime / 100 * 60 + (int)currentTime % 100;
            int nextTotalMin = next / 100 * 60 + next % 100;

            int diff = nextTotalMin - currentTotalMin;
            int h = diff / 60;
            int m = diff % 60;

            return $"{h:00}h {m:00}m";
        }

        public static int GetNextSchedule()
        {
            float time = NetworkSingleton<TimeManager>.Instance.CurrentTime;

            if (time >= 700 && time < 1200) return 1200;
            if (time >= 1203 && time < 1800) return 1800;
            if (time >= 1803 && time < 2357) return 2357;

            return (int)time;
        }

        [HarmonyPatch(typeof(Bed), "CanSleep")]
        public static class Bed_AlwaysAllow
        {
            [HarmonyPrefix]
            public static bool Prefix(ref bool __result)
            {
                if (Core.SkillData == null || Core.SkillData.AllowSleepAthEne == 0)
                    return true;

                float currentTime = NetworkSingleton<TimeManager>.Instance.CurrentTime;
                if (currentTime > 700 && currentTime < 1800 && Core.SkillData.SkipSchedule == 0)
                    return true;

                __result = true;
                return false;
            }
        }

        [HarmonyPatch(typeof(Bed), "Hovered")]
        public static class Bed_Hovered_Patch
        {
            [HarmonyPrefix]
            public static bool Prefix(Bed __instance)
            {
                if (Core.SkillData == null || Core.SkillData.SkipSchedule == 0)
                    return true;

                if (Singleton<ManagementClipboard>.Instance.IsEquipped || __instance.AssignedEmployee != null)
                    return true;

                float currentTime = NetworkSingleton<TimeManager>.Instance.CurrentTime;
                if (currentTime >= 0 && currentTime < 700)
                    return true;

                if (!CanUseBedSkill() && currentTime <= 1800)
                {
                    __instance.intObj.SetMessage("You've already rested today! Use it only tomorrow.");
                }
                else if (CanUseBedSkill() && currentTime < 2357)
                {
                    string remaining = GetTimeRemaining(currentTime);
                    __instance.intObj.SetMessage($"Next Shift in: {remaining}");
                }
                else
                    return true;

                return false;
            }
        }

        [HarmonyPatch(typeof(Bed), "Interacted")]
        public static class Bed_Interacted_Patch
        {
            [HarmonyPrefix]
            public static bool Prefix()
            {
                if (Core.SkillData == null || Core.SkillData.SkipSchedule == 0)
                    return true;

                if (!CanUseBedSkill())
                {
                    MelonLogger.Msg("[BedSkill] You've already rested today! You can't use it until tomorrow.");
                    return true;
                }

                float currentTime = NetworkSingleton<TimeManager>.Instance.CurrentTime;

                if (currentTime >= 700)
                {
                    int nextTarget = GetNextSchedule();

                    int totalMinutesPassed = CalculateMinutesBetween(currentTime, nextTarget) / 3;

                    if (totalMinutesPassed > 0)
                    {
                        foreach (GrowContainer container in UnityEngine.Object.FindObjectsOfType<GrowContainer>())
                            AccessTools.Method(typeof(GrowContainer), "DrainMoisture")?.Invoke(container, new object[] { totalMinutesPassed * 3 });
                        foreach (Plant plant in UnityEngine.Object.FindObjectsOfType<Plant>())
                            plant.MinPass(totalMinutesPassed);
                    }

                    lastDayUsed = (int)NetworkSingleton<TimeManager>.Instance.CurrentDay;

                    NetworkSingleton<TimeManager>.Instance.SetTime(nextTarget);

                    MelonLogger.Msg($"[BedSkill] Interaction detected. Next schedule set for: {nextTarget}");
                    return false;
                }
                return true;
            }
        }

        private static int CalculateMinutesBetween(float start, float end)
        {
            if (end == 0) end = 2400;

            int startHours = (int)start / 100;
            int startMins = (int)start % 100;
            int endHours = (int)end / 100;
            int endMins = (int)end % 100;

            int startTotal = startHours * 60 + startMins;
            int endTotal = endHours * 60 + endMins;

            return endTotal - startTotal;
        }
    }
}