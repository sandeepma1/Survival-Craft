﻿using UnityEngine;
using System.Collections;

public class Item
{
	public int id;
	public ItemType type;
	public ItemTool tool;
	public string name;
	public float hardness;
	public bool isHandMined;
	public float experience;
	public int drops;
	public int dropRateMin;
	public int dropRateMax;
	//public bool isPlacabel;
	//public bool flammable;
	//public int blastResistance
	//public float itemDegrade;
	//public bool isDegradeable;
	//public bool isCurrentlyDropped;
	//public int itemQuality;


	public Item (int itemId, string itemName, ItemType itemType, ItemTool itemTool, float itemHardness, bool isItemHandMined, float itemExperience, int itemDrops, int itemDropRateMin, int itemDropRateMax)
	{
		id = itemId;
		type = itemType;
		tool = itemTool;
		name = itemName;
		hardness = itemHardness;
		isHandMined = isItemHandMined;
		experience = itemExperience;
		drops = itemDrops;
		dropRateMin = itemDropRateMin;
		dropRateMax = itemDropRateMax;
	}

	public Item ()
	{

	}

	public enum ItemType
	{
		RawMaterial,
		Ore,
		Miniral
		/*Consumable,
		Quest,
		Survival,
		Build,
		Head,
		Hands,
		Shoes,
		Chest,
		Trousers,
		Necklace,
		Rings*/
	}

	public enum ItemTool
	{
		Axe,
		Pickaxe,
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