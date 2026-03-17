using HarmonyLib;
using Il2CppScheduleOne.GameTime;
using Il2CppScheduleOne.Levelling;
using Il2CppScheduleOne.PlayerScripts;
using MelonLoader;
using S1API.Lifecycle;
using SkillTree.Core;
using SkillTree.Core.Patches.Compatibility;
using SkillTree.Core.Patches.Miscellaneous;
using SkillTree.Core.Patches.Special;
using SkillTree.Core.Patches.Stats;
using SkillTree.Core.Skills;
using SkillTree.Core.Utilities;
using System;
using System.Collections;
using UnityEngine;

[assembly: MelonInfo(typeof(Core), "SkillTree", "2.5.2", "CrazyReizor & VindicatedVendetta", null)]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace SkillTree.Core
{
    public class Core : MelonMod
    {
        private readonly float delayTime = 3f;
        private bool setupComplete = false;
        public static Action OnOpenKeyPressed;

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

            if (Input.GetKeyDown(ConfigManager.MenuHotkey.GetValue()))
            {
                OnOpenKeyPressed.Invoke();
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
                setupComplete = false;

                AllowSleep.Reset();
                SkillActive.ResetSkillCooldowns();
                NPCPatches.Reset();
                SaveManager.LoadDefaultValues();
                GameLifecycle.OnSaveComplete -= SaveManager.SaveFile;
                S1API.Leveling.LevelManager.OnRankUp -= SkillPoints.ProcessLevelUp;
                S1API.GameTime.TimeManager.OnDayPass -= SkillActive.ResetSkillCooldowns;
                OnOpenKeyPressed = null;
            }

            if (sceneName == "Main")
            {
                GameLifecycle.OnSaveComplete += SaveManager.SaveFile;
                S1API.Leveling.LevelManager.OnRankUp += SkillPoints.ProcessLevelUp;
                S1API.GameTime.TimeManager.OnDayPass += SkillActive.ResetSkillCooldowns;
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
                int mod = i % 3;
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
            var (statsExpected, operationsExpected, socialExpected, specialExpected) = GetExpectedPointTotals();
            var (statsSpent, operationsSpent, socialSpent, specialSpent) = SkillTreeData.GetCategoryPointsSpent();

            int expectedTotal = statsExpected + operationsExpected + socialExpected + specialExpected;
            int pointsSpent = statsSpent + operationsSpent + socialSpent + specialSpent;
            int pointsRemaining = SkillPoints.StatsPoints + SkillPoints.OperationsPoints + SkillPoints.SocialPoints + SkillPoints.SpecialPoints;

            MelonLogger.Msg($"Expected: Stats {statsExpected} | Operations {operationsExpected} | Social {socialExpected} | Special {specialExpected} | Total {expectedTotal}");
            MelonLogger.Msg($"Spent:    Stats {statsSpent} | Operations {operationsSpent} | Social {socialSpent} | Special {specialSpent} | Total {pointsSpent}");
            MelonLogger.Msg($"Left:     Stats {SkillPoints.StatsPoints} | Operations {SkillPoints.OperationsPoints} | Social {SkillPoints.SocialPoints} | Special {SkillPoints.SpecialPoints} | Total {pointsRemaining}");

            if (expectedTotal < pointsSpent + pointsRemaining)
            {
                MelonLogger.Warning($"Current character is below the expected level for this save file or save file is corrupt, resetting save data. Expected Total: {expectedTotal} | Actual Total: {pointsSpent + pointsRemaining}.");
                SaveManager.LoadDefaultValues();
                return;
            }

            int missingStats = statsExpected - (statsSpent + SkillPoints.StatsPoints);
            int missingOperations = operationsExpected - (operationsSpent + SkillPoints.OperationsPoints);
            int missingSocial = socialExpected - (socialSpent + SkillPoints.SocialPoints);
            int missingSpecial = specialExpected - (specialSpent + SkillPoints.SpecialPoints);

            if (missingStats != 0)
            {
                MelonLogger.Warning($"Adjusting Stats points by {missingStats}");
            }
            if (missingOperations != 0)
            {
                MelonLogger.Warning($"Adjusting Operations points by {missingOperations}");
            }
            if (missingSocial != 0)
            {
                MelonLogger.Warning($"Adjusting Social points by {missingSocial}");
            }
            if (missingSpecial != 0)
            {
                MelonLogger.Warning($"Adjusting Special points by {missingSpecial}");
            }
            SkillPoints.AddSkillPoints(missingStats, missingOperations, missingSocial, missingSpecial);
        }
    }
}