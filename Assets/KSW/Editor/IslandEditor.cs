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

        IslandManager manager = (IslandManager)target;

        GUILayout.Space(10);
        EditorGUILayout.LabelField("�õ� ��", titleStyle);

        EditorGUILayout.BeginVertical("box");
        {
            EditorGUILayout.LabelField("Noise Seed X: "+manager.mapSeed.x, seedLabelStyle);
            EditorGUILayout.LabelField("Noise Seed Z: "+manager.mapSeed.z, seedLabelStyle);
            EditorGUILayout.LabelField("Height Seed X: "+manager.mapSeed.heightX, seedLabelStyle);
            EditorGUILayout.LabelField("Height Seed Z: "+manager.mapSeed.heightZ, seedLabelStyle);
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");
        {
            EditorGUILayout.LabelField($"������ ��� ��: "+manager.totalBlocks + "��", seedLabelStyle);
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");
        {
            GUI.backgroundColor = Color.yellow;
            if (GUILayout.Button("�õ� ����", buttonStyle))
            {
                Undo.RecordObject(manager, "Init Seed");
                manager.Init();
            }

            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("�� ����", buttonStyle))
            {
                Undo.RecordObject(manager, "Generate Island");
                manager.Create();
            }

            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("�� ����", buttonStyle))
            {
                Undo.RecordObject(manager, "Clear Island");
                manager.mapSeed.x = 0;
                manager.mapSeed.z = 0;
                manager.mapSeed.heightX = 0;
                manager.mapSeed.heightZ = 0;
                manager.totalBlocks = 0;
                manager.Clear();
            }

            GUI.backgroundColor = Color.white;
        }
        EditorGUILayout.EndVertical();
    }
}