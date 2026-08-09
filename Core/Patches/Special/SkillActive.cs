using Il2CppScheduleOne.Combat;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.Effects;
using Il2CppScheduleOne.Money;
using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.Trash;
using Il2CppScheduleOne.UI;
using MelonLoader;
using S1API.Money;
using S1API.Property;
using SkillTree.Core.Skills;
using SkillTree.Core.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SkillTree.Core.Serialization.Cooldowns;
using static SkillTree.Core.Utilities.ConfigManager;
using static SkillTree.Core.Utilities.LocalizationManager;

namespace SkillTree.Core.Patches.Special
{
    public static class SkillActive
    {
        public static void GoodSamaritan()
        {
            if (GoodSamaritanUsed)
            {
                Singleton<NotificationsManager>.Instance.SendNotification(
                    GetNotificationTitle("GoodSamaritan", "Cooldown"),
                    GetNotificationSubtitle("GoodSamaritan", "Cooldown"),
                    IconManager.LoadSprite(IconManager.IconTrashcan));
            }
            else
            {
                int total = 0;
                int count = NetworkSingleton<TrashManager>.Instance.trashItems.Count;

                if (count == 0)
                {
                    Singleton<NotificationsManager>.Instance.SendNotification(
                        GetNotificationTitle("GoodSamaritan", "InvalidUse"),
                        GetNotificationSubtitle("GoodSamaritan", "InvalidUse"),
                        IconManager.LoadSprite(IconManager.IconTrashcan));
                    return;
                }

                foreach (var item in NetworkSingleton<TrashManager>.Instance.trashItems)
                {
                    total += item.SellValue;
                }

                //if (InstanceFinder.IsServer)
                //{
                NetworkSingleton<MoneyManager>.Instance.CreateOnlineTransaction(
                    $"Payment for {count} pieces of trash destroyed",
                    total, 1f, string.Empty);

                LogManager.LogMessage($"[Special] Payment of ${total} processed for destroying {count} pieces of trash", LogLevel.Debug);
                //}

                NetworkSingleton<TrashManager>.Instance.DestroyAllTrash();
                //TrashManager.Instance.DestroyAllTrash();
                Singleton<NotificationsManager>.Instance.SendNotification(
                    GetNotificationTitle("GoodSamaritan", "Success"),
                    string.Format(GetNotificationSubtitle("GoodSamaritan", "Success"), MoneyManager.FormatAmount(total)),
                    IconManager.LoadSprite(IconManager.IconTrashcan));
                GoodSamaritanUsed = true;
            }

        }

        public static void BloodRush()
        {
            if (BloodRushUsed)
                Singleton<NotificationsManager>.Instance.SendNotification(
                    GetNotificationTitle("BloodRush", "Cooldown"),
                    GetNotificationSubtitle("BloodRush", "Cooldown"),
                    IconManager.LoadSprite(IconManager.IconHeart));
            else
            {
                Effects.BloodRush.ApplyToPlayer();
                float oldHp = Player.Local.Health.CurrentHealth;
                Player.Local.Health.RecoverHealth(SkillModifiers.GetPlayerMaxHealth());
                Singleton<NotificationsManager>.Instance.SendNotification(
                    GetNotificationTitle("BloodRush", "Success"),
                    string.Format(GetNotificationSubtitle("BloodRush", "Success"), oldHp, Player.Local.Health.CurrentHealth),
                    IconManager.LoadSprite(IconManager.IconHeart));
                BloodRushUsed = true;
            }
        }

        public static void SiphonFunds()
        {
            if (SiphonFundsUsed)
                Singleton<NotificationsManager>.Instance.SendNotification(
                    GetNotificationTitle("SiphonFunds", "Cooldown"),
                    GetNotificationSubtitle("SiphonFunds", "Cooldown"),
                    IconManager.LoadSprite(IconManager.IconCash));
            else
            {
                float totalCash = 0f;
                float totalOnlineBalance = 0f;

                foreach (Dealer dealer in Dealer.AllPlayerDealers)
                {
                    float amountConverted = dealer.Cash * SkillModifiers.GetSiphonFundsConversionMultiplier();
                    totalCash += dealer.Cash - amountConverted;
                    totalOnlineBalance += amountConverted;
                    LogManager.LogMessage($"Dealer Cash: {dealer.Cash} | Unconverted Amount: {dealer.Cash - amountConverted} | Converted Amount: {amountConverted} | Conversion Rate: {SkillModifiers.GetSiphonFundsConversionMultiplier()}", LogLevel.Debug);

                    dealer.SetCash(0f);
                }

                if (!(totalCash > 0) && !(totalOnlineBalance > 0))
                {
                    Singleton<NotificationsManager>.Instance.SendNotification(
                        GetNotificationTitle("SiphonFunds", "InvalidUse"),
                        GetNotificationSubtitle("SiphonFunds", "InvalidUse"),
                        IconManager.LoadSprite(IconManager.IconCash));
                    return;
                }

                Money.ChangeCashBalance(totalCash, true, true);

                //if (InstanceFinder.IsServer)
                //{
                NetworkSingleton<MoneyManager>.Instance.CreateOnlineTransaction(
                    $"Siphon Funds",
                    totalOnlineBalance, 1f, string.Empty);
                //}

                Singleton<NotificationsManager>.Instance.SendNotification(
                    GetNotificationTitle("SiphonFunds", "Success"),
                    string.Format(GetNotificationSubtitle("SiphonFunds", "Success"), MoneyManager.FormatAmount(totalCash), MoneyManager.FormatAmount(totalOnlineBalance)),
                    IconManager.LoadSprite(IconManager.IconCash));
                SiphonFundsUsed = true;
            }
        }

        public static void TrickleDownEconomics()
        {
            if (TrickleDownUsed)
                Singleton<NotificationsManager>.Instance.SendNotification(
                    GetNotificationTitle("TrickleDown", "Cooldown"),
                    GetNotificationSubtitle("TrickleDown", "Cooldown"),
                    IconManager.LoadSprite(IconManager.IconWashingMachine));
            else
            {
                float moneyToLaunder = Money.GetCashBalance() - TrickleDownCashReserve.GetValue(UseDefault.GetValue());
                float amountLaundered = 0;

                if (BusinessManager.GetOwnedBusinesses().Count <= 0)
                {
                    Singleton<NotificationsManager>.Instance.SendNotification(
                        GetNotificationTitle("TrickleDown", "InvalidUse1"),
                        GetNotificationSubtitle("TrickleDown", "InvalidUse1"),
                        IconManager.LoadSprite(IconManager.IconWashingMachine));
                    return;
                }

                if (moneyToLaunder <= 0)
                {
                    Singleton<NotificationsManager>.Instance.SendNotification(
                        GetNotificationTitle("TrickleDown", "InvalidUse2"),
                        string.Format(GetNotificationSubtitle("TrickleDown", "InvalidUse2"), MoneyManager.FormatAmount(TrickleDownCashReserve.GetValue(UseDefault.GetValue()))),
                        IconManager.LoadSprite(IconManager.IconWashingMachine));
                    return;
                }

                foreach (var business in BusinessManager.GetOwnedBusinesses())
                {
                    if (moneyToLaunder <= 0) break;

                    if (business.AppliedLaunderLimit > 0)
                    {
                        float amount = business.AppliedLaunderLimit <= moneyToLaunder ? business.AppliedLaunderLimit : moneyToLaunder;
                        business.AddLaunderingOperation(amount, 0);
                        amountLaundered += amount;
                        moneyToLaunder -= business.AppliedLaunderLimit;

                        Singleton<NotificationsManager>.Instance.SendNotification(
                            GetNotificationTitle("TrickleDown", "Success"),
                            string.Format(GetNotificationSubtitle("TrickleDown", "Success"), MoneyManager.FormatAmount(amount), business.PropertyName),
                            IconManager.LoadSprite(IconManager.IconWashingMachine));
                        LogManager.LogMessage($"Sent {MoneyManager.FormatAmount(amount)} to {business.PropertyName}", LogLevel.Debug);
                        TrickleDownUsed = true;
                    }
                }

                if (!TrickleDownUsed)
                {
                    Singleton<NotificationsManager>.Instance.SendNotification(
                        GetNotificationTitle("TrickleDown", "InvalidUse3"),
                        GetNotificationSubtitle("TrickleDown", "InvalidUse3"),
                        IconManager.LoadSprite(IconManager.IconWashingMachine));
                    return;
                }

                Money.ChangeCashBalance(-amountLaundered, true);
            }
        }

        public static void BloodMoney()
        {
            if (BloodMoneyUsed)
                Singleton<NotificationsManager>.Instance.SendNotification(
                    GetNotificationTitle("BloodMoney", "Cooldown"),
                    GetNotificationSubtitle("BloodMoney", "Cooldown"),
                    IconManager.LoadSprite(IconManager.IconHeart));
            else
            {
                Effects.BloodMoney.ApplyToPlayer();
                BloodMoneyUsed = true;
            }
        }

        private static readonly HashSet<string> afflicted = [];
        public static void InfectiousPersonality()
        {
            Effect[] toxicEffects = [new Toxic(), new Laxative(), new Smelly()];
            Effect[] explosiveEffects = [new Explosive(), new Spicy()];
            float toxicDelay = 8f;
            float explosionDelay = 30f;
            string EffectName = "InfectiousPersonality";

            if (InfectiousPersonalityUsed)
                Singleton<NotificationsManager>.Instance.SendNotification(
                    GetNotificationTitle(EffectName, "Cooldown"),
                    GetNotificationSubtitle(EffectName, "Cooldown"),
                    IconManager.LoadSprite(IconManager.IconHeart));
            else
            {
                Collider[] array = Physics.OverlapSphere(Player.Local.CenterPointTransform.position, InfectiousPersonalityRange.GetValue(UseDefault.GetValue()));
                foreach (var item in array)
                {
                    NPC npc = item.GetComponentInParent<NPC>();
                    if (npc != null && !npc.Health.IsDead && !npc.Health.IsKnockedOut)
                    {
                        int num = Random.RandomRangeInt(0, 100);
                        if (num < 50)
                        {
                            Infect(npc, toxicEffects, toxicDelay);
                        }
                        else
                        {
                            Infect(npc, explosiveEffects, explosionDelay);
                        }
                        InfectiousPersonalityUsed = true;
                    }
                }

                if (!InfectiousPersonalityUsed)
                {
                    Singleton<NotificationsManager>.Instance.SendNotification(
                        GetNotificationTitle(EffectName, "InvalidUse"),
                        GetNotificationSubtitle(EffectName, "InvalidUse"),
                        IconManager.LoadSprite(IconManager.IconHeart));
                }
            }

            void Infect(NPC npc, Effect[] effects, float delay)
            {
                if (npc == null || npc.Health.IsDead || npc.Health.IsKnockedOut || afflicted.Contains(npc.name)) return;

                afflicted.Add(npc.name);
                npc.Behaviour.activeBehaviour.Pause();
                npc.Behaviour.CombatBehaviour.Pause();
                npc.Behaviour.CoweringBehaviour.Enable();
                npc.Behaviour.CoweringBehaviour.Activate();

                foreach (Effect effect in effects)
                {
                    effect.ApplyToNPC(npc);
                }

                Core.AddCoroutine(EffectName + npc.FullName, MelonCoroutines.Start(SpreadInfection(npc, effects, delay)));
            }

            IEnumerator SpreadInfection(NPC npc, Effect[] effects, float delay)
            {
                yield return new WaitForSeconds(delay);
                foreach (Effect effect in effects)
                {
                    effect.ClearFromNPC(npc);
                }
                InfectArea(npc.CenterPoint, InfectiousPersonalityRange.GetValue(UseDefault.GetValue()), effects, delay);
                Money.ChangeCashBalance(npc.Health.MaxHealth / 2);
                NetworkSingleton<MoneyManager>.Instance.CreateOnlineTransaction(
                    $"Infectious Personality payment",
                    npc.Health.MaxHealth / 2, 1f, string.Empty);
                npc.ReceiveImpact(new Impact(Vector3.zero, Vector3.zero, 0f, npc.Health.MaxHealth, EImpactType.Explosion, Player.Local.NetworkObject));
                Core.RemoveCoroutine(EffectName + npc.FullName);
            }

            void InfectArea(Vector3 source, float radius, Effect[] effects, float delay)
            {
                Collider[] array = Physics.OverlapSphere(source, radius);

                foreach (var item in array)
                {
                    NPC npc = item.GetComponentInParent<NPC>();
                    if (npc != null && !npc.Health.IsDead && !npc.Health.IsKnockedOut)
                    {
                        Infect(npc, effects, delay);
                    }
                }
            }
        }

        public static void AdrenalineSurge()
        {
            if (Effects.AdrenalineSurge.IsAdrenalineSurgeActive)
            {
                Singleton<NotificationsManager>.Instance.SendNotification(
                    GetNotificationTitle("AdrenalineSurge", "NotExpired"),
                    GetNotificationSubtitle("AdrenalineSurge", "NotExpired"),
                    IconManager.LoadSprite(IconManager.IconHeart));
                return;
            }

            if (AdrenalineSurgeRemainingCharges > 0)
            {
                Effects.AdrenalineSurge.ApplyToPlayer();
                AdrenalineSurgeRemainingCharges--;
            }
            Singleton<NotificationsManager>.Instance.SendNotification(
                GetNotificationTitle("AdrenalineSurge", "Charges"),
                string.Format(GetNotificationSubtitle("AdrenalineSurge", "Charges"), AdrenalineSurgeRemainingCharges),
                IconManager.LoadSprite(IconManager.IconHeart));
        }

        public static void AntiGravityBong()
        {
            if (!AntiGravityBongUsed)
            {
                AntiGravityBongUsed = true;
                Core.AddCoroutine("AntiGravityBong", MelonCoroutines.Start(SpawnBong()));
                Core.AddCoroutine("ResetAntiGravityBong", MelonCoroutines.Start(ResetAntiGravityBong()));
            }
            else
            {
                Singleton<NotificationsManager>.Instance.SendNotification(
                    GetNotificationTitle("AntiGravityBong", "Cooldown"),
                    GetNotificationSubtitle("AntiGravityBong", "Cooldown"),
                    IconManager.LoadSprite(IconManager.IconClock));
            }

            static IEnumerator SpawnBong()
            {
                TrashItem bong = NetworkSingleton<TrashManager>.Instance.CreateTrashItem("bong", Player.Local.CameraPosition, Random.rotation, default, "", false);
                bong.SetPhysicsActive(false);
                bong.CanGoInContainer = false;
                for (int i = 0; i < AntiGravityBongDuration.GetValue(UseDefault.GetValue()); i++)
                {
                    float radius = i == AntiGravityBongRadius.GetValue(UseDefault.GetValue()) - 2 ?
                        AntiGravityBongRadius.GetValue(UseDefault.GetValue()) * 1.5f :
                        AntiGravityBongRadius.GetValue(UseDefault.GetValue());

                    Collider[] array = Physics.OverlapSphere(bong.transform.position, radius);
                    foreach (var item in array)
                    {
                        NPC npc = item.GetComponentInParent<NPC>();
                        if (npc != null)
                        {
                            if (i >= AntiGravityBongDuration.GetValue(UseDefault.GetValue()) - 2)
                            {
                                npc.Movement.ActivateRagdoll_Server(bong.transform.position, (bong.transform.position - npc.CenterPoint).normalized + new Vector3(0f, 0.15f, 0f), 250f);
                            }
                            else
                            {
                                npc.Movement.ActivateRagdoll_Server(bong.transform.position, (npc.CenterPoint - bong.transform.position).normalized + new Vector3(0f, 0.5f, 0f), 100f);
                            }
                        }
                    }
                    yield return new WaitForSeconds(1f);
                }
                NetworkSingleton<CombatManager>.Instance.CreateExplosion(bong.transform.position, ExplosionData.DefaultSmall);
                bong.DestroyTrash();
                Core.RemoveCoroutine("AntiGravityBong");
            }
        }

        public static void ResetAfflicted()
        {
            afflicted.Clear();
        }
    }
}
