using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DebugTextHandler : MonoBehaviour
{

	public static DebugTextHandler m_instance = null;
	Text debugText;

	void Awake ()
	{
		m_instance = this;
		debugText = transform.GetComponent <Text> ();
	}

	public void DisplayDebugText (string text)
	{
		debugText.text = text;
	}
}
