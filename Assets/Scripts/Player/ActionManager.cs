using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ActionManager : MonoBehaviour
{
    public static ActionManager m_instance = null;
    public SpriteRenderer playerRightHandTool;
    public Sprite[] weaponsSprite;
    public GameObject progressBar, progressBarBG;
    public bool isReadyToAttack = false;
    public GameObject[] inventoryItems;
    public GameObject consumeButtonInUI, containerUI;
    public RuntimeAnimatorController treeAnimator;
    public GameObject droppedItem;
    private float baseTime = 0.0f, baseTimeStatic, progressVal = 0;
    private float nextUpdate = 0f;

    public item currentSelectedItem = new item();
    private Scene currentScene;

    //public Devdog.InventorySystem.InventoryUIItemWrapper[] itemsInInventory;

    private void Awake()
    {
        m_instance = this;
        currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
    }

    private void Start()
    {
        //itemsInInventory = containerUI.GetComponentsInChildren <Devdog.InventorySystem.InventoryUIItemWrapper> ();
        //currentWeildedItem = new Item ();	
        progressBarBG.SetActive(false);
    }

    /*
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
			PlayerMovement.Instance.CalculateNearestItem (0, 1, false);
			UpdateAllItemsInInventory ();
		}	
	}*/

    /*public void UpdatePlayerWeapon ()
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
	}*/

    /*public void GetAllInventoryItems (Devdog.InventorySystem.ItemCollectionBase[] items)
	{
		allInventoryItems = items;
	}*/

    public void ActionButtonPressed()
    {
        if (PlayerMovement.Instance.IsPlayerBuildingSelected())
        { // if player is building something
            if (PlayerMovement.Instance.isBuildingPlacable)
            {
                LoadMapFromSave_PG.m_instance.InstantiatePlacedObject(
                    LoadMapFromSave_PG.m_instance.items[Inventory.m_instance.playerSelectedTool.ID].gameObject,
                    PlayerMovement.Instance.itemPlacer.transform.position,
                    LoadMapFromSave_PG.m_instance.mapChunks[PlayerPrefs.GetInt("mapChunkPosition")].transform, PlayerPrefs.GetInt("mapChunkPosition"),
                    Inventory.m_instance.playerSelectedTool.ID, -1);
                Inventory.m_instance.RemoveItem(Inventory.m_instance.playerSelectedTool.ID);
                print("placed");
            }
            return;
        }
        else
        {
            GetCurrentTile();
        }
    }

    private void GetCurrentTile()
    {
        if (Inventory.m_instance.playerSelectedTool == null)
        {
            //Inventory.m_instance.playerSelectedTool = tempItem;
            Inventory.m_instance.playerSelectedTool.ID = 0;
            //currentWeildedItem.IsPlaceable = false;
            //currentWeildedItem.Tool = "Hand";
            //currentWeildedItem.itemQuality = 1;
            //currentWeildedItem.itemID = "0,-1";
        }
        if (LoadMapFromSave_PG.m_instance.GetTile(GameEventManager.currentSelectedTilePosition).id > 0)
        {
            currentSelectedItem = LoadMapFromSave_PG.m_instance.GetTile(GameEventManager.currentSelectedTilePosition);
            CalculateHardness();
        }
        else
        {
            print("No items nearby");
            currentSelectedItem = new item();
            /*if (currentWeildedItem.isPlaceable && !PlayerMovement.Instance.isRightStick && Devdog.InventorySystem.InventoryManager.FindAll (currentWeildedItem.ID, false).Count > 0) {
				PlaceItem ();
			}*/
        }
    }

    private void CalculateHardness()
    {
        if (currentSelectedItem.id > 0)
        {
            if (ItemDatabase.m_instance.items[currentSelectedItem.id].IsHandMined)
            {
                print("Using Bare Hands");
                PlayerMovement.Instance.SetPickUpAnimation();
                baseTime = 0.35f;
                baseTimeStatic = baseTime;
                isReadyToAttack = true;
                PlayerMovement.Instance.SetPickUpAnimation();
                return;
            }

            //*********************************Common for all tools********************************************	

            if (Inventory.m_instance.playerSelectedTool.ID == -1 || Inventory.m_instance.playerSelectedTool.Tool == ItemTool.Hand)
            {
                baseTime = (GameEventManager.baseStrengthWithoutTool * ItemDatabase.m_instance.items[currentSelectedItem.id].Hardness);
            }
            else
            {
                if (Inventory.m_instance.playerSelectedTool.Type == ItemType.Tool && Inventory.m_instance.playerSelectedTool.Tool == ItemDatabase.m_instance.items[currentSelectedItem.id].Tool)
                {
                    baseTime = (GameEventManager.baseStrengthWithProperTool * (ItemDatabase.m_instance.items[currentSelectedItem.id].Hardness) / Inventory.m_instance.playerSelectedTool.ToolQuality);
                }
                else if (Inventory.m_instance.playerSelectedTool.Type == ItemType.Tool)
                {
                    baseTime = (GameEventManager.baseStrengthWithAnyTool * (ItemDatabase.m_instance.items[currentSelectedItem.id].Hardness) / Inventory.m_instance.playerSelectedTool.ToolQuality);
                }
            }
            baseTimeStatic = baseTime;
            isReadyToAttack = true;
            //*************************************************************************************************
            switch (ItemDatabase.m_instance.items[currentSelectedItem.id].Tool)
            {
                case ItemTool.Hand:
                    //print ("using hands, this will never execute!!");
                    break;
                case ItemTool.Axe:
                    PlayerMovement.Instance.SetSlashingAnimation(true);
                    break;
                case ItemTool.Pickaxe:
                    PlayerMovement.Instance.SetSlashingAnimation(true);
                    //print ("using pickaxe");
                    break;
                case ItemTool.Shovel:
                    PlayerMovement.Instance.SetDigUpAnimation();
                    //print ("using shovel");
                    break;
                case ItemTool.FishingRod:
                    //print ("using FishingRod");
                    break;
                case ItemTool.Hammer:
                    //print ("using hammer");
                    break;
                case ItemTool.Hoe:
                    //print ("using hoe");
                    break;
                case ItemTool.Sword:
                    //print ("using Sword");
                    break;
                case ItemTool.None:
                    //print ("using none");
                    break;
                default:
                    break;
            }
            //}
        }
    }

    private void PlaceItem()
    {
        //string[] itemss = currentWeildedItem.itemID.Split (',');
        //LoadMapFromSave_PG.m_instance.InstantiatePlacedObject (LoadMapFromSave_PG.m_instance.items [sbyte.Parse (itemss [0])], GameEventManager.currentSelectedTilePosition, LoadMapFromSave_PG.m_instance.mapChunks [PlayerPrefs.GetInt ("mapChunkPosition")].transform,
        //PlayerPrefs.GetInt ("mapChunkPosition"), sbyte.Parse (itemss [0]), sbyte.Parse (itemss [1]));
    }

    public void PlaceItemByButton()
    {
        /*string[] itemss = currentWeildedItem.itemID.Split (',');
		ItemPlacer.m_instance.itemPlacer.transform.position = new Vector3 (PlayerMovement.Instance.gameObject.transform.position.x, PlayerMovement.Instance.gameObject.transform.position.y - 1, 0);
		MoreInventoryButton.m_instance.ToggleInventorySize ();
*/
        /*LoadMapFromSave_PG.m_instance.InstantiatePlacedObject (LoadMapFromSave_PG.m_instance.items [sbyte.Parse (itemss [0])], 
			GameEventManager.currentSelectedTilePosition, LoadMapFromSave_PG.m_instance.mapChunks [PlayerPrefs.GetInt ("mapChunkPosition")].transform,
			PlayerPrefs.GetInt ("mapChunkPosition"), sbyte.Parse (itemss [0]), sbyte.Parse (itemss [1]));*/
    }

    private void UpdateEverySecond()
    {
        if (isReadyToAttack)
        {
            //baseTime -= Time.deltaTime;
            baseTime--;
            print(baseTime);
            if (currentSelectedItem.id == 8 || currentSelectedItem.id == 9)
            {
                currentSelectedItem.GO.GetComponent<Animator>().SetTrigger("isTreeCutting");
            }
            progressVal = baseTime / baseTimeStatic;
            progressBar.transform.localScale = new Vector3(progressVal, 0.3f, 1);
            progressBarBG.SetActive(true);
            if (baseTime < 0)
            {
                DropBreakedItem();
                isReadyToAttack = false;
                progressBar.transform.localScale = Vector3.zero;
                progressBarBG.SetActive(false);
                PlayerMovement.Instance.ActionCompleted();
                if (currentSelectedItem.id == 8 || currentSelectedItem.id == 9)
                {
                    print("TreeFalling");
                    currentSelectedItem.GO.GetComponent<Animator>().SetTrigger("TreeFalling");
                }
            }
        }
        else
        {
            progressBar.transform.localScale = Vector3.zero;
            progressBarBG.SetActive(false);
        }
    }


    private void Update()
    {
        if (isReadyToAttack)
        {
            if (Time.time >= nextUpdate)
            {
                // Call your fonction
                UpdateEverySecond();
                nextUpdate = Time.time + 0.5f;
            }
        }
    }

    public void EatConsumableItem()
    {
        /*if (currentWeildedItem.isConsumable) {
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
		}*/
    }

    public void DestoryInventoryItem()
    {
        /*if (Devdog.InventorySystem.InventoryManager.FindAll (ActionManager.m_AC_instance.currentWeildedItem.ID, false).Count > 0) {
			Devdog.InventorySystem.InventoryManager.RemoveItem (ActionManager.m_AC_instance.currentWeildedItem.ID, 1, false);
			UpdateAllItemsInInventory ();
		}*/
    }

    private void DropBreakedItem()
    {
        PlayerMovement.Instance.SetAttackAnimation(false);
        int ran1 = 0;
        int ran2 = 0;
        int ran3 = 0;

        Inventory.m_instance.DecreseWeaponDurability(ItemDatabase.m_instance.items[currentSelectedItem.id].ReduceToolDurability);
        /*	currentWeildedItem.itemDurability = currentWeildedItem.itemDurability - ItemDatabase.m_instance.items [currentSelectedItem.id].reduceToolDurability;
		if (currentWeildedItem.itemDurability < 1) {
			DestoryInventoryItem ();
			Devdog.InventorySystem.InventoryUIItemWrapper.m_instance.InventorySlotClicked ();
		}*/

        ran1 = Random.Range(ItemDatabase.m_instance.items[currentSelectedItem.id].Drop1RateMin, ItemDatabase.m_instance.items[currentSelectedItem.id].Drop1RateMax); // calculate random drop rate with min and max drop rate
        ran2 = Random.Range(ItemDatabase.m_instance.items[currentSelectedItem.id].Drop2RateMin, ItemDatabase.m_instance.items[currentSelectedItem.id].Drop2RateMax); // calculate random drop rate with min and max drop rate
        ran3 = Random.Range(ItemDatabase.m_instance.items[currentSelectedItem.id].Drop3RateMin, ItemDatabase.m_instance.items[currentSelectedItem.id].Drop3RateMax); // calculate random drop rate with min and max drop rate

        switch (currentSelectedItem.id)
        {
            case 8:
            case 9:
                currentSelectedItem.GO.GetComponent<Animator>().SetTrigger("TreeFalling"); //tree falling animation
                                                                                           //currentSelectedItem.GO.GetComponent<SpriteRenderer> ().color = new Color (1f, 1f, 1f, 1f);// Fixed issue when stup remains transperent if tree chopped from south facing
                break;
            case 99:
            case 100:
                if (currentSelectedItem.age == ItemDatabase.m_instance.items[currentSelectedItem.id].MaxAge)
                {  // if item age is max then drop max else drop 0
                    ran1 = Random.Range(ItemDatabase.m_instance.items[currentSelectedItem.id].Drop1RateMin, ItemDatabase.m_instance.items[currentSelectedItem.id].Drop1RateMax); // calculate random drop rate with min and max drop rate
                }
                else
                {
                    ran1 = 0;
                }
                break;
            default:
                break;
        }

        if (ItemDatabase.m_instance.items[currentSelectedItem.id].Drops1 >= 0)
        {
            InstansiateDropGameObject(ItemDatabase.m_instance.items[currentSelectedItem.id].Drops1, ran1);// drop item upon break
        }
        if (ItemDatabase.m_instance.items[currentSelectedItem.id].Drops2 >= 0)
        {
            InstansiateDropGameObject(ItemDatabase.m_instance.items[currentSelectedItem.id].Drops2, ran2);// drop item upon break
        }
        if (ItemDatabase.m_instance.items[currentSelectedItem.id].Drops3 >= 0)
        {
            InstansiateDropGameObject(ItemDatabase.m_instance.items[currentSelectedItem.id].Drops3, ran3);// drop item upon break
        }

        UpdateItemAndSaveToFile();  //update Gameobject and save in file
        currentSelectedItem = new item();// set current tile position to -1 i.e. invalid
        PlayerMovement.Instance.CalculateNearestItem(0, 0, false);
    }

    public void InstansiateDropGameObject(int id, int dropValue)
    {
        //print ("id " + id + " value " + dropValue);
        for (int i = 0; i < dropValue; i++)
        {
            //GameObject parent = new GameObject ();
            //parent.name = "parent";
            Vector2 ran = GameEventManager.currentSelectedTilePosition + Random.insideUnitCircle / 2;
            //parent.transform.position = new Vector3 (ran.x, ran.y, 0);
            GameObject drop = GameObject.Instantiate(droppedItem, new Vector3(ran.x, ran.y, 0), Quaternion.identity) as GameObject;
            drop.transform.localScale = GameEventManager.dropItemSize;
            //drop.transform.parent = parent.transform;
            drop.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = ItemDatabase.m_instance.items[id].Sprite;
            drop.transform.GetChild(0).GetComponent<Animator>().Play("itemDrop");
            drop.transform.GetComponent<DroppedItem>().droppedItemID = id;
            StartCoroutine(DropItemsLiveAfterSeconds(drop));
            //Inventory.m_instance.AddItem (id);	
        }
    }

    public void UpdateItemAndSaveToFile()
    {
        //print (currentSelectedItem.id);
        switch (currentSelectedItem.id)
        {
            case 8: //Replace Tree with stump
            case 9: //Replace Tree with stump
                PlayerMovement.Instance.itemSelector.transform.parent = this.transform;
                PlayerMovement.Instance.DisableItemSelector();
                LoadMapFromSave_PG.m_instance.SaveMapItemData(currentSelectedItem.id, currentSelectedItem.age, GameEventManager.currentSelectedTilePosition, onHarvest.RegrowToStump);
                break;
            case 120: //Berry Bush implement later
                if (currentSelectedItem.age == ItemDatabase.m_instance.items[currentSelectedItem.id].MaxAge)
                {
                    currentSelectedItem.age = 3;
                    LoadMapFromSave_PG.m_instance.SaveMapItemData(currentSelectedItem.id, currentSelectedItem.age, GameEventManager.currentSelectedTilePosition, onHarvest.Renewable);
                }
                else
                { //Replace Stump with nothing
                    LoadMapFromSave_PG.m_instance.SaveMapItemData(currentSelectedItem.id, currentSelectedItem.age, GameEventManager.currentSelectedTilePosition, onHarvest.Destory);
                }
                break;
            default:
                PlayerMovement.Instance.itemSelector.transform.parent = this.transform;
                PlayerMovement.Instance.DisableItemSelector();
                Destroy(currentSelectedItem.GO);
                LoadMapFromSave_PG.m_instance.SaveMapItemData(currentSelectedItem.id, currentSelectedItem.age, GameEventManager.currentSelectedTilePosition, onHarvest.Destory);
                break;
        }
    }

    private IEnumerator LateDestroyItems()
    {
        yield return new WaitForSeconds(0.75f);
    }

    private IEnumerator DropItemsLiveAfterSeconds(GameObject go)
    {
        yield return new WaitForSeconds(0.75f);
        go.transform.GetComponent<BoxCollider2D>().enabled = true;
        go.transform.GetComponent<DroppedItem>().isLive = true;
        //go.transform.DetachChildren ();
        //Destroy (go);
    }

    private int GetItemIDByIndex(string s, int index)
    {
        string[] sArray = s.Split(',');
        return int.Parse(sArray[index]);
    }
}

