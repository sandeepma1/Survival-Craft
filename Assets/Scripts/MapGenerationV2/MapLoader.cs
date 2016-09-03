using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MapLoader : MonoBehaviour
{
	public static MapLoader m_instance = null;
	public GameObject[] items;
	public GameObject[] inventoryItems;
	// tree, stone, stone1, grass, log;
	Vector3 currentLocalPlayerPosition;
	string[][,] mapItems;
	item[][,] mapItemGO;
	GameObject[] mapChunks;
	int mapSize = 0;
	/*float time = 0.0f;
	bool startDrop = false;
	float dropTime = 0.0f;*/

	void Start ()
	{
		m_instance = this;
		Test ();
		LoadMapChunks ();
		LoadMapData ();
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
		GameObject[] chunks = Resources.LoadAll <GameObject> ("Map/Chunks/");
		mapChunks = new GameObject[chunks.Length];
		for (int i = 0; i < chunks.Length; i++) {			
			mapChunks [i] = GameObject.Instantiate (chunks [i]);
			mapChunks [i].name = chunks [i].name;
		}
	}

	public void LoadMapData ()
	{
		mapItems = new string[mapChunks.Length][,];
		mapItemGO = new item[mapChunks.Length] [,];
		for (int i = 0; i < mapChunks.Length; i++) {
			mapItems [i] = ES2.Load2DArray<string> (mapChunks [i].name + ".txt");
		}
		mapSize = (int)Mathf.Sqrt (mapItems [0].Length);

		for (int i = 0; i < mapChunks.Length; i++) {
			mapItemGO [i] = new item[mapSize, mapSize];
		}		

		for (int i = 0; i < mapChunks.Length; i++) {
			for (int x = 0; x < mapSize; x++) {
				for (int y = 0; y < mapSize; y++) {
					if (mapItems [i] [x, y].Length > 2) {
						mapItems [i] [x, y] = mapItems [i] [x, y].TrimEnd (new char[] { '\r', '\n' });
						string[] itemss = mapItems [i] [x, y].Split (',');
						mapItemGO [i] [x, y].id = int.Parse (itemss [0]);
						mapItemGO [i] [x, y].age = int.Parse (itemss [1]);
						switch (itemss [0]) { // item index
							case "11":
								InstantiateObject (items [11], new Vector3 (x, y, 0), mapChunks [i].transform, i, mapItemGO [i] [x, y].age);
								break;
							case "0":
								InstantiateObject (items [0], new Vector3 (x, y, 0), mapChunks [i].transform, i, mapItemGO [i] [x, y].age);
								break;
							case "1":
								InstantiateObject (items [1], new Vector3 (x, y, 0), mapChunks [i].transform, i, mapItemGO [i] [x, y].age);
								break;
							case "5":
								InstantiateObject (items [5], new Vector3 (x, y, 0), mapChunks [i].transform, i, mapItemGO [i] [x, y].age);
								break;
							case "10":
								InstantiateObject (items [10], new Vector3 (x, y, 0), mapChunks [i].transform, i, mapItemGO [i] [x, y].age);
								break;
							default:
								break;
						}
					}
				}
			}
		}
	}

	public void InstantiateObject (GameObject go, Vector3 pos, Transform parent, int i, int age)
	{
		mapItemGO [i] [(int)pos.x, (int)pos.y].GO = Instantiate (go);
		mapItemGO [i] [(int)pos.x, (int)pos.y].GO.name = go.name;
		mapItemGO [i] [(int)pos.x, (int)pos.y].GO.transform.parent = parent;
		mapItemGO [i] [(int)pos.x, (int)pos.y].GO.transform.localPosition = new Vector3 (pos.x, pos.y, 0);

		if (age > 0 && age < ItemDatabase.m_instance.items [mapItemGO [i] [(int)pos.x, (int)pos.y].id].maxAge) {
			mapItemGO [i] [(int)pos.x, (int)pos.y].GO.transform.GetChild (0).gameObject.SetActive (false);
			string spriteName = mapItemGO [i] [(int)pos.x, (int)pos.y].GO.transform.GetChild (1).GetComponent <SpriteRenderer> ().sprite.name;
			mapItemGO [i] [(int)pos.x, (int)pos.y].GO.transform.GetChild (1).gameObject.GetComponent <SpriteRenderer> ().sprite = Resources.LoadAll<Sprite> ("Textures/Map/Items/Trees/" + spriteName) [age];
		}
		if (age == ItemDatabase.m_instance.items [mapItemGO [i] [(int)pos.x, (int)pos.y].id].maxAge) {
			mapItemGO [i] [(int)pos.x, (int)pos.y].GO.transform.GetChild (0).gameObject.SetActive (true);
			string spriteName = mapItemGO [i] [(int)pos.x, (int)pos.y].GO.transform.GetChild (1).GetComponent <SpriteRenderer> ().sprite.name;
			mapItemGO [i] [(int)pos.x, (int)pos.y].GO.transform.GetChild (1).gameObject.GetComponent <SpriteRenderer> ().sprite = Resources.LoadAll<Sprite> ("Textures/Map/Items/Trees/" + spriteName) [0];
		}
		//mapItemGO [i] [(int)pos.x, (int)pos.y].GetComponent <SpriteRenderer> ().sortingOrder = (int)(transform.localPosition.y * -10);
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

	public int GetTile (Vector2 pos)
	{
		pos = GetPlayersLocalPosition (pos);
		if (mapItemGO [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y].id >= 0) {
			return mapItemGO [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y].id;
		}
		return -1;
	}

	public void InstansiateDropGameObject (int id, int dropValue)
	{		
		for (int i = 0; i < dropValue; i++) {
			Vector2 ran = GameEventManager.currentSelectedTilePosition + Random.insideUnitCircle;
			GameObject drop = GameObject.Instantiate (inventoryItems [id], new Vector3 (ran.x, ran.y, 0), Quaternion.identity) as GameObject;
			drop.GetComponent <Devdog.InventorySystem.ObjectTriggererItem> ().isPickable = true;
			drop.transform.localScale = new Vector3 (0.75f, 0.75f, 0.75f);
			/*drop.AddComponent <Devdog.InventorySystem.UnusableInventoryItem> ().ID = 2;
			drop.AddComponent <Devdog.InventorySystem.ObjectTriggererItem> ();
			drop.GetComponent <Devdog.InventorySystem.ObjectTriggererItem> ().isPickable = true;
			drop.GetComponent <Devdog.InventorySystem.ObjectTriggererItem> ().Toggle (true);*/
		}
	}

	public void UpdateItemandSave (Vector2 pos)
	{
		pos = GetPlayersLocalPosition (pos);
		//if (mapItemGO [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y] != null) {
		//Destroy (mapItemGO [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y]);
		switch (mapItemGO [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y].id) {
			case 11: //Replace Tree with stump
				mapItemGO [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y].GO.transform.GetChild (0).gameObject.SetActive (false);
				break;
			default:
				Destroy (mapItemGO [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y].GO);
				break;
		}
		//}
		SaveMapItemData (pos);
	}

	public void SaveMapItemData (Vector2 pos)
	{
		string s = mapItemGO [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y].id + "," + mapItemGO [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y].age;
		mapItems [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y] = s;
		ES2.Save (mapItems [PlayerPrefs.GetInt ("mapChunkPosition")], mapChunks [PlayerPrefs.GetInt ("mapChunkPosition")].name + ".txt");
	}

	public Vector2 GetPlayersLocalPosition (Vector2 curentPos)
	{
		Vector2 pos = curentPos - new Vector2 (mapChunks [PlayerPrefs.GetInt ("mapChunkPosition")].transform.position.x, mapChunks [PlayerPrefs.GetInt ("mapChunkPosition")].transform.position.y);
		return pos;
	}
}

[SerializeField]
public struct item
{
	public int id;
	public int age;
	public GameObject GO;
}
/*void Update ()
	{
		if (GameEventManager.GetState () == GameEventManager.E_STATES.e_game) {
			if (startDrop) {
				time += Time.deltaTime;
				if (time >= dropTime) {
					//drop()
					print ("droped");
					startDrop = false;
					dropTime = 0;
				}
			}
		}
	}*/