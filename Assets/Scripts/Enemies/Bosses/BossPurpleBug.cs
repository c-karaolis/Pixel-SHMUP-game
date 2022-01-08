using DG.Tweening;
using Foxlair.Enemies.Weapons;
using Foxlair.Tools;
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
        private float percentileChanceToAttack = 45f;
        public EnemySpaceshipWeapons spaceshipWeapons;
        private Vector3 splinePausedPosition;
        int currentPoint;
        public MoveToLocation moveToLocation;
        enum BossPurpleBugState
        {
            Idle,
            Moving,
            SpecialAttack,
        }

        private void Start()
        {
            InvokeRepeating("Attack",0f,1f);
            Invoke("SpecialAttack", 3f);
        }

        private void Update()
        {
            switch (state)
            {
                case BossPurpleBugState.Idle:
                    Idle();
                    break;
                case BossPurpleBugState.Moving:
                    Moving();
                    break;
                case BossPurpleBugState.SpecialAttack:
                    SpecialAttack();
                    break;
                default:
                    return;
            }
        }

        private void SpecialAttack()
        {

            splineMove.Pause();
            currentPoint = splineMove.currentPoint;
            Debug.Log(currentPoint);
            splinePausedPosition = transform.position;
            Debug.Log(splinePausedPosition);
            transform.DOMove(shootingPosition.transform.position, .5f);

            Invoke("ContinueMovement",3f);
            //
        }

        private void ContinueMovement()
        {
            //transform.DOMove(splinePausedPosition, .3f).OnComplete(() => splineMove.Resume());
            //transform.DOMove(splineMove.waypoints[currentPoint], 1f).OnComplete(() => splineMove.Resume());

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

        private void Awake()
        {
            spaceshipWeapons = GetComponent<EnemySpaceshipWeapons>();
        }

    }
}