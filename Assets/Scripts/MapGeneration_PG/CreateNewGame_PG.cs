using Bronz.Ui;
using System.Collections.Generic;
using UnityEngine;

public partial class CreateNewGame_PG : MonoBehaviour
{
    [SerializeField] private Noise.NormalizeMode normalizeMode;
    [SerializeField] private int mapChunkSize = 128;
    [SerializeField] private float noiseScale;
    [SerializeField] private int octaves;
    [Range(0, 1)] [SerializeField] private float persistance;
    [SerializeField] private float lacunarity;
    [SerializeField] private int seed;
    [SerializeField] private Vector2 offset, falloff;
    [SerializeField] private bool useFalloff;
    [SerializeField] private bool autoUpdate;
    [SerializeField] private TerrainType[] regions;
    [SerializeField] private Vector2 islandsGridSize = new Vector2(1, 3);
    [SerializeField] private int islandSizeMin = 64;
    [SerializeField] private int islandSizeMax = 128;
    [SerializeField] private int chunkSpacing = 300;

    private float[,] falloffMap;
    private Transform TilesHolder;
    private string[,] mapItems;
    private sbyte[,] mapTiles;
    private int countFileName = 0;
    private bool isPlayerPosSET = false;
    private List<Vector2> islandsLocations = new List<Vector2>();

    private void Start()
    {
        UiMainMenuCanvas.OnNewGameButtonClick += CreateNewSave;
    }

    private void CreateNewSave()
    {
        ResetAllValues();
        InitializeFirstVariables();
        CreateRandomGrid();
    }

    private void CreateMaps(int size)
    {
        falloffMap = FalloffGenerator.GenerateFalloffMap(size, falloff);
        MapData mapData = GenerateMapData(Vector2.zero);
        MapDisplay display = FindObjectOfType<MapDisplay>();
        SaveTextFile(TextureGenerator.TextureFromColourMap(mapData.colourMap, size, size));
    }

    private void CreateRandomGrid()
    {
        Vector2[] gridRect = new Vector2[(int)islandsGridSize.x * (int)islandsGridSize.y];
        int ctr = 0;
        for (int i = 0; i < islandsGridSize.x; i++)
        {
            for (int j = 0; j < islandsGridSize.y; j++)
            {
                gridRect[ctr] = new Vector2((i + 1) * chunkSpacing, (j + 1) * chunkSpacing);
                ctr++;
            }
        }

        for (int i = 0; i < gridRect.Length; i++)
        {
            Vector2 temp = gridRect[i] + (UnityEngine.Random.insideUnitCircle * (chunkSpacing / 3));
            islandsLocations.Add(new Vector2(Mathf.Round(temp.x), Mathf.Round(temp.y)));
            int randomIslandSize = UnityEngine.Random.Range(islandSizeMin, islandSizeMax);
            if (i == 0)
            { // first island is always whats inside this loop
                mapChunkSize = islandSizeMax;
                CreateMaps(islandSizeMax);
            }
            else
            {
                mapChunkSize = randomIslandSize;
                CreateMaps(randomIslandSize);
            }
            countFileName++;
        }
        ES2.Save(islandsLocations, "islandLocations");
    }

    private void SaveTextFile(Texture2D tex) //SaveTexture
    {
        ES2.SaveImage(tex, "Map.png");
        PopulateGameitems(tex);
        //TileBeautifier ();
        SaveAllInFiles();
        //LoadMainLevel.m_instance.LoadMainScene_ProceduralGeneration ();  //Load level afer calculations
        UiMainMenuCanvas.OnLoadMainScene?.Invoke();
    }

    private void TileBeautifier()
    {
        int mapTilesSize = (int)Mathf.Sqrt(mapTiles.Length);
        for (int x = 0; x < mapTilesSize; x++)
        {
            for (int y = 0; y < mapTilesSize; y++)
            {
                if (mapTiles[x, y] == -1)
                {
                    if (x + 1 >= mapTilesSize || y + 1 >= mapTilesSize || x - 1 <= 0 || y - 1 <= 0)
                    {
                    }
                    else
                    {
                        if (mapTiles[x - 1, y + 1] == 0)
                        {
                            mapTiles[x, y] = 25;
                        }
                        if (mapTiles[x - 1, y - 1] == 0)
                        {
                            mapTiles[x, y] = 26;
                        }
                        if (mapTiles[x + 1, y - 1] == 0)
                        {
                            mapTiles[x, y] = 28;
                        }
                        if (mapTiles[x + 1, y + 1] == 0)
                        {
                            mapTiles[x, y] = 27;
                        }
                    }
                    if (x + 1 >= mapTilesSize || y + 1 >= mapTilesSize || x - 1 <= 0 || y - 1 <= 0)
                    {
                    }
                    else
                    {
                        bool above = false, below = false, left = false, right = false;
                        if (mapTiles[x, y + 1] == 0)
                        { //above
                            above = true;
                        }
                        if (mapTiles[x + 1, y] == 0)
                        { //right
                            right = true;
                        }
                        if (mapTiles[x - 1, y] == 0)
                        { //left
                            left = true;
                        }
                        if (mapTiles[x, y - 1] == 0)
                        { //below
                            below = true;
                        }
                        if (CalculateTileIndex(above, below, left, right) > 0)
                        {
                            mapTiles[x, y] = CalculateTileIndex(above, below, left, right);
                        }
                        if (mapTiles[x, y] == -1)
                        {
                            mapTiles[x, y] = 16;
                        }
                    }
                }
            }
        }
    }

    //********************************************************************************************************************************
    private void SaveAllInFiles()
    {
        ES2.Save(mapItems, countFileName + "i.txt");
        ES2.Save(mapTiles, countFileName + "t.txt");
    }

    private MapData GenerateMapData(Vector2 centre)
    {
        seed = (int)System.DateTime.Now.Ticks;
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize + 2, mapChunkSize + 2, seed, noiseScale, octaves, persistance, lacunarity, centre + offset, normalizeMode);

        Color[] colourMap = new Color[mapChunkSize * mapChunkSize];
        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                if (useFalloff)
                {
                    noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
                }
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight >= regions[i].height)
                    {
                        colourMap[y * mapChunkSize + x] = regions[i].colour;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        return new MapData(noiseMap, colourMap);
    }

    public void PopulateGameitems(Texture2D map)
    {
        map.filterMode = FilterMode.Point;
        mapItems = new string[mapChunkSize, mapChunkSize];
        mapTiles = new sbyte[mapChunkSize, mapChunkSize];
        for (int x = 0; x < map.height; x++)
        {
            for (int y = 0; y < map.width; y++)
            {
                if (map.GetPixel(x, y) == regions[0].colour)
                {//Deep Water
                    FillTileInfo(0, x, y);
                    FillArrayBlank(x, y);
                }
                else if (map.GetPixel(x, y) == regions[1].colour)
                {//Water
                    FillTileInfo(1, x, y);
                    FillArrayBlank(x, y);
                }
                else if (map.GetPixel(x, y) == regions[2].colour)
                { //Sand
                    FillTileInfo(2, x, y);
                    FillItemInfo("14,-1", x, y, 0.01f); //star fish
                }
                else if (map.GetPixel(x, y) == regions[3].colour)
                { //Land
                    FillTileInfo(3, x, y);
                    FillLandTilesWithItems(x, y);
                }
                else if (map.GetPixel(x, y) == regions[4].colour)
                { //Stones
                    FillTileInfo(4, x, y);
                    FillArrayBlank(x, y);
                }
                else
                {
                    FillTileInfo(0, x, y);
                    FillArrayBlank(x, y);
                }
            }
        }
    }

    private void FillLandTilesWithItems(int x, int y)
    {
        int ran = UnityEngine.Random.Range(0, ItemDatabase.m_instance.items.Count);
        if (ran == 0)
        {
        }
        if (ItemDatabase.m_instance.items[ran].SpawnsOnTerrian == 3)
        {
            FillItemInfo(ran + ",-1", x, y, ItemDatabase.m_instance.items[ran].SpawnProbability);
        }
        else
        {
            FillArrayBlank(x, y);
        }
    }

    private void FillItemInfo(string itemName, int x, int y, float probability)
    {
        if (UnityEngine.Random.value <= probability)
        {
            mapItems[x, y] = itemName;
        }
        else
        {
            mapItems[x, y] = "";
        }
    }

    private void FillArrayBlank(int x, int y)
    {
        mapItems[x, y] = "";
    }

    private void FillTileInfo(sbyte tileid, int x, int y)
    {
        mapTiles[x, y] = tileid;
    }

    private void ResetAllValues()
    {
        PlayerPrefs.DeleteAll();
        ES2.DeleteDefaultFolder();
    }

    private void InitializeFirstVariables()
    {
        PlayerPrefs.DeleteAll();
        if (PlayerPrefs.GetInt("InitGamePrefs") == 0)
        {
            PlayerPrefs.SetFloat("PlayerHunger", 100);
            PlayerPrefs.SetFloat("PlayerHealth", 100);
            PlayerPrefs.SetInt("mapChunkPosition", 0);
            PlayerPrefs.SetInt("gameTime", 0);
            PlayerPrefs.SetInt("gameDay", 1);
            PlayerPrefs.SetInt("currentPhase", 0);
            PlayerPrefs.SetInt("TouchControls", 0);
            PlayerPrefs.SetFloat("sunRotationZ", 0);
            PlayerPrefs.SetFloat("moonRotationZ", 0);
            PlayerPrefs.SetInt("backgroundPositionX", 0);
            PlayerPrefs.SetFloat("PlayerPositionX_", 32);
            PlayerPrefs.SetFloat("PlayerPositionY_", 32);

            ES2.DeleteDefaultFolder();
            PlayerPrefs.SetInt("InitGamePrefs", 1);
        }
    }

    private void OnValidate()
    {
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }
        falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize, falloff);
    }

    private sbyte CalculateTileIndex(bool above, bool below, bool left, bool right)
    {
        sbyte sum = 0;
        if (above)
        {
            sum += 1;
        }

        if (left)
        {
            sum += 2;
        }

        if (below)
        {
            sum += 4;
        }

        if (right)
        {
            sum += 8;
        }

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

    public MapData(float[,] heightMap, Color[] colourMap)
    {
        this.heightMap = heightMap;
        this.colourMap = colourMap;
    }
}
