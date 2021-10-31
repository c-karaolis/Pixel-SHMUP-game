using BulletPro;
using Foxlair.Interfaces;
using UnityEngine;

namespace Foxlair
{
    [RequireComponent(typeof(Animator), typeof(AudioSource), typeof(BulletReceiver))]
    public abstract class Spaceship : MonoBehaviour, IHealthOwner
    {
        public Animator animator;
        public AudioSource audioSource;
        public BulletReceiver bulletReceiver;

        public abstract void OnHealthLost(float damage);

        public abstract void OnHealthGained(float healAmount);

        public abstract void OnDeath();

        public abstract void Die();

        public virtual void Start()
        {
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            bulletReceiver = GetComponent<BulletReceiver>();
        }
    }
}