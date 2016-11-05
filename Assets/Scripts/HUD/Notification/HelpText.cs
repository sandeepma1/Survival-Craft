using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HelpText : MonoBehaviour
{
	public Text topNotification;
	GameObject helptext, helpTextOutline;
	static public string currentText, tempText;
	static public float textTimer = 4;
	// Use this for initialization
	void Awake ()
	{
		print ("safsdfds");
		tempText = "";
		currentText = "Survive as long as possible";
	}
	
	// Update is called once per frame
	void LateUpdate ()
	{
		topNotification.text = currentText;
	}

	public static void MainNotification (string text)
	{
		currentText = text;
		textTimer = 4;
	}

	void Update ()
	{	
		if (tempText != currentText) {						
			textTimer -= Time.deltaTime;
		}				
				
		//Removes Text
		if (textTimer <= 0) {
			textTimer = 0;
			currentText = "";
		}
		
	}
}
