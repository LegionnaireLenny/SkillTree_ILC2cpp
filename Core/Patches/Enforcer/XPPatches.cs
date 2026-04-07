using HarmonyLib;
using Il2CppScheduleOne.Levelling;
using Il2CppScheduleOne.Quests;
using MelonLoader;
using SkillTree.Core.Serialization;
using SkillTree.Core.Skills;
using SkillTree.Core.Utilities;
using UnityEngine;

namespace SkillTree.Core.Patches.Enforcer
{
    public class XPPatches
    {
        [HarmonyPatch(typeof(LevelManager), "AddXP")]
        [HarmonyPrefix]
        public static void Prefix_AddXP(LevelManager __instance, ref int xp)
        {
            if (SkillTreeData.FastLearner.CurrentLevel == 0 && SkillTreeData.TurboNerdo.CurrentLevel == 0)
                return;

            int original = xp;
            int bonus = Mathf.CeilToInt(xp * (SkillModifiers.GetXPGainMultiplier() - 1));
            xp = (int)(xp * SkillModifiers.GetXPGainMultiplier());
            LogManager.LogMessage($"[MoreXP] Earned {bonus} XP from {original} | Skill bonus is {(int)(SkillModifiers.GetXPGainMultiplier() % 1 * 100)}% | {__instance.TotalXP} + {xp} = {__instance.TotalXP + xp}", LogLevel.Debug);
        }

        [HarmonyPatch(typeof(Contract), "SubmitPayment")]
        [HarmonyPostfix]
        public static void Postfix(Contract __instance, float bonusTotal)
        {
            if (LevelManager.Instance == null || SkillTreeData.Kingpin.CurrentLevel == 0)
                return;

            int bonusXP = Mathf.CeilToInt((__instance.Payment + bonusTotal) * SkillModifiers.GetSaleXPBonus());
            LevelManager.Instance.AddXP(bonusXP);
            LogManager.LogMessage($"[MoreXPWhenEarnMoney] Earned {bonusXP} bonus XP from ${__instance.Payment + bonusTotal} sale | Skill bonus is {(int)(SkillModifiers.GetSaleXPBonus() * 100)}%", LogLevel.Debug);
        }
    }
}