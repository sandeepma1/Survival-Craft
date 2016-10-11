using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MoreInventoryButton : MonoBehaviour
{
	public static MoreInventoryButton m_instance = null;
	public RectTransform mainUIWindow;
	public GameObject mainCanvas, inventoryMenu;
	public GameObject leftStick, rightStick, inventoryTab, craftingTab, settingsTab, infoTab, sortButton, runWalkButton, actionButton;
	public GameObject craftingMenu;
	private bool toggleCrafting = false;
	float heightAdjuster;
	int tabIndex = 0;

	// = FindObjectsOfType (typeof(Devdog.InventorySystem.InventoryUIItemWrapper)) as Devdog.InventorySystem.InventoryUIItemWrapper[];

	void Start ()
	{
		
		tabIndex = inventoryMenu.GetComponent <RectTransform> ().GetSiblingIndex ();
		heightAdjuster = ((mainCanvas.GetComponent <RectTransform> ().rect.height / 2) + 200) * -1;
		ToggleInventorySize (true);
	}


	void Awake ()
	{
		m_instance = this;
	}


	public void ToggleInventorySize (bool isInventoryDown)
	{		
		//toggleInventory = !toggleInventory;				
		leftStick.SetActive (isInventoryDown);
		rightStick.SetActive (isInventoryDown);

		inventoryTab.SetActive (!isInventoryDown);
		sortButton.SetActive (!isInventoryDown);
		craftingTab.SetActive (!isInventoryDown);
		settingsTab.SetActive (!isInventoryDown);
		infoTab.SetActive (!isInventoryDown);
		runWalkButton.SetActive (isInventoryDown);
		actionButton.SetActive (isInventoryDown);

		if (isInventoryDown) {
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
