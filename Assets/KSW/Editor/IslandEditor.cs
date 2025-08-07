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

        if (GUILayout.Button("�õ� ����"))
        {
            Undo.RecordObject(manager, "Init Seed");
            manager.Init();
        }

        if (GUILayout.Button("�� ����"))
        {
            Undo.RecordObject(manager, "Generate Island");
            manager.Create();
        }

        if (GUILayout.Button("�� ����"))
        {
            Undo.RecordObject(manager, "Clear Island");
            manager.Clear();
        }
    }
}