using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class MoreInventoryButton : MonoBehaviour
{
	public static MoreInventoryButton m_instance = null;
	public bool useLeftAnalogStick = false;
	public RectTransform mainUIWindow;
	public GameObject mainCanvas, inventoryMenu;
	public GameObject leftStick, inventoryTab, craftingTab, settingsTab, infoTab, sortButton, runWalkButton, actionButton, inventoryUpButton, craftingUpButton, miniMapButton, closeInventoryButton;
	//public rightStick;
	public GameObject craftingMenu;

	private bool toggleCrafting = false;

	float heightAdjuster;
	int tabIndex = 0;

	// = FindObjectsOfType (typeof(Devdog.InventorySystem.InventoryUIItemWrapper)) as Devdog.InventorySystem.InventoryUIItemWrapper[];
	void Awake ()
	{
		m_instance = this;
	}

	void Start ()
	{		
		tabIndex = inventoryMenu.GetComponent <RectTransform> ().GetSiblingIndex ();
		heightAdjuster = ((mainCanvas.GetComponent <RectTransform> ().rect.height / 2) + 200) * -1;
		//heightAdjuster = (mainUIWindow.GetComponent <RectTransform> ().rect.height + 120);
		ToggleInventorySize (true);
	}

	public void ToggleInventorySize (bool isInventoryDown)
	{		
		ToggleInventory (isInventoryDown);

		if (isInventoryDown) {
			inventoryMenu.GetComponent <RectTransform> ().SetSiblingIndex (tabIndex);
			GameEventManager.SetState (GameEventManager.E_STATES.e_game);
			mainUIWindow.anchoredPosition = new Vector3 (mainUIWindow.anchoredPosition.x, heightAdjuster);
			//mainUIWindow.anchoredPosition = Vector3.zero;
			GameEventManager.SetMenuState (GameEventManager.E_MenuState.e_menuDown);
		} else {
			inventoryMenu.GetComponent <RectTransform> ().SetSiblingIndex (tabIndex); 
			GameEventManager.SetState (GameEventManager.E_STATES.e_pause);
			//mainUIWindow.anchoredPosition = new Vector3 (mainUIWindow.anchoredPosition.x, heightAdjuster);
			mainUIWindow.anchoredPosition = Vector3.zero;
			GameEventManager.SetMenuState (GameEventManager.E_MenuState.e_menuUp);
		}		
	}

	public void ShowCraftingMenu (bool isCraftingDown)
	{
		ToggleInventory (isCraftingDown);
		if (isCraftingDown) {
			craftingMenu.GetComponent <RectTransform> ().SetSiblingIndex (tabIndex);
			GameEventManager.SetState (GameEventManager.E_STATES.e_game);
			mainUIWindow.anchoredPosition = new Vector3 (mainUIWindow.anchoredPosition.x, heightAdjuster);
			GameEventManager.SetMenuState (GameEventManager.E_MenuState.e_menuDown);
		} else {
			craftingMenu.GetComponent <RectTransform> ().SetSiblingIndex (tabIndex); 
			GameEventManager.SetState (GameEventManager.E_STATES.e_pause);
			mainUIWindow.anchoredPosition = Vector3.zero;
			GameEventManager.SetMenuState (GameEventManager.E_MenuState.e_menuUp);
		}		
	}

	void ToggleInventory (bool flag)
	{
		//if (Bronz.LocalStore.Instance.GetBool ("TouchControls")) {
		leftStick.SetActive (flag);
		//}
		closeInventoryButton.SetActive (!flag);
		inventoryTab.SetActive (!flag);
		sortButton.SetActive (!flag);
		craftingTab.SetActive (!flag);
		settingsTab.SetActive (!flag);
		infoTab.SetActive (!flag);
		runWalkButton.SetActive (flag);
		actionButton.SetActive (flag);
		inventoryUpButton.SetActive (flag);
		craftingUpButton.SetActive (flag);
		miniMapButton.SetActive (flag);
	}

	public void ToggleCrafting ()
	{
		toggleCrafting = !toggleCrafting;
	}

	IEnumerator SaveToPNG ()
	{
		yield return new WaitForEndOfFrame ();
		Texture2D screenTexture = new Texture2D (Screen.width, Screen.height, TextureFormat.RGB24, true);
		screenTexture.ReadPixels (new Rect (0f, 0f, Screen.width, Screen.height), 0, 0);
		screenTexture.Apply ();
		byte[] dataToSave = screenTexture.EncodeToPNG ();
		string destination = Path.Combine (Application.persistentDataPath, System.DateTime.Now.ToString ("yyyy-MM-dd-HHmmss") + ".png");
		File.WriteAllBytes (destination, dataToSave);
	}
}
