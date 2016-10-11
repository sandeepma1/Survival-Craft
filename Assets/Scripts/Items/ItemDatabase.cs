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
		items = new Item[100];
		//stones
		items [0] = new Item (-1, "Stone", -1, -1, Item.ItemType.Mineral, Item.ItemTool.Pickaxe, 2, true, 0, 0, 1, 1);
		items [1] = new Item (1, "Flint", -1, -1, Item.ItemType.RawMaterial, Item.ItemTool.Hand, 0.2f, true, 0, 2, 1, 1);
		items [2] = new Item (2, "Stone", -1, -1, Item.ItemType.RawMaterial, Item.ItemTool.Pickaxe, 2, false, 0, 3, 1, 1);
		items [3] = new Item (3, "Stone", -1, -1, Item.ItemType.RawMaterial, Item.ItemTool.Pickaxe, 2, false, 0, 3, 1, 1);
		items [4] = new Item (4, "Stone", -1, -1, Item.ItemType.RawMaterial, Item.ItemTool.Pickaxe, 2, false, 0, 3, 1, 1);
		//Grass
		items [5] = new Item (5, "Grass", -1, -1, Item.ItemType.RawMaterial, Item.ItemTool.Hand, 0.2f, true, 0, 1, 1, 1);
		items [6] = new Item (6, "Grass", -1, -1, Item.ItemType.RawMaterial, Item.ItemTool.Hand, 0.2f, true, 0, 1, 1, 1);
		items [7] = new Item (7, "Grass", -1, -1, Item.ItemType.RawMaterial, Item.ItemTool.Hand, 0.2f, true, 0, 1, 1, 1);
		items [8] = new Item (8, "Grass", -1, -1, Item.ItemType.RawMaterial, Item.ItemTool.Hand, 0.2f, true, 0, 1, 1, 1);
		items [9] = new Item (9, "Stick", -1, -1, Item.ItemType.RawMaterial, Item.ItemTool.Hand, 0.2f, true, 0, 8, 1, 1);
		//Wood
		items [10] = new Item (10, "Log ", -1, -1, Item.ItemType.RawMaterial, Item.ItemTool.Hand, 1, true, 0, 5, 1, 1);
		//Trees
		items [11] = new Item (11, "Stump Tree", 1, -1, Item.ItemType.Tree, Item.ItemTool.Shovel, 1, false, 0, 5, 1, 1);
		items [12] = new Item (12, "Small Tree", 2, 13, Item.ItemType.Tree, Item.ItemTool.Axe, 2, false, 0, 5, 1, 1);
		items [13] = new Item (13, "Medium Tree", 2, 14, Item.ItemType.Tree, Item.ItemTool.Axe, 5, false, 0, 5, 1, 2);
		items [14] = new Item (14, "Large Tree", 2, 15, Item.ItemType.Tree, Item.ItemTool.Axe, 10, false, 0, 5, 2, 3);
		items [15] = new Item (15, "Mature Tree", 2, -1, Item.ItemType.Tree, Item.ItemTool.Axe, 12, false, 0, 5, 2, 3);
		//Berries
		items [16] = new Item (16, "Berries", -1, -1, Item.ItemType.Food, Item.ItemTool.Hand, 0.1f, true, 0, 0, 1, 1);
		//Berry Bushes
		items [17] = new Item (17, "Berry Bush Small", 8, -1, Item.ItemType.Plant, Item.ItemTool.Hand, 0.2f, true, 0, 0, 1, 2);
		items [18] = new Item (18, "Berry Bush Mature", 8, -1, Item.ItemType.Plant, Item.ItemTool.Hand, 0.2f, true, 0, 0, 1, 2);
		items [19] = new Item (19, "Berry Bush", 8, -1, Item.ItemType.Plant, Item.ItemTool.Hand, 0.2f, true, 0, 0, 1, 2);
		items [20] = new Item (20, "Berry Bush", 8, -1, Item.ItemType.Plant, Item.ItemTool.Hand, 0.2f, true, 0, 0, 1, 2);
		//Radish
		items [21] = new Item (21, "Radish", -1, -1, Item.ItemType.Food, Item.ItemTool.Hand, 0.1f, true, 0, 7, 1, 3);
		//Radish Plant
		items [22] = new Item (22, "Radish Plant", 5, -1, Item.ItemType.RawMaterial, Item.ItemTool.Hand, 0.1f, true, 0, 7, 1, 2);
		items [23] = new Item (23, "Star fish", -1, -1, Item.ItemType.Food, Item.ItemTool.Hand, 0.5f, true, 0, 6, 1, 1);
		items [24] = new Item (24, "Star fish", -1, -1, Item.ItemType.Food, Item.ItemTool.Hand, 0.5f, true, 0, 6, 1, 1); // not used
		items [25] = new Item (25, "Campfire", -1, -1, Item.ItemType.Build, Item.ItemTool.Hammer, 2f, false, 0, -1, 0, 0);
	}
}
