using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DungeonGenerator))]
public class CustomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if(GUILayout.Button("Generate"))
        {
            ((DungeonGenerator)target).tilemapVisualizer.Clear();
            ((DungeonGenerator)target).RunProceduralGeneration();
        }
    }
}
