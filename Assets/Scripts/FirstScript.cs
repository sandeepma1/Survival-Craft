using UnityEngine;
using System.Collections;

public class FirstScript : MonoBehaviour
{
	public GameObject inventoryMenu, mainCanvas;
	// Use this for initialization
	void Awake ()
	{
		mainCanvas.SetActive (true);
		inventoryMenu.SetActive (true); 
	}

	void Start ()
	{
		//mainCanvas.SetActive (true);
		//IGMMenu.m_instance.ToggleControls ("d");
	}
}
