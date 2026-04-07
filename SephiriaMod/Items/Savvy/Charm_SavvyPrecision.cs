using HeathenEngineering.SteamworksIntegration.API;
using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaMod.Items.Savvy
{
    public class Charm_SavvyPrecision : Charm_StatusInstance
    {
        public int[] moneyByLevel = [5, 5, 10];
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.OnAttackUnit += OnAttackUnit;
        }
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? moneyByLevel.SafeRandomAccess(0) + "→" + moneyByLevel.SafeRandomAccess(maxLevel) : moneyByLevel.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[1]
            {
            new Loc.KeywordValue("LEAF", "+" + value, GetPositiveColor(virtualLevelOffset))
            };
        }

        private void OnAttackUnit(UnitAvatar avatar, DamageInstance damage)
        {
            if (damage.isCriticalAttack)
            {
                var add = moneyByLevel.SafeRandomAccess(CurrentLevelToIdx());
                //add += add * NetworkAvatar.GetCustomStat(ECustomStat.MoneyDrop) / 100;
                //SephiriaPrefabs.SpawnMoney(add, avatar.transform.position);
                NetworkAvatar.AddMoney(add);
            }
            if (damage.isExecutionAttack)
                return;
            var dig = damage.criticalChancePercent - 100;
            if (dig <= 0 || !dig.Percent())
                return;
            int money = NetworkAvatar.Money / 500;
            if (money > 0 && money.Percent())
            {
                this.AddRandomJewelry();
            }
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.OnAttackUnit -= OnAttackUnit;
        }
    }
}
