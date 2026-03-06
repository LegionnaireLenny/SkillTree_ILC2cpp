using HarmonyLib;
using Il2CppScheduleOne.Cartel;
using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.Police;
using MelonLoader;
using SkillTree.Core.Skills;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace SkillTree.Core.Patches.Special
{
    [HarmonyPatch]
    public class NPCPatches
    {
        public static int PoliceKilled { get; private set; } = 0;
        public static int CartelKilled { get; private set; } = 0;
        public static int CivilianKilled { get; private set; } = 0;

        //private void SetupPoI()
        //{
        //    if (this.DealerPoI == null)
        //    {
        //        this.DealerPoI = global::UnityEngine.Object.Instantiate<NPCPoI>(NetworkSingleton<NPCManager>.Instance.NPCPoIPrefab, base.transform);
        //        this.DealerPoI.SetMainText(base.fullName + "\n(Dealer)");
        //        this.DealerPoI.SetNPC(this);
        //        this.DealerPoI.transform.localPosition = Vector3.zero;
        //        this.DealerPoI.enabled = this.IsRecruited;
        //    }
        //    if (this.PotentialDealerPoI == null)
        //    {
        //        this.PotentialDealerPoI = global::UnityEngine.Object.Instantiate<NPCPoI>(NetworkSingleton<NPCManager>.Instance.PotentialDealerPoIPrefab, base.transform);
        //        this.PotentialDealerPoI.SetMainText("Potential Dealer\n" + base.fullName);
        //        this.PotentialDealerPoI.SetNPC(this);
        //        float num = (float)(this.FirstName[0] % '$') * 10f;
        //        float num2 = Mathf.Clamp((float)this.FirstName.Length * 1.5f, 1f, 10f);
        //        Vector3 vector = base.transform.forward;
        //        vector = Quaternion.Euler(0f, num, 0f) * vector;
        //        this.PotentialDealerPoI.transform.localPosition = vector * num2;
        //    }
        //    this.UpdatePotentialDealerPoI();
        //}

        [HarmonyPatch(typeof(NPC), "OnDie")]
        [HarmonyPostfix]
        public static void Patch_NpcOnDie(NPC __instance)
        {
            // OnDie is called twice for police, so PoliceKilled will increase by two every time a cop is killed
            if (__instance.TryCast<PoliceOfficer>() != null)
            {
                PoliceKilled++;
                //MelonLogger.Msg($"Killed police: {__instance.fullName} | Total police killed: {PoliceKilled}");
            }
            else if (__instance.TryCast<CartelGoon>() != null)
            {
                CartelKilled++;
                //MelonLogger.Msg($"Killed cartel goon: {__instance.fullName} | Total cartel killed: {CartelKilled}");
            }
            else if (__instance.TryCast<CartelDealer>() != null)
            {
                CartelKilled++;
                //MelonLogger.Msg($"Killed cartel dealer: {__instance.fullName} | Total cartel killed: {CartelKilled}");
            }
            else
            {
                CivilianKilled++;
                //MelonLogger.Msg($"Killed civilian: {__instance.fullName} | Total civilians killed: {CivilianKilled}");
            }
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
                    MelonLogger.Warning($"Failed to load value for {property.Name} from file {e}");
                }
            }
        }
    }
}
