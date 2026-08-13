namespace SkillTree.Core.Utilities
{
    public class Formatter
    {
        public static string FormatAsPercentage(float value)
        {
            return $"{(int)(value * 100)}%";
        }
    }
}
