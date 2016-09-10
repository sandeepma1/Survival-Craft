using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MapLoader : MonoBehaviour
{
	public static MapLoader m_instance = null;
	public GameObject[] items;
      
	string[][,] mapItemsFromSave;
	item[][,] mapItemGO;
	public GameObject[] mapChunks;
	int mapSize = 0;
	string spriteName = "";

	void Start ()
	{
		m_instance = this;
		LoadMapChunks ();
		LoadMapData ();
		SpwanObjects ();
		DisableUnusedMapChunks ();
	}

	void Test ()
	{
		string test = "12,age,1";
		string[] t = test.Split (',');
		foreach (var a in t) {
			print (a);
		}
	}

	void LoadMapChunks ()
	{
		GameObject[] chunks = Resources.LoadAll<GameObject> ("Map/Chunks/");
		mapChunks = new GameObject[chunks.Length];
		for (int i = 0; i < chunks.Length; i++) {
			mapChunks [i] = GameObject.Instantiate (chunks [i]);
			mapChunks [i].name = chunks [i].name;
		}
	}

	public void LoadMapData ()
	{
		mapItemsFromSave = new string[mapChunks.Length][,];
		mapItemGO = new item[mapChunks.Length][,];
		for (int i = 0; i < mapChunks.Length; i++) {
			mapItemsFromSave [i] = ES2.Load2DArray<string> (mapChunks [i].name + ".txt");
		}
		mapSize = (int)Mathf.Sqrt (mapItemsFromSave [0].Length);

		for (int i = 0; i < mapChunks.Length; i++) {
			mapItemGO [i] = new item[mapSize, mapSize];
		}

		for (int i = 0; i < mapChunks.Length; i++) {
			for (int x = 0; x < mapSize; x++) {
				for (int y = 0; y < mapSize; y++) {
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
			for (int x = 0; x < mapSize; x++) {
				for (int y = 0; y < mapSize; y++) {
					if (mapItemsFromSave [i] [x, y].Length > 2) {
						switch (mapItemGO [i] [x, y].id) {
							case 11://trees
							case 16://berries
							case 21://berries
								if (mapItemGO [i] [x, y].age != ItemDatabase.m_instance.items [mapItemGO [i] [x, y].id].maxAge) {
									mapItemGO [i] [x, y].age = (sbyte)(mapItemGO [i] [x, y].age + 1);
									mapItemsFromSave [i] [x, y] = mapItemGO [i] [x, y].id + "," + mapItemGO [i] [x, y].age;
								}
								break;
							default:
								break;
						}
					}
				}
			}
		}
		ES2.Save (mapItemsFromSave [PlayerPrefs.GetInt ("mapChunkPosition")], mapChunks [PlayerPrefs.GetInt ("mapChunkPosition")].name + ".txt");
		SpwanObjects ();
	}

	public void SpwanObjects ()
	{
		for (int i = 0; i < mapChunks.Length; i++) {
			for (int x = 0; x < mapSize; x++) {
				for (int y = 0; y < mapSize; y++) {
					InstantiateObject (items [mapItemGO [i] [x, y].id], new Vector3 (x, y, 0), mapChunks [i].transform, i, mapItemGO [i] [x, y].id, mapItemGO [i] [x, y].age);
				}
			}
		}
	}

	public void InstantiateObject (GameObject go, Vector3 pos, Transform parent, int i, int id, int age)
	{
		if (id <= 0) {
			return;
		}
		if (mapItemGO [i] [(int)pos.x, (int)pos.y].GO != null) {
			Destroy (mapItemGO [i] [(int)pos.x, (int)pos.y].GO);
		}
		mapItemGO [i] [(int)pos.x, (int)pos.y].GO = Instantiate (go);
		mapItemGO [i] [(int)pos.x, (int)pos.y].GO.name = id + "," + age;
		mapItemGO [i] [(int)pos.x, (int)pos.y].GO.transform.parent = parent;
		mapItemGO [i] [(int)pos.x, (int)pos.y].GO.transform.localPosition = new Vector3 (pos.x, pos.y, 0);

		switch (mapItemGO [i] [(int)pos.x, (int)pos.y].id) {
			case 11: // Trees
				spriteName = mapItemGO [i] [(int)pos.x, (int)pos.y].GO.transform.GetChild (1).GetComponent<SpriteRenderer> ().sprite.name;
				if (age < ItemDatabase.m_instance.items [mapItemGO [i] [(int)pos.x, (int)pos.y].id].maxAge) {
					mapItemGO [i] [(int)pos.x, (int)pos.y].GO.transform.GetChild (0).gameObject.SetActive (false);
					mapItemGO [i] [(int)pos.x, (int)pos.y].GO.transform.GetChild (1).gameObject.GetComponent<SpriteRenderer> ().sprite = Resources.LoadAll<Sprite> ("Textures/Map/Items/Trees/" + spriteName) [age];
				}
				if (age == ItemDatabase.m_instance.items [mapItemGO [i] [(int)pos.x, (int)pos.y].id].maxAge) {
					mapItemGO [i] [(int)pos.x, (int)pos.y].GO.transform.GetChild (0).gameObject.SetActive (true);
					mapItemGO [i] [(int)pos.x, (int)pos.y].GO.transform.GetChild (1).gameObject.GetComponent<SpriteRenderer> ().sprite = Resources.LoadAll<Sprite> ("Textures/Map/Items/Trees/" + spriteName) [0];
				}
				break;
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

	public void InstantiatePlacedObject (GameObject go, Vector3 pos, Transform parent, int i, int id, int age)
	{
		if (id > 0 && mapItemGO [i] [(int)pos.x, (int)pos.y].GO == null) {
			mapItemGO [i] [(int)pos.x, (int)pos.y].GO = Instantiate (go);
			mapItemGO [i] [(int)pos.x, (int)pos.y].GO.name = id + "," + age;
			mapItemGO [i] [(int)pos.x, (int)pos.y].GO.transform.parent = parent;
			mapItemGO [i] [(int)pos.x, (int)pos.y].GO.transform.localPosition = new Vector3 (pos.x, pos.y, 0);
			mapItemGO [i] [(int)pos.x, (int)pos.y].id = (sbyte)id;
			mapItemGO [i] [(int)pos.x, (int)pos.y].age = (sbyte)age;

			switch (mapItemGO [i] [(int)pos.x, (int)pos.y].id) {
				case 11: // Trees
					spriteName = mapItemGO [i] [(int)pos.x, (int)pos.y].GO.transform.GetChild (1).GetComponent<SpriteRenderer> ().sprite.name;
					if (age < ItemDatabase.m_instance.items [mapItemGO [i] [(int)pos.x, (int)pos.y].id].maxAge) {
						mapItemGO [i] [(int)pos.x, (int)pos.y].GO.transform.GetChild (0).gameObject.SetActive (false);
						mapItemGO [i] [(int)pos.x, (int)pos.y].GO.transform.GetChild (1).gameObject.GetComponent<SpriteRenderer> ().sprite = Resources.LoadAll<Sprite> ("Textures/Map/Items/Trees/" + spriteName) [age];
					}
					if (age == ItemDatabase.m_instance.items [mapItemGO [i] [(int)pos.x, (int)pos.y].id].maxAge) {
						mapItemGO [i] [(int)pos.x, (int)pos.y].GO.transform.GetChild (0).gameObject.SetActive (true);
						mapItemGO [i] [(int)pos.x, (int)pos.y].GO.transform.GetChild (1).gameObject.GetComponent<SpriteRenderer> ().sprite = Resources.LoadAll<Sprite> ("Textures/Map/Items/Trees/" + spriteName) [0];
					}
					break;
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

			//ActionManager.m_AC_instance.currentWeildedItem.ID
			if (Devdog.InventorySystem.InventoryManager.FindAll (ActionManager.m_AC_instance.currentWeildedItem.ID, false).Count > 0) {
				Devdog.InventorySystem.InventoryManager.RemoveItem (ActionManager.m_AC_instance.currentWeildedItem.ID, 1, false);

			}

			mapItemsFromSave [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y] = id + "," + age;
			ES2.Save (mapItemsFromSave [PlayerPrefs.GetInt ("mapChunkPosition")], mapChunks [PlayerPrefs.GetInt ("mapChunkPosition")].name + ".txt");
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
		pos = GetPlayersLocalPosition (pos);
		if (mapItemGO [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y].id >= 0) {
			return mapItemGO [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y];
		}
		return new item ();
	}

	public void SaveMapItemData (sbyte id, sbyte age, Vector2 pos, onHarvest harvestType)
	{
		pos = GetPlayersLocalPosition (pos);
		switch (harvestType) {
			case onHarvest.Destory:  //Carrots
				mapItemsFromSave [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y] = "";
				Destroy (mapItemGO [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y].GO);
				mapItemGO [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y].GO = null;
				mapItemGO [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y].id = 0;
				break;
			case onHarvest.RegrowToZero:  // Trees
				mapItemsFromSave [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y] = id + "," + age;
				mapItemGO [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y].id = id;
				mapItemGO [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y].age = age;
				mapItemGO [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y].GO.name = id + "," + age;
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
		ES2.Save (mapItemsFromSave [PlayerPrefs.GetInt ("mapChunkPosition")], mapChunks [PlayerPrefs.GetInt ("mapChunkPosition")].name + ".txt");
	}

	public Vector2 GetPlayersLocalPosition (Vector2 curentPos)
	{
		Vector2 pos = curentPos - new Vector2 (mapChunks [PlayerPrefs.GetInt ("mapChunkPosition")].transform.position.x, mapChunks [PlayerPrefs.GetInt ("mapChunkPosition")].transform.position.y);
		return pos;
	}
}

