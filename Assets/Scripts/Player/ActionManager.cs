using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ActionManager :MonoBehaviour
{
	public static ActionManager m_AC_instance = null;
	public GameObject weaponGameObject;
	SpriteRenderer weaponSprite;
	float baseTime = 0.0f;
	public bool isReadyToAttack = false;
	public Text debugText;

	Devdog.InventorySystem.InventoryItemBase currentWeildedItem, currentSelectedTile;

	void Awake ()
	{
		m_AC_instance = this;
		weaponSprite = weaponGameObject.GetComponent <SpriteRenderer> ();
		currentWeildedItem = new Devdog.InventorySystem.InventoryItemBase ();
		currentSelectedTile = new Devdog.InventorySystem.InventoryItemBase ();
	}

	public void ActionButtonPressed ()
	{
		GetCurrentTile ();
		CalculateHardness ();
	}

	void CalculateHardness ()
	{		
		if (currentSelectedTile != null && currentSelectedTile.isHandMined) {
			if (currentWeildedItem != null && currentWeildedItem.rarity.name == currentSelectedTile.rarity.name) { // if tool
				print ("Using Tools");				
				baseTime = (GameEventManager.baseStrengthWithTool * currentSelectedTile.properties [0].floatValue) / currentWeildedItem.itemQuality;			
				isReadyToAttack = true;
			} else { // if not tool
				print ("Using Bare Hands");
				baseTime = GameEventManager.baseStrengthWithoutTool * currentSelectedTile.properties [0].floatValue;
				isReadyToAttack = true;	
			}
		} else {
			print ("No items nearby");
		} 
	}

	void Update ()
	{
		if (isReadyToAttack) {
			baseTime -= Time.deltaTime;
			debugText.text = baseTime.ToString ("F");
			if (baseTime <= 0) {
				DropBreakedItem ();
				isReadyToAttack = false;
			}
		}
	}

	void DropBreakedItem ()
	{
		currentSelectedTile.GetComponent <Devdog.InventorySystem.ObjectTriggererItem> ().isPickable = true;
		currentSelectedTile.GetComponent <Devdog.InventorySystem.ObjectTriggererItem> ().Toggle (true);
		print (currentSelectedTile.transform.position);
		LoadMapFromSave.m_instance.SaveMapitemData ((int)currentSelectedTile.transform.position.x, (int)currentSelectedTile.transform.position.y);
		currentSelectedTile = null;
	}

	void GetCurrentTile ()
	{
		if (LoadMapFromSave.m_instance.GetTile (GameEventManager.currentSelectedTilePosition) != null) {		
			currentSelectedTile = LoadMapFromSave.m_instance.GetTile (GameEventManager.currentSelectedTilePosition).GetComponent <Devdog.InventorySystem.InventoryItemBase> ();
		} else {
			currentSelectedTile = null;
		}
	}

	public void GetCurrentWeildedTool (Devdog.InventorySystem.InventoryItemBase i)
	{
		currentWeildedItem = i;
	}
}

