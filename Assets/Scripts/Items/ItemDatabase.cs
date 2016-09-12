using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemDatabase : MonoBehaviour
{
	public static ItemDatabase m_instance = null;
	//public List<Item> items = new List<Item> ();
	public Item[] items;

	void Awake ()
	{
		m_instance = this;
		Initialize ();
	}
	
	// Update is called once per frame
	void Initialize ()
	{
		items = new Item[23];
		//stones
		items [0] = new Item (-1, "Stone", -1, Item.ItemType.RawMaterial, Item.ItemTool.Pickaxe, 2, true, 0, 1, 1, 1);
		items [1] = new Item (1, "Stone", -1, Item.ItemType.RawMaterial, Item.ItemTool.Pickaxe, 2, true, 0, 1, 1, 1);
		items [2] = new Item (2, "Stone", -1, Item.ItemType.RawMaterial, Item.ItemTool.Pickaxe, 2, true, 0, 1, 1, 1);
		items [3] = new Item (3, "Stone", -1, Item.ItemType.RawMaterial, Item.ItemTool.Pickaxe, 2, true, 0, 1, 1, 1);
		items [4] = new Item (4, "Stone", -1, Item.ItemType.RawMaterial, Item.ItemTool.Pickaxe, 2, true, 0, 1, 1, 1);
		//Grass
		items [5] = new Item (5, "Grass", -1, Item.ItemType.RawMaterial, Item.ItemTool.Axe, 0.5f, true, 0, 5, 1, 1);
		items [6] = new Item (6, "Grass", -1, Item.ItemType.RawMaterial, Item.ItemTool.Axe, 0.5f, true, 0, 1, 1, 1);
		items [7] = new Item (7, "Grass", -1, Item.ItemType.RawMaterial, Item.ItemTool.Axe, 0.5f, true, 0, 1, 1, 1);
		items [8] = new Item (8, "Grass", -1, Item.ItemType.RawMaterial, Item.ItemTool.Axe, 0.5f, true, 0, 1, 1, 1);
		items [9] = new Item (9, "Grass", -1, Item.ItemType.RawMaterial, Item.ItemTool.Axe, 0.5f, true, 0, 1, 1, 1);
		//Wood
		items [10] = new Item (10, "Wood", -1, Item.ItemType.RawMaterial, Item.ItemTool.Axe, 1, true, 0, 10, 1, 1);
		//Trees
		items [11] = new Item (11, "Tree", 14, Item.ItemType.RawMaterial, Item.ItemTool.Axe, 5, false, 0, 10, 4, 6);
		items [12] = new Item (12, "Tree", 14, Item.ItemType.RawMaterial, Item.ItemTool.Axe, 5, false, 0, 10, 4, 6);
		items [13] = new Item (13, "Tree", 14, Item.ItemType.RawMaterial, Item.ItemTool.Axe, 5, false, 0, 10, 4, 6);
		items [14] = new Item (14, "Tree", 14, Item.ItemType.RawMaterial, Item.ItemTool.Axe, 5, false, 0, 10, 4, 6);
		//Berries
		items [15] = new Item (15, "Berries", -1, Item.ItemType.RawMaterial, Item.ItemTool.Axe, 0.1f, true, 0, 15, 1, 1);
		//Berry Bushes
		items [16] = new Item (16, "Berry Bush", 8, Item.ItemType.RawMaterial, Item.ItemTool.Axe, 0.1f, true, 0, 15, 1, 3);
		items [17] = new Item (17, "Berry Bush", 8, Item.ItemType.RawMaterial, Item.ItemTool.Axe, 0.1f, true, 0, 15, 1, 3);
		items [18] = new Item (18, "Berry Bush", 8, Item.ItemType.RawMaterial, Item.ItemTool.Axe, 0.1f, true, 0, 15, 1, 3);
		items [19] = new Item (19, "Berry Bush", 8, Item.ItemType.RawMaterial, Item.ItemTool.Axe, 0.1f, true, 0, 15, 1, 3);
		//Radish
		items [20] = new Item (20, "Radish", -1, Item.ItemType.RawMaterial, Item.ItemTool.Axe, 0.1f, true, 0, 20, 1, 3);
		//Radish Plant
		items [21] = new Item (21, "Radish Plant", 5, Item.ItemType.RawMaterial, Item.ItemTool.Axe, 0.1f, true, 0, 20, 1, 3);
		items [22] = new Item (22, "Radish Plant", 5, Item.ItemType.RawMaterial, Item.ItemTool.Axe, 0.1f, true, 0, 20, 1, 3);
	}
}
