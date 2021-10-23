using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FunkyCode.Utilities;

public class CameraTransform
{
    public static List<CameraTransform> list = new List<CameraTransform>();

    private Camera camera = null;

    private Polygon2 worldPolygon = null;
    private Rect worldRect = new Rect();

    private Polygon2 polygon = null;

    private Vector2 position = Vector2.zero;
    private float rotation = 0;
    private Vector2 scale = Vector2.one;
    private float size = 0;

    // Change
    public static float GetRadius(Camera camera) {
        float cameraRadius = camera.orthographicSize;

        if (camera.pixelWidth > camera.pixelHeight) {
            cameraRadius *= (float)camera.pixelWidth / camera.pixelHeight;
        }

        cameraRadius = Mathf.Sqrt(cameraRadius * cameraRadius + cameraRadius * cameraRadius);

        return(cameraRadius);
    }





    public static Rect GetWorldRect(Camera camera) {
        CameraTransform cameraTransform = GetCamera(camera);

        return(cameraTransform.WorldRect());
    }

    public static CameraTransform GetCamera(Camera camera) {
        if (camera == null) {
            Debug.LogError("Camera == Null");
        }

        foreach(CameraTransform transform in list) {
            if (transform.camera == camera) {
                return(transform);
            }
        }

        CameraTransform cameraTransform = null;

        cameraTransform = new CameraTransform();

        cameraTransform.camera = camera;

        list.Add(cameraTransform);

        return(cameraTransform);
    }









    public void Update() {
        if (camera == null) {
            return;
        }
        
        Transform transform = camera.transform;

        Vector2 position2D = LightingPosition.GetPosition2D(transform.position);
        Vector2 scale2D = transform.lossyScale;
        float rotation2D = LightingPosition.GetRotation2D(transform);
        float size2D = camera.orthographicSize;

        bool update = false;

        if (position != position2D) {
            position = position2D;

            update = true;
        }

        if (scale != scale2D) {
            scale = scale2D;

            update = true;
        }

        if (rotation != rotation2D) {
            rotation = rotation2D;

            update = true;
        }

        if (size != size2D) {
            size = size2D;

            update = true;
        }

        if (update) {
            worldPolygon = null;
        }
    }





    private Polygon2 WorldPolyon() {
        if (worldPolygon != null) {
            return(worldPolygon);
        }

        float cameraSizeY = camera.orthographicSize;
        float cameraSizeX = cameraSizeY * (float)camera.pixelWidth / camera.pixelHeight;

        float sizeX = cameraSizeX * 2;
        float sizeY = cameraSizeY * 2;

        float x = -sizeX / 2;
        float y = -sizeY / 2;

        worldPolygon = Polygon();

        worldPolygon.points[0].x = x;
        worldPolygon.points[0].y = y;

        worldPolygon.points[1].x = x + sizeX;
        worldPolygon.points[1].y = y;

        worldPolygon.points[2].x = x + sizeX;
        worldPolygon.points[2].y = y + sizeY;

        worldPolygon.points[3].x = x;
        worldPolygon.points[3].y = y + sizeY;

        worldPolygon.ToRotationSelf(rotation * Mathf.Deg2Rad);
        worldPolygon.ToOffsetSelf(position);

        worldRect = worldPolygon.GetRect();

        return(worldPolygon);
    }

    private Rect WorldRect() {
        WorldPolyon();

        return(worldRect);
    }
    
    private Polygon2 Polygon() {
        if (polygon == null) {
            polygon = new Polygon2(4);
            polygon.points[0] = Vector2.zero;
            polygon.points[1] = Vector2.zero;
            polygon.points[2] = Vector2.zero;
            polygon.points[3] = Vector2.zero;
        }

        return(polygon);
    }
}
