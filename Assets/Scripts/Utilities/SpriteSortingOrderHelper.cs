using UnityEngine;
using System.Collections;

public class SpriteSortingOrderHelper : MonoBehaviour
{
	public int order = 0;

	public void ChangeLayer (int n)
	{
		this.GetComponent <SpriteRenderer> ().sortingOrder = n;
	}
}
