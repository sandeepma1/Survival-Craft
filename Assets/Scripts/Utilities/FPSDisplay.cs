using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
	public bool enableFPS = true;
	public Text fpsText;
	float deltaTime = 0.0f;
	float msec;
	float fps;
	string text;

 
	void LateUpdate ()
	{
		if (enableFPS) {
			deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
			msec = deltaTime * 1000.0f;
			fps = 1.0f / deltaTime;
			text = string.Format ("{0:0.0} ms ({1:0.} fps)", msec, fps);
			fpsText.text = text;
		}
	}
}