using MelonLoader;
using Mirror;
using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Registries
{
    public class ModWeapon
    {
        public static ModWeapon CreateWeapon(string name, int copy, int dependency = -1)
        {
            return new ModWeapon().SetWeapon(name, copy, dependency);
        }
        internal ModWeapon SetWeapon(string name, int copy, int dependency)
        {
            SetItem(name);
            Copy = copy;
            Dependency = dependency;
            return this;
        }
        internal ModWeapon SetItem(string name)
        {
            Name = name;
            LocalizedName = new LocalizedString("Weapon_" + name + "_Name");
            IconFileName = ModUtil.WeaponPath + name;
            MainSpriteFileName = ModUtil.WeaponPath + name + "_Main";
            BladeSpriteFileName = ModUtil.WeaponPath + name + "_Blade";
            SubSpriteFileName = ModUtil.WeaponPath + name + "_Sub";
            HeadSpriteFileName = ModUtil.WeaponPath + name + "_Head";
            return this;
        }
        public string Name { get; internal set; }
        public int Id { get; internal set; }
        public uint AssetId { get; internal set; }
        public WeaponEntity WeaponEntity { get; internal set; }
        public LocalizedString LocalizedName { get; internal set; }
        public GameObject MainWeaponPrefab { get; internal set; }
        public string MainSpriteFileName { get; internal set; }
        public string SubSpriteFileName { get; internal set; }
        public string BladeSpriteFileName { get; internal set; }
        public string HeadSpriteFileName { get; internal set; }
        public string IconFileName { get; internal set; }
        public Sprite Icon { get; internal set; }
        public int Dependency { get; internal set; }
        public int Copy { get; internal set; }
        public WeaponWieldEntity WeaponWieldEntity { get; internal set; }
        public List<int> StandardEnhancements { get; internal set; } = [];
        public Action<WeaponSimple> MainPrefabModifier { get; internal set; }
        public bool HasBladeSprite { get; internal set; }
        public Vector3? BladeSpritePosition { get; internal set; }
        public bool HasHeadSprite { get; internal set; }
        public Vector3? HeadSpritePosition { get; internal set; }

        public virtual void Init(int id, uint assetId)
        {
            Id = id;
            AssetId = assetId;
        }
        public virtual void Init(WeaponEntity copy)
        {
            Melon<Core>.Logger.Msg("CreateWeaponEntity from " + copy.name);
            WeaponWieldEntity = copy.wieldEntity;
            var main = UnityEngine.Object.Instantiate(copy.mainWeaponPrefab);
            main.name = "Weapon_" + Name;
            UnityEngine.Object.Destroy(main.GetComponent<NetworkIdentity>());

            if (main.TryGetComponent<WeaponSimple>(out var simple))
            {
                if (simple.mainWeaponBody != null)
                {
                    simple.mainWeaponBody.weaponSpriteRenderer.sprite = SpriteLoader.LoadSprite(MainSpriteFileName);
                    simple.mainWeaponBody.weaponStencilRenderer.sprite = SpriteLoader.LoadSprite(MainSpriteFileName);

                    if (HasBladeSprite && simple.mainWeaponBody.bladeAddOnRenderer != null)
                    {
                        simple.mainWeaponBody.bladeAddOnRenderer.sprite = SpriteLoader.LoadSprite(BladeSpriteFileName);
                        if (BladeSpritePosition.HasValue)
                            simple.mainWeaponBody.bladeAddOnRenderer.transform.localPosition = BladeSpritePosition.Value;
                    }
                    if (HasHeadSprite)
                    {
                        var head = simple.mainWeaponBody.weaponSpriteRenderer.transform.Find("Head");
                        if(head != null && head.gameObject.TryGetComponent<SpriteRenderer>(out var headSprite))
                        {
                            headSprite.sprite = SpriteLoader.LoadSprite(HeadSpriteFileName);
                            if (HeadSpritePosition.HasValue)
                                head.localPosition = HeadSpritePosition.Value;
                        }
                    }
                }
                if (simple.subWeaponBody != null)
                {
                    simple.subWeaponBody.weaponSpriteRenderer.sprite = SpriteLoader.LoadSprite(SubSpriteFileName);
                    simple.subWeaponBody.weaponStencilRenderer.sprite = SpriteLoader.LoadSprite(SubSpriteFileName);
                }
                if(simple.subWeapon != null && simple.subWeapon.gameObject.TryGetComponent<SubWeapon>(out var shield))
                {
                    shield.weaponSpriteRenderer.sprite = SpriteLoader.LoadSprite(SubSpriteFileName);
                    shield.weaponStencilRenderer.sprite = SpriteLoader.LoadSprite(SubSpriteFileName);
                }
                MainPrefabModifier?.Invoke(simple);
            }

            MainWeaponPrefab = main;

            WeaponEntity = CreateWeaponEntity();
        }
        public WeaponEntity CreateWeaponEntity()
        {
            var entity = ScriptableObject.CreateInstance<WeaponEntity>();
            entity.name = Id + "_" + Name;
            entity.id = Id;
            entity.aName = LocalizedName;
            entity.mainWeaponPrefab = MainWeaponPrefab;
            entity.icon = Icon ?? SpriteLoader.LoadSprite(IconFileName);
            entity.wieldEntity = WeaponWieldEntity;
            return entity;
        }
    }
}
