using Il2CppScheduleOne;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.Employees;
using Il2CppScheduleOne.GameTime;
using Il2CppScheduleOne.Levelling;
using Il2CppScheduleOne.Money;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.UI;
using MelonLoader;
using SkillTree.Json;
using SkillTree.SkillEffect;
using SkillTree.SkillsJson;
using SkillTree.SkillSpecial.SkillEmployee;
using SkillTree.UI;
using UnityEngine;
using static SkillTree.SkillActive.SkillActive;

[assembly: MelonInfo(typeof(SkillTree.Core), "SkillTree", "1.0.0", "CrazyReizor", null)]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace SkillTree
{
    public class Core : MelonMod
    {
        public static Core Instance;

        private SkillTreeData skillData;
        private SkillConfig skillConfig;
        private SkillTreeUI skillTreeUI;
        private int skillPointValid = 0;
        private int specialSkillPointValid = 0;

        private int lastProcessedTier = -1;
        private ERank lastProcessedRank = (ERank)(-1);

        private float timer = 2f;
        private bool waiting = true;
        private bool treeUiChange = false;

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("SkillTree Initialized.");
            Instance = this;

            var harmony = new HarmonyLib.Harmony("com.reizor.skilltree");
            harmony.PatchAll();

            LoggerInstance.Msg("Harmony patches applied.");
        }

        public void Reset()
        {
            skillPointValid = 0;
            specialSkillPointValid = 0;

            lastProcessedTier = -1;
            lastProcessedRank = (ERank)(-1);

            timer = 2f;
            waiting = true;
            treeUiChange = false;
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

            if (waiting)
            {
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    skillData = SkillTreeSaveManager.LoadOrCreate();
                    skillConfig = SkillTreeSaveManager.LoadConfig();
                    skillTreeUI = new SkillTreeUI(skillData, skillConfig);

                    ItemUnlocker.UnlockSpecificItems();
                    ValidSave();
                    AttPoints();
                    waiting = false;
                }

                if (waiting)
                {
                    return;
                }
            }

            if (lastProcessedTier != LevelManager.Instance.Tier)
                AttPoints(true);

            ActiveSkills();

            if (Input.GetKeyDown(skillConfig.MenuHotkey))
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
        }

        public void ActiveSkills()
        {
            ValidSkill();
            if (Input.GetKeyDown(KeyCode.F1) && SkillEnabled.enabledTrash)
                ClearTrash();

            if (Input.GetKeyDown(KeyCode.F2) && SkillEnabled.enabledHeal)
                Heal();

            if (Input.GetKeyDown(KeyCode.F3) && SkillEnabled.enabledGetCash)
                GetCashDealer();
        }

        public void AttPoints(bool levelUp = false)
        {
            int currentRank = (int)LevelManager.Instance.Rank;
            int currentTier = LevelManager.Instance.Tier - 1;

            if (currentRank == 0 && currentTier == 0)
                return;

            if (levelUp && currentTier == (lastProcessedTier - 1) && (int)LevelManager.Instance.Rank == (int)lastProcessedRank)
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

            int totalSkillPoint = skillData.StatsPoints + skillData.OperationsPoints + skillData.SocialPoints + skillData.UsedSkillPoints;
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

                if (skillTreeUI == null)
                    skillTreeUI = new SkillTreeUI(skillData, skillConfig);

                if (skillTreeUI != null)
                    skillTreeUI.AddPoints(statsGained, opsGained, socialGained, specialGained);

                MelonLogger.Msg($"[SkillTree] Processed: Rank {LevelManager.Instance.Rank} Tier {LevelManager.Instance.Tier}. Gains: Stats+{statsGained} Operations+{opsGained} Social+{socialGained} Special+{specialGained}");
            }
        }

        private void ValidSave()
        {
            int currentRank = (int)LevelManager.Instance.Rank;
            int currentTier = LevelManager.Instance.Tier - 1;

            int maxPointsPossible = (currentRank * 7) + currentTier;
            int maxPointsJson = skillData.StatsPoints + skillData.OperationsPoints + skillData.SocialPoints + skillData.SpecialPoints + skillData.UsedSkillPoints;

            if (maxPointsPossible != maxPointsJson)
            {
                MelonLogger.Msg($"Max Points: ({currentRank} * 7) + {currentTier} = {(currentRank * 7) + currentTier}");
                MelonLogger.Msg($"Max Points JSON: {skillData.StatsPoints} + {skillData.OperationsPoints} + " +
                    $"{skillData.SocialPoints} + {skillData.SpecialPoints} + {skillData.UsedSkillPoints} = " +
                    $"{skillData.StatsPoints + skillData.OperationsPoints + skillData.SocialPoints + skillData.SpecialPoints + skillData.UsedSkillPoints}");
                MelonLogger.Msg("Desync detected! Synchronizing points with saved XP in the game...");
                string path = SkillTreeSaveManager.GetDynamicPath();
                if (File.Exists(path))
                    File.Delete(path);
                skillData = SkillTreeSaveManager.LoadOrCreate();
                skillConfig = SkillTreeSaveManager.LoadConfig();
                skillTreeUI = new SkillTreeUI(skillData, skillConfig);
                skillPointValid = maxPointsPossible - currentRank;
                specialSkillPointValid = currentRank;
            }
            SkillSystem.ApplyAll(skillData);
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