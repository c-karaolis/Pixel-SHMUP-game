using DG.Tweening;
using Foxlair.Interfaces;
using Foxlair.StageStructure;
using Foxlair.Tools.Events;
using SWS;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Foxlair.Enemies
{
    public class EnemySpaceship : Spaceship, IEnemy
    {
        #region Fields
        public EnemyFormationWave enemyWave;
        SplineMove splineMove;
        public Slot occupiedSlot;
        #endregion

        #region Methods
        void Awake()
        {
            splineMove = GetComponent<SplineMove>();
            splineMove.movementEndEvent += OnMovementEnd;
        }

        public override void Start()
        {
            base.Start();
            AssignToSlot();
        }
        private void AssignToSlot()
        {
            List<Slot> slots = enemyWave.formation.slots;
            foreach (Slot slot in slots)
            {
                if (slot.enemySpaceship == null)
                {
                    occupiedSlot = slot;
                    occupiedSlot.enemySpaceship = this;
                    break;
                }

            }

        }

        public override void Die()
        {
            //GameObject deathVisualEffects = Instantiate(deathVFX, transform);
            splineMove.ChangeSpeed(splineMove.speed / 3);
            if(bulletReceiver != null)
            bulletReceiver.enabled = false;
            if(animator != null)
            animator.SetTrigger("Die");
            if (deathSFX && audioSource) audioSource.PlayOneShot(deathSFX);
        }

        public override void OnDeath()
        {
            if(occupiedSlot)
            occupiedSlot.enemySpaceship = null;
            FoxlairEventManager.Instance.EnemyHealthSystem_OnDeath_Event?.Invoke(this,enemyWave);
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
            //transform.DOShakePosition(0.1f, 0.05f, 2, 90, false, true);
            //if(animator)
            //animator.SetTrigger("Hit");
            //TODO: do X amount of damage to Enemies.
            //FoxlairEventManager.Instance.EnemyHealthSystem_OnHealthLost_Event(damage);
        }

        void OnMovementEnd()
        {
            GoToSlot();


        }
        void OnReachedSlot()
        {
            transform.parent = occupiedSlot.transform;
            FoxlairEventManager.Instance.Enemy_OnReachedSlot_Event?.Invoke(this, occupiedSlot);
        }
        void GoToSlot()
        {
            transform.DOMove(occupiedSlot.transform.position,1f).OnComplete(OnReachedSlot);
        }


        private void OnDestroy()
        {
            splineMove.movementEndEvent -= OnMovementEnd;
        }
    }
    #endregion
}