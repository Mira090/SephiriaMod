using MelonLoader;
using Mirror;
using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Items.Jewelry
{
    public class Charm_Jewelry : Charm_VariableMaxLevel
    {
        public int[] consumeMedium = [5];
        public int[] consumeSmall = [2];
        public virtual int[] Consume => consumeSmall;
        public virtual int MoneyPerLevel => 200;
        public override string StatusName => string.Empty;
        public int moneyLevel = -1;
        protected virtual bool ConsumeAll => true;
        protected override void OnConnected(int instanceID)
        {
            base.OnConnected(instanceID);
            if (!NetworkServer.active)
                return;
            LoadItemOnServer(instanceID, SaveManager.CurrentRun);
            if (moneyLevel < 0)
            {
                var money = NetworkAvatar.Money;
                moneyLevel = money / MoneyPerLevel;
                SetAdditionalMaxLevel(moneyLevel);
                if (ConsumeAll)
                {
                    NetworkAvatar.SetMoney(0);
                }
                else
                {
                    var max = Mathf.Min(moneyLevel, ValiableMax - OriginalMaxLevel);
                    var remain = money - max * MoneyPerLevel;
                    NetworkAvatar.SetMoney(remain);
                }
                SaveItemOnServer(instanceID, SaveManager.CurrentRun);

                var instance = StatusDatabase.CreateStatusEntity("JewelryCount".ToSephiriaId(), 1);
                NetworkAvatar.AddOrphanedStatusInstance(instance);

                RpcSetAdditionalMaxLevel(moneyLevel);
            }
            else
            {
                SetAdditionalMaxLevel(moneyLevel);
                RpcSetAdditionalMaxLevel(moneyLevel);
            }
        }
        protected virtual int FirstLevel => 2;
        protected virtual int SecondLevel => 0;
        public override int ValiableMax => 5;
        public override string GetEffectString(int idx, int level, int virtualLevelOffset, bool showAllLevel)
        {
            if (idx == 0 && level < FirstLevel)
                return "<color=#666666>" + base.GetEffectString(idx, level, virtualLevelOffset, showAllLevel) + "</color>";
            if (idx == 1 && level < SecondLevel)
                return "<color=#666666>" + base.GetEffectString(idx, level, virtualLevelOffset, showAllLevel) + "</color>";
            return base.GetEffectString(idx, level, virtualLevelOffset, showAllLevel);
        }
        public override void SaveItemOnServer(ISaveData saveData)
        {
            if (Item == null)
                return;
            base.SaveItemOnServer(saveData);
            saveData.SetInt($"CharmSaveData_Jewelry_{Item?.InstanceID}_Stack", moneyLevel);
        }
        public void SaveItemOnServer(int instanceID, ISaveData saveData)
        {
            if (Item == null)
                return;
            base.SaveItemOnServer(saveData);
            saveData.SetInt($"CharmSaveData_Jewelry_{instanceID}_Stack", moneyLevel);
        }

        public override void LoadItemOnServer(ISaveData saveData)
        {
            if (Item == null)
                return;
            base.LoadItemOnServer(saveData);
            moneyLevel = saveData?.GetInt($"CharmSaveData_Jewelry_{Item?.InstanceID}_Stack", -1) ?? -1;
        }
        public void LoadItemOnServer(int instanceID, ISaveData saveData)
        {
            if (Item == null)
                return;
            base.LoadItemOnServer(saveData);
            moneyLevel = saveData?.GetInt($"CharmSaveData_Jewelry_{instanceID}_Stack", -1) ?? -1;
        }

        
    }
}
