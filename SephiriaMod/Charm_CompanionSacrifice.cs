using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod
{
    public class Charm_CompanionSacrifice : Charm_StatusInstance
    {
        public static LocalizedString Notice = new LocalizedString("Item_Companion_Sacrifice_Notice");
        public Sprite iconSprite;
        public int[] healPercent = [10, 20, 30, 40, 50];
        public bool ability = false;
        private int defense = 0;

        public int[] followerDamage = [20, 40, 60, 80, 120];
        public int[] followerDamage2 = [50, 75, 100, 120, 160];
        private void Awake()
        {
            showStatsEffectStringFirst = true;
        }
        public void Start()
        {
            iconSprite = SpriteLoader.LoadSprite(ModUtil.MiscPath + "Companion_Charm");
        }
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = (showAllLevel ? (healPercent.SafeRandomAccess(0) + "→" + healPercent.SafeRandomAccess(maxLevel)) : healPercent.SafeRandomAccess(LevelToIdx(level)).ToString());
            string value2 = (showAllLevel ? (followerDamage.SafeRandomAccess(0) + "→" + followerDamage.SafeRandomAccess(maxLevel)) : followerDamage.SafeRandomAccess(LevelToIdx(level)).ToString());
            string value3 = (showAllLevel ? (followerDamage2.SafeRandomAccess(0) + "→" + followerDamage2.SafeRandomAccess(maxLevel)) : followerDamage2.SafeRandomAccess(LevelToIdx(level)).ToString());
            return new Loc.KeywordValue[3]
            {
                new Loc.KeywordValue("HEAL", value + "%", Charm_Basic.GetPositiveColor(virtualLevelOffset)),
                new Loc.KeywordValue("FOLLOWER1", "+" + value2 + "%", Charm_Basic.GetPositiveColor(virtualLevelOffset)),
                new Loc.KeywordValue("FOLLOWER2", "+" + value3 + "%", Charm_Basic.GetPositiveColor(virtualLevelOffset))
            };
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            base.NetworkAvatar.OnDamagedServerside += Avatar_OnDamagedServerside;
            defense = NetworkAvatar.GetCustomStat(ECustomStat.DamageReduction);
            EnableDefenseEffects();
        }
        private void EnableDefenseEffects()
        {
            if (defense <= -25)
            {

            }
            if (defense <= -50)
            {
                NetworkAvatar.AddCustomStatUnsafe("FollowerDamage", followerDamage.SafeRandomAccess(CurrentLevelToIdx()));
            }
            if (defense <= -75)
            {

            }
            if (defense <= -100)
            {
                NetworkAvatar.AddCustomStatUnsafe("FollowerDamage", followerDamage2.SafeRandomAccess(CurrentLevelToIdx()));
            }
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            base.NetworkAvatar.OnDamagedServerside -= Avatar_OnDamagedServerside;
            DisableDefenseEffects();
        }
        private void DisableDefenseEffects()
        {
            if (defense <= -25)
            {

            }
            if (defense <= -50)
            {
                NetworkAvatar.AddCustomStatUnsafe("FollowerDamage", -followerDamage.SafeRandomAccess(CurrentLevelToIdx()));
            }
            if (defense <= -75)
            {

            }
            if (defense <= -100)
            {
                NetworkAvatar.AddCustomStatUnsafe("FollowerDamage", -followerDamage2.SafeRandomAccess(CurrentLevelToIdx()));
            }
        }

        private void Avatar_OnDamagedServerside(DamageInstance damage)
        {
            if (base.NetworkAvatar.hp <= 0f && !ability)
            {

                NewItemOwnInstance newItemOwnInstance = base.NetworkAvatar.Inventory.FindItem(new ItemPosition(base.Item.XIdx - 1, base.Item.YIdx));

                if (newItemOwnInstance == null || !newItemOwnInstance.Charm || !newItemOwnInstance.Charm.IsEffectEnabled)
                    return;
                if (newItemOwnInstance.Charm is not ICompanionCharm companion)
                    return;
                ability = true;
                SaveItemOnServer(SaveManager.CurrentRun);
                if ((bool)DungeonManager.Instance)
                {
                    DungeonManager.Instance.Chat(base.NetworkAvatar as PlayerAvatar, "Mod", "/sacrifice");
                }

                base.NetworkAvatar.Networkhp = 0f;
                base.NetworkAvatar.HealPercent(healPercent.SafeRandomAccess(CurrentLevelToIdx()));
                base.NetworkAvatar.StartReviveInvulnerable();
                using (new GridInventory.Permission(base.Inventory))
                {
                    base.Inventory.ForceRemoveItem((sbyte)(xIdx - 1), yIdx);
                }
            }
        }
        public override string GetEffectString(int idx, int level, int virtualLevelOffset, bool showAllLevel)
        {
            if (ability && idx == 3)
                return null;
            return base.GetEffectString(idx, level, virtualLevelOffset, showAllLevel);
        }
        public override void SaveItemOnServer(ISaveData saveData)
        {
            base.SaveItemOnServer(saveData);
            saveData.SetBool($"CharmSaveData_CompanionSacrifice_{base.Item.InstanceID}_Stack", ability);
        }
        public override void LoadItemOnServer(ISaveData saveData)
        {
            base.LoadItemOnServer(saveData);
            ability = saveData.GetBool($"CharmSaveData_CompanionSacrifice_{base.Item.InstanceID}_Stack", false);
        }
        public override void OnCharmEffectRefreshed()
        {
            base.OnCharmEffectRefreshed();
            base.Inventory.UpdatePing(base.Item.Position);
            var d = NetworkAvatar.GetCustomStat(ECustomStat.DamageReduction);
            if(d != defense)
            {
                DisableDefenseEffects();
                defense = d;
                EnableDefenseEffects();
            }
        }
        public override CharmConnectionData[] GetConnectedCharmPositions()
        {
            if (!IsEffectEnabled)
            {
                return null;
            }

            NewItemOwnInstance newItemOwnInstance = base.NetworkAvatar.Inventory.FindItem(new ItemPosition((sbyte)(base.Item.XIdx + 1), (sbyte)(base.Item.YIdx)));
            if (newItemOwnInstance == null || !newItemOwnInstance.Charm || !newItemOwnInstance.Charm.IsEffectEnabled)
            {
                return new CharmConnectionData[1]
                {
                new CharmConnectionData(iconSprite, new Vector2Int(base.Item.XIdx - 1, base.Item.YIdx))
                };
            }
            if (newItemOwnInstance.Charm is not ICompanionCharm)
            {
                return new CharmConnectionData[1]
                {
                new CharmConnectionData(iconSprite, new Vector2Int(base.Item.XIdx - 1, base.Item.YIdx))
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

            NewItemOwnInstance newItemOwnInstance = base.NetworkAvatar.Inventory.FindItem(new ItemPosition(pos.x - 1, pos.y));
            if (newItemOwnInstance != null && (bool)newItemOwnInstance.Charm && newItemOwnInstance.Charm.IsEffectEnabled)
            {
                if (newItemOwnInstance.Charm is ICompanionCharm companion)
                    return companion.GetCompanionIcon(newItemOwnInstance.Charm.CurrentLevelToIdx());
            }

            return null;
        }

        public override Vector2 GetSubIconImageOffset(int index)
        {
            return new Vector2(4f, -4f);
        }
    }
}
