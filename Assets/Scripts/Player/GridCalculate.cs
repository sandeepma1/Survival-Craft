using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class GridCalculate : MonoBehaviour
{
	public Transform playerTransform;
	public GameObject tile;
	Vector3 cursorPosition;


	public  static GridCalculate m_instance = null;

	void Awake ()
	{
		m_instance = this;
	}

	public void DisableCursorTile ()
	{
		tile.SetActive (false);
	}

	/*void FixedUpdate ()
	{
		playerPosition = new Vector3 (Mathf.Round (playerTransform.position.x), Mathf.Round (playerTransform.position.y - 0.75f), Mathf.Round (playerTransform.position.z));
		tile.transform.position = playerPosition;
	}
*/
	public void CalculatePlayerGrid (float x, float y)
	{
		tile.SetActive (true);
		cursorPosition = new Vector3 (Mathf.Round (playerTransform.position.x), Mathf.Round (playerTransform.position.y - 0.75f), Mathf.Round (playerTransform.position.z));
		cursorPosition.x += Mathf.Round (x);
		cursorPosition.y += Mathf.Round (y);
		tile.transform.position = cursorPosition;
		StartCoroutine (StartAction ());
	}

	IEnumerator StartAction ()
	{
		yield return new WaitForSeconds (1.5f);
//		print (cursorPosition);
		MapGenerator.m_instance.GetTileInfo (cursorPosition);
	}

}
