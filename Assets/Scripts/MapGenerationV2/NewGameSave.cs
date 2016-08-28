using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class NewGameSave : MonoBehaviour
{
	TextAsset[] mapItems;

	public void NewSave ()
	{
		ResetAllValues ();
		InitializeFirstVariables ();
		mapItems = Resources.LoadAll <TextAsset> ("Saves/");		
		for (int i = 0; i < mapItems.Length; i++) {			
			string[] array = mapItems [i].text.Split ('\n');
			ES2.Save (SingleToMulti ((string[])array), mapItems [i].name + ".txt");
		}
	}

	static string[,] SingleToMulti (string[] array)
	{
		int index = 0;
		int sqrt = (int)Mathf.Sqrt (array.Length);
		string[,] multi = new string[sqrt, sqrt];
		for (int y = 0; y < sqrt; y++) {
			for (int x = 0; x < sqrt; x++) {
				multi [x, y] = array [index];
				index++;
			}
		}
		return multi;
	}

	void ResetAllValues ()
	{
		PlayerPrefs.DeleteAll ();
	}

	void InitializeFirstVariables ()
	{		
		if (PlayerPrefs.GetInt ("IniPlayerPos") == 0) {			
			PlayerPrefs.SetFloat ("PlayerPositionX", 25);
			PlayerPrefs.SetFloat ("PlayerPositionY", 60);
			PlayerPrefs.SetInt ("mapChunkPosition", 0);
			PlayerPrefs.SetInt ("IniPlayerPos", 1);
		}
	}
}