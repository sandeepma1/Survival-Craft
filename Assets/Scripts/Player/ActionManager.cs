using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ActionManager :MonoBehaviour
{
	public static ActionManager m_AC_instance = null;
	public GameObject weaponGameObject, progressBar, progressBarBG;
	public bool isReadyToAttack = false;
	public GameObject[] inventoryItems;

	SpriteRenderer weaponSprite;
	float baseTime = 0.0f, baseTimeStatic, progressVal = 0;
	Devdog.InventorySystem.InventoryItemBase currentWeildedItem;
	item currentSelectedItem = new item ();

	void Awake ()
	{
		m_AC_instance = this;
		weaponSprite = weaponGameObject.GetComponent <SpriteRenderer> ();
		currentWeildedItem = new Devdog.InventorySystem.InventoryItemBase ();
		//	print (Devdog.InventorySystem.InventoryUIItemWrapper.m_instance.OnPointerUp ());
		progressBarBG.SetActive (false);
	}

	public void GetCurrentWeildedTool (Devdog.InventorySystem.InventoryItemBase i)
	{
		currentWeildedItem = i;
	}

	public void ActionButtonPressed ()
	{
		GetCurrentTile ();
	}

	void GetCurrentTile ()
	{
		if (MapLoader.m_instance.GetTile (GameEventManager.currentSelectedTilePosition).id > 0) {		
			currentSelectedItem = MapLoader.m_instance.GetTile (GameEventManager.currentSelectedTilePosition);
			PlayerMovement.m_instance.AttackCalculation ();
			PlayerMovement.m_instance.SetAttackAnimation (true);
			CalculateHardness ();
		} else {
			PlayerMovement.m_instance.AttackCalculation ();
			PlayerMovement.m_instance.SetAttackAnimation (false);
			print ("No items nearby");
			currentSelectedItem = new item ();
		}
	}

	void CalculateHardness ()
	{		
		if (currentSelectedItem.id > 0 && ItemDatabase.m_instance.items [currentSelectedItem.id].isHandMined) {
			if (currentWeildedItem != null && currentWeildedItem.rarity.name == ItemDatabase.m_instance.items [currentSelectedItem.id].tool.ToString ()) { // if tool
				print ("Using Tools");				
				baseTime = (GameEventManager.baseStrengthWithTool * ItemDatabase.m_instance.items [currentSelectedItem.id].hardness) / currentWeildedItem.itemQuality;
				baseTimeStatic = baseTime;		
				isReadyToAttack = true;
			} else { // if not tool
				print ("Using Bare Hands");
				baseTime = GameEventManager.baseStrengthWithoutTool * ItemDatabase.m_instance.items [currentSelectedItem.id].hardness;
				baseTimeStatic = baseTime;
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
			progressVal = baseTime / baseTimeStatic;

			progressBar.transform.localScale = new Vector3 (progressVal, 0.1f, 1);
			progressBarBG.SetActive (true);
			if (baseTime <= 0) {
				if (ItemDatabase.m_instance.items [currentSelectedItem.id].isHandMined) { // if object ishandmined then drop items
					DropBreakedItem ();
				}
				isReadyToAttack = false;
				progressBar.transform.localScale = Vector3.zero;
				progressBarBG.SetActive (false);
			}
		} else {			
			progressBar.transform.localScale = Vector3.zero;
			progressBarBG.SetActive (false);
		}
	}

	void DropBreakedItem ()
	{
		PlayerMovement.m_instance.SetAttackAnimation (false);
		int ran = 0;

		switch (currentSelectedItem.id) {
			case 11:
				if (currentSelectedItem.age == ItemDatabase.m_instance.items [currentSelectedItem.id].maxAge) {  // if item age is max then drop max else drop 1
					ran = Random.Range (ItemDatabase.m_instance.items [currentSelectedItem.id].dropRateMin, ItemDatabase.m_instance.items [currentSelectedItem.id].dropRateMax); // calculate random drop rate with min and max drop rate
				} else {
					ran = 1;
				}
				break;
			case 16:
				if (currentSelectedItem.age == ItemDatabase.m_instance.items [currentSelectedItem.id].maxAge) {  // if item age is max then drop max else drop 1
					ran = Random.Range (ItemDatabase.m_instance.items [currentSelectedItem.id].dropRateMin, ItemDatabase.m_instance.items [currentSelectedItem.id].dropRateMax); // calculate random drop rate with min and max drop rate
				} else {
					ran = 0;
				}
				break;
			default:
				if (currentSelectedItem.age == ItemDatabase.m_instance.items [currentSelectedItem.id].maxAge) {  // if item age is max then drop max else drop 1
					ran = Random.Range (ItemDatabase.m_instance.items [currentSelectedItem.id].dropRateMin, ItemDatabase.m_instance.items [currentSelectedItem.id].dropRateMax); // calculate random drop rate with min and max drop rate
				} else {
					ran = 1;
				}
				break;
		}
		InstansiateDropGameObject (ItemDatabase.m_instance.items [currentSelectedItem.id].drops, ran); // drop item upon break

		UpdateItemandSave ();  //update Gameobject and save in file

		currentSelectedItem = new item ();// set current tile position to -1 i.e. invalid
	}

	public void InstansiateDropGameObject (int id, int dropValue)
	{		
		for (int i = 0; i < dropValue; i++) {
			Vector2 ran = GameEventManager.currentSelectedTilePosition + Random.insideUnitCircle;
			GameObject drop = GameObject.Instantiate (inventoryItems [id], new Vector3 (ran.x, ran.y, 0), Quaternion.identity) as GameObject;
			drop.GetComponent <Devdog.InventorySystem.ObjectTriggererItem> ().isPickable = true;
			drop.transform.localScale = new Vector3 (0.75f, 0.75f, 0.75f);
		}
	}

	public void UpdateItemandSave ()
	{		
		switch (currentSelectedItem.id) {
			case 11: //Replace Tree with stump and stump with nothing
				if (currentSelectedItem.age == ItemDatabase.m_instance.items [currentSelectedItem.id].maxAge) {
					currentSelectedItem.age = 0;
					currentSelectedItem.GO.transform.GetChild (0).gameObject.SetActive (false);
					MapLoader.m_instance.SaveMapItemData (currentSelectedItem.id, currentSelectedItem.age, GameEventManager.currentSelectedTilePosition, onHarvest.RegrowToZero);
				} else { //Replace Stump with nothing
					MapLoader.m_instance.SaveMapItemData (currentSelectedItem.id, currentSelectedItem.age, GameEventManager.currentSelectedTilePosition, onHarvest.Destory);
				}
				break;
			case 16: //Berry Bush
				if (currentSelectedItem.age == ItemDatabase.m_instance.items [currentSelectedItem.id].maxAge) {
					currentSelectedItem.age = 3;
					MapLoader.m_instance.SaveMapItemData (currentSelectedItem.id, currentSelectedItem.age, GameEventManager.currentSelectedTilePosition, onHarvest.Renewable);
				} else { //Replace Stump with nothing
					MapLoader.m_instance.SaveMapItemData (currentSelectedItem.id, currentSelectedItem.age, GameEventManager.currentSelectedTilePosition, onHarvest.Destory);
				}
				break;
			default:
				Destroy (currentSelectedItem.GO);
				MapLoader.m_instance.SaveMapItemData (currentSelectedItem.id, currentSelectedItem.age, GameEventManager.currentSelectedTilePosition, onHarvest.Destory);			
				break;
		}
	}


}

