using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiResolution2D
{
    /// <summary>
    /// The Camera Anchor component is used to automatically anchors the gameobject position to the anchor point.
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(Transform))]
    public class CameraAnchor : MonoBehaviour
    {
        /// <summary>
        /// The rendering camera.
        /// </summary>
        [SerializeField]
        Camera renderCamera;

        [SerializeField]
        [Tooltip("0,0 corresponds to anchoring to the lower left corner of the screen rectangle, while 1,1 corresponds to anchoring to the upper right corner of the screen rectangle.")]
        Vector2 anchorPoint = new Vector2(0.5f, 0.5f);

        /// <summary>
        /// The offset in pixels from the Anchor Point.
        /// </summary>
        [Tooltip("Anchor Point offset in pixels.")]
        public Vector2 offset = Vector2.zero;

        /// <summary>
        /// The anchor point defined as a fraction of the size of the screen rectangle. 
        /// 0,0 corresponds to anchoring to the lower left corner of the screen rectangle, while 1,1 corresponds to anchoring to the upper right corner of the screen rectangle.
        /// </summary>
        public Vector2 AnchorPoint
        {
            get { return this.anchorPoint; }
            set
            {
                this.anchorPoint = new Vector2(Mathf.Clamp01(value.x), Mathf.Clamp01(value.y));
            }
        }


        CameraScaler cameraScaler = null;

        Camera RenderCamera {
            get {
                return this.renderCamera;
            }
            set {
                this.renderCamera = value;

                // Get the CameraScaler component if exist (and refresh it if user replace the render camera)
                this.cameraScaler = null;
                if (this.renderCamera != null)
                {
                    this.cameraScaler = this.renderCamera.GetComponent<CameraScaler>();
                }
            }
        }

        Transform cachedTransform;
        Transform CachedTransform {
            get
            {
                if (this.cachedTransform == null) {
                    this.cachedTransform = transform;
                }
                return this.cachedTransform;
            }
        } 

        private void OnValidate()
        {
            AnchorPoint = this.anchorPoint;
            RenderCamera = this.renderCamera;
        }

        void Start()
        {
            UpdateAnchoredPosition();
        }

        // Update is called once per frame
        void LateUpdate()
        {
            UpdateAnchoredPosition();
        }

        void UpdateAnchoredPosition()
        {
            // Can not update if not camera referenced
            if (this.RenderCamera == null)
            {
                return;
            }

            Rect screenRect = this.RenderCamera.pixelRect;

            // Get the anchored position
            Vector3 anchoredPosition = new Vector3 (
                (screenRect.xMin + screenRect.xMax) * this.AnchorPoint.x,
                (screenRect.yMin + screenRect.yMax) * this.AnchorPoint.y,
                0f
            );

            float scale = 1.0f;
            if (this.cameraScaler != null) {
                scale = this.cameraScaler.ActiveScale;
            }

            // Apply the screen offset in pixels
            anchoredPosition = anchoredPosition + new Vector3(this.offset.x * scale, this.offset.y * scale, 0f);


            // Set the anchored position
            Vector3 worldAnchoredPosition = this.RenderCamera.ScreenToWorldPoint(anchoredPosition);
            worldAnchoredPosition.z = this.CachedTransform.position.z;
            
            if (this.CachedTransform.position != worldAnchoredPosition) {
                this.CachedTransform.position = worldAnchoredPosition;
            }
        }
    }
}
