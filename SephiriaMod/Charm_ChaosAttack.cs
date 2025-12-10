using System;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

namespace SephiriaMod
{
    public class Charm_ChaosAttack : Charm_VariableMaxLevel
    {
        public int[] chanceByLevel = new int[10] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };
        public int[] percentByLevel = new int[10] { 10, 20, 25, 30, 35, 40, 50, 60, 70, 80 };
        public string damageId = "Charm_ChaosAttack";
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = (showAllLevel ? ((chanceByLevel.SafeRandomAccess(0)) + "→" + (chanceByLevel.SafeRandomAccess(maxLevel))) : (chanceByLevel.SafeRandomAccess(LevelToIdx(level))).ToString());
            string value2 = ((avatar.GetCustomStat("FireDamage") + avatar.GetCustomStat("IceDamage") + avatar.GetCustomStat("LightningDamage")) * percentByLevel.SafeRandomAccess(LevelToIdx(level)) / 100).ToString();
            string value3 = (showAllLevel ? ((percentByLevel.SafeRandomAccess(0)) + "→" + (percentByLevel.SafeRandomAccess(maxLevel))) : (percentByLevel.SafeRandomAccess(LevelToIdx(level))).ToString());
            return new Loc.KeywordValue[3]
            {
            new Loc.KeywordValue("CHANCE", value + "%", Charm_Basic.GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("PERCENT", value3 + "%", Charm_Basic.GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("DAMAGE", value2, Charm_Basic.GetPositiveColor(virtualLevelOffset))
            };
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            UnitAvatar networkAvatar = base.NetworkAvatar;
            networkAvatar.OnAttackUnit += OnAttackUnit;
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            UnitAvatar networkAvatar = base.NetworkAvatar;
            networkAvatar.OnAttackUnit -= OnAttackUnit;
        }

        private void OnAttackUnit(UnitAvatar unitAvatar, DamageInstance damageInstance)
        {
            if (!base.NetworkAvatar.IsDead && IsEffectEnabled && damageInstance.id != damageId && damageInstance.fromType == EDamageFromType.DirectAttack)
            {
                float customStatUnsafe = base.NetworkAvatar.GetCustomStat("FireDamage") + base.NetworkAvatar.GetCustomStat("IceDamage") + base.NetworkAvatar.GetCustomStat("LightningDamage");
                customStatUnsafe *= percentByLevel.SafeRandomAccess(CurrentLevelToIdx()) / 100f;
                if (customStatUnsafe > 0 && unitAvatar != null && !(UnityEngine.Random.Range(0f, 1f) > chanceByLevel.SafeRandomAccess(CurrentLevelToIdx()) / 100f))
                {
                    DamageInstance damage = DamageInstance.GetDamage(base.NetworkAvatar, damageId, unitAvatar.transform.position, 4294967295L, customStatUnsafe, EDamageType.ElementalEffectDamage, EDamageFromType.None, Vector2.zero, 0, 0f);
                    damage.SetCustomColor(true, new Color(0.5f, 0, 0));
                    unitAvatar.ApplyDamage(damage);
                }
            }
        }
    }
}
