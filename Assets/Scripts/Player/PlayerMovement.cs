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
	public GameObject camp;
	public Animator anim;
	public float speed = 1.5f, runSpeedMultiplier = 1.5f;
	public static PlayerMovement m_instance = null;
	public GameObject cursorTile, cursorTile_grid;

	public float animationPivotAdjuster = 0.75f;
	public float input_x, input_y, r_input_a, r_input_b = 0f;
	public Material SpriteOutline, SpriteUnlit;

	public bool isRunning;
	public bool SavePlayerLocation = true;
	public float autoPickupItemRadius = 3;
	public bool isRightStick, isLeftStick = false;

	private Vector3 cursorPosition;
	private int attackTempA = 0, attackTempB = 0, calculateNearestTempA = 0, calculateNearestTempB = 0;
	item[] playerSurroundings = new item[8];
	private float attackTime, attackCounter;
	Vector3 nearestItemPosition = Vector3.zero;
	bool walkTowards = false;
	bool actionButtonPressed = false;
	float speedTemp = 0;
	GameObject closestItemGO;
	//	List<Collider2D> cols = new List<Collider2D> ();
	//Devdog.InventorySystem.InventoryItemBase currentWeildedItem, currentSelectedTile;

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
		CalculateNearestItem (Mathf.RoundToInt (transform.position.x), Mathf.RoundToInt (transform.position.y), false);
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

			if (isRightStick && isLeftStick) {  // if both Right and Left stick are pressed
				isLeftStick = false;
			}

			if (isRightStick) {  //Attacking/working							
				AttackCalculation (Mathf.RoundToInt (r_input_a), Mathf.RoundToInt (r_input_b));
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
				CalculateNearestItem (Mathf.RoundToInt (transform.position.x), Mathf.RoundToInt (transform.position.y), true);
				WalkingCalculation (input_x, input_y);
				return;
			}
			anim.SetBool ("isWalking", false);

			if (walkTowards && actionButtonPressed) {
				if (nearestItemPosition != Vector3.zero) {
					AutoPickUpCalculation ();
				}			
			}
		}
	}

	void LateUpdate () // Set player storing order to front and Player Run toggle
	{
		this.gameObject.GetComponent<SpriteRenderer> ().sortingOrder = (int)(transform.position.y * -10); // can optimized
		if (Input.GetKey (KeyCode.LeftShift) || isRunning) {
			speed = speedTemp * runSpeedMultiplier;
		} else {
			speed = speedTemp;
		}
	}

	public void CalculateNearestItem (int a, int b, bool isOptimize)
	{
		if (isOptimize) {
			if (a == calculateNearestTempA && b == calculateNearestTempB && isOptimize) {
				return;
			}
			calculateNearestTempA = a;
			calculateNearestTempB = b;
		}
		Collider2D[] colliders = Physics2D.OverlapCircleAll (transform.position, autoPickupItemRadius, 1 << LayerMask.NameToLayer ("Default"));
		List<Collider2D> sortedColliders = new List<Collider2D> ();

		if (colliders.Length > 0) {
			foreach (var cols in colliders) {
				cols.gameObject.transform.GetChild (cols.gameObject.transform.childCount - 1).gameObject.SetActive (false);
				cols.gameObject.transform.GetChild (cols.gameObject.transform.childCount - 1).GetComponent <TextMesh> ().color = new Color (0.75f, 0.75f, 0.75f, 1);
				if (ItemDatabase.m_instance.items [GetItemID (cols.gameObject.name)].tool.ToString () == ActionManager.m_AC_instance.currentWeildedItem.rarity.name || ItemDatabase.m_instance.items [GetItemID (cols.gameObject.name)].tool.ToString () == "Hand") {
					sortedColliders.Add (cols);
				}
			}
			//print (sortedColliders [0].name);
		} else {
			sortedColliders.Clear ();
			nearestItemPosition = Vector3.zero;
			closestItemGO = null;
			return;
		}

		if (sortedColliders.Count > 0) {
			closestItemGO = GetClosestItem_List (sortedColliders).gameObject;
			closestItemGO.transform.GetChild (closestItemGO.gameObject.transform.childCount - 1).GetComponent <TextMesh> ().color = Color.white;
			nearestItemPosition = closestItemGO.transform.position;
			foreach (var cols in sortedColliders) {
				if (ItemDatabase.m_instance.items [GetItemID (cols.gameObject.name)].tool.ToString () == ActionManager.m_AC_instance.currentWeildedItem.rarity.name || ItemDatabase.m_instance.items [GetItemID (cols.gameObject.name)].tool.ToString () == "Hand") {
					cols.gameObject.transform.GetChild (cols.gameObject.transform.childCount - 1).gameObject.SetActive (true);
				}
			}
		} else {
			nearestItemPosition = Vector3.zero;
			closestItemGO = null;
		}
	}

	public void ActionButtonDown ()
	{		
		actionButtonPressed = true;
		CalculateNearestItem (Mathf.RoundToInt (transform.position.x), Mathf.RoundToInt (transform.position.y), false);

		if (nearestItemPosition != Vector3.zero) { //&& cols.Count > 0			
			walkTowards = true;
			GameEventManager.currentSelectedTilePosition = nearestItemPosition;
		}
	}

	public void ActionButtonUp ()
	{
		actionButtonPressed = false;
		ActionManager.m_AC_instance.isReadyToAttack = false;
		SetAttackAnimation (false);
	}

	public void AutoPickUpCalculation ()
	{
		print ("picking");
		anim.SetBool ("isWalking", true);
		if (Vector3.Distance (transform.position, nearestItemPosition) <= 1) {   //stop walking towards objects if less than 1 distance					
			Vector3 dir = (nearestItemPosition - transform.position).normalized;
			walkTowards = false;
			SetPickUpAnimation ();
			//anim.SetFloat ("PickUpX", Mathf.RoundToInt (dir.x));
			//anim.SetFloat ("PickUpY", Mathf.RoundToInt (dir.y));
			ActionManager.m_AC_instance.ActionButtonPressed ();
			//AttackCalculation (Mathf.RoundToInt (dir.x), Mathf.RoundToInt (dir.y));
			IsCursorEnable (true);
			return;
		}
		Vector3 playerDir = (nearestItemPosition - transform.position).normalized;
		WalkingCalculation (playerDir.x, playerDir.y);
		SetAttackAnimation (false);
	}

	public void PickUpCalculation (int a, int b)
	{	
		anim.SetFloat ("AttackingX", a);
		anim.SetFloat ("AttackingY", b);

		ActionManager.m_AC_instance.isReadyToAttack = false;
		SetCursorTilePosition (Mathf.RoundToInt (r_input_a), Mathf.RoundToInt (r_input_b));
		ActionManager.m_AC_instance.ActionButtonPressed ();
	}

	public void WalkingCalculation (float x, float y)
	{
		anim.SetFloat ("WalkingX", x);
		anim.SetFloat ("WalkingY", y);
		anim.SetBool ("isWalking", true);
		transform.position += new Vector3 (x, y, 0).normalized * Time.deltaTime * speed;
	}

	public void AttackCalculation (int a, int b)
	{		
		if (a == attackTempA && b == attackTempB) {
			return;
		}
		attackTempA = a;
		attackTempB = b;

		anim.SetFloat ("AttackingX", a);
		anim.SetFloat ("AttackingY", b);

		ActionManager.m_AC_instance.isReadyToAttack = false;
		SetCursorTilePosition (Mathf.RoundToInt (r_input_a), Mathf.RoundToInt (r_input_b));
		ActionManager.m_AC_instance.ActionButtonPressed ();
	}

	public void AttackCalculation ()
	{
		anim.SetFloat ("AttackingX", Mathf.RoundToInt (r_input_a));
		anim.SetFloat ("AttackingY", Mathf.RoundToInt (r_input_b));
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

	void OnTriggerEnter2D (Collider2D other)
	{
		/*switch (other.tag) {			
			case "Grass":
				other.GetComponent<Animator> ().enabled = true;
				other.GetComponent<Animator> ().SetTrigger ("shouldMove");
				break;
			default:
				break;
		}*/

		/*if (other.gameObject.transform.childCount > 0) {	
			print (ItemDatabase.m_instance.items [GetItemID (other.gameObject.name)].tool.ToString () + "==" + ActionManager.m_AC_instance.currentWeildedItem.rarity.name);	
			if (ItemDatabase.m_instance.items [GetItemID (other.gameObject.name)].tool.ToString () == ActionManager.m_AC_instance.currentWeildedItem.rarity.name) {
			other.gameObject.transform.GetChild (other.gameObject.transform.childCount - 1).gameObject.SetActive (true);
			}
		}*/
	}

	void OnTriggerExit2D (Collider2D other)
	{
		/*switch (other.tag) {			
			case "Grass":
				StartCoroutine (DisableItemsAfterTime (other));
				break;
			default:
				break;
		}*/

		// Disable item names
		if (other.gameObject.transform.childCount > 0) {
			other.gameObject.transform.GetChild (other.gameObject.transform.childCount - 1).gameObject.SetActive (false);
		}
	}

	IEnumerator DisableItemsAfterTime (Collider2D other)
	{
		yield return new WaitForSeconds (1);
		other.GetComponent<Animator> ().enabled = false;
	}

	Transform GetClosestItem_Cols (Collider2D[] colliders)
	{
		Transform tMin = null;		
		float minDist = Mathf.Infinity;
		if (colliders.Length == 1) {
			return colliders [0].transform;
		}
		foreach (Collider2D t in colliders) {
			float dist = Vector3.Distance (t.transform.position, transform.position);
			if (dist < minDist) {
				tMin = t.transform;
				minDist = dist;
			}
		}
		return tMin;
	}

	Transform GetClosestItem_List (List<Collider2D> colliders)
	{
		Transform tMin = null;		
		float minDist = Mathf.Infinity;
		if (colliders.Count == 1 && colliders [0] != null) {
			return colliders [0].transform;
		}
		foreach (Collider2D t in colliders) {
			if (t != null) {
				float dist = Vector3.Distance (t.transform.position, transform.position);
				if (dist < minDist) {
					tMin = t.transform;
					minDist = dist;
				}
			}
		}
		return tMin;
	}

	Transform GetClosestItem_Go (GameObject[] gos)
	{
		Transform tMin = null;		
		float minDist = Mathf.Infinity;
		if (gos.Length == 1 && gos [0] != null) {
			return gos [0].transform;
		}
		foreach (var t in gos) {
			if (t != null) {
				float dist = Vector3.Distance (t.transform.position, transform.position);
				if (dist < minDist) {
					tMin = t.transform;
					minDist = dist;
				}
			}
		}
		return tMin;
	}

	int GetItemID (string s)
	{
		string[] sArray = s.Split (',');
		return int.Parse (sArray [0]);
	}


}