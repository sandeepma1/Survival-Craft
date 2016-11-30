using UnityEngine;
using System.Collections;

public class ItemPlacer : MonoBehaviour
{
	public static ItemPlacer m_instance = null;
	public GameObject itemPlacer, itemPlacerButtons, inventoryMenu;
	private Vector3 screenPoint;
	private float markerOffset = 3;
	bool isItemPlacable = false;

	void Awake ()
	{
		m_instance = this;
		itemPlacerButtons.SetActive (false);
	}

	void OnMouseDrag ()
	{
		Vector3 cursorPosition = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0));
		itemPlacer.transform.position = new Vector3 ((int)cursorPosition.x, (int)cursorPosition.y + markerOffset, 0);
		CheckForItemPlacement ();
	}

	public void PlaceItemByButton ()
	{
		//print (ActionManager.m_AC_instance.currentWeildedItem.itemID);
		MoreInventoryButton.m_instance.ToggleInventorySize (false);
		itemPlacer.GetComponent <SpriteRenderer> ().sprite = 
		LoadMapFromSave_PG.m_instance.items [ActionManager.m_AC_instance.currentWeildedItem.itemID].gameObject.GetComponent <SpriteRenderer> ().sprite;
		HideAllHUD (true);
		itemPlacer.transform.position = new Vector3 (PlayerMovement.m_instance.gameObject.transform.position.x, PlayerMovement.m_instance.gameObject.transform.position.y - 1, 0);
		CheckForItemPlacement ();
	}

	public void ConfirmPlaceItem ()
	{
		if (isItemPlacable) {
			LoadMapFromSave_PG.m_instance.InstantiatePlacedObject (LoadMapFromSave_PG.m_instance.items [ActionManager.m_AC_instance.currentWeildedItem.itemID].gameObject,
				itemPlacer.transform.position, LoadMapFromSave_PG.m_instance.mapChunks [Bronz.LocalStore.Instance.GetInt ("mapChunkPosition")].transform, Bronz.LocalStore.Instance.GetInt ("mapChunkPosition"),
				ActionManager.m_AC_instance.currentWeildedItem.itemID, -1);
			HideAllHUD (false);
		}

	}

	public void CancelPlaceItem ()
	{
		HideAllHUD (false);
	}

	void HideAllHUD (bool flag)
	{
		itemPlacer.SetActive (flag);
		inventoryMenu.SetActive (!flag);
		MoreInventoryButton.m_instance.ToggleInventorySize (!flag);
		itemPlacerButtons.SetActive (flag);
		//MoreInventoryButton.m_instance.rightStick.SetActive (!flag);
		//MoreInventoryButton.m_instance.leftStick.SetActive (!flag);
		CircleCollider2D[] cols = PlayerMovement.m_instance.GetComponents <CircleCollider2D> ();
		foreach (var item in cols) {
			item.enabled = !flag;
		}
	}

	void CheckForItemPlacement ()
	{
		if (LoadMapFromSave_PG.m_instance.CheckForItemPlacement (itemPlacer.transform.position)) {
			itemPlacer.GetComponent <SpriteRenderer> ().color = Color.green;
			isItemPlacable = true;
		} else {
			itemPlacer.GetComponent <SpriteRenderer> ().color = Color.red;
			isItemPlacable = false;
		}
	}
}



/*void LateUpdate ()
	{
		if (Input.GetMouseButtonDown (2)) {
			Vector2 rayPos = new Vector2 (Camera.main.ScreenToWorldPoint (Input.mousePosition).x, Camera.main.ScreenToWorldPoint (Input.mousePosition).y);
			RaycastHit2D hit = Physics2D.Raycast (rayPos, Vector2.zero, 0f);         
			if (hit) {
				if (hit.transform.name == "Player") {
					itemPlacer.transform.position = new Vector3 ((int)hit.point.x, (int)hit.point.y);
					print (itemPlacer.transform.position);
					if (LoadMapFromSave_PG.m_instance.CheckForItemPlacement (itemPlacer.transform.position)) {
						itemPlacer.GetComponent <SpriteRenderer> ().color = Color.green;
					} else {
						itemPlacer.GetComponent <SpriteRenderer> ().color = Color.red;
					}
				}
			}
		}

	}*/

/*void Update ()
	{
		if (Input.GetMouseButton (0)) {
			Vector2 rayPos = new Vector2 (Camera.main.ScreenToWorldPoint (Input.mousePosition).x, Camera.main.ScreenToWorldPoint (Input.mousePosition).y);
			RaycastHit2D hit = Physics2D.Raycast (rayPos, Vector2.zero, 0f);         
			if (hit) {
				if (hit.transform.name == "Player") {
					startDragging = true;
				} else {
					startDragging = false;
				}
			}
		}	
	}*/

/*GameObject ClickSelect ()
	{
		Vector2 rayPos = new Vector2 (Camera.main.ScreenToWorldPoint (Input.mousePosition).x, Camera.main.ScreenToWorldPoint (Input.mousePosition).y);
		RaycastHit2D hit = Physics2D.Raycast (rayPos, Vector2.zero, 0f);
         
		if (hit) {
			if (hit.transform.name == "Player") {
				itemPlacer.transform.position = hit.point;
				movemarkerSelected = true;
			}
			return hit.transform.gameObject;
		} else
			return null;
	}*/