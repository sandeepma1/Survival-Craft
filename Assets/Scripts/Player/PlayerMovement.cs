using System;
using UnityEngine;
using UnityEngine.EventSystems;
using CnControls;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Devdog.InventorySystem.UI;

//namespace Devdog.InventorySystem.Demo
//{
public class PlayerMovement : MonoBehaviour
{
	public Animator anim;
	public float speed = 2.0f, speedTemp = 0;
	public static PlayerMovement m_instance = null;
	public GameObject cursorTile, cursorTile_grid;

	public float animationPivotAdjuster = 0.75f;

	private Vector3 cursorPosition;
	private int tempX, tempY = 0;
	public float input_x, input_y, r_input_a, r_input_b = 0f;

	private bool isRightStick, isLeftStick = false;
	private float attackTime, attackCounter;

	Devdog.InventorySystem.InventoryItemBase currentWeildedItem, currentSelectedTile;

	void Awake ()
	{		
		m_instance = this;
		GetPlayerPosition ();
	}

	void Start ()
	{
		anim = GetComponent<Animator> ();	
		speedTemp = speed;	
		InvokeRepeating ("SavePlayerPosition", 0, 2);		

		ES2Settings setting = new ES2Settings ();
		setting.saveLocation = ES2Settings.SaveLocation.PlayerPrefs;
	}

	public void StopPlayer ()
	{
		anim.SetBool ("isWalking", false);
		GameEventManager.SetState (GameEventManager.E_STATES.e_pause);
	}

	public void StartPlayer ()
	{
		anim.SetBool ("isWalking", true);
		GameEventManager.SetState (GameEventManager.E_STATES.e_game);
	}

	void Update ()
	{
		if (GameEventManager.GetState () == GameEventManager.E_STATES.e_game) {
			ShowGrid ();
			input_x = CnInputManager.GetAxisRaw ("Horizontal");
			input_y = CnInputManager.GetAxisRaw ("Vertical");
			r_input_a = CnInputManager.GetAxisRaw ("Horizontal_Right");
			r_input_b = CnInputManager.GetAxisRaw ("Vertical_Right");

			isLeftStick = (Mathf.Abs (input_x) + Mathf.Abs (input_y)) > 0;
			isRightStick = (Mathf.Abs (r_input_a) + Mathf.Abs (r_input_b)) > 0;

			DebugTextHandler.m_instance.DisplayDebugText ("Left: " + isLeftStick + "Right: " + isRightStick);

			if (isRightStick && isLeftStick) {  // if both Right and Left stick are pressed
				isLeftStick = false;
			}

			if (isRightStick) {  //Attacking/working							
				AttackCalculation (Mathf.RoundToInt (r_input_a), Mathf.RoundToInt (r_input_b));
				IsCursorEnable (true);
			} else {				
				ActionManager.m_AC_instance.isReadyToAttack = false;
				IsCursorEnable (false);
				SetAttackAnimation (false);
			}

			if (isLeftStick) {  // Walking	
				WalkingCalculation (input_x, input_y);
				return;
			}

			anim.SetBool ("isWalking", false);
		}
	}

	public void AttackCalculation (int a, int b)
	{		
		if (a == tempX && b == tempY) {
			return;
		}
		tempX = a;
		tempY = b;

		anim.SetFloat ("x", a);
		anim.SetFloat ("y", b);

		ActionManager.m_AC_instance.isReadyToAttack = false;
		SetCursorTilePosition (Mathf.RoundToInt (r_input_a), Mathf.RoundToInt (r_input_b));
		ActionManager.m_AC_instance.ActionButtonPressed ();
	}

	public void WalkingCalculation (float x, float y)
	{
		anim.SetFloat ("x", input_x);
		anim.SetFloat ("y", input_y);
		anim.SetBool ("isWalking", isLeftStick);
		transform.position += new Vector3 (input_x, input_y, 0).normalized * Time.deltaTime * speed;
	}

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

	public void IsCursorEnable (bool flag)
	{
		cursorTile.GetComponent <SpriteRenderer> ().enabled = flag;
	}

	public void AttackCalculation ()
	{
		anim.SetFloat ("a", Mathf.RoundToInt (r_input_a));
		anim.SetFloat ("b", Mathf.RoundToInt (r_input_b));
	}

	public void SetAttackAnimation (bool flag)
	{
		anim.SetBool ("isAttacking", flag);
	}

	public void SavePlayerPosition ()
	{
		PlayerPrefs.SetFloat ("PlayerPositionX", transform.position.x);
		PlayerPrefs.SetFloat ("PlayerPositionY", transform.position.y);
	}

	public void GetPlayerPosition ()
	{		
		transform.position = new Vector3 (PlayerPrefs.GetFloat ("PlayerPositionX"), PlayerPrefs.GetFloat ("PlayerPositionY"), 0);
	}

	void LateUpdate () // Set player storing order to front and Player Run toggle
	{
		this.gameObject.GetComponent <SpriteRenderer> ().sortingOrder = (int)(transform.position.y * -10);
		if (Input.GetKey (KeyCode.LeftShift)) {
			speed = speedTemp * 2;
		} else {
			speed = speedTemp;
		}
	}
}
//}
