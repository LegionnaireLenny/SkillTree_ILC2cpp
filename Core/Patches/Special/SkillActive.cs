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

namespace SkillTree.Core.Patches.Special
{
    public static class SkillActive
    {
        public static void GoodSamaritan()
        {
            if (GoodSamaritanUsed)
            {
                Singleton<NotificationsManager>.Instance.SendNotification(
                    "Good Samaritan on Cooldown",
                    $"<color=#FF0000>Wait one day</color>",
                    IconManager.LoadSprite(IconManager.IconTrashcan));
            }
            else
            {
                int total = 0;
                int count = NetworkSingleton<TrashManager>.Instance.trashItems.Count;

                if (count == 0)
                {
                    Singleton<NotificationsManager>.Instance.SendNotification(
                        "Good Samaritan",
                        "No trash found",
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

                MelonLogger.Msg($"[Special] Payment of ${total} processed for destroying {count} pieces of trash");
                //}

                NetworkSingleton<TrashManager>.Instance.DestroyAllTrash();
                //TrashManager.Instance.DestroyAllTrash();
                Singleton<NotificationsManager>.Instance.SendNotification(
                    "Good Samaritan",
                    $"Earned <color=#4CBFFF>{MoneyManager.FormatAmount(total)}</color>",
                    IconManager.LoadSprite(IconManager.IconTrashcan));
                GoodSamaritanUsed = true;
            }

        }

        public static void BloodRush()
        {
            if (BloodRushUsed)
                Singleton<NotificationsManager>.Instance.SendNotification(
                                "Blood Rush on Cooldown",
                                $"<color=#FF0000>Wait one day</color>",
                                IconManager.LoadSprite(IconManager.IconHeart));
            else
            {
                Effects.BloodRush.ApplyToPlayer();
                float oldHp = Player.Local.Health.CurrentHealth;
                Player.Local.Health.RecoverHealth(SkillModifiers.GetPlayerMaxHealth());
                Singleton<NotificationsManager>.Instance.SendNotification(
                                "Blood Rush",
                                $"<color=#FF0000>{oldHp}</color> to <color=#FF0000>{Player.Local.Health.CurrentHealth}</color>",
                                IconManager.LoadSprite(IconManager.IconHeart));
                BloodRushUsed = true;
            }
        }

        public static void SiphonFunds()
        {
            if (SiphonFundsUsed)
                Singleton<NotificationsManager>.Instance.SendNotification(
                                "Siphon Funds on Cooldown",
                                "<color=#FF0000>Wait one day</color>",
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
                    //MelonLogger.Msg($"Dealer Cash: {dealer.Cash} | Unconverted Amount: {dealer.Cash - amountConverted} | Converted Amount: {amountConverted} | Conversion Rate: {SkillModifiers.GetSiphonFundsConversionMultiplier()}");

                    dealer.SetCash(0f);
                }

                if (!(totalCash > 0) && !(totalOnlineBalance > 0))
                {
                    Singleton<NotificationsManager>.Instance.SendNotification(
                        "Siphon Funds",
                        $"Dealers had no funds",
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
                    "Siphon Funds",
                    $"<color=#54E717>{MoneyManager.FormatAmount(totalCash)}</color> and <color=#4CBFFF>{MoneyManager.FormatAmount(totalOnlineBalance)}</color>",
                    IconManager.LoadSprite(IconManager.IconCash));
                SiphonFundsUsed = true;
            }
        }

        public static void TrickleDownEconomics()
        {
            if (TrickleDownUsed)
                Singleton<NotificationsManager>.Instance.SendNotification(
                    "Trickle-down on Cooldown",
                    "<color=#FF0000>Wait one day</color>",
                    IconManager.LoadSprite(IconManager.IconWashingMachine));
            else
            {
                float moneyToLaunder = Money.GetCashBalance() - ConfigManager.TrickleDownCashReserve.GetValue();
                float amountLaundered = 0;

                if (BusinessManager.GetOwnedBusinesses().Count <= 0)
                {
                    Singleton<NotificationsManager>.Instance.SendNotification(
                        "Trickle-down Economics",
                        $"No owned businesses",
                        IconManager.LoadSprite(IconManager.IconWashingMachine));
                    return;
                }

                if (moneyToLaunder <= 0)
                {
                    Singleton<NotificationsManager>.Instance.SendNotification(
                        "Trickle-down Economics",
                        $"<color=#FF0000>Cash does not exceed {MoneyManager.FormatAmount(ConfigManager.TrickleDownCashReserve.GetValue())} reserve</color>",
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
                            "Trickle-down Economics",
                            $"<color=#54E717>{MoneyManager.FormatAmount(amount)}</color> to {business.PropertyName}",
                            IconManager.LoadSprite(IconManager.IconWashingMachine));
                        MelonLogger.Msg($"Sent {MoneyManager.FormatAmount(amount)} to {business.PropertyName}");
                        TrickleDownUsed = true;
                    }
                }

                if (!TrickleDownUsed)
                {
                    Singleton<NotificationsManager>.Instance.SendNotification(
                        "Trickle-down Economics",
                        $"No laundering capacity",
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
                                "Blood Money on Cooldown",
                                $"<color=#FF0000>Wait one day</color>",
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

            if (InfectiousPersonalityUsed)
                Singleton<NotificationsManager>.Instance.SendNotification(
                                "Infectious Personality on Cooldown",
                                $"<color=#FF0000>Wait one day</color>",
                                IconManager.LoadSprite(IconManager.IconHeart));
            else
            {
                Collider[] array = Physics.OverlapSphere(Player.Local.CenterPointTransform.position, ConfigManager.InfectiousPersonalityRange.GetValue());
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
                        "Infectious Personality",
                        $"No one found nearby",
                        IconManager.LoadSprite(IconManager.IconHeart));
                }
            }

            void Infect(NPC npc, Effect[] effects, float delay)
            {
                if (npc.Health.IsDead || npc.Health.IsKnockedOut || afflicted.Contains(npc.name)) return;

                afflicted.Add(npc.name);
                npc.Behaviour.activeBehaviour.Pause();
                npc.Behaviour.CombatBehaviour.Pause();
                npc.Behaviour.CoweringBehaviour.Enable();
                npc.Behaviour.CoweringBehaviour.Activate();

                foreach (Effect effect in effects)
                {
                    effect.ApplyToNPC(npc);
                }

                MelonCoroutines.Start(SpreadInfection(npc, effects, delay));
            }

            IEnumerator SpreadInfection(NPC npc, Effect[] effects, float delay)
            {
                yield return new WaitForSeconds(delay);
                foreach (Effect effect in effects)
                {
                    effect.ClearFromNPC(npc);
                }
                InfectArea(npc.CenterPoint, ConfigManager.InfectiousPersonalityRange.GetValue(), effects, delay);
                Money.ChangeCashBalance(npc.Health.MaxHealth / 2);
                NetworkSingleton<MoneyManager>.Instance.CreateOnlineTransaction(
                    $"Infectious Personality payment",
                    npc.Health.MaxHealth / 2, 1f, string.Empty);
                npc.ReceiveImpact(new Impact(Vector3.zero, Vector3.zero, 0f, npc.Health.MaxHealth, EImpactType.Explosion, Player.Local.NetworkObject));
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
            if (AdrenalineSurgeCharges > 0)
            {
                Effects.AdrenalineSurge.ApplyToPlayer();
                AdrenalineSurgeCharges--;
            }
            Singleton<NotificationsManager>.Instance.SendNotification(
                "Adrenaline Surge",
                $"{AdrenalineSurgeCharges} charges left",
                IconManager.LoadSprite(IconManager.IconHeart));
        }

        public static void AntiGravityBong()
        {
            if (!AntiGravityBongUsed)
            {
                AntiGravityBongUsed = true;
                MelonCoroutines.Start(SpawnBong());
                MelonCoroutines.Start(ResetAntiGravityBong());
            }
            else
            {
                Singleton<NotificationsManager>.Instance.SendNotification(
                    "Anti-Gravity Bong",
                    $"On cooldown",
                    IconManager.LoadSprite(IconManager.IconClock));
            }

            IEnumerator SpawnBong()
            {
                TrashItem bong = NetworkSingleton<TrashManager>.Instance.CreateTrashItem("bong", Player.Local.CameraPosition, Random.rotation, default, "", false);
                bong.SetPhysicsActive(false);
                for (int i = 0; i < ConfigManager.AntiGravityBongDuration.GetValue(); i++)
                {
                    float radius = i == ConfigManager.AntiGravityBongRadius.GetValue() - 2 ? ConfigManager.AntiGravityBongRadius.GetValue() * 1.5f : ConfigManager.AntiGravityBongRadius.GetValue();
                    Collider[] array = Physics.OverlapSphere(bong.transform.position, radius);
                    foreach (var item in array)
                    {
                        NPC npc = item.GetComponentInParent<NPC>();
                        if (npc != null)
                        {
                            if (i >= ConfigManager.AntiGravityBongDuration.GetValue() - 2)
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
            }
        }

        public static void ResetAfflicted()
        {
            afflicted.Clear();
        }
    }
}
