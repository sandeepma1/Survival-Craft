using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadMainLevel : MonoBehaviour
{
	public GameObject loadingScreen;
	public static LoadMainLevel m_instance = null;

	void Awake ()
	{
		m_instance = this;
	}

	public void LoadMainScene_Landscape ()
	{
		loadingScreen.SetActive (true);
		SceneManager.LoadScene ("Main_L");
	}

	public void LoadMainScene_Portrait ()
	{
		loadingScreen.SetActive (true);
		SceneManager.LoadScene ("Main_P");
	}

	public void QuitGame ()
	{
		Application.Quit ();
	}
}
