using UnityEngine;
using System;
using System.IO;
using System.Collections;
using LitJson;

// LitJson http://lbv.github.io/litjson/
// LitJSON Quickstart Guide http://lbv.github.io/litjson/docs/quickstart.html

public class MapGenerator : MonoBehaviour
{
	public GameObject floor, water, grass;
	public static MapGenerator m_instance = null;
	JsonData mapJsonObject;
	int x, y = 0;
	int mapSize = 0;
	private Transform MapHolder;
	TextAsset map;

	int[,] layer1;
	int[,] layer2;
	int[,] layer3;
	int[,] layer4;

	GameObject[,] G_layer1;
	GameObject[,] G_layer2;
	GameObject[,] G_layer3;
	GameObject[,] G_layer4;

	void Awake ()
	{
		m_instance = this;
	}

	void Start ()
	{
		map = (TextAsset)Resources.Load ("JSON/TileTest_128", typeof(TextAsset));
		mapJsonObject = JsonMapper.ToObject (map.text);
		GetmapSize ();
		InitializeVariables ();
		LitJSONFunction ();
	}

	void GetmapSize ()
	{
		mapSize = (int)mapJsonObject ["height"];
	}

	void InitializeVariables ()
	{
		layer1 = new int[mapSize, mapSize];
		layer2 = new int[mapSize, mapSize];
		layer3 = new int[mapSize, mapSize];
		layer4 = new int[mapSize, mapSize];

		G_layer1 = new GameObject[mapSize, mapSize];
		G_layer2 = new GameObject[mapSize, mapSize];
		G_layer3 = new GameObject[mapSize, mapSize];
		G_layer4 = new GameObject[mapSize, mapSize];
	}

	void LitJSONFunction ()
	{
		JSonDataTo2DArray (mapJsonObject ["layers"] [0] ["data"], layer1);
		JSonDataTo2DArray (mapJsonObject ["layers"] [1] ["data"], layer2);
		JSonDataTo2DArray (mapJsonObject ["layers"] [2] ["data"], layer3);
		JSonDataTo2DArray (mapJsonObject ["layers"] [3] ["data"], layer4);

		BuildMap (layer1);
		//BuildMap (layer2);
		BuildMap (layer3);
		//BuildMap (layer4);
	}

	void JSonDataTo2DArray (JsonData jLayer, int[,] aLayer)
	{
		int[] iL1 = new int[mapSize * mapSize];
		x = 0;
		y = 0;
		for (int i = 0; i < jLayer.Count; i++) {
			iL1 [i] = (int)jLayer [i];
		}
		// Inverse Read From JSON
		for (int i = iL1.Length - 1; i >= 0; i--) {						
			aLayer [x, y] = iL1 [i];
			x++;
			if (x >= mapSize) {
				y++;
				x = 0;
			}
		}
	}

	void BuildMap (int[,]layer)
	{	
		MapHolder = new GameObject ("Map").transform;
		int layerWidth = (int)Mathf.Sqrt (layer.Length);

		for (int x = 0; x < layerWidth; x++) {
			for (int y = 0; y < layerWidth; y++) {
				switch (layer [x, y]) {
					case 1:						
						InstantiatePrefab (floor, x, y);
						break;
					case 284:
						InstantiatePrefab (water, x, y);
						break;
					case 139:
						G_layer3 [x, y] = Instantiate (grass, new Vector3 (x, y, 0), Quaternion.identity) as GameObject;
						G_layer3 [x, y].transform.SetParent (MapHolder);
						break;
					default:
						break;
				}
			}
		}
	}

	void InstantiatePrefab (GameObject g, int x, int y)
	{
		GameObject inst = Instantiate (g, new Vector3 (x, y, 0), Quaternion.identity) as GameObject;
		inst.transform.SetParent (MapHolder);
	}

	public void GetTileInfo (Vector3 pos)
	{
		if (G_layer3 [(int)pos.x, (int)pos.y] != null) {
			G_layer3 [(int)pos.x, (int)pos.y].SetActive (false);
		}
	}
}