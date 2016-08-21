using UnityEngine;
using System.Collections;

public class MapExpoterEditor : MonoBehaviour
{
	public static MapExpoterEditor m_instance = null;

	void Awake ()
	{
		m_instance = this;
	}

	public void SavePrefab (GameObject obj, string path)
	{
		UnityEditor.PrefabUtility.CreatePrefab (path, obj);
	}
}
