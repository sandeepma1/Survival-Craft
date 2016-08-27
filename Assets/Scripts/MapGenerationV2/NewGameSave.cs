using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class NewGameSave : MonoBehaviour
{
	// Use this for initialization
	TextAsset[] mapChunks;

	public void NewSave ()
	{
		mapChunks = Resources.LoadAll <TextAsset> ("Saves");
		for (int i = 0; i < mapChunks.Length; i++) {
			print (mapChunks [i].name);
			string[,] TestArray = (string[,])Deserialize ("Assets/Resources/Saves/" + mapChunks [i].name + ".txt");
			ES2.Save (TestArray, "Saves/" + mapChunks [i].name + ".txt");
		}
	}

	public static object Deserialize (string path)
	{
		using (Stream stream = File.Open (path, FileMode.Open)) {
			BinaryFormatter bformatter = new BinaryFormatter ();
			return bformatter.Deserialize (stream);
		}
	}

}