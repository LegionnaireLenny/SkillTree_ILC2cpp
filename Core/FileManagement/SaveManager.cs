using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Persistence;
using Il2CppScheduleOne.Persistence.Datas;
using MelonLoader;
using MelonLoader.Utils;
using Newtonsoft.Json;
using SkillTree.Core.Skills;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;


namespace SkillTree.Core.FileManagement
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

        public static void Save()
        {
            Dictionary<string, int> skillData = [];

            foreach (var item in SkillPoints.GetSaveData())
            {
                skillData.Add(item.Key, item.Value);
            }

            foreach (var item in SkillTreeData.GetSaveData())
            {
                skillData.Add(item.Key, item.Value);
            }

            string path = GetSaveFilePath();
            string json = JsonConvert.SerializeObject(skillData, Formatting.Indented);
            File.WriteAllText(path, json);
        }

        public static void SaveDefault()
        {
            Dictionary<string, int> skillData = [];

            foreach (var item in SkillPoints.GetDefaultSaveData())
            {
                skillData.Add(item.Key, item.Value);
            }

            foreach (var item in SkillTreeData.GetDefaultSaveData())
            {
                skillData.Add(item.Key, item.Value);
            }

            string path = GetSaveFilePath();
            string json = JsonConvert.SerializeObject(skillData, Formatting.Indented);
            File.WriteAllText(path, json);
        }

        public static void Load()
        {
            string path = GetSaveFilePath();

            if (!File.Exists(path))
            {
                MelonLogger.Msg($"[SkillTree] Skill data file not found: {path}");
                SaveDefault();
            }

            using FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            using JsonDocument doc = JsonDocument.Parse(fs);
            JsonElement root = doc.RootElement.Clone();

            SkillPoints.LoadFromFile(root);
            SkillTreeData.LoadFromFile(root);
        }
    }
}
