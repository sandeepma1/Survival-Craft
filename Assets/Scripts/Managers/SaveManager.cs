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
		//touchToggleControlUI.GetComponent <Toggle> ().isOn = Bronz.LocalStore.Instance.GetBool ("TouchControls");
		InvokeRepeating ("SaveInventory", 0, 2);
	}

	void SaveInventory ()
	{
		inventorySave.GetComponent <Devdog.InventorySystem.CollectionSaverLoaderBase> ().Save ();
	}

	public void SaveGameTime (int time)
	{
		Bronz.LocalStore.Instance.SetInt ("gameTime", time);
	}

	public void SaveGameDays (int day)
	{		
		Bronz.LocalStore.Instance.SetInt ("gameDay", day);
	}

	public void SaveGameCurrentPhase (int phase)
	{		
		Bronz.LocalStore.Instance.SetInt ("currentPhase", phase);
	}

	/*public void SaveTouchControlToggleOption (bool flag)
	{
		Bronz.LocalStore.Instance.SetBool ("TouchControls", flag);
		if (!flag) {
			MoreInventoryButton.m_instance.leftStick.SetActive (true);
		}
	}
*/
}
