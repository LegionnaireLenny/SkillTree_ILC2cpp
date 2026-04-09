using MelonLoader;

namespace SkillTree.Core.Utilities
{
    public class LogManager
    {
        public static void LogMessage(string txt, LogLevel level)
        {
            if (level > ConfigManager.LoggingLevel.GetValue()) return;

            switch (level)
            {
                case LogLevel.DebugVerbose:
                case LogLevel.Debug:
                case LogLevel.Info:
                    MelonLogger.Msg(txt);
                    break;
                case LogLevel.Warning:
                    MelonLogger.Warning(txt);
                    break;
            }
        }

        public static void LogMessage(object obj, LogLevel level)
        {
            LogMessage(obj.ToString(), level);
        }
    }
}
