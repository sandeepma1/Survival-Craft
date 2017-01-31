using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IGMMenu : MonoBehaviour
{
	public static IGMMenu m_instance = null;
	public GameObject playerGrid;
	public GameObject loadingScreen, igmMenu;

	void Awake ()
	{
		m_instance = this;
	}

	void Update ()
	{
		if (Input.GetKey (KeyCode.Space)) {
			print ("escape");
		}
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WindowsEditor) {
			if (Input.GetKey (KeyCode.Escape)) {
				print ("escape");
				if (SceneManager.GetActiveScene ().name == "Menu") {
					Application.Quit ();
				} else {
					ShowIGMMenu ();
				}
				return;
			}
		}
	}

	public void ShowIGMMenu ()
	{
		igmMenu.SetActive (true);
	}

	public void HideIGMMenu ()
	{
		igmMenu.SetActive (false);
	}

	public void LoadMainLevel ()
	{
		loadingScreen.SetActive (true);
		SceneManager.LoadScene ("Main");
	}

	public void LoadMenuLevelDeleteSaves ()
	{
		loadingScreen.SetActive (true);
		Bronz.LocalStore.Instance.DeleteAll ();
		ES2.DeleteDefaultFolder ();
		SceneManager.LoadScene ("Menu");
	}

	public void LoadMenuLevel ()
	{
		loadingScreen.SetActive (true);
		SceneManager.LoadScene ("Menu");
	}

	public void LoadMenuLevel_Portrait ()
	{
		loadingScreen.SetActive (true);
		SceneManager.LoadScene ("Menu_Portrait");
	}

	// Pause Menu
	public void OpenPauseMenu ()
	{
		//pauseMenu.SetActive (true);
	}

	public void ClosePauseMenu ()
	{
		//pauseMenu.SetActive (false);
	}
	//Toggle Controls
	public void ToggleGrid (bool flag)
	{
		print (flag);
		playerGrid.GetComponent<SpriteRenderer> ().enabled = flag;		
	}

	public void QuitGame ()
	{
		Application.Quit ();
	}

}
