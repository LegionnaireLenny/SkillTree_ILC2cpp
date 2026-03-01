using MelonLoader;
using System;
using System.Collections.Generic;

namespace SkillTree.Core.Skills
{
    public class Skill
    {
        public string Name;
        public string Description;
        public SkillCategory Category;
        public int MaxLevel;
        public int CurrentLevel;
        public Skill Parent;
        public List<Skill> Children;
        public Action[] OnLevelUp;

        public Skill(
            string name,
            string description,
            SkillCategory category,
            int maxLevel,
            int currentLevel,
            Skill parent,
            List<Skill> children,
            Action[] onLevelUp = null)
        {
            Name = name;
            Description = description;
            Category = category;
            MaxLevel = maxLevel;
            CurrentLevel = currentLevel;
            Parent = parent;
            Children = children;
            OnLevelUp = onLevelUp;
        }

        public bool IsParentUnlocked()
        {
            if (Parent == null)
            {
                return true;
            }

            if (Parent.CurrentLevel < Parent.MaxLevel)
            {
                return false;
            }

            return true;
        }

        public bool IsSkillMaxLevel()
        {
            return CurrentLevel >= MaxLevel;
        }

        private bool IsLevelUpValid()
        {
            if (!IsParentUnlocked())
            {
                MelonLogger.Msg($"Parent skill {Parent.Name} is not unlocked");
                return false;
            }

            if (!SkillPoints.ArePointsAvailable(Category))
            {
                MelonLogger.Msg($"Not enough {Category} points");
                return false;
            }
                
            if (IsSkillMaxLevel())
            {
                MelonLogger.Msg($"{Name} is already max level");
                return false;
            }

            return true;
        }

        public bool IncreaseSkillLevel()
        {
            bool isValid = IsLevelUpValid();
            if (isValid)
            {
                CurrentLevel++;
                SkillPoints.ConsumeSkillPoint(Category);
                ApplySkillEffect();
            }
            return isValid;
        }

        public void ApplySkillEffect()
        {
            if (OnLevelUp == null)
            {
                return;
            }

            foreach (var action in OnLevelUp)
            {
                action();
            }
        }

        public override string ToString()
        {
            string children = "";
            foreach (var child in Children)
            {
                children += $"{child.Name}, ";
            }

            return $"{Name} | {Description} | {Category} | {MaxLevel} | {CurrentLevel} | {Parent?.Name ?? "None"} | {children}";
        }
    }
}
