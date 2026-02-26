using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Items
{
    public class Charm_DashFlameSword : Charm_StatusInstance
    {
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.OnDashServerside += OnDashServerside;
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.OnDashServerside -= OnDashServerside;
        }
        private void OnDashServerside(Vector2 motionTo, bool consume)
        {
            var combo = NetworkAvatar.Inventory.FindComboEffect(ItemCategories.FlameSword);
            if(combo == null || !combo.isEnabled || combo is not ComboEffect_FlameSword flame)
                return;


            flame.InvokeLocalFireSword((Vector2)NetworkAvatar.transform.position + motionTo / 2f, false, false);
        }
    }
}
