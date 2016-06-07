using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
	public Transform target;
	public float cameraSmooth = 0.1f;
	Camera myCam;

	// Use this for initialization
	void Start ()
	{
		myCam = GetComponent<Camera> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		//myCam.orthographicSize = (Screen.height / 100f) / 4f; // Might remove
		if (target) {
			transform.position = Vector3.Lerp (transform.position, target.position, cameraSmooth) + new Vector3 (0, 0, -10);
		}

	}
}
