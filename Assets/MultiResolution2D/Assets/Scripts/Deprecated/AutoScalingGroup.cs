using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// OLD classes which represents an auto scaling group
// OBSOLETE: please use AutoScalingGroupData instead.
namespace MultiResolution2D
{
    [System.Serializable]
    public class AutoScalingGroup : ScriptableObject
    {
        public enum ScaleMode
        {
            Manual,
            AspectFitWidth,
            AspectFitHeight,
            AspectFit,
            AspectFill,
            ClosestWholeNumber,
            PixelPerfect,
        }

        public AutoScalingGroup.ScaleMode scaleMode;

        [SerializeField]
        public string autoScalingGroupName;

        [SerializeField]
        public float manualScale;

        [SerializeField]
        public List<ScreenMatchingRule> matchingRules;
    }
}