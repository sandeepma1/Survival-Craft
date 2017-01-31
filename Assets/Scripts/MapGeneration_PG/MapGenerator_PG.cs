using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Collections.Generic;
using System.IO;

public class MapGenerator_PG : MonoBehaviour
{

	public enum DrawMode
	{
		NoiseMap,
		ColourMap,
		FalloffMap}

	;

	public DrawMode drawMode;

	public Noise.NormalizeMode normalizeMode;

	public int mapChunkSize = 128;
	public float noiseScale;

	public int octaves;
	[Range (0, 1)]
	public float persistance;
	public float lacunarity;

	public int seed;
	public Vector2 offset, falloff;

	public bool useFalloff;

	/*	public float meshHeightMultiplier;
	public AnimationCurve meshHeightCurve;*/

	public bool autoUpdate;

	public TerrainType_Editor[] regions;
	public bool GenerateTerrian = false;

	float[,] falloffMap;
	Transform TilesHolder;
	//= new GameObject ("TilesHolder").transform;

	Queue<MapThreadInfo<MapData_Editor>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData_Editor>> ();
	//Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>> ();

	void Start ()
	{
		falloffMap = FalloffGenerator.GenerateFalloffMap (mapChunkSize, falloff);
	}

	public void DrawMapInEditor ()
	{
		MapData_Editor mapData = GenerateMapData_Editor (Vector2.zero);
		MapDisplay display = FindObjectOfType<MapDisplay> ();
		if (drawMode == DrawMode.NoiseMap) {
			display.DrawTexture (TextureGenerator.TextureFromHeightMap (mapData.heightMap));
		} else if (drawMode == DrawMode.FalloffMap) {
			display.DrawTexture (TextureGenerator.TextureFromHeightMap (FalloffGenerator.GenerateFalloffMap (mapChunkSize, falloff)));
		} else if (drawMode == DrawMode.ColourMap) {
			SaveTextureToFile (TextureGenerator.TextureFromColourMap (mapData.colourMap, mapChunkSize, mapChunkSize));
			display.DrawTexture (LoadTextureFromFile ((Texture2D)ES2.LoadImage ("EditorTest.png")));
			if (GenerateTerrian) {
				//GenerateMap (TextureGenerator.TextureFromColourMap (mapData.colourMap, mapChunkSize, mapChunkSize));
			}
		}
	}

	void GenerateMap (Texture2D tex)
	{
		Color color;
		TilesHolder = new GameObject ("TilesHolder").transform;
		TilesHolder.transform.position = new Vector3 (mapChunkSize / 2, mapChunkSize / 2);
		for (int i = 0; i < tex.width; i++) {
			for (int j = 0; j < tex.height; j++) {
				color = tex.GetPixel (i, j);
				if (color == regions [0].colour) {
					InstansiateTiles (regions [0].tile, i, j);
				} else if (color == regions [1].colour) {					
					InstansiateTiles (regions [1].tile, i, j);
				} else if (color == regions [2].colour) {
					InstansiateTiles (regions [2].tile, i, j);
				} else if (color == regions [3].colour) {
					InstansiateTiles (regions [3].tile, i, j);				
				} else if (color == regions [4].colour) {
					InstansiateTiles (regions [4].tile, i, j);
				} else if (color == regions [5].colour) {
					InstansiateTiles (regions [5].tile, i, j);
				} else if (color == regions [6].colour) {
					InstansiateTiles (regions [6].tile, i, j);
				} else if (color == regions [7].colour) {
					InstansiateTiles (regions [7].tile, i, j);
				} else {
					InstansiateTiles (regions [0].tile, i, j);
				}
			}
		}
		TilesHolder.transform.position = Vector3.zero;
	}

	void InstansiateTiles (GameObject tile, int x, int y)
	{
		GameObject inst = Instantiate (tile, new Vector3 (x, y, 0), Quaternion.identity) as GameObject;
		inst.transform.SetParent (TilesHolder);
	}

	void SaveTextureToFile (Texture2D tex) //SaveTexture
	{
		ES2.SaveImage (tex, "EditorTest.png");
	}

	Texture2D LoadTextureFromFile (Texture2D tex) //LoadTexture
	{
		Texture2D texture = new Texture2D (tex.width, tex.height);
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.SetPixels (tex.GetPixels ());
		texture.Apply ();
		return texture;
	}

	//******************Other Stuff*****************************************************************************************************
	public void RequestMapData_Editor (Vector2 centre, Action<MapData_Editor> callback)
	{
		ThreadStart threadStart = delegate {
			MapData_EditorThread (centre, callback);
		};
		new Thread (threadStart).Start ();
	}

	void MapData_EditorThread (Vector2 centre, Action<MapData_Editor> callback)
	{
		MapData_Editor mapData = GenerateMapData_Editor (centre);
		lock (mapDataThreadInfoQueue) {
			mapDataThreadInfoQueue.Enqueue (new MapThreadInfo<MapData_Editor> (callback, mapData));
		}
	}

	MapData_Editor GenerateMapData_Editor (Vector2 centre)
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
		return new MapData_Editor (noiseMap, colourMap);
	}

	void OnValidate ()
	{
		if (lacunarity < 1) {
			lacunarity = 1;
		}
		if (octaves < 0) {
			octaves = 0;
		}
		falloffMap = FalloffGenerator.GenerateFalloffMap (mapChunkSize, falloff);
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
}

[System.Serializable]
public struct TerrainType_Editor
{
	public string name;
	public float height;
	public Color colour;
	public GameObject tile;
}

public struct MapData_Editor
{
	public readonly float[,] heightMap;
	public readonly Color[] colourMap;

	public MapData_Editor (float[,] heightMap, Color[] colourMap)
	{
		this.heightMap = heightMap;
		this.colourMap = colourMap;
	}
}
