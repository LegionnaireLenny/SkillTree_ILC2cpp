using Il2CppScheduleOne.Equipping;
using Il2CppScheduleOne.ItemFramework;
using SkillTree.Core.Skills;
using SkillTree.Core.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SkillTree.Core.Patches.Enforcer
{
    public class RangedWeaponPatches
    {
        public static void SetWeaponStats()
        {
            List<Equippable_RangedWeapon> allWeapons = Resources.FindObjectsOfTypeAll<Equippable_RangedWeapon>().ToList();
            List<IntegerItemDefinition> allInteger = Resources.FindObjectsOfTypeAll<IntegerItemDefinition>().ToList();
            Cache.FillCache(allWeapons);

            foreach (Equippable_RangedWeapon weapon in allWeapons)
            {
                if (Cache.OriginalRangedWeapons.ContainsKey(weapon.name))
                {
                    weapon.AccuracyChangeDuration = Cache.OriginalRangedWeapons[weapon.name].AccuracyChangeDuration * SkillModifiers.GetAimTimeMultiplier();
                    weapon.AimDuration = Cache.OriginalRangedWeapons[weapon.name].AimDuration * SkillModifiers.GetAimTimeMultiplier();
                    //weapon.Damage = Cache.OriginalRangedWeapons[weapon.name].Damage * 2f;
                    weapon.MagazineSize = Cache.OriginalRangedWeapons[weapon.name].MagazineSize * SkillModifiers.GetAmmoCapacityMultiplier();
                    weapon.MaxSpread = Cache.OriginalRangedWeapons[weapon.name].MaxSpread * SkillModifiers.GetMaxSpreadMultiplier();
                    weapon.MinSpread = Cache.OriginalRangedWeapons[weapon.name].MinSpread * SkillModifiers.GetMinSpreadMultiplier();

                    //if (Cache.OriginalRangedWeapons[weapon.name].PelletCount > 0)
                    //{
                    //    weapon.Cast<Equippable_PumpShotgun>().PelletCount = Cache.OriginalRangedWeapons[weapon.name].PelletCount * 2;
                    //}
                    LogManager.LogMessage($"Increased stats for {weapon.name}", LogLevel.Debug);
                }
            }

            foreach (var item in allInteger)
            {
                if (Cache.OriginalMagazineSizes.ContainsKey(item.ID))
                {
                    item.DefaultValue = Cache.OriginalMagazineSizes[item.ID] * SkillModifiers.GetAmmoCapacityMultiplier();
                    LogManager.LogMessage($"Increased magazine size for {item.name}", LogLevel.Debug);
                }
            }
        }
    }
}
