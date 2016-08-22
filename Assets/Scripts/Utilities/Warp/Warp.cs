using UnityEngine;
using System.Collections;

public class Warp : MonoBehaviour
{
	public Transform warpTarget;
	public float camMinX, camMaxX, camMinY, camMaxY;

	IEnumerator OnTriggerEnter2D (Collider2D other)
	{
		if (other.gameObject.tag == "Player") {

			ScreenFader sf = GameObject.FindGameObjectWithTag ("Fader").GetComponent<ScreenFader> ();
			yield return StartCoroutine (sf.FadeToBlack ());

			warpTarget.transform.root.gameObject.SetActive (true);

			print (transform.root.name);
			other.gameObject.transform.position = warpTarget.position;
			Camera.main.transform.position = warpTarget.position;

			Camera.main.transform.GetComponent <CameraFollow> ().minX = camMinX;
			Camera.main.transform.GetComponent <CameraFollow> ().maxX = camMaxX;
			Camera.main.transform.GetComponent <CameraFollow> ().minY = camMinY;
			Camera.main.transform.GetComponent <CameraFollow> ().maxY = camMaxY;
		
			yield return StartCoroutine (sf.FadeToClear ());
			transform.root.gameObject.SetActive (false);
		}
	}

}
