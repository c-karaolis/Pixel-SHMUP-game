using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Foxlair
{
    public class HealthSystem : MonoBehaviour
    {

        float health;
        public float maxHealth;

        [Header("Min/Max damage and armor")]
        public float minDamageCanReceive = 1f;
        public float maxDamageCanReceive = 999f;
        public float armor;

        [Header("Customisation per unit")]
        public bool unitRegeneratesHealth = false;
        public bool unitHasHealthbar = false;
        public float healthRegeneration;


        Spaceship spaceship;

        public Image healthBar;

        private void Start()
        {
            spaceship = GetComponent<Spaceship>();
            maxHealth = 50f;
            health = maxHealth;
            healthRegeneration = 0.1f;
        }



        public void TakeDamage(float damage)
        {
            damage -= armor;
            if (damage < minDamageCanReceive) { damage = minDamageCanReceive; }

            if (health - damage <= 0)
            {
                health = 0;

                spaceship.OnSpaceshipHealthLost(damage);
                Die();
            }
            else
            {
                health -= damage;
                spaceship.OnSpaceshipHealthLost(damage);
            }
        }


        public void TakeDamage(float damage, Spaceship _spaceship)
        {
            damage -= armor;

            if (health - damage <= 0)
            {
                health = 0;

                spaceship.OnSpaceshipHealthLost(damage);
                Die();
            }
            else
            {
                health -= damage;
                spaceship.OnSpaceshipHealthLost(damage);
                //spaceship.lastAttacker = _spaceship;
            }

        }

        public void Heal(float healAmount)
        {
            if (health + healAmount <= maxHealth)
            {
                health += healAmount;
                spaceship.OnSpaceshipHealthGained(healAmount);
            }
            else
            {
                health = maxHealth;
                spaceship.OnSpaceshipHealthGained(healAmount);
            }
        }

        protected void Die()
        {
            spaceship.OnSpaceshipDeath();
            spaceship.Die();
            //Debug.Log($"Actor({actor.name}) Died");
            //Destroy(gameObject);
        }

        protected void Update()
        {
            if (unitRegeneratesHealth)
            {
                RegenerateHealth();
            }
            if (unitHasHealthbar)
            {
                HandleHealthBar();
            }
        }

        private void HandleHealthBar()
        {
            if (healthBar == null)
            {
                Debug.LogWarning($"{gameObject}: This Unit has no healthbar assigned. Either add it or untick the hasHealthbar bool");
                return;
            }

            healthBar.fillAmount = health / maxHealth;
        }

        protected void RegenerateHealth()
        {
            float regeneratedAmount;

            if (health == maxHealth)
            {
                regeneratedAmount = 0;
            }
            else if (health > maxHealth)
            {
                regeneratedAmount = 0;
                health = maxHealth;
            }
            else
            {
                regeneratedAmount = healthRegeneration;
                health += healthRegeneration * Time.deltaTime;
            }

            spaceship.OnSpaceshipHealthGained(regeneratedAmount);

        }
    }
}