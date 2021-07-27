using BulletPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foxlair.Player
{
    public class SpaceshipWeapons : MonoBehaviour
    {

        private BulletEmitter activeMainWeapon = null;
        private BulletEmitter activeSpecialWeapon = null;

        private int activeMainWeaponIndex = 0;


        #region Getters
        GameObject MainWeaponsParentObject
        {
            get
            {
                return transform.Find("Main Weapons").gameObject;
            }
        }

        GameObject SpecialWeaponsParentObject
        {
            get
            {
                return transform.Find("Special Weapons").gameObject;
            }
        }

        BulletEmitter[] MainWeapons
        {
            get
            {
                return MainWeaponsParentObject.GetComponentsInChildren<BulletEmitter>();
            }
        }
        BulletEmitter[] SpecialWeapons
        {
            get
            {
                return SpecialWeaponsParentObject.GetComponentsInChildren<BulletEmitter>();
            }
        }
        #endregion
        //TODO maybe add on enable add to list in editor. Bring in Odin?

        void Start()
        {
            ActivateMainWeapon(MainWeapons[activeMainWeaponIndex]);
        }

        public void UpgradeMainWeapon()
        {
            DeactivateMainWeapon(MainWeapons[activeMainWeaponIndex]);
            activeMainWeaponIndex++;
            ActivateMainWeapon(MainWeapons[activeMainWeaponIndex]);
        }

        private void ActivateMainWeapon(BulletEmitter bulletEmitter)
        {
            activeMainWeapon = bulletEmitter;
            bulletEmitter.Play();
        }

        private void DeactivateMainWeapon(BulletEmitter bulletEmitter)
        {
            bulletEmitter.Stop();
            if (activeMainWeapon == bulletEmitter)
            {
                activeMainWeapon = null;
            }
        }

        public void ActivateSpecialWeapon(BulletEmitter bulletEmitter)
        {
            activeSpecialWeapon = bulletEmitter;
            bulletEmitter.Play();
        }

        public void ActivateSpecialWeapon(string specialWeaponName)
        {
            activeSpecialWeapon = SpecialWeaponsParentObject.transform.Find(specialWeaponName).GetComponent<BulletEmitter>();
            activeSpecialWeapon?.Play();
        }

        public void DeactivateSpecialWeapon(BulletEmitter bulletEmitter)
        {
            bulletEmitter.Stop();
            if (activeSpecialWeapon == bulletEmitter)
            {
                activeSpecialWeapon = null;
            }
        }
    }

}