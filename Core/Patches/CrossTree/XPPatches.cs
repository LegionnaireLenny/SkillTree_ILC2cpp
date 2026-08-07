using HarmonyLib;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.Levelling;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.PlayerTasks;
using Il2CppScheduleOne.Product;
using Il2CppScheduleOne.Product.Packaging;
using Il2CppScheduleOne.Quests;
using SkillTree.Core.Serialization;
using SkillTree.Core.Skills;
using SkillTree.Core.Utilities;
using UnityEngine;
using static SkillTree.Core.Utilities.ConfigManager;

namespace SkillTree.Core.Patches.CrossTree
{
    [HarmonyPatch]
    public class XPPatches
    {
        [HarmonyPatch(typeof(LevelManager), "AddXP")]
        [HarmonyPrefix]
        public static void Prefix_AddXP(LevelManager __instance, ref int xp)
        {
            if (SkillTreeData.SchoolOfHardKnocks.CurrentLevel == 0 && SkillTreeData.Meister.CurrentLevel == 0 && SkillTreeData.MultiLevelMarketeer.CurrentLevel == 0 && SkillTreeData.EducatedWorkforce.CurrentLevel == 0)
                return;

            int original = xp;
            int bonus = Mathf.CeilToInt(xp * (SkillModifiers.GetXPGainMultiplier() - 1));
            xp = (int)(xp * SkillModifiers.GetXPGainMultiplier());
            LogManager.LogMessage($"[AddXP] Original XP: {original} | Bonus XP: {bonus} | Total Gain: {xp} | Skill Multiplier: x{SkillModifiers.GetXPGainMultiplier()} | {__instance.TotalXP} + {xp} = {__instance.TotalXP + xp}", LogLevel.Debug);
        }

        [HarmonyPatch(typeof(Contract), "SubmitPayment")]
        [HarmonyPostfix]
        public static void Postfix_SubmitPayment(Contract __instance, float bonusTotal)
        {
            if (LevelManager.Instance == null || SkillTreeData.Grifter.CurrentLevel == 0)
                return;

            int bonusXP = Mathf.CeilToInt((__instance.Payment + bonusTotal) * SkillModifiers.GetSaleValueXPBonus());
            LogManager.LogMessage($"[Grifter] Sale Value: ${__instance.Payment + bonusTotal} | Bonus XP: {bonusXP} | Conversion Rate: {SkillModifiers.GetSaleValueXPBonus() * 100}%", LogLevel.Debug);
            NetworkSingleton<LevelManager>.Instance.AddXP(bonusXP);
        }

        [HarmonyPatch(typeof(MixingStation), "TryCreateOutputItems")]
        [HarmonyPrefix]
        public static void Prefix_TryCreateOutputItems(MixingStation __instance)
        {
            if (LevelManager.Instance == null || __instance.CurrentMixOperation == null || SkillTreeData.Apprenticeship.CurrentLevel == 0)
                return;

            int xp = DrugMixingXP.GetValue(UseDefault.GetValue()) * __instance.CurrentMixOperation.Quantity;
            LogManager.LogMessage($"[Apprenticeship] Drug Production XP (Mixing): {xp}", LogLevel.Debug);
            NetworkSingleton<LevelManager>.Instance.AddXP(xp);
        }

        [HarmonyPatch(typeof(ProductManager), "FinishAndNameMix", [typeof(string), typeof(string), typeof(string)])]
        [HarmonyPostfix]
        public static void Postfix_FinishAndNameMix()
        {
            if (LevelManager.Instance == null || SkillTreeData.Apprenticeship.CurrentLevel == 0)
                return;

            int xp = Mathf.CeilToInt(BaseNewMixXPGain.GetValue(UseDefault.GetValue()) * SkillModifiers.GetNewMixXPMultiplier());
            LogManager.LogMessage($"[Apprenticeship] Base New Mix XP: {BaseNewMixXPGain.GetValue(UseDefault.GetValue())} | XP Gained {xp} | Skill Multiplier: x{SkillModifiers.GetNewMixXPMultiplier()}", LogLevel.Debug);
            NetworkSingleton<LevelManager>.Instance.AddXP(xp);
        }

        [HarmonyPatch(typeof(PackagingStation), "PackSingleInstance")]
        [HarmonyPostfix]
        public static void Postfix_PackSingleInstance(PackagingStation __instance)
        {
            if (LevelManager.Instance == null || __instance == null || SkillTreeData.Apprenticeship.CurrentLevel == 0)
                return;

            int xp = DrugPackagingXP.GetValue(UseDefault.GetValue()) * __instance.PackagingSlot.ItemInstance.Definition.Cast<PackagingDefinition>().Quantity;
            LogManager.LogMessage($"[Apprenticeship] Drug Packaging XP (Packaging Station): {xp}", LogLevel.Debug);
            NetworkSingleton<LevelManager>.Instance.AddXP(xp);
        }

        [HarmonyPatch(typeof(BrickPress), "CompletePress")]
        [HarmonyPostfix]
        public static void Postfix_CompletePress(BrickPress __instance)
        {
            if (LevelManager.Instance == null || __instance == null || SkillTreeData.Apprenticeship.CurrentLevel == 0)
                return;

            int xp = DrugPackagingXP.GetValue(UseDefault.GetValue()) * __instance.BrickPackaging.Quantity;
            LogManager.LogMessage($"[Apprenticeship] Drug Packaging XP (Brick Press): {xp}", LogLevel.Debug);
            NetworkSingleton<LevelManager>.Instance.AddXP(xp);
        }

        [HarmonyPatch(typeof(SowSeedTask), "Success")]
        [HarmonyPostfix]
        public static void Postfix_SowSeedTask()
        {
            if (LevelManager.Instance == null || SkillTreeData.Apprenticeship.CurrentLevel == 0)
                return;

            int xp = PlantSeedXP.GetValue(UseDefault.GetValue());
            LogManager.LogMessage($"[Apprenticeship] Plant Seed XP: {xp}", LogLevel.Debug);
            NetworkSingleton<LevelManager>.Instance.AddXP(xp);
        }

        [HarmonyPatch(typeof(ApplyShroomSpawnTask), "Success")]
        [HarmonyPostfix]
        public static void Postfix_ApplyShroomSpawnTask()
        {
            if (LevelManager.Instance == null || SkillTreeData.Apprenticeship.CurrentLevel == 0)
                return;

            int xp = PlantSeedXP.GetValue(UseDefault.GetValue());
            LogManager.LogMessage($"[Apprenticeship] Plant Shroom Spawn XP: {xp}", LogLevel.Debug);
            NetworkSingleton<LevelManager>.Instance.AddXP(xp);
        }

        [HarmonyPatch(typeof(Customer), "RpcLogic___ProcessCounterOfferServerSide_900355577")]
        [HarmonyPostfix]
        public static void RpcLogic___ProcessCounterOfferServerSide_900355577(Customer __instance)
        {
            if (__instance?.OfferedContractInfo?.IsCounterOffer == true)
            {
                int xp = BaseCounterOfferXPGain.GetValue(UseDefault.GetValue()) * SkillModifiers.GetCounterOfferXPMultiplier();
                LogManager.LogMessage($"[Grifter] Base Counter Offer XP: {BaseCounterOfferXPGain.GetValue(UseDefault.GetValue())} | XP Gained: {xp} | Skill Multiplier: x{SkillModifiers.GetCounterOfferXPMultiplier()}", LogLevel.Debug);
                NetworkSingleton<LevelManager>.Instance.AddXP(xp);
            }
        }
    }
}