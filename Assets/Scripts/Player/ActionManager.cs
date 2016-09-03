using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ActionManager :MonoBehaviour
{
	public static ActionManager m_AC_instance = null;
	public GameObject weaponGameObject, progressBar, progressBarBG;
	SpriteRenderer weaponSprite;
	float baseTime = 0.0f, baseTimeStatic, progressVal = 0;
	public bool isReadyToAttack = false;
	public Text debugText;

	Devdog.InventorySystem.InventoryItemBase currentWeildedItem;
	//, currentSelectedTile;
	int currentSelectedTileId = -1;

	void Awake ()
	{
		m_AC_instance = this;
		weaponSprite = weaponGameObject.GetComponent <SpriteRenderer> ();
		currentWeildedItem = new Devdog.InventorySystem.InventoryItemBase ();
		//currentSelectedTile = new Devdog.InventorySystem.InventoryItemBase ();
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
		if (MapLoader.m_instance.GetTile (GameEventManager.currentSelectedTilePosition) > 0) {		
			currentSelectedTileId = MapLoader.m_instance.GetTile (GameEventManager.currentSelectedTilePosition);
			PlayerMovement.m_instance.AttackCalculation ();
			PlayerMovement.m_instance.SetAttackAnimation (true);
			CalculateHardness ();
		} else {
			PlayerMovement.m_instance.AttackCalculation ();
			PlayerMovement.m_instance.SetAttackAnimation (false);
			print ("No items nearby");
			currentSelectedTileId = 0;
		}
	}

	void CalculateHardness ()
	{		
		if (currentSelectedTileId >= 0 && ItemDatabase.m_instance.items [currentSelectedTileId].isHandMined) {
			if (currentWeildedItem != null && currentWeildedItem.rarity.name == ItemDatabase.m_instance.items [currentSelectedTileId].tool.ToString ()) { // if tool
				print ("Using Tools");				
				baseTime = (GameEventManager.baseStrengthWithTool * ItemDatabase.m_instance.items [currentSelectedTileId].hardness) / currentWeildedItem.itemQuality;
				baseTimeStatic = baseTime;		
				isReadyToAttack = true;
			} else { // if not tool
				print ("Using Bare Hands");
				baseTime = GameEventManager.baseStrengthWithoutTool * ItemDatabase.m_instance.items [currentSelectedTileId].hardness;
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
				if (ItemDatabase.m_instance.items [MapLoader.m_instance.GetTile (GameEventManager.currentSelectedTilePosition)].isHandMined) { // if object ishandmined then drop items
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
		int ran = Random.Range (ItemDatabase.m_instance.items [MapLoader.m_instance.GetTile (GameEventManager.currentSelectedTilePosition)].dropRateMin, 
			          ItemDatabase.m_instance.items [MapLoader.m_instance.GetTile (GameEventManager.currentSelectedTilePosition)].dropRateMax);           // calculate random drop rate with min and max drop rate

		MapLoader.m_instance.InstansiateDropGameObject (ItemDatabase.m_instance.items [MapLoader.m_instance.GetTile (GameEventManager.currentSelectedTilePosition)].drops, ran); // drop item upon break

		MapLoader.m_instance.UpdateItemandSave (GameEventManager.currentSelectedTilePosition);  //update Gameobject and save in file

		currentSelectedTileId = -1; // set current tile position to -1 i.e. invalid
	}

}

