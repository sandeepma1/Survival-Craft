﻿using System;
using UnityEngine;
using UnityEngine.EventSystems;
using CnControls;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
	public static PlayerMovement m_instance = null;
	public GameObject touchCameraGO, characterGO;
	public SpriteRenderer playerHead, playerEyes, playerBody, playerLimbLeft, playerLimbRight, playerLegLeft, playerLegRight, playerRightWeapon;
	public Animator anim;
	public float speed = 1.5f, speedTemp = 0, runSpeedMultiplier = 1.5f, runSpeedMultiplierTemp = 0;

	public GameObject itemPlacer, itemSelector, moreButton, actionButtonImage;

	public float animationPivotAdjuster = 0.75f, playerEyesBounds = 0.7f;
	public float input_x, input_y, r_input_a, r_input_b = 0f;
	public Material SpriteOutline, SpriteUnlit;

	public bool isRunning, isPlayerRunning = false;
	public bool SavePlayerLocation = true;
	public float autoPickupItemRadius = 1f;
	public bool isRightStick, isLeftStick = false;
	public bool isPlayerNearFire = false;

	private Vector3 cursorPosition;
	private int attackTempA = 0, attackTempB = 0, calculateNearestTempA = 0, calculateNearestTempB = 0;
	item[] playerSurroundings = new item[8];
	private float attackTime, attackCounter;
	Vector3 nearestItemPosition = Vector3.zero;
	bool walkTowards = false;
	bool actionButtonPressed = false;
	//	public float speedTemp = 0;
	GameObject closestItemGO;
	public Color visibleColor, nearstColor;
	public bool isBuildingPlacable = false;


	//	List<Collider2D> cols = new List<Collider2D> ();
	//Devdog.InventorySystem.InventoryItemBase currentWeildedItem, currentSelectedTile;

	void Awake ()
	{
		m_instance = this;
		GetPlayerPosition ();
	}

	void Start ()
	{
		//anim = GetComponent<Animator> ();
		speedTemp = speed;
		runSpeedMultiplierTemp = runSpeedMultiplier;
		if (SavePlayerLocation) {
			InvokeRepeating ("SavePlayerPosition", 0, 2);
		}
		ES2Settings setting = new ES2Settings ();
		setting.saveLocation = ES2Settings.SaveLocation.PlayerPrefs;

		//IsCursorEnable (false);
		CircleCollider2D[] cols = GetComponents <CircleCollider2D> ();
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
			input_x = CnInputManager.GetAxisRaw ("Horizontal");
			input_y = CnInputManager.GetAxisRaw ("Vertical");

			isLeftStick = (Mathf.Abs (input_x) + Mathf.Abs (input_y)) > 0;

			if (isLeftStick) {  // Walking	
				ActionCompleted ();
				LoadMapFromSave_PG.m_instance.PlayerTerrianState ((int)transform.position.x, (int)transform.position.y);
				ToggleItemSelectorORItemPlacer ();
				WalkingCalculation (input_x, input_y);

				return;
			}
			anim.SetBool ("isWalking", false);
			anim.SetBool ("isRunning", false);

			if (walkTowards) {
				if (nearestItemPosition != Vector3.zero) {
					AutoPickUpCalculation ();
				}			
			}
		}
	}

	public void ToggleItemSelectorORItemPlacer ()
	{
		if (IsPlayerBuildingSelected ()) {// if selected item is building
			SetCursorTilePosition (Mathf.RoundToInt (input_x), Mathf.RoundToInt (input_y));
			DisableItemSelector ();
			if (LoadMapFromSave_PG.m_instance.GetTile (cursorPosition).GO == null) {
				itemPlacer.GetComponent <SpriteRenderer> ().color = Color.white;
				isBuildingPlacable = true;
			} else {
				itemPlacer.GetComponent <SpriteRenderer> ().color = Color.red;
				isBuildingPlacable = false;
			}
		} else {
			SetCursorTilePosition (5000, 5000);
			CalculateNearestItem (Mathf.RoundToInt (transform.position.x), Mathf.RoundToInt (transform.position.y), false);
		}
	}

	public bool IsPlayerBuildingSelected ()
	{
		if (Inventory.m_instance.playerSelectedTool.Type == ItemType.Build) {			
			return true;
		}
		return false;		
	}

	void LateUpdate () // Set player storing order to front and Player Run toggle
	{		
		playerHead.sortingOrder = (int)(transform.position.y * -10) + 1;
		playerBody.sortingOrder = (int)(transform.position.y * -10);
		playerLimbLeft.sortingOrder = (int)(transform.position.y * -10) + 2;
		playerLimbRight.sortingOrder = (int)(transform.position.y * -10) - 2;
		playerRightWeapon.sortingOrder = (int)(transform.position.y * -10) - 2;
		playerLegLeft.sortingOrder = (int)(transform.position.y * -10) + 1;
		playerLegRight.sortingOrder = (int)(transform.position.y * -10) - 1;

		/*if (Input.GetKey (KeyCode.LeftShift) || isRunning) { Enable this for speed run
			speed = speedTemp * runSpeedMultiplier;
			isPlayerRunning = true;
		} else {
			speed = speedTemp;
			isPlayerRunning = false;
		}*/
	}

	public void SetPlayerWeaponInHand (Sprite sprite)
	{
		playerRightWeapon.sprite = sprite;
	}

	public void CalculateNearestItem (int a, int b, bool isOptimize)
	{
		if (isOptimize) {
			/*if (a == calculateNearestTempA && b == calculateNearestTempB && isOptimize) {
				return;
			}*/
			calculateNearestTempA = a;
			calculateNearestTempB = b;
		}
		Collider2D[] colliders = Physics2D.OverlapCircleAll (transform.position, autoPickupItemRadius, 1 << LayerMask.NameToLayer ("Default"));
		List<Collider2D> sortedColliders = new List<Collider2D> ();

		if (colliders.Length > 0) {
			foreach (var cols in colliders) {				
				if (cols.tag == "Item") {
					sortedColliders.Add (cols);
				}
			}
		} else {
			sortedColliders.Clear ();
			nearestItemPosition = Vector3.zero;
			closestItemGO = null;
			DisableItemSelector ();
			return;
		}

		if (sortedColliders.Count > 0) {
			closestItemGO = GetClosestItem_List (sortedColliders).gameObject;
			//print (ActionManager.m_instance.currentSelectedItem.age);
			nearestItemPosition = closestItemGO.transform.position;	

			if (ItemDatabase.m_instance.FetchItemByID (LoadMapFromSave_PG.m_instance.GetTile (nearestItemPosition).id).Type == ItemType.Build) {
				moreButton.SetActive (true);
			} else {
				moreButton.SetActive (false);
			}
		} else {
			nearestItemPosition = Vector3.zero;
			closestItemGO = null;
		}
	}

	public void DisableItemSelector ()
	{
		itemSelector.transform.position = new Vector3 (1000, 1000, 1000);
	}

	public void ActionButtonDown ()
	{
		//SetIdleAnimation (false);
		actionButtonPressed = true;
		if (IsPlayerBuildingSelected ()) {
			ActionManager.m_instance.ActionButtonPressed ();
		} else {
			CalculateNearestItem (Mathf.RoundToInt (transform.position.x), Mathf.RoundToInt (transform.position.y), true);
			if (nearestItemPosition != Vector3.zero) { //&& cols.Count > 0			
				walkTowards = true;
				GameEventManager.currentSelectedTilePosition = nearestItemPosition;
			}
		}
	}

	public void ActionButtonUp ()
	{		
		walkTowards = false;
		ActionCompleted ();	
	}

	public void MoreButtonDown ()
	{

	}

	public void MoreButtonUp ()
	{

	}

	public void ActionCompleted ()
	{
		//SetIdleAnimation (true);
		actionButtonPressed = false;
		ActionManager.m_instance.isReadyToAttack = false;
		SetAttackAnimation (false);
		SetSlashingAnimation (false);
		//SetDiggingAnimation (false);
	}

	public void AutoPickUpCalculation ()
	{
		if (Vector3.Distance (transform.position, nearestItemPosition) <= GameEventManager.walkTowardsItemSafeDistance) {   //stop walking towards objects if less than 1 distance					
			Vector3 dir = (nearestItemPosition - transform.position).normalized;
			walkTowards = false;
			ActionManager.m_instance.ActionButtonPressed ();
			return;
		}
		Vector3 playerDir = (nearestItemPosition - transform.position).normalized;
		WalkingCalculation (playerDir.x, playerDir.y);
	}

	public void WalkingCalculation (float x, float y)
	{
		print (ActionManager.m_instance.currentSelectedItem.id);
		actionButtonImage.GetComponent <Image> ().sprite =
			ItemDatabase.m_instance.FetchItemByID (LoadMapFromSave_PG.m_instance.GetTile (nearestItemPosition).id).Sprite;
		//ItemDatabase.m_instance.items [ActionManager.m_instance.currentSelectedItem.id].Sprite;	
		if (x < 0) {
			characterGO.transform.localScale = new Vector3 (1, 1, 1);
			playerEyes.transform.localPosition = new Vector3 (input_x * 0.07f, input_y * 0.07f);
		} else {
			characterGO.transform.localScale = new Vector3 (-1, 1, 1);
			playerEyes.transform.localPosition = new Vector3 (-input_x * 0.07f, input_y * 0.07f);
		}
		if (isPlayerRunning) {
			anim.SetBool ("isRunning", true);
			anim.SetFloat ("RunningX", x);
			anim.SetFloat ("RunningY", y);
		} else {
			anim.SetBool ("isWalking", true);
			anim.SetFloat ("WalkingX", x);
			anim.SetFloat ("WalkingY", y);
		}
		transform.position += new Vector3 (x, y, 0).normalized * Time.deltaTime * speed;
		//cursorTile.transform.position = new Vector3 ((int)transform.position.x, (int)transform.position.y);

	}

	public void AttackCalculation (int a, int b)
	{		
		if (a == attackTempA && b == attackTempB) {
			return;
		}
		attackTempA = a;
		attackTempB = b;

		anim.SetFloat ("SlashingX", a);
		anim.SetFloat ("SlashingY", b);

		ActionManager.m_instance.isReadyToAttack = false;
		//SetCursorTilePosition (Mathf.RoundToInt (r_input_a), Mathf.RoundToInt (r_input_b));
		ActionManager.m_instance.ActionButtonPressed ();
	}

	/*public void AttackCalculation ()
	{
		anim.SetTrigger ("Slashing");
	}*/

	public void SetIdleAnimation (bool flag)
	{
		anim.SetBool ("isIdle", flag);
	}

	public void SetSlashingAnimation (bool flag)
	{
		anim.SetBool ("isSlashing", flag);
	}

	public void SetDigUpAnimation ()
	{
		anim.SetTrigger ("DigUp");
	}

	public void SetAttackAnimation (bool flag)
	{
		anim.SetBool ("isAttacking", flag);
	}

	public void SetPickUpAnimation ()
	{
		anim.SetTrigger ("PickingUp");
	}

	public void SetCursorTilePosition (int a, int b)
	{
		itemPlacer.GetComponent <SpriteRenderer> ().sprite = Inventory.m_instance.playerSelectedTool.Sprite;

		cursorPosition = new Vector3 (Mathf.Round (transform.position.x), Mathf.Round (
			transform.position.y - animationPivotAdjuster), Mathf.Round (transform.position.z));
		cursorPosition.x += Mathf.Round (a);
		cursorPosition.y += Mathf.Round (b);
		itemPlacer.transform.position = cursorPosition;
	}

	public void IsCursorEnable (bool flag)
	{
		itemPlacer.GetComponent<SpriteRenderer> ().enabled = flag;
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
		switch (other.tag) {			
			case "Grass":
				other.GetComponent<Animator> ().enabled = true;
				other.GetComponent<Animator> ().SetTrigger ("shouldMove");
				break;
			case "DroppedItem":				
				//Destroy (other.gameObject);
				break;		
			default:
				break;
		}

		if (other.gameObject.transform.childCount > 0) {	
			//print (ItemDatabase.m_instance.items [GetItemID (other.gameObject.name)].tool.ToString () + "==" + ActionManager.m_AC_instance.currentWeildedItem.rarity.name);	
			//if (ItemDatabase.m_instance.items [GetItemID (other.gameObject.name)].tool.ToString () == ActionManager.m_AC_instance.currentWeildedItem.rarity.name) {
			//other.gameObject.transform.GetChild (other.gameObject.transform.childCount - 1).gameObject.SetActive (true);

			//}
		}
		//if (other.gameObject.GetComponent <SpriteRenderer> ()) {
		//other.gameObject.GetComponent <SpriteRenderer> ().color = Color.white;
		//}

	}

	void OnTriggerExit2D (Collider2D other)
	{
		switch (other.tag) {			
			case "Grass":
				StartCoroutine (DisableItemsAfterTime (other));
				break;		
			default:
				break;
		}

		// Disable item names
		/*if (other.gameObject.tag == "Item" &&
		    other.gameObject.transform.GetChild (other.gameObject.transform.childCount - 1).CompareTag ("ItemSelector")) {	
			other.gameObject.transform.GetChild (other.gameObject.transform.childCount - 1).transform.position = new Vector3 (1000, 1000);
		}*/
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
				//t.transform.GetChild (t.gameObject.transform.childCount - 1).GetComponent <TextMesh> ().color = visibleColor;
			}
		}
		//tMin.transform.GetChild (tMin.gameObject.transform.childCount - 1).GetComponent <TextMesh> ().color = nearstColor;
		itemSelector.transform.parent = tMin.transform;
		itemSelector.transform.localPosition = Vector3.zero;
		//tMin.GetComponent ();
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
		int a;
		if (int.TryParse (sArray [0], out a)) {
			return a;
		} else {
			return 0;
		}
	}
}