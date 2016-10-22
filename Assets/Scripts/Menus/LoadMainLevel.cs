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

	public void LoadMainScene_SandBox ()
	{
		loadingScreen.SetActive (true);
		SceneManager.LoadScene ("Main_SB");
	}

	public void LoadMainScene_ProceduralGeneration ()
	{
		loadingScreen.SetActive (true);
		SceneManager.LoadScene ("Main_PG");
	}

	public void LoadMainScene_ProceduralGeneration_Portrait ()
	{
		loadingScreen.SetActive (true);
		SceneManager.LoadScene ("Main_PG_Portrait");
	}

	public void LoadMainScene_SpriteLightDemo ()
	{
		loadingScreen.SetActive (true);
		SceneManager.LoadScene ("SpriteLightKitScene");
	}

	public void QuitGame ()
	{
		Application.Quit ();
	}
}
