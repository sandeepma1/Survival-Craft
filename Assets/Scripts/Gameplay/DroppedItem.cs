using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
	public int droppedItemID = -1;
	public float speed;
	public bool isLive = false;

	Transform target;

	void Start ()
	{
		target = GameObject.FindGameObjectWithTag ("Player").transform;
	}

	void Update ()
	{
		if (isLive) {
			float step = speed * Time.deltaTime;
			transform.position = Vector3.MoveTowards (transform.position, target.position, step);
			if (Vector3.Distance (transform.position, target.position) <= 0.2f) {				
				Inventory.m_instance.AddItem (droppedItemID);
				Destroy (gameObject);
			}
		}
	}
}
