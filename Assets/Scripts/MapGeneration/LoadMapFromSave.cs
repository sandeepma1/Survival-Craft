using UnityEngine;
using System.Collections;

public class LoadMapFromSave : MonoBehaviour
{
	public static LoadMapFromSave m_instance = null;
	Transform tilesHolder, mapItemHolder;
	public MapGenerator mapGenerator;
	public GameObject stone, log, grass, tree;
	Texture2D mapData;
	string[,] mapItems;
	GameObject[,] mapItemGO;
	int mapSize = 0;

	void Awake ()
	{
		m_instance = this;
	}

	void Start ()
	{
		mapData = LoadTextureFromFile ((Texture2D)ES2.LoadImage ("SaveSlot1.png"));
		mapItems = ES2.Load2DArray<string> ("mapItems.txt");
		mapSize = (int)Mathf.Sqrt (mapItems.Length);
		mapItemGO = new GameObject[mapSize, mapSize];
		GenerateMap (mapData);
	}

	void GenerateMap (Texture2D tex)
	{		
		tilesHolder = new GameObject ("TilesHolder").transform;
		mapItemHolder = new GameObject ("MapItemHolder").transform;

		tilesHolder.transform.position = new Vector3 (mapGenerator.mapChunkSize / 2, mapGenerator.mapChunkSize / 2);
		mapItemHolder.transform.position = new Vector3 (mapGenerator.mapChunkSize / 2, mapGenerator.mapChunkSize / 2);
		for (int i = 0; i < tex.width; i++) {
			for (int j = 0; j < tex.height; j++) {				
				if (tex.GetPixel (i, j) == mapGenerator.regions [0].colour) {
					InstansiateTiles (mapGenerator.regions [0].tile, i, j);
				} else if (tex.GetPixel (i, j) == mapGenerator.regions [1].colour) {					
					InstansiateTiles (mapGenerator.regions [1].tile, i, j);
				} else if (tex.GetPixel (i, j) == mapGenerator.regions [2].colour) {
					InstansiateTiles (mapGenerator.regions [2].tile, i, j);
				} else if (tex.GetPixel (i, j) == mapGenerator.regions [3].colour) {
					InstansiateTiles (mapGenerator.regions [3].tile, i, j);				
				} else if (tex.GetPixel (i, j) == mapGenerator.regions [4].colour) {
					InstansiateTiles (mapGenerator.regions [4].tile, i, j);
				} else if (tex.GetPixel (i, j) == mapGenerator.regions [5].colour) {
					InstansiateTiles (mapGenerator.regions [5].tile, i, j);
				} else if (tex.GetPixel (i, j) == mapGenerator.regions [6].colour) {
					InstansiateTiles (mapGenerator.regions [6].tile, i, j);
				} else if (tex.GetPixel (i, j) == mapGenerator.regions [7].colour) {
					InstansiateTiles (mapGenerator.regions [7].tile, i, j);
				}
			}
		}
		LoadMapItems ();
	}

	void LoadMapItems ()
	{			
		for (int x = 0; x < mapSize; x++) {
			for (int y = 0; y < mapSize; y++) {	
				switch (mapItems [x, y]) {
					case "stone":
						InstansiateMapItem (stone, x, y);
						break;
					case "grass":
						InstansiateMapItem (grass, x, y);
						break;
					case "log":
						InstansiateMapItem (log, x, y);
						break;	
					case "treePrefab":
						InstansiateMapItem (log, x, y);
						break;	
					default:
						break;
				}
			}
		}
		//tilesHolder.transform.position = Vector3.zero;
		//mapItemHolder.transform.position = Vector3.zero;
	}

	void InstansiateTiles (GameObject tile, int x, int y)
	{
		GameObject inst = Instantiate (tile, new Vector3 (x, y, 0), Quaternion.identity) as GameObject;
		inst.transform.SetParent (tilesHolder);
	}

	void InstansiateMapItem (GameObject item, int x, int y)
	{
		mapItemGO [x, y] = Instantiate (item, new Vector3 (x, y, 0), Quaternion.identity) as GameObject;
		mapItemGO [x, y].transform.SetParent (mapItemHolder);
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

	public GameObject GetTile (Vector2 pos)
	{	
		if (mapItemGO [(int)pos.x, (int)pos.y] != null) {
//			print (mapItemGO [(int)pos.x, (int)pos.y].name);
			return mapItemGO [(int)pos.x, (int)pos.y];
		}
		return null;
	}

	public void SaveMapitemData (int x, int y)
	{
		//mapItems [x, y] = "";
		//ES2.Save (mapItems, "mapItems.txt");
	}
}
