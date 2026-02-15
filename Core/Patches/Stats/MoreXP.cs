using HarmonyLib;
using Il2CppScheduleOne.Levelling;
using Il2CppScheduleOne.Quests;
using MelonLoader;
using UnityEngine;

namespace SkillTree.Core.Patches.Stats
{
    [HarmonyPatch(typeof(LevelManager), "AddXP")]
    public class MoreXP
    {
        [HarmonyPrefix]
        public static void Prefix_AddXP(LevelManager __instance, ref int xp)
        {
            if (Core.SkillData.MoreXP == 0 && Core.SkillData.MoreXP2 == 0)
                return;

            int original = xp;
            int bonus = Mathf.CeilToInt(xp * SkillModifiers.GetXPGainBonus());
            xp += bonus;
            MelonLogger.Msg($"[XP] Earned {bonus} XP from {original} | Skill bonus is {(int)(SkillModifiers.GetXPGainBonus() * 100)}% | {__instance.TotalXP} + {xp} = {__instance.TotalXP + xp}");
        }
    }

    [HarmonyPatch(typeof(Contract), "SubmitPayment")]
    public class PatchContractPayment
    {
        [HarmonyPostfix]
        public static void Postfix(Contract __instance, float bonusTotal)
        {
            if (LevelManager.Instance == null || Core.SkillData == null || Core.SkillData.MoreXPWhenEarnMoney == 0)
                return;

            int bonusXP = Mathf.CeilToInt((__instance.Payment + bonusTotal) * SkillModifiers.GetSaleXPBonus());

            MelonLogger.Msg($"[Contract] Earned {bonusXP} bonus XP from ${__instance.Payment + bonusTotal} sale | Skill bonus is {(int)(SkillModifiers.GetSaleXPBonus() * 100)}%");
            LevelManager.Instance.AddXP(bonusXP);
        }
    }
}