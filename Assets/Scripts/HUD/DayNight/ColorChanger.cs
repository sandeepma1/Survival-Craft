using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ColorChanger : MonoBehaviour
{
	public float transitionDuration = 1;
	public Color targetColor;
	public Camera cam;
	public bool startTransition = false;
	float time = 0;

	void Start ()
	{
		time = transitionDuration;
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.C)) {
			startTransition = true;
		}

		if (startTransition) {
			time -= Time.deltaTime;
			cam.backgroundColor = Color.Lerp (cam.backgroundColor, targetColor, Time.deltaTime / time);
			print (time);
			if (time <= 0) {
				startTransition = false;
				time = transitionDuration;
			}
		}
	}
}

/*if (Input.GetKeyDown("space")){
     t = 0;
   }
   renderer.material.color = Color.Lerp(startColor, endColor, t);
   if (t < 1){ // while t below the end limit...
     // increment it at the desired rate every update:
     t += Time.deltaTime/duration;
   }*/
