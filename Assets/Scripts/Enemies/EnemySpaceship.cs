using DG.Tweening;
using Foxlair.Tools.Events;
using SWS;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Foxlair.Enemies
{
    public class EnemySpaceship : Spaceship
    {
        #region Fields
        EnemyWave enemyWave;
        SplineMove splineMove;
        public Slot occupiedSlot;
        #endregion

        #region Methods
        void Awake()
        {
            splineMove = GetComponent<SplineMove>();
            splineMove.movementEndEvent += OnMovementEnd;
            AssignToSlot();
        }

        private void AssignToSlot()
        {
           // if (!enemyWave.formation)
           //     return;
           // List<Slot> slots = enemyWave.formation.GetSlots();
           //foreach (Slot slot in slots)
           // {
           //     if (slot.enemySpaceship)
           //         continue;

           //     occupiedSlot = slot;
           //     occupiedSlot.enemySpaceship = this;
           // }
        }

        public override void Die()
        {
            //GameObject deathVisualEffects = Instantiate(deathVFX, transform);
            splineMove.ChangeSpeed(splineMove.speed / 3);
            bulletReceiver.enabled = false;
            animator.SetTrigger("Die");
            if (deathSFX) audioSource.PlayOneShot(deathSFX);
        }

        public override void OnDeath()
        {
            FoxlairEventManager.Instance.EnemyHealthSystem_OnDeath_Event(this);
        }

        public override void OnDeathAnimationEnd()
        {
            Destroy(gameObject);
        }

        public override void OnHealthGained(float healAmount)
        {
            throw new System.NotImplementedException();
        }

        public override void OnHealthLost(float damage)
        {
            transform.DOShakePosition(0.1f, 0.05f, 2, 90, false, true);
            animator.SetTrigger("Hit");
            //TODO: do X amount of damage to Enemies.
            //FoxlairEventManager.Instance.EnemyHealthSystem_OnHealthLost_Event(damage);
        }

        void OnMovementEnd()
        {
            Debug.Log("Movement end");
            GoToSlot();
        }
        void GoToSlot()
        {
            transform.DOMove(occupiedSlot.transform.position,1f);
        }


        private void OnDestroy()
        {
            splineMove.movementEndEvent -= OnMovementEnd;
        }
    }
    #endregion
}