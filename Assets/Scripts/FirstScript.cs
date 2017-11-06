using UnityEngine;
using System.Collections;

public class FirstScript : MonoBehaviour
{
    public GameObject[] objects;
    // Use this for initialization
    void Awake()
    {
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].SetActive(true);
        }
    }

    //void Start()
    //{
    //    if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Main_SB")
    //    {
    //        SetupCamera();
    //        mainMap.SetActive(false);
    //    }
    //}

    void SetupCamera()
    {
        /*Camera.main.transform.GetComponent <CameraFollow> ().minX = WarpManager.m_instance.warp [PlayerPrefs.GetInt ("mapChunkPosition")].camMinX;
		Camera.main.transform.GetComponent <CameraFollow> ().maxX = WarpManager.m_instance.warp [PlayerPrefs.GetInt ("mapChunkPosition")].camMaxX;
		Camera.main.transform.GetComponent <CameraFollow> ().minY = WarpManager.m_instance.warp [PlayerPrefs.GetInt ("mapChunkPosition")].camMinY;
		Camera.main.transform.GetComponent <CameraFollow> ().maxY = WarpManager.m_instance.warp [PlayerPrefs.GetInt ("mapChunkPosition")].camMaxY;*/
    }
}
