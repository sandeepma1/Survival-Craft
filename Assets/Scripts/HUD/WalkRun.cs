using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WalkRun : MonoBehaviour
{
	public bool isRunning = false;
	public Image run, walk;

	void Start ()
	{
		ChangeImages ();
	}

	public void ToggleWalkRun ()
	{
		isRunning = !isRunning;
		ChangeImages ();
	}

	void ChangeImages ()
	{
		if (isRunning) {
			run.enabled = true;
			walk.enabled = false;
			PlayerMovement.m_instance.isRunning = true;
		} else {
			run.enabled = false;
			walk.enabled = true;
			PlayerMovement.m_instance.isRunning = false;
		}
	}
}
