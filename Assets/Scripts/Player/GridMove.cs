using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GridMove : MonoBehaviour
{
	public float speed = 2.0f;
	public float movementSize = 1.0f;
	public GameObject player;
	Animator anim;
	Vector3 pos;
	Transform tr;

	void Start ()
	{
		anim = player.GetComponent<Animator> ();
		anim.Play ("none");
		pos = transform.position;
		tr = transform;
	}

	void Update ()
	{
		if (Input.GetKey (KeyCode.W) && tr.position == pos) {
			pos += (Vector3.up) / movementSize;
			anim.Play ("MoveUp");
		} else if (Input.GetKey (KeyCode.D) && tr.position == pos) {
			pos += (Vector3.right) / movementSize;
			anim.Play ("MoveRight");
		} else if (Input.GetKey (KeyCode.S) && tr.position == pos) {
			pos += (Vector3.down) / movementSize;
			anim.Play ("MoveDown");
		} else if (Input.GetKey (KeyCode.A) && tr.position == pos) {
			pos += (Vector3.left) / movementSize;
			anim.Play ("MoveLeft");
		}
		//anim.Play ("none");
		transform.position = Vector3.MoveTowards (transform.position, pos, Time.deltaTime * speed);
	}

	void OnCollisionEnter2D (Collision2D coll)
	{
		print (coll.gameObject.name);
	}
}
