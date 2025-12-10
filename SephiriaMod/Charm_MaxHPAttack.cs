using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod
{
    internal class Charm_MaxHPAttack : Charm_StatusInstance
    {

        public int[] damageByLevel = new int[6] { 3, 4, 5, 6, 7, 8 };
        public string damageId = "Charm_MaxHPAttack";
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            var maxHP = avatar.MaxHp;
            string value = (showAllLevel ? (damageByLevel.SafeRandomAccess(0) + "→" + damageByLevel.SafeRandomAccess(maxLevel)) : damageByLevel.SafeRandomAccess(LevelToIdx(level)).ToString());
            string value2 = (showAllLevel ? (Mathf.RoundToInt(maxHP * 0.01f * damageByLevel.SafeRandomAccess(0)) + "→" + Mathf.RoundToInt(maxHP * 0.01f * damageByLevel.SafeRandomAccess(maxLevel))) : Mathf.RoundToInt(maxHP * 0.01f * damageByLevel.SafeRandomAccess(LevelToIdx(level))).ToString());
            return new Loc.KeywordValue[2]
            {
            new Loc.KeywordValue("PERCENT", value, Charm_Basic.GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("DAMAGE", value2, Charm_Basic.GetPositiveColor(virtualLevelOffset))
            };
        }
        protected override void OnConnected(int instanceID)
        {
            base.OnConnected(instanceID);
            UnitAvatar networkAvatar = base.NetworkAvatar;
            networkAvatar.OnAttackUnit += OnAttackUnit;
        }

        protected override void OnDisconnected()
        {
            base.OnDisconnected();
            UnitAvatar networkAvatar = base.NetworkAvatar;
            networkAvatar.OnAttackUnit -= OnAttackUnit;
        }

        private void OnAttackUnit(UnitAvatar unitAvatar, DamageInstance damageInstance)
        {
            if (!base.NetworkAvatar.IsDead && IsEffectEnabled && damageInstance.id != damageId)
            {
                float customStatUnsafe = base.NetworkAvatar.MaxHp * (damageByLevel.SafeRandomAccess(CurrentLevelToIdx()) * 0.01f);
                if (customStatUnsafe > 0 && unitAvatar != null)
                {
                    DamageInstance damage = DamageInstance.GetDamage(base.NetworkAvatar, damageId, unitAvatar.transform.position, 4294967295L, customStatUnsafe, EDamageType.ElementalEffectDamage, EDamageFromType.None, Vector2.zero, 0, 0f);
                    //damage.elementalType = EDamageElementalType.Chaos;
                    damage.SetCustomColor(true, new Color(1f, 0.7f, 0.7f));
                    unitAvatar.ApplyDamage(damage);
                }
            }
        }

    }
}
