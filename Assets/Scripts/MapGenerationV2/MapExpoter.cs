using UnityEngine;
using System.Collections;

public class MapExpoter : MonoBehaviour
{
	//public bool autoUpdate;
	public GameObject parent;
	//public string path;
	//public string name;
	GameObject tempMap;
	string[,] mapObjects;

	void Start ()
	{
		ExportMap ();
	}

	public void ExportMap ()
	{
		mapObjects = new string[128, 128];

		for (int x = 0; x < 128; x++) {
			for (int y = 0; y < 128; y++) {				
				mapObjects [x, y] = "";				
			}
		}

		for (int i = 0; i < parent.transform.childCount; i++) {
			if (parent.transform.GetChild (i).gameObject.name.StartsWith ("0") || parent.transform.GetChild (i).gameObject.name.StartsWith ("1")) {
//				mapObjects [(int)parent.transform.GetChild (i).position.x, (int)parent.transform.GetChild (i).position.y] = "";
			} else {
				mapObjects [(int)parent.transform.GetChild (i).localPosition.x, (int)parent.transform.GetChild (i).localPosition.y] = parent.transform.GetChild (i).gameObject.name;
			}
		}

		for (int x = 0; x < 128; x++) {
			for (int y = 0; y < 128; y++) {
				if (mapObjects [x, y] != "") {
					print (mapObjects [x, y] + " X:" + x + " y: " + y);
				}
			}
		}

		ES2.Save (mapObjects, "mapobjects.txt");
		DestroyChildrenAndSaveAsPrefab (parent);
	}

	void DestroyChildrenAndSaveAsPrefab (GameObject obj)
	{
		tempMap = GameObject.Instantiate (obj);
		for (int i = 0; i < tempMap.transform.childCount; i++) {			
			if (tempMap.transform.GetChild (i).gameObject.name.StartsWith ("0") || tempMap.transform.GetChild (i).gameObject.name.StartsWith ("1")) {
				//Nothing
			} else {
				Destroy (tempMap.transform.GetChild (i).gameObject);
			}
		}		
		StartCoroutine (SaveMap_c ());
	}

	IEnumerator SaveMap_c ()
	{
		yield return new WaitForSeconds (1f);
		
	}

}