using HarmonyLib;
using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaMod.Items
{
    public class Charm_OverFlameSword : Charm_StatusInstance
    {
        public static readonly string OverFlameSword = "OverFlameSword".ToUpperInvariant();
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.AddCustomStatUnsafe(OverFlameSword, 1);
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.AddCustomStatUnsafe(OverFlameSword, -1);
        }

        [HarmonyPatch(typeof(ComboEffect_FlameSword), "AddSwordServer")]
        public static class Patch
        {
            static void Prefix(ComboEffect_FlameSword __instance, int amount)
            {
                if (__instance.Networkavatar.GetCustomStatUnsafe(OverFlameSword) <= 0)
                    return;

                int b = __instance.maxSword + __instance.Networkavatar.GetCustomStatUnsafe("FLAMESWORDMAX");
                var over = (__instance.currentSword + amount) - b;
                for (int q = 0; q < over; q++)
                {
                    __instance.InvokeLocalFireSword(__instance.Networkavatar.transform.position, false, false);
                }
            }
        }
    }
}
