using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ActionManager : MonoBehaviour
{
	public static ActionManager m_AC_instance = null;
	public SpriteRenderer playerRightHandTool;
	public Sprite[] weaponsSprite;
	public GameObject progressBar, progressBarBG;
	public bool isReadyToAttack = false;
	public GameObject[] inventoryItems;
	public GameObject consumeButtonInUI, containerUI;
	public RuntimeAnimatorController treeAnimator;
	public Devdog.InventorySystem.InventoryItemBase currentWeildedItem, tempItem;
	float baseTime = 0.0f, baseTimeStatic, progressVal = 0;

	item currentSelectedItem = new item ();
	Scene currentScene;
	public Devdog.InventorySystem.InventoryUIItemWrapper[] itemsInInventory;

	void Awake ()
	{
		m_AC_instance = this;
		currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene ();
	}

	void Start ()
	{
		itemsInInventory = containerUI.GetComponentsInChildren <Devdog.InventorySystem.InventoryUIItemWrapper> ();
		currentWeildedItem = new Devdog.InventorySystem.InventoryItemBase ();	
		progressBarBG.SetActive (false);
	}

	public void RemoveBorder ()
	{
		for (int i = 0; i < itemsInInventory.Length; i++) {
			itemsInInventory [i].border.gameObject.SetActive (false);
		}
	}

	public void UpdateAllItemsInInventory ()
	{
		System.Array.Clear (itemsInInventory, 0, itemsInInventory.Length);
		itemsInInventory = containerUI.GetComponentsInChildren <Devdog.InventorySystem.InventoryUIItemWrapper> ();
		foreach (var slot in itemsInInventory) {
			if (slot.item != null) {
				slot.itemUseBar.rectTransform.sizeDelta = new Vector2 (slot.item.itemDurability, slot.itemUseBar.rectTransform.rect.height);
			}
		}
		UpdatePlayerWeapon ();
	}


	public void GetCurrentWeildedTool (Devdog.InventorySystem.InventoryItemBase i)
	{
		if (i == null) {
			currentWeildedItem = tempItem;
		} else {
			currentWeildedItem = i;
			if (currentWeildedItem != null && currentWeildedItem.isConsumable) {
				consumeButtonInUI.SetActive (true);
			} else {
				consumeButtonInUI.SetActive (false);
			}
			PlayerMovement.m_instance.CalculateNearestItem (0, 1, false);
			UpdateAllItemsInInventory ();
		}	
	}

	public void UpdatePlayerWeapon ()
	{
		switch (currentWeildedItem.rarity.name) {
			case "Hand":
				playerRightHandTool.sprite = weaponsSprite [0];
				break;
			case "Axe":
				playerRightHandTool.sprite = weaponsSprite [1];
				break;
			case "Pickaxe":
				playerRightHandTool.sprite = weaponsSprite [2];
				break;
			case "Hammer":
				playerRightHandTool.sprite = weaponsSprite [3];
				break;
			case "Hoe":
				playerRightHandTool.sprite = weaponsSprite [4];
				break;
			case "Shovel":
				playerRightHandTool.sprite = weaponsSprite [5];
				break;
			case "FishingRod":
				playerRightHandTool.sprite = weaponsSprite [6];
				break;
			case "Sword":
				playerRightHandTool.sprite = weaponsSprite [7];
				break;		
			default:
				break;
		}
	}

	/*public void GetAllInventoryItems (Devdog.InventorySystem.ItemCollectionBase[] items)
	{
		allInventoryItems = items;
	}*/

	public void ActionButtonPressed ()
	{
		GetCurrentTile ();
	}

	void GetCurrentTile ()
	{
		if (currentWeildedItem == null) {			
			currentWeildedItem = tempItem;
			/*currentWeildedItem.ID = 0;
			currentWeildedItem.isPlaceable = false;
			currentWeildedItem.rarity.name = "Hand";
			currentWeildedItem.itemQuality = 1;
			currentWeildedItem.itemID = "0,-1";*/			
		} 
		if (LoadMapFromSave_PG.m_instance.GetTile (GameEventManager.currentSelectedTilePosition).id > 0) {
			currentSelectedItem = LoadMapFromSave_PG.m_instance.GetTile (GameEventManager.currentSelectedTilePosition);
			CalculateHardness ();
		} else {
			//print ("No items nearby");
			currentSelectedItem = new item ();
			if (currentWeildedItem.isPlaceable && !PlayerMovement.m_instance.isRightStick && Devdog.InventorySystem.InventoryManager.FindAll (currentWeildedItem.ID, false).Count > 0) {
				PlaceItem ();
			}
		}		
	}

	void CalculateHardness ()
	{
		//print (currentWeildedItem.rarity.name);
		if (currentSelectedItem.id > 0) {
			if (ItemDatabase.m_instance.items [currentSelectedItem.id].isHandMined) {		
				//print ("Using Bare Hands");
				PlayerMovement.m_instance.SetPickUpAnimation ();
				baseTime = 0.35f;
				baseTimeStatic = baseTime;
				isReadyToAttack = true;
				PlayerMovement.m_instance.SetPickUpAnimation ();
				return;
			} 

			if (currentWeildedItem.rarity.name == ItemDatabase.m_instance.items [currentSelectedItem.id].tool.ToString ()) { // if tool in hand and using tool but not bare hands as above
				//*********************************Common for all tools********************************************
				baseTime = (GameEventManager.baseStrengthWithTool * ItemDatabase.m_instance.items [currentSelectedItem.id].hardness) / currentWeildedItem.itemQuality;
				baseTimeStatic = baseTime;
				isReadyToAttack = true;
				//*************************************************************************************************
				switch (ItemDatabase.m_instance.items [currentSelectedItem.id].tool) {
					case Item.ItemTool.Hand:
						//print ("using hands, this will never execute!!");
						break;
					case Item.ItemTool.Axe:						
						PlayerMovement.m_instance.SetSlashingAnimation (true);
						if (currentSelectedItem.id == 14 || currentSelectedItem.id == 15) {
							currentSelectedItem.GO.transform.GetChild (0).GetComponent <Animator> ().runtimeAnimatorController = treeAnimator;
						}
						//print ("using Axe");
						break;
					case Item.ItemTool.Pickaxe:
						PlayerMovement.m_instance.SetSlashingAnimation (true);
						//print ("using pickaxe");
						break;
					case Item.ItemTool.Shovel:
						PlayerMovement.m_instance.SetDigUpAnimation ();
						//print ("using shovel");
						break;
					case Item.ItemTool.FishingRod:
						//print ("using FishingRod");
						break;
					case Item.ItemTool.Hammer:
						//print ("using hammer");
						break;
					case Item.ItemTool.Hoe:
						//print ("using hoe");
						break;
					case Item.ItemTool.Sword:
						//print ("using Sword");
						break;
					case Item.ItemTool.None:
						//print ("using none");
						break;
					default:
						break;
				}
			}
		}
	}

	void PlaceItem ()
	{
		//string[] itemss = currentWeildedItem.itemID.Split (',');
		//LoadMapFromSave_PG.m_instance.InstantiatePlacedObject (LoadMapFromSave_PG.m_instance.items [sbyte.Parse (itemss [0])], GameEventManager.currentSelectedTilePosition, LoadMapFromSave_PG.m_instance.mapChunks [PlayerPrefs.GetInt ("mapChunkPosition")].transform,
		//PlayerPrefs.GetInt ("mapChunkPosition"), sbyte.Parse (itemss [0]), sbyte.Parse (itemss [1]));
	}

	public void PlaceItemByButton ()
	{
		/*string[] itemss = currentWeildedItem.itemID.Split (',');
		ItemPlacer.m_instance.itemPlacer.transform.position = new Vector3 (PlayerMovement.m_instance.gameObject.transform.position.x, PlayerMovement.m_instance.gameObject.transform.position.y - 1, 0);
		MoreInventoryButton.m_instance.ToggleInventorySize ();
*/
		/*LoadMapFromSave_PG.m_instance.InstantiatePlacedObject (LoadMapFromSave_PG.m_instance.items [sbyte.Parse (itemss [0])], 
			GameEventManager.currentSelectedTilePosition, LoadMapFromSave_PG.m_instance.mapChunks [PlayerPrefs.GetInt ("mapChunkPosition")].transform,
			PlayerPrefs.GetInt ("mapChunkPosition"), sbyte.Parse (itemss [0]), sbyte.Parse (itemss [1]));*/
	}

	void Update ()
	{
		if (isReadyToAttack) {
			if (currentSelectedItem.id == 14 || currentSelectedItem.id == 15) {
				currentSelectedItem.GO.transform.GetChild (0).GetComponent<Animator> ().SetTrigger ("isTreeCutting");
			}
			baseTime -= Time.deltaTime;
			progressVal = baseTime / baseTimeStatic;

			progressBar.transform.localScale = new Vector3 (progressVal, 0.3f, 1);
			progressBarBG.SetActive (true);

			if (baseTime <= 0) {				
				DropBreakedItem ();
				isReadyToAttack = false;
				progressBar.transform.localScale = Vector3.zero;
				progressBarBG.SetActive (false);
				PlayerMovement.m_instance.ActionCompleted ();
			}
		} else {
			progressBar.transform.localScale = Vector3.zero;
			progressBarBG.SetActive (false);
		}
	}

	public void EatConsumableItem ()
	{
		if (currentWeildedItem.isConsumable) {
			for (int i = 0; i < currentWeildedItem.properties.Length; i++) {
				switch (currentWeildedItem.properties [i].property.name) {
					case "RestoreHealth":
						Health.m_instance.modifyHealth (currentWeildedItem.properties [0].intValue);
						break;
					case "RestoreHunger":
						Food.m_instance.modifyHunger (currentWeildedItem.properties [0].intValue);
						break;
					default:
						break;
				}
			}
			currentWeildedItem.Use ();
		}
	}

	public void DestoryInventoryItem ()
	{
		if (Devdog.InventorySystem.InventoryManager.FindAll (ActionManager.m_AC_instance.currentWeildedItem.ID, false).Count > 0) {
			Devdog.InventorySystem.InventoryManager.RemoveItem (ActionManager.m_AC_instance.currentWeildedItem.ID, 1, false);
			UpdateAllItemsInInventory ();
		}
	}

	void DropBreakedItem ()
	{
		PlayerMovement.m_instance.SetAttackAnimation (false);
		int ran1 = 0;
		int ran2 = 0;
		int ran3 = 0;
		currentWeildedItem.itemDurability = currentWeildedItem.itemDurability - ItemDatabase.m_instance.items [currentSelectedItem.id].reduceToolDurability;
		if (currentWeildedItem.itemDurability < 1) {
			DestoryInventoryItem ();
			Devdog.InventorySystem.InventoryUIItemWrapper.m_instance.InventorySlotClicked ();
		}

		ran1 = Random.Range (ItemDatabase.m_instance.items [currentSelectedItem.id].drop1RateMin, ItemDatabase.m_instance.items [currentSelectedItem.id].drop1RateMax); // calculate random drop rate with min and max drop rate
		ran2 = Random.Range (ItemDatabase.m_instance.items [currentSelectedItem.id].drop2RateMin, ItemDatabase.m_instance.items [currentSelectedItem.id].drop2RateMax); // calculate random drop rate with min and max drop rate
		ran3 = Random.Range (ItemDatabase.m_instance.items [currentSelectedItem.id].drop3RateMin, ItemDatabase.m_instance.items [currentSelectedItem.id].drop3RateMax); // calculate random drop rate with min and max drop rate

		switch (currentSelectedItem.id) {
			case 14:
			case 15:
				currentSelectedItem.GO.transform.GetChild (0).GetComponent<Animator> ().SetBool ("TreeChopped", true); //tree falling animation
				currentSelectedItem.GO.transform.GetChild (1).GetComponent<SpriteRenderer> ().color = new Color (1f, 1f, 1f, 1f);// Fixed issue when stup remains transperent if tree chopped from south facing
				break;
			case 16:
			case 21:
				if (currentSelectedItem.age == ItemDatabase.m_instance.items [currentSelectedItem.id].maxAge) {  // if item age is max then drop max else drop 0
					ran1 = Random.Range (ItemDatabase.m_instance.items [currentSelectedItem.id].drop1RateMin, ItemDatabase.m_instance.items [currentSelectedItem.id].drop1RateMax); // calculate random drop rate with min and max drop rate
				} else {
					ran1 = 0;
				}
				break;
			default:				
				break;
		}

		if (ItemDatabase.m_instance.items [currentSelectedItem.id].drops1 >= 0) {
			InstansiateDropGameObject (ItemDatabase.m_instance.items [currentSelectedItem.id].drops1, ran1);// drop item upon break
		}
		if (ItemDatabase.m_instance.items [currentSelectedItem.id].drops2 >= 0) {
			InstansiateDropGameObject (ItemDatabase.m_instance.items [currentSelectedItem.id].drops2, ran2);// drop item upon break
		}
		if (ItemDatabase.m_instance.items [currentSelectedItem.id].drops3 >= 0) {
			InstansiateDropGameObject (ItemDatabase.m_instance.items [currentSelectedItem.id].drops3, ran3);// drop item upon break
		}

		UpdateItemAndSaveToFile ();  //update Gameobject and save in file
		currentSelectedItem = new item ();// set current tile position to -1 i.e. invalid
		PlayerMovement.m_instance.CalculateNearestItem (0, 0, false);
		UpdateAllItemsInInventory ();
	}

	public void InstansiateDropGameObject (int id, int dropValue)
	{
		//print ("id " + id + " value " + dropValue);
		for (int i = 0; i < dropValue; i++) {
			GameObject parent = new GameObject ();
			parent.name = "parent";
			Vector2 ran = GameEventManager.currentSelectedTilePosition + Random.insideUnitCircle;
			parent.transform.position = new Vector3 (ran.x, ran.y, 0);
			GameObject drop = GameObject.Instantiate (inventoryItems [id], new Vector3 (ran.x, ran.y, 0), Quaternion.identity) as GameObject;
			drop.transform.localScale = new Vector3 (GameEventManager.dropItemSize, GameEventManager.dropItemSize, GameEventManager.dropItemSize);
			drop.transform.parent = parent.transform;
			drop.GetComponent<Devdog.InventorySystem.ObjectTriggererItem> ().isPickable = true;
			drop.GetComponent<Animator> ().Play ("itemDrop");
			StartCoroutine (DropItemsLiveAfterSeconds (parent.gameObject));			
		}
	}

	public void UpdateItemAndSaveToFile ()
	{
		//print (currentSelectedItem.id);
		switch (currentSelectedItem.id) {
			
			case 14: //Replace Tree with stump
			case 15: //Replace Tree with stump
				LoadMapFromSave_PG.m_instance.SaveMapItemData (currentSelectedItem.id, currentSelectedItem.age, GameEventManager.currentSelectedTilePosition, onHarvest.RegrowToStump);
				break;
			case 16: //Berry Bush
				if (currentSelectedItem.age == ItemDatabase.m_instance.items [currentSelectedItem.id].maxAge) {
					currentSelectedItem.age = 3;
					LoadMapFromSave_PG.m_instance.SaveMapItemData (currentSelectedItem.id, currentSelectedItem.age, GameEventManager.currentSelectedTilePosition, onHarvest.Renewable);
				} else { //Replace Stump with nothing
					LoadMapFromSave_PG.m_instance.SaveMapItemData (currentSelectedItem.id, currentSelectedItem.age, GameEventManager.currentSelectedTilePosition, onHarvest.Destory);
				}
				break;
			default:
				//Debug.Log ("destroying");
				Destroy (currentSelectedItem.GO);
				LoadMapFromSave_PG.m_instance.SaveMapItemData (currentSelectedItem.id, currentSelectedItem.age, GameEventManager.currentSelectedTilePosition, onHarvest.Destory);
				break;
		}
	}

	IEnumerator DropItemsLiveAfterSeconds (GameObject go)
	{
		yield return new WaitForSeconds (0.5f);
		go.transform.GetChild (0).GetComponent<BoxCollider2D> ().enabled = true;
		go.transform.DetachChildren ();
		Destroy (go);
	}

	int GetItemIDByIndex (string s, int index)
	{
		string[] sArray = s.Split (',');
		return int.Parse (sArray [index]);
	}


}

