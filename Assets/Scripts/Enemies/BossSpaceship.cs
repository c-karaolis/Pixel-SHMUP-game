using Foxlair.Interfaces;
using Foxlair.StageStructure;
using Foxlair.Tools.Events;
using SWS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foxlair.Enemies
{
    public class BossSpaceship : Spaceship, IEnemy
    {
        #region Fields
        public BossWave enemyWave;
        SplineMove splineMove;
        private BossState state;

        #endregion


        enum BossState
        {
            Idle,
            Moving,
            Attack,
            SpecialAttack,
        }


        private void Update()
        {
            switch (state)
            {
                case BossState.Idle:
                    Idle();
                    break;
                case BossState.Moving:
                    Moving();
                    break;
                case BossState.Attack:
                    Attack();
                    break;
                case BossState.SpecialAttack:
                    SpecialAttack();
                    break;
                default:
                    return;
            }
        }

        private void SpecialAttack()
        {
            throw new NotImplementedException();
        }

        private void Attack()
        {
            throw new NotImplementedException();
        }

        private void Moving()
        {
            throw new NotImplementedException();
        }

        private void Idle()
        {
            throw new NotImplementedException();
        }

      
      
        #region Methods
        void Awake()
        {
            splineMove = GetComponent<SplineMove>();
            splineMove.movementEndEvent += OnMovementEnd;
        }

        public override void Start()
        {
            base.Start();
        }
       
        public override void Die()
        {
            //GameObject deathVisualEffects = Instantiate(deathVFX, transform);
            splineMove.ChangeSpeed(splineMove.speed / 3);
            if (bulletReceiver != null)
                bulletReceiver.enabled = false;
            if (animator != null)
                animator.SetTrigger("Die");
            if (deathSFX && audioSource) audioSource.PlayOneShot(deathSFX);
        }

        public override void OnDeath()
        {
            FoxlairEventManager.Instance.EnemyHealthSystem_OnDeath_Event?.Invoke(this, enemyWave);
        }

        public override void OnDeathAnimationEnd()
        {
            Destroy(gameObject);
        }

        public override void OnHealthGained(float healAmount)
        {
        }

        public override void OnHealthLost(float damage)
        {
        }

        void OnMovementEnd()
        {
        }

        private void OnDestroy()
        {
            splineMove.movementEndEvent -= OnMovementEnd;
        }
    }
    #endregion
}

