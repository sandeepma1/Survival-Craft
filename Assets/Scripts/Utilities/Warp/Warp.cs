using UnityEngine;
using System.Collections;

public class Warp : MonoBehaviour
{
	public Transform warpTarget;

	IEnumerator OnTriggerEnter2D (Collider2D other)
	{
		if (other.gameObject.tag == "Player") {
			PlayerMovement.m_instance.StopPlayer ();
			ScreenFader sf = GameObject.FindGameObjectWithTag ("Fader").GetComponent<ScreenFader> ();
			yield return StartCoroutine (sf.FadeToBlack ());

			Bronz.LocalStore.Instance.SetInt ("mapChunkPosition", int.Parse (warpTarget.name.Substring (0, 1)));

			LoadMapFromSave_PG.m_instance.DisableUnusedMapChunks ();

			other.gameObject.transform.position = warpTarget.position;

			Camera.main.transform.position = warpTarget.position;

			Camera.main.transform.GetComponent <CameraFollow> ().minX = WarpManager.m_instance.warp [Bronz.LocalStore.Instance.GetInt ("mapChunkPosition")].camMinX;
			Camera.main.transform.GetComponent <CameraFollow> ().maxX = WarpManager.m_instance.warp [Bronz.LocalStore.Instance.GetInt ("mapChunkPosition")].camMaxX;
			Camera.main.transform.GetComponent <CameraFollow> ().minY = WarpManager.m_instance.warp [Bronz.LocalStore.Instance.GetInt ("mapChunkPosition")].camMinY;
			Camera.main.transform.GetComponent <CameraFollow> ().maxY = WarpManager.m_instance.warp [Bronz.LocalStore.Instance.GetInt ("mapChunkPosition")].camMaxY;
		
			yield return StartCoroutine (sf.FadeToClear ());
			PlayerMovement.m_instance.StartPlayer ();
		}
	}
}