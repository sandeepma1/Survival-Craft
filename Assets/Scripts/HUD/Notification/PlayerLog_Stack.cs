using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerLog_Stack : MonoBehaviour
{
	// Private VARS
	public static PlayerLog_Stack m_instance;
	private List<string> Eventlog = new List<string> ();
	//private string UIText = "";
	float timer = 0, fadeTimer = 0;

	// Public VARS
	public int maxLines = 10;
	public Text l1, l2, l3;
	public float fadeOutTime = 3;
	bool startFading = false;

	void Start ()
	{
		m_instance = this;
		Eventlog.Add ("");
		Eventlog.Add ("");
		Eventlog.Add ("");
	}

	public void AddEvent (string eventString)
	{
		Eventlog.Add (eventString);
		if (Eventlog.Count >= maxLines) {
			Eventlog.RemoveAt (0);
		}
//				print (Eventlog.Count);
		l1.text = Eventlog [0];
		l2.text = Eventlog [1];
		l3.text = Eventlog [2];
		timer = 0;
		StopCoroutine ("StopFading");
		StartCoroutine ("StopFading");
	}

	void Update ()
	{				
		if (Input.GetKeyDown (KeyCode.Space)) {
			AddEvent ("Some" + Random.Range (0, 100));
			//HelpText.MainNotification ("aaaaa");
		}
		/*if (l3.text != "") {
			timer += Time.deltaTime;
		}
		if (timer > fadeOutTime) {
			timer = 0;	
			//Eventlog.Clear ();
			startFading = true;
			StartCoroutine ("StopFading");
		}				
		if (startFading) {
			fadeTimer += Time.deltaTime;	
			switch ((fadeTimer.ToString ("F0"))) {
				case ("1"):	
					l1.text = "";								
					break;
				case ("2"):
					l2.text = "";
					break;				
				case ("3"):
					l3.text = "";
					break;
			}
		}*/
	}

	IEnumerator StopFading ()
	{
		yield return new WaitForSeconds (5.01f);
		Eventlog.Clear ();
		Eventlog.Add ("");
		l1.text = "";
		Eventlog.Add ("");
		l2.text = "";
		Eventlog.Add ("");
		l3.text = "";
		startFading = false;
		fadeTimer = 0;		
	}
}