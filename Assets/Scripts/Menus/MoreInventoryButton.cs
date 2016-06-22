using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MoreInventoryButton : MonoBehaviour
{
	public RectTransform mainUIWindow;
	public GameObject mainCanvas, inventoryMenu;
	public GameObject leftStick, rightStick, pauseButton, actionButton, inventoryTab, craftingTab, settingsTab, infoTab;
	public GameObject craftingMenu;
	private bool toggleInventory = false, toggleCrafting = false;
	float heightAdjuster;
	int tabIndex = 0;

	void Start ()
	{
		tabIndex = inventoryMenu.GetComponent <RectTransform> ().GetSiblingIndex ();
		heightAdjuster = ((mainCanvas.GetComponent <RectTransform> ().rect.height / 2) + 200) * -1;
		print (mainCanvas.GetComponent <RectTransform> ().rect);
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
		pauseButton.SetActive (toggleInventory);
		inventoryTab.SetActive (!toggleInventory);
		craftingTab.SetActive (!toggleInventory);
		settingsTab.SetActive (!toggleInventory);
		infoTab.SetActive (!toggleInventory);
		if (toggleInventory) {
			inventoryMenu.GetComponent <RectTransform> ().SetSiblingIndex (tabIndex);
			GameEventManager.SetState (GameEventManager.E_STATES.e_game);
			mainUIWindow.anchoredPosition = new Vector3 (mainUIWindow.anchoredPosition.x, heightAdjuster);
		} else {
			inventoryMenu.GetComponent <RectTransform> ().SetSiblingIndex (tabIndex); 
			GameEventManager.SetState (GameEventManager.E_STATES.e_pause);
			mainUIWindow.anchoredPosition = Vector3.zero;
		}		
	}

	public void ToggleCrafting ()
	{
		toggleCrafting = !toggleCrafting;
	}
}
