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
                MelonLogger.Msg($"Parent skill {Parent.Name} is not unlocked");
                return false;
            }

            if (!SkillPoints.ArePointsAvailable(Category))
            {
                MelonLogger.Msg($"Not enough {Category} points");
                return false;
            }
                
            if (IsMaxLevel() || IsOverLeveled())
            {
                MelonLogger.Msg($"{Name} is already max level");
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
                //MelonLogger.Msg($"Parent: {Parent?.Name} {Parent?.CurrentLevel}/{Parent?.MaxLevel} | Current: {Name} {CurrentLevel}/{MaxLevel}");
                IncreaseLevel();
                return true;
            }

            int pointsNeeded = Parent.GetPointsToLevelBranch() + 1;
            int pointsAvailable = SkillPoints.GetPointsAvailable(Category);

            //MelonLogger.Msg($"{Category} points | Needed: {pointsNeeded} | Available: {pointsAvailable}");
            if (pointsNeeded <= pointsAvailable)
            {
                Parent.UnlockParents();
                IncreaseLevel();
                return true;
            }
            else
            {
                MelonLogger.Msg($"Not enough {Category} points to level branch. Needed: {pointsNeeded} | Available: {pointsAvailable}");
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
            //MelonLogger.Msg($"Leveling {Name} to max.");
            for (int i = CurrentLevel; i < MaxLevel; i++)
            {
                IncreaseLevel();
            }
        }

        public void RemoveAllLevels()
        {
            if (CurrentLevel > 0)
            {
                MelonLogger.Warning($"Setting {Name} to level 0. Refunding {CurrentLevel} {Category} points.");
                SkillPoints.ConsumeSkillPoints(Category, -CurrentLevel);
                CurrentLevel = 0;
            }
        }

        public void ApplySkillEffect()
        {
            if (OnLevelUp == null || CurrentLevel == 0)
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
                MelonLogger.Warning($"{Name} is overleveled {CurrentLevel}/{MaxLevel}. Reducing level and refunding {difference} {Category} points.");
                CurrentLevel -= difference;
                SkillPoints.ConsumeSkillPoints(Category, -difference);
            }
        }

        public void FixSkills()
        {
            FixOverleveledSkill();
            if (!IsMaxLevel())
            {
                //MelonLogger.Msg($"{Name} is not max level. Validating children.");
                foreach (Skill child in Children)
                {
                    child.RemoveAllLevels();
                    child.FixSkills();
                }
            }
            else
            {
                //MelonLogger.Msg($"{Name} is max level. Validating children.");
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
