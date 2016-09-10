using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Collections.Generic;
using System.IO;

public partial class CreateNewGame_PG : MonoBehaviour
{
	public Noise.NormalizeMode normalizeMode;

	public int mapChunkSize = 128;
	public float noiseScale;

	public int octaves;
	[Range (0, 1)]
	public float persistance;
	public float lacunarity;

	public int seed;
	public Vector2 offset;

	public bool useFalloff;

	public bool autoUpdate;

	public TerrainType[] regions;

	float[,] falloffMap;
	Transform TilesHolder;
	string[,] mapItems;

	Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>> ();

	void Start ()
	{
		falloffMap = FalloffGenerator.GenerateFalloffMap (mapChunkSize);
	}

	public void CreateNewSave ()
	{
		MapData mapData = GenerateMapData (Vector2.zero);
		MapDisplay display = FindObjectOfType<MapDisplay> ();

		SaveTextureToFile (TextureGenerator.TextureFromColourMap (mapData.colourMap, mapChunkSize, mapChunkSize));
	}

	void SaveTextureToFile (Texture2D tex) //SaveTexture
	{
		ES2.SaveImage (tex, "SaveSlot1.png");
		PopulateGameitems (tex);
		LoadMainLevel.m_instance.LoadMainScene_ProceduralGeneration ();
	}

	//******************Other Stuff*****************************************************************************************************
	public void RequestMapData (Vector2 centre, Action<MapData> callback)
	{
		ThreadStart threadStart = delegate {
			MapDataThread (centre, callback);
		};
		new Thread (threadStart).Start ();
	}

	void MapDataThread (Vector2 centre, Action<MapData> callback)
	{
		MapData mapData = GenerateMapData (centre);
		lock (mapDataThreadInfoQueue) {
			mapDataThreadInfoQueue.Enqueue (new MapThreadInfo<MapData> (callback, mapData));
		}
	}

	MapData GenerateMapData (Vector2 centre)
	{

		seed = (int)System.DateTime.Now.Ticks;
		float[,] noiseMap = Noise.GenerateNoiseMap (mapChunkSize + 2, mapChunkSize + 2, seed, noiseScale, octaves, persistance, lacunarity, centre + offset, normalizeMode);

		Color[] colourMap = new Color[mapChunkSize * mapChunkSize];
		for (int y = 0; y < mapChunkSize; y++) {
			for (int x = 0; x < mapChunkSize; x++) {
				if (useFalloff) {
					noiseMap [x, y] = Mathf.Clamp01 (noiseMap [x, y] - falloffMap [x, y]);
				}
				float currentHeight = noiseMap [x, y];
				for (int i = 0; i < regions.Length; i++) {
					if (currentHeight >= regions [i].height) {
						colourMap [y * mapChunkSize + x] = regions [i].colour;
					} else {
						break;
					}
				}
			}
		}
		return new MapData (noiseMap, colourMap);
	}

	void OnValidate ()
	{
		if (lacunarity < 1) {
			lacunarity = 1;
		}
		if (octaves < 0) {
			octaves = 0;
		}
		falloffMap = FalloffGenerator.GenerateFalloffMap (mapChunkSize);
	}

	struct MapThreadInfo<T>
	{
		public readonly Action<T> callback;
		public readonly T parameter;

		public MapThreadInfo (Action<T> callback, T parameter)
		{
			this.callback = callback;
			this.parameter = parameter;
		}
	}

	public void PopulateGameitems (Texture2D map)
	{
		mapItems = new string[mapChunkSize, mapChunkSize];
		for (int x = 0; x < map.height; x++) {
			for (int y = 0; y < map.width; y++) {
				if (map.GetPixel (x, y) == regions [0].colour) {
					FillArrayBlank (x, y);
				} else if (map.GetPixel (x, y) == regions [1].colour) {
					FillArrayBlank (x, y);
				} else if (map.GetPixel (x, y) == regions [2].colour) {
					FillArrayBlank (x, y);
				} else if (map.GetPixel (x, y) == regions [3].colour) {
					Fill2DArray ("grass", x, y, 0.15f);
				} else if (map.GetPixel (x, y) == regions [4].colour) {
					Fill2DArray ("treePrefab", x, y, 0.05f);
				} else if (map.GetPixel (x, y) == regions [5].colour) {
					Fill2DArray ("stone", x, y, 0.5f);
				} else if (map.GetPixel (x, y) == regions [6].colour) {
					Fill2DArray ("log", x, y, 0.1f);
				} else if (map.GetPixel (x, y) == regions [7].colour) {
					FillArrayBlank (x, y);
				}
			}
		}
		ES2.Save (mapItems, "mapItems.txt");
	}

	void Fill2DArray (string itemName, int x, int y, float probability)
	{
		if (UnityEngine.Random.value <= probability) {
			mapItems [x, y] = itemName;
		} else {
			mapItems [x, y] = "";
		}
	}

	void FillArrayBlank (int x, int y)
	{
		mapItems [x, y] = "";
	}

}

[System.Serializable]
public struct TerrainType
{
	public string name;
	public float height;
	public Color colour;
	public GameObject tile;
}

public struct MapData
{
	public readonly float[,] heightMap;
	public readonly Color[] colourMap;

	public MapData (float[,] heightMap, Color[] colourMap)
	{
		this.heightMap = heightMap;
		this.colourMap = colourMap;
	}
}
