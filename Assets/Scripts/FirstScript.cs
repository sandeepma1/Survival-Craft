using UnityEngine;
using System.Collections;

public class FirstScript : MonoBehaviour
{
	public GameObject inventoryMenu, mainCanvas, mapReference, mainMap;
	// Use this for initialization
	void Awake ()
	{		
		//mapReference.SetActive (false);
		mainCanvas.SetActive (true);
		inventoryMenu.SetActive (true); 
	}

	void Start ()
	{
		if (UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name == "Main_SB") {
			SetupCamera ();
			mainMap.SetActive (false);
		}
	}

	void SetupCamera ()
	{
		Camera.main.transform.GetComponent <CameraFollow> ().minX = WarpManager.m_instance.warp [Bronz.LocalStore.Instance.GetInt ("mapChunkPosition")].camMinX;
		Camera.main.transform.GetComponent <CameraFollow> ().maxX = WarpManager.m_instance.warp [Bronz.LocalStore.Instance.GetInt ("mapChunkPosition")].camMaxX;
		Camera.main.transform.GetComponent <CameraFollow> ().minY = WarpManager.m_instance.warp [Bronz.LocalStore.Instance.GetInt ("mapChunkPosition")].camMinY;
		Camera.main.transform.GetComponent <CameraFollow> ().maxY = WarpManager.m_instance.warp [Bronz.LocalStore.Instance.GetInt ("mapChunkPosition")].camMaxY;
	}
}
