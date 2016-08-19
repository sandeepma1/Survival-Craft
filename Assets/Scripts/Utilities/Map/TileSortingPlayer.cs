using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TileSortingPlayer : MonoBehaviour
{

	void LateUpdate ()
	{
		this.gameObject.GetComponent <SpriteRenderer> ().sortingOrder = (int)(transform.position.y * -10);
	
	}
}


