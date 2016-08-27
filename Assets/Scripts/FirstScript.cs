using UnityEngine;
using System.Collections;

public class FirstScript : MonoBehaviour
{
	public GameObject inventoryMenu, mainCanvas, mapReference, mainMap;
	// Use this for initialization
	void Awake ()
	{		
		mapReference.SetActive (false);
		mainMap.SetActive (false);
		mainCanvas.SetActive (true);
		inventoryMenu.SetActive (true); 
	}

	void Start ()
	{
		//mainCanvas.SetActive (true);
		//IGMMenu.m_instance.ToggleControls ("d");
	}
}
