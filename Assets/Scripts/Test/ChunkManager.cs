using UnityEngine;
using System.Collections;

public class ChunkManager : MonoBehaviour
{

	public GameObject chunk;
	GameObject[] chunkChildren;
	Vector3[] objPos;
	public static ChunkManager m_instance = null;
	// Use this for initialization
	void Start ()
	{
		m_instance = this;
		chunkChildren = new GameObject[chunk.transform.childCount];
		objPos = new Vector3[chunk.transform.childCount];
		for (int i = 0; i < chunk.transform.childCount; i++) {
			chunkChildren [i] = chunk.transform.GetChild (i).gameObject;
			objPos [i] = chunk.transform.GetChild (i).transform.localPosition;
		}
	}

	public void SendObjectInfo (Vector2 pos)
	{
		print ("");
	}
}
