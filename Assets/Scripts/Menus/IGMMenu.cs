using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class IGMMenu : MonoBehaviour
{
	public static IGMMenu m_instance = null;
	public GameObject actionButton, rightAnalogStick;
	//GameObject pauseMenu;

	// Use this for initialization
	void Awake ()
	{
		m_instance = this;
		//pauseMenu = transform.FindChild ("PauseMenu").gameObject;	
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
	public void ToggleControls (string index)
	{
		if (index == "s") {
			PlayerPrefs.SetString ("Controls", "s");
			actionButton.SetActive (true);
			rightAnalogStick.SetActive (false);
		}
		if (index == "d") {
			PlayerPrefs.SetString ("Controls", "d");
			actionButton.SetActive (false);
			rightAnalogStick.SetActive (true);
		}
	}

}
