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
	public float speed = 1.5f, speedTemp = 0;
	public static PlayerMovement m_instance = null;
	public GameObject cursorTile, cursorTile_grid;

	public float animationPivotAdjuster = 0.75f;
	public float input_x, input_y, r_input_a, r_input_b = 0f;
	public Material SpriteOutline, SpriteUnlit;

	public bool isRunning;
	public bool SavePlayerLocation = true;
	private Vector3 cursorPosition;
	private int tempX, tempY = 0;
	item[] playerSurroundings = new item[8];

	public bool isRightStick, isLeftStick = false;
	private float attackTime, attackCounter;

	Vector3 nearestItemPosition = Vector3.zero;
	bool walkTowards = false;
	bool actionButtonPressed = false;
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
		if (SavePlayerLocation) {
			InvokeRepeating ("SavePlayerPosition", 0, 2);
		}
		ES2Settings setting = new ES2Settings ();
		setting.saveLocation = ES2Settings.SaveLocation.PlayerPrefs;
		CalculateNearestItem ();
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

			//DebugTextHandler.m_instance.DisplayDebugText ("loc: " + GameEventManager.currentSelectedTilePosition);

			if (isRightStick && isLeftStick) {  // if both Right and Left stick are pressed
				isLeftStick = false;
			}

			if (isRightStick) {  //Attacking/working							
				AttackCalculation (Mathf.RoundToInt (r_input_a), Mathf.RoundToInt (r_input_b));
				print (Mathf.RoundToInt (r_input_a) + " " + Mathf.RoundToInt (r_input_b));
				IsCursorEnable (true);
			} else {
				AttackCalculation (Mathf.RoundToInt (r_input_a), Mathf.RoundToInt (r_input_b));
				//******************************************************************************************************************************************************
				//uncomment this
				//ActionManager.m_AC_instance.isReadyToAttack = false;
				//******************************************************************************************************************************************************
				IsCursorEnable (false);
				//SetAttackAnimation (false);
			}

			if (isLeftStick) {  // Walking	
				CalculateNearestItem ();
				WalkingCalculation (input_x, input_y);
				return;
			}
			anim.SetBool ("isWalking", false);
		}

		if (walkTowards && actionButtonPressed) {

			anim.SetBool ("isWalking", true);
			if (Vector3.Distance (transform.position, nearestItemPosition) <= 1) {   //stop walking towards objects if less than 1 distance					
				Vector3 dir = (nearestItemPosition - transform.position).normalized.normalized;
				walkTowards = false;
				print (dir);
				SetAttackAnimation (true);
				anim.SetFloat ("a", Mathf.RoundToInt (dir.x));
				anim.SetFloat ("b", Mathf.RoundToInt (dir.y));
				AttackCalculation (Mathf.RoundToInt (dir.x), Mathf.RoundToInt (dir.y));
				IsCursorEnable (true);
				return;
			}
			//transform.position = Vector3.Lerp (transform.position, nearestItemPosition, (speed / 1.25F) * Time.deltaTime);
			Vector3 playerDir = (nearestItemPosition - transform.position).normalized;
			transform.position += new Vector3 (playerDir.x, playerDir.y, 0).normalized * Time.deltaTime * speed;
			SetAttackAnimation (false);

			//anim.SetBool ("isWalking", true);
			anim.SetFloat ("x", Mathf.RoundToInt (playerDir.x));
			anim.SetFloat ("y", Mathf.RoundToInt (playerDir.y));

		}
	}

	void CalculateNearestItem ()
	{
		Collider2D[] colliders = Physics2D.OverlapCircleAll (transform.position, 3, 1 << LayerMask.NameToLayer ("Default"));
		//	DebugTextHandler.m_instance.DisplayDebugText (colliders.Length.ToString ());
		if (colliders.Length > 0) {			
			nearestItemPosition = GetClosestItem_c (colliders).transform.position;
		} else {
			nearestItemPosition = Vector3.zero;
		}
	}

	public void ActionButtonDown ()
	{
		actionButtonPressed = true;
		if (nearestItemPosition != Vector3.zero) {
			CalculateNearestItem ();
			walkTowards = true;
			GameEventManager.currentSelectedTilePosition = nearestItemPosition;
		}
	}

	public void ActionButtonUp ()
	{
		actionButtonPressed = false;
		ActionManager.m_AC_instance.isReadyToAttack = false;
		SetAttackAnimation (false);
		//	walkTowards = false;
		//AttackCalculation (Mathf.RoundToInt (r_input_a), Mathf.RoundToInt (r_input_b));
		/*ActionManager.m_AC_instance.isReadyToAttack = false;
		IsCursorEnable (false);
		SetAttackAnimation (false);*/
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

	public void SetCursorTilePosition (int a, int b)
	{
		cursorPosition = new Vector3 (Mathf.Round (transform.position.x), Mathf.Round (transform.position.y - animationPivotAdjuster), Mathf.Round (transform.position.z));
		cursorPosition.x += Mathf.Round (a);
		cursorPosition.y += Mathf.Round (b);
		cursorTile.transform.position = cursorPosition;
		if (isRightStick) { //Modified this to place items on map
			GameEventManager.currentSelectedTilePosition = cursorTile.transform.position;
		}
	}

	public void IsCursorEnable (bool flag)
	{
		cursorTile.GetComponent<SpriteRenderer> ().enabled = flag;
	}

	public void ShowGrid ()
	{
		cursorTile_grid.transform.position = new Vector3 (Mathf.RoundToInt (transform.position.x), Mathf.RoundToInt (transform.position.y - animationPivotAdjuster), Mathf.RoundToInt (transform.position.z));
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

	public void SetPickUpAnimation ()
	{
		anim.SetTrigger ("isPickingUp");
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
		this.gameObject.GetComponent<SpriteRenderer> ().sortingOrder = (int)(transform.position.y * -10);
		if (Input.GetKey (KeyCode.LeftShift) || isRunning) {
			speed = speedTemp * 1.5f;
		} else {
			speed = speedTemp;
		}
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		switch (other.tag) {
		/*case "Disappear":
				other.GetComponent<SpriteRenderer> ().color = new Color (1f, 1f, 1f, 0.45f);
				other.transform.parent.transform.GetChild (1).GetComponent<SpriteRenderer> ().color = new Color (1f, 1f, 1f, 0.45f);
				break;*/
			case "Grass":
				other.GetComponent<Animator> ().enabled = true;
				other.GetComponent<Animator> ().SetTrigger ("shouldMove");
				break;
			default:
				break;
		}
		if (other.gameObject.transform.childCount > 0) {
			other.gameObject.transform.GetChild (other.gameObject.transform.childCount - 1).gameObject.SetActive (true);
		}
	}

	void OnTriggerExit2D (Collider2D other)
	{
		switch (other.tag) {
		/*case "Disappear":
				other.GetComponent<SpriteRenderer> ().color = new Color (1f, 1f, 1f, 1f);
				other.transform.parent.transform.GetChild (1).GetComponent<SpriteRenderer> ().color = new Color (1f, 1f, 1f, 1f);
				break;*/
			case "Grass":
				StartCoroutine (DisableItemsAfterTime (other));
				break;
			default:
				break;
		}
		if (other.gameObject.transform.childCount > 0) {
			other.gameObject.transform.GetChild (other.gameObject.transform.childCount - 1).gameObject.SetActive (false);
		}
	}

	IEnumerator DisableItemsAfterTime (Collider2D other)
	{
		yield return new WaitForSeconds (1);
		other.GetComponent<Animator> ().enabled = false;
	}

	Transform GetClosestItem_c (Collider2D[] colliders)
	{
		Transform tMin = null;		
		float minDist = Mathf.Infinity;
		if (colliders.Length == 1) {
			return colliders [0].transform;
		}
		foreach (Collider2D t in colliders) {
			//if (t.gameObject.tag != "Player") {
			float dist = Vector3.Distance (t.transform.position, transform.position);
			if (dist < minDist) {
				tMin = t.transform;
				minDist = dist;
			}
		}
		return tMin;
	}
}
//}
