using HarmonyLib;
using SephiriaMod.Items.Jewelry;
using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static MelonLoader.MelonLogger;

namespace SephiriaMod.Items
{
    public class Charm_VariableMaxLevel : Charm_StatusInstance
    {
        public virtual string StatusName => "STARGAZELEVEL";
        public virtual int ValiableMax => 16;
        public int AdditionalMaxLevel { get; protected set; }
        public int OriginalMaxLevel { get; protected set; }
        private void Awake()
        {
            UITierPatch.Init();
        }
        protected override void OnConnected(int instanceID)
        {
            base.OnConnected(instanceID);
            OriginalMaxLevel = maxLevel;
            //Melon<Core>.Logger.Msg($"OnConnected: {OriginalMaxLevel}");
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            //OriginalMaxLevel = maxLevel;
            if(NetworkAvatar != null)
            {
                if(!string.IsNullOrEmpty(StatusName))
                    SetAdditionalMaxLevel(NetworkAvatar.GetCustomStatUnsafe(StatusName));
            }
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            //maxLevel = OriginalMaxLevel;
        }
        public virtual void SetAdditionalMaxLevel(int level)
        {
            if(OriginalMaxLevel + level > ValiableMax)
            {
                level = ValiableMax - OriginalMaxLevel;
            }
            AdditionalMaxLevel = level;
            maxLevel = OriginalMaxLevel + AdditionalMaxLevel;
        }
        public override void OnCharmEffectRefreshed()
        {
            base.OnCharmEffectRefreshed();
            if (NetworkAvatar != null)
            {
                if (!string.IsNullOrEmpty(StatusName))
                {
                    SetAdditionalMaxLevel(NetworkAvatar.GetCustomStatUnsafe(StatusName));
                    Inventory.UpdatePing(Item.Position);
                }
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

            public static Sprite JewelryTier;
            public static Sprite JewelryTierDisable;
            public static Sprite JewelryTierVirtual;
            public static Sprite JewelryTierEnchant;
            public static Sprite JewelryTierEnchantVirtual;
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

                if (JewelryTier == null)
                    JewelryTier = SpriteLoader.LoadSprite(ModUtil.UIPath + "JewelryTier");
                if (JewelryTierDisable == null)
                    JewelryTierDisable = SpriteLoader.LoadSprite(ModUtil.UIPath + "JewelryTier_disable");
                if (JewelryTierVirtual == null)
                    JewelryTierVirtual = SpriteLoader.LoadSprite(ModUtil.UIPath + "JewelryTier_virtual");
                if (JewelryTierEnchant == null)
                    JewelryTierEnchant = SpriteLoader.LoadSprite(ModUtil.UIPath + "JewelryTier_Enchant");
                if (JewelryTierEnchantVirtual == null)
                    JewelryTierEnchantVirtual = SpriteLoader.LoadSprite(ModUtil.UIPath + "JewelryTier_EnchantVirtual");
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

                bool jewelry = item.Charm is Charm_Jewelry;
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
                    if (jewelry)
                        SetJewelryTier(__instance, q);
                    else
                        SetStargazeTier(__instance, q);
                }
            }
            private static void SetStargazeTier(UI_CharmTierDisplay __instance, int index)
            {
                if (__instance.starImages[index].sprite == __instance.emptyImage)
                {
                    __instance.starImages[index].sprite = StargazeTierDisable;
                }
                else if (__instance.starImages[index].sprite == __instance.realImage)
                {
                    __instance.starImages[index].sprite = StargazeTier;
                }
                else if (__instance.starImages[index].sprite == __instance.virtualImage)
                {
                    __instance.starImages[index].sprite = StargazeTierVirtual;
                }
                else if (__instance.starImages[index].sprite == __instance.enchantRealImage)
                {
                    __instance.starImages[index].sprite = StargazeTierEnchant;
                }
                else if (__instance.starImages[index].sprite == __instance.enchantVirtualImage)
                {
                    __instance.starImages[index].sprite = StargazeTierEnchantVirtual;
                }
            }
            private static void SetJewelryTier(UI_CharmTierDisplay __instance, int index)
            {
                if (__instance.starImages[index].sprite == __instance.emptyImage)
                {
                    __instance.starImages[index].sprite = JewelryTierDisable;
                }
                else if (__instance.starImages[index].sprite == __instance.realImage)
                {
                    __instance.starImages[index].sprite = JewelryTier;
                }
                else if (__instance.starImages[index].sprite == __instance.virtualImage)
                {
                    __instance.starImages[index].sprite = JewelryTierVirtual;
                }
                else if (__instance.starImages[index].sprite == __instance.enchantRealImage)
                {
                    __instance.starImages[index].sprite = JewelryTierEnchant;
                }
                else if (__instance.starImages[index].sprite == __instance.enchantVirtualImage)
                {
                    __instance.starImages[index].sprite = JewelryTierEnchantVirtual;
                }
            }
        }
    }
}
