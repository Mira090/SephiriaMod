using Mirror;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod
{
    public class Charm_CreateStoneTablet : Charm_VariableMaxLevel
    {
        private int rewardId = 2001;//石版「希望」
        private int count;
        private int countView;
        private int countRequire = 10;
        private int useMax = 6;
        [SyncVar]
        private int useCount;
        private int useCountView;

        private bool questCleared;
        private System.Random random = new System.Random();
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            //var maxHP = avatar.MaxHp;
            //string value = (showAllLevel ? (damageByLevel.SafeRandomAccess(0) + "→" + damageByLevel.SafeRandomAccess(maxLevel)) : damageByLevel.SafeRandomAccess(LevelToIdx(level)).ToString());
            //string value2 = (showAllLevel ? (Mathf.RoundToInt(maxHP * 0.01f * damageByLevel.SafeRandomAccess(0)) + "→" + Mathf.RoundToInt(maxHP * 0.01f * damageByLevel.SafeRandomAccess(maxLevel))) : Mathf.RoundToInt(maxHP * 0.01f * damageByLevel.SafeRandomAccess(LevelToIdx(level))).ToString());
            return new Loc.KeywordValue[5]
            {
                new Loc.KeywordValue("QUEST", countRequire.ToString()),
            new Loc.KeywordValue("REWARD", new LocalizedString("Item_StoneTablet_Hope_Name").ToString()),
            new Loc.KeywordValue("CURRENT", countView.ToString(), Charm_Basic.GetPositiveColor(virtualLevelOffset)),
                new Loc.KeywordValue("MAX", useMax.ToString()),
                new Loc.KeywordValue("COUNT", useCountView.ToString()),
            };
        }
        public override string GetEffectString(int idx, int level, int virtualLevelOffset, bool showAllLevel)
        {
            if (useCountView >= useMax && idx == 0)
                return null;
            if (useCountView >= useMax && idx == 1)
                return null;
            return base.GetEffectString(idx, level, virtualLevelOffset, showAllLevel);
        }
        private void Awake()
        {
            effectHUD_ID = "CREATESTONETABLET";
        }
        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (useCount >= useMax)
                return;
            if (questCleared)
            {
                ItemPosition pos = new ItemPosition(base.Item.XIdx, base.Item.YIdx);
                GridInventory inventory = base.Inventory;
                int instanceID = base.Item.InstanceID;

                useCount++;
                useCountView = useCount;
                ModEvent.CommandValue(NetworkAvatar, Item, useCountView);
                using (new GridInventory.Permission(inventory))
                {
                    base.Inventory.AddDungeonTempLevel(pos, -1);
                    //if (useCount >= useMax)
                        //inventory.ForceRemoveItem(base.Item.XIdx, base.Item.YIdx);
                }
                inventory.AddItem(new ItemMetadata(ItemDatabase.GenerateInstanceID(random), rewardId, 1));
                count = 0;
                questCleared = false;

                if (Item != null)
                    SaveItemOnServer(SaveManager.CurrentRun);
            }
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.OnKillUnit += OnKillUnit;
            base.NetworkAvatar.SetEffectHUDValue(GetCharmHUDID(), $"{countView}/{countRequire}");
        }

        protected void OnKillUnit(UnitAvatar avatar, DamageInstance damage)
        {
            if (questCleared)
                return;
            count++;
            countView = count % countRequire;
            ModEvent.CommandValue(NetworkAvatar, Item, countView);
            questCleared = count >= countRequire;
            base.NetworkAvatar.SetEffectHUDValue(GetCharmHUDID(), $"{countView}/{countRequire}");
            base.NetworkAvatar.SetEffectHUDFlash(GetCharmHUDID());
            SaveItemOnServer(SaveManager.CurrentRun);
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.OnKillUnit -= OnKillUnit;
        }

        public override void SaveItemOnServer(ISaveData saveData)
        {
            base.SaveItemOnServer(saveData);
            saveData.SetInt($"CharmSaveData_CreateStoneTablet_{base.Item.InstanceID}_Stack", count);
            saveData.SetInt($"CharmSaveData_CreateStoneTablet_{base.Item.InstanceID}_Stack2", useCount);
        }

        public override void LoadItemOnServer(ISaveData saveData)
        {
            base.LoadItemOnServer(saveData);
            count = saveData.GetInt($"CharmSaveData_CreateStoneTablet_{base.Item.InstanceID}_Stack", 0);
            useCount = saveData.GetInt($"CharmSaveData_CreateStoneTablet_{base.Item.InstanceID}_Stack2", 0);
            countView = count;
            ModEvent.CommandValue(NetworkAvatar, Item, countView);
            useCountView = useCount;
            ModEvent.CommandValue(NetworkAvatar, Item, useCountView);
        }
    }
}
