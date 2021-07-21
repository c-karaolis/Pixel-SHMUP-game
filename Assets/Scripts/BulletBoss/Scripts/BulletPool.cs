using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Built from this tutorial by Alexander Zotov:
    https://www.youtube.com/watch?v=Mq2zYk5tW_E

    This script creates a "pool" of bullet objects to be reused as they are fired by the bullet boss
    It's more efficient to do it this way instead of constantly instantiating and destroying gameobjects
    When a bullet is needed, an external script will call this GetBullet() function. The function looks
    through the list of bullets and grabs the first inactive bullet available, and returns it. It is the job
    of the external function to re-activate the bullet. If there are no inactive bullets available, only then
    does the function instantiate a new bullet. References to all bullet objects are stored in this script
    in the bullets variable

*/

public class BulletPool : MonoBehaviour
{
    //Static instance of this bullet pool for external reference
    public static BulletPool bulletPoolInstance;
    
    //Prefab of bullet object to shoot
    [SerializeField]
    private GameObject bulletPrefab;

    //All bullets that have been instantiated
    private List<GameObject> bullets = new List<GameObject>();

    private void Awake(){
        bulletPoolInstance = this;
        if(!bulletPrefab){
            bulletPrefab = new GameObject();
        }
    }

    /// <summary>
    /// Public function that returns the reference to a bullet object - either newly created, or previously used but inactive.
    /// We recycle bullets here.
    /// </summary>
    public GameObject GetBullet(){
        if(bullets.Count > 0){
            //Goes through all bulets in the list
            for(int i = 0; i < bullets.Count; i++){
                //Grabs the first inactive one
                if(!bullets[i].activeInHierarchy){
                    //Returns the inactive bullet
                    return bullets[i];
                }
            }
        }
        // If it gets here then there were not enough bullets in the pool, aka there were no inactive bullets to instantiate
        // So a new bullet is instantiated, added to the list, and returnet.
        GameObject bul = Instantiate(bulletPrefab);
        bul.SetActive(false);
        bullets.Add(bul);
        return bul;
    }
    
}
