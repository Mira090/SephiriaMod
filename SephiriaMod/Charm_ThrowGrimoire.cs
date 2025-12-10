using MelonLoader;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod
{
    [Obsolete]
    public class Charm_ThrowGrimoire : Charm_Basic
    {
        public string damageId = "Charm_ThrowGrimoire";

        public float[] throwChanceByLevel = new float[1] { 50f };

        public float defaultChance = 10f;

        public GameObject BulletPrefab => SephiriaPrefabs.PallasBigBullet;

        public float bulletDamage = 100f;

        public int staggeringLevel = 1;

        public float externalForcePower = 1f;

        public Timer throwIntervalTimer = new Timer(0.1f, resetOnTime: false);

        public Sprite bulletSprite;
        private void Start()
        {
            bulletSprite = SpriteLoader.LoadSprite(ModUtil.MiscPath + "GrimoireBullet");
        }

        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            float num = 1f;
            if ((bool)avatar && avatar.TryGetComponent<WeaponControllerSimple>(out var component) && (bool)component.currentWeapon)
            {
                num = component.currentWeapon.AttackWeightPerSwing;
            }

            string value = (showAllLevel ? ((throwChanceByLevel.SafeRandomAccess(0) * num).ToString("0.#") + "→" + (throwChanceByLevel.SafeRandomAccess(maxLevel) * num).ToString("0.#") + "%") : ((throwChanceByLevel.SafeRandomAccess(LevelToIdx(level)) * num).ToString("0.#") + "%"));
            if ((bool)avatar)
            {
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

            return new Loc.KeywordValue[3]
            {
            new Loc.KeywordValue("DEFAULT", defaultChance.ToString()),
            new Loc.KeywordValue("CHANCE", value, Charm_Basic.GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("DAMAGE", bulletDamage.ToString())
            };
        }

        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            if ((bool)base.WeaponController)
            {
                WeaponControllerSimple weaponController = base.WeaponController;
                weaponController.OnBeginAttackAnimation += OnBeginAttackAnimation;
            }
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            if ((bool)base.WeaponController)
            {
                WeaponControllerSimple weaponController = base.WeaponController;
                weaponController.OnBeginAttackAnimation -= OnBeginAttackAnimation;
            }
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (!throwIntervalTimer.Check())
            {
                throwIntervalTimer.AddTimer(Time.deltaTime);
            }
        }

        private void OnBeginAttackAnimation(int idx)
        {
            if (!base.WeaponController || !base.WeaponController.currentWeapon || !throwIntervalTimer.Check())
            {
                return;
            }

            int count = UnityEngine.Random.Range(4, 10);
            float anglePer = 4f;
            float startAngle = anglePer * (float)(count - 1) * 0.5f;
            float angle = (base.WeaponController.aimedPositionClientside - base.WeaponController.transform.position).GetAngle();
            for (int q = 0; q < count; q++)
            {
                Vector3 vector3FromAngle = HorayUtility.GetVector3FromAngle(angle + (0f - startAngle + anglePer * q));
                Vector3 motionDataBegin = base.NetworkAvatar.transform.position + vector3FromAngle * 0.2f;
                Vector3 motionDataEnd = base.NetworkAvatar.transform.position + vector3FromAngle * 8f;
                bool flag = UnityEngine.Random.Range(0f, 1f) < 0.2f;
                //Melon<Core>.Logger.Msg("Prefab: " + (BulletPrefab != null));
                Bullet bullet = Bullet.Pool.Spawn(BulletPrefab, base.NetworkAvatar.transform.position, canBeTransparentOnMultiplayer: true, EDamageFromType.None, damageId, bulletDamage / (float)count, staggeringLevel, externalForcePower, base.NetworkAvatar, base.NetworkAvatar.GetHostileFactionLayers(EDamageFromType.None), base.NetworkAvatar.TopdownActor.CenterYPos, motionDataBegin, motionDataEnd, null, null);
                ModifyPrefab(bullet);
                Vector3 pos = base.NetworkAvatar.transform.position + (Vector3)(UnityEngine.Random.insideUnitCircle * 3f);
                bullet.SetSpeedScale(3);

                if (bullet.MoveModule is BulletMoveModule_FireworkHoming bulletMoveModule_FireworkHoming)
                {
                    //bulletMoveModule_FireworkHoming.TurnOnFakeTarget(pos);
                }
            }

            throwIntervalTimer.Ratio = 0f;
        }
        public void ModifyPrefab(Bullet bullet)
        {
            //Melon<Core>.Logger.Msg("0: " + (bullet != null));
            var wrapper = bullet.transform.GetChild(0);
            //Melon<Core>.Logger.Msg("1: " + (wrapper != null));
            var body = wrapper.GetChild(0);
            //Melon<Core>.Logger.Msg("2: " + (body != null));
            var animator = body.GetComponent<Animator2D_SpriteRenderer>();
            //Melon<Core>.Logger.Msg("3: " + (animator != null));
            //Melon<Core>.Logger.Msg("4: " + (animator.currentSet != null));
            var stateinfo = animator.currentSet.sprites;
            //Melon<Core>.Logger.Msg("5: " + (stateinfo != null));
            if (stateinfo.Count > 0)
            {
                var sprites = stateinfo[0].timeline;
                //Melon<Core>.Logger.Msg("6: " + (sprites.Count));
                for (int q = 0; q < sprites.Count; q++)
                {
                    sprites[q].sprite = bulletSprite;
                }
            }
            var key = animator.GetCurrentBakedKeyFrame();
            foreach(var time in key.timeline.Values)
            {
                for (int q = 0; q < time.Count; q++)
                {
                    time[q] = bulletSprite;
                }
            }
            var destroy = bullet.gameObject.GetComponent<BulletDestroyModule_DestroyImmediate>();
            //animator.ChangeSet(animator.currentSet);
        }

        public override bool Weaved()
        {
            return true;
        }
    }
}
