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
		public float animationPivotAdjuster = 0.75f;

		private Vector3 cursorPosition;
		//private GameObject UIInfoBox = null;
		//private Devdog.InventorySystem.InfoBoxUI UIInfoBox = null;
		//InventoryPlayer p1;
		//controllers variables
		private int tempX, tempY = 0;
		private float input_x, input_y, r_input_a, r_input_b = 0f;
		private float passingTime = 0;


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
			ShowGrid ();
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
				r_input_a = CnInputManager.GetAxisRaw ("Horizontal_Right");
				r_input_b = CnInputManager.GetAxisRaw ("Vertical_Right");


				isWalking = (Mathf.Abs (input_x) + Mathf.Abs (input_y)) > 0;
				isRightStick = (Mathf.Abs (r_input_a) + Mathf.Abs (r_input_b)) > 0;

				isLeftStick = (Mathf.Abs (input_x) + Mathf.Abs (input_y)) > 0;
				isAttacking = (Mathf.Abs (r_input_a) + Mathf.Abs (r_input_b)) > 0;

				anim.SetBool ("isAttacking", isAttacking);
				anim.SetBool ("isAttacking", isAttacking);

				if (isRightStick) {				
					//print ("aaaaaaaaaaa");	
					//anim.SetBool ("isAttacking", isAttacking);
					DualStickCalculation (Mathf.RoundToInt (r_input_a), Mathf.RoundToInt (r_input_b));
					anim.SetFloat ("a", Mathf.RoundToInt (r_input_a));
					anim.SetFloat ("b", Mathf.RoundToInt (r_input_b));
					//isWalking = false;
					//print (UIInfoBox.GetComponent <>());
					//print (UIInfoBox.GetComponent<Devdog.InventorySystem.InfoBoxUI> ().uiName.text);
					//Debug.Log ("Currently hovering: " + InventoryUIUtility.currentlyHoveringWrapper.item.name);
				}
				if (!isRightStick) {
					passingTime = 0;
					debugText.text = passingTime.ToString ();
				}

				if (isWalking) { 
					anim.SetBool ("isWalking", isWalking);	
					SingleButtonCalculation (input_x, input_y);		
					anim.SetFloat ("x", input_x);
					anim.SetFloat ("y", input_y);
					transform.position += new Vector3 (input_x, input_y, 0).normalized * Time.deltaTime * speed;
					ShowGrid ();
					return;
				}

				anim.SetBool ("isWalking", false);
			}
		}

		public void ShowGrid ()
		{
			cursorTile_grid.transform.position = new Vector3 (Mathf.RoundToInt (transform.position.x), Mathf.RoundToInt (transform.position.y - animationPivotAdjuster), Mathf.RoundToInt (transform.position.z));
		}

		public void isCursorTileEnable (bool flag)
		{
			cursorTile.SetActive (true);
		}

		public void DualStickCalculation (int x, int y)
		{		
			if (x == tempX && y == tempY) {
				return;
			}
			tempX = x;
			tempY = y;
			isCursorTileEnable (true);
			cursorPosition = new Vector3 (Mathf.Round (transform.position.x), Mathf.Round (transform.position.y - animationPivotAdjuster), Mathf.Round (transform.position.z));
			cursorPosition.x += Mathf.Round (x);
			cursorPosition.y += Mathf.Round (y);
			cursorTile.transform.position = cursorPosition;

			ShowGrid ();
			StopAllCoroutines ();
			StartCoroutine (StartAction ());
		}

		public void SingleButtonCalculation (float x, float y)
		{
			isCursorTileEnable (true);
			cursorPosition = new Vector3 (Mathf.Round (transform.position.x), Mathf.Round (transform.position.y - animationPivotAdjuster), Mathf.Round (transform.position.z));
			cursorPosition.x += Mathf.Round (x);
			cursorPosition.y += Mathf.Round (y);
			cursorTile.transform.position = cursorPosition;
			cursorTile.transform.position = new Vector3 (Mathf.Round (transform.position.x), Mathf.Round (transform.position.y - animationPivotAdjuster), Mathf.Round (transform.position.z));
		}

		public void ActionButtonPressed ()
		{
			//MapGenerator.m_instance.GetTile (cursorPosition);
			//print ("aaaaaaaaaaa");
			StopAllCoroutines ();
			StartCoroutine (StartAction ());
		}

		IEnumerator StartAction ()
		{

			yield return new WaitForSeconds (0.75f);
			MapGenerator.m_instance.GetTileInfo (cursorPosition);
		}

		IEnumerator StartFadingAnimation ()
		{
			yield return new WaitForSeconds (0.75f);
		}

		public virtual void OnTriggerEnter (Collider col)
		{
			print (col + "Trigger");
		}

		void LateUpdate ()
		{
			if (isAttacking) {
				passingTime += Time.deltaTime;
				debugText.text = passingTime.ToString ();
			}

		}
	}
}
