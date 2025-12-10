using HeathenEngineering.SteamworksIntegration.API;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod
{
    public class Charm_BondMaker : Charm_StatusInstance
    {
        private int requireLevel = 3;
        private bool quest = false;
        private bool reward = false;
        private Sprite charmSprite;
        private void Start()
        {
            charmSprite = SpriteLoader.LoadSprite(ModUtil.MiscPath + "DealUIArtifact");
        }
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            return new Loc.KeywordValue[1]
            {
            new Loc.KeywordValue("LEVEL", requireLevel.ToString())
            };
        }
        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (quest && !reward)
            {
                CreateBond();
            }
        }
        protected override void OnUpdatedLevel(int oldLevel, int newLevel)
        {
            base.OnUpdatedLevel(oldLevel, newLevel);
            quest = LevelToIdx(newLevel) >= requireLevel && CanCreateBond();
        }
        private bool CanCreateBond()
        {
            ItemPosition pos1 = new ItemPosition(base.Item.XIdx - 1, base.Item.YIdx);
            ItemPosition pos2 = new ItemPosition(base.Item.XIdx + 1, base.Item.YIdx);
            GridInventory inventory = base.Inventory;
            var item1 = inventory.FindItem(pos1);
            var item2 = inventory.FindItem(pos2);

            if (item1 != null && (bool)item1.Charm && item1.Charm.IsEffectEnabled && item1.Entity.categories.Count > 0)
            {
                if (item2 != null && (bool)item2.Charm && item2.Charm.IsEffectEnabled && item1.Entity.categories.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }
        private void CreateBond()
        {
            ItemPosition pos = new ItemPosition(base.Item.XIdx, base.Item.YIdx);
            ItemPosition pos1 = new ItemPosition(base.Item.XIdx - 1, base.Item.YIdx);
            ItemPosition pos2 = new ItemPosition(base.Item.XIdx + 1, base.Item.YIdx);
            GridInventory inventory = base.Inventory;
            int instanceID = base.Item.InstanceID;
            var item1 = inventory.FindItem(pos1);
            var item2 = inventory.FindItem(pos2);
            if (item1 != null && (bool)item1.Charm && item1.Charm.IsEffectEnabled)
            {
                if (item2 != null && (bool)item2.Charm && item2.Charm.IsEffectEnabled)
                {
                    var categories = new List<string>();
                    categories.AddRange(item1.Entity.categories);
                    categories.AddRange(item2.Entity.categories);
                    var list = new List<ItemEntity>();
                    foreach(var item in ReflectionExtensions.GetItemDictionary().Values)
                    {
                        if (item.isDual && item.categories.All(c => categories.Contains(c)))
                        {
                            list.Add(item);
                        }
                    }
                    if (list.Count > 0)
                    {
                        using (new GridInventory.Permission(inventory))
                        {
                            inventory.ForceRemoveItem(pos.x, pos.y);
                            inventory.ForceRemoveItem(pos1.x, pos1.y);
                            inventory.ForceRemoveItem(pos2.x, pos2.y);
                        }
                        inventory.AddItemAtPosition(new ItemMetadata(instanceID, list.GetRandom(), 1), pos);
                        reward = true;
                    }
                }

            }
        }
        public override void OnCharmEffectRefreshed()
        {
            base.OnCharmEffectRefreshed();
            base.Inventory.UpdatePing(base.Item.Position);
        }

        public override CharmConnectionData[] GetConnectedCharmPositions()
        {
            if (!IsEffectEnabled)
            {
                return null;
            }
            var list = new List<CharmConnectionData>();

            NewItemOwnInstance newItemOwnInstance = base.NetworkAvatar.Inventory.FindItem(new ItemPosition((sbyte)(base.Item.XIdx - 1), (sbyte)(base.Item.YIdx)));
            if (newItemOwnInstance == null || !newItemOwnInstance.Charm || !newItemOwnInstance.Charm.IsEffectEnabled)
            {
                list.Add(new CharmConnectionData(charmSprite, new Vector2Int(base.Item.XIdx - 1, base.Item.YIdx)));
            }
            NewItemOwnInstance newItemOwnInstance2 = base.NetworkAvatar.Inventory.FindItem(new ItemPosition((sbyte)(base.Item.XIdx + 1), (sbyte)(base.Item.YIdx)));
            if (newItemOwnInstance2 == null || !newItemOwnInstance2.Charm || !newItemOwnInstance2.Charm.IsEffectEnabled)
            {
                list.Add(new CharmConnectionData(charmSprite, new Vector2Int(base.Item.XIdx + 1, base.Item.YIdx)));
            }

            if (list.Count == 0)
                return null;
            return list.ToArray();
        }

        public override int GetSubIconCount()
        {
            return 0;//2以上にするとエラーが出る
        }

        public override Sprite GetSubIconImage(ItemPosition pos, bool isInstance, int idx)
        {
            if (!isInstance)
            {
                return null;
            }

            if (!base.NetworkAvatar)
            {
                return null;
            }

            ItemPosition p = idx == 0 ? new ItemPosition((sbyte)(pos.x - 1), (pos.y)) : new ItemPosition((sbyte)(pos.x + 1), (pos.y));
            NewItemOwnInstance newItemOwnInstance = base.NetworkAvatar.Inventory.FindItem(p);
            if (newItemOwnInstance != null && (bool)newItemOwnInstance.Charm && newItemOwnInstance.Charm.IsEffectEnabled)
            {
                return newItemOwnInstance.Entity.Icon;
            }

            return null;
        }

        public override Vector2 GetSubIconImageOffset(int index)
        {
            return index == 0 ? new Vector2(-4f, 0) : new Vector2(4f, 0);
        }
        public override bool Weaved()
        {
            return true;
        }
    }
}
