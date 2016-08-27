using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MapLoader : MonoBehaviour
{
	//public GameObject mapChunk;
	public static MapLoader m_instance = null;
	public GameObject tree, stone, grass;

	string[][,] mapItems;
	GameObject[] mapChunks;
	int mapSize = 0;

	void Awake ()
	{
		m_instance = null;
		LoadMapChunks ();
		LoadMapData ();
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
		for (int i = 0; i < mapChunks.Length; i++) {
			mapItems [i] = ES2.Load2DArray<string> ("Saves/" + mapChunks [i].name + ".txt");
		}

		mapSize = (int)Mathf.Sqrt (mapItems [0].Length);
		for (int i = 0; i < mapChunks.Length; i++) {
			for (int x = 0; x < mapSize; x++) {
				for (int y = 0; y < mapSize; y++) {
					if (mapItems [i] [x, y] != "") {
						switch (mapItems [i] [x, y]) {
							case "Item_tree_#6_ItemDatabase_PFB":
								InstantiateObject (tree, new Vector3 (x, y, 0), mapChunks [i].transform);
								break;
							case "Item_stone_#2_ItemDatabase_PFB":
								InstantiateObject (stone, new Vector3 (x, y, 0), mapChunks [i].transform);
								break;
							case "Item_grass_#1_ItemDatabase_PFB":
								InstantiateObject (grass, new Vector3 (x, y, 0), mapChunks [i].transform);
								break;
							default:
								break;
						}
					}
				}
			}
		}
	}

	public void InstantiateObject (GameObject go, Vector3 pos, Transform parent)
	{
		GameObject g = Instantiate (go);
		g.name = go.name;
		g.transform.parent = parent;
		g.transform.localPosition = pos;
	}

	public void GetTile ()
	{	
		print ("pos");
		/*if (mapItemGO [(int)pos.x, (int)pos.y] != null) {
//			print (mapItemGO [(int)pos.x, (int)pos.y].name);
			return mapItemGO [(int)pos.x, (int)pos.y];
		}*/
		//	return null;
	}
}
