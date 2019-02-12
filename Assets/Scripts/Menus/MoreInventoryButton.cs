using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MoreInventoryButton : MonoBehaviour
{
    public bool useLeftAnalogStick = false;
    public RectTransform mainUIWindow;
    public GameObject mainCanvas, inventoryMenu, buttonsAndInfoBox;
    public GameObject leftStick, actionButton;
    public SecondaryMenu e_secondaryMenu = SecondaryMenu.normal;
    public GameObject[] bottomMenusGO;

    private bool toggleCrafting = false, toggleInventory = false;
    private float heightAdjuster;

    private void Start()
    {
        heightAdjuster = ((mainCanvas.GetComponent<RectTransform>().rect.height / 2) + 150) * -1;
        ToggleCraftingSize(true);
        ToggleInventorySize();
    }

    private void ToggleInventorySize()
    {
        ToggleInventory(toggleInventory);
        if (toggleInventory)
        { // not show
            GameEventManager.SetState(GameEventManager.E_STATES.e_game);
            mainUIWindow.GetComponent<Image>().enabled = false;
            inventoryMenu.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -250);
            DisableRestMenus();
            GameEventManager.SetMenuState(GameEventManager.E_MenuState.e_menuDown);
        }
        else
        { // show
            GameEventManager.SetState(GameEventManager.E_STATES.e_pause);
            inventoryMenu.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            mainUIWindow.GetComponent<Image>().enabled = true;
            DisableRestMenus();
            bottomMenusGO[(int)e_secondaryMenu].SetActive(true);
            GameEventManager.SetMenuState(GameEventManager.E_MenuState.e_menuUp);
        }
    }

    public void OpenSecondaryMenu(int a)
    {
        e_secondaryMenu = (SecondaryMenu)a;
        ToggleInventorySize();
    }

    private void ToggleInventory(bool flag)
    {
        leftStick.SetActive(!flag);
        actionButton.SetActive(!flag);
        buttonsAndInfoBox.SetActive(flag);
        inventoryMenu.GetComponent<Image>().enabled = flag;
        toggleInventory = !flag;
    }

    private void DisableRestMenus()
    {
        foreach (var menu in bottomMenusGO)
        {
            menu.SetActive(false);
        }
    }

    public void ToggleCraftingSize(bool isCraftingDown)
    {
        ToggleCrafting(isCraftingDown);
        if (isCraftingDown)
        {
            GameEventManager.SetState(GameEventManager.E_STATES.e_game);
            //craftingMenu.SetActive (false);
            //mainUIWindow.anchoredPosition = new Vector2 (0, -270);
            GameEventManager.SetMenuState(GameEventManager.E_MenuState.e_menuDown);
        }
        else
        {
            GameEventManager.SetState(GameEventManager.E_STATES.e_pause);
            //craftingMenu.SetActive (true);
            //mainUIWindow.anchoredPosition = new Vector2 (0, -50);
            GameEventManager.SetMenuState(GameEventManager.E_MenuState.e_menuUp);
        }
    }

    public void ShowCraftingMenu(bool isCraftingDown)
    {
        ToggleInventory(isCraftingDown);
        if (isCraftingDown)
        {
            GameEventManager.SetState(GameEventManager.E_STATES.e_game);
            //mainUIWindow.anchoredPosition = new Vector3 (mainUIWindow.anchoredPosition.x, heightAdjuster);
            GameEventManager.SetMenuState(GameEventManager.E_MenuState.e_menuDown);
        }
        else
        {
            GameEventManager.SetState(GameEventManager.E_STATES.e_pause);
            //mainUIWindow.anchoredPosition = Vector3.zero;
            GameEventManager.SetMenuState(GameEventManager.E_MenuState.e_menuUp);
        }
    }

    private void ToggleCrafting(bool flag)
    {
        leftStick.SetActive(flag);
        //closeInventoryButton.SetActive (!flag);	
        actionButton.SetActive(flag);
        //inventoryUpButton.SetActive (flag);
        inventoryMenu.GetComponent<Image>().enabled = !flag;
    }

    public void ToggleCrafting()
    {
        toggleCrafting = !toggleCrafting;
    }

    private IEnumerator SaveToPNG()
    {
        yield return new WaitForEndOfFrame();
        Texture2D screenTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);
        screenTexture.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height), 0, 0);
        screenTexture.Apply();
        byte[] dataToSave = screenTexture.EncodeToPNG();
        string destination = Path.Combine(Application.persistentDataPath, System.DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".png");
        File.WriteAllBytes(destination, dataToSave);
    }
}

public enum SecondaryMenu
{
    normal,
    crafting,
    factory,
    anvil,
    waterPurifier
}