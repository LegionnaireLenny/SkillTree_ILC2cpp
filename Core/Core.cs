using HarmonyLib;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.PlayerScripts;
using MelonLoader;
using MelonLoader.Utils;
using S1API.GameTime;
using S1API.Leveling;
using S1API.Lifecycle;
using SkillTree.Core;
using SkillTree.Core.Patches.Compatibility;
using SkillTree.Core.Patches.Hustler;
using SkillTree.Core.Patches.Logistician;
using SkillTree.Core.Patches.Miscellaneous;
using SkillTree.Core.Patches.Special;
using SkillTree.Core.Serialization;
using SkillTree.Core.Utilities;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using static SkillTree.Core.Utilities.ConfigManager;

[assembly: MelonInfo(typeof(Core), "SkillTree", "3.3.1", "CrazyReizor & VindicatedVendetta", null)]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace SkillTree.Core
{
    public class Core : MelonMod
    {
        private readonly float delayTime = 3f;
        private bool setupComplete = false;
        public static Core Instance;
        public static Action OnOpenKeyPressed;
        public static Action OnLevelSkillKeyPressed;
        public static Action RemoveOnSceneChange;
        public static bool ApplyeMployeePatch { get; private set; } = false;

        public static readonly string BaseDirectory = Path.Combine(MelonEnvironment.UserDataDirectory, "SkillTree");
        public static readonly string IconDirectory = Path.Combine(BaseDirectory, "Icons");
        public static readonly string SaveDirectory = Path.Combine(BaseDirectory, "Saves");
        public static readonly string LocalizationDirectory = Path.Combine(BaseDirectory, "Localization");

        public static readonly string ConfigFile = Path.Combine(BaseDirectory, "SkillTree_Config.cfg");

        public static readonly string OldIconDirectory = Path.Combine(MelonEnvironment.UserDataDirectory, "S1API", "Icons", "SkillTree");
        public static readonly string OldSaveDirectory = MelonEnvironment.UserDataDirectory;
        public static readonly string OldConfigFile = Path.Combine(MelonEnvironment.UserDataDirectory, "SkillTree_Config.cfg");

        public override void OnInitializeMelon()
        {
            Instance = this;
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
                catch (Exception ex)
                {
                    LoggerInstance.Msg($"Empire 2.0 patch failed {ex}");
                }
            }

            if (MelonBase.RegisteredMelons.Contains(FindMelon("eMployee", "V4LEXL")))
            {
                ApplyeMployeePatch = true;
                LoggerInstance.Msg("eMployee found, bypassing Night Shift skill patches");
            }

            InitializeDirectories();
            MigrateFiles(LoggerInstance);
            ConfigManager.Initialize();
            IconManager.ExtractIcons();
            LocalizationManager.Initialize();
            SkillTreeData.CreateTrees();

            LoggerInstance.Msg("SkillTree Initialized.");
        }

        public static void InitializeDirectories()
        {
            Directory.CreateDirectory(BaseDirectory);
            Directory.CreateDirectory(IconDirectory);
            Directory.CreateDirectory(SaveDirectory);
            Directory.CreateDirectory(LocalizationDirectory);
        }

        public static void MigrateFiles(MelonLogger.Instance instance)
        {
            try
            {
                if (File.Exists(OldConfigFile))
                {
                    instance.Msg($"Migrating config file: {OldConfigFile} to {ConfigFile}");
                    File.Move(OldConfigFile, ConfigFile);
                }
            }
            catch (Exception ex)
            {
                instance.Warning($"Failed migrating config file: {ex}"); 
            }

            try
            {
                if (Directory.Exists(OldIconDirectory) && !Directory.Exists(IconDirectory))
                {
                    instance.Msg($"Migrating icon directory: {OldIconDirectory} to {IconDirectory}");
                    Directory.Move(OldIconDirectory, IconDirectory);
                }
            }
            catch (Exception ex) 
            {
                instance.Warning($"Failed migrating icon files: {ex}"); 
            }

            try
            {
                string[] saveFiles = Directory.GetFiles(OldSaveDirectory, "SkillTree_*");
                if (saveFiles.Length > 0)
                {
                    foreach (string file in saveFiles)
                    {
                        string destination = Path.Combine(SaveDirectory, Path.GetFileName(file));
                        instance.Msg($"Migrating save file: {file} to {destination}");
                        File.Move(file, destination);                   
                    }
                }
            }
            catch (Exception ex)
            {
                instance.Warning($"Failed migrating save files: {ex}"); 
            }
        }

        private IEnumerator DelayedSetup()
        {
            yield return new WaitForSeconds(delayTime);
            ShopPatches.ChangeItemRankRequirements();
            SkillPoints.ValidateTotalSkillPoints();
            SaveManager.SaveFile();
            SkillTreeData.ApplyAllSkills();
            setupComplete = true;
        }

        public override void OnUpdate()
        {
            if (PlayerMovement.Instance == null ||
                PlayerCamera.Instance == null ||
                PlayerInventory.Instance == null ||
                PlayerManager.Instance == null ||
                Player.Local == null)
                return;

            if (!setupComplete)
            {
                return;
            }

            if (Input.GetKeyDown(MenuHotkey.GetValue()))
                OnOpenKeyPressed?.Invoke();
            if (Input.GetKeyDown(LevelSkillHotkey.GetValue()))
                OnLevelSkillKeyPressed?.Invoke();

            if (Cursor.lockState != CursorLockMode.None && 
                !PlayerSingleton<PlayerCamera>.Instance.activeUIElements.Contains("Console"))
            {
                if (Input.GetKeyDown(GoodSamaritanHotkey.GetValue()) && SkillTreeData.GoodSamaritan.CurrentLevel == 1)
                    SkillActive.GoodSamaritan();

                if (Input.GetKeyDown(BloodRushHotkey.GetValue()) && SkillTreeData.BloodRush.CurrentLevel == 1)
                    SkillActive.BloodRush();

                if (Input.GetKeyDown(SiphonFundsHotkey.GetValue()) && SkillTreeData.SiphonFunds.CurrentLevel == 1)
                    SkillActive.SiphonFunds();

                if (Input.GetKeyDown(TrickledownHotkey.GetValue()) && SkillTreeData.TrickleDown.CurrentLevel == 1)
                    SkillActive.TrickleDownEconomics();

                if (Input.GetKeyDown(BloodMoneyHotkey.GetValue()) && SkillTreeData.BloodMoney.CurrentLevel == 1)
                    SkillActive.BloodMoney();

                if (Input.GetKeyDown(InfectiousPersonalityHotkey.GetValue()) && SkillTreeData.InfectiousPersonality.CurrentLevel == 1)
                    SkillActive.InfectiousPersonality();

                if (Input.GetKeyDown(AdrenalineSurgeHotkey.GetValue()) && SkillTreeData.AdrenalineSurge.CurrentLevel == 1)
                    SkillActive.AdrenalineSurge();

                if (Input.GetKeyDown(AntiGravityBongHotkey.GetValue()) && SkillTreeData.AntiGravityBong.CurrentLevel == 1)
                    SkillActive.AntiGravityBong();
            }
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            base.OnSceneWasLoaded(buildIndex, sceneName);
            if (sceneName != "Main")
            {
                setupComplete = false;

                NPCPatches.Reset();
                ChemistBehaviorPatches.CleanupCoroutines();
                SaveManager.LoadDefaultValues();
                GameLifecycle.OnSaveComplete -= SaveManager.SaveFile;
                LevelManager.OnRankUp -= SkillPoints.ProcessLevelUp;
                TimeManager.OnDayPass -= Cooldowns.ResetDailySkills;
                TimeManager.OnDayPass -= SkillActive.ResetAfflicted;
                OnOpenKeyPressed = null;
                OnLevelSkillKeyPressed = null;
                SkillPoints.OnSkillPointsChanged = null;
                LocalizationManager.OnLocaleUpdated -= RemoveOnSceneChange;
                RemoveOnSceneChange = null;
            }

            if (sceneName == "Main")
            {
                GameLifecycle.OnSaveComplete += SaveManager.SaveFile;
                LevelManager.OnRankUp += SkillPoints.ProcessLevelUp;
                TimeManager.OnDayPass += Cooldowns.ResetDailySkills;
                TimeManager.OnDayPass += SkillActive.ResetAfflicted;
                if (ResetSkills.GetValue())
                {
                    LogManager.LogMessage($"Reset skills option is enabled. This happens the first time a save loaded with version 2.1.0 and later or when manually enabled by the player. Resetting skills.", LogLevel.Warning);
                    ResetSkills.SetValue(false);
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
    }
}