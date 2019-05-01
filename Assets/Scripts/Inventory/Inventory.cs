using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory m_instance = null;
    public GameObject inventorySlotPanel, armourSlotPanel, chestSlotPanel;
    public GameObject inventorySlot, armorSlot, chestSlot;
    public GameObject inventoryItem;
    public int inventorySlotAmount = 10, armourSlotAmount = 4, chestSlotAmount = 6;
    public Image slotSelectedImage;
    public List<Item> l_items = new List<Item>();
    public List<GameObject> slotsGO = new List<GameObject>();
    public Item playerSelectedTool = null;
    //public Image selectedItemSprite;
    public Text selectedItemName;
    //, selectedItemDescription;
    public int selectedSlotID = -1;
    public int maxStackAmount = 10;
    public Text debug;
    private int inputFeildID = -1, inputFeildAmount = -1;
    private InventoryItems[] myInventory;

    private void Awake()
    {
        m_instance = this;
    }

    private void Start()
    {
        InitializeInventorySlots();
        AddFewItems();
        SelectFirstSlot();
        Crafting.m_instance.CheckHighlight_ALL_CraftableItems();
    }

    private void InitializeInventorySlots()
    {
        for (int i = 0; i < inventorySlotAmount; i++)
        {
            l_items.Add(new Item());
            slotsGO.Add(Instantiate(inventorySlot, inventorySlotPanel.transform));
            slotsGO[i].GetComponent<InventorySlot>().id = i;
            slotsGO[i].GetComponent<RectTransform>().localScale = Vector3.one;
        }
        for (int i = inventorySlotAmount; i < inventorySlotAmount + armourSlotAmount; i++)
        {
            l_items.Add(new Item());
            slotsGO.Add(Instantiate(armorSlot, armourSlotPanel.transform));
            slotsGO[i].GetComponent<ArmourSlot>().id = i;
            slotsGO[i].GetComponent<RectTransform>().localScale = Vector3.one;
        }
        for (int i = inventorySlotAmount + armourSlotAmount; i < inventorySlotAmount + armourSlotAmount + chestSlotAmount; i++)
        {
            l_items.Add(new Item());
            slotsGO.Add(Instantiate(chestSlot, chestSlotPanel.transform));
            slotsGO[i].GetComponent<ChestSlot>().id = i;
            slotsGO[i].GetComponent<RectTransform>().localScale = Vector3.one;
        }
        myInventory = new InventoryItems[slotsGO.Count];
    }

    private void AddFewItems()
    {
        AddItem(3);
        AddItem(3);
        AddItem(3);
        AddItem(5);
        AddItem(4);
        AddItem(5);
        AddItem(4);
        AddItem(5);
        AddItem(4);
        AddItem(5);
        AddItem(4);
        AddItem(5);
        AddItem(4);
        AddItem(5);
        AddItem(4);
        AddItem(5);
        AddItem(16);
        AddItem(17);
        AddItem(18);
    }

    private void SelectFirstSlot()
    {
        if (slotsGO[0].transform.GetChild(0).CompareTag("Item"))
        {
            playerSelectedTool = slotsGO[0].transform.GetChild(0).GetComponent<InventoryItemData>().item;
        }
        else
        {

        }
        InventorySlot firstSlot = slotsGO[0].GetComponent<InventorySlot>();
        firstSlot.SelectSlot();
    }

    public void AddItem(int id)
    {
        List<int> occurance = new List<int>();
        Item itemsToAdd = ItemDatabase.m_instance.FetchItemByID(id);

        if (itemsToAdd == null)
        {
            print("Item to add is NULL");
            return;
        }

        if (itemsToAdd.IsStackable)
        {
            for (int i = 0; i < l_items.Count - (armourSlotAmount + chestSlotAmount); i++)
            {
                if (l_items[i].ID == id)
                {
                    occurance.Add(i);
                }
            }
            if (occurance.Count > 0)
            {
                for (int i = 0; i < occurance.Count; i++)
                {
                    InventoryItemData data = slotsGO[occurance[i]].transform.GetChild(0).GetComponent<InventoryItemData>();
                    if (data.amount >= maxStackAmount)
                    {
                        if (i == occurance.Count - 1)
                        {
                            AddNewItemInUI(itemsToAdd);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        data.amount++;
                        data.transform.GetChild(0).GetComponent<Text>().text = data.amount.ToString();
                        break;
                    }
                }
            }
            else
            {
                AddNewItemInUI(itemsToAdd);
            }
        }
        else
        {
            AddNewItemInUI(itemsToAdd);
        }
        SaveInventoryItems();
        Crafting.m_instance.CheckHighlight_ALL_CraftableItems();

    }

    private void AddNewItemInUI(Item itemsToAdd)
    {
        if (CheckInventoryHasAtleastOneSpace())
        {
            for (int i = 0; i < l_items.Count - (armourSlotAmount + chestSlotAmount); i++)
            {
                if (l_items[i].ID == -1)
                {
                    l_items[i] = itemsToAdd;
                    GameObject itemsGO = Instantiate(inventoryItem, slotsGO[i].transform);
                    itemsGO.transform.SetAsFirstSibling();
                    itemsGO.GetComponent<RectTransform>().localScale = Vector3.one;
                    itemsGO.GetComponent<InventoryItemData>().slotID = i;
                    itemsGO.GetComponent<InventoryItemData>().type = itemsToAdd.Type;
                    if (itemsToAdd.Durability > 0)
                    {
                        itemsGO.GetComponent<InventoryItemData>().durability = itemsToAdd.Durability;
                        itemsGO.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(90, 10);
                    }
                    else
                    {
                        itemsGO.GetComponent<InventoryItemData>().durability = -1;
                        itemsGO.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(0, 10);
                    }
                    itemsGO.GetComponent<InventoryItemData>().item = itemsToAdd;
                    itemsGO.GetComponent<Image>().sprite = itemsToAdd.Sprite;
                    itemsGO.GetComponent<RectTransform>().anchoredPosition = Vector3.one;
                    break;
                }
            }
        }
    }

    public void RemoveItem(int id)
    {
        Item itemsToRemove = ItemDatabase.m_instance.FetchItemByID(id);
        //print ("removed " + itemsToRemove.Name);
        if (itemsToRemove.IsStackable)
        {
            for (int i = 0; i < l_items.Count - (armourSlotAmount + chestSlotAmount); i++)
            {
                if (l_items[i].ID == id)
                {
                    InventoryItemData data = slotsGO[i].transform.GetChild(0).GetComponent<InventoryItemData>();
                    if (data.amount > 1)
                    {
                        data.amount--;
                        data.transform.GetChild(0).GetComponent<Text>().text = data.amount.ToString();
                    }
                    else
                    {
                        DestroyItem(i);
                    }
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < l_items.Count; i++)
            {
                if (l_items[i].ID == id)
                {
                    slotsGO[i].transform.GetChild(0).GetComponent<InventoryItemData>().amount--;
                    DestroyItem(i);
                    break;
                }
            }
        }
        //SaveInventoryItems ();
        Crafting.m_instance.CheckHighlight_ALL_CraftableItems();
        ItemSelectedInInventory(selectedSlotID);
    }

    public void DeleteSelectedItem()
    {
        /*if (selectedSlotID >= 0 && slotsGO [selectedSlotID].transform.childCount > 0 && slotsGO [selectedSlotID].transform.GetChild (0).CompareTag ("Item")) {
			l_items [selectedSlotID] = new MyItem ();
			Destroy (slotsGO [selectedSlotID].transform.GetChild (0).gameObject);
		}*/
        DestroyItem(selectedSlotID);
        Crafting.m_instance.CheckHighlight_ALL_CraftableItems();
        ItemSelectedInInventory(selectedSlotID);
    }

    public void DestroyItem(int id)
    {
        for (int i = 0; i < slotsGO[id].transform.childCount; i++)
        {
            if (slotsGO[id].transform.GetChild(i).CompareTag("Item"))
            {
                DestroyImmediate(slotsGO[id].transform.GetChild(i).gameObject); // Be careful used DestroyImmediate, come here if there is any issue
                break;
            }
        }
        l_items[id] = new Item();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < slotsGO.Count; i++)
            {
                if (slotsGO[i].GetComponent<InventorySlot>())
                {
                    if (CheckIfSlotHasItem(i))
                    {
                        print("inv>>>>> " + slotsGO[i].GetComponent<InventorySlot>().id + " " + l_items[i].ID + " " +
                        slotsGO[i].GetComponent<InventorySlot>().transform.GetChild(0).GetComponent<InventoryItemData>().amount);
                    }
                    else
                    {
                        print("inv>>>>> " + slotsGO[i].GetComponent<InventorySlot>().id + " " + l_items[i].ID + " 0");
                    }
                }
                if (slotsGO[i].GetComponent<ArmourSlot>())
                {
                    if (CheckIfSlotHasItem(i))
                    {
                        print("arm  " + slotsGO[i].GetComponent<ArmourSlot>().id + " " + l_items[i].ID + " " +
                        slotsGO[i].GetComponent<ArmourSlot>().transform.GetChild(0).GetComponent<InventoryItemData>().amount);
                    }
                    else
                    {
                        print("arm  " + slotsGO[i].GetComponent<ArmourSlot>().id + " " + l_items[i].ID + " 0");
                    }
                }
                if (slotsGO[i].GetComponent<ChestSlot>())
                {
                    if (CheckIfSlotHasItem(i))
                    {
                        print("che0000000" + slotsGO[i].GetComponent<ChestSlot>().id + " " + l_items[i].ID + " " +
                        slotsGO[i].GetComponent<ChestSlot>().transform.GetChild(0).GetComponent<InventoryItemData>().amount);
                    }
                    else
                    {
                        print("che0000000 " + slotsGO[i].GetComponent<ChestSlot>().id + " " + l_items[i].ID + " 0");
                    }
                }
            }
        }
    }

    private bool CheckItemInInventory(Item item)
    {
        for (int i = 0; i < inventorySlotAmount; i++)
        {
            if (l_items[i].ID == item.ID)
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckIfSlotHasItem(int slotID)
    {
        if (slotsGO[slotID].transform.childCount > 0 && slotsGO[slotID].transform.GetChild(0).CompareTag("Item"))
        {
            return true;
        }
        return false;
    }

    public int CheckItemAmountInInventory(int id) //do this by slots or items saved
    {
        int amount = 0;
        for (int i = 0; i < inventorySlotAmount; i++)
        {
            if (slotsGO[i].transform.childCount > 0 && slotsGO[i].transform.GetChild(0).CompareTag("Item") && slotsGO[i].transform.GetChild(0).GetComponent<InventoryItemData>().item.ID == id)
            {
                amount += slotsGO[i].transform.GetChild(0).GetComponent<InventoryItemData>().amount;
            }
        }
        return amount;
    }

    public bool CheckInventoryHasAtleastOneSpace()
    {
        for (int i = 0; i < inventorySlotAmount; i++)
        {
            if (slotsGO[i].transform.childCount <= 0)
            {
                return true;
            }
            else if (!slotsGO[i].transform.GetChild(0).CompareTag("Item"))
            {
                return true;
            }
        }
        print("unable to add inventory full");
        return false;
    }

    private void SaveInventoryItems()
    {
        for (int i = 0; i < slotsGO.Count; i++)
        {
            if (slotsGO[i].transform.childCount > 0 && slotsGO[i].transform.GetChild(0).CompareTag("Item"))
            {
                myInventory[i] = new InventoryItems(slotsGO[i].transform.GetChild(0).GetComponent<InventoryItemData>().item.ID,
                    slotsGO[i].transform.GetChild(0).GetComponent<InventoryItemData>().amount,
                    slotsGO[i].transform.GetChild(0).GetComponent<InventoryItemData>().slotID,
                    slotsGO[i].transform.GetChild(0).GetComponent<InventoryItemData>().item.Durability);
            }
            else
            {
                myInventory[i] = new InventoryItems();
            }
        }
    }

    private void LoadInventoryItems()
    {
        InventoryItems[] myInventory = new InventoryItems[slotsGO.Count];// = new InventoryItems ();
        for (int i = 0; i < slotsGO.Count; i++)
        {
            if (slotsGO[i].transform.childCount > 0 && slotsGO[i].transform.GetChild(0).CompareTag("Item"))
            {
                myInventory[i].ID = slotsGO[i].transform.GetChild(0).GetComponent<InventoryItemData>().item.ID;
                myInventory[i].Amount = slotsGO[i].transform.GetChild(0).GetComponent<InventoryItemData>().amount;
                myInventory[i].SlotID = slotsGO[i].transform.GetChild(0).GetComponent<InventoryItemData>().slotID;
                myInventory[i].Health = slotsGO[i].transform.GetChild(0).GetComponent<InventoryItemData>().item.Durability;
            }
            else
            {
                myInventory[i] = new InventoryItems();
            }
        }
    }

    public void DecreseWeaponDurability(int amount)
    {
        if (selectedSlotID >= 0 && slotsGO[selectedSlotID].transform.childCount > 0 &&
            slotsGO[selectedSlotID].transform.GetChild(0).transform != null &
            slotsGO[selectedSlotID].transform.GetChild(0).CompareTag("Item") &&
            slotsGO[selectedSlotID].transform.GetChild(0).GetComponent<InventoryItemData>().durability >= 0)
        {

            slotsGO[selectedSlotID].transform.GetChild(0).GetComponent<InventoryItemData>().DecreaseItemDurability(amount);
        }
    }

    public void AddItemButton()
    {
        AddItem(inputFeildID);
    }

    public void RemoveItemButton()
    {
        RemoveItem(inputFeildID);
    }

    public void InputFeildID(string text)
    {
        int id;
        int.TryParse(text, out id);
        inputFeildID = id;
    }

    public void InputFeildAmount(string text)
    {
        int amount;
        int.TryParse(text, out amount);
        inputFeildAmount = amount;
    }

    public void ItemSelectedInInventory(int slotID)
    {
        if (slotsGO[slotID].transform.childCount > 0 && slotsGO[slotID].transform.GetChild(0).CompareTag("Item"))
        { // if some item selected
            playerSelectedTool = slotsGO[slotID].transform.GetChild(0).GetComponent<InventoryItemData>().item;
            selectedItemName.text = playerSelectedTool.Name;
            //selectedItemDescription.text = playerSelectedTool.Description;
            //selectedItemSprite.color = new Color (1, 1, 1, 1);
            //selectedItemSprite.sprite = playerSelectedTool.Sprite;
            if (playerSelectedTool.Type == ItemType.Tool || playerSelectedTool.Type == ItemType.Weapon)
            {
                PlayerMovement.Instance.SetPlayerWeaponInHand(playerSelectedTool.Sprite);
            }
            else
            {
                //PlayerMovement.Instance.SetPlayerWeaponInHand(new Sprite());
            }
        }
        else
        { // if selected slot has nothing
            playerSelectedTool = new Item();
            selectedItemName.text = "";
            //selectedItemDescription.text = "";
            //selectedItemSprite.color = new Color (0, 0, 0, 0);
            //PlayerMovement.Instance.SetPlayerWeaponInHand(new Sprite());
        }
        PlayerMovement.Instance.ActionCompleted(); // end player's all current action
        PlayerMovement.Instance.ToggleItemSelectorORItemPlacer(); // Toggle either ItemSelector OR ItemPlacer
    }
}

[System.Serializable]
public class InventoryItems
{
    public int ID { get; set; }
    public int Amount { get; set; }
    public int SlotID { get; set; }
    public int Health { get; set; }
    public InventoryItems(int id, int amount, int slotID, int health)
    {
        this.ID = id;
        this.Amount = amount;
        this.SlotID = slotID;
        this.Health = health;
    }
    public InventoryItems()
    {
        this.ID = -1;
        this.Amount = -1;
        this.SlotID = -1;
        this.Health = -1;
    }
}