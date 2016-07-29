using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class IGMMenu : MonoBehaviour
{
	public static IGMMenu m_instance = null;
	public GameObject playerGrid;
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
	public void ToggleGrid (bool flag)
	{
		print (flag);
		playerGrid.GetComponent<SpriteRenderer> ().enabled = flag;
		
	}

}
