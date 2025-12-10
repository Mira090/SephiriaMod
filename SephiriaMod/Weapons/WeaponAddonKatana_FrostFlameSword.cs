using HarmonyLib;
using MelonLoader;
using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Weapons
{
    public class WeaponAddonKatana_FrostFlameSword : WeaponAddonCommon_AdditionalElementalDamage
    {
        protected override void OnEnableAddon()
        {
            base.OnEnableAddon();
            parent.Networkowner.unitAvatar.AddCustomStatUnsafe("FrostFlameSword".ToSephiriaUpperId(), 1);
        }
        protected override void OnDisableAddon()
        {
            base.OnDisableAddon();
            parent.Networkowner.unitAvatar.AddCustomStatUnsafe("FrostFlameSword".ToSephiriaUpperId(), -1);
        }
        [HarmonyPatch(typeof(ComboEffect_FlameSword), "HandleAttackUnit", [typeof(UnitAvatar), typeof(DamageInstance)])]
        public class FlameSwordPatch
        {
            static List<string> FrostRelics = ["Charm_IceSpear", "Charm_IceHammer", "Charm_IceBow", "Charm_AirSlash"];
            static void Postfix(UnitAvatar avatar, DamageInstance damage, ComboEffect_FlameSword __instance)
            {
                if (avatar && !avatar.IsDead)
                {
                    if (__instance.Networkavatar.GetCustomStatUnsafe("FrostFlameSword".ToSephiriaUpperId()) <= 0)
                        return;
                    if (__instance.GetIsCooldown())
                    {
                        return;
                    }
                    if (FrostRelics.Contains(damage.id))
                    {
                        __instance.SetIsCooldown(true);
                        __instance.SetCooldownTimer(0.1f);
                        __instance.InvokeLocalFireSword(avatar.transform.position);
                    }
                }
            }
        }
    }
}
