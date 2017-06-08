using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SaveManager : MonoBehaviour
{
	public static SaveManager m_instance = null;
	public GameObject inventorySave;
	//, touchToggleControlUI;
	// Use this for initialization

	void Awake ()
	{
		m_instance = this;
	}

	void Start ()
	{		
		//touchToggleControlUI.GetComponent <Toggle> ().isOn = PlayerPrefs.GetBool ("TouchControls");
		InvokeRepeating ("SaveInventory", 0, 2);
	}

	void SaveInventory ()
	{
		//inventorySave.GetComponent <Devdog.InventorySystem.CollectionSaverLoaderBase> ().Save ();
	}

	public void SaveGameTime (int time)
	{
		//PlayerPrefs.SetInt ("gameTime", time);
	}

	public void SaveGameDays (int day)
	{		
		//	PlayerPrefs.SetInt ("gameDay", day);
	}

	public void SaveGameCurrentPhase (int phase)
	{		
		//PlayerPrefs.SetInt ("currentPhase", phase);
	}

	/*public void SaveTouchControlToggleOption (bool flag)
	{
		PlayerPrefs.SetBool ("TouchControls", flag);
		if (!flag) {
			MoreInventoryButton.m_instance.leftStick.SetActive (true);
		}
	}
*/
}
