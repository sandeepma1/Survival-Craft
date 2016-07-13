using UnityEngine;
using System.Collections;

public class FirstScript : MonoBehaviour
{
	public GameObject inventoryMenu;
	// Use this for initialization
	void Awake ()
	{
		inventoryMenu.SetActive (true); 
	}

	void Start ()
	{
		//mainCanvas.SetActive (true);
		IGMMenu.m_instance.ToggleControls ("d");
	}
}
