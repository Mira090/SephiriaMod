using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaMod.Miracles
{
    [Obsolete]
    public class Miracle_AddJewelry : Miracle_StatusInstance
    {
        protected override void SetOwnerInner(UnitAvatar owner)
        {
            base.SetOwnerInner(owner);
            this.AddRandomJewelry();
        }
        protected override void DestroyInner()
        {
            base.DestroyInner();
        }
    }
}
