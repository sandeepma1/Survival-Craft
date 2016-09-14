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
	sbyte[,] mapTiles;
	int countFileName = 0;

	//Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>> ();

	public void CreateNewSave ()
	{
		ResetAllValues ();
		InitializeFirstVariables ();
		mapChunkSize = 128;
		CreateMaps (128);
		countFileName++;

		mapChunkSize = 64;
		CreateMaps (64);
		countFileName++;

		mapChunkSize = 64;
		CreateMaps (64);
		countFileName = 0;
	}

	void CreateMaps (int size)
	{
		falloffMap = FalloffGenerator.GenerateFalloffMap (size);
		MapData mapData = GenerateMapData (Vector2.zero);
		MapDisplay display = FindObjectOfType<MapDisplay> ();
		SaveTextFile (TextureGenerator.TextureFromColourMap (mapData.colourMap, size, size));
	}

	void SaveTextFile (Texture2D tex) //SaveTexture
	{
		PopulateGameitems (tex);
		TileBeautifier ();
		SaveAllInFiles ();
		LoadMainLevel.m_instance.LoadMainScene_ProceduralGeneration ();
	}

	void TileBeautifier ()
	{
		int mapTilesSize = (int)Mathf.Sqrt (mapTiles.Length);
		for (int x = 0; x < mapTilesSize; x++) {
			for (int y = 0; y < mapTilesSize; y++) {					
				if (mapTiles [x, y] == -1) {					
					if (mapTiles [x - 1, y + 1] == 0) {
						mapTiles [x, y] = 25;
					}
					if (mapTiles [x - 1, y - 1] == 0) {
						mapTiles [x, y] = 26;
					}
					if (mapTiles [x + 1, y - 1] == 0) {
						mapTiles [x, y] = 28;
					}
					if (mapTiles [x + 1, y + 1] == 0) {
						mapTiles [x, y] = 27;
					}		
					if (x + 1 >= mapTilesSize || y + 1 >= mapTilesSize || x - 1 <= 0 || y - 1 <= 0) {						
					} else {		
						bool above = false, below = false, left = false, right = false;			
						if (mapTiles [x, y + 1] == 0) { //above
							above = true;
						}
						if (mapTiles [x + 1, y] == 0) { //right
							right = true;
						}
						if (mapTiles [x - 1, y] == 0) { //left
							left = true;
						}
						if (mapTiles [x, y - 1] == 0) { //below
							below = true;
						}
						if ((sbyte)calculateTileIndex (above, below, left, right) > 0) {
							mapTiles [x, y] = calculateTileIndex (above, below, left, right);
						}
						if (mapTiles [x, y] == -1) {
							mapTiles [x, y] = 16;
						}
					}
				}
			}
		}
	}

	sbyte calculateTileIndex (bool above, bool below, bool left, bool right)
	{
		sbyte sum = 0;
		if (above)
			sum += 1;
		if (left)
			sum += 2;
		if (below)
			sum += 4;
		if (right)
			sum += 8;
		return sum;
	}

	//********************************************************************************************************************************
	void SaveAllInFiles ()
	{
		ES2.Save (mapItems, countFileName + "i.txt");
		ES2.Save (mapTiles, countFileName + "t.txt");
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

	public void PopulateGameitems (Texture2D map)
	{
		mapItems = new string[mapChunkSize, mapChunkSize];
		mapTiles = new sbyte[mapChunkSize, mapChunkSize];
		for (int x = 0; x < map.height; x++) {
			for (int y = 0; y < map.width; y++) {
				if (map.GetPixel (x, y) == regions [0].colour) {//Deep Water
					FillTileInfo (0, x, y);
					FillArrayBlank (x, y);
				} else if (map.GetPixel (x, y) == regions [1].colour) { //Shallow water
					FillTileInfo (-1, x, y);
					FillArrayBlank (x, y);
				} else if (map.GetPixel (x, y) == regions [2].colour) { //Sand
					FillTileInfo (18, x, y);
					Fill2DArray ("10,-1", x, y, 0.05f); //log
				} else if (map.GetPixel (x, y) == regions [3].colour) { //Land
					FillTileInfo (19, x, y);
					Fill2DArray ("5,-1", x, y, 0.05f);
				} else if (map.GetPixel (x, y) == regions [4].colour) { // Trees
					FillTileInfo (20, x, y);
					Fill2DArray ("11,14", x, y, 0.05f);
				} else if (map.GetPixel (x, y) == regions [5].colour) { //Stones
					FillTileInfo (21, x, y);
					Fill2DArray ("2,-1", x, y, 0.25f);
				} else if (map.GetPixel (x, y) == regions [6].colour) { //Hills
					FillTileInfo (22, x, y);
					FillArrayBlank (x, y);
				} else if (map.GetPixel (x, y) == regions [7].colour) {//Big Stones
					FillTileInfo (23, x, y);
					FillArrayBlank (x, y);
				} else {
					FillTileInfo (0, x, y);
					FillArrayBlank (x, y);
				}
			}
		}

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

	void FillTileInfo (sbyte tileid, int x, int y)
	{
		mapTiles [x, y] = tileid;
	}

	void ResetAllValues ()
	{
		PlayerPrefs.DeleteAll ();
		ES2.DeleteDefaultFolder ();
	}

	void InitializeFirstVariables ()
	{
		if (PlayerPrefs.GetInt ("IniPlayerPos") == 0) {

			PlayerPrefs.SetFloat ("PlayerPositionX", 64);
			PlayerPrefs.SetFloat ("PlayerPositionY", 64);
			PlayerPrefs.SetInt ("mapChunkPosition", 0);
			ES2.DeleteDefaultFolder ();
			PlayerPrefs.SetInt ("IniPlayerPos", 1);
		}
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
