using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MapLoader : MonoBehaviour
{
	public static MapLoader m_instance = null;
	public GameObject tree, stone, grass, log;

	string[][,] mapItems;
	GameObject[][,] mapItemGO;
	GameObject[] mapChunks;
	int mapSize = 0;

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
							case "Item_tree_#6_ItemDatabase_PFB\r":					
								InstantiateObject (tree, new Vector3 (x, y, 0), mapChunks [i].transform, i);
								break;
							case "Item_stone_#2_ItemDatabase_PFB\r":
								InstantiateObject (stone, new Vector3 (x, y, 0), mapChunks [i].transform, i);
								break;
							case "Item_grass_#1_ItemDatabase_PFB\r":
								InstantiateObject (grass, new Vector3 (x, y, 0), mapChunks [i].transform, i);
								break;
							case "Item_log_#3_ItemDatabase_PFB\r":
								InstantiateObject (log, new Vector3 (x, y, 0), mapChunks [i].transform, i);
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
		mapItemGO [i] [(int)pos.x, (int)pos.y].GetComponent <SpriteRenderer> ().sortingOrder = (int)(transform.localPosition.y * -10);
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
//		print (pos);
		if (mapItemGO [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y] != null) {
			return mapItemGO [PlayerPrefs.GetInt ("mapChunkPosition")] [(int)pos.x, (int)pos.y];
		}
		return null;
	}

	public void SaveMapitemData (int x, int y)
	{
		Vector3 presentPosition = (mapChunks [PlayerPrefs.GetInt ("mapChunkPosition")].transform.position);
		x = x - (int)presentPosition.x;
		y = y - (int)presentPosition.y;
		mapItems [PlayerPrefs.GetInt ("mapChunkPosition")] [x, y] = "";
		ES2.Save (mapItems [PlayerPrefs.GetInt ("mapChunkPosition")], mapChunks [PlayerPrefs.GetInt ("mapChunkPosition")].name + ".txt");
	}
}
