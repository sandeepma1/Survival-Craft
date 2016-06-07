﻿using System;
using UnityEngine;
using UnityEngine.EventSystems;
using CnControls;

public class PlayerMovement : MonoBehaviour
{
	Animator anim;
	public float speed = 2.0f;
	public static PlayerMovement m_instance = null;
	// Use this for initialization

	void Awake ()
	{
		m_instance = this;
	}

	void Start ()
	{
		anim = GetComponent<Animator> ();
		BoardManager.m_instance.SetupScene ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		float input_x = CnInputManager.GetAxisRaw ("Horizontal");
		float input_y = CnInputManager.GetAxisRaw ("Vertical");
		float r_input_x = CnInputManager.GetAxisRaw ("Horizontal_Right");
		float r_input_y = CnInputManager.GetAxisRaw ("Vertical_Right");
		bool isWalking = (Mathf.Abs (input_x) + Mathf.Abs (input_y)) > 0;
		anim.SetBool ("isWalking", isWalking);

		GridCalculate.m_instance.CalculatePlayerGrid (r_input_x, r_input_y);		

		if (isWalking) {			
			anim.SetFloat ("x", input_x);
			anim.SetFloat ("y", input_y);
			transform.position += new Vector3 (input_x, input_y, 0).normalized * Time.deltaTime * speed;
		}
	}
}
