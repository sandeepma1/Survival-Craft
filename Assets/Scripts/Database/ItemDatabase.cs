using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;

public class ItemDatabase : MonoBehaviour
{
	public static ItemDatabase m_instance = null;
	public List<Item> items = new List<Item> ();
	string fileName = "Itemsz";

	void Awake ()
	{
		m_instance = this;	
		ConstructItemDatabase ();
	}

	public Item FetchItemByID (int id)
	{
		for (int i = 0; i < items.Count; i++) {
			if (items [i].ID == id) {
				return items [i];			
			}
		}
		return null;
	}

	void ConstructItemDatabase ()
	{
		string[] lines = new string[100];
		string[] chars = new string[100];
		TextAsset itemCSV =	Resources.Load ("CSV/" + fileName) as TextAsset;
		lines = Regex.Split (itemCSV.text, "\r\n");
		for (int i = 1; i < lines.Length - 1; i++) {			
			chars = Regex.Split (lines [i], ",");

			items.Add (new Item (
				IntParse (chars [0]),
				chars [1],
				IntParse (chars [2]),
				IntParse (chars [3]),
				(ItemType)System.Enum.Parse (typeof(ItemType), chars [4]),
				(ItemTool)System.Enum.Parse (typeof(ItemTool), chars [5]),
				IntParse (chars [6]),
				FloatParse (chars [7]), 
				bool.Parse (chars [8]), 
				IntParse (chars [9]),
				IntParse (chars [10]), 
				IntParse (chars [11]), 
				IntParse (chars [12]), 
				IntParse (chars [13]), 
				IntParse (chars [14]),
				IntParse (chars [15]), 
				IntParse (chars [16]), 
				IntParse (chars [17]), 
				IntParse (chars [18]),
				chars [19],
				chars [20],
				IntParse (chars [21]),
				bool.Parse (chars [22]), 
				bool.Parse (chars [23]), 
				IntParse (chars [24]),
				IntParse (chars [25]),
				IntParse (chars [26]),
				IntParse (chars [27]),
				IntParse (chars [28]),
				IntParse (chars [29]),
				IntParse (chars [30]),
				IntParse (chars [31])
			));
		}
	}

	int IntParse (string text)
	{
		int num;
		if (int.TryParse (text, out num)) {
			return num;
		} else
			return 0;
	}

	float FloatParse (string text)
	{
		float result = 0.01f;
		float.TryParse (text, out result);
		return result;
	}
}

[System.Serializable]
public class Item
{
	public int ID{ get; set; }

	public string Name{ get; set; }

	public int MaxAge{ get; set; }

	public int NextStage{ get; set; }

	public ItemType Type{ get; set; }

	public ItemTool Tool{ get; set; }

	public int ReduceToolDurability{ get; set; }

	public float Hardness{ get; set; }

	public bool IsHandMined{ get; set; }

	public float Experience{ get; set; }

	public int Drops1{ get; set; }

	public int Drop1RateMin{ get; set; }

	public int Drop1RateMax{ get; set; }

	public int Drops2{ get; set; }

	public int Drop2RateMin{ get; set; }

	public int Drop2RateMax{ get; set; }

	public int Drops3{ get; set; }

	public int Drop3RateMin{ get; set; }

	public int Drop3RateMax{ get; set; }

	public string Slug{ get; set; }

	public string Description{ get; set; }

	public int Durability{ get; set; }

	public bool IsStackable{ get; set; }

	public bool IsPlaceable{ get; set; }

	public int ItemID1{ get; set; }

	public int ItemAmount1{ get; set; }

	public int ItemID2{ get; set; }

	public int ItemAmount2{ get; set; }

	public int ItemID3{ get; set; }

	public int ItemAmount3{ get; set; }

	public int ItemID4{ get; set; }

	public int ItemAmount4{ get; set; }

	public Sprite Sprite{ get; set; }

	//public bool isPlacabel;
	//public bool flammable;
	//public int blastResistance
	//public float itemDegrade;
	//public bool isDegradeable;
	//public bool isCurrentlyDropped;
	//public int itemQuality;
	public Item (int itemId, string itemName, int itemMaxAge, int itemNextStage, ItemType itemType, ItemTool itemTool, 
	             int itemReduceToolDurability, float itemHardness, bool isItemHandMined, float itemExperience, 
	             int itemDrops1, int itemDrop1RateMin, int itemDrop1RateMax, int itemDrops2, int itemDrop2RateMin, 
	             int itemDrop2RateMax, int itemDrops3, int itemDrop3RateMin, int itemDrop3RateMax, string slug,
	             string description, int durability, bool isStackable, bool isPlaceable, int itemID1, int itemAmount1, 
	             int itemID2, int itemAmount2, int itemID3, int itemAmount3, int itemID4, int itemAmount4)
	{
		ID = itemId;
		Name = itemName;
		MaxAge = itemMaxAge;
		NextStage = itemNextStage;
		Type = itemType;
		Tool = itemTool;
		ReduceToolDurability = itemReduceToolDurability;
		Hardness = itemHardness;
		IsHandMined = isItemHandMined;
		Experience = itemExperience;
		Drops1 = itemDrops1;
		Drop1RateMin = itemDrop1RateMin;
		Drop1RateMax = itemDrop1RateMax;
		Drops2 = itemDrops2;
		Drop2RateMin = itemDrop2RateMin;
		Drop2RateMax = itemDrop2RateMax;
		Drops3 = itemDrops3;
		Drop3RateMin = itemDrop3RateMin;
		Drop3RateMax = itemDrop3RateMax;
		Slug = slug;
		Description = description;
		Durability = durability;
		IsStackable = isStackable;
		IsPlaceable = isPlaceable;
		ItemID1 = ItemID1;
		ItemAmount1 = itemAmount1;
		ItemID2 = ItemID2;
		ItemAmount2 = itemAmount2;
		ItemID3 = ItemID3;
		ItemAmount3 = itemAmount3;
		ItemID4 = ItemID4;
		ItemAmount4 = itemAmount4;
		this.Sprite = Resources.Load <Sprite> ("Textures/Inventory/" + slug);
	}

	public Item ()
	{
		this.ID = -1;
	}
}

public enum ItemType
{
	Armor,
	Food,
	Plant,
	Tree,
	RawMaterial,
	Ore,
	Mineral,
	Tool,
	Weapon,
	Consumable,
	Quest,
	Build
}

public enum ItemTool
{
	None,
	Hand,
	Axe,
	Pickaxe,
	Hammer,
	Hoe,
	Shovel,
	FishingRod,
	Sword
}
