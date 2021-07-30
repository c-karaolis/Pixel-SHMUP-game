using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foxlair.Interfaces
{
    public interface IHealthOwner
    {
        public abstract void OnHealthLost(float damage);

        public abstract void OnHealthGained(float healAmount);

        public abstract void OnDeath();

        public abstract void Die();
    }
}