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
		public UnityEngine.UI.Image border;
	
		bool isSelected = false;

		void Start ()
		{
			m_instance = this;
			if (!isSelected) {
				border.gameObject.SetActive (false);
			}

			if (PlayerPrefs.GetInt ("ItemSlotIndex") == (int)this.index) {				
				border.gameObject.SetActive (true);
				ActionManager.m_AC_instance.GetCurrentWeildedTool (item);
			}
			//print ();
		}

		public void ItemClicked ()
		{
			print ("clicked");
			border.gameObject.SetActive (false);
			MoreInventoryButton.m_instance.RemoveBorder ();
			border.gameObject.SetActive (true);
			ActionManager.m_AC_instance.GetCurrentWeildedTool (item);
			PlayerPrefs.SetInt ("ItemSlotIndex", (int)item.index);
		}
	}
}