using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaMod.StatusInstances
{
    public class StatusInstance_MaxMiracleCount : StatusInstance
    {
        protected override void SetTargetInner(UnitAvatar target)
        {
            base.SetTargetInner(target);
        }

        protected override void ApplyStatusInner(bool runtime)
        {
            base.ApplyStatusInner(runtime);
            if (CurrentTarget.gameObject.TryGetComponent<MiracleController>(out var miracle))
            {
                miracle.maxMiracleCount += Value;
            }
        }

        protected override void RemoveStatusInner()
        {
            base.RemoveStatusInner();
            if (CurrentTarget.gameObject.TryGetComponent<MiracleController>(out var miracle))
            {
                miracle.maxMiracleCount -= Value;
            }
        }
    }
}
