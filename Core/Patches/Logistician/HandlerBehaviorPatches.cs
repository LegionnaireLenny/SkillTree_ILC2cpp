using Il2CppScheduleOne.Employees;
using SkillTree.Core.Skills;
using UnityEngine;

namespace SkillTree.Core.Patches.Logistician
{
    public class HandlerBehaviorPatches
    {
        public static void SetHandlerPackagingSpeed()
        {
            Packager[] packagers = Object.FindObjectsOfType<Packager>();

            foreach (Packager packager in packagers)
            {
                packager.PackagingSpeedMultiplier = SkillModifiers.GetHandlerPackagingSpeedMultiplier();
            }
        }
    }
}
