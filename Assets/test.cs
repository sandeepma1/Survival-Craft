using UnityEngine;
using System;
using System.IO;
using System.Collections;
using LitJson;

// LitJson http://lbv.github.io/litjson/
// LitJSON Quickstart Guide http://lbv.github.io/litjson/docs/quickstart.html

public class test : MonoBehaviour
{
	public GameObject floor, water, grass;
	public static test m_instance = null;
	JsonData mapJsonObject;
	int mapSize = 0;
	private Transform MapHolder;
	TextAsset map;

	public Sprite[] sprites;

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
		map = (TextAsset)Resources.Load ("JSON/one/three", typeof(TextAsset));
		sprites = Resources.LoadAll<Sprite> ("JSON/one/farm_big");
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
		JSonDataTo2DArray (mapJsonObject ["layers"] [0] ["data"], layer1, mapJsonObject ["layers"] [0] ["name"].ToString ());
		JSonDataTo2DArray (mapJsonObject ["layers"] [1] ["data"], layer2, mapJsonObject ["layers"] [1] ["name"].ToString ());
		JSonDataTo2DArray (mapJsonObject ["layers"] [2] ["data"], layer3, mapJsonObject ["layers"] [2] ["name"].ToString ());
		//JSonDataTo2DArray (mapJsonObject ["layers"] [1] ["data"], layer2);
		//JSonDataTo2DArray (mapJsonObject ["layers"] [2] ["data"], layer3);
	}

	void JSonDataTo2DArray (JsonData jLayer, int[,] aLayer, string layerName)
	{
		int[] iL1 = new int[mapSize * mapSize];
		for (int i = 0; i < jLayer.Count; i++) {
			iL1 [i] = (int)jLayer [i];
			//print (iL1 [i]);
		}
		int cnt = 0;
		for (int x = mapSize - 1; x >= 0; x--) {
			for (int y = 0; y < mapSize; y++) {
				aLayer [y, x] = iL1 [cnt];
				print (sprites [aLayer [y, x]].name);
				if (aLayer [y, x] > 0) {
					InstantiateSprite (sprites [aLayer [y, x] - 1], y, x, layerName);
				}
									
				cnt++;
			}
		}
	}

	void InstantiateSprite (Sprite s, int x, int y, string sortingLayerName)
	{
		GameObject go = new GameObject ("New Sprite");
		go.transform.position = new Vector3 (x, y, 0);
		go.transform.SetParent (transform);
		SpriteRenderer renderer = go.AddComponent<SpriteRenderer> ();
		renderer.sortingLayerName = sortingLayerName;
		renderer.sprite = s;
	}
}