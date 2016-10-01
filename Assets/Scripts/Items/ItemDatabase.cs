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
//int itemId, string itemName, int itemMaxAge, ItemType itemType, ItemTool itemTool, float itemHardness, bool isItemHandMined, float itemExperience, int itemDrops, int itemDropRateMin, int itemDropRateMax
		items = new Item[23];
		//stones
		items [0] = new Item (-1, "Stone", -1, Item.ItemType.Mineral, Item.ItemTool.Pickaxe, 2, true, 0, 0, 1, 1);
		items [1] = new Item (1, "Flint", -1, Item.ItemType.RawMaterial, Item.ItemTool.Hand, 2, true, 0, 1, 1, 1);
		items [2] = new Item (2, "Stone", -1, Item.ItemType.RawMaterial, Item.ItemTool.Pickaxe, 2, false, 0, 2, 1, 1);
		items [3] = new Item (3, "Stone", -1, Item.ItemType.RawMaterial, Item.ItemTool.Pickaxe, 2, false, 0, 2, 1, 1);
		items [4] = new Item (4, "Stone", -1, Item.ItemType.RawMaterial, Item.ItemTool.Pickaxe, 2, false, 0, 2, 1, 1);
		//Grass
		items [5] = new Item (5, "Grass", -1, Item.ItemType.RawMaterial, Item.ItemTool.Hand, 0.5f, true, 0, 5, 1, 1);
		items [6] = new Item (6, "Grass", -1, Item.ItemType.RawMaterial, Item.ItemTool.Hand, 0.5f, true, 0, 5, 1, 1);
		items [7] = new Item (7, "Grass", -1, Item.ItemType.RawMaterial, Item.ItemTool.Hand, 0.5f, true, 0, 5, 1, 1);
		items [8] = new Item (8, "Grass", -1, Item.ItemType.RawMaterial, Item.ItemTool.Hand, 0.5f, true, 0, 5, 1, 1);
		items [9] = new Item (9, "Stick", -1, Item.ItemType.RawMaterial, Item.ItemTool.Hand, 0.5f, true, 0, 9, 1, 1);
		//Wood
		items [10] = new Item (10, "Log ", -1, Item.ItemType.RawMaterial, Item.ItemTool.Hand, 1, true, 0, 10, 1, 1);
		//Trees
		items [11] = new Item (11, "Tree", 14, Item.ItemType.Tree, Item.ItemTool.Axe, 12, false, 0, 10, 2, 3);
		items [12] = new Item (12, "Tree", 14, Item.ItemType.Tree, Item.ItemTool.Axe, 10, false, 0, 10, 2, 3);
		items [13] = new Item (13, "Tree", 14, Item.ItemType.Tree, Item.ItemTool.Axe, 5, false, 0, 10, 2, 3);
		items [14] = new Item (14, "Tree", 14, Item.ItemType.Tree, Item.ItemTool.Axe, 5, false, 0, 10, 2, 3);
		//Berries
		items [15] = new Item (15, "Berries", -1, Item.ItemType.Food, Item.ItemTool.Hand, 0.1f, true, 0, 15, 1, 1);
		//Berry Bushes
		items [16] = new Item (16, "Berry Bush", 8, Item.ItemType.Plant, Item.ItemTool.Hand, 0.1f, true, 0, 15, 1, 2);
		items [17] = new Item (17, "Berry Bush", 8, Item.ItemType.Plant, Item.ItemTool.Hand, 0.1f, true, 0, 15, 1, 2);
		items [18] = new Item (18, "Berry Bush", 8, Item.ItemType.Plant, Item.ItemTool.Hand, 0.1f, true, 0, 15, 1, 2);
		items [19] = new Item (19, "Berry Bush", 8, Item.ItemType.Plant, Item.ItemTool.Hand, 0.1f, true, 0, 15, 1, 2);
		//Radish
		items [20] = new Item (20, "Radish", -1, Item.ItemType.Food, Item.ItemTool.Hand, 0.1f, true, 0, 20, 1, 3);
		//Radish Plant
		items [21] = new Item (21, "Radish Plant", 5, Item.ItemType.RawMaterial, Item.ItemTool.Hand, 0.1f, true, 0, 20, 1, 2);
		items [22] = new Item (22, "Star fish", -1, Item.ItemType.Food, Item.ItemTool.Hand, 0.5f, true, 0, 22, 1, 1);
	}
}
