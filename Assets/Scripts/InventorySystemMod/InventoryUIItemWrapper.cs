using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Devdog.InventorySystem.Models;
using Devdog.InventorySystem.UI;
using UnityEngine.Assertions;

namespace Devdog.InventorySystem
{
	public partial class InventoryUIItemWrapper // My new partial class 
	{
		public static InventoryUIItemWrapper m_instance = null;
		public UnityEngine.UI.Image border, itemUseBar;
	
		bool isSelected = false;

		void Start ()
		{			
			m_instance = this;
			if (!isSelected) {
				border.gameObject.SetActive (false);
			}
			if (Bronz.LocalStore.Instance.GetInt ("ItemSlotIndex") == (int)this.index) {				
				border.gameObject.SetActive (true);
				ActionManager.m_AC_instance.GetCurrentWeildedTool (item);
			}
			StartCoroutine ("SetItem");
		}

		IEnumerator SetItem ()
		{
			yield return new WaitForSeconds (0.5f);
			ActionManager.m_AC_instance.GetCurrentWeildedTool (itemCollection [PlayerPrefs.GetInt ("ItemSlotIndex")].item);	
			if (item != null) {
				if (item.itemDurability > 1) { // if item have uses
					itemUseBar.gameObject.SetActive (true);
				}
			}
		}

		public void InventorySlotClicked ()
		{
			if (border != null) {
				border.gameObject.SetActive (false);
				itemUseBar.gameObject.SetActive (false);
				ActionManager.m_AC_instance.RemoveBorder ();
				ActionManager.m_AC_instance.UpdateAllItemsInInventory ();
				border.gameObject.SetActive (true);
				ActionManager.m_AC_instance.GetCurrentWeildedTool (item);
				PlayerPrefs.SetInt ("ItemSlotIndex", (int)this.index);
			}
			if (item != null) {
				if (item.itemDurability > 1) { // if item have uses
					itemUseBar.gameObject.SetActive (true);
					//print (item + " Durability " + item.itemDurability);
				}
			}	

		}
	}
}