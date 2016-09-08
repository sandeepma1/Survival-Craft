using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ActionManager : MonoBehaviour {
	public static ActionManager m_AC_instance = null;
	public GameObject weaponGameObject, progressBar, progressBarBG;
	public bool isReadyToAttack = false;
	public GameObject[] inventoryItems;

	SpriteRenderer weaponSprite;
	float baseTime = 0.0f, baseTimeStatic, progressVal = 0;
	public Devdog.InventorySystem.InventoryItemBase currentWeildedItem;
	item currentSelectedItem = new item();

	void Awake() {
		m_AC_instance = this;
		weaponSprite = weaponGameObject.GetComponent<SpriteRenderer>();
		currentWeildedItem = new Devdog.InventorySystem.InventoryItemBase();
		progressBarBG.SetActive(false);
	}

	public void GetCurrentWeildedTool(Devdog.InventorySystem.InventoryItemBase i) {
		currentWeildedItem = i;
	}

	public void ActionButtonPressed() {
		GetCurrentTile();
	}

	void GetCurrentTile() {
		if (MapLoader.m_instance.GetTile(GameEventManager.currentSelectedTilePosition).id > 0) {
			currentSelectedItem = MapLoader.m_instance.GetTile(GameEventManager.currentSelectedTilePosition);
			CalculateHardness();
		} else {
			print("No items nearby");
			currentSelectedItem = new item();
			if (currentWeildedItem.isPlaceable && !PlayerMovement.m_instance.isRightStick && Devdog.InventorySystem.InventoryManager.FindAll(currentWeildedItem.ID, false).Count > 0) {
				PlaceItem();
			}
		}
	}

	void PlaceItem() {
		string[] itemss = currentWeildedItem.itemID.Split(',');
		MapLoader ml = MapLoader.m_instance;
		ml.InstantiatePlacedObject(ml.items[sbyte.Parse(itemss[0])], GameEventManager.currentSelectedTilePosition, ml.mapChunks[PlayerPrefs.GetInt("mapChunkPosition")].transform,
			PlayerPrefs.GetInt("mapChunkPosition"), sbyte.Parse(itemss[0]), sbyte.Parse(itemss[1]));
	}

	void CalculateHardness() {
		if (currentSelectedItem.id > 0 && !currentWeildedItem.isPlaceable) {//ItemDatabase.m_instance.items [currentSelectedItem.id].isHandMined) {
			if (currentWeildedItem != null && currentWeildedItem.rarity.name == ItemDatabase.m_instance.items[currentSelectedItem.id].tool.ToString()) { // if tool
				print("Using Tools");
				baseTime = (GameEventManager.baseStrengthWithTool * ItemDatabase.m_instance.items[currentSelectedItem.id].hardness) / currentWeildedItem.itemQuality;
				baseTimeStatic = baseTime;
				isReadyToAttack = true;
			} else { // if not tool
				print("Using Bare Hands");
				baseTime = GameEventManager.baseStrengthWithoutTool * ItemDatabase.m_instance.items[currentSelectedItem.id].hardness;
				baseTimeStatic = baseTime;
				isReadyToAttack = true;
			}
			PlayerMovement.m_instance.AttackCalculation();
			PlayerMovement.m_instance.SetAttackAnimation(true);
		} else {
			PlayerMovement.m_instance.AttackCalculation();
			PlayerMovement.m_instance.SetAttackAnimation(false);
			print("No items nearby");
		}
	}

	void Update() {

		if (isReadyToAttack) {
			if (currentSelectedItem.id == 11) {
				currentSelectedItem.GO.transform.GetChild(0).GetComponent<Animator>().SetTrigger("isTreeCutting");
			}
			baseTime -= Time.deltaTime;
			progressVal = baseTime / baseTimeStatic;

			progressBar.transform.localScale = new Vector3(progressVal, 0.1f, 1);
			progressBarBG.SetActive(true);

			if (baseTime <= 0) {
				if (ItemDatabase.m_instance.items[currentSelectedItem.id].isHandMined) { // if object ishandmined then drop items
					if (currentSelectedItem.id == 11) {
						currentSelectedItem.GO.transform.GetChild(0).GetComponent<Animator>().SetBool("TreeChopped", true);
					}
					DropBreakedItem();

				}
				isReadyToAttack = false;
				progressBar.transform.localScale = Vector3.zero;
				progressBarBG.SetActive(false);
			}
		} else {
			progressBar.transform.localScale = Vector3.zero;
			progressBarBG.SetActive(false);
		}
	}

	void DropBreakedItem() {
		PlayerMovement.m_instance.SetAttackAnimation(false);
		int ran = 0;
		switch (currentSelectedItem.id) {
			case 11:
			if (currentSelectedItem.age == ItemDatabase.m_instance.items[currentSelectedItem.id].maxAge) {  // if item age is max then drop max else drop 1
				ran = Random.Range(ItemDatabase.m_instance.items[currentSelectedItem.id].dropRateMin, ItemDatabase.m_instance.items[currentSelectedItem.id].dropRateMax); // calculate random drop rate with min and max drop rate
			} else {
				ran = 1;
			}
			break;
			case 16:
			case 21:
			if (currentSelectedItem.age == ItemDatabase.m_instance.items[currentSelectedItem.id].maxAge) {  // if item age is max then drop max else drop 0
				ran = Random.Range(ItemDatabase.m_instance.items[currentSelectedItem.id].dropRateMin, ItemDatabase.m_instance.items[currentSelectedItem.id].dropRateMax); // calculate random drop rate with min and max drop rate
			} else {
				ran = 0;
			}
			break;
			default:
			//if (currentSelectedItem.age == ItemDatabase.m_instance.items [currentSelectedItem.id].maxAge) {  // if item age is max then drop max else drop 1
			ran = Random.Range(ItemDatabase.m_instance.items[currentSelectedItem.id].dropRateMin, ItemDatabase.m_instance.items[currentSelectedItem.id].dropRateMax); // calculate random drop rate with min and max drop rate
																																									  //} else {
																																									  //ran = 1;
																																									  //}
			break;
		}
		InstansiateDropGameObject(ItemDatabase.m_instance.items[currentSelectedItem.id].drops, ran); // drop item upon break

		UpdateItemandSave();  //update Gameobject and save in file

		currentSelectedItem = new item();// set current tile position to -1 i.e. invalid
	}

	public void InstansiateDropGameObject(int id, int dropValue) {
		for (int i = 0; i < dropValue; i++) {
			GameObject parent = new GameObject();
			parent.name = "parent";
			Vector2 ran = GameEventManager.currentSelectedTilePosition + Random.insideUnitCircle;
			parent.transform.position = new Vector3(ran.x, ran.y, 0);
			GameObject drop = GameObject.Instantiate(inventoryItems[id], new Vector3(ran.x, ran.y, 0), Quaternion.identity) as GameObject;
			drop.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
			drop.transform.parent = parent.transform;
			drop.GetComponent<Devdog.InventorySystem.ObjectTriggererItem>().isPickable = true;
			drop.GetComponent<Animator>().Play("itemDrop");
			StartCoroutine(DropItemsLiveAfterSeconds(parent.gameObject));
		}
	}

	IEnumerator DropItemsLiveAfterSeconds(GameObject go) {
		yield return new WaitForSeconds(0.5f);
		go.transform.GetChild(0).GetComponent<BoxCollider2D>().enabled = true;
		go.transform.DetachChildren();
		Destroy(go);
	}

	public void UpdateItemandSave() {
		switch (currentSelectedItem.id) {
			case 11: //Replace Tree with stump and stump with nothing
			if (currentSelectedItem.age == ItemDatabase.m_instance.items[currentSelectedItem.id].maxAge) {
				currentSelectedItem.age = 0;
				MapLoader.m_instance.SaveMapItemData(currentSelectedItem.id, currentSelectedItem.age, GameEventManager.currentSelectedTilePosition, onHarvest.RegrowToZero);
				currentSelectedItem.GO.transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f); // Fixed issue when stup remains transperent if tree chopped from south facing
			} else { //Replace Stump with nothing
				MapLoader.m_instance.SaveMapItemData(currentSelectedItem.id, currentSelectedItem.age, GameEventManager.currentSelectedTilePosition, onHarvest.Destory);
			}
			break;
			case 16: //Berry Bush
			if (currentSelectedItem.age == ItemDatabase.m_instance.items[currentSelectedItem.id].maxAge) {
				currentSelectedItem.age = 3;
				MapLoader.m_instance.SaveMapItemData(currentSelectedItem.id, currentSelectedItem.age, GameEventManager.currentSelectedTilePosition, onHarvest.Renewable);
			} else { //Replace Stump with nothing
				MapLoader.m_instance.SaveMapItemData(currentSelectedItem.id, currentSelectedItem.age, GameEventManager.currentSelectedTilePosition, onHarvest.Destory);
			}
			break;
			default:
			Destroy(currentSelectedItem.GO);
			MapLoader.m_instance.SaveMapItemData(currentSelectedItem.id, currentSelectedItem.age, GameEventManager.currentSelectedTilePosition, onHarvest.Destory);
			break;
		}
	}
}

