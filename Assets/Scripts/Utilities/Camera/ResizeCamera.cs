using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Camera))]
public class ResizeCamera : MonoBehaviour
{
	private float DesignOrthographicSize;
	private float DesignAspect;
	private float DesignWidth;

	public  float DesignAspectHeight;
	public  float DesignAspectWidth;

	public void Awake ()
	{
		DesignAspectHeight = Screen.height;
		DesignAspectWidth = Screen.width;
		DesignOrthographicSize = Camera.main.orthographicSize;
		DesignAspect = DesignAspectHeight / DesignAspectWidth;
		DesignWidth = DesignOrthographicSize * DesignAspect;

		Resize ();
	}

	public void Resize ()
	{       
		float wantedSize = DesignWidth / Camera.main.aspect;
		Camera.main.orthographicSize = Mathf.Max (wantedSize, DesignOrthographicSize);
		print (DesignWidth);
	}
}
