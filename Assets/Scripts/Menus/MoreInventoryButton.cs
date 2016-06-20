using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MoreInventoryButton : MonoBehaviour
{

	public RectTransform inventoryMenu;
	public GameObject leftStick, rightStick, pauseButton, actionButton;
	private bool toggleInventory = false;

	public void ToggleInventorySize ()
	{
		toggleInventory = !toggleInventory;				
		isInventoryVisible (!toggleInventory);		
	}

	void isInventoryVisible (bool flag)
	{
		leftStick.SetActive (flag);
		rightStick.SetActive (flag);
		pauseButton.SetActive (flag);
		if (flag) {
			GameEventManager.SetState (GameEventManager.E_STATES.e_game);
			inventoryMenu.anchoredPosition = new Vector2 (inventoryMenu.anchoredPosition.x, -50);
		} else {
			GameEventManager.SetState (GameEventManager.E_STATES.e_pause);
			inventoryMenu.localPosition = new Vector2 (inventoryMenu.localPosition.x, 100);
		}
	}
}
