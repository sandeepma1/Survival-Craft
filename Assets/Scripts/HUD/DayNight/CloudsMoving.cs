using UnityEngine;
using System.Collections;

public class CloudsMoving : MonoBehaviour
{
	public Transform target;
	public float speed;
	float step = 0;

	void Update ()
	{
		step = speed * Time.deltaTime;
		transform.position = Vector3.MoveTowards (transform.position, target.position, step);
		if (transform.localPosition.x >= 400) {
			Destory ();
		}
	}

	void Destory ()
	{
		transform.localPosition = new Vector3 (0, transform.localPosition.y);
		gameObject.SetActive (false);
	}
}
