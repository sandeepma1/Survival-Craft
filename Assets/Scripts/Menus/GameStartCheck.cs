using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartCheck : MonoBehaviour
{
	public GameObject continueButton;
	// Use this for initialization
	void Awake ()
	{		
		string[] ass = ES2.GetFiles ("");
		if (ass.Length >= 2) {
			continueButton.SetActive (true);
		} else {
			continueButton.SetActive (false);
		}
	}
}
