using Foxlair.Interfaces;
using UnityEngine;

namespace Foxlair
{
    public abstract class Spaceship : MonoBehaviour, IHealthOwner
    {
        public GameObject deathVFX;
        public GameObject deathSFX;

        public abstract void OnHealthLost(float damage);

        public abstract void OnHealthGained(float healAmount);

        public abstract void OnDeath();

        public abstract void Die();
    }
}