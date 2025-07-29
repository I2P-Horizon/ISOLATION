using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(IslandManager))]
public class IslandEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        IslandManager manager = (IslandManager)target;

        GUILayout.Space(10);

        if (GUILayout.Button("矫靛 积己"))
        {
            Undo.RecordObject(manager, "Init Seed");
            manager.Init();
        }

        if (GUILayout.Button("级 积己"))
        {
            Undo.RecordObject(manager, "Generate Island");
            manager.Create();
        }

        if (GUILayout.Button("级 昏力"))
        {
            Undo.RecordObject(manager, "Clear Island");
            manager.Clear();
        }
    }
}