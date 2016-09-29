using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DayNight_GameTime : MonoBehaviour
{
	public int maxTime = 480;
	public bool useTimeText;
	public Text timeText;
	public GameObject sun, background;
	public float updatePeriod = 1.0f;
	public SeasonTypes[] seasons;

	private float timeMultiplier = 60f;
	float timer = 0;
	int minutes = 0;
	int seconds = 0;
	int day = 0;
	float dayTime = 0;
	float sunRotationTotalAngle = 120;
	float degreesTick = 0;
	float sunRotationZ = 0;

	//float moveDistance = 10f;
	//Vector3 startPos = new Vector3 (10, 0, 0);
	//Vector3 endPos = new Vector3 (20, 0, 0);

	private float nextActionTime = 0.0f;

	protected void Start ()
	{
		timer = PlayerPrefs.GetFloat ("gameTime");
		day = PlayerPrefs.GetInt ("gameDay");
		//startPos = transform.position;
		//endPos = transform.position + transform.up * moveDistance;
		InvokeRepeating ("SaveGameTime", 1, 2);

		dayTime = (seasons [0].night_start - seasons [0].dawn_start) * timeMultiplier;
		degreesTick = sunRotationTotalAngle / dayTime;
	}

	void Update ()
	{
		if (GameEventManager.GetState () == GameEventManager.E_STATES.e_game) {
			timer += Time.deltaTime;
			if (Time.time > nextActionTime) { //upadate every n seconds
				nextActionTime += updatePeriod;
				if (timer <= maxTime) {
					background.transform.localPosition = new Vector3 (timer * -1, 0, 0);
					FormatDisplayTime ();
					//CalculateDayPhases ();
					CalculateDayPhases1 ();
				} else {
					timer = 0; //day over new day 12am
					day++;
					SaveGameDay ();
					//day = PlayerPrefs.GetInt ("gameDay");
					LoadMapFromSave_PG.m_instance.RepaintMapItems ();
					print ("new day");

					sun.transform.rotation = Quaternion.Euler (0, 180, -60);
					background.transform.position = new Vector3 (400, 0, 0);
				}
			}
			return;
		}
	}

	void  CalculateDayPhases1 ()
	{
		if (timer > seasons [0].dawn_start * timeMultiplier && timer < seasons [0].night_start * timeMultiplier) { // day		
			sunRotationZ += degreesTick;		
			sun.transform.rotation = Quaternion.Euler (0, 180, -60 + sunRotationZ);
			if (timer > seasons [0].dawn_start * timeMultiplier && timer < seasons [0].day_start * timeMultiplier) {
				//print ("dawn");
			} else if (timer > seasons [0].day_start * timeMultiplier && timer < seasons [0].dusk_start * timeMultiplier) {			
				//print ("day");
			} else if (timer > seasons [0].dusk_start * timeMultiplier && timer < seasons [0].night_start * timeMultiplier) {			
				//print ("dusk");
			}		
		} else if (timer > seasons [0].night_start * timeMultiplier && timer < maxTime) {	// night
			//sun.transform.rotation = Quaternion.Euler (0, 180, (timer / 4));		
			//print ("night");
		} /*else if (timer > 0 && timer < seasons [0].dawn_start * timeMultiplier) {	// one day end
			//sun.transform.rotation = Quaternion.Euler (0, 180, (timer / 4));		
			//print ("night");
		}*/
	}


	void FormatDisplayTime ()
	{
		//day = PlayerPrefs.GetInt ("gameDay");
		//timeText.text = timer.ToString ("mm-ddd");
		minutes = Mathf.FloorToInt (timer / 60F);
		seconds = Mathf.FloorToInt (timer - minutes * 60);
		timeText.text = string.Format ("{0:0}:{1:00}", minutes, seconds + " Day:" + day);
	}

	void CalculateDayPhases ()
	{
		if (timer > seasons [0].dawn_start * timeMultiplier && timer < seasons [0].night_start * timeMultiplier) {			// day
			//sun.transform.rotation = Quaternion.Euler (0, 180, (((timer / 4) + 180) * 1.25f) * 0.12f);
			//sun.transform.rotation = Quaternion.Euler (0, 180, ((timer * 0.125f) - 90)); // good one summer 
			sun.transform.rotation = Quaternion.Euler (0, 180, ((timer * 0.125f) - 90));
			if (timer > seasons [0].dawn_start * timeMultiplier && timer < seasons [0].day_start * timeMultiplier) {
				//print ("dawn");
			} else if (timer > seasons [0].day_start * timeMultiplier && timer < seasons [0].dusk_start * timeMultiplier) {			
				//print ("day");
			} else if (timer > seasons [0].dusk_start * timeMultiplier && timer < seasons [0].night_start * timeMultiplier) {			
				//print ("dusk");
			}		
		} else if (timer > seasons [0].night_start * timeMultiplier && timer < maxTime) {	
			//sun.transform.rotation = Quaternion.Euler (0, 180, (timer / 4));		
			//print ("night");
		} else if (timer > 0 && timer < seasons [0].dawn_start * timeMultiplier) {	
			//sun.transform.rotation = Quaternion.Euler (0, 180, (timer / 4));		
			//print ("night");
		}
	}

	void SaveGameTime ()
	{
		SaveManager.m_instance.SaveGameTime (timer);
	}

	void SaveGameDay ()
	{
		SaveManager.m_instance.SaveGameTime (day);
	}

	public void ModifyGameSpeed (float speed)
	{
		Time.timeScale = speed;
	}

	public void NextDay ()
	{
		timer = maxTime - 3;
	}

	public void ResetTime ()
	{
		timer = 0;
	}
}

[System.Serializable]
public struct SeasonTypes
{
	public string name;
	public float dawn_start;
	public float day_start;
	public float dusk_start;
	public float night_start;
}
 
/*if (timer > seasons [0].dawn_start * timeMultiplier && timer < seasons [0].day_start * timeMultiplier) {			
			//sun.transform.rotation = Quaternion.Euler (0, 0, (-timer));
			print ("dawn");
		} else if (timer > seasons [0].day_start * timeMultiplier && timer < seasons [0].dusk_start * timeMultiplier) {			
			//sun.transform.rotation = Quaternion.Euler (0, 0, (-timer));
			print ("day");
		} else if (timer > seasons [0].dusk_start * timeMultiplier && timer < seasons [0].night_start * timeMultiplier) {			
			//sun.transform.rotation = Quaternion.Euler (0, 0, (-timer));
			print ("dusk");
		} else if (timer > seasons [0].night_start * timeMultiplier && timer < 1440) {			
			//sun.transform.rotation = Quaternion.Euler (0, 0, (-timer));
			print ("night");
		} else if (timer > 0 && timer < seasons [0].dawn_start * timeMultiplier) {			
			//sun.transform.rotation = Quaternion.Euler (0, 0, (-timer));
			print ("night");
		}*/
 