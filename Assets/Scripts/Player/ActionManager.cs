using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ActionManager : MonoBehaviour
{
	public static ActionManager m_AC_instance = null;
	public GameObject weaponGameObject, progressBar, progressBarBG;
	public bool isReadyToAttack = false;
	public GameObject[] inventoryItems;
	public GameObject consumeButtonInUI;
	//public RuntimeAnimatorController treeAnimator;
	public RuntimeAnimatorController treeAnimator;
	public Devdog.InventorySystem.InventoryItemBase currentWeildedItem, tempItem;
	/*public Devdog.InventorySystem.ItemCollectionBase[] allInventoryItems;*/
	SpriteRenderer weaponSprite;
	float baseTime = 0.0f, baseTimeStatic, progressVal = 0;

	item currentSelectedItem = new item ();
	Scene currentScene;

	void Awake ()
	{
		m_AC_instance = this;
		currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene ();
	}

	void Start ()
	{
		weaponSprite = weaponGameObject.GetComponent<SpriteRenderer> ();
		currentWeildedItem = new Devdog.InventorySystem.InventoryItemBase ();	
		progressBarBG.SetActive (false);
		//GetCurrentTile ();
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
			print ("once");
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
			print ("No items nearby");
			currentSelectedItem = new item ();
			if (currentWeildedItem.isPlaceable && !PlayerMovement.m_instance.isRightStick && Devdog.InventorySystem.InventoryManager.FindAll (currentWeildedItem.ID, false).Count > 0) {
				PlaceItem ();
			}
		}		
	}

	void CalculateHardness ()
	{
		/**/
		if (currentSelectedItem.id > 0 && !currentWeildedItem.isPlaceable) {				
			if (currentWeildedItem != null && currentWeildedItem.rarity.name == ItemDatabase.m_instance.items [currentSelectedItem.id].tool.ToString ()) { // if tool
				print ("Using Tools");
				baseTime = (GameEventManager.baseStrengthWithTool * ItemDatabase.m_instance.items [currentSelectedItem.id].hardness) / currentWeildedItem.itemQuality;
				baseTimeStatic = baseTime;
				isReadyToAttack = true;
				//PlayerMovement.m_instance.AttackCalculation ();
				//PlayerMovement.m_instance.SetAttackAnimation (true);
				if (currentSelectedItem.id == 14 || currentSelectedItem.id == 15) {
					currentSelectedItem.GO.transform.GetChild (0).GetComponent <Animator> ().runtimeAnimatorController = treeAnimator;
				}
			} else { // if not tool
				if (ItemDatabase.m_instance.items [currentSelectedItem.id].isHandMined) {		
					print ("Using Bare Hands");
					baseTime = 0.25f;
					baseTimeStatic = baseTime;
					isReadyToAttack = true;
					//PlayerMovement.m_instance.AttackCalculation ();
					PlayerMovement.m_instance.SetPickUpAnimation ();
				} else {
					isReadyToAttack = false;
					print ("cannot harvest");
					PlayerMovement.m_instance.AttackCalculation ();
					PlayerMovement.m_instance.SetAttackAnimation (false);
				}
			}
		} else {
			PlayerMovement.m_instance.AttackCalculation ();
			PlayerMovement.m_instance.SetAttackAnimation (false);
			print ("No items nearby");
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

			progressBar.transform.localScale = new Vector3 (progressVal, 0.1f, 1);
			progressBarBG.SetActive (true);

			if (baseTime <= 0) {				
				DropBreakedItem ();
				isReadyToAttack = false;
				progressBar.transform.localScale = Vector3.zero;
				progressBarBG.SetActive (false);
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
		//currentWeildedItem.Use ();		
		if (Devdog.InventorySystem.InventoryManager.FindAll (ActionManager.m_AC_instance.currentWeildedItem.ID, false).Count > 0) {
			Devdog.InventorySystem.InventoryManager.RemoveItem (ActionManager.m_AC_instance.currentWeildedItem.ID, 1, false);
		}
	}

	void DropBreakedItem ()
	{
		PlayerMovement.m_instance.SetAttackAnimation (false);
		int ran = 0;
		switch (currentSelectedItem.id) {
			case 14:
			case 15:
				print (currentWeildedItem.itemUse + "bef");
				currentWeildedItem.itemUse = currentWeildedItem.itemUse - 10;
				print (currentWeildedItem.itemUse + "action");

				currentSelectedItem.GO.transform.GetChild (0).GetComponent<Animator> ().SetBool ("TreeChopped", true); //tree falling animation
				currentSelectedItem.GO.transform.GetChild (1).GetComponent<SpriteRenderer> ().color = new Color (1f, 1f, 1f, 1f);// Fixed issue when stup remains transperent if tree chopped from south facing
				ran = Random.Range (ItemDatabase.m_instance.items [currentSelectedItem.id].dropRateMin, ItemDatabase.m_instance.items [currentSelectedItem.id].dropRateMax); // calculate random drop rate with min and max drop rate			

				break;
			case 16:
			case 21:
				if (currentSelectedItem.age == ItemDatabase.m_instance.items [currentSelectedItem.id].maxAge) {  // if item age is max then drop max else drop 0
					ran = Random.Range (ItemDatabase.m_instance.items [currentSelectedItem.id].dropRateMin, ItemDatabase.m_instance.items [currentSelectedItem.id].dropRateMax); // calculate random drop rate with min and max drop rate
				} else {
					ran = 0;
				}
				break;
			default:
				ran = Random.Range (ItemDatabase.m_instance.items [currentSelectedItem.id].dropRateMin, ItemDatabase.m_instance.items [currentSelectedItem.id].dropRateMax); // calculate random drop rate with min and max drop rate
				break;
		}
		InstansiateDropGameObject (ItemDatabase.m_instance.items [currentSelectedItem.id].drops, ran); // drop item upon break
		UpdateItemAndSaveToFile ();  //update Gameobject and save in file
		currentSelectedItem = new item ();// set current tile position to -1 i.e. invalid
		PlayerMovement.m_instance.CalculateNearestItem (0, 0, false);
	}

	public void UpdateItemAndSaveToFile ()
	{
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
				Destroy (currentSelectedItem.GO);
				LoadMapFromSave_PG.m_instance.SaveMapItemData (currentSelectedItem.id, currentSelectedItem.age, GameEventManager.currentSelectedTilePosition, onHarvest.Destory);
				break;
		}
	}

	public void InstansiateDropGameObject (int id, int dropValue)
	{		
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

