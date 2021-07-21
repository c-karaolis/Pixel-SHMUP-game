using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Part of Julia's Bullet Hell Package

    This script is responsible for the back-and-forth movement of the bullet boss. It detects the edges of the camera and ensures
    that the enemy only moves within the bounds of the camera. It sorrrt of works when the camera is set to follow the player, but
    ideally it's a stationary camera with unchanging bounds.

    The default movement of the ship is moving right until reaching the camera bounds, stopping for 3 seconds, then moving left until the
    left side camera bounds, stopping for 3 seconds, then moving back right. This class also has functions for external triggering of the movement. 
    The external movement trigger basically informs the script when to stop and resume the default movement. So the manual stop keeps
    the bullet boss stationary until the manual start is triggered. once the manual start is triggered, the script resumes default movement 
    as described above.

    The reason I added manual stops in is because in the FiringSequencer sometimes it is better for the bullet boss to be stationary
    when firing a specific spread of bullets. So when that bullet spread/preset comes up in the sequence, the movement needs to stop,
    wherever the bullet boss is, for the duration of that step. The duration of the manual stop is handled outside of this class,
    and then manual start must also be called from outside this class in order for the movement to start again.

*/


public class BossShipMovement : MonoBehaviour
{
    //Movement speed of the bullet boss
    [SerializeField]
    private float speed = 5f;

    //Maximum and minimum x values for the bullet boss's movement. AKA the camera bounds
    private float maxX, minX;

    [SerializeField]
    public float percentDistFromEdge = 10f;
    //Reference to the camera object to obtain the bounds
    private Camera cam;
    //Animator of the bullet boss object
    private Animator anim;

    //Counts down the seconds since we called the StopForTime function. So we can tell the bullet boss how long to stop for
    private float liveCountdown;
    //If we have stopped and it was a manual stop, we set this to true
    private bool manualStop = false;
    //Tracks if the bullet boss is moving left and right
    public bool moveRight = true;
    //Tracks if the bullet boss is moving or not
    public bool stopped = false;

    private float unitDistFromEdge;


    void Start(){
        //Set the camera to the main camera
        cam =  GameObject.Find ("Main Camera").GetComponent<Camera>();
        //Determine the bounds of the camera
        SetMaxAndMin();
        //Grab the animator object of the bullet boss - it gets all animators in children of the
        //main game object and then grabs the first one. So this can get funky if you have multiple animators, beware
        Animator[] animators = GetComponentsInChildren<Animator>();
        if(animators.Length >0){
            anim = animators[0];
        }
        //Set the initial movement of the animation to match the script
        if(anim){
            anim.SetBool("MoveRight", moveRight);
        }

        unitDistFromEdge = (maxX - minX)*(percentDistFromEdge/100);
    }

    /// <summary>
    /// Stops the movement of the bullet boss for a specified time
    /// This function structured this way because it is called frequently within the update function
    /// </summary>
    public void StopForTime(float countdown = 0){
        //If the bullet boss has already stopped then we continue the countdown from where it left off
        if(stopped){
            //subtract deltaTime from live countdown
            liveCountdown -= Time.deltaTime;
            //Once countdown is done, we can start the movement again
            if(liveCountdown <= 0){
                StartMovement();
            }
        }
        //If countdown hasn't started yet then we set the countdown and stop the movement
        else{
            liveCountdown = countdown;
            StopMovement();
        }
    }

    /// <summary>
    /// This is used from outside this class to stop the bullet boss's movement (IE In the FiringSequencer script)
    /// It resets the countdown so it will override any Stop that is in progress from the default movement. 
    /// </summary>
    public void ManualStop(){
        manualStop = true;
        liveCountdown = 0;
        StopMovement();
    }

    /// <summary>
    /// Turns the boss movement back on
    /// </summary>
    public void ManualStart(){
        manualStop = false;
        StartMovement();
    }
    /// <summary>
    /// Sets the boss state to stopped, and updates the animation variable to run the stop animation
    /// </summary>
    public void StopMovement(){
        stopped = true;
        if(anim){
            anim.SetBool("Stopped", true);
        }
    }

    /// <summary>
    /// Sets the state to not-stopped, and updates the animation variables accordingly
    /// </summary>
    public void StartMovement(){
        stopped = false;
        if(anim){
            anim.SetBool("Stopped", false);
            anim.SetBool("MoveRight", moveRight);
        }
    }
    
    void Update()
    {
        //Call movement function if not stopped
        if(!stopped){
            MoveLeftRight();
        }
        //If it is stopped and not a manual stop, continue the countdown in the StopForTime function
        else if(!manualStop){
            StopForTime();
        }
       
    }

    //General movement back and forth, with 3s stop at each camera edge
    private void MoveLeftRight(){
        if(moveRight){
            //Adjust position of game object to be moved in the +x (right) direction at the declared speed. Keep y the same.
            transform.position = new Vector2(transform.position.x + speed * Time.deltaTime, transform.position.y);
        }
        else{
            //Adjust position of game object to be moved in the -x (left) direction at the declared speed. Keep y the same.
            transform.position = new Vector2(transform.position.x - speed * Time.deltaTime, transform.position.y);
        }
        //If the position is within 20 units from the right side screen wall (maxX) (20 worked for me based on my sprite size, you can adjust this buffer)
        //Then stop the bullet boss for 3 seconds and change the direction to left.
        if(transform.position.x > maxX - unitDistFromEdge){
            moveRight = false;
            StopForTime(3.0f);
        }
        //if the position is within 2- units from the left side screen wall (minX) then stop for 3 seconds and change direction to right.
        else if(transform.position.x < minX + unitDistFromEdge){
            moveRight = true;
            StopForTime(3.0f);
        }
    }

    //Converts camera edges to world points and set those x values as the min and max x for the bullet boss to move.
    private void SetMaxAndMin(){
        Vector2 camBottomLeft = cam.ViewportToWorldPoint(new Vector3(0,0,0));
        Vector2 camTopRight = cam.ViewportToWorldPoint(new Vector3(1,1,0));
        minX = camBottomLeft.x;
        maxX = camTopRight.x;
    }
}
