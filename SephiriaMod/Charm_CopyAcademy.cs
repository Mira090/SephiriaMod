using MelonLoader;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod
{
    public class Charm_CopyAcademy : Charm_StatusInstance
    {
        public LocalizedString semanticName = new LocalizedString("ItemType_Charm_Magic");

        public string semantic = "GRIMOIRE";

        public int semanticDropWeight = 2;

        private int count;
        private int countView;
        private int countRequire = 20;

        private bool questCleared;
        private bool rewardReceived;
        public Sprite magicCharmIconSprite;
        private void Awake()
        {
            effectHUD_ID = "Copy_Academy".ToSephiriaUpperId();
        }
        public void Start()
        {
            magicCharmIconSprite = SpriteLoader.LoadSprite(ModUtil.MiscPath + "Academy_Charm");
            ModEvent.OnValueRecieved += OnValueRecieved;
        }
        public void OnDestroy()
        {
            ModEvent.OnValueRecieved -= OnValueRecieved;
        }
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            //var maxHP = avatar.MaxHp;
            //string value = (showAllLevel ? (levelByLevel.SafeRandomAccess(0) + "→" + levelByLevel.SafeRandomAccess(maxLevel)) : levelByLevel.SafeRandomAccess(LevelToIdx(level)).ToString());
            //string value2 = (showAllLevel ? (Mathf.RoundToInt(maxHP * 0.01f * damageByLevel.SafeRandomAccess(0)) + "→" + Mathf.RoundToInt(maxHP * 0.01f * damageByLevel.SafeRandomAccess(maxLevel))) : Mathf.RoundToInt(maxHP * 0.01f * damageByLevel.SafeRandomAccess(LevelToIdx(level))).ToString());
            return new Loc.KeywordValue[4]
            {
            new Loc.KeywordValue("ITEM_TYPE", semanticName.ToString()),
            new Loc.KeywordValue("DROP_PERCENT", (semanticDropWeight * 50).ToString()),
                new Loc.KeywordValue("QUEST", countRequire.ToString()),
            //new Loc.KeywordValue("REWARD",value, Charm_Basic.GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("CURRENT", countView.ToString(), Charm_Basic.GetPositiveColor(virtualLevelOffset))
            };
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            base.NetworkAvatar.Inventory.AddItemDropBonusBySemantic(semantic, semanticDropWeight);
            base.NetworkAvatar.SetEffectHUDValue(GetCharmHUDID(), $"{countView}/{countRequire}");
        }

        private void OnValueRecieved(string command, uint netId, int value)
        {
            //Melon<Core>.Logger.Msg("OnValueRecieved: " + netId + " to " + base.netId);
            if(netId == base.netId)
            {
                countView = value;
            }
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            ModEvent.OnValueRecieved -= OnValueRecieved;
            base.NetworkAvatar.Inventory.AddItemDropBonusBySemantic(semantic, -semanticDropWeight);
        }
        public override void OnCharmEffectRefreshed()
        {
            base.OnCharmEffectRefreshed();
            base.Inventory.UpdatePing(base.Item.Position);
        }
        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (questCleared && !rewardReceived)
            {
                ItemPosition pos = new ItemPosition(base.Item.XIdx, base.Item.YIdx);
                GridInventory inventory = base.Inventory;
                int instanceID = base.Item.InstanceID;
                var random = new System.Random(instanceID);

                using (new GridInventory.Permission(inventory))
                {
                    inventory.ForceRemoveItem(base.Item.XIdx, base.Item.YIdx);
                }
                var item = inventory.FindItem(new ItemPosition(pos.x, pos.y + 1));
                if (item != null && (bool)item.Charm && item.Charm.IsEffectEnabled && !item.Charm.isUniqueEffect && item.Entity.categories.Contains(ItemCategories.Academy))
                {
                    //Melon<Core>.Logger.Msg("add: " + item.Entity.aName.ToString());
                    inventory.AddItemAtPosition(new ItemMetadata(instanceID, item.Entity, 1), pos);
                }
                rewardReceived = true;
            }
        }
        protected override void OnConnected(int instanceID)
        {
            base.OnConnected(instanceID);
            UnitAvatar networkAvatar = base.NetworkAvatar;
            if(networkAvatar is PlayerAvatar player)
            {
                var skill = player.GetSkillController();
                if(skill != null)
                {
                    skill.OnBeginCastMagicServerside += OnBeginCastMagicServerside;
                }
                else
                {
                    Melon<Core>.Logger.Msg("could not get SkillController");
                }
            }
            //networkAvatar.OnMpUsedServerside += OnMpUsedServerside;
        }

        private void OnBeginCastMagicServerside()
        {
            if (questCleared)
                return;
            if (base.NetworkAvatar.IsDead || !base.NetworkAvatar.IsInBattle || !IsEffectEnabled)
                return;
            if (GetConnectedCharmPositions() != null)
                return;
            count++;
            countView = count;
            ModEvent.CommandValue(NetworkAvatar, Item, count);
            questCleared = count >= countRequire;
            base.NetworkAvatar.SetEffectHUDValue(GetCharmHUDID(), $"{count}/{countRequire}");
            base.NetworkAvatar.SetEffectHUDFlash(GetCharmHUDID());
            SaveItemOnServer(SaveManager.CurrentRun);
        }

        private void OnMpUsedServerside(int mp)
        {
            if (questCleared)
                return;
            if (base.NetworkAvatar.IsDead || !base.NetworkAvatar.IsInBattle || !IsEffectEnabled)
                return;
            if (GetConnectedCharmPositions() != null)
                return;
            count += mp;
            questCleared = count >= countRequire;
            base.NetworkAvatar.SetEffectHUDValue(GetCharmHUDID(), $"{count}/{countRequire}");
            base.NetworkAvatar.SetEffectHUDFlash(GetCharmHUDID());
            SaveItemOnServer(SaveManager.CurrentRun);
        }

        protected override void OnDisconnected()
        {
            base.OnDisconnected();
            UnitAvatar networkAvatar = base.NetworkAvatar;
            if (networkAvatar is PlayerAvatar player)
            {
                var skill = player.GetSkillController();
                if (skill != null)
                {
                    skill.OnBeginCastMagicServerside -= OnBeginCastMagicServerside;
                }
                else
                {
                    Melon<Core>.Logger.Msg("could not get SkillController");
                }
            }
            //networkAvatar.OnMpUsedServerside -= OnMpUsedServerside;
        }

        public override void SaveItemOnServer(ISaveData saveData)
        {
            base.SaveItemOnServer(saveData);
            saveData.SetInt($"CharmSaveData_CopyAcademy_{base.Item.InstanceID}_Stack", count);
        }

        public override void LoadItemOnServer(ISaveData saveData)
        {
            base.LoadItemOnServer(saveData);
            count = saveData.GetInt($"CharmSaveData_CopyAcademy_{base.Item.InstanceID}_Stack", 0);
            countView = count;
            ModEvent.CommandValue(NetworkAvatar, Item, countView);
        }
        public override CharmConnectionData[] GetConnectedCharmPositions()
        {
            if (!IsEffectEnabled)
            {
                return null;
            }

            NewItemOwnInstance newItemOwnInstance = base.NetworkAvatar.Inventory.FindItem(new ItemPosition(base.Item.XIdx, (sbyte)(base.Item.YIdx + 1)));
            if (newItemOwnInstance == null || !newItemOwnInstance.Charm || !newItemOwnInstance.Charm.IsEffectEnabled || newItemOwnInstance.Charm.isUniqueEffect || !newItemOwnInstance.Entity.categories.Contains(ItemCategories.Academy))
            {
                return new CharmConnectionData[1]
                {
                new CharmConnectionData(magicCharmIconSprite, new Vector2Int(base.Item.XIdx, base.Item.YIdx + 1))
                };
            }

            return null;
        }

        public override int GetSubIconCount()
        {
            return 1;
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

            NewItemOwnInstance newItemOwnInstance = base.NetworkAvatar.Inventory.FindItem(new ItemPosition(pos.x, (sbyte)(pos.y + 1)));
            if (newItemOwnInstance != null && (bool)newItemOwnInstance.Charm && newItemOwnInstance.Charm.IsEffectEnabled && !newItemOwnInstance.Charm.isUniqueEffect && newItemOwnInstance.Entity.categories.Contains(ItemCategories.Academy))
            {
                return newItemOwnInstance.Entity.Icon;
            }

            return null;
        }

        public override Vector2 GetSubIconImageOffset(int index)
        {
            return new Vector2(4f, -4f);
        }

        public override bool Weaved()
        {
            return true;
        }
    }
}
