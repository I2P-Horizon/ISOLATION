using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(IslandManager))]
public class IslandEditor : Editor
{
    private GUIStyle titleStyle;
    private GUIStyle buttonStyle;
    private GUIStyle seedLabelStyle;

    private void InitStyles()
    {
        titleStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 14,
            normal = { textColor = Color.cyan }
        };

        buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 12,
            fixedHeight = 30
        };

        seedLabelStyle = new GUIStyle(EditorStyles.label)
        {
            fontSize = 11,
            normal = { textColor = Color.white }
        };
    }

    public override void OnInspectorGUI()
    {
        if (titleStyle == null || buttonStyle == null || seedLabelStyle == null) InitStyles();

        DrawDefaultInspector();

        GUILayout.Space(15);
        EditorGUILayout.LabelField("����� ��", titleStyle);
        GUILayout.Space(5);

        IslandManager islandManager = (IslandManager)target;

        GUILayout.Space(10);
        EditorGUILayout.LabelField("�õ� ��", titleStyle);

        EditorGUILayout.BeginVertical("box");
        {
            EditorGUILayout.LabelField("Noise Seed X: "+ islandManager.mapSeed.x, seedLabelStyle);
            EditorGUILayout.LabelField("Noise Seed Z: "+ islandManager.mapSeed.z, seedLabelStyle);
            EditorGUILayout.LabelField("Height Seed X: "+ islandManager.mapSeed.heightX, seedLabelStyle);
            EditorGUILayout.LabelField("Height Seed Z: "+ islandManager.mapSeed.heightZ, seedLabelStyle);
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");
        {
            GUI.backgroundColor = Color.yellow;
            if (GUILayout.Button("�õ� ����", buttonStyle))
            {
                Undo.RecordObject(islandManager, "Init Seed");
                islandManager.Init();
            }

            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("�� ����", buttonStyle))
            {
                Undo.RecordObject(islandManager, "Generate Island");
                islandManager.Create();
            }

            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("�� ����", buttonStyle))
            {
                Undo.RecordObject(islandManager, "Clear Island");
                islandManager.mapSeed.x = 0;
                islandManager.mapSeed.z = 0;
                islandManager.mapSeed.heightX = 0;
                islandManager.mapSeed.heightZ = 0;
                islandManager.Clear();
            }

            GUI.backgroundColor = Color.white;
        }
        EditorGUILayout.EndVertical();
    }
}