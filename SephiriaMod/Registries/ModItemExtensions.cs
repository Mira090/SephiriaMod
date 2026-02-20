using SephiriaMod.Buffs;
using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using static Miracle;
using static StoneTablet;

namespace SephiriaMod.Registries
{
    public static class ModItemExtensions
    {
        public static T SetRarity<T>(this T item, EItemRarity rarity) where T : ModItem
        {
            item.Rarity = rarity;
            item.Cost = rarity switch
            {
                EItemRarity.Common => 400,
                EItemRarity.Uncommon => 600,
                EItemRarity.Rare => 800,
                EItemRarity.Legend => 1100,
                _ => 200
            };
            item.SapphirePrice = rarity switch
            {
                EItemRarity.Common => 2,
                EItemRarity.Uncommon => 3,
                EItemRarity.Rare => 4,
                EItemRarity.Legend => 5,
                _ => 1
            };
            return item;
        }
        public static T SetSacrifice<T>(this T item) where T : ModItem
        {
            item.Rarity = ECustomItemRarity.Sacrifice.ToSephiria();
            item.Cost = 999999999;
            item.SapphirePrice = 0;
            //item.CannotBeReward = true;
            item.CannotThrow = true;
            if(item is ModCharm charm)
                charm.IsUniqueEffect = true;
            item.IconFileName = ModUtil.ItemPath + "Sacrifice";
            return item;
        }
        public static T SetCategory<T>(this T item, params List<string> categories) where T : ModItem
        {
            item.Categories = categories;
            return item;
        }
        public static T SetEffects<T>(this T item, params LocalizedString[] effects) where T : ModCharm
        {
            item.Effects = effects;
            return item;
        }
        public static T SetEffects<T>(this T item, params string[] effects) where T : ModCharm
        {
            item.Effects = effects.Select(effect => new LocalizedString(effect)).ToArray();
            return item;
        }
        public static T SetActiveType<T>(this T item, EItemActiveType activeType) where T : ModItem
        {
            item.ActiveType = activeType;
            return item;
        }
        public static T SetCannotBeReward<T>(this T item, bool cannotBeReward = true) where T : ModItem
        {
            item.CannotBeReward = cannotBeReward;
            return item;
        }
        public static T SetCannotThrow<T>(this T item, bool cannotThrow = true) where T : ModItem
        {
            item.CannotThrow = cannotThrow;
            return item;
        }
        public static T SetIsDual<T>(this T item, bool isDual = true) where T : ModItem
        {
            item.IsDual = isDual;
            return item;
        }
        public static T SetSimpleEffect<T>(this T item) where T : ModCharm
        {
            item.Effects = [new LocalizedString("Item_" + item.Name + "_Effect")];
            return item;
        }
        public static T SetSimpleEffects<T>(this T item, int count) where T : ModCharm
        {
            var list = new List<LocalizedString>();
            for(int q = 0; q < count; q++)
            {
                if (q == 0)
                    list.Add(new LocalizedString("Item_" + item.Name + "_Effect"));
                else
                    list.Add(new LocalizedString("Item_" + item.Name + "_Effect" + (q + 1).ToString()));
            }
            item.Effects = list.ToArray();
            return item;
        }
        public static T SetIsUniqueEffect<T>(this T item, bool isUniqueEffect = true) where T : ModCharm
        {
            item.IsUniqueEffect = isUniqueEffect;
            return item;
        }
        public static T SetRelatedWeapon<T>(this T item, EWeaponType relatedWeapon) where T : ModCharm
        {
            item.RelatedWeapon = relatedWeapon;
            item.IsWeaponRelatedCharm = true;
            return item;
        }
        public static T SetDamageId<T>(this T item) where T : ModCharm
        {
            item.DamageId = ModDamageId.CreateCharm(item.Name);
            return item;
        }
        public static T SetDamageIdAbility<T>(this T item) where T : ModComboEffect
        {
            item.DamageId = ModDamageId.CreateAbility(item.Name);
            return item;
        }
        public static T SetStats<T>(this T item, params Charm_StatusInstance.StatusGroup[] stats) where T : ModCharmStatus
        {
            item.Stats = stats;
            return item;
        }
        public static T SetFromType<T>(this T item, EDamageFromType fromType) where T : ModCharmSacrificeDamage
        {
            item.FromType = fromType;
            return item;
        }
        public static T SetElementalType<T>(this T item, EDamageElementalType elementalType) where T : ModCharmSacrificeDamage
        {
            item.ElementalType = elementalType;
            return item;
        }
        public static T SetDamageType<T>(this T item, EDamageFromType fromType, EDamageElementalType elementalType) where T : ModCharmSacrificeDamage
        {
            item.FromType = fromType;
            item.ElementalType = elementalType;
            return item;
        }
        public static T SetConditionQuery<T>(this T item, string query) where T : ModStoneTablet
        {
            item.ConditionQuery = query;
            return item;
        }
        public static T SetStats<T>(this T item, params ComboEffectBase.ComboStat[] stats) where T : ModComboEffect
        {
            item.Stats = stats;
            return item;
        }
        public static T SetDefaultEffect<T>(this T item, LocalizedString defaultEffect) where T : ModComboEffect
        {
            item.DefaultEffect = defaultEffect;
            return item;
        }
        public static T SetDefaultEffect<T>(this T item, string defaultEffect) where T : ModComboEffect
        {
            item.DefaultEffect = new LocalizedString(defaultEffect);
            return item;
        }
        public static T SetDefaultEffect<T>(this T item) where T : ModComboEffect
        {
            item.DefaultEffect = new LocalizedString("ComboEffectDefault_" + item.Name + "_Description");
            return item;
        }
        public static T SetCategories<T>(this T item, params string[] categories) where T : ModMiracle
        {
            item.Categories = categories;
            item.GiveItem = EItemGiveType.GiveItem;
            return item;
        }
        public static T SetMiracleEffects<T>(this T item, params Effect[] effects) where T : ModMiracle
        {
            item.Effects = effects;
            return item;
        }
        public static T SetManuallyGivenItems<T>(this T item, params int[] manuallyGivenItems) where T : ModMiracle
        {
            item.ManuallyGivenItemsId = manuallyGivenItems;
            item.GiveItem = EItemGiveType.GiveItemManually;
            return item;
        }
        public static T SetNotGiveItem<T>(this T item) where T : ModMiracle
        {
            item.GiveItem = EItemGiveType.NotGiveItem;
            item.Effects = [CreateNegativeEffect("Miracle_Gambler_Effect2")];
            return item;
        }
        public static T SetGiveItem<T>(this T item) where T : ModMiracle
        {
            item.GiveItem = EItemGiveType.GiveItem;
            return item;
        }
        public static T SetGiveItem<T>(this T item, EItemGiveType giveItem) where T : ModMiracle
        {
            item.GiveItem = giveItem;
            return item;
        }
        public static T SetTier<T>(this T item, ETier tier) where T : ModMiracle
        {
            item.Tier = tier;
            return item;
        }
        public static T SetNotAutoGenerateEffectString<T>(this T item) where T : ModMiracleStatus
        {
            item.AutoGenerateEffectString = false;
            return item;
        }
        public static T SetNotAutoGenerateEffectString<T>(this T item, params Effect[] effects) where T : ModMiracleStatus
        {
            item.AutoGenerateEffectString = false;
            item.SetMiracleEffects(effects);
            return item;
        }
        public static T SetNotAutoGenerateEffectString<T>(this T item, int count, EEffectType type, bool notGiveItem = false) where T : ModMiracleStatus
        {
            item.AutoGenerateEffectString = false;
            var list = new List<Effect>();
            for (int q = 1; q <= count; q++)
                list.Add(CreateEffect("Miracle_" + item.Name + "_Effect" + (q == 1 ? "" : q.ToString()), type));
            if(notGiveItem)
                list.Add(CreateNegativeEffect("Miracle_Gambler_Effect2"));
            item.SetMiracleEffects(list.ToArray());
            return item;
        }
        public static T SetDivideForDisplay<T>(this T item, int divideForDisplay) where T : ModCustomStatus
        {
            item.DivideForDisplay = divideForDisplay;
            return item;
        }
        public static T SetPercentSymbol<T>(this T item) where T : ModCustomStatus
        {
            item.Symbol = "%";
            return item;
        }
        public static T SetSymbol<T>(this T item, string symbol) where T : ModCustomStatus
        {
            item.Symbol = symbol;
            return item;
        }
        public static T SetLocalizedSymbol<T>(this T item, string symbol) where T : ModCustomStatus
        {
            item.LocalizedSymbol = new LocalizedString(symbol);
            item.UseLocalizedSymbol = true;
            return item;
        }
        public static T SetIncludePositiveNegativeSign<T>(this T item, bool includePositiveNegativeSign = false) where T : ModCustomStatus
        {
            item.IncludePositiveNegativeSign = includePositiveNegativeSign;
            return item;
        }
        public static T SetDisplayValue<T>(this T item, bool displayValue = false) where T : ModCustomStatus
        {
            item.DisplayValue = displayValue;
            return item;
        }
        public static T SetDisplayDetails<T>(this T item, bool displayDetails = false) where T : ModKeyword
        {
            item.DisplayDetails = displayDetails;
            return item;
        }
        public static T SetDisplayDetails<T>(this T item, LocalizedString detailedValue) where T : ModKeyword
        {
            item.DetailedValue = detailedValue;
            return item;
        }
        public static T SetTextColor<T>(this T item, Color color) where T : ModKeyword
        {
            item.TextColor = color;
            item.TextColorOriginal = null;
            return item;
        }
        public static T SetKeywordImage<T>(this T item, Func<Sprite> image) where T : ModKeyword
        {
            item.KeywordImage = image;
            item.KeywordImageOriginal = null;
            return item;
        }
        public static T SetOriginal<T>(this T item, string keyword) where T : ModKeyword
        {
            item.TextColorOriginal = keyword;
            item.KeywordImageOriginal = keyword;
            return item;
        }
        public static T SetTextColorOriginal<T>(this T item, string keyword) where T : ModKeyword
        {
            item.TextColorOriginal = keyword;
            return item;
        }
        public static T SetKeywordImageOriginal<T>(this T item, string keyword) where T : ModKeyword
        {
            item.KeywordImageOriginal = keyword;
            return item;
        }
        public static T SetConnectedDetailEntities<T>(this T item, params string[] keywords) where T : ModKeyword
        {
            item.ConnectedDetailEntities = keywords.ToList();
            return item;
        }
        public static T SetStandardEnhancements<T>(this T item, params int[] enhancements) where T : ModWeapon
        {
            item.StandardEnhancements = enhancements.ToList();
            return item;
        }
        public static T SetMainPrefabModifier<T>(this T item, Action<WeaponSimple> modifier) where T : ModWeapon
        {
            item.MainPrefabModifier = modifier;
            return item;
        }
        public static T SetBladeSprite<T>(this T item, Vector3? pos = null) where T : ModWeapon
        {
            item.HasBladeSprite = true;
            item.BladeSpritePosition = pos;
            return item;
        }
        public static T SetHeadSprite<T>(this T item, Vector3? pos = null) where T : ModWeapon
        {
            item.HasHeadSprite = true;
            item.HeadSpritePosition = pos;
            return item;
        }
        public static T SetDefaultDuration<T>(this T item, float duration) where T : CharacterBuffMod
        {
            item.defaultDuration = duration;
            return item;
        }
        public static ModPassivePerk CreatePerk<T>(this T item, EPassivePerkLv lv, string name) where T : ModPassive
        {
            var perk = ModPassivePerk.CreatePassivePerk(item, name, lv);
            if(lv == EPassivePerkLv.lv5)
            {
                item.Lv5Perk = perk;
            }
            else if(lv == EPassivePerkLv.lv10)
            {
                item.Lv10Perk = perk;
            }
            else if(lv == EPassivePerkLv.lv20)
            {
                item.Lv20Perk = perk;
            }
            return perk;
        }
        public static T SetPerkSupplier<T>(this T item, Func<GameObject, PassiveObject> supplier) where T : ModPassivePerk
        {
            item.PerkSupplier = supplier;
            return item;
        }
        public static T SetPerkSupplierStatus<T>(this T item, params string[] stats) where T : ModPassivePerk
        {
            item.PerkSupplier = gameObject =>
            {
                var perk = gameObject.AddComponent<PassiveObject_StatusInstance>();
                perk.stats = stats;
                return perk;
            };
            return item;
        }


        public static Charm_StatusInstance.StatusGroup CreateStatusGroup(string id, params int[] values)
        {
            return new Charm_StatusInstance.StatusGroup() { statusID = id, valuesByLevel = values };
        }
        public static ComboEffectBase.ComboStat CreateComboStat(int count, params string[] status)
        {
            return new ComboEffectBase.ComboStat() { comboCount = count, status = status };
        }
        public static Effect CreateEffect(string effect, EEffectType effectType)
        {
            return new Effect() { effectString = new LocalizedString(effect), effectType = effectType };
        }
        public static Effect CreatePositiveEffect(string effect)
        {
            return new Effect() { effectString = new LocalizedString(effect), effectType = EEffectType.Positive };
        }
        public static Effect CreateNegativeEffect(string effect)
        {
            return new Effect() { effectString = new LocalizedString(effect), effectType = EEffectType.Negative };
        }
        public static Effect CreateNeutralEffect(string effect)
        {
            return new Effect() { effectString = new LocalizedString(effect), effectType = EEffectType.Neutral };
        }
        public static Miracle_StatusInstance.StatInfo CreateStat(string status, EEffectType type)
        {
            return new Miracle_StatusInstance.StatInfo() { type = type, status = status };
        }
        public static Miracle_StatusInstance.StatInfo CreatePositiveStat(string status)
        {
            return new Miracle_StatusInstance.StatInfo() { type = EEffectType.Positive, status = status };
        }
        public static Miracle_StatusInstance.StatInfo CreateNegativeStat(string status)
        {
            return new Miracle_StatusInstance.StatInfo() { type = EEffectType.Negative, status = status };
        }
        public static Miracle_StatusInstance.StatInfo CreateNeutralStat(string status)
        {
            return new Miracle_StatusInstance.StatInfo() { type = EEffectType.Neutral, status = status };
        }
    }
}
