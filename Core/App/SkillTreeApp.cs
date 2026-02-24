using UnityEngine;
using UnityEngine.UI;
using S1API.PhoneApp;
using S1API.UI;
using S1API.Utils;
using SkillTree.Core.FileManagement;
using System.Collections.Generic;

namespace SkillTree.Core.App
{
    public class SkillBranch
    {
        public SkillCategory Category;
        public List<SkillNode> skillNodes = new List<SkillNode>();
    }
    
    public class SkillNode : MonoBehaviour
    {
        public string Name;
        public string Description;
        public SkillCategory Category;
        public int MaxLevel;
        public int CurrentLevel;
        public List<SkillNode> parents;
        public List<SkillNode> children;
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
            float buttonWidth = 100f;
            float buttonHeight = 100f;
            Color backgroundColor = new Color(0.1f, 0.1f, 0.1f);
            Color buttonColor = new Color(0.2f, 0.2f, 0.2f);
            Color detailsColor = new Color(0.2f, 0.2f, 0.2f);
            
            var background = UIFactory.Panel("MainPanel", container.transform, backgroundColor, fullAnchor:true);
            var detailsPanel = UIFactory.Panel("DetailsPanel", container.transform, detailsColor, new Vector2(0.7f, 0f), new Vector2(1f, 1f));

            var categoryContainer = UIFactory.ButtonRow("SkillCategories", container.transform);
            var buttonCategoryStats = UIFactory.ButtonWithLabel("ButtonCategoryStats", "Stats", categoryContainer.transform, buttonColor, buttonWidth, buttonHeight);
            var buttonCategoryOperations = UIFactory.ButtonWithLabel("ButtonCategoryOperations", "Operations", categoryContainer.transform, buttonColor, buttonWidth, buttonHeight);
            var buttonCategorySocial = UIFactory.ButtonWithLabel("ButtonCategorySocial", "Social", categoryContainer.transform, buttonColor, buttonWidth, buttonHeight);
            var buttonCategorySpecial = UIFactory.ButtonWithLabel("ButtonCategorySpecial", "Special", categoryContainer.transform, buttonColor, buttonWidth, buttonHeight);

            categoryContainer.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 1f);
            categoryContainer.GetComponent<RectTransform>().anchorMax = new Vector2(0f, 1f);
            categoryContainer.GetComponent<RectTransform>().offsetMax = new Vector2(0f, 0f);
            categoryContainer.GetComponent<RectTransform>().offsetMin = new Vector2(0f, -buttonHeight);
            //categoryContainer.GetComponent<RectTransform>().;

            ButtonUtils.AddListener(buttonCategoryStats.Item2, () =>
            {
                Logger.Msg("Stats Category Selected!");
            });
            ButtonUtils.AddListener(buttonCategoryOperations.Item2, () =>
            {
                Logger.Msg("Operations Category selected!");
            });
            ButtonUtils.AddListener(buttonCategorySocial.Item2, () =>
            {
                Logger.Msg("Social Category Selected!");
            });
            ButtonUtils.AddListener(buttonCategorySpecial.Item2, () =>
            {
                Logger.Msg("Special Category Selected!");
            });
        }
    }
}