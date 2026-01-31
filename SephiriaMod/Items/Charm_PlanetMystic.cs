using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Items
{
    public class Charm_PlanetMystic : Charm_StatusInstance
    {
        public int[] speed = [1, 1, 2, 2, 3];
        public List<Charm_SummonGreenBat> recentPlanets = new();
        public int stat = 0;
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? speed.SafeRandomAccess(0) + "→" + speed.SafeRandomAccess(maxLevel) : speed.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[1]
            {
                new Loc.KeywordValue("SPEED", "+" + value + "%", GetPositiveColor(virtualLevelOffset))
            };
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
        }
        public override void OnCharmEffectRefreshed()
        {
            base.OnCharmEffectRefreshed();
            ClearEnhancements();
            ClearStats();
            if (IsEffectEnabled)
            {
                UpdateEnhancements();
                UpdateStats();
            }
        }
        private void ClearEnhancements()
        {
            foreach (var planet in recentPlanets)
            {
                planet.SetEnhancement(false);
            }
            recentPlanets.Clear();
        }
        private void UpdateEnhancements()
        {
            var combo = NetworkAvatar.Inventory.FindComboEffect(ItemCategories.Mystic);
            if (combo == null || !combo.isEnabled || combo is not ComboEffect_Mystic mystic)
                return;

            List<ItemPosition> pos = new();
            if (mystic.comboCount >= mystic.first)
            {
                for (int q = 0; q < mystic.firstEngravingCount; q++)
                {
                    pos.Add(mystic.GetPosition(NetworkAvatar, q));
                }
            }
            if (mystic.comboCount >= mystic.second)
            {
                for (int q = 0; q < mystic.secondEngravingCount; q++)
                {
                    pos.Add(mystic.GetPosition(NetworkAvatar, q + mystic.firstEngravingCount));
                }
            }

            foreach (var p in pos)
            {
                var item = NetworkAvatar.Inventory.FindItem(p);
                if (item == null || item.Charm == null || item.Charm is not Charm_SummonGreenBat planet)
                    continue;
                planet.SetEnhancement(true);
                recentPlanets.Add(planet);
            }
        }
        private void ClearStats()
        {
            if (stat == 0)
                return;
            NetworkAvatar.AddCustomStatUnsafe("PlanetAttackSpeed".ToUpperInvariant(), -stat);
            stat = 0;
        }
        private void UpdateStats()
        {
            if(recentPlanets.Count == 0)
                return;
            int level = recentPlanets.Sum(x => Mathf.Min(x.DisplayedLevel, x.maxLevel));
            stat = level * speed.SafeRandomAccess(CurrentLevelToIdx());
            NetworkAvatar.AddCustomStatUnsafe("PlanetAttackSpeed".ToUpperInvariant(), stat);
        }
        protected override void OnUpdatedLevel(int oldLevel, int newLevel)
        {
            base.OnUpdatedLevel(oldLevel, newLevel);
            ClearStats();
            if (IsEffectEnabled)
            {
                UpdateStats();
            }
        }
    }
}
