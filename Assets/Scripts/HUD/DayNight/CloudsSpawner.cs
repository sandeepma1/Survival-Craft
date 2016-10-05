using UnityEngine;
using System.Collections;

public class CloudsSpawner : MonoBehaviour
{
	public float timeBetweenSpawns = 1;
	public GameObject[] cloudsPrefab;
	int cloudInUse = 0;
	float timeSinceLastSpawn;

	void Start ()
	{
		InvokeRepeating ("SpawnStuff", 1, timeBetweenSpawns);
	}


	void SpawnStuff ()
	{	
		int ranNo = Random.Range (0, cloudsPrefab.Length);

		if (ranNo != cloudInUse && !cloudsPrefab [ranNo].activeSelf && GameEventManager.GetState () == GameEventManager.E_STATES.e_game) {
			cloudsPrefab [ranNo].gameObject.SetActive (true);
			cloudInUse = ranNo;
			cloudsPrefab [ranNo].transform.localPosition = new Vector3 (this.transform.localPosition.x, Random.Range (30, -35), 0);
		}
	}
}
