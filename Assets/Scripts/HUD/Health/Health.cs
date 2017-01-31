using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
	public static Health m_instance = null;
	public int startHealth;
	public int healthPerHeart;
	public float currentHealth;
	public Sprite[] heartImages;
	public Image heartGUI;
	// Spacing:
	public float maxHeartsOnRow;
	public float spacingX;
	public float spacingY;
	public GameObject youDiedMenu;
	private int maxHealth;
	private ArrayList hearts = new ArrayList ();

	void Awake ()
	{
		m_instance = this;
	}

	void Start ()
	{
		spacingX = heartGUI.GetComponent <RectTransform> ().rect.width;
		spacingY = -heartGUI.GetComponent <RectTransform> ().rect.height;
	
		AddHearts (startHealth / healthPerHeart);
		currentHealth = Bronz.LocalStore.Instance.GetFloat ("PlayerHealth");
		UpdateHearts ();
	}

	public void AddHearts (int n)
	{		
		for (int i = 0; i < n; i++) { 
			//Transform newHeart = ((GameObject)Instantiate (heartGUI.gameObject, this.transform.position, Quaternion.identity)).transform;
			//GameObject go = ((GameObject)Instantiate (heartGUI.gameObject, heartGUI.GetComponent<RectTransform> ().anchoredPosition3D, Quaternion.identity)); // Creates a new heart
			//RectTransform newHeart = go.GetComponent <RectTransform> ();
			RectTransform newHeart = ((GameObject)Instantiate (heartGUI.gameObject, this.transform.position, Quaternion.identity)).GetComponent <RectTransform> ();
			newHeart.parent = transform;			

			int y = (int)(Mathf.FloorToInt (hearts.Count / maxHeartsOnRow));
			int x = (int)(hearts.Count - y * maxHeartsOnRow);

			newHeart.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (x * spacingX, y * spacingY);
			newHeart.GetComponent<RectTransform> ().localScale = Vector3.one; //TODO: (QuickFix)converting to vector.one; newHeart is scaling to 1.5,1.5,1
			newHeart.GetComponent<Image> ().overrideSprite = heartImages [0];
			hearts.Add (newHeart);
		}
		maxHealth += n * healthPerHeart;
		currentHealth = maxHealth;
		UpdateHearts ();
	}

	public void modifyHealth (int amount)
	{
		currentHealth += amount;
		currentHealth = Mathf.Clamp (currentHealth, 0, maxHealth);
		UpdateHearts ();
		Bronz.LocalStore.Instance.SetFloat ("PlayerHealth", currentHealth);
		if (currentHealth <= 0) {
			print ("Player Died");
			youDiedMenu.SetActive (true);
		}
	}


	void UpdateHearts ()
	{
		bool restAreEmpty = false;
		int i = 0;
		
		foreach (Transform heart in hearts) {			
			if (restAreEmpty) {
				heart.GetComponent<Image> ().overrideSprite = heartImages [0]; // heart is empty
			} else {
				i += 1; // current iteration
				if (currentHealth >= i * healthPerHeart) {
					heart.GetComponent<Image> ().overrideSprite = heartImages [heartImages.Length - 1]; // health of current heart is full
				} else {
					int currentHeartHealth = (int)(healthPerHeart - (healthPerHeart * i - currentHealth));
					int healthPerImage = healthPerHeart / heartImages.Length; // how much health is there per image
					int imageIndex = currentHeartHealth / healthPerImage;					
					if (imageIndex == 0 && currentHeartHealth > 0) {
						imageIndex = 1;
					}
					heart.GetComponent<Image> ().overrideSprite = heartImages [imageIndex];
					restAreEmpty = true;
				}
			}			
		}
	}
}
