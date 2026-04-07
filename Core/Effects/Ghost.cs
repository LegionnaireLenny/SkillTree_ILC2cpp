using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.Vision;
using MelonLoader;
using SkillTree.Core.Serialization;
using SkillTree.Core.Skills;
using SkillTree.Core.Utilities;

namespace SkillTree.Core.Effects
{
    public class Ghost
    {
        private static VisibilityAttribute visibilityAttribute;
        public static readonly string Name = "ghost";

        public static void ApplyToPlayer()
        {
            if (SkillTreeData.Ghost.CurrentLevel != 0 && Player.Local.Visibility.GetAttribute(Name) == null)
            {
                visibilityAttribute = new VisibilityAttribute(Name, 0f, SkillModifiers.GetVisbilityMultiplier(), -1);
                LogManager.LogMessage($"Ghost effect applied", LogLevel.Debug);
            }
        }

        public static void ClearFromPlayer()
        {
            visibilityAttribute?.Delete();
            LogManager.LogMessage($"Ghost effect removed", LogLevel.Debug);
        }
    }
}
