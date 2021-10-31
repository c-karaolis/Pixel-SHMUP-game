using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiResolution2D
{
    /// <summary>
    /// An Auto Scaling Group defines what to do (i.e. how to scale) when running the game at a specific resolution, aspect or density.
    /// </summary>
    [System.Serializable]
    public class AutoScalingGroupData
    {
        /// <summary>
        /// Gets the default auto scaling group.
        /// </summary>
        /// <value>The default auto scaling group.</value>
        public static AutoScalingGroupData DefaultAutoScalingGroup
        {
            get
            {
                AutoScalingGroupData defaultGroup = new AutoScalingGroupData
                {
                    autoScalingGroupName = "Default",
                    scaleMode = AutoScalingGroupData.ScaleMode.ClosestWholeNumber,
                    matchingRules = new List<ScreenMatchingRule>(),
                    manualScale = 1.0f
                };
                defaultGroup.matchingRules.Add(ScreenMatchingRule.DefaultScreenMatchingRule);

                return defaultGroup;
            }
        }

        public static AutoScalingGroupData CreateInstanceFromOldAutoScalingGroup(AutoScalingGroup autoScalingGroup) {
            AutoScalingGroupData defaultGroup = new AutoScalingGroupData
            {
                autoScalingGroupName = autoScalingGroup.autoScalingGroupName,
                scaleMode = (AutoScalingGroupData.ScaleMode)autoScalingGroup.scaleMode,
                matchingRules = autoScalingGroup.matchingRules,
                manualScale = autoScalingGroup.manualScale
            };

            return defaultGroup;
        }

        /// <summary>
        /// Scale mode
        /// </summary>
        public enum ScaleMode
        {
            /// <summary> Explicitly use the scale parameter. </summary>
            Manual,

            /// <summary> Scales the content to fit the width of the screen by maintaining the aspect ratio. Some portion of the height content may be clipped. </summary>
            AspectFitWidth,

            /// <summary> Scales the content to fit the height of the screen by maintaining the aspect ratio. Some portion of the width content may be clipped. </summary>
            AspectFitHeight,

            /// <summary> Scales the content to fit the size of the screen by maintaining the aspect ratio. (Best Fit) </summary>
            AspectFit,

            /// <summary> Scales the content to fill the size of the screen. Some portion of the content may be clipped to fill the screen’s bounds. </summary>
            AspectFill,

            /// <summary> Upscales the content by closest whole numbers (1x, 2x, etc) or downscales the content by closest fraction by whole numbers (1/2x, 1/3x, etc). Some portion of the content may be clipped.</summary>
            ClosestWholeNumber,

            /// <summary>
            /// Upscales the content only by the closest whole number (1x, 2x, 3x, etc). The design screen size should be at 1x to be able to upscale achieving pixel perfect.
            /// </summary>
            PixelPerfect,
        }

        /// <summary>
        /// Name of the auto scaling group.
        /// </summary>
        public string AutoScalingGroupName { get { return this.autoScalingGroupName; } }

        /// <summary>
        /// Scale value used on "Manual" scale mode selected.
        /// </summary>
        public float ManualScale { get { return this.manualScale; } }

        /// <summary>
        /// The scale mode.
        /// </summary>
        public AutoScalingGroupData.ScaleMode scaleMode;

        [SerializeField]
        string autoScalingGroupName;

        [SerializeField]
        float manualScale;

        /// <summary>
        /// Screen matching rules.
        /// The auto scaling group is valid when at least one of the matching rules is met.
        /// </summary>
        [SerializeField]
        List<ScreenMatchingRule> matchingRules;

#if UNITY_EDITOR
        public List<ScreenMatchingRule> ScreenMatchingRules { get { return this.matchingRules; } }
#endif

        /// <summary>
        /// The group is active if at least one of the criteria (matching rules) is met.
        /// </summary>
        /// <returns><c>true</c> if this auto scaling group is active; otherwise, <c>false</c>.</returns>
        /// <param name="screenSize">Current Screen Size.</param>
        public bool IsActive(Vector2 screenSize)
        {
            if (this.matchingRules != null)
            {
                int length = this.matchingRules.Count;
                for (int i = 0; i < length; ++i)
                {
                    var rule = this.matchingRules[i];
                    if (rule.IsValidWithScreenSize(screenSize))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the auto scale factor.
        /// </summary>
        /// <returns>The auto scale factor.</returns>
        /// <param name="designResolution">Design resolution.</param>
        /// <param name="targetResolution">Target resolution.</param>
        public float GetAutoScaleFactor(Vector2 designResolution, Vector2 targetResolution)
        {
            float scale = 1.0f;

            switch (this.scaleMode)
            {
                case ScaleMode.Manual:
                    scale = this.manualScale;
                    break;

                case ScaleMode.AspectFitWidth:
                    scale = targetResolution.x / designResolution.x;
                    break;

                case ScaleMode.AspectFitHeight:
                    scale = targetResolution.y / designResolution.y;
                    break;

                case ScaleMode.AspectFill:
                    scale = Mathf.Max(targetResolution.x / designResolution.x, targetResolution.y / designResolution.y);
                    break;

                case ScaleMode.AspectFit:
                case ScaleMode.ClosestWholeNumber:
                case ScaleMode.PixelPerfect:
                    // Find the scale ratio according to the screen aspect (native * scale = current)
                    float designAspect = designResolution.x / designResolution.y;
                    float currentAspect = targetResolution.x / targetResolution.y;

                    // AspectFit scale
                    if (currentAspect < designAspect)
                    {
                        scale = targetResolution.x / designResolution.x;
                    }
                    else
                    {
                        scale = targetResolution.y / designResolution.y;
                    }


                    if (scale >= 1.0f && (this.scaleMode == ScaleMode.ClosestWholeNumber || this.scaleMode == ScaleMode.PixelPerfect))
                    {
                        // Upscale: Find the closest whole number and round the scale by this
                        // Round to nearest integer which is a multiple of 1.0f
                        scale = Mathf.Round(scale / 1.0f) * 1.0f;
                    }
                    else if (this.scaleMode == ScaleMode.ClosestWholeNumber)
                    {
                        // Downscale: Get scale inverse (current * scale = native) so it will be > 1.0f
                        float n = 1.0f / scale;

                        // Round it as before
                        n = Mathf.Round(n / 1.0f) * 1.0f;

                        // Inverse scale (native * scale = current) with the closest divisor: 1/2, 1/3, 1/4, ..., 1/n
                        scale = 1.0f / n;
                    }
                    else if (this.scaleMode == ScaleMode.PixelPerfect)
                    {
                        scale = 1.0f; // Do not downscale at pixel perfect mode. Keep 1x as minimum scale.
                    }
                    break;

                default:
                    scale = 1.0f;
                    break;
            }

            return scale;
        }
    }

}