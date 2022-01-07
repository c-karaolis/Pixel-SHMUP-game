using BulletPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Foxlair.Enemies.Weapons
{
    public class EnemySpaceshipWeapons : MonoBehaviour
    {
        #region Fields
        public BulletEmitter activeMainWeapon = null;
        public BulletEmitter activeSpecialWeapon = null;
        public bool hasIdleWeapon = false;
        [SerializeField] private GameObject mainWeaponsParentObject = null;
        [SerializeField] private GameObject specialWeaponsParentObject = null;
        [SerializeField] private bool activateWeaponsOnStart = true;
        [SerializeField] private List<EmitterProfile> mainWeapons;
        [SerializeField] private List<EmitterProfile> availableSpecialWeapons;
        [SerializeField] private int activeMainWeaponIndex = 0;
        [SerializeField] private Dictionary<string, EmitterProfile> specialWeapons;

        #endregion

        private void OnValidate()
        {
            //mainWeapons = mainWeaponsParentObject.GetComponentsInChildren<BulletEmitter>();
            //availableSpecialWeapons = specialWeaponsParentObject.GetComponentsInChildren<BulletEmitter>().ToList();
        }

        private void Awake()
        {
            specialWeapons = new Dictionary<string, EmitterProfile>();

            for (int i = 0; i < availableSpecialWeapons.Count; i++)
            {
                specialWeapons.Add(availableSpecialWeapons[i].name, availableSpecialWeapons[i]);
            }

        }
        void Start()
        {
            activeMainWeaponIndex = 0;
            ActivateMainWeapon(mainWeapons[activeMainWeaponIndex], activateWeaponsOnStart);
        }

        public void Shoot(BulletEmitter weapon)
        {
       //     weapon.emitterProfile = new EmitterProfile();
            weapon.Play();
        }

        public void ChangeMainWeapon(EmitterProfile newWeapon)
        {
            activeMainWeapon.SwitchProfile(newWeapon);
        }

        public void ChangeSpecialWeapon(EmitterProfile newWeapon)
        {
            activeSpecialWeapon.SwitchProfile(newWeapon);
        }

        public void UpgradeMainWeapon()
        {
            if (activeMainWeaponIndex + 1 < mainWeapons.Count)
            {
                DeactivateMainWeapon();
                activeMainWeaponIndex++;

                ActivateMainWeapon(mainWeapons[activeMainWeaponIndex], !activeSpecialWeapon);
            }
        }

        private void ActivateMainWeapon(EmitterProfile mainWeapon, bool playOnActivate = true)
        {
            activeMainWeapon.SwitchProfile(mainWeapon);
            if (playOnActivate)
            {
                activeMainWeapon.Play();
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

        public void ActivateSpecialWeapon(string specialWeaponType="")
        {
            if (specialWeapons.ContainsKey(specialWeaponType))
            {
                DeactivateSpecialWeapon();
                activeMainWeapon.Stop();
                activeSpecialWeapon.SwitchProfile(specialWeapons[specialWeaponType]);
                activeSpecialWeapon.Play();
            }
        }

        public void DefaultSpecialWeaponTestActivate()
        {
            ActivateSpecialWeapon();
        }

        public void DeactivateSpecialWeapon()
        {
            if (activeSpecialWeapon != null)
            {
                activeSpecialWeapon.Stop();
            }

            activeSpecialWeapon = null;
            
            if(hasIdleWeapon)
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