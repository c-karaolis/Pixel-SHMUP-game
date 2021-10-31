using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MultiResolution2D
{
    /// <summary>
    /// The Camera Scaler component is used to scale the Unity camera for controlling visible design world space on different resolutions.
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class CameraScaler : MonoBehaviour
    {
        // Handles data migration between versions
        static int CURRENT_VERSION = 1;

        [SerializeField]
        private int version = 0;

        /// <summary>
        /// The resolution in pixels the game is designed for.
        /// </summary>
        public Vector2 designScreenSize = new Vector2(1136, 640);

        /// <summary>
        /// How many pixels in sprite assets correspond to one unit in the world. (Must be the same value than the value on sprite textures).
        /// </summary>
        public float spritePixelsPerUnit = 1.0f;

        /// <summary>
        /// Return the target screen size in pixels.
        /// If in UNITY_EDITOR it will return the resolution chosen by the user (game window resolution)
        /// otherwise it will return the camera screen size
        /// </summary>
        /// <returns>The screen size in pixels.</returns>
        public Vector2 TargetScreenSize
        {
            get
            {
#if UNITY_EDITOR
                // Return the game window resolution
                Vector2 gameViewSize = Handles.GetMainGameViewSize();
                return gameViewSize;
#else
                // Camera resolution
                Vector2 resolution = new Vector2(this.camera.pixelWidth, this.camera.pixelHeight); 
                return resolution;
#endif
            }
        }

        /// <summary>
        /// Gets the list of auto scaling groups.
        /// </summary>
        public List<AutoScalingGroupData> AutoScalingGroups { get { return this.autoScalingGroupDatas; } }

        /// <summary>
        /// Show the camera bounds title in scene view.
        /// </summary>
        public bool showPreviewTitle = true;

        /// <summary>
        /// The coverage (How much of the design world space is visible.)
        /// </summary>
        public Vector2 Coverage { get { return this.coverage; } }

        /// <summary>
        /// The active scale value.
        /// </summary>
        public float ActiveScale { get { return this.activeScale; } }

        /// <summary>
        /// Gets the user selected auto scaling group.
        /// </summary>
        /// <value>The selected auto scaling group.</value>
        public AutoScalingGroupData SelectedAutoScalingGroup
        {
            get { return this.selectedAutoScalingGroupIndex >= 0 && this.selectedAutoScalingGroupIndex < this.AutoScalingGroups.Count ? this.AutoScalingGroups[this.selectedAutoScalingGroupIndex] : null; }
        }

        // User selected group index. -1 means no selection.
        [SerializeField]
        int selectedAutoScalingGroupIndex = -1;

        // Deprecated: Use the new one instead: autoScalingGroupDatas
        [SerializeField]
        List<AutoScalingGroup> autoScalingGroups;

        [SerializeField]
        List<AutoScalingGroupData> autoScalingGroupDatas;

        // The Unity main camera component.
        new Camera camera;

        // The coverage
        Vector2 coverage;

        // The active scale
        float activeScale;

        #region Life cycle

        // Reset is called when the user hits the Reset button in the Inspector's context menu or when adding the component the first time.
        // This function is only called in editor mode.
        void Reset()
        {
            // Reinit the component: Create the default auto scaling group that matches any resolutions.
            this.autoScalingGroupDatas.Add(AutoScalingGroupData.DefaultAutoScalingGroup);
            this.selectedAutoScalingGroupIndex = 0;
        }

        void Awake()
        {
            this.camera = GetComponent<Camera>();

            if (this.autoScalingGroupDatas == null)
            {
                this.autoScalingGroupDatas = new List<AutoScalingGroupData>();
            }

            MigrateData();
        }

        void OnEnable()
        {
            UpdateCamera();
        }

        #endregion

        #region Update Camera

#if UNITY_EDITOR
        void LateUpdate()
        {
            MigrateData();

            // Update the camera on changes
            UpdateCamera();
        }
#endif

        /// <summary>
        /// Updates the camera. It will find the active scaling group and use his scale mode to change the size of the camera.
        /// </summary>
        public void UpdateCamera()
        {
            if (!this.camera.orthographic)
            {
                return;
            }

            // Retrieve the auto scale factor according to the active auto scaling group
            float orthoSize = GetCameraOrthographicSizeForAutoScalingGroup(GetActiveAutoScalingGroup(), this.designScreenSize, this.TargetScreenSize, this.spritePixelsPerUnit);
            this.camera.orthographicSize = orthoSize;
        }

        // Return the camera orthographic size according to the scale mode of the auto scaling group.
        float GetCameraOrthographicSizeForAutoScalingGroup(AutoScalingGroupData autoScalingGroup, Vector2 designResolution, Vector2 targetResolution, float pixelsPerUnit)
        {
            // Get the scale to apply
            float scale = GetScaleForAutoScalingGroup(autoScalingGroup, designResolution, targetResolution);

            // Update the stats
            this.coverage = new Vector2(targetResolution.x / (designResolution.x * scale), targetResolution.y / (designResolution.y * scale));
            this.activeScale = scale;

            // Calculate orthographic size
            // Orthographic size = (resolution.y / (scale * ppu)) * 0.5
            float orthoSize = (targetResolution.y / (scale * pixelsPerUnit)) * 0.5f;

            return orthoSize;
        }

        // Return the scale factor of the auto scaling group according to the scale mode and design values.
        float GetScaleForAutoScalingGroup(AutoScalingGroupData autoScalingGroup, Vector2 designResolution, Vector2 targetResolution)
        {
            // Default is 1.0 if there is no auto scaling group active.
            float scale = 1.0f;
            if (autoScalingGroup != null)
            {
                scale = autoScalingGroup.GetAutoScaleFactor(designResolution, targetResolution);
            }
            return scale;
        }

        #endregion

        /// <summary>
        /// Return the first active auto scaling group.
        /// </summary>
        /// <returns>The active auto scaling group.</returns>
        public AutoScalingGroupData GetActiveAutoScalingGroup()
        {
            AutoScalingGroupData activeGroup = null;
            var activeGroupIndex = GetActiveAutoScalingGroupIndex();

            if (activeGroupIndex >= 0 && activeGroupIndex < this.AutoScalingGroups.Count) {
                activeGroup = this.AutoScalingGroups[activeGroupIndex];
            }

            return activeGroup;
        }

        /// <summary>
        /// Return the index of the first active auto scaling group.
        /// </summary>
        /// <returns>The active auto scaling group.</returns>
        public int GetActiveAutoScalingGroupIndex()
        {
            // Get current screen resolution
            Vector2 currentScreenSize = this.TargetScreenSize;

            // Find the first active auto scaling group
            return this.AutoScalingGroups.FindIndex(scalingGroup => scalingGroup.IsActive(currentScreenSize));
        }

        /// <summary>
        /// Migrates the data.
        /// </summary>
        private void MigrateData()
        {
            // Latest version: no migration needed
            if (this.version == CURRENT_VERSION)
            {
                return;
            }

            if (this.version == 0)
            {
                // Changelog: Migrate from ScriptableObjects to csharp class for the list of autoscaling groups.
                if (this.autoScalingGroups != null)
                {
                    //Debug.Log(this.autoScalingGroups.Count + " AutoScalingGroup to migrate.");
                    for (int i = 0; i < this.autoScalingGroups.Count; i++) {
                        AutoScalingGroup oldData = this.autoScalingGroups[i];

                        AutoScalingGroupData newData = AutoScalingGroupData.CreateInstanceFromOldAutoScalingGroup(oldData);
                        this.AutoScalingGroups.Add(newData);
                    }
                    this.autoScalingGroups.Clear();
                    this.autoScalingGroups = null;
                }
            }

            Debug.Log("MultiResolution2D: Data migrated from version " + this.version.ToString());
            this.version = CURRENT_VERSION;
        }

#if UNITY_EDITOR

        // Different properties and functions
        // to call them from the editor script

        public float EditorGetScaleForAutoScalingGroup(AutoScalingGroupData autoScalingGroup, Vector2 designResolution, Vector2 targetResolution)
        {
            return GetScaleForAutoScalingGroup(autoScalingGroup, designResolution, targetResolution);
        }
#endif
    }
}