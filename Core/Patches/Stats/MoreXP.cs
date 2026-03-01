using HarmonyLib;
using Il2CppScheduleOne.Levelling;
using Il2CppScheduleOne.Quests;
using MelonLoader;
using SkillTree.Core.FileManagement;
using UnityEngine;

namespace SkillTree.Core.Patches.Stats
{
    [HarmonyPatch(typeof(LevelManager), "AddXP")]
    public class MoreXP
    {
        [HarmonyPrefix]
        public static void Prefix_AddXP(LevelManager __instance, ref int xp)
        {
            if (SkillTreeData.MoreXP.CurrentLevel == 0 && SkillTreeData.MoreXP2.CurrentLevel == 0)
                return;

            int original = xp;
            int bonus = Mathf.CeilToInt(xp * (SkillModifiers.GetXPGainMultiplier() - 1));
            xp = (int)(xp * SkillModifiers.GetXPGainMultiplier());
            //MelonLogger.Msg($"[MoreXP] Earned {bonus} XP from {original} | Skill bonus is {(int)(SkillModifiers.GetXPGainMultiplier() % 1 * 100)}% | {__instance.TotalXP} + {xp} = {__instance.TotalXP + xp}");
        }
    }

    [HarmonyPatch(typeof(Contract), "SubmitPayment")]
    public class PatchContractPayment
    {
        [HarmonyPostfix]
        public static void Postfix(Contract __instance, float bonusTotal)
        {
            if (LevelManager.Instance == null || SkillTreeData.MoreXPWhenEarnMoney.CurrentLevel == 0)
                return;

            int bonusXP = Mathf.CeilToInt((__instance.Payment + bonusTotal) * SkillModifiers.GetSaleXPBonus());

            MelonLogger.Msg($"[MoreXPWhenEarnMoney] Earned {bonusXP} bonus XP from ${__instance.Payment + bonusTotal} sale | Skill bonus is {(int)(SkillModifiers.GetSaleXPBonus() * 100)}%");
            LevelManager.Instance.AddXP(bonusXP);
        }
    }
}