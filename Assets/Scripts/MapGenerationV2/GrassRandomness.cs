using UnityEngine;
using System.Collections;

public class GrassRandomness : MonoBehaviour
{

	void Start ()
	{
		foreach (Transform child in transform) {
			child.localPosition = Random.insideUnitCircle;
			child.GetComponent <SpriteRenderer> ().sortingOrder = (int)(transform.localPosition.y * -10);
		}
	}

}
