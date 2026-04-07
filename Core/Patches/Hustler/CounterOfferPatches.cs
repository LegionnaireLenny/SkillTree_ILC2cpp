using HarmonyLib;
using Il2CppScheduleOne;
using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.GameTime;
using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.Product;
using Il2CppScheduleOne.UI.Phone;
using SkillTree.Core.Serialization;
using UnityEngine;
using UnityEngine.UI;

namespace SkillTree.Core.Patches.Hustler
{
    public static class CounterOfferPatches
    {
        private static Text SuccessLabel;

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
            float num4 = Mathf.Lerp(0f, 2f, num3 * 0.5f);
            float num5 = Mathf.Lerp(1f, 0f, Mathf.Abs(num4 - 1f));

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
            float num10 = Mathf.Max(customer.CurrentAddiction, NPC.RelationData.NormalizedRelationDelta);
            float num11 = Mathf.Lerp(0f, 0.2f, num10);

            if (num9 <= num11)
                return 1f;

            if (num9 - num11 >= 0.9f)
                return 0f;

            float probability = (0.9f + num11 - num9) / 0.9f;
            return Mathf.Clamp(probability, 0f, 1f);
        }


        public static void CreateSuccessLabel(CounterofferInterface instance)
        {
            if (SuccessLabel != null)
            {
                return;
            }

            var go = Object.Instantiate(
                instance.FairPriceLabel.gameObject,
                instance.FairPriceLabel.transform.parent
            );

            go.name = "SuccessChanceLabel";

            SuccessLabel = go.GetComponent<Text>();
            SuccessLabel.font = instance.FairPriceLabel.font;
            SuccessLabel.fontSize = instance.FairPriceLabel.fontSize + 4;
            SuccessLabel.fontStyle = FontStyle.Bold;
            SuccessLabel.alignment = instance.FairPriceLabel.alignment;
            SuccessLabel.color = Color.black;
            SuccessLabel.supportRichText = true;
            SuccessLabel.enabled = true;
            SuccessLabel.text = "Success chance: --%";

            var layout = go.AddComponent<LayoutElement>();
            layout.ignoreLayout = true;

            RectTransform fairRT = instance.FairPriceLabel.rectTransform;
            RectTransform rt = SuccessLabel.rectTransform;

            rt.anchorMin = fairRT.anchorMin;
            rt.anchorMax = fairRT.anchorMax;
            rt.pivot = fairRT.pivot;
            rt.sizeDelta = fairRT.sizeDelta;


            rt.anchoredPosition = fairRT.anchoredPosition + new Vector2(0f, -23f);

            go.transform.SetAsLastSibling();

        }

        public static void UpdateSuccessLabel(CounterofferInterface instance)
        {
            if (SuccessLabel == null)
                return;

            float chance = CalculateSuccessChance(instance);
            string color =
                chance >= 0.75f ? "#4CAF50" :
                chance >= 0.4f ? "#FFC107" :
                "#F44336";

            SuccessLabel.text = $"<color={color}>Success chance: {chance * 100f:0}%</color>";
        }

        [HarmonyPatch(typeof(CounterofferInterface), "Open")]
        public static class Counteroffer_Open_Patch
        {
            [HarmonyPostfix]
            public static void Postfix(CounterofferInterface __instance)
            {
                if (SkillTreeData.CrystalBall.CurrentLevel == 0)
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
                if (SkillTreeData.CrystalBall.CurrentLevel == 0)
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
                if (SkillTreeData.CrystalBall.CurrentLevel == 0)
                    return;

                UpdateSuccessLabel(__instance);
            }
        }
    }
}