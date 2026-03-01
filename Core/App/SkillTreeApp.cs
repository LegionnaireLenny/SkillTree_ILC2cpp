using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.UI.Phone;
using MelonLoader.TinyJSON;
using S1API.PhoneApp;
using S1API.UI;
using S1API.Utils;
using SkillTree.Core.FileManagement;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SkillTree.Core.App
{
    public class SkillNode : MonoBehaviour
    {
        public Skill Skill;
        public GameObject GO;

        public SkillNode(Skill skill, GameObject go)
        {
            Skill = skill;
            GO = go;
        }
    }

    public class SkillTreeApp : PhoneApp
    {
        protected override string AppName => "SkillTreeApp";
        protected override string AppTitle => "Skills";
        protected override string IconLabel => "Skills";
        protected override string IconFileName => "SkillTree (Forked).png";

        private static Skill selectedSkill = null;

        // TODO: ButtonWithLabel is bugged, width and height are swapped. new S1API version hasn't released with fix
        private static float buttonWidth = 50f;
        private static float buttonHeight = 125f;
        private static Color colorBackground = new Color(0.1f, 0.1f, 0.1f);
        private static Color colorButton = new Color(0.25f, 0.25f, 0.25f);
        private static Color colorDetails = new Color(0.2f, 0.2f, 0.2f);
        private static Color colorMaxLevelSkill = new Color(0.25f, 0.4f, 0.25f);
        private static Color colorUnlockedSkill = new Color(0.25f, 0.25f, 0.25f);
        private static Color colorLockedSkill = new Color(0.15f, 0.15f, 0.15f);
        private static Color colorText = Color.yellow;

        protected override void OnCreated()
        {
            base.OnCreated();
        }

        protected override void OnCreatedUI(GameObject container)
        {
            var appBackground = UIFactory.Panel("MainPanel", container.transform, colorBackground, fullAnchor: true);
            var topBarContainer = UIFactory.Panel("TopBarContainer", appBackground.transform, colorBackground, anchorMin: new Vector2(0.084f, 1.1f), anchorMax: new Vector2(0.575f, 0.85f));
            var topBar = UIFactory.TopBar("TopBar", topBarContainer.transform, "Skills", 0.82f, 15, 15, 5, 5);
            topBar.GetComponentInChildren<Text>().color = colorText;
            topBar.GetComponentInChildren<LayoutElement>().minWidth = 150;

            var categoryContainer = UIFactory.ButtonRow("SkillCategories", topBar.transform);
            var buttonCategoryStats = UIFactory.ButtonWithLabel("ButtonCategoryStats", $"Stats ({SkillPoints.StatsPoints})", categoryContainer.transform, colorButton, buttonWidth, buttonHeight);
            var buttonCategoryOperations = UIFactory.ButtonWithLabel("ButtonCategoryOperations", $"Operations ({SkillPoints.OperationsPoints})", categoryContainer.transform, colorButton, buttonWidth, buttonHeight);
            var buttonCategorySocial = UIFactory.ButtonWithLabel("ButtonCategorySocial", $"Social ({SkillPoints.SocialPoints})", categoryContainer.transform, colorButton, buttonWidth, buttonHeight);
            var buttonCategorySpecial = UIFactory.ButtonWithLabel("ButtonCategorySpecial", $"Special ({SkillPoints.SpecialPoints})", categoryContainer.transform, colorButton, buttonWidth, buttonHeight);
            buttonCategoryStats.Item3.color = colorText;
            buttonCategoryOperations.Item3.color = colorText;
            buttonCategorySocial.Item3.color = colorText;
            buttonCategorySpecial.Item3.color = colorText;

            var statsTreeContainer = UIFactory.Panel("StatsTreeContainer", appBackground.transform, colorBackground, anchorMin: new Vector2(0.04f, 0.08f), anchorMax: new Vector2(0.618f, 0.8f));
            var operationsTreeContainer = UIFactory.Panel("OperationsTreeContainer", appBackground.transform, colorBackground, anchorMin: new Vector2(0.04f, 0.08f), anchorMax: new Vector2(0.618f, 0.8f));
            var socialTreeContainer = UIFactory.Panel("SocialTreeContainer", appBackground.transform, colorBackground, anchorMin: new Vector2(0.04f, 0.08f), anchorMax: new Vector2(0.618f, 0.8f));
            var specialTreeContainer = UIFactory.Panel("SpecialTreeContainer", appBackground.transform, colorBackground, anchorMin: new Vector2(0.04f, 0.08f), anchorMax: new Vector2(0.618f, 0.8f));
            UIFactory.VerticalLayoutOnGO(statsTreeContainer);
            UIFactory.VerticalLayoutOnGO(operationsTreeContainer);
            UIFactory.VerticalLayoutOnGO(socialTreeContainer);
            UIFactory.VerticalLayoutOnGO(specialTreeContainer);

            var detailsPanel = UIFactory.Panel("DetailsPanel", appBackground.transform, colorDetails, new Vector2(0.7f, 0.085f), new Vector2(0.958f, 0.915f));
            var skillName = UIFactory.Text("SkillName", "", detailsPanel.transform, fontSize: 20, anchor: TextAnchor.MiddleCenter);
            var skillDescription = UIFactory.Text("SkillDescription", "", detailsPanel.transform, fontSize: 18, anchor: TextAnchor.MiddleCenter);
            var skillLevel = UIFactory.Text("SkillLevel", "", detailsPanel.transform, fontSize:18, anchor: TextAnchor.MiddleCenter);
            var buttonLevelUpSkill = UIFactory.RoundedButtonWithLabel("ButtonLevelUpSkill", "Level Skill", detailsPanel.transform, colorButton, 100f, 50f, 18, colorText);
            skillName.color = colorText;
            skillDescription.color = colorText;
            skillLevel.color = colorText;
            UIFactory.VerticalLayoutOnGO(detailsPanel);

            List<SkillNode> statsNodes = CreateNodeTree(SkillTreeData.StatsTree, statsTreeContainer, skillName, skillDescription, skillLevel);
            List<SkillNode> operationsNodes = CreateNodeTree(SkillTreeData.OperationsTree, operationsTreeContainer, skillName, skillDescription, skillLevel);
            List<SkillNode> socialNodes = CreateNodeTree(SkillTreeData.SocialTree, socialTreeContainer, skillName, skillDescription, skillLevel);
            List<SkillNode> specialNodes = CreateNodeTree(SkillTreeData.SpecialTree, specialTreeContainer, skillName, skillDescription, skillLevel);

            operationsTreeContainer.SetActive(false);
            socialTreeContainer.SetActive(false);
            specialTreeContainer.SetActive(false);

            ButtonUtils.AddListener(buttonLevelUpSkill.Item2, () =>
            {
                bool levelUpSucceeded = selectedSkill.IncreaseSkillLevel();
                if (levelUpSucceeded)
                {
                    SkillSystem.ApplySkill(selectedSkill.Name);
                    skillLevel.text = $"Level {selectedSkill.CurrentLevel} / {selectedSkill.MaxLevel}";
                    UpdateText();
                    UpdateNodes();
                }
            });

            ButtonUtils.AddListener(buttonCategoryStats.Item2, () =>
            {
                statsTreeContainer.SetActive(true);
                operationsTreeContainer.SetActive(false);
                socialTreeContainer.SetActive(false);
                specialTreeContainer.SetActive(false);
                UpdateText();
                UpdateNodes();
            });
            ButtonUtils.AddListener(buttonCategoryOperations.Item2, () =>
            {
                statsTreeContainer.SetActive(false);
                operationsTreeContainer.SetActive(true);
                socialTreeContainer.SetActive(false);
                specialTreeContainer.SetActive(false);
                UpdateText();
                UpdateNodes();
            });
            ButtonUtils.AddListener(buttonCategorySocial.Item2, () =>
            {
                statsTreeContainer.SetActive(false);
                operationsTreeContainer.SetActive(false);
                socialTreeContainer.SetActive(true);
                specialTreeContainer.SetActive(false);
                UpdateText();
                UpdateNodes();
            });
            ButtonUtils.AddListener(buttonCategorySpecial.Item2, () =>
            {
                statsTreeContainer.SetActive(false);
                operationsTreeContainer.SetActive(false);
                socialTreeContainer.SetActive(false);
                specialTreeContainer.SetActive(true);
                UpdateText();
                UpdateNodes();
            });

            void UpdateText()
            {
                buttonCategoryStats.Item3.text = $"Stats ({SkillPoints.StatsPoints})";
                buttonCategoryOperations.Item3.text = $"Operations ({SkillPoints.OperationsPoints})";
                buttonCategorySocial.Item3.text = $"Social ({SkillPoints.SocialPoints})";
                buttonCategorySpecial.Item3.text = $"Special ({SkillPoints.SpecialPoints})";
            }

            void UpdateAppearance(List<SkillNode> nodeTree)
            {
                foreach (var node in nodeTree)
                {
                    node.GO.GetComponentInChildren<Text>().text = $"{node.Skill.Name} {node.Skill.CurrentLevel} / {node.Skill.MaxLevel}";
                    node.GO.transform.GetChild(0).GetComponent<Image>().color = GetButtonColor(node.Skill);
                }
            }

            void UpdateNodes()
            {
                UpdateAppearance(statsNodes);
                UpdateAppearance(operationsNodes);
                UpdateAppearance(socialNodes);
                UpdateAppearance(specialNodes);
            }

            static Color GetButtonColor(Skill skill)
            {
                Color color = colorLockedSkill;

                if (skill.IsParentUnlocked())
                {
                    color = colorUnlockedSkill;
                }

                if (skill.IsSkillMaxLevel())
                {
                    color = colorMaxLevelSkill;
                }

                return color;
            }

            static List<SkillNode> CreateNodeTree(HashSet<Skill> skillTree, GameObject parent, Text skillName, Text skillDescription, Text skillLevel)
            {
                static int GetDepth(Skill skill)
                {
                    if (skill.Parent != null)
                    {
                        return GetDepth(skill.Parent) + 1;
                    }
                    return 0;
                }

                List<SkillNode> nodes = new List<SkillNode>();

                foreach (var skill in skillTree)
                {
                    int offset = GetDepth(skill) * 50;

                    var temp = UIFactory.RoundedButtonWithLabel(skill.Name, $"{skill.Name} {skill.CurrentLevel} / {skill.MaxLevel}", parent.transform, GetButtonColor(skill), 30f, 30f, 14, colorText);
                    temp.Item1.GetComponent<Mask>().showMaskGraphic = false;
                    temp.Item2.GetComponent<RectTransform>().offsetMin = new Vector2(offset, 0);
                    temp.Item3.GetComponent<RectTransform>().offsetMin = new Vector2(-offset, 0);
                    SkillNode node = new SkillNode(skill, temp.Item1);
                    ButtonUtils.AddListener(temp.Item2, () =>
                    {
                        skillName.text = skill.Name;
                        skillDescription.text = skill.Description;
                        skillLevel.text = $"Level {skill.CurrentLevel} / {skill.MaxLevel}";
                        selectedSkill = skill;
                    });
                    nodes.Add(node);
                }

                return nodes;
            }
        }
    }
}