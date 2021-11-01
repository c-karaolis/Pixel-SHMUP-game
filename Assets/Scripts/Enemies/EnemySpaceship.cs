using Foxlair.Tools.Events;
using UnityEngine;

namespace Foxlair.Enemies
{
    public class EnemySpaceship : Spaceship
    {
        
        public override void Die()
        {
            //GameObject deathVisualEffects = Instantiate(deathVFX, transform);
            bulletReceiver.enabled = false;
            audioSource.PlayOneShot(deathSFX);
            animator.SetTrigger("Die");
        }

        public override void OnDeath()
        {
            Debug.Log(this.name + "died");
        }

        public override void OnDeathAnimationEnd()
        {
            Debug.Log("Test animation event");
            Destroy(gameObject);
        }

        public override void OnHealthGained(float healAmount)
        {
            throw new System.NotImplementedException();
        }

        public override void OnHealthLost(float damage)
        {
            //TODO: do X amount of damage to Enemies.
            //FoxlairEventManager.Instance.EnemyHealthSystem_OnHealthLost_Event(damage);
        }
    }
}