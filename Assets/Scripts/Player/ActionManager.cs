using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ActionManager : MonoBehaviour
{

	public static ActionManager m_instance = null;
	public GameObject weaponGameObject;
	SpriteRenderer weaponSprite;

	Devdog.InventorySystem.InventoryItemBase item;

	void Awake ()
	{
		m_instance = this;
	}

	void Start ()
	{
		weaponSprite = weaponGameObject.GetComponent <SpriteRenderer> ();
	}

	public void GetSelectedItemObject (Devdog.InventorySystem.InventoryItemBase i)
	{
		item = i;
		print (item.icon);
	}
}
