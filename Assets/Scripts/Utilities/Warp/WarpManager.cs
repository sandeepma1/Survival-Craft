using UnityEngine;
using System.Collections;

public class WarpManager : MonoBehaviour
{
	public static WarpManager m_instance = null;
	public WarpPoints[] warp;

	void Awake ()
	{
		m_instance = this;
	}

	public WarpPoints GetWarpCamera (int mapChunkPosition)
	{
		return warp [mapChunkPosition];
	}

	[System.Serializable]
	public struct WarpPoints
	{
		public float camMinX;
		public float camMaxX;
		public float camMinY;
		public float camMaxY;
	}
}

