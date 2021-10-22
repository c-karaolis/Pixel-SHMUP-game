
using UnityEditor;
using LightingSettings;

public static class Lighting2DGizmoFiles {
    public static void Initialize() {
        if (Lighting2D.ProjectSettings.editorView.drawIcons == EditorIcons.Disabled) {
            return;
        }
            
        bool icon_light = UnityEngine.Windows.File.Exists("Assets/Gizmos/light_v2.png");

        if (!icon_light) {
            UnityEngine.Debug.Log("false");
            
            try {
                FileUtil.CopyFileOrDirectory("Assets/FunkyCode/SmartLighting2D/Resources/Gizmos", "Assets/Gizmos");
            } catch {
            }

            try {
                FileUtil.CopyFileOrDirectory("Assets/FunkyCode/SmartLighting2D/Resources/Gizmos/light_v2.png", "Assets/Gizmos/light_v2.png");
                FileUtil.CopyFileOrDirectory("Assets/FunkyCode/SmartLighting2D/Resources/Gizmos/fow_v2.png", "Assets/Gizmos/fow_v2.png");
                FileUtil.CopyFileOrDirectory("Assets/FunkyCode/SmartLighting2D/Resources/Gizmos/circle_v2.png", "Assets/Gizmos/circle_v2.png");

            } catch {
            }
        }
    }
}

[InitializeOnLoad]
public class Lighting2DStartup {
    static Lighting2DStartup () {
        Lighting2DGizmoFiles.Initialize();
    }
}