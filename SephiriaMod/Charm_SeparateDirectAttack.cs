using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod
{
    public class Charm_SeparateDirectAttack : Charm_StatusInstance
    {
        public int[] separate = [56, 52, 48, 42, 36];
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = (showAllLevel ? (separate.SafeRandomAccess(0) + "→" + separate.SafeRandomAccess(maxLevel)) : separate.SafeRandomAccess(LevelToIdx(level)).ToString());
            return new Loc.KeywordValue[1]
            {
            new Loc.KeywordValue("DAMAGE", value, Charm_Basic.GetPositiveColor(virtualLevelOffset))
            };
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.OnAttackUnitBeforeOperation += OnAttackUnitBeforeOperation;
        }

        private void OnAttackUnitBeforeOperation(UnitAvatar avatar, DamageInstance damage)
        {
            if(damage.fromType == EDamageFromType.DirectAttack && damage.damage >= separate.SafeRandomAccess(CurrentLevelToIdx()))
            {
                damage.damage -= damage.damage / 2f;
                var color = damage.useCustomColor;
                if (!color)
                    damage.SetCustomColor(false, new Color(0.25f, 0.25f, 0.8f));

                float customStatUnsafe = damage.damage;
                if (customStatUnsafe > 0 && avatar != null && !avatar.IsDead)
                {
                    DamageInstance d = DamageInstance.GetDamage(base.NetworkAvatar, damage.id, avatar.transform.position, 4294967295L, customStatUnsafe, damage.damageType, damage.fromType, damage.direction, damage.staggeringLevel, damage.externalForcePower);
                    d.elementalType = damage.elementalType;
                    if (!color)
                        d.SetCustomColor(false, new Color(0.8f, 0.25f, 0.25f));
                    else
                        d.SetCustomColor(false, damage.color);
                    avatar.ApplyDamage(d);
                }
            }
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.OnAttackUnitBeforeOperation -= OnAttackUnitBeforeOperation;
        }
    }
}
