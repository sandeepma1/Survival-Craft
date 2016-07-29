using UnityEngine;
using System;
using System.IO;
using System.Collections;
using LitJson;

// LitJson http://lbv.github.io/litjson/
// LitJSON Quickstart Guide http://lbv.github.io/litjson/docs/quickstart.html

public class MapGenerator_old : MonoBehaviour
{
	public GameObject floor, water;
	public GameObject[] ranObjects;
	//grass, grass1, grass2, log, stone, stone1;
	public static MapGenerator_old m_instance = null;
	public GameObject grassOutput, stoneOutput, logOutput;
	JsonData mapJsonObject;
	int x, y = 0;
	int mapSize = 0;
	private Transform FloorMapHolder, PropsMapHolder;
	TextAsset map;

	int[,] layer1;
	int[,] layer2;

	GameObject[,] G_layer1_floor;
	GameObject[,] G_layer2_props;

	void Awake ()
	{
		m_instance = this;
	}

	void Start ()
	{
		map = (TextAsset)Resources.Load ("JSON/TestMap_64", typeof(TextAsset));
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

		G_layer1_floor = new GameObject[mapSize, mapSize];
		G_layer2_props = new GameObject[mapSize, mapSize];
	}

	void LitJSONFunction ()
	{
		JSonDataTo2DArray (mapJsonObject ["layers"] [0] ["data"], layer1);
		JSonDataTo2DArray (mapJsonObject ["layers"] [1] ["data"], layer2);

		BuildMap (layer1);
		BuildMap (layer2);
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
		FloorMapHolder = new GameObject ("Floor").transform;
		PropsMapHolder = new GameObject ("Props").transform;
		int layerWidth = (int)Mathf.Sqrt (layer.Length);

		for (int x = 0; x < layerWidth; x++) {
			for (int y = 0; y < layerWidth; y++) {
				switch (layer [x, y]) {
					case 74:						
						InstantiateFloorPrefab (floor, x, y);
						break;
					case 284:
						InstantiateFloorPrefab (water, x, y);
						break;
				//	case 139:
				//G_layer3 [x, y] = Instantiate (grass, new Vector3 (x, y, 0), Quaternion.identity) as GameObject;
				//	G_layer3 [x, y].transform.SetParent (MapHolder);
				//	break;
					case 139:
						int r = UnityEngine.Random.Range (0, 6);
						InstantiatePropPrefab (ranObjects [r], x, y);
						break;
					default:
						break;
				}
			}
		}
	}

	void InstantiateFloorPrefab (GameObject g, int x, int y)
	{
		GameObject inst = Instantiate (g, new Vector3 (x, y, 0), Quaternion.identity) as GameObject;
		inst.transform.SetParent (FloorMapHolder);
	}

	void InstantiatePropPrefab (GameObject g, int x, int y)
	{
		G_layer2_props [x, y] = Instantiate (g, new Vector3 (x, y, 0), Quaternion.identity) as GameObject;
		G_layer2_props [x, y].transform.SetParent (PropsMapHolder);
	}

	public void GetTileInfo (Vector2 pos)
	{	
		if (G_layer2_props [(int)pos.x, (int)pos.y] != null) {
			G_layer2_props [(int)pos.x, (int)pos.y].SetActive (false);
			DropOutputs (G_layer2_props [(int)pos.x, (int)pos.y].name, pos.x, pos.y);
			G_layer2_props [(int)pos.x, (int)pos.y] = null;
		}
	}

	public GameObject GetTile (Vector2 pos)
	{	
		if (G_layer2_props [(int)pos.x, (int)pos.y] != null) {
			return G_layer2_props [(int)pos.x, (int)pos.y];
			//DropOutputs (G_layer2_props [(int)pos.x, (int)pos.y].name, pos.x, pos.y);
			//G_layer2_props [(int)pos.x, (int)pos.y] = null;
		}
		return null;
	}

	void DropOutputs (string name, float x, float y)
	{
		GameObject output = null;
		switch (name) {
			case "Grass(Clone)":
			case "Grass1(Clone)":
			case "Grass2(Clone)":
				output = Instantiate (grassOutput, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
				break;
			case "Stone(Clone)":
			case "Stone1(Clone)":
				output = Instantiate (stoneOutput, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
				break;
			case"Log(Clone)":
				output = Instantiate (logOutput, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
				break;
			default:
				break;
		}
	}
}