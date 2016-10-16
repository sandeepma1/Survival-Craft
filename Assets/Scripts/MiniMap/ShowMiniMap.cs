using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShowMiniMap : MonoBehaviour
{

	public GameObject miniMapMenu, quad;
	public Image miniMap;
	//Color[,] cmap;
	// Use this for initialization
	void Start ()
	{
		//RenderMiniMap ();
	}

	void RenderMiniMap ()
	{		
		Texture2D map = ES2.LoadImage ("Map.png");
		quad.GetComponent <MeshRenderer> ().material.mainTexture = map;
		miniMap.material.mainTexture = map;
	}

	public void ToggleMiniMap (bool flag)
	{		
		miniMapMenu.SetActive (flag);
		RenderMiniMap ();
	}


}
