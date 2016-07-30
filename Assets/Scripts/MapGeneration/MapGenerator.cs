using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Collections.Generic;
using System.IO;

public class MapGenerator : MonoBehaviour
{

	public enum DrawMode
	{
		NoiseMap,
		ColourMap,
		FalloffMap}

	;

	public DrawMode drawMode;

	public Noise.NormalizeMode normalizeMode;

	public int mapChunkSize = 250;
	public float noiseScale;

	public int octaves;
	[Range (0, 1)]
	public float persistance;
	public float lacunarity;

	public int seed;
	public Vector2 offset;

	public bool useFalloff;

	/*	public float meshHeightMultiplier;
	public AnimationCurve meshHeightCurve;*/

	public bool autoUpdate;

	public TerrainType[] regions;

	float[,] falloffMap;

	Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>> ();
	//Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>> ();

	void Start ()
	{
		falloffMap = FalloffGenerator.GenerateFalloffMap (mapChunkSize);
		DrawMapInEditor ();
	}

	public void DrawMapInEditor ()
	{
		MapData mapData = GenerateMapData (Vector2.zero);

		MapDisplay display = FindObjectOfType<MapDisplay> ();
		if (drawMode == DrawMode.NoiseMap) {
			display.DrawTexture (TextureGenerator.TextureFromHeightMap (mapData.heightMap));
		} else if (drawMode == DrawMode.ColourMap) {
			display.DrawTexture (TextureGenerator.TextureFromColourMap (mapData.colourMap, mapChunkSize, mapChunkSize));
			//EncodeToPng (TextureGenerator.TextureFromColourMap (mapData.colourMap, mapChunkSize, mapChunkSize));
			//SaveTextureToFile (TextureGenerator.TextureFromColourMap (mapData.colourMap, mapChunkSize, mapChunkSize), "aa");
		} else if (drawMode == DrawMode.FalloffMap) {
			display.DrawTexture (TextureGenerator.TextureFromHeightMap (FalloffGenerator.GenerateFalloffMap (mapChunkSize)));
		}
	}

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
}

[System.Serializable]
public struct TerrainType
{
	public string name;
	public float height;
	public Color colour;
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
