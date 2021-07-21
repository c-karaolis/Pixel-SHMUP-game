using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This is the main script that controlls the bullet firing functionality of the Enemy object it is attached to.
    It requires the BulletPool and BulletScript classes to function as it uses those to organize and optimize the 
    the creation and desctruction of bullet objects.

    The functionality of this firing mechanism is determined by the parameters listed under the "Live Settings" header.
    These can be adjusted in the inspector while the game is running to see the immediate effects of what each setting does
        bulletStreams ~       This is the number of bullet streams in one array, or to put it another way, the number of bullets 
                            fired at once in an array
        bulletArrays ~      This is the number of arrays total. An array is a set of bullets that are firing at once
        arrayOffset ~       This is the angle that separates each of the arrays. This is the angle between the angle of the first
                            bullet of the first array, and the first bullet of the subsequent array. 
        fireRate ~          This is the rate at which bullets are fired, for all bullets in all arrays. It is the time, in seconds,
                            between each bullet fire. The smaller the number, the faster the bullets are fired.
        startAngle ~        This is the starting angle of the first bullet in the array
        endAngle ~          This is the angle (from 0) of the last bullet in the array (all bullets in the array will be evenly distributed
                            between the start angle and end angle)
        rotateBackAndForth ~ This rotates the object that the script is attached to back and forth at the speed rotateSpeed.
                            All angles of the bullets and bullet arrays are respective to the local position of this object,
                            so as the object rotates, the arrays automatically rotate with it.
        rotateAngle ~       The boundary angle of the rotation when rotating back and forth. The shooter will rotate from the negative value of this angle, to the positive
                            value of it. so if you put in 30, the shooter will rotate from -30 to 30.
        rotateCircle ~      When checked, this rotates the shooter in a full circle at the speed rotationSpeed. If rotateBackAndForth is true, 
                            it takes precedent over rotateCircle, so make sure rotateBackAndForth is false if you want rotateCircle to be the functionality
        rotationSpeed ~     The multiplier of which the rotation angle is increased. Bigger number, faster rotation.

    //PRESETS//
    The "Live settings" of the parameters listed above can be saved into a ShootingPreset scriptable object so the settings can be accessed
    again. To do so, you can enter the name you wish to title the asset file and click Save Preset. This functionality is handled by the 
    FireBulletEditor class. 

    Objects of this class can contain a number of presets to switch between. This was initially for testing purposes of switching back and forth
    between different ShootingPresets to build a sequence. In the inspector you can input how many presets you wish to switch between as
    the array size of the presets parameter. Then from there, you can drag preset files from your project files to the inspector to populate
    the presets array. While the game is running, you can check the "On" box of any preset to turn it on. Adjusting the live settings will
    always override any preset that is turned on. However, switching to a different preset will override the live settings again.

    //EXTERNAL USE//
    This script can be controlled by the FiringSequencer script which overrides any live settings or presets it contains.
    It is accessed by 
    Just so you know.


*/


public class FireBullet : MonoBehaviour
{
    //A little class to hold the presets and show whether they are turned on or off
    [System.Serializable]
    public class Preset
    {
        public bool on;
        public ShootingPreset values;
    }
    //The array of presets that are available for this gun to switch between, for testing (from the inspector panel)
    //It is recommended to set the presets array to 0 if the FiringSequencer is on, to avoid confusion.
    public Preset[] presets;


    //All live parameters that are currently running in this shooter. Will always be up to date.
    [Header("Live Settings")]
    [Tooltip("Number of bullet streams in one array of bullets")]
    [SerializeField] private int bulletStreams = 10;
    //Number of bullets fired at once
    [Tooltip("Number of arrays or sets of bullets")]
    [SerializeField] private int bulletArrays = 2;
    [Tooltip("Offset angle between bullet arrays, in degrees")]
    [SerializeField] private float arrayOffset = 90f;
    [Tooltip("Number of seconds between each bullet firing")]
    [SerializeField] private float fireRate = 2f;
    [Tooltip("Start and end angles (in degrees) of one bullet array")]
    [SerializeField] private float startAngle = 0f, endAngle = 90f;
    [Tooltip("Shooter rotates back and forth")]
    [SerializeField] private bool rotateBackAndForth = false;
    [Tooltip("How far from it's origin the shooter should rotate to the right, and back to the left.")]
    [SerializeField] private float rotateAngle = 30f;
    [Tooltip("Keep rotating the shooter in a circle")]
    [SerializeField] private bool rotateCircle = false;
    [Tooltip("Speed at which the shooter rotates - bigger number = faster!")]
    [SerializeField] private float rotationSpeed = 2f;

    //Rotation Variables
    private int multiplier = 1; //This is for back and forth rotation, so we know which direction to rotate
    private float oldAngle; //This is to detect if the rotateAngle changes so we can reset the rotation before starting again

    [Header("Save as Preset")]
    //What to name the preset asset file once the Save button is clicked
    public string presetName;

    //The array index for which preset is running in the presets array
    private int thisPreset;

    //Each array of bullets has a parent object that they are attached to. This is to make organization and rotation easier
    private List<GameObject> bulletArrayObjs = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        oldAngle = rotateAngle;
        thisPreset = 0;
        //Sets all values to first preset in sequence, if the presets array isn't empty.
        if(presets.Length>0){
            bulletStreams = presets[thisPreset].values.bulletStreams;
            //Number of bullets fired at once
            bulletArrays = presets[thisPreset].values.bulletArrays;
            arrayOffset = presets[thisPreset].values.arrayOffset;
            fireRate = presets[thisPreset].values.fireRate;
            startAngle = presets[thisPreset].values.startAngle;
            endAngle = presets[thisPreset].values.endAngle;
            rotateBackAndForth = presets[thisPreset].values.rotateBackAndForth;
            rotateCircle = presets[thisPreset].values.rotateCircle;
            rotateAngle = presets[thisPreset].values.rotateAngle;
            rotationSpeed = presets[thisPreset].values.rotationSpeed;
        }

        //We recursivley call the Fire function because we want to be able to change the rate at which it fires live
        Fire();
    }

    void Update(){
        //Checks to see if a different preset is switched to "on", only if there are presets in the array
        if(presets.Length>0){
            CheckPresets();
        }
        //Self evident
        if(rotateBackAndForth){
            RotateBackAndForth();
        }
        else if(rotateCircle){
            Rotate();
        }
    }

    //Rotates this game object between the negative and positive values of rotateAngle, at the speed of rotationSpeed
    private void RotateBackAndForth(){
        //transform.eulerAngles = new Vector3(0f, 0f, rotateAngle * Mathf.Sin(Time.time * rotationSpeed));
        
        //Reset angles if rotateAngle has been changed
        if(oldAngle != rotateAngle){
            transform.rotation = Quaternion.Euler(0f,0f,0f);
            oldAngle = rotateAngle;
        }

        Vector3 angles = transform.eulerAngles;
        if(angles.z > rotateAngle && angles.z < (360-rotateAngle)){
            multiplier *= -1;
        }
        transform.Rotate(0,0,6.0f*rotationSpeed*Time.deltaTime*multiplier);
    }
    private void Rotate(){
        transform.Rotate(0,0,6.0f*rotationSpeed*Time.deltaTime);
    }

    //The mother function
    private void Fire(){
        if(fireRate != 0){ //This is to prevent the function from creating a heck ton of bullets ll at once and crashing the game/unity
            
            float thisArrayOffset = 0; //Angle offset between each array
            
            //Looping through each array first
            for(int j = 0; j<bulletArrays; j++){
                //This is where the gameobject to store the bullets in an array are created
                //It will only create a game object for the array if it hasn't been created yet
                //Unfortunately it doesn't destroy the game object if you decrease the number of arrays.
                //So like if you have 3 arrays there will be 3 array gameobjects created but then if you decrease down to 2
                //arrays there will still be 3 objects, the 3rd will just sit unused. And if you increase again to 3 it'll reuse
                //that 3rd object. 
                if(bulletArrayObjs.Count < j+1){
                    GameObject bullArrayHolder = new GameObject("Array "+j);
                    bulletArrayObjs.Add(bullArrayHolder);
                }
                //Calculates the step angle between each bullet in an array. Divides the total angle by the number of bullets in the array
                float angleStep = (endAngle - startAngle) / (bulletStreams-1);
                //Starting with the start angle, and offset by the distance between each array. For the first array the offset is 0
                //But for every subsequent array the offset is increased by the value of arrayOffset
                float angle = startAngle + thisArrayOffset;

                //Now we spawn bullets. We loop through this function once for every bullet in the array.
                for(int i = 0; i<bulletStreams; i++){
                    //note to self: * pi / 180 is how you convert to radians

                    //Calculates the x and y coordinates of the direction for the bullet to move at the correct angle
                    float bulDirX = transform.position.x + Mathf.Cos((angle * Mathf.PI)/180f); 
                    float bulDirY = transform.position.y + Mathf.Sin((angle * Mathf.PI)/180f);

                    //Creates the bullet direction vector and normalizes it
                    Vector3 bulMoveVector = new Vector3(bulDirX, bulDirY, 0f);
                    Vector2 bulDir = (bulMoveVector-transform.position).normalized;

                    //Creates or activates the bullet object from the Bullet Pool. Basically this either gets a deactivated bullet
                    //or it creates a new bullet gameobject, depending on the availability in the pool.
                    GameObject bul = BulletPool.bulletPoolInstance.GetBullet();
                    //Puts the bullet at the origin of the shooter, set to the default rotation and position
                    bul.transform.position = transform.position;
                    bul.transform.rotation = transform.rotation;
                    //This rotates the sprite to face the direction of the angle it is being shot at
                    //The sprite is contained in a child objecct of the bullet prefab.
                    bul.transform.GetChild(0).transform.rotation = transform.rotation * Quaternion.Euler(0,0,angle);
                    //Activates the bullet that was grabbed from the pool
                    bul.SetActive(true); 
                    //Sets the direction for the bullet to move. In the update function of the BulletScript the bullet is constantly
                    //being Translated in the supplied direction, so this just sets the direction of which the bullet will translate.
                    bul.GetComponent<Bullet>().SetMoveDirection(bulDir);

                    //This puts the bullet in the game object for this array so we can keep them orgnized neat and tidy
                    bul.transform.SetParent(bulletArrayObjs[j].transform);

                    //We add the angle step for the next bullet to spawn within the array
                    angle += angleStep;
                }
                //We add the array offset between two arrays and start firing the next array
                thisArrayOffset += arrayOffset;
            }
        }
            //Recursively calls this function again, after fireRate seconds pass. 
            Invoke("Fire", fireRate);
    }

    //This function is called every Update() if there are any items in the presets array
    //It loops through all presets to check if a new preset has been turned on, and turns
    //off all other presets. The preset that is currently on has it's array index stored in thisPreset
    private void CheckPresets(){
        for(int i = 0; i < presets.Length; i++){
            //Only check if it's on if it's not the one that's already on
            if(i!=thisPreset){
                if(presets[i].on == true){
                    //Loop through all ther presets and turn them off
                    for(int j = 0; j < presets.Length; j++){
                        if(j!=i){//Except don't turn off the one we just turned on
                            presets[j].on = false;
                        }
                    }
                    //Set thisPreset to the new preset that was just turned on
                    thisPreset = i;
                    //Set the live settings of the shooter to be the values in the preset
                    SetPreset(presets[i].values);
                }
            }
        }
    }

    /// <summary>
    /// This creates a simple ShootingPreset scriptable object from whatever values are in the live settings
    /// And returns that ShootingPreset object. This is the function that is called from the FireBulletEditor script so that
    /// whatever live settings you have can be saved as a scriptable object in an asset file to be reused
    /// </summary>
    public ShootingPreset GetValuesAsPreset(){
        ShootingPreset thisPreset = ScriptableObject.CreateInstance<ShootingPreset>();
        thisPreset.bulletStreams = bulletStreams;
        thisPreset.bulletArrays = bulletArrays;
        thisPreset.arrayOffset = arrayOffset;
        thisPreset.fireRate = fireRate;
        thisPreset.startAngle = startAngle;
        thisPreset.endAngle = endAngle;
        thisPreset.rotateBackAndForth = rotateBackAndForth;
        thisPreset.rotateCircle = rotateCircle;
        thisPreset.rotateAngle = rotateAngle;
        thisPreset.rotationSpeed = rotationSpeed;
        thisPreset.name = presetName;
        return thisPreset;
    }

    /// <summary>
    /// This is a simple set function that takes in a preset scriptable object and sets all of the live settings of the shooter
    /// to whatever is stored in that preset. It also resets the rotation back to the origin just in case a previous preset rotated it out of place.
    /// </summary>
    public void SetPreset(ShootingPreset presetToSet){
        bulletStreams = presetToSet.bulletStreams;
        bulletArrays = presetToSet.bulletArrays;
        arrayOffset = presetToSet.arrayOffset;
        fireRate = presetToSet.fireRate;
        startAngle = presetToSet.startAngle;
        endAngle = presetToSet.endAngle;
        rotateBackAndForth = presetToSet.rotateBackAndForth;
        rotateCircle = presetToSet.rotateCircle;
        rotateAngle = presetToSet.rotateAngle;
        rotationSpeed = presetToSet.rotationSpeed;
        //reset rotation of the object
        transform.rotation = Quaternion.Euler(0f,0f,0f);
    }

}
