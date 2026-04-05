using HarmonyLib;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Equipping;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.UI;
using static SkillTree.Core.Utilities.ConfigManager;

namespace SkillTree.Core.Patches.Miscellaneous
{
    [HarmonyPatch]
    public class CrosshairPatches
    {
        [HarmonyPatch(typeof(Equippable_RangedWeapon), "Equip")]
        [HarmonyPostfix]
        public static void Patch_Equippable_RangedWeapon_Equip(ItemInstance item)
        {
            Singleton<HUD>.Instance.SetCrosshairVisible(EnableCrosshair.GetValue());
        }

        [HarmonyPatch(typeof(Equippable_RangedWeapon), "Update")]
        [HarmonyPostfix]
        public static void Patch_Equippable_RangedWeapon_Update()
        {
            Singleton<HUD>.Instance.SetCrosshairVisible(EnableCrosshair.GetValue());
        }
    }
}
