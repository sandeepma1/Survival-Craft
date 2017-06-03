using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItemData : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public MyItem item;
	public int amount = 1;
	public int durability = 0;
	public ItemType type;
	public int slotID;

	float durabilityPercentage = 0.0f;
	Vector2 offset = new Vector2 (100, 100);

	public void DecreaseItemDurability (int damage)
	{
		durability -= damage;
		durabilityPercentage = (durability * 1.0f / item.Durability * 1.0f) * 100;
		if (durability <= 0) {
			Inventory.m_instance.DeleteSelectedItem ();
		}
		transform.GetChild (1).GetComponent <RectTransform> ().sizeDelta = new Vector2 (durabilityPercentage * 0.9f, 10);
	}

	public void OnBeginDrag (PointerEventData eventData)
	{
		if (item != null) {
			offset = eventData.position - new Vector2 (this.transform.position.x, this.transform.position.y);
			this.transform.SetParent (this.transform.parent.parent);
			this.transform.position = eventData.position - offset;
			GetComponent <CanvasGroup> ().blocksRaycasts = false;
			//SelectedItem ();
		}
	}

	public void OnDrag (PointerEventData eventData)
	{
		if (item != null) {
			this.transform.position = eventData.position - offset;
		}
	}

	public void OnEndDrag (PointerEventData eventData)
	{		
		this.transform.SetParent (Inventory.m_instance.slotsGO [slotID].transform);
		this.transform.SetAsFirstSibling ();	
		this.transform.position = Inventory.m_instance.slotsGO [slotID].transform.position;		
		GetComponent <CanvasGroup> ().blocksRaycasts = true;
		Crafting.m_instance.CheckHighlight_ALL_CraftableItems ();
	}

	public void SelectedItem ()
	{
		//Inventory.m_instance.selectedItem = item;
		//Inventory.m_instance.PopulateItemInfoBox ();
	}
}
