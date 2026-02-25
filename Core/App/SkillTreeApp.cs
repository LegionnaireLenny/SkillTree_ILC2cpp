using S1API.PhoneApp;
using S1API.UI;
using S1API.Utils;
using SkillTree.Core.FileManagement;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SkillTree.Core.App
{
    public class SkillNode : MonoBehaviour
    {
        public Skill Skill;
        public Button Button;

        public SkillNode(Skill skill, Button button)
        {
            Skill = skill;
            Button = button;
        }
    }


    public class SkillTreeApp : PhoneApp
    {
        protected override string AppName => "SkillTreeApp";
        protected override string AppTitle => "Skills";
        protected override string IconLabel => "Skills";
        protected override string IconFileName => "SkillTree (Forked).png";

        protected override void OnCreated()
        {
            base.OnCreated();
        }

        protected override void OnCreatedUI(GameObject container)
        {
            // TODO: width is bugged, it sets the height. new S1API version hasn't released with fix
            float buttonWidth = 50f;
            float buttonHeight = 100f;
            Color backgroundColor = new Color(0.1f, 0.1f, 0.1f);
            Color buttonColor = new Color(0.2f, 0.2f, 0.2f);
            Color detailsColor = new Color(0.2f, 0.2f, 0.2f);

            var appBackground = UIFactory.Panel("MainPanel", container.transform, backgroundColor, fullAnchor: true);

            var topBarContainer = UIFactory.Panel("topBarContainer", appBackground.transform, backgroundColor, anchorMin: new Vector2(0.084f, 1.1f), anchorMax: new Vector2(0.575f, 0.85f));
            var topBar = UIFactory.TopBar("topBar", topBarContainer.transform, "Skill Trees", 0.82f, 15, 15, 5, 5);

            var categoryContainer = UIFactory.ButtonRow("SkillCategories", topBar.transform);
            var buttonCategoryStats = UIFactory.ButtonWithLabel("ButtonCategoryStats", "Stats", categoryContainer.transform, buttonColor, buttonWidth, buttonHeight);
            var buttonCategoryOperations = UIFactory.ButtonWithLabel("ButtonCategoryOperations", "Operations", categoryContainer.transform, buttonColor, buttonWidth, buttonHeight);
            var buttonCategorySocial = UIFactory.ButtonWithLabel("ButtonCategorySocial", "Social", categoryContainer.transform, buttonColor, buttonWidth, buttonHeight);
            var buttonCategorySpecial = UIFactory.ButtonWithLabel("ButtonCategorySpecial", "Special", categoryContainer.transform, buttonColor, buttonWidth, buttonHeight);

            var statsTreeContainer = UIFactory.Panel("skillTreeContainer", appBackground.transform, Color.black, anchorMin: new Vector2(0.04f, 0.08f), anchorMax: new Vector2(0.618f, 0.8f));
            UIFactory.VerticalLayoutOnGO(statsTreeContainer);
            var operationsTreeContainer = UIFactory.Panel("skillTreeContainer", appBackground.transform, Color.black, anchorMin: new Vector2(0.04f, 0.08f), anchorMax: new Vector2(0.618f, 0.8f));
            UIFactory.VerticalLayoutOnGO(operationsTreeContainer);
            var socialTreeContainer = UIFactory.Panel("skillTreeContainer", appBackground.transform, Color.black, anchorMin: new Vector2(0.04f, 0.08f), anchorMax: new Vector2(0.618f, 0.8f));
            UIFactory.VerticalLayoutOnGO(socialTreeContainer);
            var specialTreeContainer = UIFactory.Panel("skillTreeContainer", appBackground.transform, Color.black, anchorMin: new Vector2(0.04f, 0.08f), anchorMax: new Vector2(0.618f, 0.8f));
            UIFactory.VerticalLayoutOnGO(specialTreeContainer);

            var detailsPanel = UIFactory.Panel("DetailsPanel", appBackground.transform, detailsColor, new Vector2(0.7f, 0.085f), new Vector2(0.958f, 0.915f));
            UIFactory.VerticalLayoutOnGO(detailsPanel);
            var skillName = UIFactory.Text("SkillName", "", detailsPanel.transform, anchor: TextAnchor.MiddleCenter);
            var skillDescription = UIFactory.Text("SkillDescription", "", detailsPanel.transform, anchor: TextAnchor.MiddleCenter);
            var skillLevel = UIFactory.Text("SkillLevel", "", detailsPanel.transform, anchor: TextAnchor.MiddleCenter);
            //var levelSkillButton = UIFactory.RoundedButtonWithLabel("LevelSkillButton", "Level Skill", detailsPanel.transform, Color.blue, 100f, 50f, 10, Color.gray).Item2;

            List<SkillNode> statsNodes = CreateNodeTree(SkillTree_Test.StatsTree, statsTreeContainer, skillName, skillDescription, skillLevel);
            List<SkillNode> operationsNodes = CreateNodeTree(SkillTree_Test.OperationsTree, operationsTreeContainer, skillName, skillDescription, skillLevel);
            List<SkillNode> socialNodes = CreateNodeTree(SkillTree_Test.SocialTree, socialTreeContainer, skillName, skillDescription, skillLevel);
            List<SkillNode> specialNodes = CreateNodeTree(SkillTree_Test.SpecialTree, specialTreeContainer, skillName, skillDescription, skillLevel);

            operationsTreeContainer.SetActive(false);
            socialTreeContainer.SetActive(false);
            specialTreeContainer.SetActive(false);

            ButtonUtils.AddListener(buttonCategoryStats.Item2, () =>
            {
                Logger.Msg("Stats Category Selected!");
                SkillTree_Test.PrintTree(SkillTree_Test.StatsTree);
                statsTreeContainer.SetActive(true);
                operationsTreeContainer.SetActive(false);
                socialTreeContainer.SetActive(false);
                specialTreeContainer.SetActive(false);
            });
            ButtonUtils.AddListener(buttonCategoryOperations.Item2, () =>
            {
                Logger.Msg("Operations Category selected!");
                SkillTree_Test.PrintTree(SkillTree_Test.OperationsTree);
                statsTreeContainer.SetActive(false);
                operationsTreeContainer.SetActive(true);
                socialTreeContainer.SetActive(false);
                specialTreeContainer.SetActive(false);
            });
            ButtonUtils.AddListener(buttonCategorySocial.Item2, () =>
            {
                Logger.Msg("Social Category Selected!");
                SkillTree_Test.PrintTree(SkillTree_Test.SocialTree);
                statsTreeContainer.SetActive(false);
                operationsTreeContainer.SetActive(false);
                socialTreeContainer.SetActive(true);
                specialTreeContainer.SetActive(false);
            });
            ButtonUtils.AddListener(buttonCategorySpecial.Item2, () =>
            {
                Logger.Msg("Special Category Selected!");
                SkillTree_Test.PrintTree(SkillTree_Test.SpecialTree);
                statsTreeContainer.SetActive(false);
                operationsTreeContainer.SetActive(false);
                socialTreeContainer.SetActive(false);
                specialTreeContainer.SetActive(true);
            });
        }

        private static List<SkillNode> CreateNodeTree(HashSet<Skill> skillTree, GameObject parent, Text skillName, Text skillDescription, Text skillLevel)
        {
            List<SkillNode> nodes = new List<SkillNode>();

            foreach (var skill in skillTree)
            {
                SkillNode node = new SkillNode(skill, UIFactory.RoundedButtonWithLabel(skill.Name, skill.Name, parent.transform, Color.grey, 30f, 30f, 10, Color.yellow).Item2);
                ButtonUtils.AddListener(node.Button, () =>
                {
                    Logger.Msg(node.Skill.ToString());
                    skillName.text = skill.Name;
                    skillDescription.text = skill.Description;
                    skillLevel.text = $"Level {skill.CurrentLevel} / {skill.MaxLevel}";
                });
                nodes.Add(node);
            }

            return nodes;
        }
    }
}