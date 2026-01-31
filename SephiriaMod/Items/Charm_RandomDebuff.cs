using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Items
{
    public class Charm_RandomDebuff : Charm_StatusInstance
    {
        public Timer cooldownTimer = new Timer(1f);
        public bool isInCooldown;
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            if (avatar == null || ignoreAvatarStatus)
            {
                return new Loc.KeywordValue[2]
                {
                    new Loc.KeywordValue("PERCENT", "-" + "%"),
                new Loc.KeywordValue("COOLDOWN", cooldownTimer.time.ToString())
                };
            }
            float percent = GetDebuffChance(avatar.GetCustomStat(ECustomStat.Luck));
            string value = percent.ToString(".##");
            return new Loc.KeywordValue[2]
            {
                new Loc.KeywordValue("PERCENT", value + "%"),
                new Loc.KeywordValue("COOLDOWN", cooldownTimer.time.ToString())
            };
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.OnAttackUnit += OnAttackUnit;
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.OnAttackUnit -= OnAttackUnit;
        }
        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (isInCooldown && cooldownTimer.Update(Time.deltaTime))
            {
                isInCooldown = false;
            }
        }

        public float GetDebuffChance(int luck)
        {
            luck *= 100;
            return 100f * Mathf.Log(luck / 6200f + 1f) * 0.2f;
        }
        public CharacterDebuff GetDebuff(UnitAvatar target)
        {
            var list = new List<CharacterDebuff>
            {
                SephiriaPrefabs.Burn,
                SephiriaPrefabs.Frostbite,
                SephiriaPrefabs.Electric,
                SephiriaPrefabs.Wound,
                SephiriaPrefabs.Poison
            };
            foreach (var debuff in target.Debuffs)
            {
                if(debuff.ID == SephiriaPrefabs.Poison.ID)
                {
                    list.Remove(debuff);
                }
                else if(debuff.CurrentStack == debuff.MaxStackCount)
                {
                    list.Remove(debuff);
                }
            }

            return list.GetRandom();
        }
        private void OnAttackUnit(UnitAvatar avatar, DamageInstance damage)
        {
            if (damage.fromType != EDamageFromType.DirectAttack)
                return;
            var chance = GetDebuffChance(NetworkAvatar.GetCustomStat(ECustomStat.Luck));
            if (IsEffectEnabled && !isInCooldown && chance.Percent())
            {
                isInCooldown = true;
                avatar.ApplyDebuff(GetDebuff(avatar), NetworkAvatar);
            }
        }
    }
}
