using HarmonyLib;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.GameTime;
using Il2CppScheduleOne.Levelling;
using Il2CppScheduleOne.Money;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.UI;
using MelonLoader;
using S1API.Lifecycle;
using Semver;
using SkillTree.Core;
using SkillTree.Core.App;
using SkillTree.Core.Patches.Compatibility;
using SkillTree.Core.Patches.Miscellaneous;
using SkillTree.Core.Patches.Special;
using SkillTree.Core.Patches.Stats;
using SkillTree.Core.Skills;
using SkillTree.Core.Utilities;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

[assembly: MelonInfo(typeof(Core), "SkillTree", "2.5.0", "CrazyReizor & VindicatedVendetta", null)]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace SkillTree.Core
{
    public class Core : MelonMod
    {
        public static bool IsS1APIPatchNeeded { get; private set; } = false;

        private int skillPointValid = 0;
        private int specialSkillPointValid = 0;

        private int lastProcessedTier = -1;
        private ERank lastProcessedRank = (ERank)(-1);

        private readonly float delayTime = 3f;
        private bool setupComplete = false;


        public override void OnInitializeMelon()
        {
            if (MelonBase.RegisteredMelons.Contains(FindMelon("Empire (Forked by Kaen01)", "Aracor")))
            {
                try
                {
                    var harmony = new HarmonyLib.Harmony("com.reizor.skilltree");
                    harmony.Patch(
                        Type.GetType("Empire.EmpireSetup.GeneralSetup,Empire-S1API").GetMethod("ResetPlayerStats"),
                        prefix: new HarmonyMethod(typeof(SkillTree_EmpirePatches), nameof(SkillTree_EmpirePatches.Patch_ResetPlayerStats))
                        );
                    LoggerInstance.Msg("Empire 2.0 found, Empire.EmpireSetup.GeneralSetup.ResetPlayerStats() patched");
                }
                catch (Exception e)
                {
                    LoggerInstance.Msg($"Empire 2.0 patch failed {e}");
                }
            }

            var s1api = RegisteredMelons.FirstOrDefault(m => m.MelonAssembly.Assembly.GetName().Name.Equals("S1API"));
            var s1apiVersion = s1api.Info.SemanticVersion;
            if (s1apiVersion < new SemVersion(2, 9, 9))
            {
                IsS1APIPatchNeeded = true;
                MelonLogger.Warning($"S1API version {s1apiVersion} older than 2.9.9, applying compatibility patches.");
            }

            ConfigManager.Initialize();
            IconManager.ExtractIcons();
            SkillTreeData.AddChildren(SkillTreeData.StatsTree);
            SkillTreeData.AddChildren(SkillTreeData.OperationsTree);
            SkillTreeData.AddChildren(SkillTreeData.SocialTree);
            SkillTreeData.AddChildren(SkillTreeData.SpecialTree);

            LoggerInstance.Msg("SkillTree Initialized.");
        }


        private IEnumerator DelayedSetup()
        {
            yield return new WaitForSeconds(delayTime);
            ItemUnlocker.UnlockSpecificItems();
            ValidateTotalSkillPoints();
            CalculateSkillPoints();
            SaveManager.SaveFile();
            SkillTreeData.ApplyAllSkills();
            setupComplete = true;
        }

        public override void OnUpdate()
        {
            if (TimeManager.Instance == null ||
                LevelManager.Instance == null ||
                PlayerMovement.Instance == null || 
                PlayerCamera.Instance == null ||
                PlayerInventory.Instance == null ||
                PlayerManager.Instance == null ||
                Player.Local == null)
                return;


            if (!setupComplete)
            {
                return;
            }

            if (lastProcessedTier != LevelManager.Instance.Tier)
                CalculateSkillPoints(true);

            SkillActive.ResetSkillsIfNewDay();

            if (Input.GetKeyDown(ConfigManager.MenuHotkey.GetValue()))
            {
                SkillTreeApp.OnOpenKeyPressed.Invoke();
            }

            if (Cursor.lockState != CursorLockMode.None)
            {
                if (Input.GetKeyDown(ConfigManager.ActiveSkillOne.GetValue()) && SkillTreeData.Special.CurrentLevel == 1)
                    SkillActive.GoodSamaritan();

                if (Input.GetKeyDown(ConfigManager.ActiveSkillTwo.GetValue()) && SkillTreeData.Heal.CurrentLevel == 1)
                    SkillActive.BloodRush();

                if (Input.GetKeyDown(ConfigManager.ActiveSkillThree.GetValue()) && SkillTreeData.GetCashDealer.CurrentLevel == 1)
                    SkillActive.SiphonFunds();
            }
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            base.OnSceneWasLoaded(buildIndex, sceneName);
            if (sceneName != "Main")
            {
                skillPointValid = 0;
                specialSkillPointValid = 0;

                lastProcessedTier = -1;
                lastProcessedRank = (ERank)(-1);

                setupComplete = false;

                AllowSleep.Reset();
                SkillActive.Reset();
                NPCPatches.Reset();
                SaveManager.LoadDefaultValues();
                GameLifecycle.OnSaveComplete -= SaveManager.SaveFile;
            }

            if (sceneName == "Main")
            {
                GameLifecycle.OnSaveComplete += SaveManager.SaveFile;
                if (ConfigManager.ResetSkills.GetValue())
                {
                    MelonLogger.Warning($"Reset skills option is enabled. This happens the first time a save loaded with version 2.1.0 and later or when manually enabled by the player. Resetting skills.");
                    ConfigManager.ResetSkills.SetValue(false);
                    SaveManager.DeleteFile();
                    SaveManager.LoadDefaultValues();
                }
                else
                {
                    SaveManager.LoadFile();
                }
            }
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            base.OnSceneWasInitialized(buildIndex, sceneName);

            if (sceneName == "Main")
            {
                MelonCoroutines.Start(DelayedSetup());
            }
        }

        public void CalculateSkillPoints(bool levelUp = false)
        {
            int currentRank = (int)LevelManager.Instance.Rank;
            int currentTier = LevelManager.Instance.Tier - 1;

            if (currentRank == 0 && currentTier == 0)
                return;

            if (levelUp && currentTier == lastProcessedTier - 1 && (int)LevelManager.Instance.Rank == (int)lastProcessedRank)
                return;
            else if (levelUp)
                MelonLogger.Msg("Level Up Detected! Skill points updated.");

            if (levelUp)
            {
                skillPointValid = 1;
                if (lastProcessedTier == 5)
                {
                    skillPointValid = 2;
                    specialSkillPointValid = 1;
                }

                Singleton<NotificationsManager>.Instance.SendNotification(
                                "Level Up",
                                $"<color=#16F01C>+ {skillPointValid} Skill Points</color>", NetworkSingleton<MoneyManager>.Instance.LaunderingNotificationIcon);

                if (specialSkillPointValid > 0)
                    Singleton<NotificationsManager>.Instance.SendNotification(
                                    "Special Up",
                                    $"<color=#16F01C>+ {specialSkillPointValid} Special Points</color>", NetworkSingleton<MoneyManager>.Instance.LaunderingNotificationIcon);
            }

            lastProcessedTier = LevelManager.Instance.Tier;
            lastProcessedRank = LevelManager.Instance.Rank;

            int totalSkillPoint = SkillPoints.StatsPoints + SkillPoints.OperationsPoints + SkillPoints.SocialPoints + SkillPoints.UsedSkillPoints;

            if (skillPointValid > 0)
            {
                int statsGained = 0;
                int opsGained = 0;
                int socialGained = 0;
                int specialGained = 0;

                for (int i = 0; i < skillPointValid; i++)
                {
                    int mod = (totalSkillPoint + i) % 3;
                    switch (mod)
                    {
                        case 0:
                            statsGained++;
                            break;
                        case 1:
                            opsGained++;
                            break;
                        case 2:
                            socialGained++;
                            break;
                    }
                }

                for (int i = 0; i < specialSkillPointValid; i++)
                    specialGained++;

                if (specialSkillPointValid > 0)
                    specialSkillPointValid = 0;

                SkillPoints.AddSkillPoints(statsGained, opsGained, socialGained, specialGained);

                MelonLogger.Msg($"[SkillTree] Processed: Rank {LevelManager.Instance.Rank} Tier {LevelManager.Instance.Tier}. Gains: Stats+{statsGained} Operations+{opsGained} Social+{socialGained} Special+{specialGained}");
            }
        }

        public static (int, int, int, int) GetExpectedPointTotals()
        {
            int currentRank = (int)LevelManager.Instance.Rank;
            int currentTier = LevelManager.Instance.Tier - 1;

            int maxPointsPossible = currentRank * 7 + currentTier;
            int nonSpecialPoints = maxPointsPossible - currentRank;

            int stats = 0; 
            int operations = 0;
            int social = 0;
            int special = currentRank;

            for (int i = 0; i < nonSpecialPoints; i++)
            {
                int mod = (maxPointsPossible + i) % 3;
                switch (mod)
                {
                    case 0:
                        stats++;
                        break;
                    case 1:
                        operations++;
                        break;
                    case 2:
                        social++;
                        break;
                }
            }
            return (stats, operations, social, special);
        }

        private static void ValidateTotalSkillPoints()
        {
            var (stats, operations, social, special) = GetExpectedPointTotals();
            var (statsSpent, operationsSpent, socialSpent, specialSpent) = SkillTreeData.GetAllPointsSpent();

            int expectedTotal = stats + operations + social + special;
            int pointsSpent = statsSpent + operationsSpent + socialSpent + specialSpent;
            int pointsRemaining = SkillPoints.StatsPoints + SkillPoints.OperationsPoints + SkillPoints.SocialPoints + SkillPoints.SpecialPoints;

            MelonLogger.Msg($"Expected: Stats {stats} | Operations {operations} | Social {social} | Special {special} | Total {expectedTotal}");
            MelonLogger.Msg($"Spent:    Stats {statsSpent} | Operations {operationsSpent} | Social {socialSpent} | Special {specialSpent} | Total {pointsSpent}");
            MelonLogger.Msg($"Left:     Stats {SkillPoints.StatsPoints} | Operations {SkillPoints.OperationsPoints} | Social {SkillPoints.SocialPoints} | Special {SkillPoints.SpecialPoints} | Total {pointsRemaining}");

            if (expectedTotal < pointsSpent + pointsRemaining)
            {
                MelonLogger.Warning($"Current character is below the expected level for this save file or save file is corrupt, resetting save data. Expected Total: {expectedTotal} | Actual Total: {pointsSpent + pointsRemaining}.");
                SaveManager.LoadDefaultValues();
                return;
            }

            int missingStats = stats - (statsSpent + SkillPoints.StatsPoints);
            int missingOperations = operations - (operationsSpent + SkillPoints.OperationsPoints);
            int missingSocial = social - (socialSpent + SkillPoints.SocialPoints);
            int missingSpecial = special - (specialSpent + SkillPoints.SpecialPoints);

            if (missingStats != 0)
            {
                MelonLogger.Warning($"Refunding {missingStats} missing Stats points");
            }
            if (missingOperations != 0)
            {
                MelonLogger.Warning($"Refunding {missingOperations} missing Operations points");
            }
            if (missingSocial != 0)
            {
                MelonLogger.Warning($"Refunding {missingSocial} missing Social points");
            }
            if (missingSpecial != 0)
            {
                MelonLogger.Warning($"Refunding {missingSpecial} missing Special points");
            }
            SkillPoints.AddSkillPoints(missingStats, missingOperations, missingSocial, missingSpecial);
        }
    }
}