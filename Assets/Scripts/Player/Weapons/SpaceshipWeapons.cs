using BulletPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Foxlair.Player
{
    public class SpaceshipWeapons : MonoBehaviour
    {
        #region Fields
        public BulletEmitter activeMainWeapon = null;
        public BulletEmitter activeSpecialWeapon = null;

        [SerializeField]
        private GameObject mainWeaponsParentObject = null;
        [SerializeField]
        private GameObject specialWeaponsParentObject = null;
        [SerializeField]
        private bool activateWeaponsOnStart = true;
        [SerializeField]
        BulletEmitter[] mainWeapons;
        [SerializeField]
        private List<BulletEmitter> specialWeapons;

        private int activeMainWeaponIndex = 0;
        #endregion

        private void OnValidate()
        {
            if (mainWeaponsParentObject == null || specialWeaponsParentObject == null)
            {
                Debug.LogError("Make sure to map the weapon parent objects for this game object");
                return;
            }

            mainWeapons = mainWeaponsParentObject.GetComponentsInChildren<BulletEmitter>();
            specialWeapons = specialWeaponsParentObject.GetComponentsInChildren<BulletEmitter>().ToList();
        }
        void Start()
        {
            if (activateWeaponsOnStart)
            {
                ActivateMainWeapon(mainWeapons[activeMainWeaponIndex]);
            }
        }

        public void UpgradeMainWeapon()
        {
            DeactivateMainWeapon(mainWeapons[activeMainWeaponIndex]);
            activeMainWeaponIndex++;
            ActivateMainWeapon(mainWeapons[activeMainWeaponIndex]);
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
            activeSpecialWeapon = specialWeaponsParentObject.transform.Find(specialWeaponName).GetComponent<BulletEmitter>();
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