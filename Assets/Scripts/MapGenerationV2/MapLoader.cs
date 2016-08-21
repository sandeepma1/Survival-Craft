﻿using UnityEngine;
using System.Collections;

public class MapLoader : MonoBehaviour
{
	public GameObject tree;

	string[,] mapItems;
	int mapSize = 0;

	public void LoadMapData ()
	{
		mapItems = ES2.Load2DArray<string> ("mapobjects.txt");
		mapSize = (int)Mathf.Sqrt (mapItems.Length);
		for (int x = 0; x < mapSize; x++) {
			for (int y = 0; y < mapSize; y++) {
				if (mapItems [x, y] != "") {
					print (mapItems [x, y]);
				}
			}
		}
	}
	
}
