using HarmonyLib;
using Il2CppFishNet;
using Il2CppScheduleOne;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.Management;
using Il2CppScheduleOne.NPCs.Behaviour;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.StationFramework;
using MelonLoader;
using SkillTree.Core.Skills;
using System.Collections;
using UnityEngine;


namespace SkillTree.Core.Patches.Logistician
{
    [HarmonyPatch]
    public class ChemistBehaviorPatches
    {
        [HarmonyPatch(typeof(FinishLabOvenBehaviour), "RpcLogic___StartAction_2166136261")]
        [HarmonyPrefix]
        public static bool RpcLogic___StartAction_2166136261(FinishLabOvenBehaviour __instance)
        {
            if (__instance.actionRoutine != null)
            {
                return false;
            }
            if (__instance.targetOven == null)
            {
                return false;
            }
            __instance.actionRoutine = (Coroutine)MelonCoroutines.Start(StartAction(__instance));
            return false;
        }

        private static IEnumerator StartAction(FinishLabOvenBehaviour instance)
        {
            instance.targetOven.SetNPCUser(instance.Npc.NetworkObject);
            instance.Npc.Movement.FacePoint(instance.targetOven.transform.position, 0.5f);
            yield return new WaitForSeconds(0.5f);
            if (!instance.CanActionStart())
            {
                instance.StopAction();
                instance.Deactivate_Networked(null);
                yield break;
            }
            instance.Npc.SetEquippable_Client(null, "Avatar/Equippables/Hammer");
            instance.targetOven.Door.SetPosition(1f);
            instance.targetOven.WireTray.SetPosition(1f);
            yield return new WaitForSeconds(0.5f);
            instance.targetOven.SquareTray.SetParent(instance.targetOven.transform);
            instance.targetOven.RemoveTrayAnimation.Play();
            yield return new WaitForSeconds(0.1f);
            instance.targetOven.Door.SetPosition(0f);
            yield return new WaitForSeconds(1f);
            instance.Npc.SetAnimationBool_Networked(null, "UseHammer", true);
            yield return new WaitForSeconds(10f * SkillModifiers.GetChemistActionDurationMultiplier());
            instance.Npc.SetAnimationBool_Networked(null, "UseHammer", false);
            instance.targetOven.Shatter(instance.targetOven.CurrentOperation.Cookable.ProductQuantity, instance.targetOven.CurrentOperation.Cookable.ProductShardPrefab.gameObject);
            yield return new WaitForSeconds(1f);
            ItemInstance productItem = instance.targetOven.CurrentOperation.GetProductItem(instance.targetOven.CurrentOperation.Cookable.ProductQuantity * instance.targetOven.CurrentOperation.IngredientQuantity);
            instance.targetOven.OutputSlot.AddItem(productItem, false);
            instance.targetOven.SendCookOperation(null);
            instance.StopAction();
            instance.Deactivate_Networked(null);
            yield break;
        }

        [HarmonyPatch(typeof(StartCauldronBehaviour), "RpcLogic___BeginCauldron_2166136261")]
        [HarmonyPrefix]
        public static bool RpcLogic___BeginCauldron_2166136261(StartCauldronBehaviour __instance)
        {
            if (__instance.WorkInProgress)
            {
                return false;
            }
            if (__instance.Station == null)
            {
                return false;
            }
            __instance.WorkInProgress = true;
            __instance.Npc.Movement.FaceDirection(__instance.Station.StandPoint.forward, 0.5f);
            __instance.workRoutine = (Coroutine)MelonCoroutines.Start(BeginCauldron(__instance));
            return false;
        }

        private static IEnumerator BeginCauldron(StartCauldronBehaviour instance)
        {
            yield return new WaitForEndOfFrame();
            instance.Npc.Avatar.Animation.SetBool("UseChemistryStation", true);
            float packageTime = 15f * SkillModifiers.GetChemistActionDurationMultiplier();
            for (float i = 0f; i < packageTime; i += Time.deltaTime)
            {
                instance.Npc.Avatar.LookController.OverrideLookTarget(instance.Station.LinkOrigin.position, 0, false);
                yield return new WaitForEndOfFrame();
            }
            instance.Npc.Avatar.Animation.SetBool("UseChemistryStation", false);
            if (InstanceFinder.IsServer)
            {
                EQuality equality = instance.Station.RemoveIngredients();

                instance.Station.StartCookOperation(null, instance.Station.CookTime, equality);
            }

            instance.WorkInProgress = false;
            instance.workRoutine = null;
            yield break;
        }

        [HarmonyPatch(typeof(StartChemistryStationBehaviour), "RpcLogic___StartCook_2166136261")]
        [HarmonyPrefix]
        public static bool RpcLogic___StartCook_2166136261(StartChemistryStationBehaviour __instance)
        {
            if (__instance.cookRoutine != null)
            {
                return false;
            }
            if (__instance.targetStation == null)
            {
                return false;
            }
            __instance.cookRoutine = (Coroutine)MelonCoroutines.Start(StartCook(__instance));
            return false;
        }

        private static IEnumerator StartCook(StartChemistryStationBehaviour instance)
        {
            instance.Npc.Movement.FacePoint(instance.targetStation.transform.position, 0.5f);
            yield return new WaitForSeconds(0.5f);
            instance.Npc.SetAnimationBool_Networked(null, "UseChemistryStation", true);
            if (!instance.CanCookStart())
            {
                instance.StopCook();
                instance.Deactivate_Networked(null);
                yield break;
            }
            instance.targetStation.SetNPCUser(instance.Npc.NetworkObject);
            StationRecipe recipe = instance.targetStation.Configuration.Cast<ChemistryStationConfiguration>().Recipe.SelectedRecipe;
            instance.SetupBeaker();
            yield return new WaitForSeconds(1f);
            instance.FillBeaker(recipe, instance.beaker);
            yield return new WaitForSeconds(20f * SkillModifiers.GetChemistActionDurationMultiplier());
            Il2CppSystem.Collections.Generic.List<ItemInstance> list = new();
            for (int i = 0; i < recipe.Ingredients.Count; i++)
            {
                foreach (ItemDefinition itemDefinition in recipe.Ingredients[i].Items)
                {
                    StorableItemDefinition storableItemDefinition = itemDefinition.Cast<StorableItemDefinition>();
                    for (int j = 0; j < instance.targetStation.IngredientSlots.Length; j++)

                    {
                        if (instance.targetStation.IngredientSlots[j].ItemInstance != null && instance.targetStation.IngredientSlots[j].ItemInstance.Definition.ID == storableItemDefinition.ID)
                        {
                            list.Add(instance.targetStation.IngredientSlots[j].ItemInstance.GetCopy(recipe.Ingredients[i].Quantity));
                            instance.targetStation.IngredientSlots[j].ChangeQuantity(-recipe.Ingredients[i].Quantity, false);
                            break;
                        }
                    }
                }
            }
            EQuality equality = recipe.CalculateQuality(list);
            instance.targetStation.SendCookOperation(new ChemistryCookOperation(recipe, equality, instance.beaker.Container.LiquidColor, instance.beaker.Fillable.LiquidContainer.CurrentLiquidLevel, 0));
            instance.beaker.Destroy();
            instance.beaker = null;
            instance.StopCook();
            instance.Deactivate_Networked(null);
            yield break;
        }

        [HarmonyPatch(typeof(StartLabOvenBehaviour), "RpcLogic___StartCook_2166136261")]
        [HarmonyPrefix]
        public static bool RpcLogic___StartCook_2166136261(StartLabOvenBehaviour __instance)
        {
            if (__instance.cookRoutine != null)
            {
                return false;
            }
            if (__instance.targetOven == null)
            {
                return false;
            }
            __instance.cookRoutine = (Coroutine)MelonCoroutines.Start(StartCook(__instance));
            return false;
        }

        private static IEnumerator StartCook(StartLabOvenBehaviour instance)
        {
            instance.targetOven.SetNPCUser(instance.Npc.NetworkObject);
            instance.Npc.Movement.FacePoint(instance.targetOven.transform.position, 0.5f);
            yield return new WaitForSeconds(0.5f);
            if (!instance.CanCookStart())
            {
                instance.StopCook();
                instance.Deactivate_Networked(null);
                yield break;
            }
            instance.targetOven.Door.SetPosition(1f);
            yield return new WaitForSeconds(0.5f);

            instance.targetOven.WireTray.SetPosition(1f);
            yield return new WaitForSeconds(5f * SkillModifiers.GetChemistActionDurationMultiplier());

            instance.targetOven.Door.SetPosition(0f);
            yield return new WaitForSeconds(1f);
            ItemInstance itemInstance = instance.targetOven.IngredientSlot.ItemInstance;
            if (itemInstance == null)
            {
                Console.LogWarning("No ingredient in oven!", null);

                instance.StopCook();
                instance.Deactivate_Networked(null);
                yield break;
            }
            int num = 1;
            if (itemInstance.Definition.Cast<StorableItemDefinition>().StationItem.GetModule<CookableModule>().CookType == CookableModule.ECookableType.Solid)
            {
                num = Mathf.Min(instance.targetOven.IngredientSlot.Quantity, 10);
            }
            itemInstance.ChangeQuantity(-num);
            string id = itemInstance.Definition.Cast<StorableItemDefinition>().StationItem.GetModule<CookableModule>().Product.ID;
            EQuality equality = EQuality.Standard;
            if (itemInstance.TryCast<QualityItemInstance>() != null)
            {
                equality = itemInstance.Cast<QualityItemInstance>().Quality;
            }
            instance.targetOven.SendCookOperation(new OvenCookOperation(itemInstance.ID, equality, num, id));
            instance.StopCook();
            instance.Deactivate_Networked(null);
            yield break;
        }

        [HarmonyPatch(typeof(StartMixingStationBehaviour), "RpcLogic___StartCook_2166136261")]
        [HarmonyPrefix]
        public static bool RpcLogic___StartCook_2166136261(StartMixingStationBehaviour __instance)
        {
            if (__instance.startRoutine != null)
            {
                return false;
            }
            if (__instance.targetStation == null)
            {
                return false;
            }
            __instance.startRoutine = (Coroutine)MelonCoroutines.Start(StartCook(__instance));
            return false;
        }

        private static IEnumerator StartCook(StartMixingStationBehaviour instance)
        {
            instance.Npc.Movement.FacePoint(instance.targetStation.transform.position, 0.5f);
            yield return new WaitForSeconds(0.5f);
            if (!instance.CanCookStart())
            {
                instance.StopCook();
                instance.Deactivate_Networked(null);
                yield break;
            }
            instance.targetStation.SetNPCUser(instance.Npc.NetworkObject);
            instance.Npc.SetAnimationBool_Networked(null, "UseChemistryStation", true);
            QualityItemInstance product = instance.targetStation.ProductSlot.ItemInstance.Cast<QualityItemInstance>();
            ItemInstance mixer = instance.targetStation.MixerSlot.ItemInstance;
            int mixQuantity = instance.targetStation.GetMixQuantity();
            int num;
            for (int i = 0; i < mixQuantity; i = num + 1)
            {
                yield return new WaitForSeconds(1f * SkillModifiers.GetChemistActionDurationMultiplier());
                num = i;
            }
            if (InstanceFinder.IsServer)
            {
                instance.targetStation.ProductSlot.ChangeQuantity(-mixQuantity, false);
                instance.targetStation.MixerSlot.ChangeQuantity(-mixQuantity, false);
                MixOperation mixOperation = new MixOperation(product.ID, product.Quality, mixer.ID, mixQuantity);
                instance.targetStation.SendMixingOperation(mixOperation, 0);
            }
            instance.StopCook();
            instance.Deactivate_Networked(null);
            yield break;
        }
    }
}

