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
using SkillTree.SkillPatchSocial;
using SkillTree.SkillsJson;
using SkillTree.SkillSpecial.SkillEmployee;
using SkillTree.UI;
using UnityEngine;
using static SkillTree.SkillActive.SkillActive;

[assembly: MelonInfo(typeof(SkillTree.Core), "SkillTree", "1.0.0", "CrazyReizor", null)]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace SkillTree
{
    public static class SkillModifiers
    {
        #region Stats
        public static readonly float PlayerBaseHealth = 100f;
        public static readonly float HealthBonus = 20f;
        public static readonly float PlayerBaseMoveSpeed = 1f;
        public static readonly float MoveSpeedBonus = 0.10f;
        public static readonly float BaseXPGainRate = 100f;
        public static readonly float XPGainBonus = 0.05f;
        public static readonly float SaleXPBonus = 0.05f;
        public static readonly int InventoryStackSizeMultiplier = 2;
        public static readonly float PackagerMoveSpeedMultiplier = 2f;

        public static float GetPlayerMaxHealth()
        {
            return PlayerBaseHealth + (Core.SkillData.Stats * HealthBonus);
        }

        public static float GetPlayerMoveSpeed()
        {
            return PlayerBaseMoveSpeed + (Core.SkillData.MoreMovespeed * MoveSpeedBonus);
        }

        public static float GetXPGainBonus()
        {
            return (Core.SkillData.MoreXP + Core.SkillData.MoreXP2) * XPGainBonus;
        }

        public static float GetSaleXPBonus()
        {
            return Core.SkillData.MoreXPWhenEarnMoney * SaleXPBonus;
        }
        #endregion Stats

        #region Operations
        public static readonly int CauldronBaseOutput = 10;
        public static readonly int CauldronOutputMultiplier = 2;
        public static readonly int StackSizeMultiplier = 2;
        public static readonly int MixDryOutputSizeMultiplier = 2;
        public static readonly int ChemistStationSpeedMultiplier = 2;
        public static readonly float QualityBonusGrowTent = 0.16f;
        public static readonly float QualityBonusPlants = 0.15f;
        public static readonly float QualityBonusShrooms = 0.30f;
        public static readonly int YieldBonusPlants = 1;
        public static readonly float GrowthSpeedBonusPlants = 0.025f;

        public static int GetCauldronStackSize()
        {
            return CauldronBaseOutput * (Core.SkillData.MoreCauldronOutput * CauldronOutputMultiplier);
        }

        public static int GetChemistStationSpeedMultiplier()
        {
            return Core.SkillData.ChemistStationQuick * ChemistStationSpeedMultiplier;
        }

        public static int GetMixDryOutputMultiplier()
        {
            return Core.SkillData.MoreMixAndDryingRackOutput * MixDryOutputSizeMultiplier;
        }

        public static float GetGrowthSpeedMultiplier()
        {
            return 1f + (Core.SkillData.GrowthSpeed + Core.SkillData.GrowthSpeed2) * GrowthSpeedBonusPlants;
        }

        public static float GetGrowTentBonus()
        {
            return Core.SkillData.Operations * QualityBonusGrowTent;
        }

        public static float GetPlantBonus()
        {
            return Core.SkillData.MoreQuality * QualityBonusPlants;
        }

        #endregion Operations

        #region Social
        //public static readonly float BaseWeeklyDepositLimit = ATM.WEEKLY_DEPOSIT_LIMIT;
        public static readonly float BaseWeeklyDepositLimit = 10000f;
        //public static readonly int BaseMaxCustomer = Dealer.MAX_CUSTOMERS;
        public static readonly int BaseMaxCustomer = 10;
        public static readonly float BaseDealerCut = 0.20f;
        //public static readonly int BaseDeadDropItemLimit = Supplier.DEADDROP_ITEM_LIMIT;
        public static readonly int BaseDeadDropItemLimit = 10;
        public static readonly float ATMDepositBonus = 2000f;
        public static readonly int CustomerLimitBonus = 2;
        public static readonly float CustomerSampleAcceptBonus = 0.05f;
        public static readonly float DealerCutReduction = 0.05f;
        public static readonly float SupplierCashBonus = 0.675f;
        public static readonly float SupplierItemBonus = 0.50f;
        public static readonly float LaunderingBonus = 0.20f;
        public static readonly float CustomerCashBonus = 0.10f;
        public static readonly float DealerSpeedBonus = 1f;

        public static float GetATMLimit()
        {
            return BaseWeeklyDepositLimit + (Core.SkillData.MoreATMLimit * ATMDepositBonus);
        }

        public static int GetMaxCustomers()
        {
            return BaseMaxCustomer + (Core.SkillData.DealerMoreCustomer * CustomerLimitBonus);
        }

        public static float GetCustomerSampleBonus()
        {
            return Core.SkillData.Social * CustomerSampleAcceptBonus;
        }

        public static float GetDealerCut()
        {
            return BaseDealerCut - (Core.SkillData.DealerCutLess * DealerCutReduction);
        }

        public static float GetSupplierCashMultiplier()
        {
            return 1f + (Core.SkillData.BetterSupplier * SupplierCashBonus);
        }

        public static int GetSupplierItemLimit()
        {
            return (int)(BaseDeadDropItemLimit * (1f + (Core.SkillData.BetterSupplier * SupplierItemBonus)));
        }

        public static float GetLaunderingCapacityMultiplier()
        {
            return 1f + (Core.SkillData.BusinessEvolving * LaunderingBonus);
        }

        public static float GetCustomerCashMultiplier()
        {
            return 1f + (Core.SkillData.CityEvolving * CustomerCashBonus);
        }

        public static float GetDealerSpeedBonus()
        {
            return Core.SkillData.DealerSpeedUp * DealerSpeedBonus;
        }
        #endregion Social

        #region Special
        public static readonly float EmployeeMoveSpeedBonus = 0.33f;
        public static readonly int EmployeeStationBonus = 2;
        public static readonly int MaxChemistStations = 4;
        public static readonly int MaxBotanistStations = 8;

        //public static float GetEmployeeMoveSpeedBonus()
        //{
        //    return Core.SkillData.EmployeeMovespeed == 0 ? 1f : EmployeeMoveSpeedBonus;
        //}

        public static int GetEmployeeStationBonus()
        {
            return Core.SkillData.EmployeeMaxStation * EmployeeStationBonus;
        }

        public static (int, int) GetChemistStationBonus()
        {
            return (MaxChemistStations + GetEmployeeStationBonus(), MaxChemistStations);
        }

        public static (int, int) GetBotanistStationBonus()
        {
            return (MaxBotanistStations + GetEmployeeStationBonus(), MaxBotanistStations);
        }

        #endregion Special

    }

    public class Core : MelonMod
    {
        public static Core Instance;

        public static SkillTreeData SkillData;
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
                    SkillData = SkillTreeSaveManager.LoadOrCreate();
                    skillConfig = SkillTreeSaveManager.LoadConfig();
                    skillTreeUI = new SkillTreeUI(SkillData, skillConfig);

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
            if (Input.GetKeyDown(KeyCode.F1) && SkillData.Special == 1)
                ClearTrash();

            if (Input.GetKeyDown(KeyCode.F2) && SkillData.Heal == 1)
                Heal();

            if (Input.GetKeyDown(KeyCode.F3) && SkillData.GetCashDealer == 1)
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

                if (skillTreeUI == null)
                    skillTreeUI = new SkillTreeUI(SkillData, skillConfig);

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
            int maxPointsJson = SkillData.StatsPoints + SkillData.OperationsPoints + SkillData.SocialPoints + SkillData.SpecialPoints + SkillData.UsedSkillPoints;

            if (maxPointsPossible != maxPointsJson)
            {
                MelonLogger.Msg($"Max Points: ({currentRank} * 7) + {currentTier} = {(currentRank * 7) + currentTier}");
                MelonLogger.Msg($"Max Points JSON: {SkillData.StatsPoints} + {SkillData.OperationsPoints} + " +
                    $"{SkillData.SocialPoints} + {SkillData.SpecialPoints} + {SkillData.UsedSkillPoints} = " +
                    $"{SkillData.StatsPoints + SkillData.OperationsPoints + SkillData.SocialPoints + SkillData.SpecialPoints + SkillData.UsedSkillPoints}");
                MelonLogger.Msg("Desync detected! Synchronizing points with saved XP in the game...");
                string path = SkillTreeSaveManager.GetDynamicPath();
                if (File.Exists(path))
                    File.Delete(path);
                SkillData = SkillTreeSaveManager.LoadOrCreate();
                skillConfig = SkillTreeSaveManager.LoadConfig();
                skillTreeUI = new SkillTreeUI(SkillData, skillConfig);
                skillPointValid = maxPointsPossible - currentRank;
                specialSkillPointValid = currentRank;
            }
            SkillSystem.ApplyAll(SkillData);
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