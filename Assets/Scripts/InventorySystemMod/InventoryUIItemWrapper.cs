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
	public partial class InventoryUIItemWrapper
	{
		public UnityEngine.UI.Image border;
		bool isSelected = false;


		void Start ()
		{
			if (!isSelected) {
				border.gameObject.SetActive (false);
			}		
		}

		void ItemClicked ()
		{
			border.gameObject.SetActive (false);
			MoreInventoryButton.m_instance.RemoveBorder ();
			print (item);
			border.gameObject.SetActive (true);
		}
	}
}