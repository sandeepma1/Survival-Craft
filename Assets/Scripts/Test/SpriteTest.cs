using UnityEngine;
using System.Collections;

public class SpriteTest : MonoBehaviour
{
	public Sprite[] tree;
	int count = 0;
	SpriteRenderer src;

	void Start ()
	{
		//StartCoroutine ("GrowTree");
		src = this.GetComponent <SpriteRenderer> ();
	}

	IEnumerator GrowTree ()
	{
		yield return new WaitForSeconds (1);

		src.sprite = tree [count];

		count++;
		if (count <= 14) {
			StartCoroutine ("GrowTree");	
		}

	}

}
