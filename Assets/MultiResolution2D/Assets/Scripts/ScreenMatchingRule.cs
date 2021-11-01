using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiResolution2D
{
    /// <summary>
    /// Screen matching rule.
    /// </summary>
    [System.Serializable]
    public class ScreenMatchingRule
    {
        // Wildcard character: means all values.
        public const string Wildcard = "*";

        /// <summary>
        /// Gets the default screen matching rule.
        /// </summary>
        /// <value>The default screen matching rule.</value>
        public static ScreenMatchingRule DefaultScreenMatchingRule
        {
            get
            {
                ScreenMatchingRule defaultRule = new ScreenMatchingRule();
                defaultRule.matchBy = MatchByType.Resolution;
                defaultRule.comparison = ComparisonOperator.Equal;
                defaultRule.leftValue = ScreenMatchingRule.Wildcard;
                defaultRule.rightValue = ScreenMatchingRule.Wildcard;
                return defaultRule;
            }
        }

        /// <summary>
        /// Match by type to use.
        /// </summary>
        public enum MatchByType
        {
            /// <summary> Will use the screen pixel resolution to match. </summary>
            Resolution,

            /// <summary> Will use the screen aspect ratio to match. </summary>
            Aspect,

            /// <summary> Will use the screen density to match (using Screen.dpi) </summary>
            Density,
        }

        /// <summary>
        /// The match by mode to use.
        /// </summary>
        public MatchByType MatchBy { get { return this.matchBy; } }

        /// <summary>
        /// Comparison operator.
        /// </summary>
        public enum ComparisonOperator
        {
            Equal,
            LessThan,
            LessThanOrEqualTo,
            GreaterThan,
            GreaterThanOrEqualTo
        }

        public ComparisonOperator Comparison { get { return this.comparison; } }

        #if UNITY_EDITOR
        /// Must match ScreenMatchingRule.ComparisonOperator values and order
        public static string[] ComparisonOperatorString =
            {
                "=",
                "<",
                "≤",
                ">",
                "≥",
            };
        #endif

        [SerializeField]
        MatchByType matchBy;

        [SerializeField]
        ComparisonOperator comparison;

        [SerializeField]
        string leftValue;

        [SerializeField]
        string rightValue;

        /// <summary>
        /// Determines if there is at least one rule valid with the specified screen size.
        /// </summary>
        /// <returns><c>true</c> if there is at least one rule valid with the specified screen size; otherwise, <c>false</c>.</returns>
        /// <param name="screenSize">Screen size.</param>
        public bool IsValidWithScreenSize(Vector2 screenSize)
        {
            switch (this.matchBy)
            {
                case ScreenMatchingRule.MatchByType.Resolution:
                    return IsValidExpression(screenSize.x, this.comparison, this.leftValue) && IsValidExpression(screenSize.y, this.comparison, this.rightValue);

                case ScreenMatchingRule.MatchByType.Aspect:
                    if (this.leftValue == ScreenMatchingRule.Wildcard || this.rightValue == ScreenMatchingRule.Wildcard)
                    {
                        return true;
                    }
                    float aspectRatio = float.Parse(this.leftValue, System.Globalization.CultureInfo.InvariantCulture.NumberFormat) / float.Parse(this.rightValue, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                    return IsValidExpression(screenSize.x / screenSize.y, this.comparison, aspectRatio.ToString());

                case ScreenMatchingRule.MatchByType.Density:
                    return IsValidExpression(Screen.dpi, this.comparison, this.leftValue);
            }

            return false;
        }


        /// <summary>
        /// Determines whether this instance is valid expression with the specified leftValue, sign and rightValue.
        /// leftValue - operator - rightValue E.G: 120 < 67 will return false
        /// LeftValue is the value provided by system (screen size or dpis...).
        /// Return true if rightValue (user provided) is a WILDCARD or if the expression is valid.
        /// </summary>
        bool IsValidExpression(float leftValue, ComparisonOperator sign, string rightValue)
        {
            if (rightValue == ScreenMatchingRule.Wildcard)
            {
                return true;
            }

            // Convert value to float
            float rightFloatValue = 0.0f;
            float.TryParse(rightValue, out rightFloatValue);

            // We compare 2 floats that could have a small difference even if equal with the conversion.
            float difference = Mathf.Abs(leftValue - rightFloatValue);
            bool isValueEqual = difference < 0.01f;

            switch (sign)
            {
                case ComparisonOperator.Equal:
                    return isValueEqual;

                case ComparisonOperator.LessThan:
                    return !isValueEqual && leftValue < rightFloatValue;

                case ComparisonOperator.LessThanOrEqualTo:
                    return isValueEqual || leftValue < rightFloatValue;

                case ComparisonOperator.GreaterThan:
                    return !isValueEqual && leftValue > rightFloatValue;

                case ComparisonOperator.GreaterThanOrEqualTo:
                    return isValueEqual || leftValue > rightFloatValue;
            }

            return false;
        }
    }
}