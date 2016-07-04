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

		public float animationPivotAdjuster = 0.75f;

		private Vector3 cursorPosition;
		private int tempX, tempY = 0;
		private float input_x, input_y, r_input_a, r_input_b = 0f;
		private float passingTime = 0;
		private bool isWalking, isAttacking, isRightStick, isLeftStick = false;
		private float attackTime, attackCounter;

		Devdog.InventorySystem.InventoryItemBase currentWeildedItem, currentSelectedTile;

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
				ShowGrid ();
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

				if (isRightStick) {  //Attacking/working					
					AttackCalculation (Mathf.RoundToInt (r_input_a), Mathf.RoundToInt (r_input_b));
				} else {
					passingTime = 0;
					cursorTile.transform.position = new Vector3 (Mathf.Round (transform.position.x), Mathf.Round (transform.position.y - animationPivotAdjuster), Mathf.Round (transform.position.z));
					ActionManager.m_instance.isReadyToAttack = false;
				}

				if (isWalking) {  // Walking	
					WalkingCalculation (input_x, input_y);
					transform.position += new Vector3 (input_x, input_y, 0).normalized * Time.deltaTime * speed;
					return;
				}
				anim.SetBool ("isWalking", false);
			}
			//debugText.text = GameEventManager.currentSelectedTilePosition.ToString ();
		}

		public void AttackCalculation (int a, int b)
		{
			if (a == tempX && b == tempY) {
				return;
			}
			tempX = a;
			tempY = b;
			ActionManager.m_instance.isReadyToAttack = false;

			SetCursorTilePosition (Mathf.RoundToInt (r_input_a), Mathf.RoundToInt (r_input_b));

			ActionManager.m_instance.ActionButtonPressed ();


			anim.SetFloat ("a", Mathf.RoundToInt (r_input_a));
			anim.SetFloat ("b", Mathf.RoundToInt (r_input_b));

			//StopAllCoroutines ();
			//StartCoroutine (StartAction ());
		}

		public void WalkingCalculation (float x, float y)
		{
			anim.SetFloat ("x", input_x);
			anim.SetFloat ("y", input_y);
			anim.SetBool ("isWalking", isWalking);
		}

		/*IEnumerator StartAction ()
		{
			yield return new WaitForSeconds (1f);
			MapGenerator.m_instance.GetTileInfo (cursorPosition);
		}*/

		/*void LateUpdate ()
		{
			if (isAttacking) {
				passingTime += Time.deltaTime;
				debugText.text = passingTime.ToString ("F2");
				if (passingTime >= 1) {
					passingTime = 0;
					print ("damagePerSecond");
				}
			}
		}*/

		public void ShowGrid ()
		{
			cursorTile_grid.transform.position = new Vector3 (Mathf.RoundToInt (transform.position.x), Mathf.RoundToInt (transform.position.y - animationPivotAdjuster), Mathf.RoundToInt (transform.position.z));
		}

		public void SetCursorTilePosition (int a, int b)
		{
			cursorPosition = new Vector3 (Mathf.Round (transform.position.x), Mathf.Round (transform.position.y - animationPivotAdjuster), Mathf.Round (transform.position.z));
			cursorPosition.x += Mathf.Round (a);
			cursorPosition.y += Mathf.Round (b);
			cursorTile.transform.position = cursorPosition;
			GameEventManager.currentSelectedTilePosition = cursorTile.transform.position;
		}
	}
}
