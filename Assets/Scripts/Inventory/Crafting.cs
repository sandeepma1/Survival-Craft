using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crafting : MonoBehaviour
{
	public static Crafting m_instance = null;
	public GameObject craftSlot;
	public Image slotSelectedImage;
	public GameObject miniCraftPanel, mainCraftPanel;
	public int miniCraftAmount, mainCraftAmount;
	public int[] craftingItems;
	public List<GameObject> craftingSlotsGO = new List<GameObject> ();

	public int selectedItemID = -1;

	void Awake ()
	{
		m_instance = this;
	}

	void Start ()
	{		
		for (int i = 0; i < miniCraftAmount; i++) {		
			craftingSlotsGO.Add (Instantiate (craftSlot, miniCraftPanel.transform));
			craftingSlotsGO [i].GetComponent <CraftingSlot> ().id = i;
			craftingSlotsGO [i].GetComponent <CraftingSlot> ().itemID = craftingItems [i];
			craftingSlotsGO [i].GetComponent <RectTransform> ().localScale = Vector3.one;
			craftingSlotsGO [i].transform.GetChild (0).GetComponent<Image> ().sprite = ItemDatabase.m_instance.items [craftingItems [i]].Sprite;
		}
		for (int i = miniCraftAmount; i < miniCraftAmount + mainCraftAmount; i++) {		
			craftingSlotsGO.Add (Instantiate (craftSlot, mainCraftPanel.transform));
			craftingSlotsGO [i].GetComponent <CraftingSlot> ().id = i;
			craftingSlotsGO [i].GetComponent <CraftingSlot> ().itemID = craftingItems [i];
			craftingSlotsGO [i].GetComponent <RectTransform> ().localScale = Vector3.one;
			craftingSlotsGO [i].transform.GetChild (0).GetComponent<Image> ().sprite = ItemDatabase.m_instance.items [craftingItems [i]].Sprite;
		}
	}

	public void CheckHighlight_ALL_CraftableItems ()
	{
		for (int i = 0; i < craftingItems.Length; i++) {		
			if (craftingSlotsGO [i].transform.childCount > 0) {		
				if (CheckForRequiredItemsInInventory (craftingItems [i])) {
					craftingSlotsGO [i].transform.GetChild (0).GetComponent<Image> ().color = new Color (1, 1, 1, 1);
				} else {
					craftingSlotsGO [i].transform.GetChild (0).GetComponent<Image> ().color = new Color (0, 0, 0, 1);
				}
			}
		}
	}

	public void CraftSelectedItem ()
	{
		if (selectedItemID >= 0) {			
			if (CheckForRequiredItemsInInventory (selectedItemID) && Inventory.m_instance.CheckInventoryHasAtleastOneSpace ()) { //if inventory has all the craftable items
				RemoveItemsToCreateNewItem ();
				Inventory.m_instance.AddItem (selectedItemID);
				print ("crafted item" + selectedItemID);
			} else {
				print ("missing Items or inventory full");
			}
		}
	}

	void RemoveItemsToCreateNewItem ()
	{
		Item itemToCraft = ItemDatabase.m_instance.FetchItemByID (selectedItemID);

		if (itemToCraft.ItemID1 >= 0) {
			for (int i = 0; i < itemToCraft.ItemAmount1; i++) {
				Inventory.m_instance.RemoveItem (itemToCraft.ItemID1);
				print ("removed " + itemToCraft.ItemID1);
			}
		}
		if (itemToCraft.ItemID2 >= 0) {
			for (int i = 0; i < itemToCraft.ItemAmount2; i++) {
				Inventory.m_instance.RemoveItem (itemToCraft.ItemID2);		
				print ("removed " + itemToCraft.ItemID2);	
			}
		}
		if (itemToCraft.ItemID3 >= 0) {
			for (int i = 0; i < itemToCraft.ItemAmount3; i++) {
				Inventory.m_instance.RemoveItem (itemToCraft.ItemID3);
			}
		}
		if (itemToCraft.ItemID4 >= 0) {
			for (int i = 0; i < itemToCraft.ItemAmount4; i++) {
				Inventory.m_instance.RemoveItem (itemToCraft.ItemID4);
			}
		}
	}

	bool CheckForRequiredItemsInInventory (int id)
	{
		Item itemToCraft = ItemDatabase.m_instance.FetchItemByID (id);
		bool item1 = false;
		bool item2 = false;
		bool item3 = false;
		bool item4 = false;

		if (itemToCraft.ItemID1 >= -1 && Inventory.m_instance.CheckItemAmountInInventory (itemToCraft.ItemID1) >= itemToCraft.ItemAmount1) {
			//print (id + " 1 " + Inventory.m_instance.CheckItemAmountInInventory (itemToCraft.ItemID1) + " " + itemToCraft.ItemAmount1);			
			item1 = true;			
		}
		if (itemToCraft.ItemID2 >= -1 && Inventory.m_instance.CheckItemAmountInInventory (itemToCraft.ItemID2) >= itemToCraft.ItemAmount2) {
//			print (id + " 2 " + Inventory.m_instance.CheckItemAmountInInventory (itemToCraft.ItemID2) + " " + itemToCraft.ItemAmount2);			
			item2 = true;			
		}
		if (itemToCraft.ItemID3 >= -1 && Inventory.m_instance.CheckItemAmountInInventory (itemToCraft.ItemID3) >= itemToCraft.ItemAmount3) {
			//print (id + " 3 " + Inventory.m_instance.CheckItemAmountInInventory (itemToCraft.ItemID3) + " " + itemToCraft.ItemAmount3);			
			item3 = true;
		} 
		if (itemToCraft.ItemID4 >= -1 && Inventory.m_instance.CheckItemAmountInInventory (itemToCraft.ItemID4) >= itemToCraft.ItemAmount4) {
			//print (id + " 4 " + Inventory.m_instance.CheckItemAmountInInventory (itemToCraft.ItemID4) + " " + itemToCraft.ItemAmount4);			
			item4 = true;
		}

		//print (id + "" + item1 + " " + item2 + " " + item3 + " " + item4);
		if (item1 && item2 && item3 && item4) {
			return true;
		} else {
			return false;
		}
	}
}
