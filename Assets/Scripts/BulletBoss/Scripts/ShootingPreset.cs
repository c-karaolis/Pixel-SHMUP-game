using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
        Simple scriptable object to hold different values for the variables in the FireBullet script
*/

[CreateAssetMenu(fileName = "New Shooting Preset", menuName = "Shooting Preset")]
public class ShootingPreset : ScriptableObject
{
        public bool on;
        //Shooting Params
        public int bulletStreams;
        public int bulletArrays;
        public float arrayOffset;
        public float fireRate;
        public float startAngle, endAngle;
        public bool rotateBackAndForth;
        public bool rotateCircle;
        public float rotateAngle;
        public float rotationSpeed;

        //Movement Params
        public float speed;

        //Bullet Params
        //Coming sooN!
}
