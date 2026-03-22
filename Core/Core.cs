using HarmonyLib;
using Il2CppScheduleOne.PlayerScripts;
using MelonLoader;
using S1API.GameTime;
using S1API.Leveling;
using S1API.Lifecycle;
using SkillTree.Core;
using SkillTree.Core.Patches.Compatibility;
using SkillTree.Core.Patches.Miscellaneous;
using SkillTree.Core.Patches.Special;
using SkillTree.Core.Patches.Stats;
using SkillTree.Core.Serialization;
using SkillTree.Core.Utilities;
using System;
using System.Collections;
using UnityEngine;

[assembly: MelonInfo(typeof(Core), "SkillTree", "2.6.4", "CrazyReizor & VindicatedVendetta", null)]
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
            SkillTreeData.AddChildren(SkillTreeData.StatsTree);
            SkillTreeData.AddChildren(SkillTreeData.OperationsTree);
            SkillTreeData.AddChildren(SkillTreeData.SocialTree);
            SkillTreeData.AddChildren(SkillTreeData.SpecialTree);

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

            if (Input.GetKeyDown(ConfigManager.MenuHotkey.GetValue()))
                OnOpenKeyPressed?.Invoke();
            if (Input.GetKeyDown(ConfigManager.LevelSkillHotkey.GetValue()))
                OnLevelSkillKeyPressed?.Invoke();

            if (Cursor.lockState != CursorLockMode.None)
            {
                if (Input.GetKeyDown(ConfigManager.ActiveSkillOne.GetValue()) && SkillTreeData.Special.CurrentLevel == 1)
                    SkillActive.GoodSamaritan();

                if (Input.GetKeyDown(ConfigManager.ActiveSkillTwo.GetValue()) && SkillTreeData.Heal.CurrentLevel == 1)
                    SkillActive.BloodRush();

                if (Input.GetKeyDown(ConfigManager.ActiveSkillThree.GetValue()) && SkillTreeData.GetCashDealer.CurrentLevel == 1)
                    SkillActive.SiphonFunds();

                if (Input.GetKeyDown(ConfigManager.ActiveSkillFour.GetValue()) && SkillTreeData.TrickleDown.CurrentLevel == 1)
                    SkillActive.TrickleDownEconomics();

            }
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            base.OnSceneWasLoaded(buildIndex, sceneName);
            if (sceneName != "Main")
            {
                setupComplete = false;

                AllowSleep.Reset();
                NPCPatches.Reset();
                SaveManager.LoadDefaultValues();
                GameLifecycle.OnSaveComplete -= SaveManager.SaveFile;
                LevelManager.OnRankUp -= SkillPoints.ProcessLevelUp;
                TimeManager.OnDayPass -= Cooldowns.ResetSkillCooldowns;
                OnOpenKeyPressed = null;
                OnLevelSkillKeyPressed = null;
                SkillPoints.OnSkillPointsChanged = null;
            }

            if (sceneName == "Main")
            {
                GameLifecycle.OnSaveComplete += SaveManager.SaveFile;
                LevelManager.OnRankUp += SkillPoints.ProcessLevelUp;
                TimeManager.OnDayPass += Cooldowns.ResetSkillCooldowns;
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
    }
}