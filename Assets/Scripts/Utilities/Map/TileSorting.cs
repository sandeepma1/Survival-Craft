using UnityEngine;

[ExecuteInEditMode]
public class TileSorting : MonoBehaviour
{
	void Start ()
	{
		this.gameObject.GetComponent <SpriteRenderer> ().sortingOrder = (int)(transform.position.y * -10);
	}
}
