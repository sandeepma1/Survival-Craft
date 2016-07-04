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
//			print (item);
			/*for (int i = 0; i < item.layoutSize; i++) {
				item [i].border.gameObject.SetActive (false);	
			}*/
	
			//	ItemClicked ();
//			print ("start");
			m_instance = this;
			if (!isSelected) {
				border.gameObject.SetActive (false);
			}
		}

		void ItemClicked ()
		{
			border.gameObject.SetActive (false);
			MoreInventoryButton.m_instance.RemoveBorder ();
			border.gameObject.SetActive (true);
			ActionManager.m_instance.GetCurrentWeildedTool (item);
			print (item.index);
		}
	}
}