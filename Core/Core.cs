using HarmonyLib;
using Il2CppScheduleOne;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.GameTime;
using Il2CppScheduleOne.Levelling;
using Il2CppScheduleOne.Money;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.UI;
using MelonLoader;
using SkillTree.Core;
using SkillTree.Core.FileManagement;
using SkillTree.Core.Patches.Compatibility;
using SkillTree.Core.Patches.Special;
using SkillTree.Core.Patches.Stats;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using static SkillTree.Core.Patches.Special.SkillActive;

[assembly: MelonInfo(typeof(Core), "SkillTree", "2.1.2", "CrazyReizor & VindicatedVendetta", null)]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace SkillTree.Core
{
    public class Core : MelonMod
    {
        private static readonly string version = "2.1.2";
        private static Core Instance;

        public static SkillTreeData SkillData;
        private SkillTreeUI skillTreeUI;
        private int skillPointValid = 0;
        private int specialSkillPointValid = 0;

        private int lastProcessedTier = -1;
        private ERank lastProcessedRank = (ERank)(-1);

        private float delayTime = 3f;
        private bool setupComplete = false;
        private bool treeUiChange = false;

        private static MelonPreferences_Category Keybinds { get; set; }
        public static MelonPreferences_Entry MenuHotkey { get; set; }
        public static MelonPreferences_Entry ActiveSkillOne { get; set; }
        public static MelonPreferences_Entry ActiveSkillTwo { get; set; }
        public static MelonPreferences_Entry ActiveSkillThree { get; set; }

        private static MelonPreferences_Category ModInfo { get; set; }
        public static MelonPreferences_Entry Version { get; set; }
        public static MelonPreferences_Entry ResetSkills { get; set; }

        public override void OnInitializeMelon()
        {
            Instance = this;
            Keybinds = MelonPreferences.CreateCategory("SkillTree_Keybinds", "Keybindings");
            Keybinds.SetFilePath($"UserData/SkillTree_Config.cfg", true, false);
            MenuHotkey = Keybinds.CreateEntry<KeyCode>($"SkillTree_01_Menu Hotkey", KeyCode.BackQuote, "Menu Hotkey", "Open the skill tree menu");
            ActiveSkillOne = Keybinds.CreateEntry<KeyCode>("SkillTree_02_Skill One", KeyCode.F1, "Skill: Streetsweeper", "Activate 'Streetsweeper' skill");
            ActiveSkillTwo = Keybinds.CreateEntry<KeyCode>("SkillTree_03_Skill Two", KeyCode.F2, "Skill: Fit as a Fiddle", "Activate 'Fit as a Fiddle' skill");
            ActiveSkillThree = Keybinds.CreateEntry<KeyCode>("SkillTree_04_Skill Three", KeyCode.F3, "Skill: Siphon Funds", "Activate 'Siphon Funds' skill");

            ModInfo = MelonPreferences.CreateCategory($"SkillTree_99_ModInfo", $"Mod Version: {version}");
            Version = ModInfo.CreateEntry<string>("SkillTree_01_Version", version, $"Skill Tree Version", "Do not modify. This is used to determine if the skill tree was changed in such a way that a reset is required");
            ResetSkills = ModInfo.CreateEntry<bool>("SkillTree_02_ResetSkills", true, "Reset skills on next game load", "Debug: Enable this option and reload your save to reset your skills");
            ModInfo.SetFilePath($"UserData/SkillTree_Config.cfg", true, false);

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

            LoggerInstance.Msg("SkillTree Initialized.");
        }

        public void Reset()
        {
            skillPointValid = 0;
            specialSkillPointValid = 0;

            lastProcessedTier = -1;
            lastProcessedRank = (ERank)(-1);

            setupComplete = false;
            treeUiChange = false;

            AllowSleep.Reset();
            SkillActive.Reset();
        }
        private IEnumerator DelayedSetup()
        {
            yield return new WaitForSeconds(delayTime);
            ItemUnlocker.UnlockSpecificItems();
            ValidateSave();
            AttPoints();
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
                AttPoints(true);

            ResetSkillsIfNewDay();
            ActiveSkills();

            if (Input.GetKeyDown((KeyCode)MenuHotkey.BoxedValue) && (skillTreeUI.Visible || Cursor.lockState != CursorLockMode.None))
            {
                skillTreeUI.Visible = !skillTreeUI.Visible;
                treeUiChange = true;
            }

            if (skillTreeUI.Visible)
                PlayerCamera.Instance.SetDoFActive(true, 0.06f);

            if (!skillTreeUI.Visible)
                PlayerCamera.Instance.SetDoFActive(false, 0f);

            if (skillTreeUI.Visible && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab) || Input.GetMouseButtonDown(1)))
            {
                skillTreeUI.Visible = !skillTreeUI.Visible;
                treeUiChange = true;
            }

            if (treeUiChange)
            {
                treeUiChange = false;
                Cursor.lockState = skillTreeUI.Visible ? CursorLockMode.None : CursorLockMode.Locked;
                Cursor.visible = skillTreeUI.Visible;
                GameInput.Instance.PlayerInput.enabled = !skillTreeUI.Visible;
                PlayerInventory.Instance.SetInventoryEnabled(!skillTreeUI.Visible);
            }
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            base.OnSceneWasLoaded(buildIndex, sceneName);
            if (sceneName != "Main")
            {
                Reset();
            }

            if (sceneName == "Main")
            {
                SkillData = SkillTreeSaveManager.LoadOrCreate();
                skillTreeUI = new SkillTreeUI(SkillData);
                SkillTree_Test.AddChildren(SkillTree_Test.StatsTree);
                SkillTree_Test.AddChildren(SkillTree_Test.OperationsTree);
                SkillTree_Test.AddChildren(SkillTree_Test.SocialTree);
                SkillTree_Test.AddChildren(SkillTree_Test.SpecialTree);
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

        public void ActiveSkills()
        {
            if (Cursor.lockState == CursorLockMode.None)
            {
                return;
            }

            if (Input.GetKeyDown((KeyCode)ActiveSkillOne.BoxedValue) && SkillData.Special == 1)
                ClearTrash();

            if (Input.GetKeyDown((KeyCode)ActiveSkillTwo.BoxedValue) && SkillData.Heal == 1)
                Heal();

            if (Input.GetKeyDown((KeyCode)ActiveSkillThree.BoxedValue) && SkillData.GetCashDealer == 1)
                GetCashDealer();
        }

        public void AttPoints(bool levelUp = false)
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

            //MelonLogger.Msg("skillPointValid " + skillPointValid);

            int totalSkillPoint = SkillData.StatsPoints + SkillData.OperationsPoints + SkillData.SocialPoints + SkillData.UsedSkillPoints;
            //MelonLogger.Msg("totalSkillPoint " + totalSkillPoint);

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

                skillTreeUI ??= new SkillTreeUI(SkillData);
                skillTreeUI?.AddPoints(statsGained, opsGained, socialGained, specialGained);

                MelonLogger.Msg($"[SkillTree] Processed: Rank {LevelManager.Instance.Rank} Tier {LevelManager.Instance.Tier}. Gains: Stats+{statsGained} Operations+{opsGained} Social+{socialGained} Special+{specialGained}");
            }
        }

        private void ValidateSave()
        {
            int currentRank = (int)LevelManager.Instance.Rank;
            int currentTier = LevelManager.Instance.Tier - 1;

            int maxPointsPossible = currentRank * 7 + currentTier;
            int maxPointsJson = SkillData.StatsPoints + SkillData.OperationsPoints + SkillData.SocialPoints + SkillData.SpecialPoints + SkillData.UsedSkillPoints;
            if ((bool)ResetSkills.BoxedValue)
            {
                MelonLogger.Msg($"Reset skills option is enabled. This happens the first time loading a save with version 2.1.0.0 or when manually enabled by the player. Resetting skills.");
                string path = SkillTreeSaveManager.GetDynamicPath();
                if (File.Exists(path))
                    File.Delete(path);
                SkillData = SkillTreeSaveManager.LoadOrCreate();
                skillTreeUI = new SkillTreeUI(SkillData);
                skillPointValid = maxPointsPossible - currentRank;
                specialSkillPointValid = currentRank;
                Version.BoxedValue = version;
                ResetSkills.BoxedValue = false;
            }
            else if (!IsVersionCompatible())
            {
                MelonLogger.Msg("Invalid or outdated skill tree version detected. Resetting skills.");
                string path = SkillTreeSaveManager.GetDynamicPath();
                if (File.Exists(path))
                    File.Delete(path);
                SkillData = SkillTreeSaveManager.LoadOrCreate();
                skillTreeUI = new SkillTreeUI(SkillData);
                skillPointValid = maxPointsPossible - currentRank;
                specialSkillPointValid = currentRank;
                Version.BoxedValue = version;
                ResetSkills.BoxedValue = false;
            }
            else if (maxPointsPossible != maxPointsJson)
            {
                MelonLogger.Msg($"Max Points: ({currentRank} * 7) + {currentTier} = {currentRank * 7 + currentTier}");
                MelonLogger.Msg($"Max Points JSON: {SkillData.StatsPoints} + {SkillData.OperationsPoints} + " +
                    $"{SkillData.SocialPoints} + {SkillData.SpecialPoints} + {SkillData.UsedSkillPoints} = " +
                    $"{SkillData.StatsPoints + SkillData.OperationsPoints + SkillData.SocialPoints + SkillData.SpecialPoints + SkillData.UsedSkillPoints}");
                MelonLogger.Msg("Desync detected! Synchronizing points with saved XP in the game...");
                string path = SkillTreeSaveManager.GetDynamicPath();
                if (File.Exists(path))
                    File.Delete(path);
                SkillData = SkillTreeSaveManager.LoadOrCreate();
                skillTreeUI = new SkillTreeUI(SkillData);
                skillPointValid = maxPointsPossible - currentRank;
                specialSkillPointValid = currentRank;
            }
            SkillSystem.ApplyAll();
        }

        private static bool IsVersionCompatible()
        {
            string[] currentVersion = version.Split('.');
            string[] fileVersion = ((string)Version.BoxedValue).Split('.');

            for (int i = 0; i < 2; i++)
            {
                if (!int.TryParse(currentVersion[i], out int current) ||
                    !int.TryParse(fileVersion[i], out int file))
                {
                    return false;
                }

                if (current > file)
                {
                    return false;
                }
            }
            return true;
        }

        public override void OnGUI()
        {
            if (skillTreeUI == null || !skillTreeUI.Visible)
                return;

            skillTreeUI.EnsureSkin();

            GUI.skin = skillTreeUI.Skin;

            if (Event.current.type == EventType.MouseDown)
                GUI.FocusControl(null);

            skillTreeUI.Draw();
        }
    }
}