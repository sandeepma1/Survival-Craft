using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGameSave_SB : MonoBehaviour
{
	TextAsset[] mapItems;

	public void NewSave ()
	{
		ResetAllValues ();
		InitializeFirstVariables ();
		mapItems = Resources.LoadAll<TextAsset> ("Saves/");
		for (int i = 0; i < mapItems.Length; i++) {
			string[] array = mapItems [i].text.Split ('\n');
			ES2.Save (SingleToMulti ((string[])array), mapItems [i].name + ".txt");
		}
		LoadMainLevel.m_instance.LoadMainScene_SandBox ();
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
		Bronz.LocalStore.Instance.DeleteAll ();
		ES2.DeleteDefaultFolder ();
	}

	void InitializeFirstVariables ()
	{
		if (Bronz.LocalStore.Instance.GetInt ("IniPlayerPos") == 0) {
			Bronz.LocalStore.Instance.SetFloat ("PlayerPositionX", 25);
			Bronz.LocalStore.Instance.SetFloat ("PlayerPositionY", 60);
			Bronz.LocalStore.Instance.SetInt ("mapChunkPosition", 0);
			ES2.DeleteDefaultFolder ();
			Bronz.LocalStore.Instance.SetInt ("IniPlayerPos", 1);
		}
	}
}