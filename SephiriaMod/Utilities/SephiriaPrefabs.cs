using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Utilities
{
    public class SephiriaPrefabs
    {
        public static CharacterDebuff Burn => CombatManager.Instance.burnDebuffPrefab;
        public static CharacterDebuff Electric => CombatManager.Instance.electricDebuffPrefab;
        public static CharacterDebuff Frostbite => CombatManager.Instance.frostbiteDebuffPrefab;
        public static CharacterDebuff Wound
        {
            get
            {
                if(_wound == null)
                    _wound = WeaponDatabase.FindWeaponById(1107).mainWeaponPrefab.GetComponent<WeaponAddonCommon_DebuffAttack>().debuffPrefab;
                return _wound;
            }
        }
        private static CharacterDebuff _wound;
        public static CharacterDebuff Poison
        {
            get
            {
                if (_poison == null)
                    _poison = WeaponDatabase.FindWeaponById(119).mainWeaponPrefab.GetComponent<WeaponAddonCommon_DebuffAttack>().debuffPrefab;
                return _poison;
            }
        }
        private static CharacterDebuff _poison;
        private static GameObject _pallasBigBullet;
        public static GameObject PallasBigBullet
        {
            get
            {
                if(_pallasBigBullet == null)
                {
                    _pallasBigBullet = ItemDatabase.FindItemById(1172).resourcePrefab.GetComponent<Charm_PallasCard>().bulletBigPrefab[0];
                }
                return _pallasBigBullet;
            }
        }
        private static GameObject _meteorBullet;
        public static GameObject MeteorBullet
        {
            get
            {
                if (_meteorBullet == null)
                {
                    _meteorBullet = ItemDatabase.FindItemById(1032).resourcePrefab.GetComponent<Charm_FlameGround_Meteor>().bulletPrefab;
                }
                return _meteorBullet;
            }
        }
        private static GameObject[] _pallasBigBullets;
        public static GameObject[] PallasBigBullets
        {
            get
            {
                if (_pallasBigBullets == null)
                {
                    _pallasBigBullets = ItemDatabase.FindItemById(1172).resourcePrefab.GetComponent<Charm_PallasCard>().bulletBigPrefab;
                }
                return _pallasBigBullets;
            }
        }
        private static GameObject[] _pallasSmallBullets;
        public static GameObject[] PallasSmallBullets
        {
            get
            {
                if (_pallasSmallBullets == null)
                {
                    _pallasSmallBullets = ItemDatabase.FindItemById(1172).resourcePrefab.GetComponent<Charm_PallasCard>().bulletSmallPrefab;
                }
                return _pallasSmallBullets;
            }
        }
    }
}
