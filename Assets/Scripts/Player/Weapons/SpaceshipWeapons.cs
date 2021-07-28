using BulletPro;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Foxlair.Player
{
    public class SpaceshipWeapons : MonoBehaviour
    {
        #region Fields
        [ReadOnly] public BulletEmitter activeMainWeapon = null;
        [ReadOnly] public BulletEmitter activeSpecialWeapon = null;

        [SerializeField] [Required] private GameObject mainWeaponsParentObject = null;
        [SerializeField] [Required] private GameObject specialWeaponsParentObject = null;
        [SerializeField] private bool activateWeaponsOnStart = true;
        [SerializeField] BulletEmitter[] mainWeapons;
        [SerializeField] private List<BulletEmitter> specialWeapons;
        //Dictionary<string,BulletEmitter> 
        [SerializeField] [ReadOnly] private int activeMainWeaponIndex = 0;
        #endregion

        private void OnValidate()
        {
            mainWeapons = mainWeaponsParentObject.GetComponentsInChildren<BulletEmitter>();
            specialWeapons = specialWeaponsParentObject.GetComponentsInChildren<BulletEmitter>().ToList();
        }
        void Start()
        {
            if (activateWeaponsOnStart)
            {
                activeMainWeaponIndex = 0;
                ActivateMainWeapon(mainWeapons[activeMainWeaponIndex]);
            }
        }

        [Button]
        public void UpgradeMainWeapon()
        {
            if (activeMainWeaponIndex + 1 < mainWeapons.Length)
            {
                DeactivateMainWeapon(mainWeapons[activeMainWeaponIndex]);
                activeMainWeaponIndex++;
                ActivateMainWeapon(mainWeapons[activeMainWeaponIndex]);
            }
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
            if(activeSpecialWeapon != null)
            {
                activeSpecialWeapon.Stop();
                activeSpecialWeapon = null;
            }
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

        [Button]
        public void ResetMainWeapon()
        {
            DeactivateMainWeapon(mainWeapons[activeMainWeaponIndex]);
            activeMainWeaponIndex = 0;
            ActivateMainWeapon(mainWeapons[activeMainWeaponIndex]);
        }

        public void ResetSpecialWeapon()
        {
            activeSpecialWeapon = null;
        }
    }

}