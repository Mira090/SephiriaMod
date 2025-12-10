using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaMod
{
    public class Charm_ManyGrimoire : Charm_Basic
    {
        public int[] criticalBonusByLevel = new int[4] { 1, 1, 2, 3};

        private int enabledCriticalBonus;

        private int savedStoneTabletCount;

        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = (showAllLevel ? (((float)criticalBonusByLevel.SafeRandomAccess(0)).ToString("0.##") + "→" + ((float)criticalBonusByLevel.SafeRandomAccess(maxLevel)).ToString("0.##")) : ((float)criticalBonusByLevel.SafeRandomAccess(LevelToIdx(level))).ToString("0.##"));
            int num = 0;
            if ((bool)avatar)
            {
                num = avatar.Inventory.charms.Values.Count(charm => charm is Charm_Magic);
            }

            return new Loc.KeywordValue[2]
            {
            new Loc.KeywordValue("STATUS", "+" + value, Charm_Basic.GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("COUNT", num.ToString())
            };
        }

        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            if (!(base.NetworkAvatar.Inventory == null))
            {
                base.NetworkAvatar.Inventory.OnCharmEffectRefreshedForServer += OnItemUpdated;
                savedStoneTabletCount = base.NetworkAvatar.Inventory.charms.Values.Count(charm => charm is Charm_Magic);
                enabledCriticalBonus = criticalBonusByLevel.SafeRandomAccess(CurrentLevelToIdx()) * savedStoneTabletCount;
                base.NetworkAvatar.AddCustomStat(ECustomStat.FireDamage, enabledCriticalBonus);
                base.NetworkAvatar.AddCustomStat(ECustomStat.IceDamage, enabledCriticalBonus);
                base.NetworkAvatar.AddCustomStat(ECustomStat.LightningDamage, enabledCriticalBonus);
            }
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            base.NetworkAvatar.AddCustomStat(ECustomStat.FireDamage, -enabledCriticalBonus);
            base.NetworkAvatar.AddCustomStat(ECustomStat.IceDamage, -enabledCriticalBonus);
            base.NetworkAvatar.AddCustomStat(ECustomStat.LightningDamage, -enabledCriticalBonus);
            savedStoneTabletCount = 0;
            enabledCriticalBonus = 0;
            if (!(base.NetworkAvatar.Inventory == null))
            {
                base.NetworkAvatar.Inventory.OnCharmEffectRefreshedForServer -= OnItemUpdated;
            }
        }

        protected override void OnUpdatedLevel(int oldLevel, int newLevel)
        {
            base.OnUpdatedLevel(oldLevel, newLevel);
            base.NetworkAvatar.AddCustomStat(ECustomStat.FireDamage, -enabledCriticalBonus);
            base.NetworkAvatar.AddCustomStat(ECustomStat.IceDamage, -enabledCriticalBonus);
            base.NetworkAvatar.AddCustomStat(ECustomStat.LightningDamage, -enabledCriticalBonus);
            enabledCriticalBonus = criticalBonusByLevel.SafeRandomAccess(LevelToIdx(newLevel)) * savedStoneTabletCount;
            base.NetworkAvatar.AddCustomStat(ECustomStat.FireDamage, enabledCriticalBonus);
            base.NetworkAvatar.AddCustomStat(ECustomStat.IceDamage, enabledCriticalBonus);
            base.NetworkAvatar.AddCustomStat(ECustomStat.LightningDamage, enabledCriticalBonus);
        }

        private void OnItemUpdated()
        {
            base.NetworkAvatar.AddCustomStat(ECustomStat.FireDamage, -enabledCriticalBonus);
            base.NetworkAvatar.AddCustomStat(ECustomStat.IceDamage, -enabledCriticalBonus);
            base.NetworkAvatar.AddCustomStat(ECustomStat.LightningDamage, -enabledCriticalBonus);
            savedStoneTabletCount = base.NetworkAvatar.Inventory.charms.Values.Count(charm => charm is Charm_Magic);
            enabledCriticalBonus = criticalBonusByLevel.SafeRandomAccess(CurrentLevelToIdx()) * savedStoneTabletCount;
            base.NetworkAvatar.AddCustomStat(ECustomStat.FireDamage, enabledCriticalBonus);
            base.NetworkAvatar.AddCustomStat(ECustomStat.IceDamage, enabledCriticalBonus);
            base.NetworkAvatar.AddCustomStat(ECustomStat.LightningDamage, enabledCriticalBonus);
        }

        public override bool Weaved()
        {
            return true;
        }
    }
}
