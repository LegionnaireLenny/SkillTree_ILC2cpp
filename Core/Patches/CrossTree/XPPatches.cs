using HarmonyLib;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.Levelling;
using Il2CppScheduleOne.Product;
using Il2CppScheduleOne.Quests;
using SkillTree.Core.Serialization;
using SkillTree.Core.Skills;
using SkillTree.Core.Utilities;
using UnityEngine;
using static SkillTree.Core.Utilities.ConfigManager;

namespace SkillTree.Core.Patches.CrossTree
{
    public class XPPatches
    {
        [HarmonyPatch(typeof(LevelManager), "AddXP")]
        [HarmonyPrefix]
        public static void Prefix_AddXP(LevelManager __instance, ref int xp)
        {
            if (SkillTreeData.SchoolOfHardKnocks.CurrentLevel == 0 && SkillTreeData.Apprenticeship.CurrentLevel == 0 && SkillTreeData.SalesExperience.CurrentLevel == 0 && SkillTreeData.EducatedWorkforce.CurrentLevel == 0)
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
            if (LevelManager.Instance == null || SkillTreeData.Grifter.CurrentLevel == 0)
                return;

            int bonusXP = Mathf.CeilToInt((__instance.Payment + bonusTotal) * SkillModifiers.GetSaleValueXPBonus());
            NetworkSingleton<LevelManager>.Instance.AddXP(bonusXP);
            LogManager.LogMessage($"[Grifter] Earned {bonusXP} bonus XP from ${__instance.Payment + bonusTotal} sale | Skill bonus is {(int)(SkillModifiers.GetSaleValueXPBonus() * 100)}%", LogLevel.Debug);
        }

        [HarmonyPatch(typeof(ProductManager), "FinishAndNameMix")]
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (LevelManager.Instance == null || SkillTreeData.Meister.CurrentLevel == 0)
                return;

            int xp = Mathf.CeilToInt(BaseNewMixXPGain.GetValue(UseDefault.GetValue()) * SkillModifiers.GetNewMixXPMultiplier());
            NetworkSingleton<LevelManager>.Instance.AddXP(xp);
            LogManager.LogMessage($"[Meister] Earned {xp} bonus XP from creating a new mix", LogLevel.Debug);
        }

        [HarmonyPatch(typeof(Customer), "RpcLogic___SetContractIsCounterOffer_2166136261")]
        [HarmonyPostfix]
        public void RpcLogic___SetContractIsCounterOffer_2166136261(Customer __instance)
        {
            if (__instance.OfferedContractInfo?.IsCounterOffer == true)
            {
                int xp = BaseCounterOfferXPGain.GetValue(UseDefault.GetValue()) * SkillModifiers.GetCounterOfferXPMultiplier();
                NetworkSingleton<LevelManager>.Instance.AddXP(xp);
                LogManager.LogMessage($"[Meister] Earned {xp} bonus XP from a successful counter offer", LogLevel.Debug);
            }
        }
    }
}