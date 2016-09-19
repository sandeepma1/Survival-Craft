using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ColorChanger : MonoBehaviour
{
	public Color[] color;
	// Use this for initialization
	public Color lerpedColor;
	public float duration = 5;
	float time = 0;

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.C)) {
			time = 0;
		}
		transform.GetComponent <Image> ().color = Color.Lerp (color [0], color [1], time);
		if (time < 1) {
			time += Time.deltaTime / duration;
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
