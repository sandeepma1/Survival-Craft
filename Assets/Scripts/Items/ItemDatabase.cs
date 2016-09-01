using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemDatabase : MonoBehaviour
{
	public static ItemDatabase m_instance = null;
	//public List<Item> items = new List<Item> ();
	public Item[] items;

	void Start ()
	{
		m_instance = this;
		Initialize ();
	}
	
	// Update is called once per frame
	void Initialize ()
	{
		items = new Item[12];
		items [0] = new Item (0, "Stone", -1, Item.ItemType.RawMaterial, Item.ItemTool.Pickaxe, 2, true, 0, 1, 1, 1);
		items [1] = new Item (1, "Stone", -1, Item.ItemType.RawMaterial, Item.ItemTool.Pickaxe, 1, true, 0, 1, 1, 1);
		items [2] = new Item (2, "Stone", -1, Item.ItemType.RawMaterial, Item.ItemTool.Pickaxe, 1, true, 0, 1, 1, 1);
		items [3] = new Item (3, "Stone", -1, Item.ItemType.RawMaterial, Item.ItemTool.Pickaxe, 5, true, 0, 1, 1, 1);
		items [4] = new Item (4, "Stone", -1, Item.ItemType.RawMaterial, Item.ItemTool.Pickaxe, 5, true, 0, 1, 1, 1);
		items [5] = new Item (5, "Grass", 6, Item.ItemType.RawMaterial, Item.ItemTool.Axe, 0.5f, true, 0, 5, 1, 1);
		items [6] = new Item (6, "Stone", -1, Item.ItemType.RawMaterial, Item.ItemTool.Pickaxe, 5, true, 0, 1, 1, 1);
		items [7] = new Item (7, "Stone", -1, Item.ItemType.RawMaterial, Item.ItemTool.Pickaxe, 5, true, 0, 1, 1, 1);
		items [8] = new Item (8, "Stone", -1, Item.ItemType.RawMaterial, Item.ItemTool.Pickaxe, 5, true, 0, 1, 1, 1);
		items [9] = new Item (9, "Stone", -1, Item.ItemType.RawMaterial, Item.ItemTool.Pickaxe, 5, true, 0, 1, 1, 1);
		items [10] = new Item (10, "Wood", -1, Item.ItemType.RawMaterial, Item.ItemTool.Axe, 1, true, 0, 10, 1, 1);
		items [11] = new Item (11, "Tree", 14, Item.ItemType.RawMaterial, Item.ItemTool.Axe, 1, true, 0, 10, 4, 6);
	}
}
