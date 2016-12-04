using UnityEngine;
using System.Collections;

public class Item
{
	public int id;
	public string name;
	public int maxAge;
	public int nextStage;
	public ItemType type;
	public ItemTool tool;
	public int reduceToolDurability;
	public float hardness;
	public bool isHandMined;
	public float experience;
	public int drops1;
	public int drop1RateMin;
	public int drop1RateMax;
	public int drops2;
	public int drop2RateMin;
	public int drop2RateMax;
	public int drops3;
	public int drop3RateMin;
	public int drop3RateMax;
	//public bool isPlacabel;
	//public bool flammable;
	//public int blastResistance
	//public float itemDegrade;
	//public bool isDegradeable;
	//public bool isCurrentlyDropped;
	//public int itemQuality;
	public Item (int itemId, string itemName, int itemMaxAge, int itemNextStage, ItemType itemType, ItemTool itemTool, int itemReduceToolDurability, float itemHardness, bool isItemHandMined, float itemExperience, 
	             int itemDrops1, int itemDrop1RateMin, int itemDrop1RateMax, int itemDrops2, int itemDrop2RateMin, int itemDrop2RateMax, int itemDrops3, int itemDrop3RateMin, int itemDrop3RateMax)
	{
		id = itemId;
		name = itemName;
		maxAge = itemMaxAge;
		nextStage = itemNextStage;
		type = itemType;
		tool = itemTool;
		reduceToolDurability = itemReduceToolDurability;
		hardness = itemHardness;
		isHandMined = isItemHandMined;
		experience = itemExperience;
		drops1 = itemDrops1;
		drop1RateMin = itemDrop1RateMin;
		drop1RateMax = itemDrop1RateMax;
		drops2 = itemDrops2;
		drop2RateMin = itemDrop2RateMin;
		drop2RateMax = itemDrop2RateMax;
		drops3 = itemDrops3;
		drop3RateMin = itemDrop3RateMin;
		drop3RateMax = itemDrop3RateMax;
	}

	public enum ItemType
	{
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
		Survival,
		Build,
		Head,
		Hands,
		Shoes,
		Chest,
		Trousers,
		Necklace,
		Rings
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
}

/*public static GameItemsArray m_instance = null;
	public ItemProperty[] item;*/

/*void Start ()
	{
		m_instance = this;
		InitializeItems ();
	}*/