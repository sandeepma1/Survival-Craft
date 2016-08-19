using UnityEngine;
using System.Collections;

public class RetriveGOs : MonoBehaviour
{
	public GameObject parent;
	GameObject[,] trees = new GameObject[128, 128];

	void Start ()
	{	
		for (int i = 0; i < parent.transform.childCount; i++) {
			//print (parent.transform.GetChild (i).position);
			trees [(int)parent.transform.GetChild (i).position.x, (int)parent.transform.GetChild (i).position.x] = parent.transform.GetChild (i).gameObject;
		}
		for (int x = 0; x < 128; x++) {
			for (int y = 0; y < 128; y++) {
				if (trees [x, y] != null) {
					print (trees [x, y].gameObject.transform);
				}
			}
		}
	}
}
