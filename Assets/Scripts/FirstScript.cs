using UnityEngine;
using System.Collections;

public class FirstScript : MonoBehaviour
{
	//public GameObject mainCanvas;
	// Use this for initialization
	void Start ()
	{
		//mainCanvas.SetActive (true);
		IGMMenu.m_instance.ToggleControls ("d");
	}
}
