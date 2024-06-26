﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftingSlot : MonoBehaviour, IPointerClickHandler,IPointerDownHandler
{
	public int id;
	public int itemID;

	void Start ()
	{
		print ("start");
	}

	public void OnPointerClick (PointerEventData eventData)
	{		
		SelectSlot ();
	}

	public void OnPointerDown (PointerEventData eventData)
	{		
		SelectSlot ();
	}

	public void SelectSlot ()
	{
		print ("click");
		Crafting.m_instance.slotSelectedImage.transform.parent = this.transform;
		Crafting.m_instance.slotSelectedImage.GetComponent <RectTransform> ().anchoredPosition = Vector3.zero;
		Crafting.m_instance.selectedItemID = itemID;
		Crafting.m_instance.slotSelectedImage.transform.SetAsLastSibling ();
	}
}
