using UnityEngine;
using System.Collections;

public class TreesDisappear : MonoBehaviour
{
	void OnTriggerEnter2D (Collider2D other)
	{
		switch (other.tag) {
			case "Disappear":
				other.GetComponent<SpriteRenderer> ().color = new Color (1f, 1f, 1f, 0.45f);
				other.transform.parent.transform.GetChild (1).GetComponent<SpriteRenderer> ().color = new Color (1f, 1f, 1f, 0.45f);
				break;
			case "Fire":
				PlayerMovement.m_instance.isPlayerNearFire = true;
				break;		
			default:
				break;
		}
	}

	void OnTriggerExit2D (Collider2D other)
	{
		switch (other.tag) {
			case "Disappear":
				other.GetComponent<SpriteRenderer> ().color = new Color (1f, 1f, 1f, 1f);
				other.transform.parent.transform.GetChild (1).GetComponent<SpriteRenderer> ().color = new Color (1f, 1f, 1f, 1f);
				break;		
			case "Fire":
				PlayerMovement.m_instance.isPlayerNearFire = false;
				break;			
			default:
				break;
		}
	}
}
