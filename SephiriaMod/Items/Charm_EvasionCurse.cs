using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SephiriaMod.Items
{
    public class Charm_EvasionCurse : Charm_StatusInstance
    {
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            if(avatar == null || ignoreAvatarStatus)
            {
                return new Loc.KeywordValue[1]
                {
            new Loc.KeywordValue("PERCENT", "-", GetPositiveColor(virtualLevelOffset))
                };
            }
            float percent = GetAssasinateRate(avatar.GetCustomStat(ECustomStat.Evasion));
            string value = percent.ToString(".##");
            return new Loc.KeywordValue[1]
            {
            new Loc.KeywordValue("PERCENT", value, GetPositiveColor(virtualLevelOffset))
            };
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.AddCustomStatUnsafe("ABSOLUTEEVASION", -10000);
            NetworkAvatar.OnAttackUnitBeforeOperation += OnAttackUnitBeforeOperation;
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.AddCustomStatUnsafe("ABSOLUTEEVASION", 10000);
            NetworkAvatar.OnAttackUnitBeforeOperation -= OnAttackUnitBeforeOperation;
        }
        public override void OnCharmEffectRefreshed()
        {
            base.OnCharmEffectRefreshed();
            NetworkAvatar.OnAttackUnitBeforeOperation -= OnAttackUnitBeforeOperation;
            NetworkAvatar.OnAttackUnitBeforeOperation += OnAttackUnitBeforeOperation;
        }

        public float GetAssasinateRate(int evasion)
        {
            if (evasion > 10000)
                evasion = 10000;
            return 100f * Mathf.Log(evasion / 6200f + 1f) * 0.2f;
        }
        private void OnAttackUnitBeforeOperation(UnitAvatar avatar, DamageInstance damage)
        {
            var evasion = NetworkAvatar.GetCustomStat(ECustomStat.Evasion);
            float rate = GetAssasinateRate(evasion);
            var assassinate = Mathf.Clamp(rate, 0f, 100f);
            //Melon<Core>.Logger.Msg("暗閃: " + assassinate + "%");
            if (Random.Range(0f, 100f) < assassinate)
            {
                damage.criticalChancePercent = 0;
                damage.color = new Color(1, 0, 0, 0);
                damage.useCustomColor = true;
                damage.damage += avatar.MaxHp * 0.1f * avatar.Debuffs.Count();
                //damage.ignoreDefense += ignoreDegence.SafeRandomAccess(CurrentLevelToIdx());
                //Melon<Core>.Logger.Msg("暗閃!: " + (max / min));
            }
        }
    }
}
