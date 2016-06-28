using UnityEngine;
using System.Collections;

public class SpriteSortingOrderHelper : MonoBehaviour
{

	public void ChangeLayer (int n)
	{
		this.GetComponent <SpriteRenderer> ().sortingOrder = n;
	}
}
