using UnityEngine;
using System.Collections;

public class TabsControl : MonoBehaviour
{

	// Use this for initialization
	public GameObject tabInventory, tabCrafting;
	public GameObject menuInventory, menuCrafting;

	public void GetName (GameObject tab)
	{
		switch (tab.name) {
			case "TabInventory":
				menuCrafting.SetActive (false);
				menuInventory.SetActive (true);
				break;
			case "TabCrafting":
				menuCrafting.SetActive (true);
				menuInventory.SetActive (false);
				break;
			default:
				break;
		}
	}
}
