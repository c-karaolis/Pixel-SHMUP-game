using BulletPro;
using System;
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
       
        private GameObject m_mainWeaponsParentObject = null;
        private GameObject m_specialnWeaponsParentObject = null;

        [SerializeField]
        private bool activateWeaponsOnStart = true;

        #region Getters
        GameObject MainWeaponsParentObject
        {
            get
            {
                return m_mainWeaponsParentObject;
            }
        }

        GameObject SpecialWeaponsParentObject
        {
            get
            {
                return m_specialnWeaponsParentObject;
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

        private void OnValidate()
        {
            try
            {
                m_mainWeaponsParentObject = transform.Find("Main Weapons").gameObject;
                m_specialnWeaponsParentObject = transform.Find("Special Weapons").gameObject;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to fetch weapon arrays with error: {e.Message}. \n Make sure Main Weapons and Special Weapons game objects exist under player ship.");
            }
        }
        void Start()
        {
            if (activateWeaponsOnStart)
            {
                ActivateMainWeapon(MainWeapons[activeMainWeaponIndex]);
            }
        }

        public void UpgradeMainWeapon()
        {
            DeactivateMainWeapon(MainWeapons[activeMainWeaponIndex]);
            activeMainWeaponIndex++;
            ActivateMainWeapon(MainWeapons[activeMainWeaponIndex]);
        }

        private void ActivateMainWeapon(BulletEmitter mainWeapon)
        {
            activeMainWeapon = mainWeapon;
            mainWeapon.Play();
        }

        private void DeactivateMainWeapon(BulletEmitter mainWeapon)
        {
            mainWeapon.Stop();
            if (activeMainWeapon == mainWeapon)
            {
                activeMainWeapon = null;
            }
        }

        public void ActivateSpecialWeapon(BulletEmitter specialWeapon)
        {
            activeSpecialWeapon = specialWeapon;
            specialWeapon.Play();
        }

        public void ActivateSpecialWeapon(string specialWeaponName)
        {
            activeSpecialWeapon = SpecialWeaponsParentObject.transform.Find(specialWeaponName).GetComponent<BulletEmitter>();
            activeSpecialWeapon.Play();
        }

        public void DeactivateSpecialWeapon(BulletEmitter specialWeapon)
        {
            specialWeapon.Stop();
            if (activeSpecialWeapon == specialWeapon)
            {
                activeSpecialWeapon = null;
            }
        }

        public void ResetMainWeapon()
        {
            activeMainWeaponIndex = 0;
        }

        public void ResetSpecialWeapon()
        {
            activeSpecialWeapon = null;
        }
    }

}