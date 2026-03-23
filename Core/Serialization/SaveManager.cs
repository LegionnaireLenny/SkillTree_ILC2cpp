using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Persistence;
using MelonLoader;
using MelonLoader.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;


namespace SkillTree.Core.Serialization
{
    public static class SaveManager
    {
        public static string GetCurrentSaveID()
        {
            string fullPath = Singleton<LoadManager>.Instance.LoadedGameFolderPath;

            if (string.IsNullOrEmpty(fullPath))
                return "DefaultPlayer";

            return Path.GetFileName(fullPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        }

        public static string GetSaveFilePath()
        {
            return Path.Combine(MelonEnvironment.UserDataDirectory, $"SkillTree_{GetCurrentSaveID()}.json");
        }

        private static void BuildSaveData(Dictionary<string, string> skillData, List<Dictionary<string, string>> sources)
        {
            foreach (Dictionary<string, string> dictionary in sources)
            {
                foreach (var item in dictionary)
                {
                    skillData.Add(item.Key, item.Value);
                }
            }
        }

        public static void DeleteFile()
        {
            string path = GetSaveFilePath();
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public static Dictionary<string, string> GetSaveDataProperties<T>() where T : new()
        {
            Dictionary<string, string> data = [];
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                data[property.Name] = property.GetValue(new T()).ToString();
            }

            return data;
        }

        public static void SaveFile()
        {
            Dictionary<string, string> skillData = [];

            BuildSaveData(skillData, [GetSaveDataProperties<SkillPoints>(), SkillTreeData.GetSaveData(), GetSaveDataProperties<KillCounts>(), GetSaveDataProperties<Cooldowns>()]);

            string path = GetSaveFilePath();
            string json = JsonConvert.SerializeObject(skillData, Formatting.Indented);
            File.WriteAllText(path, json);
        }

        public static void LoadFile()
        {
            try
            {
                string path = GetSaveFilePath();

                if (!File.Exists(path))
                {
                    MelonLogger.Warning($"[SkillTree] Skill data file not found: {path}");
                    LoadDefaultValues();
                }
                else
                {
                    using FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                    using JsonDocument doc = JsonDocument.Parse(fs);
                    JsonElement root = doc.RootElement.Clone();

                    SkillPoints.LoadFromFile(root);
                    SkillTreeData.LoadFromFile(root);
                    KillCounts.LoadFromFile(root);
                    Cooldowns.LoadFromFile(root);
                }
            }
            catch (KeyNotFoundException ex)
            {
                MelonLogger.Warning(ex);
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"Error loading save data {ex}");
                DeleteFile();
                LoadDefaultValues();
            }
            finally
            {
                SkillTreeData.ValidateSkillTrees();
            }
        }

        public static void LoadDefaultValues()
        {
            SkillPoints.LoadDefaultValues();
            SkillTreeData.LoadDefaultValues();
            KillCounts.LoadDefaultValues();
            Cooldowns.LoadDefaultValues();
        }
    }
}
