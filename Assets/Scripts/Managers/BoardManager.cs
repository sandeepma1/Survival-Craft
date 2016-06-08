using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
	[Serializable]
	public class Count
	{
		public int minimum;
		public int maximum;

		public Count (int min, int max)
		{
			minimum = min;
			maximum = max;
		}
	}

	public int columns = 8;
	public int rows = 8;

	public Count grassCount = new Count (2, 7);
	public GameObject[] grasstiles;
	public GameObject[] floortiles;
	private Transform boardHolder;
	private List <Vector3> gridPosition = new List<Vector3> ();
	public static BoardManager m_instance = null;

	void Awake ()
	{
		m_instance = this;
	}

	// Use this for initialization
	void InitialiseList ()
	{
		gridPosition.Clear ();
		for (int x = 0; x < columns; x++) {
			for (int y = 0; y < rows; y++) {
				gridPosition.Add (new Vector3 (x, y, 0f));
			}
		}
	}
	
	// Update is called once per frame
	void BoardSetup ()
	{
		boardHolder = new GameObject ("Board").transform;
		for (int x = -1; x < columns + 1; x++) {
			for (int y = -1; y < rows + 1; y++) {
				GameObject toInstantiate = floortiles [Random.Range (0, floortiles.Length)];
				GameObject instance = Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
				instance.transform.SetParent (boardHolder);
			}
		}
	}

	Vector3 RandomPosition ()
	{
		int randomIndex = Random.Range (0, gridPosition.Count);
		Vector3 randomPosition = gridPosition [randomIndex];
		gridPosition.RemoveAt (randomIndex);
		return randomPosition;
	}

	void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum)
	{
		//Choose a random number of objects to instantiate within the minimum and maximum limits
		int objectCount = Random.Range (minimum, maximum + 1);
            
		//Instantiate objects until the randomly chosen limit objectCount is reached
		for (int i = 0; i < objectCount; i++) {
			//Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
			Vector3 randomPosition = RandomPosition ();
                
			//Choose a random tile from tileArray and assign it to tileChoice
			GameObject tileChoice = tileArray [Random.Range (0, tileArray.Length)];
                
			//Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
			Instantiate (tileChoice, randomPosition, Quaternion.identity);
		}
	}
        
        
	//SetupScene initializes our level and calls the previous functions to lay out the game board
	public void SetupScene ()
	{
		//Creates the outer walls and floor.
		BoardSetup ();
            
		//Reset our list of gridpositions.
		InitialiseList ();
            
		//Instantiate a random number of wall tiles based on minimum and maximum, at randomized positions.
		LayoutObjectAtRandom (grasstiles, grassCount.minimum, grassCount.maximum);
	}

	public void GetTileInfo (Vector2 tileInfo)
	{
		print (tileInfo);

	}
}
