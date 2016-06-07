using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class GridCalculate : MonoBehaviour
{
	public Transform playerTransform;
	public GameObject tile;
	Vector3 playerPosition;

	public  static GridCalculate m_instance = null;

	void Awake ()
	{
		m_instance = this;
		print (tile.name);
	}

	/*void FixedUpdate ()
	{
		playerPosition = new Vector3 (Mathf.Round (playerTransform.position.x), Mathf.Round (playerTransform.position.y - 0.75f), Mathf.Round (playerTransform.position.z));
		tile.transform.position = playerPosition;

	}
*/
	public void CalculatePlayerGrid (float x, float y)
	{
		playerPosition = new Vector3 (Mathf.Round (playerTransform.position.x), Mathf.Round (playerTransform.position.y - 0.75f), Mathf.Round (playerTransform.position.z));
		playerPosition.x += Mathf.Round (x);
		playerPosition.y += Mathf.Round (y);
		tile.transform.position = playerPosition;
	}
}
