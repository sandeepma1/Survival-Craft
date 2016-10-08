using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadMapFromSave_PG : MonoBehaviour
{
	public static LoadMapFromSave_PG m_instance = null;
	public GameObject[] items;
	public GameObject[] tiles;
	public GameObject[] ranTiles;
	public GameObject[] mapChunks;
	string[][,] mapItemsFromSave;
	sbyte[][,] mapTilesFromSave;
	item[][,] mapItemGO;

	int mapSize = 0;
	int[] chunkMapSize;
	string spriteName = "";

	item[] playerSurroundings = new item[8];


	void Start ()
	{
		PlayerPrefs.SetInt ("mapChunkPosition", 0);
		m_instance = this;
		SetSortingNamesForItems ();
		LoadMapChunks ();
		LoadMapData ();
		SpwanObjects ();
		DisableUnusedMapChunks ();
	}

	void SetSortingNamesForItems ()
	{
		for (int i = 0; i < items.Length; i++) {								
			foreach (Transform child in items [i].transform) {
				if (child.tag == "ItemName") {
					child.GetComponent <MeshRenderer> ().sortingLayerName = "Names";
				}
			}
		}
	}

	public item[] GetPlayerSurroundingTilesInfo_Item (Vector3 playerPos)
	{
		playerPos = GetLocalIslandPosition (playerPos);

		playerSurroundings [0] = mapItemGO [0] [(int)playerPos.x, (int)playerPos.y + 1]; //above
		playerSurroundings [1] = mapItemGO [0] [(int)playerPos.x + 1, (int)playerPos.y];
		playerSurroundings [2] = mapItemGO [0] [(int)playerPos.x - 1, (int)playerPos.y];
		playerSurroundings [3] = mapItemGO [0] [(int)playerPos.x, (int)playerPos.y - 1];
		playerSurroundings [4] = mapItemGO [0] [(int)playerPos.x + 1, (int)playerPos.y + 1];
		playerSurroundings [5] = mapItemGO [0] [(int)playerPos.x - 1, (int)playerPos.y - 1];
		playerSurroundings [6] = mapItemGO [0] [(int)playerPos.x + 1, (int)playerPos.y - 1];
		playerSurroundings [7] = mapItemGO [0] [(int)playerPos.x - 1, (int)playerPos.y - 1];
		return  playerSurroundings;// [0] [(int)playerPos.x + 1, (int)playerPos.y];
	}

	public GameObject[] GetPlayerSurroundingTilesInfo_GO (Vector3 playerPos)
	{
		playerPos = GetLocalIslandPosition (playerPos);
		GameObject[] playerSurroundings = new GameObject[8];

		playerSurroundings [0] = mapItemGO [0] [(int)playerPos.x, (int)playerPos.y + 1].GO; //above
		playerSurroundings [1] = mapItemGO [0] [(int)playerPos.x + 1, (int)playerPos.y].GO;
		playerSurroundings [2] = mapItemGO [0] [(int)playerPos.x - 1, (int)playerPos.y].GO;
		playerSurroundings [3] = mapItemGO [0] [(int)playerPos.x, (int)playerPos.y - 1].GO;
		playerSurroundings [4] = mapItemGO [0] [(int)playerPos.x + 1, (int)playerPos.y + 1].GO;
		playerSurroundings [5] = mapItemGO [0] [(int)playerPos.x - 1, (int)playerPos.y - 1].GO;
		playerSurroundings [6] = mapItemGO [0] [(int)playerPos.x + 1, (int)playerPos.y - 1].GO;
		playerSurroundings [7] = mapItemGO [0] [(int)playerPos.x - 1, (int)playerPos.y - 1].GO;
		return  playerSurroundings;// [0] [(int)playerPos.x + 1, (int)playerPos.y];
	}

	void LoadMapChunks ()
	{	
		List<Vector2> islandLocations = new List<Vector2> ();
		islandLocations = ES2.LoadList<Vector2> ("islandLocations");

		mapTilesFromSave = new sbyte[islandLocations.Count][,]; //**
		mapChunks = new GameObject[islandLocations.Count];
		chunkMapSize = new int[islandLocations.Count];
		for (int i = 0; i < islandLocations.Count; i++) {	//**
			mapTilesFromSave [i] = ES2.Load2DArray<sbyte> (i + "t.txt");
			mapChunks [i] = new GameObject (i.ToString ()).gameObject;
			mapChunks [i].transform.position = islandLocations [i];
			chunkMapSize [i] = (int)Mathf.Sqrt (mapTilesFromSave [i].Length);
		}

		for (int i = 0; i < mapTilesFromSave.Length; i++) {	 // load map tiles		
			for (int x = 0; x < chunkMapSize [i]; x++) {
				for (int y = 0; y < chunkMapSize [i]; y++) {
					GameObject go;
					if (mapTilesFromSave [i] [x, y] == 16) {
						go = Instantiate (ranTiles [Random.Range (0, ranTiles.Length)]);
					} else {
						go = Instantiate (tiles [mapTilesFromSave [i] [x, y]]);
					}				
					go.transform.SetParent (mapChunks [i].transform);
					go.name = mapTilesFromSave [i] [x, y].ToString ();

					go.transform.localPosition = new Vector3 (x, y);
				}
			}
		}
	}

	public void LoadMapData ()
	{
		mapItemsFromSave = new string[mapChunks.Length][,];
		mapItemGO = new item[mapChunks.Length][,];
		for (int i = 0; i < mapChunks.Length; i++) {
			mapItemsFromSave [i] = ES2.Load2DArray<string> (mapChunks [i].name + "i.txt");
		}
		mapSize = (int)Mathf.Sqrt (mapItemsFromSave [0].Length);

		for (int i = 0; i < mapChunks.Length; i++) {
			mapItemGO [i] = new item[mapSize, mapSize];
		}

		for (int i = 0; i < mapChunks.Length; i++) {
			for (int x = 0; x < chunkMapSize [i]; x++) {
				for (int y = 0; y < chunkMapSize [i]; y++) {
					if (mapItemsFromSave [i] [x, y].Length > 2) {
						mapItemsFromSave [i] [x, y] = mapItemsFromSave [i] [x, y].TrimEnd (new char[] { '\r', '\n' });
						string[] itemss = mapItemsFromSave [i] [x, y].Split (',');
						mapItemGO [i] [x, y].id = sbyte.Parse (itemss [0]);
						mapItemGO [i] [x, y].age = sbyte.Parse (itemss [1]);
					}
				}
			}
		}
	}

	public void RepaintMapItems ()
	{               // Next day repaint all game items like trees bushes etc
		for (int i = 0; i < mapChunks.Length; i++) {
			for (int x = 0; x < chunkMapSize [i]; x++) {
				for (int y = 0; y < chunkMapSize [i]; y++) {
					if (mapItemsFromSave [i] [x, y].Length > 2) {
						switch (mapItemGO [i] [x, y].id) {
							case 12://trees
							case 13://trees
							case 14://trees
							case 16://berries
							case 22://carrots
								if (mapItemGO [i] [x, y].age != ItemDatabase.m_instance.items [mapItemGO [i] [x, y].id].maxAge) {
									mapItemGO [i] [x, y].age = (sbyte)(mapItemGO [i] [x, y].age + 1);
								} else {
									mapItemGO [i] [x, y].id = (sbyte)ItemDatabase.m_instance.items [mapItemGO [i] [x, y].id].nextStage;
									mapItemGO [i] [x, y].age = 1;
								}
								mapItemsFromSave [i] [x, y] = mapItemGO [i] [x, y].id + "," + mapItemGO [i] [x, y].age;
								break;
							default:
								break;
						}
					}
				}
			}
		}
		ES2.Save (mapItemsFromSave [PlayerPrefs.GetInt ("mapChunkPosition")], mapChunks [PlayerPrefs.GetInt ("mapChunkPosition")].name + "i.txt");
		SpwanObjects ();
	}

	public void SpwanObjects ()
	{
		for (int i = 0; i < mapChunks.Length; i++) {
			for (int x = 0; x < mapSize; x++) {
				for (int y = 0; y < mapSize; y++) {
					if (mapItemGO [i] [x, y].id > 0) {
						InstantiateObject (items [mapItemGO [i] [x, y].id], new Vector3 (x, y, 0), mapChunks [i].transform, i, mapItemGO [i] [x, y].id, mapItemGO [i] [x, y].age);	
					}				
				}
			}
		}
	}

	public void InstantiateObject (GameObject go, Vector3 pos, Transform parent, int i, int id, int age)
	{
		if (mapItemGO [i] [(int)pos.x, (int)pos.y].GO != null) {
			Destroy (mapItemGO [i] [(int)pos.x, (int)pos.y].GO);
		}
		mapItemGO [i] [(int)pos.x, (int)pos.y].GO = Instantiate (go);
		mapItemGO [i] [(int)pos.x, (int)pos.y].GO.name = id + "," + age;
		mapItemGO [i] [(int)pos.x, (int)pos.y].GO.transform.parent = parent;
		mapItemGO [i] [(int)pos.x, (int)pos.y].GO.transform.localPosition = new Vector3 (pos.x, pos.y, 0);

		switch (mapItemGO [i] [(int)pos.x, (int)pos.y].id) {
		/*	case 11: // Trees
				spriteName = mapItemGO [i] [(int)pos.x, (int)pos.y].GO.transform.GetChild (1).GetComponent<SpriteRenderer> ().sprite.name;
				if (age < ItemDatabase.m_instance.items [mapItemGO [i] [(int)pos.x, (int)pos.y].id].maxAge) {
					mapItemGO [i] [(int)pos.x, (int)pos.y].GO.transform.GetChild (0).gameObject.SetActive (false);
					mapItemGO [i] [(int)pos.x, (int)pos.y].GO.transform.GetChild (1).gameObject.GetComponent<SpriteRenderer> ().sprite = Resources.LoadAll<Sprite> ("Textures/Map/Items/Trees/" + spriteName) [age];
				}
				if (age == ItemDatabase.m_instance.items [mapItemGO [i] [(int)pos.x, (int)pos.y].id].maxAge) {
					mapItemGO [i] [(int)pos.x, (int)pos.y].GO.transform.GetChild (0).gameObject.SetActive (true);
					mapItemGO [i] [(int)pos.x, (int)pos.y].GO.transform.GetChild (1).gameObject.GetComponent<SpriteRenderer> ().sprite = Resources.LoadAll<Sprite> ("Textures/Map/Items/Trees/" + spriteName) [0];
				}
				break;*/
			case 16:  //Berries
				spriteName = mapItemGO [i] [(int)pos.x, (int)pos.y].GO.GetComponent<SpriteRenderer> ().sprite.name;
				mapItemGO [i] [(int)pos.x, (int)pos.y].GO.gameObject.GetComponent<SpriteRenderer> ().sprite = Resources.LoadAll<Sprite> ("Textures/Map/Items/Bushes/" + spriteName) [age];
				break;
			case 21:  //Carrots
				spriteName = mapItemGO [i] [(int)pos.x, (int)pos.y].GO.GetComponent<SpriteRenderer> ().sprite.name;
				mapItemGO [i] [(int)pos.x, (int)pos.y].GO.gameObject.GetComponent<SpriteRenderer> ().sprite = Resources.LoadAll<Sprite> ("Textures/Map/Items/Bushes/" + spriteName) [age];
				break;
			default:
				break;
		}
	}

	public void InstantiatePlacedObject (GameObject go, Vector3 pos, Transform parent, int islandIndex, int itemID, int itemAge)
	{
		pos = GetLocalIslandPosition (pos);
		if (itemID > 0 && mapItemGO [islandIndex] [(int)pos.x, (int)pos.y].GO == null) {
			mapItemGO [islandIndex] [(int)pos.x, (int)pos.y].GO = Instantiate (go);
			mapItemGO [islandIndex] [(int)pos.x, (int)pos.y].GO.name = itemID + "," + itemAge;
			mapItemGO [islandIndex] [(int)pos.x, (int)pos.y].GO.transform.parent = parent;
			mapItemGO [islandIndex] [(int)pos.x, (int)pos.y].GO.transform.localPosition = new Vector3 (pos.x, pos.y, 0);
			mapItemGO [islandIndex] [(int)pos.x, (int)pos.y].id = (sbyte)itemID;
			mapItemGO [islandIndex] [(int)pos.x, (int)pos.y].age = (sbyte)itemAge;

			switch (mapItemGO [islandIndex] [(int)pos.x, (int)pos.y].id) {
				case 11: // Trees
					spriteName = mapItemGO [islandIndex] [(int)pos.x, (int)pos.y].GO.transform.GetChild (1).GetComponent<SpriteRenderer> ().sprite.name;
					if (itemAge < ItemDatabase.m_instance.items [mapItemGO [islandIndex] [(int)pos.x, (int)pos.y].id].maxAge) {
						mapItemGO [islandIndex] [(int)pos.x, (int)pos.y].GO.transform.GetChild (0).gameObject.SetActive (false);
						mapItemGO [islandIndex] [(int)pos.x, (int)pos.y].GO.transform.GetChild (1).gameObject.GetComponent<SpriteRenderer> ().sprite = Resources.LoadAll<Sprite> ("Textures/Map/Items/Trees/" + spriteName) [itemAge];
					}
					if (itemAge == ItemDatabase.m_instance.items [mapItemGO [islandIndex] [(int)pos.x, (int)pos.y].id].maxAge) {
						mapItemGO [islandIndex] [(int)pos.x, (int)pos.y].GO.transform.GetChild (0).gameObject.SetActive (true);
						mapItemGO [islandIndex] [(int)pos.x, (int)pos.y].GO.transform.GetChild (1).gameObject.GetComponent<SpriteRenderer> ().sprite = Resources.LoadAll<Sprite> ("Textures/Map/Items/Trees/" + spriteName) [0];
					}
					break;
				case 16:  //Berries
					spriteName = mapItemGO [islandIndex] [(int)pos.x, (int)pos.y].GO.GetComponent<SpriteRenderer> ().sprite.name;
					mapItemGO [islandIndex] [(int)pos.x, (int)pos.y].GO.gameObject.GetComponent<SpriteRenderer> ().sprite = Resources.LoadAll<Sprite> ("Textures/Map/Items/Bushes/" + spriteName) [itemAge];
					break;
				case 21:  //Carrots
					spriteName = mapItemGO [islandIndex] [(int)pos.x, (int)pos.y].GO.GetComponent<SpriteRenderer> ().sprite.name;
					mapItemGO [islandIndex] [(int)pos.x, (int)pos.y].GO.gameObject.GetComponent<SpriteRenderer> ().sprite = Resources.LoadAll<Sprite> ("Textures/Map/Items/Bushes/" + spriteName) [itemAge];
					break;
				default:
					break;
			}

			//ActionManager.m_AC_instance.currentWeildedItem.ID
			if (Devdog.InventorySystem.InventoryManager.FindAll (ActionManager.m_AC_instance.currentWeildedItem.ID, false).Count > 0) {
				Devdog.InventorySystem.InventoryManager.RemoveItem (ActionManager.m_AC_instance.currentWeildedItem.ID, 1, false);
			}

			mapItemsFromSave [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y] = itemID + "," + itemAge;
			ES2.Save (mapItemsFromSave [PlayerPrefs.GetInt ("mapChunkPosition")], mapChunks [PlayerPrefs.GetInt ("mapChunkPosition")].name + "i.txt");
		}
	}

	public void DisableUnusedMapChunks ()
	{
		int currentMapChunkPosition = PlayerPrefs.GetInt ("mapChunkPosition");
		for (int i = 0; i < mapChunks.Length; i++) {
			mapChunks [i].SetActive (true);
		}
		for (int i = 0; i < mapChunks.Length; i++) {
			if (i == currentMapChunkPosition) {

			} else {
				mapChunks [i].SetActive (false);
			}
		}
	}

	public item GetTile (Vector2 pos)
	{
		pos = GetLocalIslandPosition (pos);
		//print (pos);
		if (mapItemGO [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y].id >= 0) {
			return mapItemGO [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y];
		}
		return new item ();
	}

	public void SaveMapItemData (sbyte id, sbyte age, Vector2 pos, onHarvest harvestType)
	{
		pos = GetLocalIslandPosition (pos);
		switch (harvestType) {
			case onHarvest.Destory:  //Carrots				
				Destroy (mapItemGO [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y].GO);
				mapItemGO [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y].GO = null;
				mapItemGO [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y].id = 0;
				mapItemsFromSave [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y] = "";
				break;
			case onHarvest.RegrowToStump:  // Trees				
				Destroy (mapItemGO [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y].GO);
				InstantiateObject (items [11], pos, mapChunks [0].transform, 0, mapItemGO [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y].id, mapItemGO [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y].age);	
				mapItemGO [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y].id = 11;
				mapItemGO [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y].age = age;
				mapItemGO [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y].GO.name = 11 + "," + age;
				mapItemsFromSave [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y] = 11 + "," + age;
				break;
			case onHarvest.Renewable:  //Berries
				mapItemsFromSave [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y] = id + "," + age;
				mapItemGO [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y].id = id;
				mapItemGO [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y].age = age;
				mapItemGO [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y].GO.name = id + "," + age;
				string spriteName = mapItemGO [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y].GO.GetComponent<SpriteRenderer> ().sprite.name;
				mapItemGO [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y].GO.GetComponent<SpriteRenderer> ().sprite = Resources.LoadAll<Sprite> ("Textures/Map/Items/Bushes/" + spriteName) [age];
				break;
			default:
				break;
		}
		ES2.Save (mapItemsFromSave [PlayerPrefs.GetInt ("mapChunkPosition")], mapChunks [PlayerPrefs.GetInt ("mapChunkPosition")].name + "i.txt");
	}

	public Vector2 GetLocalIslandPosition (Vector2 currentPos)
	{
		Vector2 pos = currentPos - new Vector2 (mapChunks [PlayerPrefs.GetInt ("mapChunkPosition")].transform.position.x, mapChunks [PlayerPrefs.GetInt ("mapChunkPosition")].transform.position.y);
		return pos;
	}

	public bool CheckForItemPlacement (Vector3 pos)
	{
		pos = GetLocalIslandPosition (pos);
		if (mapTilesFromSave [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y] > 1//ugly logic works only if mapTilesFromSave[0] is Water tile
		    && mapItemGO [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y].GO == null) {			
			return true;			
		} else {
			return false;
		}
	}
}