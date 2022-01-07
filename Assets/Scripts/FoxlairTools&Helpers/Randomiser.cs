using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foxlair.Tools
{
    public static class Randomiser
    {

        /// <summary>
        /// Function to pass a percentile chance (e.g. 5%) and see if it happens after rolling a d100.
        /// </summary>
        /// <param name="percentileChance"></param>
        /// <returns>True if chance is less than the roll</returns>
        public static bool RandomFixedPercentage(float percentileChance)
        {
            float roll = Random.Range(0.0f, 100.0f);

            if (roll > percentileChance)
            {
                return false;
            }

            return true;

        } 


    }
}