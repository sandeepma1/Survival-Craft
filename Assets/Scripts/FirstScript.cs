using UnityEngine;
using System.Collections;

public class FirstScript : MonoBehaviour
{
	public GameObject inventoryMenu, mainCanvas, map;
	// Use this for initialization
	void Awake ()
	{		
		map.SetActive (false);
		mainCanvas.SetActive (true);
		inventoryMenu.SetActive (true); 
	}

	void Start ()
	{
		//mainCanvas.SetActive (true);
		//IGMMenu.m_instance.ToggleControls ("d");
	}
}
