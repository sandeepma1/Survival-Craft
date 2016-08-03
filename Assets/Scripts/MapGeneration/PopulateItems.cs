using UnityEngine;
using System.Collections;

public partial class CreateNewGame : MonoBehaviour
{
	//public GameObject stone, grass, log;

	string[,] mapItems = new string[128, 128];
	// = new string[mapChunkSize, mapChunkSize];

	public void PopulateGameitems (Texture2D map)
	{		
		for (int x = 0; x < map.height; x++) {
			for (int y = 0; y < map.width; y++) {						
				if (map.GetPixel (x, y) == regions [0].colour) {
					FillArrayBlank (x, y);
				} else if (map.GetPixel (x, y) == regions [1].colour) {					
					FillArrayBlank (x, y);
				} else if (map.GetPixel (x, y) == regions [2].colour) {
					FillArrayBlank (x, y);
				} else if (map.GetPixel (x, y) == regions [3].colour) {
					Fill2DArray ("grass", x, y, 0.15f);
				} else if (map.GetPixel (x, y) == regions [4].colour) {
					Fill2DArray ("log", x, y, 0.1f);
				} else if (map.GetPixel (x, y) == regions [5].colour) {
					Fill2DArray ("stone", x, y, 0.5f);
				} else if (map.GetPixel (x, y) == regions [6].colour) {
					FillArrayBlank (x, y);
				} else if (map.GetPixel (x, y) == regions [7].colour) {
					FillArrayBlank (x, y);
				}					
			}
		}
		ES2.Save (mapItems, "mapItems.txt");
	}

	void Fill2DArray (string itemName, int x, int y, float probability)
	{
		if (Random.value <= probability) {
			mapItems [x, y] = itemName;
		} else {
			mapItems [x, y] = "";
		}
	}

	void FillArrayBlank (int x, int y)
	{
		mapItems [x, y] = "";
	}


}
