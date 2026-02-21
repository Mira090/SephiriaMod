using MelonLoader;
using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Registries
{
    public class ModPassivePerk
    {
        public static ModPassivePerk CreatePassivePerk(ModPassive parent, string name, EPassivePerkLv lv)
        {
            return new ModPassivePerk().SetPassivePerk(parent, name, lv);
        }
        internal ModPassivePerk SetPassivePerk(ModPassive parent, string name, EPassivePerkLv lv)
        {
            Parent = parent;
            Name = name;
            EffectString = new LocalizedString($"Passive_{parent.Name}_Effect_{lv.ToUpperString()}");
            IconPath = ModUtil.PassivePath + parent.Name + "_Perk_" + lv.ToUpperString();
            return this;
        }
        public ModPassive Parent { get; private set; }
        public string Name { get; internal set; }
        public EPassivePerkLv PerkLv { get; internal set; }
        public uint AssetId { get; internal set; }
        public LocalizedString EffectString { get; internal set; }
        public string IconPath { get; internal set; }
        public Func<GameObject, PassiveObject> PerkSupplier { get; internal set; }

        public GameObject PerkPrefab { get; internal set; }

        public void Init(uint assetId)
        {
            PerkPrefab = CreateResourcePrefab();
            AssetId = assetId;
        }
        public GameObject CreateResourcePrefab()
        {
            var o = new GameObject($"{Parent.Id}_{PerkLv.ToUpperString()}_{Name}");
            var meta = o.AddComponent<PassiveObjectMetadata>();
            meta.effectString = EffectString;
            meta.icon = SpriteLoader.LoadSprite(IconPath);
            o.AddComponent<LogComponent>();
            //Melon<Core>.Logger.Msg($"CreatePassivePerk");
            o.hideFlags = HideFlags.HideAndDontSave;
            if (PerkSupplier != null)
            {
                var effect = PerkSupplier.Invoke(o);
                effect.enabled = false;
            }
            else
            {
                Melon<Core>.Logger.Warning($"{Parent.Name} Perk {Name}: PerkSupplier is null");
            }
            return o;
        }
    }
}
