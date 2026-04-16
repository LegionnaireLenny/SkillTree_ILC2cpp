using MelonLoader;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace SkillTree.Core.Utilities
{
    public class LocalizationManager
    {
        private static JsonElement _notifications;
        private static JsonElement _skills;

        public static ref JsonElement Notifications { get => ref _notifications; }
        public static ref JsonElement Skills { get => ref _skills; }
        public static Action OnLocaleUpdated;

        public static void Initialize()
        {
            ExtractLocaleData();
            LoadLocaleData();
            ConfigManager.OnLocaleChanged += LoadLocaleData;
        }

        public static string GetNotificationTitle(string root, string category)
        {
            return Notifications.GetProperty(root).GetProperty(category).GetProperty("Title").GetString();
        }

        public static string GetNotificationSubtitle(string root, string category)
        {
            return Notifications.GetProperty(root).GetProperty(category).GetProperty("Subtitle").GetString();
        }

        public static void LoadLocaleData()
        {
            string locale = ConfigManager.Locale?.GetValue() ?? "en_US";
            if (!Directory.Exists(Path.Combine(Core.LocalizationDirectory, locale)))
            {
                locale = "en_US";
            }
            LoadData(out Notifications, "Notifications.json", locale);
            LoadData(out Skills, "Skills.json", locale);
            OnLocaleUpdated?.Invoke();

            static void LoadData(out JsonElement root, string filename, string locale)
            {
                string path = Path.Combine(Core.LocalizationDirectory, locale, filename);
                if (!File.Exists(path))
                {
                    path = Path.Combine(Core.LocalizationDirectory, "en_US", filename);
                }

                using FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                using JsonDocument doc = JsonDocument.Parse(fs);
                root = doc.RootElement.Clone();

                //using var resourcePath = Assembly.GetExecutingAssembly().GetManifestResourceStream($"SkillTree.Core.LocalizedStrings.{locale}.{filename}");
                //using var resourceBackupPath = Assembly.GetExecutingAssembly().GetManifestResourceStream($"SkillTree.Core.LocalizedStrings.en_US.{filename}");
            }
        }

        public static void ExtractLocaleData()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                string[] resourcePaths = [.. assembly.GetManifestResourceNames().Where(r => r.Contains($"LocalizedStrings"))];

                foreach (var path in resourcePaths) 
                {
                    string[] temp = path.Split(".");
                    string dir = temp[temp.Length - 3];
                    string filename = temp[temp.Length - 2] + "." + temp[temp.Length - 1];
                    string destination = Path.Combine(Core.LocalizationDirectory, dir, filename);

                    if (!File.Exists(destination) || ConfigManager.OverwriteLocaleFiles.GetValue().Contains(dir))
                    {
                        Core.Instance.LoggerInstance.Msg($"Extracting translation file {path} to {destination}");
                        Directory.CreateDirectory(Path.Combine(Core.LocalizationDirectory, dir));
                        using var resource = assembly.GetManifestResourceStream(path);
                        using FileStream stream = new FileStream(destination, FileMode.Create, FileAccess.Write);
                        resource.CopyTo(stream);
                    }
                    else
                    {
                        Core.Instance.LoggerInstance.Warning($"Skipping extraction of translation file {path} to {destination}");
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Instance.LoggerInstance.Warning($"[SkillTree] Failed extracting translation data: {ex}");
            }
        }
    }
}
