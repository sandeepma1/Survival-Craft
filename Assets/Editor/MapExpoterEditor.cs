using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(MapExpoter))]
public class MapExpoterEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		MapExpoter mapExp = (MapExpoter)target;
		if (GUILayout.Button ("SaveMap DeleteChild SavePrefab")) {
			mapExp.StartProcess ();
		}
		/*if (GUILayout.Button ("SaveMap DeleteChild SavePrefab")) {
			mapExp.StartProcess ();
		}*/
		/*if (GUILayout.Button ("Delete Children")) {
			mapExp.DeleteChildren ();
		}

		if (GUILayout.Button ("Save Prefab")) {
			mapExp.SaveAsPrefab ();
		}*/
	}
}
