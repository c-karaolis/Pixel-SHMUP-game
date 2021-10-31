using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MultiResolution2D
{
    /// <summary>
    /// Create MultiResolution menus.
    /// cf: https://docs.unity3d.com/ScriptReference/MenuItem.html
    /// </summary>
    public static class Menu
    {
        /// <summary>
        /// Add a menu item to create custom MultiResolution Camera.
        /// </summary>
        /// <param name="menuCommand">Menu command.</param>
        [MenuItem("GameObject/MultiResolution 2D/Camera", false, 10)]
        static void CameraScaler(MenuCommand menuCommand)
        {
            // Create a custom game object
            GameObject go = new GameObject("MultiResolution Camera");
            go.transform.position = new Vector3(0, 0, -10.0f);

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);

            // Add components
            Camera camera = go.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 480.0f;
            camera.farClipPlane = 1000.0f;
            camera.nearClipPlane = 0.3f;
            camera.depth = -1.0f;

            go.AddComponent<CameraScaler>();

            go.AddComponent<FlareLayer>();

            if (Object.FindObjectsOfType(typeof(AudioListener)).Length == 0)
            {
                go.AddComponent<AudioListener>();
            }

            // Default Camera if the only one
            if (Camera.main == null)
            {
                go.tag = "MainCamera";
            }

            Undo.RegisterCreatedObjectUndo(go, "Create" + go.name);
            Selection.activeGameObject = go;
        }

        /// <summary>
        /// Add a menu item to create custom MultiResolution Camera Anchor.
        /// </summary>
        /// <param name="menuCommand">Menu command.</param>
        [MenuItem("GameObject/MultiResolution 2D/Camera Anchor", false, 10)]
        static void CameraAnchor(MenuCommand menuCommand)
        {
            GameObject go = menuCommand.context as GameObject;

            if (go == null) {
                // Create a custom game object
                go = new GameObject("CameraAnchor");
                go.transform.position = new Vector3(0, 0, -10.0f);

                Undo.RegisterCreatedObjectUndo(go, "Create" + go.name);
            }

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);

            // Add components
            if (go.GetComponent<CameraAnchor>() == null) {
                go.AddComponent<CameraAnchor>();
            }

            Selection.activeGameObject = go;
        }

    }

}