using HarmonyLib;
using Il2CppScheduleOne.Cartel;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Map;
using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.Police;
using MelonLoader;
using SkillTree.Core.Serialization;
using SkillTree.Core.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static SkillTree.Core.Serialization.KillCounts;

namespace SkillTree.Core.Patches.Hustler
{
    [HarmonyPatch]
    public class NPCPatches
    {
        public static readonly Dictionary<string, CustomPOI> CustomPOIManager = [];

        public class CustomPOI
        {
            public string IconName { get; set; }
            public bool IsPolice { get; set; }
            public NPCPoI POI { get; set; }

            public void SetNPC(NPC npc)
            {
                POI.NPC = npc;
                SetSprite();
            }

            public void SetSprite()
            {
                if (POI?.IconContainer != null && POI?.NPC != null)
                {
                    POI.IconContainer.Find("Outline/Icon").GetComponent<Image>().sprite = IconManager.LoadSprite(IconName);
                    POI.IconContainer.Find("Outline/Icon").GetComponent<RectTransform>().offsetMin = Vector2.zero;
                    POI.IconContainer.Find("Outline/Icon").GetComponent<RectTransform>().offsetMax = Vector2.zero;
                }
            }

            public void UpdateVisibility()
            {
                if (POI?.NPC == null) return;

                SetVisibility(POI.NPC.IsCurrentlySightable());
            }

            public void SetVisibility(bool isVisible)
            {
                if (POI == null) return;

                if (IsPolice)
                {
                    isVisible = isVisible && SkillTreeData.Informant.CurrentLevel == 1;
                }
                else
                {
                    isVisible = isVisible && SkillTreeData.Spymaster.CurrentLevel == 1;
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
                item.Value?.UpdateVisibility();
            }
        }

        private static IEnumerator SetupCustomPOI(NPC instance, string description, string iconName, bool isPolice)
        {
            yield return new WaitForSeconds(5f);
            if (!CustomPOIManager.ContainsKey(instance.ID))
            {
                CustomPOI customPOI = new CustomPOI
                {
                    IconName = iconName,
                    IsPolice = isPolice,
                    POI = Object.Instantiate(NetworkSingleton<NPCManager>.Instance.NPCPoIPrefab, instance.transform)
                };
                customPOI.POI.SetMainText($"{instance.fullName}\n{description}");
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
            if (__instance == null) return;

            if (CustomPOIManager.ContainsKey(__instance.ID))
            {
                CustomPOIManager[__instance.ID].SetVisibility(__instance.IsCurrentlySightable());
            }
        }

        [HarmonyPatch(typeof(PoliceOfficer), "Start")]
        [HarmonyPostfix]
        public static void Patch_PoliceOfficer_Start(PoliceOfficer __instance)
        {
            MelonCoroutines.Start(SetupCustomPOI(__instance, "Police Officer", IconManager.IconPolice, true));
        }

        [HarmonyPatch(typeof(CartelDealer), "Start")]
        [HarmonyPostfix]
        public static void Patch_CartelDealer_Start(CartelDealer __instance)
        {
            MelonCoroutines.Start(SetupCustomPOI(__instance, "Cartel Dealer", IconManager.IconBenziesDealer, false));
        }

        [HarmonyPatch(typeof(CartelGoon), "Start")]
        [HarmonyPostfix]
        public static void Patch_CartelGoon_Start(CartelGoon __instance)
        {
            MelonCoroutines.Start(SetupCustomPOI(__instance, "Cartel Goon", IconManager.IconBenziesGoon, false));
        }

        [HarmonyPatch(typeof(NPC), "OnDestroy")]
        [HarmonyPrefix]
        public static void Patch_NPC_OnDestroy(NPC __instance)
        {
            if (__instance?.TryCast<PoliceOfficer>() == null &&
                __instance?.TryCast<CartelGoon>() == null &&
                __instance?.TryCast<CartelDealer>() == null)
            {
                return;
            }

            CustomPOIManager.Remove(__instance.ID);
        }

        [HarmonyPatch(typeof(NPC), "OnDie")]
        [HarmonyPostfix]
        public static void Patch_Npc_OnDie(NPC __instance)
        {
            if (__instance == null) return;

            if (CustomPOIManager.ContainsKey(__instance.ID))
            {
                CustomPOIManager[__instance.ID].SetVisibility(__instance.IsCurrentlySightable());
            }

            if (SkillTreeData.Heal.CurrentLevel == 0) return;

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
    }
}
