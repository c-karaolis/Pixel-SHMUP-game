using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
    This is the script that is attached to the bullet game object
    It is in charge of moving the bullet in a specific direction at a specific speed
    It also inflicts a specific damage when colliding with the player
    The bullet will be deactivated after a certrain time (lifespan) if it does collide with the player
*/
namespace Foxlair.Bullets
{
    public class Bullet : MonoBehaviour
    {
        //The 2D direction in which the bullet will be travelling
        private Vector2 direction;
        [SerializeField]
        private float speed = 5f;
        [SerializeField]
        private float lifespan = 5f;
        [SerializeField]
        private float damage = 0.5f;


        private void OnEnable()
        {
            //Waits lifespan number of seconds before calling the destroy function
            Invoke(nameof(Disable), lifespan);
        }

        void Update()
        {
            Move();
        }

        //Moves the bullet in the direction at speed
        private void Move()
        {
            // Debug.Log("Moving at direction "+direction);
            transform.Translate(direction * speed * Time.deltaTime);
        }

        /// <sumamary>
        /// Access function so other scripts (IE FireBullet.cs) can set the direction of the bullet
        /// </summary>
        public void SetMoveDirection(Vector2 dir)
        {
            direction = dir;
        }

        //Deactivates the bullet on destroy (to be reused later without instantiating and destroying objects)
        private void Disable()
        {
            gameObject.SetActive(false);
        }

        //When this is destroyed, cancel any outstanding invoke functions (IE the one in OnEnable)
        //This is for if the bullet is destroyed/deactivated from colliding with the player, it won't
        //try to disable/destroy the bullet again after the lifespan is up.
        private void OnDisable()
        {
            CancelInvoke();
        }

        /// <summary>
        /// Changes the tag according who fired the bullet. 
        /// Makes bullet script reusable from everyone as is without inheriting bullet scripts.
        /// </summary>
        /// <param name="newTag"> Desired new tag name. Defaults to Player</param>
        public void ChangeTag(string newTag = "Player")
        {
            tag = newTag;
        }

        //The bullet IS the trigger. When it hits a collider this function is called. It checks to see 
        //if the collider is the player. ***This will only work if your player object has a tag "Player"***
        void OnTriggerEnter2D(Collider2D collider)
        {
            //Checks the collider to see if it has the "Player" tag (Case sensitive)
            if (collider.CompareTag("Player"))
            {
                try
                {
                    HealthSystem healthSystem= collider.GetComponentInParent<HealthSystem>();
                    healthSystem.TakeDamage(damage);
                }
                catch (Exception exception)
                {
                    Debug.LogWarning($"Exception caught: {exception}");
                }
                //Get the player script from the collided player object
                //You are going to have to reference your own player your own way here.
                //But this is how I did it, if it's helpful!
                // Player player = col.GetComponentInParent<Player>();
                // if(player != null){
                //     //cause the player damage
                //     player.TakeDamage(damage);
                // }
                //After colliding with the player, destroy the bullet
                Disable();
            }
        }
    }
}