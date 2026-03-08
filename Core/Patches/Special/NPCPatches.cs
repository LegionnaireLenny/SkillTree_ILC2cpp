using HarmonyLib;
using Il2CppScheduleOne.Cartel;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Map;
using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.Police;
using MelonLoader;
using S1API.Utils;
using SkillTree.Core.Skills;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using UnityEngine;
using UnityEngine.UI;

namespace SkillTree.Core.Patches.Special
{
    [HarmonyPatch]
    public class NPCPatches
    {
        public static int PoliceKilled { get; private set; } = 0;
        public static int CartelKilled { get; private set; } = 0;
        public static int CivilianKilled { get; private set; } = 0;

        public static readonly Dictionary<string, CustomPOI> CustomPOIManager = [];

        public class CustomPOI
        {
            public string IconPath { get; set; }
            public bool IsPolice {  get; set; }
            public NPCPoI POI { get; set; }

            public void SetNPC(NPC npc)
            {
                POI.NPC = npc;
                SetSprite();
            }

            public void SetSprite()
            {
                if (POI.IconContainer != null && POI.NPC != null)
                {
                    POI.IconContainer.Find("Outline/Icon").GetComponent<Image>().sprite = ImageUtils.LoadImage(IconPath);
                    POI.IconContainer.Find("Outline/Icon").GetComponent<RectTransform>().offsetMin = Vector2.zero;
                    POI.IconContainer.Find("Outline/Icon").GetComponent<RectTransform>().offsetMax = Vector2.zero;
                }
            }

            public void SetSprite(string spritePath)
            {
                if (POI.IconContainer != null && POI.NPC != null)
                {
                    POI.IconContainer.Find("Outline/Icon").GetComponent<Image>().sprite = ImageUtils.LoadImage(spritePath);
                    POI.IconContainer.Find("Outline/Icon").GetComponent<RectTransform>().offsetMin = Vector2.zero;
                    POI.IconContainer.Find("Outline/Icon").GetComponent<RectTransform>().offsetMax = Vector2.zero;
                }
            }

            public void SetSprite(Sprite sprite)
            {
                if (POI.IconContainer != null && POI.NPC != null)
                {
                    POI.IconContainer.Find("Outline/Icon").GetComponent<Image>().sprite = sprite;
                    POI.IconContainer.Find("Outline/Icon").GetComponent<RectTransform>().offsetMin = Vector2.zero;
                    POI.IconContainer.Find("Outline/Icon").GetComponent<RectTransform>().offsetMax = Vector2.zero;
                }
            }

            public void UpdateVisibility()
            {
                SetVisibility(POI.NPC.IsCurrentlySightable());
            }

            public void SetVisibility(bool isVisible)
            {
                if (IsPolice)
                {
                    isVisible = isVisible && (SkillTreeData.Informant.CurrentLevel == 1);
                }
                else
                {
                    isVisible = isVisible && (SkillTreeData.Spymaster.CurrentLevel == 1);
                }

                POI.enabled = isVisible;
                if (isVisible)
                {
                    SetSprite();
                }
            }
        }

        public static void UpdateVisibility()
        {
            foreach (var item in CustomPOIManager)
            {
                item.Value.UpdateVisibility();
            }
        }

        private static IEnumerator SetupCustomPOI(NPC instance, string description, string icon, bool isPolice)
        {
            yield return new WaitForSeconds(5f);
            if (!CustomPOIManager.ContainsKey(instance.ID))
            {
                CustomPOI customPOI = new CustomPOI
                {
                    IconPath = icon,
                    IsPolice = isPolice,
                    POI = Object.Instantiate(NetworkSingleton<NPCManager>.Instance.NPCPoIPrefab, instance.transform)
                };
                customPOI.POI.SetMainText($"{instance.fullName}\n{(description)}");
                customPOI.POI.SetNPC(instance);
                customPOI.POI.transform.localPosition = Vector3.zero;
                customPOI.SetVisibility(instance.IsCurrentlySightable());
                CustomPOIManager[instance.ID] = customPOI;
            }
        }

        [HarmonyPatch(typeof(NPC), "SetVisible")]
        [HarmonyPostfix]
        public static void Patch_NPC_SetVisible(NPC __instance)
        {
            if (CustomPOIManager.ContainsKey(__instance.ID))
            {
                CustomPOIManager[__instance.ID].SetVisibility(__instance.IsCurrentlySightable());
            }
        }

        [HarmonyPatch(typeof(PoliceOfficer), "Start")]
        [HarmonyPostfix]
        public static void Patch_PoliceOfficer_Start(PoliceOfficer __instance)
        {
            MelonCoroutines.Start(SetupCustomPOI(__instance, "Police Officer", Core.IconPolice, true));
        }

        [HarmonyPatch(typeof(CartelDealer), "Start")]
        [HarmonyPostfix]
        public static void Patch_CartelDealer_Start(CartelDealer __instance)
        {
            MelonCoroutines.Start(SetupCustomPOI(__instance, "Cartel Dealer", Core.IconBenzieDealer, false));
        }

        [HarmonyPatch(typeof(CartelGoon), "Start")]
        [HarmonyPostfix]
        public static void Patch_CartelGoon_Start(CartelGoon __instance)
        {
            MelonCoroutines.Start(SetupCustomPOI(__instance, "Cartel Goon", Core.IconBenzieGoon, false));
        }

        [HarmonyPatch(typeof(NPC), "OnDie")]
        [HarmonyPostfix]
        public static void Patch_NpcOnDie(NPC __instance)
        {
            if (CustomPOIManager.ContainsKey(__instance.ID))
            {
                CustomPOIManager[__instance.ID].SetVisibility(__instance.IsCurrentlySightable());
            }

            // OnDie is called twice for police, so PoliceKilled will increase by two every time a cop is killed
            if (__instance.TryCast<PoliceOfficer>() != null)
            {
                PoliceKilled++;
            }
            else if (__instance.TryCast<CartelGoon>() != null)
            {
                CartelKilled++;
            }
            else if (__instance.TryCast<CartelDealer>() != null)
            {
                CartelKilled++;
            }
            else
            {
                CivilianKilled++;
            }
        }

        public static void Reset()
        {
            CustomPOIManager.Clear();
        }

        public static Dictionary<string, int> GetSaveData()
        {
            Dictionary<string, int> skillData = [];

            var properties = typeof(NPCPatches).GetProperties();

            foreach (var property in properties)
            {
                skillData[property.Name] = (int)property.GetValue(new NPCPatches());
            }

            return skillData;
        }

        public static Dictionary<string, int> GetDefaultSaveData()
        {
            Dictionary<string, int> skillData = [];

            var properties = typeof(NPCPatches).GetProperties();

            foreach (var property in properties)
            {
                skillData[property.Name] = 0;
            }

            return skillData;
        }

        public static void LoadFromFile(JsonElement data)
        {
            var properties = typeof(NPCPatches).GetProperties();

            foreach (var property in properties)
            {
                try
                {
                    property.SetValue(new NPCPatches(), data.GetProperty(property.Name).GetInt32());
                }
                catch (KeyNotFoundException e)
                {
                    throw new KeyNotFoundException($"Failed to load kill counts from file {e}");
                }
            }
        }

        public static void LoadDefaultValues()
        {
            foreach (var property in typeof(NPCPatches).GetProperties())
            {
                property.SetValue(new NPCPatches(), 0);
            }
        }
    }
}
