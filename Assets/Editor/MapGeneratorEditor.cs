using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapGenerator_PG))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator_PG mapGen = (MapGenerator_PG)target;

        if (GUILayout.Button("Generate"))
        {
            mapGen.DrawMapInEditor();
        }
    }
}