using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spaceship : MonoBehaviour
{

    public abstract void OnSpaceshipHealthLost(float damage);

    public abstract void OnSpaceshipHealthGained(float healAmount);

    public abstract void OnSpaceshipDeath();

    public abstract void Die();
}
