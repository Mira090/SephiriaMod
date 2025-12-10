using MelonLoader;
using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaMod
{
    public class WeaponAddonDagger_CounterEvasion : WeaponAddon
    {
        protected override void OnEnableAddon()
        {
            base.OnEnableAddon();
            parent.Networkowner.unitAvatar.OnEvade += OnEvade;
            parent.Networkowner.unitAvatar.OnCounter += OnCounter;
        }

        private void OnEvade(DamageInstance damage)
        {
            //Melon<Core>.Logger.Msg("buff: " + Items.PhysicalDamageBuff);
            var buff = parent.Networkowner.unitAvatar.ApplyBuff(Items.PhysicalDamageBuff, 1, null, true);
        }

        private void OnCounter(DamageInstance damage)
        {
            parent.Networkowner.unitAvatar.InvokeOnEvade(damage);
        }

        protected override void OnDisableAddon()
        {
            base.OnDisableAddon();
            parent.Networkowner.unitAvatar.OnEvade -= OnEvade;
            parent.Networkowner.unitAvatar.OnCounter -= OnCounter;
        }
    }
}
