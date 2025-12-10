using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaMod
{
    public class Charm_MoreStoneTablet : Charm_VariableMaxLevel
    {
        public LocalizedString semanticName = new LocalizedString("ItemType_StoneTablet");
        public string semantic = "TABLET";
        public int[] semanticDropWeight = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = (showAllLevel ? ((semanticDropWeight.SafeRandomAccess(0) * 50) + "→" + (semanticDropWeight.SafeRandomAccess(maxLevel) * 50)) : (semanticDropWeight.SafeRandomAccess(LevelToIdx(level)) * 50).ToString());
            return new Loc.KeywordValue[2]
            {
            new Loc.KeywordValue("ITEM_TYPE", semanticName.ToString()),
            new Loc.KeywordValue("DROP_PERCENT", value + "%", Charm_Basic.GetPositiveColor(virtualLevelOffset))
            };
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            base.NetworkAvatar.Inventory.AddItemDropBonusBySemantic(semantic, semanticDropWeight.SafeRandomAccess(CurrentLevelToIdx()));
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            base.NetworkAvatar.Inventory.AddItemDropBonusBySemantic(semantic, -semanticDropWeight.SafeRandomAccess(CurrentLevelToIdx()));
        }
        protected override void OnUpdatedLevel(int oldLevel, int newLevel)
        {
            base.OnUpdatedLevel(oldLevel, newLevel);
            base.NetworkAvatar.Inventory.AddItemDropBonusBySemantic(semantic, -semanticDropWeight.SafeRandomAccess(LevelToIdx(oldLevel)));
            base.NetworkAvatar.Inventory.AddItemDropBonusBySemantic(semantic, semanticDropWeight.SafeRandomAccess(LevelToIdx(newLevel)));
        }
        public override bool Weaved()
        {
            return true;
        }
    }
}
