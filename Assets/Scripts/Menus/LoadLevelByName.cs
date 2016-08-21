using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadLevelByName : MonoBehaviour
{
	public string levelName = "Menu";

	void Awake ()
	{
		SceneManager.LoadScene (levelName);
	}
}
