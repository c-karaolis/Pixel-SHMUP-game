using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

/*
    This is an editor script that allows the FireBullet script to generate a ShootingPreset scriptable object asset file
    When the Save Preset button is clicked, a Scriptable Object that contains the live values of the FireBullet script is
    created with whatever filename is supplied. The Scriptable Object file name and preset values are all generated in the FireBullet
    script and accessed by it's getValuesAsPreset() function.
*/
namespace Foxlair.Bullets
{
    [CustomEditor(typeof(FireBullet))]
    public class FireBulletEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            //Makes the rest of the FireBullet inspector options available (IE all parameters in FireBullet that are declared as
            //public or serializable will still display in the inspector)
            DrawDefaultInspector();

            //The reference to this FireBullet script
            FireBullet myScript = (FireBullet)target;

            //Declares the Save Preset button in the expector and triggers when it is clicked
            if (GUILayout.Button("Save Preset"))
            {
                //Passes the live preset values in a ShootingPreset scriptable object from the FireBullet script 
                //into the SaveAsPreset function
                SaveAsPreset(myScript.GetValuesAsPreset());
            }
        }

        //Takes a ShootingPreset scriptable object and creates an asset file from it.
        private void SaveAsPreset(ShootingPreset thisPreset)
        {
            MonoScript ms = MonoScript.FromScriptableObject(this);
            string scriptFilePath = AssetDatabase.GetAssetPath(ms);
            DirectoryInfo presetsFolder = Directory.GetParent(scriptFilePath);

            AssetDatabase.CreateAsset(thisPreset, presetsFolder.Parent.FullName.Substring(presetsFolder.Parent.FullName.IndexOf("Assets")) + "/Presets/" + thisPreset.name.Replace(" ", "") + ".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

    }
}