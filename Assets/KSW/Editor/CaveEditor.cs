using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CaveManager))]
public class CaveEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CaveManager caveManager = (CaveManager)target;

        if (GUILayout.Button("Build Cave"))
        {
            caveManager.BuildCave();
        }

        if (GUILayout.Button("Clear Cave"))
        {
            caveManager.ClearCave();
        }
    }
}