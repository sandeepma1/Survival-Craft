using UnityEngine;
using System.Collections;

public class FirstScript : MonoBehaviour
{
	// Use this for initialization
	void Awake ()
	{
		IGMMenu.m_instance.ToggleControls ("d");
	}
}
