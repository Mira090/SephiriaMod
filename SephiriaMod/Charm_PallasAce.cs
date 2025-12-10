using Mirror;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static MelonLoader.MelonLogger;

namespace SephiriaMod
{
    public class Charm_PallasAce : Charm_Basic
    {
        [Header("PallasCard")]
        public string damageId = "Charm_PallasCard";

        public float[] throwChanceByLevel = [0f, 2.5f, 5f, 7.5f, 10f];

        public float defaultChance = 50f;

        public GameObject[] bulletBigPrefab => SephiriaPrefabs.PallasBigBullets;

        public GameObject[] bulletSmallPrefab => SephiriaPrefabs.PallasSmallBullets;

        public float bulletDamage = 24f;

        public int staggeringLevel = 1;

        public float externalForcePower = 1f;

        public Timer throwIntervalTimer = new Timer(0.1f, resetOnTime: false);

        private void Awake()
        {
            SetEnhancement(false);
        }
        public bool GetEnhancement(GridInventory inventory)
        {

            ItemPosition[] array = Charm_PallasJoker.Directions;
            foreach (ItemPosition itemPosition in array)
            {
                NewItemOwnInstance newItemOwnInstance = inventory.FindItem(new ItemPosition(xIdx, yIdx) + itemPosition);
                if (newItemOwnInstance != null)
                {
                    Charm_Basic charm = newItemOwnInstance.Charm;
                    if ((bool)charm && charm is Charm_PallasJoker)
                        return true;
                }
            }
            return false;
        }
        public void SetEnhancement(bool enhance)
        {
            if (enhance)
            {
                bulletDamage = 64;
                defaultChance = 100;
                throwChanceByLevel = [0f, 5f, 10f, 15f, 20f];
                //pallas.throwChanceByLevel = [0f, 4f, 8f, 12f, 16f];
                throwIntervalTimer.time = 0.05f;
            }
            else
            {
                bulletDamage = 32;
                defaultChance = 50;
                throwChanceByLevel = [0f, 2.5f, 5f, 7.5f, 10f];
                throwIntervalTimer.time = 0.1f;
            }
        }
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            float num = 1f;
            if ((bool)avatar && avatar.TryGetComponent<WeaponControllerSimple>(out var component) && (bool)component.currentWeapon)
            {
                num = component.currentWeapon.AttackWeightPerSwing;
            }

            if ((bool)avatar && !ignoreAvatarStatus)
            {
                if (!NetworkServer.active)
                {
                    SetEnhancement(GetEnhancement(NetworkAvatar.Inventory));
                }
                string value = (showAllLevel ? ((throwChanceByLevel.SafeRandomAccess(0) * num).ToString("0.#") + "→" + (throwChanceByLevel.SafeRandomAccess(maxLevel) * num).ToString("0.#") + "%") : ((throwChanceByLevel.SafeRandomAccess(LevelToIdx(level)) * num).ToString("0.#") + "%"));
                float num2 = defaultChance * num;
                float num3 = num2 + throwChanceByLevel.SafeRandomAccess(LevelToIdx(level)) * (float)Mathf.Clamp(avatar.GetCustomStat(ECustomStat.Luck), 0, 9999) * num;
                return new Loc.KeywordValue[4]
                {
                new Loc.KeywordValue("DEFAULT", num2.ToString("0.#")),
                new Loc.KeywordValue("CHANCE", value, Charm_Basic.GetPositiveColor(virtualLevelOffset)),
                new Loc.KeywordValue("DAMAGE", bulletDamage.ToString()),
                new Loc.KeywordValue("CURRENT", num3.ToString("0.#") + "%")
                };
            }
            else
            {
                string value = (showAllLevel ? ((throwChanceByLevel.SafeRandomAccess(0) * num).ToString("0.#") + "→" + (throwChanceByLevel.SafeRandomAccess(maxLevel) * num).ToString("0.#") + "%") : ((throwChanceByLevel.SafeRandomAccess(LevelToIdx(level)) * num).ToString("0.#") + "%"));
                return new Loc.KeywordValue[4]
                {
            new Loc.KeywordValue("DEFAULT", defaultChance.ToString()),
            new Loc.KeywordValue("CHANCE", value, Charm_Basic.GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("DAMAGE", bulletDamage.ToString()),
            new Loc.KeywordValue("CURRENT", "-%")
                };
            }

        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            WeaponController.OnSwingCreated_Dash += OnDashAttack;
        }
        private void OnDashAttack(ProjectileBase projectile)
        {
            ModEvent.InvokeOnAceSpawnChance(0, this);
            if (!base.WeaponController || !base.WeaponController.currentWeapon || !throwIntervalTimer.Check())
            {
                return;
            }

            float num = defaultChance + throwChanceByLevel.SafeRandomAccess(CurrentLevelToIdx()) * (float)Mathf.Clamp(base.NetworkAvatar.GetCustomStat(ECustomStat.Luck), 0, 9999);
            num *= base.WeaponController.currentWeapon.AttackWeightPerSwing;
            if (UnityEngine.Random.Range(0f, 1f) > num / 100f)
            {
                return;
            }

            int num2 = 3;
            float num3 = 30f;
            float num4 = num3 * (float)(num2 - 1) * 0.5f;
            float angle = (base.WeaponController.aimedPositionClientside - base.WeaponController.transform.position).GetAngle();
            for (int i = 0; i < num2; i++)
            {
                Vector3 vector3FromAngle = HorayUtility.GetVector3FromAngle(angle + (0f - num4 + num3 * (float)i));
                Vector3 vector = base.NetworkAvatar.transform.position + vector3FromAngle * 0.2f;
                Vector3 vector2 = base.NetworkAvatar.transform.position + vector3FromAngle * 8f;
                bool flag = UnityEngine.Random.Range(0f, 1f) < 0.2f;
                Bullet bullet = Bullet.Pool.Spawn(flag ? bulletBigPrefab.GetRandom() : bulletSmallPrefab.GetRandom(), base.NetworkAvatar.transform.position, canBeTransparentOnMultiplayer: true, EDamageFromType.None, damageId, bulletDamage / (float)num2 * (flag ? 2f : 1f), staggeringLevel, externalForcePower, base.NetworkAvatar, base.NetworkAvatar.GetHostileFactionLayers(EDamageFromType.None), base.NetworkAvatar.TopdownActor.CenterYPos, vector, vector2, null, null);
                bullet.TurnOnHoming(autoDetectTarget: true, null, UnityEngine.Random.Range(0.1f, 0.5f));
                Vector3 pos = base.NetworkAvatar.transform.position + (Vector3)(UnityEngine.Random.insideUnitCircle * 3f);
                if (bullet.MoveModule is BulletMoveModule_FireworkHoming bulletMoveModule_FireworkHoming)
                {
                    bulletMoveModule_FireworkHoming.TurnOnFakeTarget(pos);
                }
            }

            throwIntervalTimer.Ratio = 0f;
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            WeaponController.OnSwingCreated_Dash -= OnDashAttack;
        }
        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (!throwIntervalTimer.Check())
            {
                throwIntervalTimer.AddTimer(Time.deltaTime);
            }
        }

        public override bool Weaved()
        {
            return true;
        }
    }
}
