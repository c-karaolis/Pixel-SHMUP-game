using BulletPro;
using Foxlair.Enums;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Foxlair.Player.Weapons
{
    public class SpaceshipWeapons : SerializedMonoBehaviour
    {
        #region Fields
        [ReadOnly] public BulletEmitter activeMainWeapon = null;
        [ReadOnly] public BulletEmitter activeSpecialWeapon = null;

        [SerializeField] [Required] private GameObject mainWeaponsParentObject = null;
        [SerializeField] [Required] private GameObject specialWeaponsParentObject = null;
        [SerializeField] private bool activateWeaponsOnStart = true;
        [SerializeField] private BulletEmitter[] mainWeapons;
        [SerializeField] private List<BulletEmitter> availableSpecialWeapons;
        [SerializeField] [ReadOnly] private int activeMainWeaponIndex = 0;
        [SerializeField] [ReadOnly] private Dictionary<SpecialWeaponTypes, BulletEmitter> specialWeapons;

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

        [Button]
        public void ActivateSpecialWeapon(SpecialWeaponTypes specialWeaponType)
        {
            if ((int)specialWeaponType <= availableSpecialWeapons.Count)
            {
                DeactivateSpecialWeapon();
                activeMainWeapon.Stop();
                activeSpecialWeapon = specialWeapons[specialWeaponType];
                activeSpecialWeapon.Play();
            }
        }

        [Button]
        private void DeactivateSpecialWeapon()
        {
            if (activeSpecialWeapon != null)
            {
                activeSpecialWeapon.Stop();
            }

            activeSpecialWeapon = null;
            activeMainWeapon.Play();
        }

        [Button]
        public void ResetMainWeapon()
        {
            DeactivateMainWeapon();
            activeMainWeaponIndex = 0;
            ActivateMainWeapon(mainWeapons[activeMainWeaponIndex]);
        }

        public void ResetSpecialWeapon()
        {
            activeSpecialWeapon = null;
        }
    }

}