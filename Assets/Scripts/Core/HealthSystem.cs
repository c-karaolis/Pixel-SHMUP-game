using Foxlair.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace Foxlair.Core
{
    public class HealthSystem : MonoBehaviour
    {

        [SerializeField] float health;
        [SerializeField] float maxHealth;

        [Header("Min/Max damage and armor")]
        [SerializeField] private readonly float minDamageCanReceive = 1f;
        [SerializeField] private readonly float maxDamageCanReceive = 999f;
        [SerializeField] private readonly float armor = 0f;

        [Header("Customisation per unit")]
        [SerializeField] bool unitRegeneratesHealth = false;
        [SerializeField] float healthRegeneration;
        [SerializeField] bool unitHasHealthbar = false;
        public Image healthBar;



        [SerializeField] IHealthOwner healthOwner;

        private void OnValidate()
        {
            healthOwner = GetComponent<IHealthOwner>();

            if (maxHealth == 0f)
            {
                maxHealth = 50f;
            }

            health = maxHealth;

        }

        public void TakeDamage(float damage)
        {
            damage -= armor;
            if (damage < minDamageCanReceive) { damage = minDamageCanReceive; }
            if (damage > maxDamageCanReceive) { damage = maxDamageCanReceive; }
            if (health - damage <= 0)
            {
                health = 0;

                healthOwner.OnHealthLost(damage);
                Die();
            }
            else
            {
                health -= damage;
                healthOwner.OnHealthLost(damage);
            }
        }


        public void TakeDamage(float damage, IHealthOwner _spaceship)
        {
            damage -= armor;

            if (health - damage <= 0)
            {
                health = 0;

                healthOwner.OnHealthLost(damage);
                Die();
            }
            else
            {
                health -= damage;
                healthOwner.OnHealthLost(damage);
                //spaceship.lastAttacker = _spaceship;
            }

        }

        public void Heal(float healAmount)
        {
            if (health + healAmount <= maxHealth)
            {
                health += healAmount;
                healthOwner.OnHealthGained(healAmount);
            }
            else
            {
                health = maxHealth;
                healthOwner.OnHealthGained(healAmount);
            }
        }

        protected void Die()
        {
            healthOwner.OnDeath();
            healthOwner.Die();
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

            healthOwner.OnHealthGained(regeneratedAmount);

        }
    }
}