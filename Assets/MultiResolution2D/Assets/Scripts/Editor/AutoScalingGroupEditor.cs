using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace MultiResolution2D
{
    public class AutoScalingGroupEditor
    {
        // target prop
        public readonly SerializedProperty AutoScalingGroupProp;

        // Properties
        SerializedProperty autoScalingGroupNameProp;
        SerializedProperty scaleModeProp;
        SerializedProperty manualScaleProp;
        SerializedProperty matchingRulesProp;

        ReorderableList matchingRulesReordList;

        public AutoScalingGroupEditor(SerializedProperty autoScalingGroupProp) {
            this.AutoScalingGroupProp = autoScalingGroupProp;

            this.autoScalingGroupNameProp = autoScalingGroupProp.FindPropertyRelative("autoScalingGroupName");
            this.scaleModeProp = autoScalingGroupProp.FindPropertyRelative("scaleMode");
            this.manualScaleProp = autoScalingGroupProp.FindPropertyRelative("manualScale");
            this.matchingRulesProp = autoScalingGroupProp.FindPropertyRelative("matchingRules");

            this.matchingRulesReordList = CreateMatchingRulesReorderableList(autoScalingGroupProp);
        }
        
        public void OnInspectorGUI()
        {
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                // Name
                EditorGUILayout.LabelField(new GUIContent(this.autoScalingGroupNameProp.stringValue), EditorStyles.boldLabel);

                // Scale Mode
                EditorGUILayout.PropertyField(this.scaleModeProp);

                // Show scale value on scale mode: manual
                if ((AutoScalingGroupData.ScaleMode)this.scaleModeProp.enumValueIndex == AutoScalingGroupData.ScaleMode.Manual)
                {
                    EditorGUILayout.PropertyField(this.manualScaleProp, new GUIContent("Scale"));
                }

                // List
                EditorGUILayout.Space();
                this.matchingRulesReordList.DoLayoutList();

                if (ShowDensityWarning())
                {
                    EditorGUILayout.HelpBox("Screen density may return 0 if unable to determine the current DPI. ", MessageType.Warning);
                }

                EditorGUILayout.Space();
            }
            EditorGUILayout.EndVertical();
        }

        #region Reorderable List

        ReorderableList CreateMatchingRulesReorderableList(SerializedProperty autoScalingGroupProp)
        {
            ReorderableList reordList = new ReorderableList(autoScalingGroupProp.serializedObject, autoScalingGroupProp.FindPropertyRelative("matchingRules"), true, true, true, true);

            // Draw header
            reordList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Matching Criteria", EditorStyles.boldLabel);
            };

            // Draw element
            reordList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = reordList.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;

                // Defines Layout Rect
                Rect rMatchBy = new Rect(rect.x, rect.y, 70f, EditorGUIUtility.singleLineHeight);
                Rect rOperator = new Rect(rect.x + 72.0f, rect.y, 30.0f, EditorGUIUtility.singleLineHeight);

                // Match by
                SerializedProperty matchByProp = element.FindPropertyRelative("matchBy");
                EditorGUI.PropertyField(rMatchBy, matchByProp, GUIContent.none);

                // Comparison operator
                EditorGUI.BeginChangeCheck();
                SerializedProperty comparisonProp = element.FindPropertyRelative("comparison");
                int lengthOptions = ScreenMatchingRule.ComparisonOperatorString.Length;
                int selectedIndex = Mathf.Min(Mathf.Max(0, comparisonProp.enumValueIndex), lengthOptions - 1);
                selectedIndex = EditorGUI.Popup(rOperator, selectedIndex, ScreenMatchingRule.ComparisonOperatorString);
                if (EditorGUI.EndChangeCheck())
                {
                    comparisonProp.enumValueIndex = selectedIndex;
                }

                float unitLabelWidth = 11.0f;
                float vectorWidth = (rect.width - rMatchBy.width - rOperator.width - unitLabelWidth - 2.0f) / 2.0f;

                Rect rLeftValue = new Rect(rect.x + 104.0f, rect.y, vectorWidth, EditorGUIUtility.singleLineHeight);
                Rect rUnitLabel = new Rect(rect.x + 104.0f + vectorWidth, rect.y, unitLabelWidth, EditorGUIUtility.singleLineHeight);
                Rect rRightValue = new Rect(rUnitLabel.x + rUnitLabel.width, rect.y, vectorWidth, EditorGUIUtility.singleLineHeight);
                Rect rDpiLabel = new Rect(rect.x + 104.0f + vectorWidth, rect.y, unitLabelWidth + vectorWidth, EditorGUIUtility.singleLineHeight);

                // Left Field / Right Field
                switch ((ScreenMatchingRule.MatchByType)matchByProp.enumValueIndex)
                {
                    case ScreenMatchingRule.MatchByType.Resolution:
                    case ScreenMatchingRule.MatchByType.Aspect:
                        {
                            EditorGUI.BeginChangeCheck();

                            // Left value
                            SerializedProperty leftValueProp = element.FindPropertyRelative("leftValue");
                            string leftValue = EditorGUI.TextField(rLeftValue, leftValueProp.stringValue);

                            // Unit
                            GUIStyle style = new GUIStyle(GUI.skin.label);
                            style.alignment = TextAnchor.MiddleCenter;
                            string unitLabel = (ScreenMatchingRule.MatchByType)matchByProp.enumValueIndex == ScreenMatchingRule.MatchByType.Resolution ? "x" : ":";
                            EditorGUI.LabelField(rUnitLabel, new GUIContent(unitLabel), style);

                            // Right value
                            SerializedProperty rightValueProp = element.FindPropertyRelative("rightValue");
                            string rightValue = EditorGUI.TextField(rRightValue, rightValueProp.stringValue);

                            if (EditorGUI.EndChangeCheck())
                            {
                                leftValueProp.stringValue = ParseStringValue(leftValue);
                                rightValueProp.stringValue = ParseStringValue(rightValue);
                            }

                            break;
                        }

                    case ScreenMatchingRule.MatchByType.Density:
                        {
                            // Left Field
                            EditorGUI.BeginChangeCheck();
                            SerializedProperty leftValueProp = element.FindPropertyRelative("leftValue");
                            string dpiValue = EditorGUI.TextField(rLeftValue, leftValueProp.stringValue);
                            if (EditorGUI.EndChangeCheck())
                            {
                                leftValueProp.stringValue = ParseStringValue(dpiValue);
                            }

                            // Density label usign Screen.dpi
                            EditorGUI.LabelField(rDpiLabel, new GUIContent("DPI"));
                            break;
                        }
                }
            };

            // Add an element
            reordList.onAddCallback = (ReorderableList list) =>
            {
                int index = list.serializedProperty.arraySize;
                list.serializedProperty.arraySize++;
                list.index = index;

                // Set Screen Matching rule default values
                SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
                element.FindPropertyRelative("leftValue").stringValue = ScreenMatchingRule.Wildcard;
                element.FindPropertyRelative("rightValue").stringValue = ScreenMatchingRule.Wildcard;

                autoScalingGroupProp.serializedObject.ApplyModifiedProperties();
            };

            return reordList;
        }

        /// Transform the string value to Int or wildcard (as string).
        /// If there is an error, the result will be the wildcard character (= all values)
        string ParseStringValue(string s)
        {
            float f = 0.0f;
            if (!float.TryParse(s, out f))
            {
                s = ScreenMatchingRule.Wildcard;
            }
            else
            {
                s = ((int)Mathf.Max(0, f)).ToString();
            }
            return s;
        }

        /// <summary>
        /// Return true if there is at least one screen matching rule based on screen density.
        /// </summary>
        bool ShowDensityWarning()
        {
            var i = 0;
            bool found = false;
            while (i < this.matchingRulesProp.arraySize && !found) {
                var prop = this.matchingRulesProp.GetArrayElementAtIndex(i);
                var matchByProp = prop.FindPropertyRelative("matchBy");
                found = (ScreenMatchingRule.MatchByType)matchByProp.enumValueIndex == ScreenMatchingRule.MatchByType.Density;
                i++;
            }

            return found;
        }
        #endregion
    }
}
