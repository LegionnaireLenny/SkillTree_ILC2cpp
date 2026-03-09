using S1API.PhoneApp;
using S1API.UI;
using S1API.Utils;
using SkillTree.Core.Skills;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SkillTree.Core.App
{
    public class SkillNode
    {
        public Skill Skill;
        public (GameObject, Button, Text) GO;

        public SkillNode(Skill skill, (GameObject, Button, Text) go)
        {
            Skill = skill;
            GO = go;
        }

        public void SetText(string text)
        {
            GO.Item3.text = text;
        }

        public void SetButtonColor(Color color)
        {
            GO.Item1.transform.GetChild(0).GetComponent<Image>().color = color;
        }
    }

    public static class FactoryUtils
    {
        public static void SetButtonColor((GameObject, Button, Text) go, Color color)
        {
            go.Item1.GetComponent<Image>().color = color;
        }
    }

    public class SkillTreeApp : PhoneApp
    {
        protected override string AppName => "SkillTreeApp";
        protected override string AppTitle => "Skills";
        protected override string IconLabel => "Skills";
        protected override string IconFileName => Core.IconApp;
        protected override Sprite IconSprite => Core.GetSprite(Core.IconDirectory, Core.IconApp);

        private static SkillNode previousSkill = null;
        private static SkillNode selectedSkill = null;

        private static readonly float buttonWidth = Core.IsS1APIPatchNeeded ? 50f : 125f;
        private static readonly float buttonHeight = Core.IsS1APIPatchNeeded ? 125f : 50f;
        private static readonly Color colorBackground = new Color(0.1f, 0.1f, 0.1f);
        private static readonly Color colorButton = new Color(0.25f, 0.25f, 0.25f);
        private static readonly Color colorButtonSelected = new Color(0.35f, 0.35f, 0.35f);
        private static readonly Color colorDetails = new Color(0.2f, 0.2f, 0.2f);
        private static readonly Color colorMaxLevelSkill = new Color(0.25f, 0.4f, 0.25f);
        private static readonly Color colorMaxLevelSkillSelected = new Color(0.35f, 0.5f, 0.35f);
        private static readonly Color colorUnlockedSkill = new Color(0.25f, 0.25f, 0.25f);
        private static readonly Color colorUnlockedSkillSelected = new Color(0.35f, 0.35f, 0.45f);
        private static readonly Color colorLockedSkill = new Color(0.15f, 0.15f, 0.15f);
        private static readonly Color colorLockedSkillSelected = new Color(0.35f, 0.25f, 0.25f);
        private static readonly Color colorText = Color.yellow;

        protected override void OnCreated()
        {
            base.OnCreated();
        }

        protected override void OnCreatedUI(GameObject container)
        {
            var backgroundApp = UIFactory.Panel("MainPanel", container.transform, colorBackground, fullAnchor: true);
            var containerTopBar = UIFactory.Panel("TopBarContainer", backgroundApp.transform, colorBackground, anchorMin: new Vector2(0.084f, 1.1f), anchorMax: new Vector2(0.575f, 0.85f));
            var topBar = UIFactory.TopBar("TopBar", containerTopBar.transform, "Skills", 0.82f, 15, 15, 5, 5);
            topBar.GetComponentInChildren<Text>().color = colorText;
            topBar.GetComponentInChildren<LayoutElement>().minWidth = 150;

            var categoryContainer = UIFactory.ButtonRow("SkillCategories", topBar.transform);
            var categoryStats = UIFactory.ButtonWithLabel("ButtonCategoryStats", $"Stats ({SkillPoints.StatsPoints})", categoryContainer.transform, colorButton, buttonWidth, buttonHeight);
            var categoryOperations = UIFactory.ButtonWithLabel("ButtonCategoryOperations", $"Operations ({SkillPoints.OperationsPoints})", categoryContainer.transform, colorButton, buttonWidth, buttonHeight);
            var categorySocial = UIFactory.ButtonWithLabel("ButtonCategorySocial", $"Social ({SkillPoints.SocialPoints})", categoryContainer.transform, colorButton, buttonWidth, buttonHeight);
            var categorySpecial = UIFactory.ButtonWithLabel("ButtonCategorySpecial", $"Special ({SkillPoints.SpecialPoints})", categoryContainer.transform, colorButton, buttonWidth, buttonHeight);
            FactoryUtils.SetButtonColor(categoryStats, colorButtonSelected);
            categoryStats.Item3.color = colorText;
            categoryOperations.Item3.color = colorText;
            categorySocial.Item3.color = colorText;
            categorySpecial.Item3.color = colorText;

            var treeContainerStats = UIFactory.Panel("StatsTreeContainer", backgroundApp.transform, colorBackground, anchorMin: new Vector2(0.04f, 0.08f), anchorMax: new Vector2(0.618f, 0.8f));
            var treeContainerOperations = UIFactory.Panel("OperationsTreeContainer", backgroundApp.transform, colorBackground, anchorMin: new Vector2(0.04f, 0.08f), anchorMax: new Vector2(0.618f, 0.8f));
            var treeContainerSocial = UIFactory.Panel("SocialTreeContainer", backgroundApp.transform, colorBackground, anchorMin: new Vector2(0.04f, 0.08f), anchorMax: new Vector2(0.618f, 0.8f));
            var treeContainerSpecial = UIFactory.Panel("SpecialTreeContainer", backgroundApp.transform, colorBackground, anchorMin: new Vector2(0.04f, 0.08f), anchorMax: new Vector2(0.618f, 0.8f));
            UIFactory.VerticalLayoutOnGO(treeContainerStats);
            UIFactory.VerticalLayoutOnGO(treeContainerOperations);
            UIFactory.VerticalLayoutOnGO(treeContainerSocial);
            UIFactory.VerticalLayoutOnGO(treeContainerSpecial);

            var detailsPanel = UIFactory.Panel("DetailsPanel", backgroundApp.transform, colorDetails, new Vector2(0.7f, 0.085f), new Vector2(0.958f, 0.915f));
            var detailsSkillName = UIFactory.Text("SkillName", "", detailsPanel.transform, fontSize: 20, anchor: TextAnchor.MiddleCenter);
            var detailsSkillDescription = UIFactory.Text("SkillDescription", "", detailsPanel.transform, fontSize: 18, anchor: TextAnchor.MiddleCenter);
            var detailsSkillLevel = UIFactory.Text("SkillLevel", "", detailsPanel.transform, fontSize:18, anchor: TextAnchor.MiddleCenter);
            var detailsLevelUpSkill = UIFactory.RoundedButtonWithLabel("ButtonLevelUpSkill", "Level Skill", detailsPanel.transform, colorButton, 100f, 50f, 18, colorText);
            detailsSkillName.color = colorText;
            detailsSkillDescription.color = colorText;
            detailsSkillLevel.color = colorText;
            UIFactory.VerticalLayoutOnGO(detailsPanel);

            List<SkillNode> nodesStats = CreateNodeTree(SkillTreeData.StatsTree, treeContainerStats, detailsSkillName, detailsSkillDescription, detailsSkillLevel);
            List<SkillNode> nodesOperations = CreateNodeTree(SkillTreeData.OperationsTree, treeContainerOperations, detailsSkillName, detailsSkillDescription, detailsSkillLevel);
            List<SkillNode> nodesSocial = CreateNodeTree(SkillTreeData.SocialTree, treeContainerSocial, detailsSkillName, detailsSkillDescription, detailsSkillLevel);
            List<SkillNode> nodesSpecial = CreateNodeTree(SkillTreeData.SpecialTree, treeContainerSpecial, detailsSkillName, detailsSkillDescription, detailsSkillLevel);

            selectedSkill = nodesStats[0];
            UpdateDetails();
            treeContainerStats.SetActive(true);
            treeContainerOperations.SetActive(false);
            treeContainerSocial.SetActive(false);
            treeContainerSpecial.SetActive(false);

            ButtonUtils.AddListener(detailsLevelUpSkill.Item2, () =>
            {
                bool levelUpSucceeded = selectedSkill?.Skill.IncreaseSkillLevel() ?? false;
                if (levelUpSucceeded)
                {
                    detailsSkillLevel.text = $"Level {selectedSkill.Skill.CurrentLevel} / {selectedSkill.Skill.MaxLevel}";
                    UpdateCategoryText();
                    UpdateNodes();
                }
            });

            ButtonUtils.AddListener(categoryStats.Item2, () =>
            {
                selectedSkill = nodesStats[0];

                treeContainerStats.SetActive(true);
                treeContainerOperations.SetActive(false);
                treeContainerSocial.SetActive(false);
                treeContainerSpecial.SetActive(false);

                FactoryUtils.SetButtonColor(categoryStats, colorButtonSelected);
                FactoryUtils.SetButtonColor(categoryOperations, colorButton);
                FactoryUtils.SetButtonColor(categorySocial, colorButton);
                FactoryUtils.SetButtonColor(categorySpecial, colorButton);

                UpdateDetails();
                UpdateCategoryText();
                UpdateNodes();
            });
            ButtonUtils.AddListener(categoryOperations.Item2, () =>
            {
                selectedSkill = nodesOperations[0];

                treeContainerStats.SetActive(false);
                treeContainerOperations.SetActive(true);
                treeContainerSocial.SetActive(false);
                treeContainerSpecial.SetActive(false);

                FactoryUtils.SetButtonColor(categoryStats, colorButton);
                FactoryUtils.SetButtonColor(categoryOperations, colorButtonSelected);
                FactoryUtils.SetButtonColor(categorySocial, colorButton);
                FactoryUtils.SetButtonColor(categorySpecial, colorButton);

                UpdateDetails();
                UpdateCategoryText();
                UpdateNodes();
            });
            ButtonUtils.AddListener(categorySocial.Item2, () =>
            {
                selectedSkill = nodesSocial[0];

                treeContainerStats.SetActive(false);
                treeContainerOperations.SetActive(false);
                treeContainerSocial.SetActive(true);
                treeContainerSpecial.SetActive(false);

                FactoryUtils.SetButtonColor(categoryStats, colorButton);
                FactoryUtils.SetButtonColor(categoryOperations, colorButton);
                FactoryUtils.SetButtonColor(categorySocial, colorButtonSelected);
                FactoryUtils.SetButtonColor(categorySpecial, colorButton);

                UpdateDetails();
                UpdateCategoryText();
                UpdateNodes();
            });
            ButtonUtils.AddListener(categorySpecial.Item2, () =>
            {
                selectedSkill = nodesSpecial[0];

                treeContainerStats.SetActive(false);
                treeContainerOperations.SetActive(false);
                treeContainerSocial.SetActive(false);
                treeContainerSpecial.SetActive(true);

                FactoryUtils.SetButtonColor(categoryStats, colorButton);
                FactoryUtils.SetButtonColor(categoryOperations, colorButton);
                FactoryUtils.SetButtonColor(categorySocial, colorButton);
                FactoryUtils.SetButtonColor(categorySpecial, colorButtonSelected);

                UpdateDetails();
                UpdateCategoryText();
                UpdateNodes();
            });

            void UpdateCategoryText()
            {
                categoryStats.Item3.text = $"Stats ({SkillPoints.StatsPoints})";
                categoryOperations.Item3.text = $"Operations ({SkillPoints.OperationsPoints})";
                categorySocial.Item3.text = $"Social ({SkillPoints.SocialPoints})";
                categorySpecial.Item3.text = $"Special ({SkillPoints.SpecialPoints})";
            }

            void UpdateDetails()
            {
                detailsSkillName.text = selectedSkill?.Skill.Name;
                detailsSkillDescription.text = selectedSkill?.Skill.Description;
                detailsSkillLevel.text = $"Level {selectedSkill?.Skill.CurrentLevel} / {selectedSkill?.Skill.MaxLevel}";
            }

            void UpdateAppearance(List<SkillNode> nodeTree)
            {
                foreach (var node in nodeTree)
                {
                    node.SetText($"{node.Skill.Name} {node.Skill.CurrentLevel} / {node.Skill.MaxLevel}");
                    node.SetButtonColor(GetButtonColor(node.Skill, selectedSkill));
                }
            }

            void UpdateNodes()
            {
                UpdateAppearance(nodesStats);
                UpdateAppearance(nodesOperations);
                UpdateAppearance(nodesSocial);
                UpdateAppearance(nodesSpecial);
            }

            static Color GetButtonColor(Skill skill, SkillNode selectedNode)
            {
                Color color = Color.red;

                if (!skill.Name.Equals(selectedNode?.Skill.Name))
                {
                    color = colorLockedSkill;

                    if (skill.IsParentUnlocked())
                    {
                        color = colorUnlockedSkill;
                    }

                    if (skill.IsSkillMaxLevel())
                    {
                        color = colorMaxLevelSkill;
                    }
                }
                else
                {
                    color = colorLockedSkillSelected;

                    if (skill.IsParentUnlocked())
                    {
                        color = colorUnlockedSkillSelected;
                    }

                    if (skill.IsSkillMaxLevel())
                    {
                        color = colorMaxLevelSkillSelected;
                    }
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

                List<SkillNode> nodes = [];

                foreach (Skill skill in skillTree)
                {
                    int offset = GetDepth(skill) * 50;

                    var temp = UIFactory.RoundedButtonWithLabel(skill.Name, $"{skill.Name} {skill.CurrentLevel} / {skill.MaxLevel}", parent.transform, GetButtonColor(skill, selectedSkill), 30f, 30f, 14, colorText);
                    temp.Item1.GetComponent<Mask>().showMaskGraphic = false;
                    temp.Item2.GetComponent<RectTransform>().offsetMin = new Vector2(offset, 0);
                    temp.Item3.GetComponent<RectTransform>().offsetMin = new Vector2(-offset, 0);
                    SkillNode node = new SkillNode(skill, temp);
                    ButtonUtils.AddListener(temp.Item2, () =>
                    {
                        skillName.text = skill.Name;
                        skillDescription.text = skill.Description;
                        skillLevel.text = $"Level {skill.CurrentLevel} / {skill.MaxLevel}";
                        previousSkill = selectedSkill;
                        selectedSkill = node;
                        previousSkill?.SetButtonColor(GetButtonColor(previousSkill?.Skill, selectedSkill));
                        selectedSkill?.SetButtonColor(GetButtonColor(node.Skill, selectedSkill));
                    });
                    nodes.Add(node);
                }

                return nodes;
            }
        }
    }
}