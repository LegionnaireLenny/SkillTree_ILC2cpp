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
            return CurrentLevel == MaxLevel;
        }

        public bool IsSkillOverLeveled()
        {
            return CurrentLevel > MaxLevel;
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
                
            if (IsSkillMaxLevel() || IsSkillOverLeveled())
            {
                MelonLogger.Msg($"{Name} is already max level");
                return false;
            }

            return true;
        }

        public int GetPointsToLevelParents()
        {
            if (IsParentUnlocked())
            {
                return MaxLevel - CurrentLevel;
            }
            
            return (MaxLevel - CurrentLevel) + Parent.GetPointsToLevelParents();
        }

        public void UnlockParents()
        {
            if (IsParentUnlocked())
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
            int pointsNeeded = GetPointsToLevelParents() + 1;
            int pointsAvailable = SkillPoints.GetPointsAvailable(Category);

            //MelonLogger.Msg($"{Category} points | Needed: {pointsNeeded} | Available: {pointsAvailable}");
            if (pointsNeeded <= pointsAvailable)
            {
                Parent?.UnlockParents();
                IncreaseSkillLevel();
                //MelonLogger.Msg($"Unlocked parents and leveled {Name}");
                return true;
            }
            else
            {
                MelonLogger.Msg($"Not enough {Category} points to level branch. Needed: {pointsNeeded} | Available: {pointsAvailable}");
                return false;
            }
        }

        public bool IncreaseSkillLevel()
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
            for (int i = 0; i <= MaxLevel - CurrentLevel; i++)
            {
                IncreaseSkillLevel();
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
            if (IsSkillOverLeveled())
            {
                int difference = CurrentLevel - MaxLevel;
                MelonLogger.Warning($"{Name} is overleveled {CurrentLevel}/{MaxLevel}. Reducing level and refuding {difference} {Category} points.");
                CurrentLevel -= difference;
                SkillPoints.ConsumeSkillPoints(Category, -difference);
            }
        }

        public void FixSkills()
        {
            FixOverleveledSkill();
            if (!IsSkillMaxLevel())
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
