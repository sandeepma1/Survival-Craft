using UnityEngine;
using UnityEngine.SceneManagement;

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
		LoadMainLevel.m_instance.LoadMainScene_Landscape ();
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
		ES2.DeleteDefaultFolder ();
	}

	void InitializeFirstVariables ()
	{		
		if (PlayerPrefs.GetInt ("IniPlayerPos") == 0) {	
				
			PlayerPrefs.SetFloat ("PlayerPositionX", 25);
			PlayerPrefs.SetFloat ("PlayerPositionY", 60);
			PlayerPrefs.SetInt ("mapChunkPosition", 0);
			ES2.DeleteDefaultFolder ();
			PlayerPrefs.SetInt ("IniPlayerPos", 1);
		}
	}
}