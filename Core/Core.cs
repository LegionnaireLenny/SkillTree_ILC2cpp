using HarmonyLib;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.PlayerScripts;
using MelonLoader;
using S1API.GameTime;
using S1API.Leveling;
using S1API.Lifecycle;
using SkillTree.Core;
using SkillTree.Core.Patches.Compatibility;
using SkillTree.Core.Patches.Hustler;
using SkillTree.Core.Patches.Miscellaneous;
using SkillTree.Core.Patches.Special;
using SkillTree.Core.Serialization;
using SkillTree.Core.Utilities;
using System;
using System.Collections;
using UnityEngine;
using static SkillTree.Core.Utilities.ConfigManager;

[assembly: MelonInfo(typeof(Core), "SkillTree", "3.0.0", "CrazyReizor & VindicatedVendetta", null)]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace SkillTree.Core
{
    public class Core : MelonMod
    {
        private readonly float delayTime = 3f;
        private bool setupComplete = false;
        public static Action OnOpenKeyPressed;
        public static Action OnLevelSkillKeyPressed;

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
            SkillTreeData.CreateTrees();

            LoggerInstance.Msg("SkillTree Initialized.");
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

            if (Input.GetKeyDown(MenuHotkey.GetValue(UseDefaultSkillParameters.GetValue())))
                OnOpenKeyPressed?.Invoke();
            if (Input.GetKeyDown(LevelSkillHotkey.GetValue(UseDefaultSkillParameters.GetValue())))
                OnLevelSkillKeyPressed?.Invoke();

            if (Cursor.lockState != CursorLockMode.None && 
                !PlayerSingleton<PlayerCamera>.Instance.activeUIElements.Contains("Console"))
            {
                if (Input.GetKeyDown(GoodSamaritanHotkey.GetValue(UseDefaultSkillParameters.GetValue())) && SkillTreeData.GoodSamaritan.CurrentLevel == 1)
                    SkillActive.GoodSamaritan();

                if (Input.GetKeyDown(BloodRushHotkey.GetValue(UseDefaultSkillParameters.GetValue())) && SkillTreeData.BloodRush.CurrentLevel == 1)
                    SkillActive.BloodRush();

                if (Input.GetKeyDown(SiphonFundsHotkey.GetValue(UseDefaultSkillParameters.GetValue())) && SkillTreeData.SiphonFunds.CurrentLevel == 1)
                    SkillActive.SiphonFunds();

                if (Input.GetKeyDown(TrickledownHotkey.GetValue(UseDefaultSkillParameters.GetValue())) && SkillTreeData.TrickleDown.CurrentLevel == 1)
                    SkillActive.TrickleDownEconomics();

                if (Input.GetKeyDown(BloodMoneyHotkey.GetValue(UseDefaultSkillParameters.GetValue())) && SkillTreeData.BloodMoney.CurrentLevel == 1)
                    SkillActive.BloodMoney();

                if (Input.GetKeyDown(InfectiousPersonalityHotkey.GetValue(UseDefaultSkillParameters.GetValue())) && SkillTreeData.InfectiousPersonality.CurrentLevel == 1)
                    SkillActive.InfectiousPersonality();

                if (Input.GetKeyDown(AdrenalineSurgeHotkey.GetValue(UseDefaultSkillParameters.GetValue())) && SkillTreeData.AdrenalineSurge.CurrentLevel == 1)
                    SkillActive.AdrenalineSurge();

                if (Input.GetKeyDown(AntiGravityBongHotkey.GetValue(UseDefaultSkillParameters.GetValue())) && SkillTreeData.AntiGravityBong.CurrentLevel == 1)
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
                SaveManager.LoadDefaultValues();
                GameLifecycle.OnSaveComplete -= SaveManager.SaveFile;
                LevelManager.OnRankUp -= SkillPoints.ProcessLevelUp;
                TimeManager.OnDayPass -= Cooldowns.ResetDailySkills;
                TimeManager.OnDayPass -= SkillActive.ResetAfflicted;
                OnOpenKeyPressed = null;
                OnLevelSkillKeyPressed = null;
                SkillPoints.OnSkillPointsChanged = null;
            }

            if (sceneName == "Main")
            {
                GameLifecycle.OnSaveComplete += SaveManager.SaveFile;
                LevelManager.OnRankUp += SkillPoints.ProcessLevelUp;
                TimeManager.OnDayPass += Cooldowns.ResetDailySkills;
                TimeManager.OnDayPass += SkillActive.ResetAfflicted;
                if (ResetSkills.GetValue(UseDefaultSkillParameters.GetValue()))
                {
                    MelonLogger.Warning($"Reset skills option is enabled. This happens the first time a save loaded with version 2.1.0 and later or when manually enabled by the player. Resetting skills.");
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