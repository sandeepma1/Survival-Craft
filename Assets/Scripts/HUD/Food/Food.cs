﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Food : MonoBehaviour
{
	public static Food m_instance = null;
	public int startHunger;
	public int hungerPerBurger;
	
	private int maxHunger;
	private int currentHunger;
	
	public Sprite[] burgerImages;
	public Image burgerGUI;
	
	private ArrayList burgers = new ArrayList ();
	
	// Spacing:
	public float maxBurgersOnRow;
	public float spacingX;
	public float spacingY;

	void Awake ()
	{
		m_instance = this;
	}

	void Start ()
	{
		spacingX = burgerGUI.GetComponent <RectTransform> ().rect.width;
		spacingY = -burgerGUI.GetComponent <RectTransform> ().rect.height;
	
		AddBurgers (startHunger / hungerPerBurger);
	}

	public void AddBurgers (int n)
	{		
		for (int i = 0; i < n; i++) { 
			RectTransform newBurger = ((GameObject)Instantiate (burgerGUI.gameObject, this.transform.position, Quaternion.identity)).GetComponent <RectTransform> ();
			newBurger.parent = transform;			

			int y = (int)(Mathf.FloorToInt (burgers.Count / maxBurgersOnRow));
			int x = (int)(burgers.Count - y * maxBurgersOnRow);

			newBurger.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (x * spacingX, y * spacingY);
			newBurger.GetComponent<RectTransform> ().localScale = Vector3.one; //TODO: (QuickFix)converting to vector.one; newHeart is scaling to 1.5,1.5,1
			newBurger.GetComponent<Image> ().overrideSprite = burgerImages [0];
			burgers.Add (newBurger);
		}
		maxHunger += n * hungerPerBurger;
		currentHunger = maxHunger;
		UpdateBurgers ();
	}

	public void modifyHunger (int amount)
	{
		currentHunger += amount;
		currentHunger = Mathf.Clamp (currentHunger, 0, maxHunger);
		UpdateBurgers ();
	}

	void UpdateBurgers ()
	{
		bool restAreEmpty = false;
		int i = 0;
		
		foreach (Transform burger in burgers) {			
			if (restAreEmpty) {
				burger.GetComponent<Image> ().overrideSprite = burgerImages [0]; // heart is empty
			} else {
				i += 1; // current iteration
				if (currentHunger >= i * hungerPerBurger) {
					burger.GetComponent<Image> ().overrideSprite = burgerImages [burgerImages.Length - 1]; // health of current heart is full
				} else {
					int currentBurgerHunger = (int)(hungerPerBurger - (hungerPerBurger * i - currentHunger));
					int hungerPerImage = hungerPerBurger / burgerImages.Length; // how much health is there per image
					int imageIndex = currentBurgerHunger / hungerPerImage;					
					if (imageIndex == 0 && currentBurgerHunger > 0) {
						imageIndex = 1;
					}
					burger.GetComponent<Image> ().overrideSprite = burgerImages [imageIndex];
					restAreEmpty = true;
				}
			}			
		}
	}

}
