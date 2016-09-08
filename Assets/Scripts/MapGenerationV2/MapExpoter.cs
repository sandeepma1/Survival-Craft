using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class MapExpoter : MonoBehaviour {
	//public bool autoUpdate;
	public GameObject[] mapChunks;
	//public string path;
	//public string name;
	GameObject tempMap;
	string[,] mapObjects;
	TextAsset[] mapItems;

	public void ExportToCurrentMaps() {
		mapItems = Resources.LoadAll<TextAsset>("Saves/");
		for (int i = 0; i < mapItems.Length; i++) {
			string[] array = mapItems[i].text.Split('\n');
			if (ES2.Exists(mapItems[i].name + ".txt")) {
				ES2.Delete(mapItems[i].name + ".txt");
				ES2.Save(SingleToMulti((string[])array), mapItems[i].name + ".txt");
			}
		}
	}

	public void StartProcess() {
		mapChunks = GameObject.FindGameObjectsWithTag("MapChunks");

		print("Found " + mapChunks.Length + " Map Chunks");
		foreach (var maps in mapChunks) {
			GameObject map = GameObject.Instantiate(maps);
			map.name = maps.name;
			ExportMap(map);
			print(map.name + " processed..");
			DestroyImmediate(map);
		}

	}

	public void ExportMap(GameObject map) {
		int mapSize = (int)map.GetComponent<CreativeSpore.SuperTilemapEditor.Tilemap>().MapBounds.size.x;
		mapObjects = new string[mapSize, mapSize];
		for (int x = 0; x < mapSize; x++) {
			for (int y = 0; y < mapSize; y++) {
				mapObjects[x, y] = "";
			}
		}

		for (int i = 0; i < map.transform.childCount; i++) {
			if (map.transform.GetChild(i).gameObject.tag == "Chunks") {
			} else {
				mapObjects[(int)map.transform.GetChild(i).localPosition.x, (int)map.transform.GetChild(i).localPosition.y] = map.transform.GetChild(i).gameObject.name;
			}
		}
		if (File.Exists("Assets/Resources/Saves/" + map.name + ".txt")) {
			File.Delete("Assets/Resources/Saves/" + map.name + ".txt");
			File.WriteAllLines("Assets/Resources/Saves/" + map.name + ".txt", MultiToSingle(mapObjects));
			//ExportToCurrentMaps ();
		} else {
			File.WriteAllLines("Assets/Resources/Saves/" + map.name + ".txt", MultiToSingle(mapObjects));
		}
		print("Map saved");
		DeleteChildren(map);
	}

	public void DeleteChildren(GameObject map) {
		tempMap = GameObject.Instantiate(map);
		tempMap.name = map.name;

		int childCount = tempMap.transform.childCount;
		for (int i = childCount - 1; i > 0; i--) {
			if (tempMap.transform.GetChild(i).gameObject.tag == "Chunks") {
				//Nothing
			} else {
				DestroyImmediate(tempMap.transform.GetChild(i).gameObject);
			}
		}
		print("Deleted Children");
		SaveAsPrefab(map);
	}

	public void SaveAsPrefab(GameObject map) {
#if UNITY_EDITOR
		UnityEditor.PrefabUtility.CreatePrefab("Assets/Resources/Map/Chunks/" + tempMap.name + ".prefab", tempMap);
		DestroyImmediate(tempMap);
		print("prefab saved");
#endif
	}

	void DisplaySavedMapData() {
		for (int x = 0; x < 128; x++) {
			for (int y = 0; y < 128; y++) {
				if (mapObjects[x, y] != "") {
					print(mapObjects[x, y] + " X:" + x + " y: " + y);
				}
			}
		}
	}

	static string[] MultiToSingle(string[,] array) {
		int index = 0;
		int width = array.GetLength(0);
		int height = array.GetLength(1);
		string[] single = new string[width * height];
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				single[index] = array[x, y];
				index++;
			}
		}
		return single;
	}

	static string[,] SingleToMulti(string[] array) {
		int index = 0;
		int sqrt = (int)Mathf.Sqrt(array.Length);
		string[,] multi = new string[sqrt, sqrt];
		for (int y = 0; y < sqrt; y++) {
			for (int x = 0; x < sqrt; x++) {
				multi[x, y] = array[index];
				index++;
			}
		}
		return multi;
	}
}