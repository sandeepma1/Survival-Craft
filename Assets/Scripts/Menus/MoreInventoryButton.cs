using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MoreInventoryButton : MonoBehaviour
{
	public static MoreInventoryButton m_instance = null;
	public RectTransform mainUIWindow;
	public GameObject mainCanvas, inventoryMenu;
	public GameObject leftStick, rightStick, actionButton, inventoryTab, craftingTab, settingsTab, infoTab;
	public GameObject craftingMenu;
	private bool toggleInventory = false, toggleCrafting = false;
	float heightAdjuster;
	int tabIndex = 0;
	public Devdog.InventorySystem.InventoryUIItemWrapper[] items;
	// = FindObjectsOfType (typeof(Devdog.InventorySystem.InventoryUIItemWrapper)) as Devdog.InventorySystem.InventoryUIItemWrapper[];

	void Awake ()
	{
		m_instance = this;
	}

	public void RemoveBorder ()
	{
		for (int i = 0; i < items.Length; i++) {
			items [i].border.gameObject.SetActive (false);	
		}
	}

	void Start ()
	{
		items = FindObjectsOfType (typeof(Devdog.InventorySystem.InventoryUIItemWrapper)) as Devdog.InventorySystem.InventoryUIItemWrapper[];
		tabIndex = inventoryMenu.GetComponent <RectTransform> ().GetSiblingIndex ();
		heightAdjuster = ((mainCanvas.GetComponent <RectTransform> ().rect.height / 2) + 200) * -1;
//		print (mainCanvas.GetComponent <RectTransform> ().rect);
		ToggleInventorySize ();
	}

	public void ToggleInventorySize ()
	{		
		toggleInventory = !toggleInventory;				
		leftStick.SetActive (toggleInventory);
		if (PlayerPrefs.GetString ("Controls") == "d") {
			rightStick.SetActive (toggleInventory);
		} else {
			actionButton.SetActive (toggleInventory);
		}
		inventoryTab.SetActive (!toggleInventory);
		craftingTab.SetActive (!toggleInventory);
		settingsTab.SetActive (!toggleInventory);
		infoTab.SetActive (!toggleInventory);
		if (toggleInventory) {
			inventoryMenu.GetComponent <RectTransform> ().SetSiblingIndex (tabIndex);
			GameEventManager.SetState (GameEventManager.E_STATES.e_game);
			mainUIWindow.anchoredPosition = new Vector3 (mainUIWindow.anchoredPosition.x, heightAdjuster);
			GameEventManager.SetMenuState (GameEventManager.E_MenuState.e_menuDown);
		} else {
			inventoryMenu.GetComponent <RectTransform> ().SetSiblingIndex (tabIndex); 
			GameEventManager.SetState (GameEventManager.E_STATES.e_pause);
			mainUIWindow.anchoredPosition = Vector3.zero;
			GameEventManager.SetMenuState (GameEventManager.E_MenuState.e_menuUp);
		}		
	}

	public void ToggleCrafting ()
	{
		toggleCrafting = !toggleCrafting;
	}
}
