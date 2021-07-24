using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Written by Julia
    @juul1a everywhere
    juul1a.com

    This is the system that controls the shooting patterns of the bullet hell enemy (aka "shooter") it is attached to
    In the inspector you can supply the number of steps you want the shooter to go through as the size of the steps array.
    From there you can add the step data for each step that the shooter will cycle through
    The preset from each step will be loaded into the FireBullet script and stay on that preset for the duration supplied
    The movement of the shooter is either "on" or "off" based on that step. This can be expanded on by adjusting the ShipMovement script.
    Coming Soon: Each step can have its own bullet prefab - this can allow for different shapes and speeds and damage dealt by different attacks.
*/
namespace Foxlair.Bullets
{
    public class FiringSequencer : MonoBehaviour
    {
        [Tooltip("Check this box to disable the sequencer")]
        public bool disable = false; //Disable sequencer   
        private bool disabled = false;
        [System.Serializable]
        //Object to contain data for each step in the firing sequence
        public class Step
        {
            [Tooltip("Number of seconds to run this step")]
            public float duration; //Duration in seconds that this step will last
            [Tooltip("Shooting preset to run")]
            public ShootingPreset preset; //Preset scriptable object with parameters for the FireBullet script
            [Tooltip("Whether or not the bullet boss should move back and forth")]
            public bool movement; //true = moving left and right, false = stationary
        }

        //array of the steps that the sequencer will loop through
        public Step[] steps;

        //References to the firing and movement scripts of the enemy
        //This is how the sequencer accesses and controls the enemy object
        private FireBullet fireScript;
        private BossShipMovement movementScript;

        //Contains the step we are currently on, which is the index value of the steps[] array
        private int stepNum;


        // Start is called before the first frame update
        void Awake()
        {
            stepNum = 0;
            fireScript = GetComponentsInChildren<FireBullet>()[0]; //Gets the firebullet script associated with this enemy gameobject
            movementScript = gameObject.GetComponent<BossShipMovement>(); //Gets the shipmovement script associated with this enemy gameobject
            disabled = disable;
            RunStep();//Invokes the recursive RunStep function immediately
        }

        void Update()
        {
            //So it only calls Runstep once when enabled
            if (!disable && disabled)
            {
                disabled = false;
                RunStep();
            }
            if (disable && !disabled)
            {
                disabled = true;
            }
        }

        private void RunStep()
        {
            if (!disable)
            {
                fireScript.SetPreset(steps[stepNum].preset); // First sets the firing preset according to the current StepNum
                                                             //Movement of enemy class is preprogrammed in ShipMovement to go left and right, so this just turns movement "on" or "off"
                                                             //which is move back and forth left and right, and stop moving, respectively.
                if (steps[stepNum].movement == false)
                {
                    movementScript.ManualStop(); //Turns off movement of enemy 
                }
                else
                {
                    movementScript.ManualStart(); //Turns on movement of enemy
                }
                if (stepNum + 1 == steps.Length)
                {
                    //If this is the last step in the array then reset the Stepnum to 0
                    //Since we set the stepnum before we invoke the recursive function we have to invoke if based on the value it was previously 
                    stepNum = 0;
                    Invoke("RunStep", steps[steps.Length - 1].duration);
                }
                else
                {
                    // Increase the step number, but then invoke it for the step we were just on.
                    stepNum++;
                    Invoke("RunStep", steps[stepNum - 1].duration);
                }
            }
        }
    }
}