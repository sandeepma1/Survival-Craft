using UnityEngine;
using System.Collections;

public class SaveManager : MonoBehaviour
{
	public static SaveManager m_instance = null;
	public GameObject inventorySave;
	// Use this for initialization

	void Awake ()
	{
		m_instance = this;
	}

	void Start ()
	{
		InvokeRepeating ("SaveInventory", 0, 2);
	}

	void SaveInventory ()
	{
		inventorySave.GetComponent <Devdog.InventorySystem.CollectionSaverLoaderBase> ().Save ();
	}

	public void SaveGameTime (int time)
	{
		PlayerPrefs.SetInt ("gameTime", time);
	}

	public void SaveGameDays (int day)
	{		
		PlayerPrefs.SetInt ("gameDay", day);
	}

	public void SaveGameCurrentPhase (int phase)
	{		
		PlayerPrefs.SetInt ("currentPhase", phase);
	}

}
