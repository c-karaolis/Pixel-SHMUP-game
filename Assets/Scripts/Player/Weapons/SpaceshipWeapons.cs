using BulletPro;
using Foxlair.Enums;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Foxlair.Player.Weapons
{
    public class SpaceshipWeapons : MonoBehaviour
    {
        #region Fields
        public BulletEmitter activeMainWeapon = null;
        public BulletEmitter activeSpecialWeapon = null;

        [SerializeField] private GameObject mainWeaponsParentObject = null;
        [SerializeField] private GameObject specialWeaponsParentObject = null;
        [SerializeField] private bool activateWeaponsOnStart = true;
        [SerializeField] private BulletEmitter[] mainWeapons;
        [SerializeField] private List<BulletEmitter> availableSpecialWeapons;
        [SerializeField] private int activeMainWeaponIndex = 0;
        [SerializeField] private Dictionary<SpecialWeaponTypes, BulletEmitter> specialWeapons;

        #endregion

        private void OnValidate()
        {
            specialWeapons = new Dictionary<SpecialWeaponTypes, BulletEmitter>();

            mainWeapons = mainWeaponsParentObject.GetComponentsInChildren<BulletEmitter>();
            availableSpecialWeapons = specialWeaponsParentObject.GetComponentsInChildren<BulletEmitter>().ToList();

            for (int i = 0; i < availableSpecialWeapons.Count; i++)
            {
                specialWeapons.Add((SpecialWeaponTypes)i, availableSpecialWeapons[i]);
            }
        }

        void Start()
        {
            activeMainWeaponIndex = 0;

            ActivateMainWeapon(mainWeapons[activeMainWeaponIndex], activateWeaponsOnStart);

        }

        public void UpgradeMainWeapon()
        {
            if (activeMainWeaponIndex + 1 < mainWeapons.Length)
            {
                DeactivateMainWeapon();
                activeMainWeaponIndex++;

                ActivateMainWeapon(mainWeapons[activeMainWeaponIndex], !activeSpecialWeapon);
            }
        }

        private void ActivateMainWeapon(BulletEmitter mainWeapon, bool playOnActivate = true)
        {
            activeMainWeapon = mainWeapon;
            if (playOnActivate)
            {
                mainWeapon.Play();
            }
        }

        private void DeactivateMainWeapon()
        {
            if (activeMainWeapon != null)
            {
                activeMainWeapon.Stop();
            }

            activeMainWeapon = null;
        }

        public void ActivateSpecialWeapon(SpecialWeaponTypes specialWeaponType = 0 )
        {
            if ((int)specialWeaponType <= availableSpecialWeapons.Count)
            {
                DeactivateSpecialWeapon();
                activeMainWeapon.Stop();
                activeSpecialWeapon = specialWeapons[specialWeaponType];
                activeSpecialWeapon.Play();
            }
        }

        public void DefaultSpecialWeaponTestActivate()
        {
            DeactivateSpecialWeapon();
            activeMainWeapon.Stop();
            activeSpecialWeapon = specialWeapons[0];
            activeSpecialWeapon.Play();
        }

        public void DeactivateSpecialWeapon()
        {
            if (activeSpecialWeapon != null)
            {
                activeSpecialWeapon.Stop();
            }

            activeSpecialWeapon = null;
            activeMainWeapon.Play();
        }

        public void ResetMainWeapon()
        {
            DeactivateMainWeapon();
            activeMainWeaponIndex = 0;
            ActivateMainWeapon(mainWeapons[activeMainWeaponIndex], !activeSpecialWeapon);
        }

        public void ResetSpecialWeapon()
        {
            activeSpecialWeapon = null;
        }
    }

}