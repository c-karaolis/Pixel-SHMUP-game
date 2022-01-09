using Foxlair.Interfaces;
using System;
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

        public Action onHealthDroppedBelow75;
        public Action onHealthDroppedBelow50;
        public Action onHealthDroppedBelow25;
        bool healthBelow75Invoked = false;
        bool healthBelow50Invoked = false;
        bool healthBelow25Invoked = false;

        [SerializeField]public IHealthOwner healthOwner;
              
        private void Awake()
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
            if (healthOwner == null)
            {
                Debug.LogWarning("EMPTY HEALTH OWNER");
                return;
            }
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
                if ((health / maxHealth) * 100 <= 25 && !healthBelow25Invoked)
                {
                    onHealthDroppedBelow25?.Invoke();
                    healthBelow25Invoked=true;
                }
                else if ((health / maxHealth) * 100 <= 50 && !healthBelow50Invoked)
                {
                    onHealthDroppedBelow50?.Invoke();
                    healthBelow50Invoked=true;
                }
                else if ((health / maxHealth) * 100  <= 75 && !healthBelow75Invoked)
                {
                    onHealthDroppedBelow75?.Invoke();
                    healthBelow75Invoked=true;
                }
                
                
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