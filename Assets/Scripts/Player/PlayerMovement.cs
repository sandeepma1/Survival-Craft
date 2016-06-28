using System;
using UnityEngine;
using UnityEngine.EventSystems;
using CnControls;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Devdog.InventorySystem.UI;

namespace Devdog.InventorySystem.Demo
{
	public class PlayerMovement : MonoBehaviour
	{
		Animator anim;
		public float speed = 2.0f;
		public static PlayerMovement m_instance = null;
		public GameObject cursorTile, cursorTile_grid;
		public Text debugText;

		private Vector3 cursorPosition;
		//private GameObject UIInfoBox = null;
		//private Devdog.InventorySystem.InfoBoxUI UIInfoBox = null;
		//InventoryPlayer p1;
		//controllers variables
		private int tempX, tempY = 0;
		private float input_x, input_y, r_input_x, r_input_y = 0f;

		private bool isWalking, isAttacking, isRightStick, isLeftStick = false;
		private float attackTime, attackCounter;


		void Awake ()
		{		
			m_instance = this;
			//p1 = transform.Find ("RangeHelper").GetComponent <InventoryPlayer> ();
		
		}

		void Start ()
		{
			anim = GetComponent<Animator> ();	
			//UIInfoBox = GameObject.Find ("InfoBox").GetComponent <Devdog.InventorySystem.InfoBoxUI> ();
			//UIInfoBox = GameObject.Find ("InfoBox");
			//'print (UIInfoBox.name);

		}

		void Update ()
		{
//			debugText.text = (InventoryUIUtility.currentlyHoveringWrapper.item.name);
			//	Debug.Log ("Currently hovering: " + InventoryUIUtility.currentlyHoveringWrapper.item.name);
			if (GameEventManager.GetState () == GameEventManager.E_STATES.e_game) {
				input_x = CnInputManager.GetAxisRaw ("Horizontal");
				input_y = CnInputManager.GetAxisRaw ("Vertical");
				r_input_x = CnInputManager.GetAxisRaw ("Horizontal_Right");
				r_input_y = CnInputManager.GetAxisRaw ("Vertical_Right");


				isWalking = (Mathf.Abs (input_x) + Mathf.Abs (input_y)) > 0;
				isRightStick = (Mathf.Abs (r_input_x) + Mathf.Abs (r_input_y)) > 0;
				isLeftStick = (Mathf.Abs (input_x) + Mathf.Abs (input_y)) > 0;
				isAttacking = (Mathf.Abs (r_input_x) + Mathf.Abs (r_input_y)) > 0;

				anim.SetBool ("isWalking", isWalking);
				anim.SetBool ("isAttacking", isAttacking);

				if (isRightStick) {
					DualStickCalculation (Mathf.RoundToInt (r_input_x), Mathf.RoundToInt (r_input_y));
					anim.SetFloat ("x", r_input_x);
					anim.SetFloat ("y", r_input_y);
					isWalking = false;
					//print (UIInfoBox.GetComponent <>());
					//print (UIInfoBox.GetComponent<Devdog.InventorySystem.InfoBoxUI> ().uiName.text);
					//Debug.Log ("Currently hovering: " + InventoryUIUtility.currentlyHoveringWrapper.item.name);
				}
				if (!isRightStick && PlayerPrefs.GetString ("Controls") == "d") {
					isCursorTileEnable (false);
					StopAllCoroutines ();
				}

				if (isWalking) {
					isRightStick = false;
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

		public void ShowGrid ()
		{
			cursorTile_grid.transform.position = new Vector3 (Mathf.RoundToInt (transform.position.x), Mathf.RoundToInt (transform.position.y - 0.75f), Mathf.RoundToInt (transform.position.z));
		}

		public void isCursorTileEnable (bool flag)
		{
			cursorTile.SetActive (flag);
			cursorTile_grid.SetActive (flag);
		}

		public void DualStickCalculation (int x, int y)
		{		
			if (x == tempX && y == tempY) {
				return;
			}
			tempX = x;
			tempY = y;
			isCursorTileEnable (true);
			cursorPosition = new Vector3 (Mathf.Round (transform.position.x), Mathf.Round (transform.position.y - 0.75f), Mathf.Round (transform.position.z));
			cursorPosition.x += x;
			cursorPosition.y += y;
			cursorTile.transform.position = cursorPosition;
			ShowGrid ();
			StopAllCoroutines ();
			StartCoroutine (StartAction ());
		}

		public void SingleButtonCalculation (float x, float y)
		{
			isCursorTileEnable (true);
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
		}
	}
}
