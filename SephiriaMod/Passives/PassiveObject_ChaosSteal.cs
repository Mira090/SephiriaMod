using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaMod.Passives
{
    public class PassiveObject_ChaosSteal : PassiveObject
    {
        public int Steal = 20;
        protected override void OnEffectEnabled(PlayerAvatar player, bool runtime)
        {
            base.OnEffectEnabled(player, runtime);
            player.OnAttackUnit += OnAttackUnit;
        }
        protected override void OnEffectDisabled()
        {
            base.OnEffectDisabled();
            player.OnAttackUnit -= OnAttackUnit;
        }

        private void OnAttackUnit(UnitAvatar avatar, DamageInstance damage)
        {
            if (!damage.IsSameElementalType(EDamageElementalType.Chaos))
                return;
            damage.hpSteal += Steal;
        }
        public override Loc.KeywordValue[] BuildKeywords()
        {
            return [new Loc.KeywordValue() { keyword = "VAL0", value = Steal.ToString() }];
        }
    }
}
