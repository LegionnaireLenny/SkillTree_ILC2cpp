using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.Vision;
using SkillTree.Core.Serialization;
using SkillTree.Core.Skills;
using SkillTree.Core.Utilities;

namespace SkillTree.Core.Effects
{
    public class Ghost
    {
        private static VisibilityAttribute visibilityAttribute;
        private static readonly string EffectName = "ghost";

        public static void ApplyToPlayer()
        {
            if (SkillTreeData.Ghost.CurrentLevel != 0 && Player.Local.Visibility.GetAttribute(EffectName) == null)
            {
                visibilityAttribute = new VisibilityAttribute(EffectName, 0f, SkillModifiers.GetVisbilityMultiplier(), -1);
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
