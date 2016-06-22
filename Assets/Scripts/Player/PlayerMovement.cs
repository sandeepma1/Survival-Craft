using System;
using UnityEngine;
using UnityEngine.EventSystems;
using CnControls;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
	Animator anim;
	public float speed = 2.0f;
	public static PlayerMovement m_instance = null;
	public GameObject cursorTile;
	Vector3 cursorPosition;

	//controllers variables
	int tempX, tempY = 0;
	float input_x, input_y, r_input_x, r_input_y = 0f;

	bool isWalking, isRightStick = false;

	void Awake ()
	{		
		m_instance = this;
	}

	void Start ()
	{
		anim = GetComponent<Animator> ();	
	}

	void Update ()
	{
		if (GameEventManager.GetState () == GameEventManager.E_STATES.e_game) {
			input_x = CnInputManager.GetAxisRaw ("Horizontal");
			input_y = CnInputManager.GetAxisRaw ("Vertical");
			r_input_x = CnInputManager.GetAxisRaw ("Horizontal_Right");
			r_input_y = CnInputManager.GetAxisRaw ("Vertical_Right");

			isWalking = (Mathf.Abs (input_x) + Mathf.Abs (input_y)) > 0;
			isRightStick = (Mathf.Abs (r_input_x) + Mathf.Abs (r_input_y)) > 0;

			anim.SetBool ("isWalking", isWalking);

			if (isRightStick) {
				DualStickCalculation (Mathf.RoundToInt (r_input_x), Mathf.RoundToInt (r_input_y));
			}
			if (!isRightStick && PlayerPrefs.GetString ("Controls") == "d") {
				isCursorTileEnable (false);
				StopAllCoroutines ();
			}

			if (isWalking) {
				isCursorTileEnable (false);
				if (PlayerPrefs.GetString ("Controls") == "s") {				
					SingleButtonCalculation (input_x, input_y);							
				}
				anim.SetFloat ("x", input_x);
				anim.SetFloat ("y", input_y);
				transform.position += new Vector3 (input_x, input_y, 0).normalized * Time.deltaTime * speed;
			}
		}
	}

	public void isCursorTileEnable (bool flag)
	{
		cursorTile.SetActive (flag);
	}

	public void DualStickCalculation (int x, int y)
	{		
		if (x == tempX && y == tempY) {
			return;
		}
		tempX = x;
		tempY = y;
		cursorTile.SetActive (true);
		cursorPosition = new Vector3 (Mathf.Round (transform.position.x), Mathf.Round (transform.position.y - 0.75f), Mathf.Round (transform.position.z));
		cursorPosition.x += x;
		cursorPosition.y += y;
		cursorTile.transform.position = cursorPosition;
		StopAllCoroutines ();
		StartCoroutine (StartAction ());
	}

	public void SingleButtonCalculation (float x, float y)
	{
		cursorTile.SetActive (true);
		cursorPosition = new Vector3 (Mathf.Round (transform.position.x), Mathf.Round (transform.position.y - 0.75f), Mathf.Round (transform.position.z));
		cursorPosition.x += Mathf.Round (x);
		cursorPosition.y += Mathf.Round (y);
		cursorTile.transform.position = cursorPosition;
	}

	public void ActionButtonPressed ()
	{
		StopAllCoroutines ();
		StartCoroutine (StartAction ());
	}

	IEnumerator StartAction ()
	{
		yield return new WaitForSeconds (0.25f);
		MapGenerator.m_instance.GetTileInfo (cursorPosition);
	}

	public virtual void OnTriggerEnter (Collider col)
	{
		print (col + "Trigger");
		//TryPickup (col.gameObject);
	}
}
