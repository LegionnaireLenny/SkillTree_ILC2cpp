using HarmonyLib;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.GameTime;
using Il2CppScheduleOne.Growing;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.Tools;
using MelonLoader;
using SkillTree.Core.Serialization;
using static SkillTree.Core.Serialization.Cooldowns;

namespace SkillTree.Core.Patches.Stats
{
    public class AllowSleep
    {
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
                if (SkillTreeData.CircadianMastery.CurrentLevel == 0)
                    return true;

                float currentTime = NetworkSingleton<TimeManager>.Instance.CurrentTime;
                if (currentTime > 700 && currentTime < 1800)
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
                if (__instance == null || SkillTreeData.CircadianMastery.CurrentLevel == 0)
                    return true;

                if (Singleton<ManagementClipboard>.Instance.IsEquipped || __instance.AssignedEmployee != null)
                    return true;

                float currentTime = NetworkSingleton<TimeManager>.Instance.CurrentTime;
                if (currentTime >= 0 && currentTime < 700)
                    return true;

                if (CircadianMasteryUsed && currentTime <= 1800)
                {
                    __instance.intObj.SetMessage("Sleep. Schedule has already been skipped today.");
                }
                else if (!CircadianMasteryUsed && currentTime < 2357)
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
                if (SkillTreeData.CircadianMastery.CurrentLevel == 0)
                    return true;

                if (CircadianMasteryUsed)
                {
                    MelonLogger.Msg("[BedSkill] You've already rested today! You can't use it until tomorrow.");
                    return true;
                }

                float currentTime = NetworkSingleton<TimeManager>.Instance.CurrentTime;

                if (currentTime >= 700)
                {
                    int nextTarget = GetNextSchedule();
                    int totalMinutesPassed = CalculateMinutesBetween(currentTime, nextTarget);

                    if (totalMinutesPassed > 0)
                    {
                        foreach (GrowContainer container in UnityEngine.Object.FindObjectsOfType<GrowContainer>())
                        {
                            container.DrainMoisture(totalMinutesPassed);
                            container.TryCast<Pot>()?.OnTimeSkipped(totalMinutesPassed / 3);
                            container.TryCast<MushroomBed>()?.OnTimeSkipped(totalMinutesPassed / 3);
                        }
                    }

                    CircadianMasteryUsed = true;
                    NetworkSingleton<TimeManager>.Instance.SetTimeAndSync(nextTarget);
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