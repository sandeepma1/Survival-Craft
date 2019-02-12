using UnityEngine;

public class FirstScript : MonoBehaviour
{
    [SerializeField] private GameObject inventoryMenu;
    [SerializeField] private GameObject mainCanvas;

    private void Awake()
    {
        mainCanvas.SetActive(true);
        inventoryMenu.SetActive(true);
    }
}
