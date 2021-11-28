using Foxlair.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foxlair.Enemies
{
    public class BossSpaceship : Spaceship, IEnemy
    {
        public override void Die()
        {
            throw new System.NotImplementedException();
        }

        public override void OnDeath()
        {
            throw new System.NotImplementedException();
        }

        public override void OnDeathAnimationEnd()
        {
            throw new System.NotImplementedException();
        }

        public override void OnHealthGained(float healAmount)
        {
            throw new System.NotImplementedException();
        }

        public override void OnHealthLost(float damage)
        {
            throw new System.NotImplementedException();
        }
    }
}
