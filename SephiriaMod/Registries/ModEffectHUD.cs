using MelonLoader;
using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using static Miracle;

namespace SephiriaMod.Registries
{
    public class ModEffectHUD
    {
        public static ModEffectHUD CreateStackEffectHUD(string name, UI_EffectHUD_Basic.EEffectType type)
        {
            var hud = new ModEffectHUD();
            hud.Type = EffectHUDType.Stack;
            hud.EffectType = type;
            hud.Name = name;
            hud.Id = name.ToSephiriaUpperId();
            hud.LocalizedName = new LocalizedString("EffectHUD_" + name + "_Name");
            hud.FlavorText = new LocalizedString("EffectHUD_" + name + "_FlavorText");
            hud.IconFileName = ModUtil.EffectHUDPath + name;
            return hud;
        }
        public EffectHUDType Type { get; internal set; }
        public string Name { get; internal set; }
        public string Id { get; internal set; }
        public Sprite Icon { get; internal set; }
        public string IconFileName { get; internal set; }
        public UI_EffectHUD_Basic.EEffectType EffectType { get; internal set; }
        public LocalizedString LocalizedName { get; internal set; }
        public LocalizedString FlavorText {  get; internal set; }
        public GameObject ResourcePrefab { get; internal set; }
        public void SetResourcePrefab(GameObject prefab)
        {
            //Melon<Core>.Logger.Msg("SetResourcePrefab: " + LocalizedName.ToString());
            var basic = prefab.GetComponent<UI_EffectHUD_Basic>();
            basic.effectName = LocalizedName;
            basic.effectFlavorText = FlavorText;
            basic.effectType = EffectType;
            var rect = prefab.transform as RectTransform;
            var icon = rect.GetChild(1).GetComponent<Image>();//Icon
            icon.sprite = (Icon ?? SpriteLoader.LoadSprite(IconFileName)) ?? SpriteLoader.LoadSprite(ModUtil.EffectHUDPath + "Empty");
            ResourcePrefab = prefab;
        }
        public EffectHUDEntity CreateEntity()
        {
            var entity = ScriptableObject.CreateInstance<EffectHUDEntity>();
            entity.name = Name;
            entity.id = Id;
            entity.hudPrefab = ResourcePrefab;
            return entity;
        }
        public enum EffectHUDType
        {
            Stack,
            Buff
        }
    }
}
