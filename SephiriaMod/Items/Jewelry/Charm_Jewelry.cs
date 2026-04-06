using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaMod.Items.Jewelry
{
    public class Charm_Jewelry : Charm_VariableMaxLevel
    {
        public static readonly int MoneyPerLevel = 100;
        public override string StatusName => string.Empty;
        protected override void OnConnected(int instanceID)
        {
            base.OnConnected(instanceID);
            var money = NetworkAvatar.Money;
            var level = money / MoneyPerLevel;
            SetAdditionalMaxLevel(level);
            NetworkAvatar.SetMoney(0);
        }
        protected virtual int FirstLevel => 4;
        protected virtual int SecondLevel => 8;
        protected virtual int ThirdLevel => 12;
        public override string GetEffectString(int idx, int level, int virtualLevelOffset, bool showAllLevel)
        {
            if (idx == 0 && level < FirstLevel)
                return null;
            if (idx == 1 && level < SecondLevel)
                return null;
            if (idx == 2 && level < ThirdLevel)
                return null;
            return base.GetEffectString(idx, level, virtualLevelOffset, showAllLevel);
        }
    }
}
