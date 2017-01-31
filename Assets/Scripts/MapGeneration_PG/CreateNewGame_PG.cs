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
	public Vector2 offset, falloff;
	public bool useFalloff;
	public bool autoUpdate;
	public TerrainType[] regions;
	public Vector2 islandsGridSize = new Vector2 (1, 3);
	public int islandSizeMin = 64;
	public int islandSizeMax = 128;
	public int chunkSpacing = 300;

	float[,] falloffMap;
	Transform TilesHolder;
	string[,] mapItems;
	sbyte[,] mapTiles;
	int countFileName = 0;
	bool isPlayerPosSET = false;

	List<Vector2> islandsLocations = new List<Vector2> ();

	public void CreateNewSave ()
	{		
		ResetAllValues ();
		InitializeFirstVariables ();
		//for (int i = 0; i < numberOfIslands.Length; i++) {
		//int randomIslandSize = UnityEngine.Random.Range (64, 256);
		CreateRandomGrid ();
		//}
	}

	void CreateMaps (int size)
	{
		falloffMap = FalloffGenerator.GenerateFalloffMap (size, falloff);
		MapData mapData = GenerateMapData (Vector2.zero);
		MapDisplay display = FindObjectOfType<MapDisplay> ();
		SaveTextFile (TextureGenerator.TextureFromColourMap (mapData.colourMap, size, size));
	}

	void CreateRandomGrid ()
	{		
		Vector2[] gridRect = new Vector2[(int)islandsGridSize.x * (int)islandsGridSize.y];
		int ctr = 0;
		for (int i = 0; i < islandsGridSize.x; i++) {
			for (int j = 0; j < islandsGridSize.y; j++) {
				gridRect [ctr] = new Vector2 ((i + 1) * chunkSpacing, (j + 1) * chunkSpacing);
				ctr++;
			}
		}

		for (int i = 0; i < gridRect.Length; i++) {
			Vector2 temp = gridRect [i] + (UnityEngine.Random.insideUnitCircle * (chunkSpacing / 3));
			islandsLocations.Add (new Vector2 (Mathf.Round (temp.x), Mathf.Round (temp.y)));
			int randomIslandSize = UnityEngine.Random.Range (islandSizeMin, islandSizeMax);
			if (i == 0) { // first island is always whats inside this loop
				mapChunkSize = islandSizeMax;
				CreateMaps (islandSizeMax);
			} else {
				mapChunkSize = randomIslandSize;
				CreateMaps (randomIslandSize);
			}
			countFileName++;
		}

		ES2.Save (islandsLocations, "islandLocations");

	}

	void SaveTextFile (Texture2D tex) //SaveTexture
	{		
		ES2.SaveImage (tex, "Map.png");
		PopulateGameitems (tex);
		//TileBeautifier ();
		SaveAllInFiles ();
		//LoadMainLevel.m_instance.LoadMainScene_ProceduralGeneration ();  //Load level afer calculations
		LoadMainLevel.m_instance.LoadMainScene_ProceduralGeneration ();// Load PG in Portrait
	}


	void TileBeautifier ()
	{
		int mapTilesSize = (int)Mathf.Sqrt (mapTiles.Length);
		for (int x = 0; x < mapTilesSize; x++) {
			for (int y = 0; y < mapTilesSize; y++) {					
				if (mapTiles [x, y] == -1) {					
					if (x + 1 >= mapTilesSize || y + 1 >= mapTilesSize || x - 1 <= 0 || y - 1 <= 0) {						
					} else {
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

	public void PopulateGameitems (Texture2D map)
	{
		map.filterMode = FilterMode.Point;
		mapItems = new string[mapChunkSize, mapChunkSize];
		mapTiles = new sbyte[mapChunkSize, mapChunkSize];
		for (int x = 0; x < map.height; x++) {
			for (int y = 0; y < map.width; y++) {
				if (map.GetPixel (x, y) == regions [0].colour) {//Deep Water
					FillTileInfo (0, x, y);
					FillArrayBlank (x, y);
				} else if (map.GetPixel (x, y) == regions [1].colour) {//Water
					FillTileInfo (1, x, y);
					FillArrayBlank (x, y);
				} else if (map.GetPixel (x, y) == regions [2].colour) { //Sand
					FillTileInfo (2, x, y);
					FillItemInfo ("23,-1", x, y, 0.01f); //star fish
				} else if (map.GetPixel (x, y) == regions [3].colour) { //Land
					FillTileInfo (3, x, y);
					FillLandTilesWithItems (x, y);
				} else if (map.GetPixel (x, y) == regions [4].colour) { //Stones
					FillTileInfo (1, x, y);
					FillArrayBlank (x, y);
				} else {
					FillTileInfo (0, x, y);
					FillArrayBlank (x, y);
				}
			}
		}
	}

	void FillLandTilesWithItems (int x, int y)
	{
		int ran = UnityEngine.Random.Range (0, 8);
		switch (ran) {
			case 0:
				FillItemInfo ("5,-1", x, y, 0.7f); //grass
				break;
			case 1:
				FillItemInfo ("1,-1", x, y, 0.09f); // flint
				break;
			case 2:
				FillItemInfo ("6,-1", x, y, 0.25f); //grass1
				if (!isPlayerPosSET) { // Player Spwan Position first time
					Bronz.LocalStore.Instance.SetFloat ("PlayerPositionX", x + islandsLocations [0].x);
					Bronz.LocalStore.Instance.SetFloat ("PlayerPositionY", y + islandsLocations [0].y);
					isPlayerPosSET = true;
				}
				break;
			case 3:
				FillItemInfo ("9,-1", x, y, 0.9f); //Stick
				break;
			case 4:
				int ranTree = UnityEngine.Random.Range (0, 4);
				FillItemInfo (ranTree + 12 + ",1", x, y, 3f); //trees
				break;
			case 5:
				FillItemInfo ("17,8", x, y, 0.1f); //berrybush
				break;
			case 6:
				FillItemInfo ("22,5", x, y, 0.1f); //radishPlant
				break;
			case 7:
				FillItemInfo ("2,-1", x, y, 5f); //rocks
				break;
		/*case 8:
				//FillItemInfo ("28,-1", x, y, 2f); //grass
				break;*/
			default:
				break;
		}
	}

	void FillItemInfo (string itemName, int x, int y, float probability)
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
		Bronz.LocalStore.Instance.DeleteAll ();
		ES2.DeleteDefaultFolder ();
	}

	void InitializeFirstVariables ()
	{
		Bronz.LocalStore.Instance.DeleteAll ();
		if (Bronz.LocalStore.Instance.GetInt ("InitGamePrefs") == 0) {			
			Bronz.LocalStore.Instance.SetFloat ("PlayerHunger", 100);
			Bronz.LocalStore.Instance.SetFloat ("PlayerHealth", 100);
			Bronz.LocalStore.Instance.SetInt ("mapChunkPosition", 0);
			Bronz.LocalStore.Instance.SetInt ("gameTime", 0);
			Bronz.LocalStore.Instance.SetInt ("gameDay", 1);
			Bronz.LocalStore.Instance.SetInt ("currentPhase", 0);
			Bronz.LocalStore.Instance.SetInt ("TouchControls", 0);
			Bronz.LocalStore.Instance.SetFloat ("sunRotationZ", 0);
			Bronz.LocalStore.Instance.SetFloat ("moonRotationZ", 0);
			Bronz.LocalStore.Instance.SetInt ("backgroundPositionX", 0);	

			ES2.DeleteDefaultFolder ();
			Bronz.LocalStore.Instance.SetInt ("InitGamePrefs", 1);
		}
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
