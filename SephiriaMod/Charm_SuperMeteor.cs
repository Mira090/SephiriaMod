using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod
{
    public class Charm_SuperMeteor : Charm_StatusInstance
    {
        public string damageId = "Charm_SuperMeteor";

        public int[] speed = [50, 60, 70, 80, 90, 100];
        public int[] damage = [32, 48, 64, 70, 96, 112];
        public int[] percent = [0, 0, 0, 0, 0, 30];
        public int[] meteor = [300];

        public Timer cooldownTimer = new Timer(1f, resetOnTime: false);
        private bool isInCooldown = false;
        private void Awake()
        {
            showStatsEffectStringFirst = true;
        }
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            float num = 1f;
            if ((bool)avatar && avatar.TryGetComponent<WeaponControllerSimple>(out var component) && (bool)component.currentWeapon)
            {
                num = component.currentWeapon.AttackWeightPerSwing;
            }
            string value = (showAllLevel ? (speed.SafeRandomAccess(0) + "→" + speed.SafeRandomAccess(maxLevel)) : speed.SafeRandomAccess(LevelToIdx(level)).ToString());
            string value2 = (showAllLevel ? (damage.SafeRandomAccess(0) + "→" + damage.SafeRandomAccess(maxLevel)) : damage.SafeRandomAccess(LevelToIdx(level)).ToString());
            string value3 = (showAllLevel ? ((percent.SafeRandomAccess(0) * num).ToString("0.##") + "→" + (percent.SafeRandomAccess(maxLevel) * num).ToString("0.##")) : (percent.SafeRandomAccess(LevelToIdx(level)) * num).ToString("0.##"));
            string value4 = (showAllLevel ? (meteor.SafeRandomAccess(0) + "→" + meteor.SafeRandomAccess(maxLevel)) : meteor.SafeRandomAccess(LevelToIdx(level)).ToString());
            return new Loc.KeywordValue[4]
            {
                new Loc.KeywordValue("SPEED", "+" + value + "%", Charm_Basic.GetPositiveColor(virtualLevelOffset)),
                new Loc.KeywordValue("DAMAGE", "+" + value2 + "%", Charm_Basic.GetPositiveColor(virtualLevelOffset)),
                new Loc.KeywordValue("METEOR", value4 + "%", Charm_Basic.GetPositiveColor(virtualLevelOffset)),
                new Loc.KeywordValue("PERCENT", value3 + "%", Charm_Basic.GetPositiveColor(virtualLevelOffset))
            };
        }
        public override string GetEffectString(int idx, int level, int virtualLevelOffset, bool showAllLevel)
        {
            if (LevelToIdx(level) < maxLevel && idx == 3)
                return null;
            return base.GetEffectString(idx, level, virtualLevelOffset, showAllLevel);
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            //NetworkAvatar.OnAddedDebuffOnTarget += OnAddedDebuffOnTarget;
            NetworkAvatar.OnAttackUnit += OnAttackUnit;
            NetworkAvatar.AddCustomStatUnsafe("METEORSPEED", speed.SafeRandomAccess(CurrentLevelToIdx()));
            NetworkAvatar.AddCustomStatUnsafe("METEORDAMAGE", damage.SafeRandomAccess(CurrentLevelToIdx()));
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            //NetworkAvatar.OnAddedDebuffOnTarget -= OnAddedDebuffOnTarget;
            NetworkAvatar.OnAttackUnit -= OnAttackUnit;
            NetworkAvatar.AddCustomStatUnsafe("METEORSPEED", -speed.SafeRandomAccess(CurrentLevelToIdx()));
            NetworkAvatar.AddCustomStatUnsafe("METEORDAMAGE", -damage.SafeRandomAccess(CurrentLevelToIdx()));
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (isInCooldown && cooldownTimer.Update(Time.deltaTime * (1 + (100 + NetworkAvatar.GetCustomStat(ECustomStat.AttackSpeed)) / 100f)))
            {
                isInCooldown = false;
            }
        }
        private void OnAttackUnit(UnitAvatar avatar, DamageInstance damage)
        {
            if (damage.id == "Debuff_Burn" || isInCooldown)
                return;
            if (SephiriaPrefabs.MeteorBullet == null)
                return;
            float num = 1f;
            if ((bool)NetworkAvatar && NetworkAvatar.TryGetComponent<WeaponControllerSimple>(out var component) && (bool)component.currentWeapon)
            {
                num = component.currentWeapon.AttackWeightPerSwing;
            }
            if (UnityEngine.Random.Range(0f, 100f) < percent.SafeRandomAccess((CurrentLevelToIdx())) * num)
            {
                SpawnMeteor();
            }
        }

        protected override void OnUpdatedLevel(int oldLevel, int newLevel)
        {
            base.OnUpdatedLevel(oldLevel, newLevel);
            NetworkAvatar.AddCustomStatUnsafe("METEORSPEED", -speed.SafeRandomAccess(LevelToIdx(oldLevel)));
            NetworkAvatar.AddCustomStatUnsafe("METEORDAMAGE", -damage.SafeRandomAccess(LevelToIdx(oldLevel)));
            NetworkAvatar.AddCustomStatUnsafe("METEORSPEED", speed.SafeRandomAccess(LevelToIdx(newLevel)));
            NetworkAvatar.AddCustomStatUnsafe("METEORDAMAGE", damage.SafeRandomAccess(LevelToIdx(newLevel)));
        }
        private List<UnitAvatar> SearchTarget(Vector3 aimPosition)
        {
            List<UnitAvatar> list = new List<UnitAvatar>();
            foreach (UnitAvatar allCreature in CombatManager.Instance.AllCreatures)
            {
                if ((bool)allCreature && !allCreature.IsDead && !(allCreature == base.NetworkAvatar) && !allCreature.canBeTarget.IsFalse() && !(allCreature.TopdownActor.YPos > 5f) && allCreature.gameObject.activeSelf && CombatManager.ContainsAttackableFaction(allCreature.GetHostileFactionLayers(EDamageFromType.None), base.NetworkAvatar.faction) && !(Vector3.Distance(allCreature.transform.position, aimPosition) > 8f))
                {
                    list.Add(allCreature);
                }
            }

            return list;
        }
        private void OnAddedDebuffOnTarget(CharacterDebuff debuff, string id)
        {
            if (debuff.ID != "BURN" || isInCooldown)
                return;
            if (SephiriaPrefabs.MeteorBullet == null)
                return;
            float num = 1f;
            if ((bool)NetworkAvatar && NetworkAvatar.TryGetComponent<WeaponControllerSimple>(out var component) && (bool)component.currentWeapon)
            {
                num = component.currentWeapon.AttackWeightPerSwing;
            }
            if (UnityEngine.Random.Range(0f, 100f) < percent.SafeRandomAccess((CurrentLevelToIdx())) * num)
            {
                SpawnMeteor();
            }
        }
        private void SpawnMeteor()
        {
            List<UnitAvatar> list = SearchTarget(base.WeaponController.aimedPositionClientside);
            Vector2 vector;
            if (list.Count > 0)
            {
                int index = UnityEngine.Random.Range(0, list.Count);
                vector = list[index].transform.position;
                list.RemoveAt(index);
            }
            else
            {
                vector = (Vector2)base.NetworkAvatar.transform.position + UnityEngine.Random.insideUnitCircle * 4f;
            }

            float x = ((UnityEngine.Random.Range(0, 2) == 0) ? 2f : (-2f));
            float damage = (float)base.NetworkAvatar.GetCustomStat(ECustomStat.FireDamage) * ((float)meteor.SafeRandomAccess(CurrentLevelToIdx()) / 100f);
            Bullet.Pool.Spawn(SephiriaPrefabs.MeteorBullet, vector, canBeTransparentOnMultiplayer: true, EDamageFromType.None, damageId, damage, 25, 3f, base.NetworkAvatar, base.NetworkAvatar.GetHostileFactionLayers(EDamageFromType.None), 4.5f, vector + new Vector2(x, 0f), vector, null, HandleAttack, 0f, EDamageElementalType.Fire);
        }
        private void HandleAttack(CombatBehaviour behaviour, DamageInstance instance, ProjectileBase @base)
        {
            if (behaviour is UnitAvatar unitAvatar && !instance.ignoreDebuff)
            {
                //unitAvatar.ApplyDebuff(SephiriaPrefabs.Burn, base.NetworkAvatar);
            }
        }

        [HarmonyPatch(typeof(Bullet), nameof(Bullet.OnSpawnFinalized))]
        public static class OnSpawnPatch
        {
            static void Postfix(Bullet __instance)
            {
                if(__instance.gameObject.name == "Meteor(Clone)")
                {
                    //__instance.SetSpeedScale(__instance.speedScale * (1 + __instance.Owner.GetCustomStatUnsafe("METEORSPEED") / 100f));
                    __instance.SetDamage(__instance.Damage * (1 + __instance.Owner.GetCustomStatUnsafe("METEORDAMAGE") / 100f));
                    if(__instance.gameObject.TryGetComponent<BulletMoveModule_Meteor>(out var meteor))
                    {
                        meteor.fallingTimer.time *= 1f / (1 + __instance.Owner.GetCustomStatUnsafe("METEORSPEED") / 100f);
                    }
                }
            }
        }
    }
}
