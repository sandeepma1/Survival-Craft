using UnityEngine;
using System.Collections;

public class SpriteTest : MonoBehaviour
{
	int count = 0;
	Sprite[] sprite;
	SpriteRenderer src;

	void Start ()
	{
		sprite = Resources.LoadAll<Sprite> ("Textures/Map/Items/Trees/springTrees1_");
		StartCoroutine ("GrowTree");
		src = this.GetComponent <SpriteRenderer> ();
	}

	IEnumerator GrowTree ()
	{
		yield return new WaitForSeconds (1);
		src.sprite = sprite [count];

		count++;
		if (count <= 14) {
			StartCoroutine ("GrowTree");	
		}

	}

}
