using System;
using UnityEngine;
using UnityEngine.EventSystems;
using CnControls;

public class PlayerMovement : MonoBehaviour
{
	Animator anim;
	public GameObject mainCanvas;
	public float speed = 2.0f;
	public static PlayerMovement m_instance = null;
	// Use this for initialization

	void Awake ()
	{
		m_instance = this;
		mainCanvas.SetActive (true);
	}

	void Start ()
	{
		anim = GetComponent<Animator> ();
		GridCalculate.m_instance.DisableCursorTile ();
//		BoardManager.m_instance.SetupScene ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		float input_x = CnInputManager.GetAxisRaw ("Horizontal");
		float input_y = CnInputManager.GetAxisRaw ("Vertical");
		float r_input_x = CnInputManager.GetAxisRaw ("Horizontal_Right");
		float r_input_y = CnInputManager.GetAxisRaw ("Vertical_Right");

		bool isWalking = (Mathf.Abs (input_x) + Mathf.Abs (input_y)) > 0;
		bool isRightStick = (Mathf.Abs (r_input_x) + Mathf.Abs (r_input_y)) > 0;

		if (isRightStick) {
			GridCalculate.m_instance.CalculatePlayerGrid (r_input_x, r_input_y);
		}
		anim.SetBool ("isWalking", isWalking);		
		
		if (isWalking) {
			GridCalculate.m_instance.DisableCursorTile ();
			//GridCalculate.m_instance.CalculatePlayerGrid (r_input_x, r_input_y);					
			anim.SetFloat ("x", input_x);
			anim.SetFloat ("y", input_y);
			transform.position += new Vector3 (input_x, input_y, 0).normalized * Time.deltaTime * speed;
		}
	}
}
