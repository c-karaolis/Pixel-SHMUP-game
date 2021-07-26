using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Vector2 screenBounds;
    private Vector2 mousePos;
    private Camera mainCamera;

    private float playerWidth;
    private float playerHeight;
    public float boundsOffsetWidth;
    public float boundsOffsetHeight;

    public float inputSmoothness = 0.01f;

    void Start()
    {
        playerWidth = transform.GetComponent<SpriteRenderer>().bounds.extents.x;
        playerHeight = transform.GetComponent<SpriteRenderer>().bounds.extents.y;

        mainCamera = Camera.main;

        screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));

        mousePos = Vector2.zero;
    }

    void Update()
    {
        if(!Input.GetButton("Fire1")) { return; }

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        mousePos = new Vector2(
            Mathf.Clamp(mousePos.x, screenBounds.x * -1 + playerWidth - boundsOffsetWidth, screenBounds.x - playerWidth + boundsOffsetWidth),
            Mathf.Clamp(mousePos.y, screenBounds.y * -1 + playerHeight - boundsOffsetHeight, screenBounds.y - playerHeight + boundsOffsetHeight)
            );

        Vector3 viewPos = transform.position;

        transform.position = new Vector3(
            Mathf.Lerp(viewPos.x, mousePos.x , inputSmoothness),
            Mathf.Lerp(viewPos.y, mousePos.y , inputSmoothness),
            viewPos.z
            );
    }

}
