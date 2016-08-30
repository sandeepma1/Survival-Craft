using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MapLoader : MonoBehaviour
{
	public static MapLoader m_instance = null;
	public GameObject[] items;
	public GameObject[] inventoryItems;
	// tree, stone, stone1, grass, log;

	string[][,] mapItems;
	GameObject[][,] mapItemGO;
	GameObject[] mapChunks;
	int mapSize = 0;
	float time = 0.0f;
	bool startDrop = false;
	float dropTime = 0.0f;

	void Awake ()
	{
		m_instance = this;
		LoadMapChunks ();
		LoadMapData ();
		DisableUnusedMapChunks ();
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
		mapItemGO = new GameObject[mapChunks.Length] [,];
		for (int i = 0; i < mapChunks.Length; i++) {
			mapItems [i] = ES2.Load2DArray<string> (mapChunks [i].name + ".txt");
		}
		mapSize = (int)Mathf.Sqrt (mapItems [0].Length);

		for (int i = 0; i < mapChunks.Length; i++) {
			mapItemGO [i] = new GameObject[mapSize, mapSize];
		}		

		for (int i = 0; i < mapChunks.Length; i++) {
			for (int x = 0; x < mapSize; x++) {
				for (int y = 0; y < mapSize; y++) {
					if (mapItems [i] [x, y] != "\r") {
						switch (mapItems [i] [x, y]) {
							case "11\r":					
								InstantiateObject (items [11], new Vector3 (x, y, 0), mapChunks [i].transform, i);
								break;
							case "0\r":
								InstantiateObject (items [0], new Vector3 (x, y, 0), mapChunks [i].transform, i);
								break;
							case "1\r":
								InstantiateObject (items [1], new Vector3 (x, y, 0), mapChunks [i].transform, i);
								break;
							case "5\r":
								InstantiateObject (items [5], new Vector3 (x, y, 0), mapChunks [i].transform, i);
								break;
							case "10\r":
								InstantiateObject (items [10], new Vector3 (x, y, 0), mapChunks [i].transform, i);
								break;
							default:
								break;
						}
					}
				}
			}
		}
	}

	public void InstantiateObject (GameObject go, Vector3 pos, Transform parent, int i)
	{
		mapItemGO [i] [(int)pos.x, (int)pos.y] = Instantiate (go);
		mapItemGO [i] [(int)pos.x, (int)pos.y].name = go.name;
		mapItemGO [i] [(int)pos.x, (int)pos.y].transform.parent = parent;
		mapItemGO [i] [(int)pos.x, (int)pos.y].transform.localPosition = new Vector3 (pos.x, pos.y, 0);

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

	public GameObject GetTile (Vector2 pos)
	{
		Vector3 presentPosition = (mapChunks [PlayerPrefs.GetInt ("mapChunkPosition")].transform.position);
		pos = new Vector2 (pos.x - presentPosition.x, pos.y - presentPosition.y);
		if (mapItemGO [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y] != null) {
			return mapItemGO [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y];
		}
		return null;
	}

	public void InstansiatePickableGameObject (int id, int dropValue)
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

	public void DestoryTile (Vector2 pos)
	{
		if (mapItemGO [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y] != null) {
			Destroy (mapItemGO [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y]);
		}
	}

	public void SaveMapitemData (Vector2 pos)
	{
		Vector3 presentPosition = (mapChunks [PlayerPrefs.GetInt ("mapChunkPosition")].transform.position);
		pos = pos - new Vector2 (presentPosition.x, presentPosition.y);
		mapItems [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y] = "";
		ES2.Save (mapItems [PlayerPrefs.GetInt ("mapChunkPosition")], mapChunks [PlayerPrefs.GetInt ("mapChunkPosition")].name + ".txt");
	}
}
