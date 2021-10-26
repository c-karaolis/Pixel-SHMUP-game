using UnityEngine;

namespace Foxlair.Player.Movement
{
    public class PlayerMovement : MonoBehaviour
    {
        private Vector2 screenBounds;
        private Vector2 mousePosition;
        public Camera mainCamera;

        private float playerSpriteWidth;
        private float playerSpriteHeight;

        [Range(0f, 15f)]
        public float boundsWidthOffset;
        [Range(0f, 15f)]
        public float boundsHeightOffset;
        [Range(0f, 2f)]
        public float traversalSmoothness = 0.01f;

        void Start()
        {
            playerSpriteWidth = transform.GetComponent<SpriteRenderer>().bounds.extents.x;
            playerSpriteHeight = transform.GetComponent<SpriteRenderer>().bounds.extents.y;

            mainCamera = Camera.main;

            screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));

            mousePosition = Vector2.zero;
        }

        void Update()
        {
            if (!Input.GetButton("Fire1")) { return; }

            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            mousePosition = new Vector2(
                Mathf.Clamp(mousePosition.x, screenBounds.x * -1 + playerSpriteWidth - boundsWidthOffset, screenBounds.x - playerSpriteWidth + boundsWidthOffset),
                Mathf.Clamp(mousePosition.y, screenBounds.y * -1 + playerSpriteHeight - boundsHeightOffset, screenBounds.y - playerSpriteHeight + boundsHeightOffset)
                );

            Vector3 viewPos = transform.position;

            transform.position = new Vector3(
                Mathf.Lerp(viewPos.x, mousePosition.x, traversalSmoothness),
                Mathf.Lerp(viewPos.y, mousePosition.y, traversalSmoothness),
                viewPos.z
                );
        }

    }
}
