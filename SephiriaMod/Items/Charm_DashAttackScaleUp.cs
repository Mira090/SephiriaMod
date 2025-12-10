using MelonLoader;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

namespace SephiriaMod.Items
{
    public class Charm_DashAttackScaleUp : Charm_StatusInstance
    {
        public int[] rangeByLevel = [30, 40, 50, 60, 70];
        private bool effect = false;
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? rangeByLevel.SafeRandomAccess(0) + "→" + rangeByLevel.SafeRandomAccess(maxLevel) : rangeByLevel.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[1]
            {
            new Loc.KeywordValue("RANGE", "+" + value + "%", GetPositiveColor(virtualLevelOffset))
            };
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            Events.OnPreBasicAttack += OnPreAttack;
            Events.OnPreSpecialAttack += OnPreAttack;
            WeaponController.OnBaisAttackSwing += OnAttackSwing;
            WeaponController.OnSpecialAttackSwing += OnAttackSwing;
            if (!effect)
            {
                NetworkAvatar.AddCustomStat(ECustomStat.WeaponRange, rangeByLevel.SafeRandomAccess(CurrentLevelToIdx()));
                effect = true;
            }
        }

        private void OnPreAttack(WeaponControllerSimple weapon, UnitAvatar avatar)
        {
            if (avatar != NetworkAvatar)
                return;
            //Melon<Core>.Logger.Msg("OnPreAttack: " + effect);
            if (effect)
            {
                effect = false;
                NetworkAvatar.AddCustomStat(ECustomStat.WeaponRange, -rangeByLevel.SafeRandomAccess(CurrentLevelToIdx()));
            }
        }

        private void OnAttackSwing(int idx)
        {
            //Melon<Core>.Logger.Msg("OnAttackSwing1: " + effect);
            if (!effect)
            {
                NetworkAvatar.AddCustomStat(ECustomStat.WeaponRange, rangeByLevel.SafeRandomAccess(CurrentLevelToIdx()));
                effect = true;
            }
            //Melon<Core>.Logger.Msg("OnAttackSwing2: " + effect);
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            Events.OnPreBasicAttack -= OnPreAttack;
            Events.OnPreSpecialAttack -= OnPreAttack;
            WeaponController.OnBaisAttackSwing -= OnAttackSwing;
            WeaponController.OnSpecialAttackSwing -= OnAttackSwing;
            if (effect)
            {
                effect = false;
                NetworkAvatar.AddCustomStat(ECustomStat.WeaponRange, -rangeByLevel.SafeRandomAccess(CurrentLevelToIdx()));
            }
        }
        protected override void OnUpdatedLevel(int oldLevel, int newLevel)
        {
            base.OnUpdatedLevel(oldLevel, newLevel);
            if (effect)
            {
                NetworkAvatar.AddCustomStat(ECustomStat.WeaponRange, -rangeByLevel.SafeRandomAccess(LevelToIdx(oldLevel)));
                NetworkAvatar.AddCustomStat(ECustomStat.WeaponRange, rangeByLevel.SafeRandomAccess(LevelToIdx(newLevel)));
            }
        }
    }
}
