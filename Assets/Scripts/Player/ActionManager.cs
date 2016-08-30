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
		for (int i = 0; i < 10; i++) {
			print (Random.Range (0, 2));
		}
		m_AC_instance = this;
		weaponSprite = weaponGameObject.GetComponent <SpriteRenderer> ();
		currentWeildedItem = new Devdog.InventorySystem.InventoryItemBase ();
		//currentSelectedTile = new Devdog.InventorySystem.InventoryItemBase ();
		progressBarBG.SetActive (false);
	}

	public void ActionButtonPressed ()
	{
		GetCurrentTile ();
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
				if (ItemDatabase.m_instance.items [int.Parse (MapLoader.m_instance.GetTile (GameEventManager.currentSelectedTilePosition).name)].isHandMined) { // if object ishandmined then drop items
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

	void GetCurrentTile ()
	{
		if (MapLoader.m_instance.GetTile (GameEventManager.currentSelectedTilePosition) != null) {		
			currentSelectedTileId = int.Parse (MapLoader.m_instance.GetTile (GameEventManager.currentSelectedTilePosition).name);
			CalculateHardness ();
		} else {
			print ("No items nearby");
			currentSelectedTileId = -1;
		}
	}

	void DropBreakedItem ()
	{
		//currentSelectedTileId.GetComponent <Devdog.InventorySystem.ObjectTriggererItem> ().isPickable = true;
		//currentSelectedTileId.GetComponent <Devdog.InventorySystem.ObjectTriggererItem> ().Toggle (true);
		int ran = Random.Range (ItemDatabase.m_instance.items [int.Parse (MapLoader.m_instance.GetTile (GameEventManager.currentSelectedTilePosition).name)].dropRateMin, 
			          ItemDatabase.m_instance.items [int.Parse (MapLoader.m_instance.GetTile (GameEventManager.currentSelectedTilePosition).name)].dropRateMax);
		MapLoader.m_instance.InstansiatePickableGameObject (ItemDatabase.m_instance.items [int.Parse (MapLoader.m_instance.GetTile (GameEventManager.currentSelectedTilePosition).name)].drops, ran);
		RemoveAndSaveItem ();
	}

	void RemoveAndSaveItem ()
	{
		MapLoader.m_instance.DestoryTile (GameEventManager.currentSelectedTilePosition);
		MapLoader.m_instance.SaveMapitemData (GameEventManager.currentSelectedTilePosition);
		currentSelectedTileId = -1;
	}

	public void GetCurrentWeildedTool (Devdog.InventorySystem.InventoryItemBase i)
	{
		currentWeildedItem = i;
	}
}

