using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Persistence;
using Il2CppScheduleOne.Persistence.Datas;
using MelonLoader;
using MelonLoader.Utils;
using Newtonsoft.Json;
using SkillTree.Core.Patches.Special;
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

        private static void BuildSaveData(ref Dictionary<string, int> skillData, List<Dictionary<string, int>> sources)
        {
            foreach (Dictionary<string, int> dictionary in sources)
            {
                foreach(var item in dictionary)
                {
                    skillData.Add(item.Key, item.Value);
                }
            }
        }

        public static void DeleteFile()
        {
            if (File.Exists(GetSaveFilePath()))
            {
                File.Delete(GetSaveFilePath());
            }
        }

        public static void SaveFile()
        {
            Dictionary<string, int> skillData = [];

            BuildSaveData(ref skillData, [SkillPoints.GetSaveData(), SkillTreeData.GetSaveData(), NPCPatches.GetSaveData()]);

            string path = GetSaveFilePath();
            string json = JsonConvert.SerializeObject(skillData, Formatting.Indented);
            File.WriteAllText(path, json);
        }

        public static void SaveDefaultFile()
        {
            Dictionary<string, int> skillData = [];

            BuildSaveData(ref skillData, [SkillPoints.GetDefaultSaveData(), SkillTreeData.GetDefaultSaveData(), NPCPatches.GetDefaultSaveData()]);

            string path = GetSaveFilePath();
            string json = JsonConvert.SerializeObject(skillData, Formatting.Indented);
            File.WriteAllText(path, json);
        }

        public static void LoadFile()
        {
            string path = GetSaveFilePath();

            if (!File.Exists(path))
            {
                MelonLogger.Msg($"[SkillTree] Skill data file not found: {path}");
                LoadDefaultValues();
                return;
            }

            try
            {
                using FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                using JsonDocument doc = JsonDocument.Parse(fs);
                JsonElement root = doc.RootElement.Clone();

                SkillPoints.LoadFromFile(root);
                SkillTreeData.LoadFromFile(root);
                NPCPatches.LoadFromFile(root);
            }
            catch (Exception ex) 
            {
                MelonLogger.Warning($"Error loading save data {ex}");
                DeleteFile();
                LoadDefaultValues();
            }
        }

        public static void LoadDefaultValues()
        {
            SkillPoints.LoadDefaultValues();
            SkillTreeData.LoadDefaultValues();
            NPCPatches.LoadDefaultValues();
        }
    }
}
