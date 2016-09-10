using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(MapGenerator_PG))]
public class MapGeneratorEditor : Editor
{

	public override void OnInspectorGUI ()
	{
		MapGenerator_PG mapGen = (MapGenerator_PG)target;

		if (DrawDefaultInspector ()) {
			if (mapGen.autoUpdate) {
				//mapGen.DrawMapInEditor ();
			}
		}

		if (GUILayout.Button ("Generate")) {
			mapGen.DrawMapInEditor ();
		}
	}
}