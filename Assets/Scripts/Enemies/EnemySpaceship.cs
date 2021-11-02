using DG.Tweening;
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
            animator.SetTrigger("Die");
            if (deathSFX) audioSource.PlayOneShot(deathSFX);

        }

        public override void OnDeath()
        {
            FoxlairEventManager.Instance.EnemyHealthSystem_OnDeath_Event(this);
        }

        public override void OnDeathAnimationEnd()
        {
            Destroy(gameObject);
        }

        public override void OnHealthGained(float healAmount)
        {
            throw new System.NotImplementedException();
        }

        public override void OnHealthLost(float damage)
        {
            transform.DOShakePosition(0.1f,0.1f,2,90,false,false);
            animator.SetTrigger("Hit");

            //TODO: do X amount of damage to Enemies.
            //FoxlairEventManager.Instance.EnemyHealthSystem_OnHealthLost_Event(damage);
        }
    }
}