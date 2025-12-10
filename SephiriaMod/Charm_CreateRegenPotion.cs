using HeathenEngineering.SteamworksIntegration.API;
using System;
using System.Collections.Generic;
using System.Text;
using static MelonLoader.MelonLogger;

namespace SephiriaMod
{
    public class Charm_CreateRegenPotion : Charm_StatusInstance
    {
        public int count = 0;
        public int require = 4;
        public PlayerAvatar playerAvatar;
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            return new Loc.KeywordValue[3]
            {
                new Loc.KeywordValue("REQUIRE", require.ToString()),
            new Loc.KeywordValue("ITEM", new LocalizedString("Item_PotionOfRegeneration_Name").ToString()),
            new Loc.KeywordValue("CURRENT", count.ToString(), Charm_Basic.GetPositiveColor(virtualLevelOffset))
            };
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            playerAvatar = base.NetworkAvatar as PlayerAvatar;
            if (playerAvatar != null)
            {
                playerAvatar.OnEnteredFloorServerside += HandleEnterFloor;
            }
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            if (playerAvatar != null)
            {
                playerAvatar.OnEnteredFloorServerside -= HandleEnterFloor;
            }

            playerAvatar = null;
            //base.NetworkAvatar.RemoveShield(shieldId);
        }

        private void HandleEnterFloor(string f)
        {
            if (!base.NetworkAvatar.IsDead)
            {
                count++;
                if(count == require)
                {
                    count = 0;
                    var random = new System.Random(Item.InstanceID);

                    Inventory.AddItem(new ItemMetadata(ItemDatabase.GenerateInstanceID(random), 0, 1));
                }
                SaveItemOnServer(SaveManager.CurrentRun);
            }
        }
        public override void SaveItemOnServer(ISaveData saveData)
        {
            base.SaveItemOnServer(saveData);
            saveData.SetInt($"CharmSaveData_CreateRegenPotion_{base.Item.InstanceID}_Stack", count);
        }

        public override void LoadItemOnServer(ISaveData saveData)
        {
            base.LoadItemOnServer(saveData);
            count = saveData.GetInt($"CharmSaveData_CreateRegenPotiony_{base.Item.InstanceID}_Stack", 0);
        }
    }
}
