using System;
using Foxlair.Achievements;
using Foxlair.Enemies;

namespace Foxlair.Tools.Events
{
    public class FoxlairEventManager : PersistentSingletonMonoBehaviour<FoxlairEventManager>
    {

        #region Achievements
        public Action<Achievement> Achievements_Achieved_Event;
        #endregion

        #region Currency Events
        public Action Currency_OnCurrencyChanged_Event;
        #endregion

        #region Enemy Events
        public Action<float> EnemyHealthSystem_OnHealthGained_Event;
        public Action<float> EnemyHealthSystem_OnHealthLost_Event;
        public Action<EnemySpaceship> EnemyHealthSystem_OnDeath_Event;
        public Action<EnemySpaceship> Enemy_OnBirth_Event;
        public Action<EnemySpaceship, Slot> Enemy_OnReachedSlot_Event;
        #endregion

        #region Player Events
        public Action<float> PlayerHealthSystem_OnHealthGained_Event;
        public Action<float> PlayerHealthSystem_OnHealthLost_Event;
        public Action PlayerHealthSystem_OnPlayerDeath_Event;
        #endregion

        #region Weapon System
        //public Action<Weapon> WeaponSystem_OnWeaponEquipped_Event;
        //public Action<Weapon> WeaponSystem_OnWeaponUnEquipped_Event;
        //public Action WeaponSystem_OnEquippedWeaponDestroyed_Event;
        #endregion

    }


}