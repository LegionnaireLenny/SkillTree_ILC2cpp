using SkillTree.Core.Serialization;
using SkillTree.Core.Utilities;
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
        public int CurrentLevel = 0;
        public Skill Parent;
        public List<Skill> Children = [];
        public Action[] OnLevelUp;

        public Skill(
            string name,
            SkillCategory category,
            int maxLevel,
            Skill parent,
            Action[] onLevelUp = null)
        {
            Name = name;
            Description = LocalizationManager.Skills.GetProperty(Name).ToString();
            Category = category;
            MaxLevel = maxLevel;
            Parent = parent;
            OnLevelUp = onLevelUp;
            LocalizationManager.OnLocaleUpdated += UpdateDescription;
        }

        public void UpdateDescription()
        {
            Description = LocalizationManager.Skills.GetProperty(Name).ToString();
        }

        public bool IsParentNullOrMaxLevel()
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

        public bool IsMaxLevel()
        {
            return CurrentLevel == MaxLevel;
        }

        public bool IsOverLeveled()
        {
            return CurrentLevel > MaxLevel;
        }

        private bool IsLevelUpValid()
        {
            if (!IsParentNullOrMaxLevel())
            {
                LogManager.LogMessage($"Parent skill {Parent.Name} is not unlocked", LogLevel.Info);
                return false;
            }

            if (!SkillPoints.ArePointsAvailable(Category))
            {
                LogManager.LogMessage($"Not enough {Category} points", LogLevel.Info);
                return false;
            }

            if (IsMaxLevel() || IsOverLeveled())
            {
                LogManager.LogMessage($"{Name} is already max level", LogLevel.Info);
                return false;
            }

            return true;
        }

        public int GetPointsToLevelBranch()
        {
            if (IsParentNullOrMaxLevel())
            {
                return MaxLevel - CurrentLevel;
            }

            return (MaxLevel - CurrentLevel) + Parent.GetPointsToLevelBranch();
        }

        public void UnlockParents()
        {
            if (IsParentNullOrMaxLevel())
            {
                LevelToMax();
            }
            else
            {
                Parent.UnlockParents();
                LevelToMax();
            }
        }

        public bool LevelAndUnlockParents()
        {
            if (IsParentNullOrMaxLevel())
            {
                LogManager.LogMessage($"Parent: {Parent?.Name} {Parent?.CurrentLevel}/{Parent?.MaxLevel} | Current: {Name} {CurrentLevel}/{MaxLevel}", LogLevel.Debug);
                IncreaseLevel();
                return true;
            }

            int pointsNeeded = Parent.GetPointsToLevelBranch() + 1;
            int pointsAvailable = SkillPoints.GetPointsAvailable(Category);

            LogManager.LogMessage($"{Category} points | Needed: {pointsNeeded} | Available: {pointsAvailable}", LogLevel.Debug);
            if (pointsNeeded <= pointsAvailable)
            {
                Parent.UnlockParents();
                IncreaseLevel();
                return true;
            }
            else
            {
                LogManager.LogMessage($"Not enough {Category} points to level branch. Needed: {pointsNeeded} | Available: {pointsAvailable}", LogLevel.Info);
                return false;
            }
        }

        public bool IncreaseLevel()
        {
            bool isValid = IsLevelUpValid();
            if (isValid)
            {
                CurrentLevel++;
                SkillPoints.ConsumeSkillPoints(Category, 1);
                ApplySkillEffect();
            }
            return isValid;
        }

        public void LevelToMax()
        {
            LogManager.LogMessage($"Leveling {Name} to max.", LogLevel.Debug);
            for (int i = CurrentLevel; i < MaxLevel; i++)
            {
                IncreaseLevel();
            }
        }

        public void RemoveAllLevels()
        {
            if (CurrentLevel > 0)
            {
                LogManager.LogMessage($"Setting {Name} to level 0. Refunding {CurrentLevel} {Category} points.", LogLevel.Warning);
                SkillPoints.ConsumeSkillPoints(Category, -CurrentLevel);
                CurrentLevel = 0;
            }
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

        public void FixOverleveledSkill()
        {
            if (IsOverLeveled())
            {
                int difference = CurrentLevel - MaxLevel;
                LogManager.LogMessage($"{Name} is overleveled {CurrentLevel}/{MaxLevel}. Reducing level and refunding {difference} {Category} points.", LogLevel.Warning);
                CurrentLevel -= difference;
                SkillPoints.ConsumeSkillPoints(Category, -difference);
            }
        }

        public void FixSkills()
        {
            FixOverleveledSkill();
            if (!IsMaxLevel())
            {
                LogManager.LogMessage($"{Name} is not max level. Validating children.", LogLevel.Debug);
                foreach (Skill child in Children)
                {
                    child.RemoveAllLevels();
                    child.FixSkills();
                }
            }
            else
            {
                LogManager.LogMessage($"{Name} is max level. Validating children.", LogLevel.Debug);
                foreach (Skill child in Children)
                {
                    child.FixSkills();
                }
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
