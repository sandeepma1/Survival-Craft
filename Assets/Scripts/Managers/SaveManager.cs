using UnityEngine;
using System.Collections;

public class SaveManager : MonoBehaviour
{
	public GameObject inventorySave;
	// Use this for initialization
	void Start ()
	{
		InvokeRepeating ("SaveInventory", 0, 2);		
	}

	void SaveInventory ()
	{
		inventorySave.GetComponent <Devdog.InventorySystem.CollectionSaverLoaderBase> ().Save ();
	}
	// Update is called once per frame
	void Update ()
	{
	
	}
}
