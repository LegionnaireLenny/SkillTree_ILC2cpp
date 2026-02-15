using HarmonyLib;
using MelonLoader;
using Il2CppScheduleOne;
using Il2CppScheduleOne.Delivery;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.GameTime;
using Il2CppScheduleOne.Growing;
using Il2CppScheduleOne.Interaction;
using Il2CppScheduleOne.Levelling;
using Il2CppScheduleOne.Messaging;
using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.NPCs.CharacterClasses;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.ObjectScripts.Cash;
using Il2CppScheduleOne.PlayerScripts.Health;
using Il2CppScheduleOne.Product;
using Il2CppScheduleOne.Quests;
using Il2CppScheduleOne.Tools;
using Il2CppScheduleOne.UI.Phone;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Il2CppScheduleOne.PlayerScripts;

namespace SkillTree.Core.Patches.Stats
{
    [HarmonyPatch(typeof(PlayerHealth))]
    public class PlayerHealthPatches
    {
        [HarmonyPatch("SetHealth")]
        [HarmonyPrefix]
        public static bool Prefix_SetHealth(PlayerHealth __instance, float health)
        {
            __instance.CurrentHealth = Mathf.Clamp(health, 0f, SkillModifiers.GetPlayerMaxHealth());
            __instance.onHealthChanged?.Invoke(__instance.CurrentHealth);
            if (__instance.CurrentHealth <= 0f)
            {
                __instance.SendDie();
            }
            return false;
        }

        [HarmonyPatch("RecoverHealth")]
        [HarmonyPrefix]
        public static bool Prefix_RecoverHealth(PlayerHealth __instance, float recovery)
        {
            if (__instance.CurrentHealth <= 0f)
            {
                Il2CppScheduleOne.Console.LogWarning("RecoverHealth called on dead player. Use Revive() instead.", null);
                return false;
            }
            __instance.CurrentHealth = Mathf.Clamp(__instance.CurrentHealth + recovery, 0f, SkillModifiers.GetPlayerMaxHealth());
            __instance.onHealthChanged?.Invoke(__instance.CurrentHealth);
            return false;
        }

        [HarmonyPatch("RpcLogic___TakeDamage_3505310624")]
        [HarmonyPrefix]
        public static bool Prefix_RpcLogic_TakeDamage(PlayerHealth __instance, float damage, bool flinch = true, bool playBloodMist = true)
        {
            if (!__instance.IsAlive)
            {
                return false;
            }

            if (!__instance.CanTakeDamage)
            {
                Il2CppScheduleOne.Console.LogWarning("Player cannot take damage right now.", null);
                return false;
            }

            MelonLogger.MsgPastel($"Player health: {__instance.CurrentHealth} - {damage} = {__instance.CurrentHealth - damage}");
            __instance.CurrentHealth = Mathf.Clamp(__instance.CurrentHealth - damage, 0f, SkillModifiers.GetPlayerMaxHealth());
            __instance.TimeSinceLastDamage = 0f;
            __instance.onHealthChanged?.Invoke(__instance.CurrentHealth);

            if (__instance.Player.IsOwner)
            {
                if (flinch && PlayerSingleton<PlayerCamera>.InstanceExists)
                {
                    PlayerSingleton<PlayerCamera>.Instance.JoltCamera();
                }

                if (__instance.CurrentHealth <= 0f)
                {
                    __instance.SendDie();
                }
            }

            if (playBloodMist)
            {
                __instance.PlayBloodMist();
            }

            return false;
        }
    }

    /// <summary>
    /// INCREASE XP GAIN
    /// </summary>

    [HarmonyPatch(typeof(LevelManager), "AddXP")]
    public class PatchLevelManager
    {
        private static bool processed = false;
        [HarmonyPrefix]
        public static void Prefix(LevelManager __instance, ref int xp)
        {
            if (Core.SkillData.MoreXP == 0 && Core.SkillData.MoreXP2 == 0)
                return;

            if (processed)
                return;

            processed = true;
            int xpOriginal = xp;
            xp = Mathf.CeilToInt(xp * (1 + SkillModifiers.GetXPGainBonus()));
            MelonLogger.Msg($"[XP] Apply: {xpOriginal} -> {xp} (Base: {SkillModifiers.BaseXPGainRate + SkillModifiers.GetXPGainBonus() * 100}%)");
            MelonLogger.Msg("Total XP Now: " + (__instance.TotalXP + xp));
        }

        [HarmonyPostfix]
        public static void Postfix()
        {
            processed = false;
        }
    }

    /// <summary>
    /// MORE XP BASE IN DEAL PAYMENTS
    /// </summary>
    [HarmonyPatch(typeof(Contract), "SubmitPayment")]
    public class PatchContractPayment
    {
        private static bool processed = false;
        [HarmonyPrefix]
        public static void Prefix(Contract __instance, float bonusTotal)
        {
            if (processed)
                return;

            if (Core.SkillData.MoreXPWhenEarnMoney == 0)
                return;

            if (__instance.Payment <= 0)
                return;

            int bonusXP = Mathf.RoundToInt(__instance.Payment * SkillModifiers.GetSaleXPBonus());

            if (bonusXP > 0)
            {
                LevelManager levelManager = LevelManager.Instance;

                if (levelManager != null)
                {
                    MelonLogger.Msg($"[Contract] Payment of ${__instance.Payment} converted into {bonusXP} base XP.");

                    levelManager.AddXP(bonusXP);
                    processed = true;
                }
            }
        }

        [HarmonyPostfix]
        public static void Postfix()
        {
            processed = false;
        }
    }

    /// <summary>
    /// SLEEP SYSTEM
    /// </summary>

    public static class ScheduleLogic
    {
        private static int lastDayUsed = -1;

        public static bool CanUseBedSkill()
        {
            int currentDay = (int)NetworkSingleton<TimeManager>.Instance.CurrentDay;
            return currentDay != lastDayUsed;
        }

        public static string GetTimeRemaining(float currentTime)
        {
            int next = GetNextSchedule();
            if (next == 0) next = 2400;

            int currentTotalMin = (int)currentTime / 100 * 60 + (int)currentTime % 100;
            int nextTotalMin = next / 100 * 60 + next % 100;

            int diff = nextTotalMin - currentTotalMin;
            int h = diff / 60;
            int m = diff % 60;

            return $"{h:00}h {m:00}m";
        }

        public static int GetNextSchedule()
        {
            float time = NetworkSingleton<TimeManager>.Instance.CurrentTime;

            if (time >= 700 && time < 1200) return 1200;
            if (time >= 1203 && time < 1800) return 1800;
            if (time >= 1803 && time < 2357) return 2357;

            return (int)time;
        }

        [HarmonyPatch(typeof(Bed), "CanSleep")]
        public static class Bed_AlwaysAllow
        {
            [HarmonyPrefix]
            public static bool Prefix(ref bool __result)
            {
                float currentTime = NetworkSingleton<TimeManager>.Instance.CurrentTime;

                if (Core.SkillData.AllowSleepAthEne == 0)
                    return true;

                if (currentTime > 700 && currentTime < 1800 && Core.SkillData.SkipSchedule == 0)
                    return true;

                __result = true;
                return false;
            }
        }

        [HarmonyPatch(typeof(Bed), "Hovered")]
        public static class Bed_Hovered_Patch
        {
            [HarmonyPrefix]
            public static bool Prefix(Bed __instance)
            {
                if (Core.SkillData.SkipSchedule == 0)
                    return true;

                if (Singleton<ManagementClipboard>.Instance.IsEquipped || __instance.AssignedEmployee != null)
                    return true;

                float currentTime = NetworkSingleton<TimeManager>.Instance.CurrentTime;
                if (currentTime >= 0 && currentTime < 700)
                    return true;

                if (!CanUseBedSkill() && currentTime <= 1800)
                {
                    __instance.intObj.SetMessage("You've already rested today! Use it only tomorrow.");
                }
                else if (CanUseBedSkill() && currentTime < 2357)
                {
                    string remaining = GetTimeRemaining(currentTime);
                    __instance.intObj.SetMessage($"Next Shift in: {remaining}");
                }
                else
                    return true;

                return false;
            }
        }

        [HarmonyPatch(typeof(Bed), "Interacted")]
        public static class Bed_Interacted_Patch
        {
            [HarmonyPrefix]
            public static bool Prefix()
            {
                if (Core.SkillData.SkipSchedule == 0)
                    return true;

                if (!CanUseBedSkill())
                {
                    MelonLogger.Msg("[BedSkill] You've already rested today! You can't use it until tomorrow.");
                    return true;
                }

                float currentTime = NetworkSingleton<TimeManager>.Instance.CurrentTime;

                if (currentTime >= 700)
                {
                    int nextTarget = GetNextSchedule();

                    int totalMinutesPassed = CalculateMinutesBetween(currentTime, nextTarget) / 3;

                    if (totalMinutesPassed > 0)
                    {
                        foreach (GrowContainer container in UnityEngine.Object.FindObjectsOfType<GrowContainer>())
                            AccessTools.Method(typeof(GrowContainer), "DrainMoisture")?.Invoke(container, new object[] { totalMinutesPassed * 3 });
                        foreach (Plant plant in UnityEngine.Object.FindObjectsOfType<Plant>())
                            plant.MinPass(totalMinutesPassed);
                    }

                    lastDayUsed = (int)NetworkSingleton<TimeManager>.Instance.CurrentDay;

                    NetworkSingleton<TimeManager>.Instance.SetTime(nextTarget);

                    MelonLogger.Msg($"[BedSkill] Interaction detected. Next schedule set for: {nextTarget}");
                    return false;
                }
                return true;
            }
        }

        private static int CalculateMinutesBetween(float start, float end)
        {
            if (end == 0) end = 2400;

            int startHours = (int)start / 100;
            int startMins = (int)start % 100;
            int endHours = (int)end / 100;
            int endMins = (int)end % 100;

            int startTotal = startHours * 60 + startMins;
            int endTotal = endHours * 60 + endMins;

            return endTotal - startTotal;
        }
    }

    /// <summary>
    /// COUNTER OFFER 
    /// </summary>
    /// 
    public static class CounterofferHelper
    {
        public static float CalculateSuccessChance(CounterofferInterface instance)
        {
            var conversation = instance.conversation;
            var price = instance.price;
            var product = instance.selectedProduct;
            var quantity = instance.quantity;

            Customer customer = conversation.sender.GetComponent<Customer>(); 
            CustomerData customerData = customer.CustomerData; 
            NPC NPC = customer.NPC;

            float adjustedWeeklySpend = customerData.GetAdjustedWeeklySpend(NPC.RelationData.RelationDelta / 5f);

            Il2CppSystem.Collections.Generic.List<EDay> orderDays = customerData.GetOrderDays(customer.CurrentAddiction, NPC.RelationData.RelationDelta / 5f);
            float num = adjustedWeeklySpend / orderDays.Count;

            if (price >= num * 3f) 
                return 0f;

            float valueProposition = Customer.GetValueProposition(Registry.GetItem<ProductDefinition>(customer.OfferedContractInfo.Products.entries[0].ProductID), 
                                    customer.OfferedContractInfo.Payment / customer.OfferedContractInfo.Products.entries[0].Quantity);

            float productEnjoyment = customer.GetProductEnjoyment(product, customerData.Standards.GetCorrespondingQuality());

            float num2 = Mathf.InverseLerp(-1f, 1f, productEnjoyment);

            float valueProposition2 = Customer.GetValueProposition(product, price / quantity);

            float num3 = Mathf.Pow(quantity / (float)customer.OfferedContractInfo.Products.entries[0].Quantity, 0.6f);

            float num4 = Mathf.Lerp(0f, 2f, num3 * 0.5f); float num5 = Mathf.Lerp(1f, 0f, Mathf.Abs(num4 - 1f));

            if (valueProposition2 * num5 > valueProposition)
                return 1f;

            if (valueProposition2 < 0.12f) 
                return 0f;

            float num6 = productEnjoyment * valueProposition;
            float num7 = num2 * num5 * valueProposition2;
            if (num7 > num6) 
              return 1f;

            float num8 = num6 - num7;
            float num9 = Mathf.Lerp(0f, 1f, num8 / 0.2f);
            float t = Mathf.Max(customer.CurrentAddiction, NPC.RelationData.NormalizedRelationDelta);
            float num10 = Mathf.Lerp(0f, 0.2f, t);

            if (num9 <= num10) 
                return 1f;

            if (num9 - num10 >= 0.9f) 
                return 0f;

            float probability = (0.9f + num10 - num9) / 0.9f;
            return Mathf.Clamp(probability, 0f, 1f);
        }

        public static Text SuccessLabel;

        public static void CreateSuccessLabel(CounterofferInterface instance)
        {
            if (SuccessLabel != null)
                return;

            var fairLabel = instance.FairPriceLabel;

            var parent = fairLabel.transform.parent;

            var go = UnityEngine.Object.Instantiate(
                fairLabel.gameObject,
                parent
            );

            go.name = "SuccessChanceLabel";

            SuccessLabel = go.GetComponent<Text>();
            SuccessLabel.font = fairLabel.font;
            SuccessLabel.fontSize = fairLabel.fontSize;
            SuccessLabel.fontStyle = FontStyle.Bold;
            SuccessLabel.alignment = fairLabel.alignment;
            SuccessLabel.color = Color.black;
            SuccessLabel.supportRichText = true;
            SuccessLabel.enabled = true;
            SuccessLabel.text = "Success chance: --%";

            var layout = go.AddComponent<LayoutElement>();
            layout.ignoreLayout = true;

            RectTransform fairRT = fairLabel.rectTransform;
            RectTransform rt = SuccessLabel.rectTransform;

            rt.anchorMin = fairRT.anchorMin;
            rt.anchorMax = fairRT.anchorMax;
            rt.pivot = fairRT.pivot;
            rt.sizeDelta = fairRT.sizeDelta;

            rt.anchoredPosition = fairRT.anchoredPosition + new Vector2(0f, -23f);

            go.transform.SetAsLastSibling();
            //MelonLogger.Msg("SuccessChanceLabel visível abaixo do FairPrice");

        }

        public static void UpdateSuccessLabel(CounterofferInterface instance)
        {
            if (SuccessLabel == null)
                return;

            float chance = CalculateSuccessChance(instance);
            //MelonLogger.Msg($"CalculateSuccessChance {chance}");

            string color =
                chance >= 0.75f ? "#4CAF50" :
                chance >= 0.4f ? "#FFC107" :
                "#F44336";

            SuccessLabel.text =
                $"<color={color}>Success chance: {chance * 100f:0}%</color>";
            //MelonLogger.Msg($"SuccessLabel.text {SuccessLabel.text}");
        }

        [HarmonyPatch(typeof(CounterofferInterface), "Open")]
        public static class Counteroffer_Open_Patch
        {
            [HarmonyPostfix]
            public static void Postfix(CounterofferInterface __instance)
            {
                if (Core.SkillData.AllowSeeCounteroffChance == 0)
                    return;

                CreateSuccessLabel(__instance);
                UpdateSuccessLabel(__instance);
            }
        }

        [HarmonyPatch(typeof(CounterofferInterface), "ChangeQuantity")]
        public static class Counteroffer_ChangeQuantity_Patch
        {
            [HarmonyPostfix]
            public static void Postfix(CounterofferInterface __instance)
            {
                if (Core.SkillData.AllowSeeCounteroffChance == 0)
                    return;

                UpdateSuccessLabel(__instance);
            }
        }

        [HarmonyPatch(typeof(CounterofferInterface), "ChangePrice")]
        public static class Counteroffer_ChangePrice_Patch
        {
            [HarmonyPostfix]
            public static void Postfix(CounterofferInterface __instance)
            {
                if (Core.SkillData.AllowSeeCounteroffChance == 0)
                    return;

                UpdateSuccessLabel(__instance);
            }
        }

    }

    /// <summary>
    /// BETTER DELIVERY
    /// </summary>

    [HarmonyPatch(typeof(DeliveryManager), "SendDelivery")]
    public static class DeliveryTime_Patch
    {
        [HarmonyPrefix]
        public static void Prefix(ref DeliveryInstance delivery)
        {
            if (Core.SkillData.BetterDelivery == 0)
                return;

            if (delivery == null)
                return;

            if (delivery.TimeUntilArrival <= 120)
                return;

            int originalTime = delivery.TimeUntilArrival;

            float ratio = Mathf.InverseLerp(60f, 360f, originalTime);

            int newTime = Mathf.RoundToInt(Mathf.Lerp(30f, 120f, ratio));

            delivery.TimeUntilArrival = newTime;

            MelonLogger.Msg($"[DeliverySkill] Delivery scaling adjusted. Original: {originalTime}m | New: {newTime}m");
        }
    }
}