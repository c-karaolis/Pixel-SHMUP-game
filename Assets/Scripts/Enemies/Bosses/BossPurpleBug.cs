using DG.Tweening;
using Foxlair.Enemies.Weapons;
using Foxlair.Tools;
using Foxlair.Tools.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foxlair.Enemies
{
    public class BossPurpleBug : BossSpaceship
    {
        private BossPurpleBugState state = BossPurpleBugState.Moving;
        List<BossPurpleBugState> bossStatesThatCanRandomAttack = new List<BossPurpleBugState>() { BossPurpleBugState.Moving };
        private float percentileChanceToAttack = 80f;
        public EnemySpaceshipWeapons spaceshipWeapons;
        private Vector3 splinePausedPosition;
        public MoveToLocation moveToLocation;
        EnemyHealth healthSystem;
        enum BossPurpleBugState
        {
            Idle,
            Moving,
            SpecialAttack,
        }

        private void Start()
        {
          healthSystem.onHealthDroppedBelow25  += OnHealthDroppedBelow25;
          healthSystem.onHealthDroppedBelow50  += OnHealthDroppedBelow50;
          healthSystem.onHealthDroppedBelow75  += OnHealthDroppedBelow75;
            InvokeRepeating("Attack",0f,1f);
            //Invoke("SpecialAttack", 10f);
        }

        private void Update()
        {
            //switch (state)
            //{
            //    case BossPurpleBugState.Idle:
            //        Idle();
            //        break;
            //    case BossPurpleBugState.Moving:
            //        Moving();
            //        break;
            //    case BossPurpleBugState.SpecialAttack:
            //        SpecialAttack();
            //        break;
            //    default:
            //        return;
            //}
        }

        private void SpecialAttack()
        {
            if (state != BossPurpleBugState.SpecialAttack)
            {

                ChangeState(BossPurpleBugState.SpecialAttack);
                splineMove.Pause();
                splinePausedPosition = transform.position;
                transform.DOMove(shootingPosition.transform.position, .5f).
                    OnComplete(() => { spaceshipWeapons.activeSpecialWeapon.Play(); });
            }
            else
            {
                CancelInvoke("ContinueMovement");
            }

            Invoke("ContinueMovement",15f);
        }

        private void ContinueMovement()
        {
            ChangeState(BossPurpleBugState.Moving);
            //transform.DOMove(splinePausedPosition, .3f).OnComplete(() => splineMove.Resume());
            //transform.DOMove(splineMove.waypoints[currentPoint], 1f).OnComplete(() => splineMove.Resume());
            spaceshipWeapons.DeactivateSpecialWeapon();
            moveToLocation.MoveTo(transform, splinePausedPosition, 5f);
        }

        public void ResumeSplinePath()
        {
            splineMove.Resume();
        }

        private void Attack()
        {
            if (Randomiser.RandomFixedPercentage(percentileChanceToAttack) && bossStatesThatCanRandomAttack.Contains(state))
            {
                spaceshipWeapons.activeMainWeapon.Play();
                spaceshipWeapons.activeMainWeapon.Reinitialize();

            }
        }

        private void Moving()
        {
        }

        private void Idle()
        {
        }

        void ChangeState(BossPurpleBugState newState)
        {
            state = newState;
        }

        private void Awake()
        {
            healthSystem = GetComponent<EnemyHealth>();
            spaceshipWeapons = GetComponent<EnemySpaceshipWeapons>();
        }

        public override void OnDeath()
        {
            FoxlairEventManager.Instance.EnemyHealthSystem_OnDeath_Event?.Invoke(this, enemyWave);
            spaceshipWeapons.activeMainWeapon.Kill();
            spaceshipWeapons.activeSpecialWeapon.Kill();
        }

        private void OnDestroy()
        {
            healthSystem.onHealthDroppedBelow25 -= OnHealthDroppedBelow25;
            healthSystem.onHealthDroppedBelow50 -= OnHealthDroppedBelow50;
            healthSystem.onHealthDroppedBelow75 -= OnHealthDroppedBelow75;
        }
        private void OnHealthDroppedBelow50()
        {
            SpecialAttack();
        }

        private void OnHealthDroppedBelow25()
        {
            SpecialAttack();
        }

        private void OnHealthDroppedBelow75()
        {
            SpecialAttack();
        }


    }
}