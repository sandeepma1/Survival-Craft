using UnityEngine;
using System.Collections;

public class MapExpoter : MonoBehaviour
{
	//public bool autoUpdate;
	public GameObject[] mapChunks;
	//public string path;
	//public string name;
	GameObject tempMap;
	string[,] mapObjects;

	void Start ()
	{
		//ExportMap ();
	}

	public void StartProcess ()
	{
		mapChunks = GameObject.FindGameObjectsWithTag ("MapChunks");
		print ("Found " + mapChunks.Length + " Map Chunks");
		foreach (var maps in mapChunks) {
			GameObject map = GameObject.Instantiate (maps);
			map.name = maps.name;
			ExportMap (map);
			print (map.name + " processed..");
			DestroyImmediate (map);
		}
	}

	public void ExportMap (GameObject map)
	{		
		mapObjects = new string[128, 128];
		for (int x = 0; x < 128; x++) {
			for (int y = 0; y < 128; y++) {				
				mapObjects [x, y] = "";				
			}
		}

		for (int i = 0; i < map.transform.childCount; i++) {
			if (map.transform.GetChild (i).gameObject.name.StartsWith ("0") || map.transform.GetChild (i).gameObject.name.StartsWith ("1")) {
			} else {
				mapObjects [(int)map.transform.GetChild (i).localPosition.x, (int)map.transform.GetChild (i).localPosition.y] = map.transform.GetChild (i).gameObject.name;
				/*if (parent.transform.childCount != 0) {
					foreach (Transform child in parent.transform.GetChild (i)) {
						child.gameObject.GetComponent <SpriteRenderer> ().sortingOrder = (int)(child.transform.position.y * -10); // Sprite renderer sorting for for all grandchildren or transfor with more than 1 cild
					}
				}
				print (parent.transform.GetChild (i).gameObject.name);*/
			}
		}

		ES2.Save (mapObjects, map.gameObject.name + ".txt");
		print ("Map saved");
		DeleteChildren (map);
	}

	public void DeleteChildren (GameObject map)
	{
		tempMap = GameObject.Instantiate (map);
		tempMap.name = map.name;

		int childCount = tempMap.transform.childCount;
		for (int i = childCount - 1; i > 0; i--) {			
			if (tempMap.transform.GetChild (i).gameObject.name.StartsWith ("0") || tempMap.transform.GetChild (i).gameObject.name.StartsWith ("1")) {
				//Nothing
			} else {
				DestroyImmediate (tempMap.transform.GetChild (i).gameObject);
			}
		}
		print ("Deleted Children");
		SaveAsPrefab (map);
	}

	public void SaveAsPrefab (GameObject map)
	{
#if UNITY_EDITOR
		UnityEditor.PrefabUtility.CreatePrefab ("Assets/Resources/Map/Chunks/" + tempMap.name + ".prefab", tempMap);
		DestroyImmediate (tempMap);
		print ("prefab saved");
#endif
	}

	void DisplaySavedMapData ()
	{
		for (int x = 0; x < 128; x++) {
			for (int y = 0; y < 128; y++) {
				if (mapObjects [x, y] != "") {
					print (mapObjects [x, y] + " X:" + x + " y: " + y);
				}
			}
		}
	}

}