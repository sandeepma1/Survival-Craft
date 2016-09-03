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

	public void SaveGameTime (float time)
	{
		PlayerPrefs.SetFloat ("gameTime", time);
	}

	public void SaveGameTime (int day)
	{
		PlayerPrefs.SetInt ("gameDay", day);
	}
}
