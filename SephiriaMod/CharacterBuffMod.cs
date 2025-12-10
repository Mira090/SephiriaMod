using Mirror;
using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaMod
{
    public abstract class CharacterBuffMod : CharacterBuff
    {
        public uint AssetId;
        public string id = "CUSTOM_";
        public override string ID => id;
        protected override void InitializeInner(UnitAvatar target, float amplified)
        {
            base.InitializeInner(target, amplified);
            enabled = true;
            var identity = gameObject.AddComponent<NetworkIdentity>();
            identity.SetAssetId(AssetId);
        }
    }
}
