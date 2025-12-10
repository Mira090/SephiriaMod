using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod
{
    public class Charm_VariableMaxLevel : Charm_StatusInstance
    {
        public virtual string StatusName => "STARGAZELEVEL";
        public virtual int ValiableMax => 16;
        public int AdditionalMaxLevel { get; private set; }
        public int OriginalMaxLevel { get; private set; }
        private void Awake()
        {
            UITierPatch.Init();
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            OriginalMaxLevel = maxLevel;
            if(NetworkAvatar != null)
            {
                SetAdditionalMaxLevel(NetworkAvatar.GetCustomStatUnsafe(StatusName));
            }
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            base.maxLevel = OriginalMaxLevel;
        }
        public virtual void SetAdditionalMaxLevel(int level)
        {
            if(OriginalMaxLevel + level > ValiableMax)
            {
                level = ValiableMax - OriginalMaxLevel;
            }
            AdditionalMaxLevel = level;
            base.maxLevel = OriginalMaxLevel + AdditionalMaxLevel;
        }
        public override void OnCharmEffectRefreshed()
        {
            base.OnCharmEffectRefreshed();
            if (NetworkAvatar != null)
            {
                SetAdditionalMaxLevel(NetworkAvatar.GetCustomStatUnsafe(StatusName));
                base.Inventory.UpdatePing(base.Item.Position);
            }
        }
        [HarmonyPatch(typeof(UI_CharmTierDisplay), nameof(UI_CharmTierDisplay.SetTier), [typeof(int), typeof(int), typeof(int), typeof(int)])]
        public static class UITierPatch
        {
            public static Sprite StargazeTier;
            public static Sprite StargazeTierDisable;
            public static Sprite StargazeTierVirtual;
            public static Sprite StargazeTierEnchant;
            public static Sprite StargazeTierEnchantVirtual;
            public static void Init()
            {
                if (StargazeTier == null)
                    StargazeTier = SpriteLoader.LoadSprite(ModUtil.UIPath + "Tier");
                if (StargazeTierDisable == null)
                    StargazeTierDisable = SpriteLoader.LoadSprite(ModUtil.UIPath + "Tier_disable");
                if (StargazeTierVirtual == null)
                    StargazeTierVirtual = SpriteLoader.LoadSprite(ModUtil.UIPath + "Tier_virtual");
                if (StargazeTierEnchant == null)
                    StargazeTierEnchant = SpriteLoader.LoadSprite(ModUtil.UIPath + "Tier_Enchant");
                if (StargazeTierEnchantVirtual == null)
                    StargazeTierEnchantVirtual = SpriteLoader.LoadSprite(ModUtil.UIPath + "Tier_EnchantVirtual");
            }
            static void Postfix(int maxTier, int currentTier, int virtualTierOffset, int enchant, UI_CharmTierDisplay __instance)
            {
                if (!__instance.transform.parent.TryGetComponent<UI_CharmTooltip>(out var tooltip))
                    return;

                if (tooltip.TooltipObject is not NewItemOwnInstance item)
                    return;

                int additional = 0;

                if (item.Charm == null)
                    return;

                if(item.Charm is Charm_VariableMaxLevel variable)
                {
                    additional = variable.AdditionalMaxLevel;
                }
                else if(item.Charm is Charm_VariableMaxLevelWhitePaper whitepaper)
                {
                    additional = whitepaper.AdditionalMaxLevel;
                }

                if (additional <= 0)
                    return;

                for (int q = 0; q < maxTier; q++)
                {
                    if (q < maxTier - additional)
                        continue;
                    if(__instance.starImages[q].sprite == __instance.emptyImage)
                    {
                        __instance.starImages[q].sprite = StargazeTierDisable;
                    }
                    else if(__instance.starImages[q].sprite == __instance.realImage)
                    {
                        __instance.starImages[q].sprite = StargazeTier;
                    }
                    else if (__instance.starImages[q].sprite == __instance.virtualImage)
                    {
                        __instance.starImages[q].sprite = StargazeTierVirtual;
                    }
                    else if (__instance.starImages[q].sprite == __instance.enchantRealImage)
                    {
                        __instance.starImages[q].sprite = StargazeTierEnchant;
                    }
                    else if (__instance.starImages[q].sprite == __instance.enchantVirtualImage)
                    {
                        __instance.starImages[q].sprite = StargazeTierEnchantVirtual;
                    }
                }
            }
        }
    }
}
