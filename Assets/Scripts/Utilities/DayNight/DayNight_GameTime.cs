using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DayNight_GameTime : MonoBehaviour
{
	public bool useTimeText;
	public Text timeText;
	public GameObject sun;
	public float updatePeriod = 1.0f;
	public float timeMultiplier = 60f;

	public SeasonTypes[] seasons;
	float timer = 0;
	int minutes = 0;
	int seconds = 0;

	float moveDistance = 10f;
	Vector3 startPos = new Vector3 (10, 0, 0);
	Vector3 endPos = new Vector3 (20, 0, 0);

	private float nextActionTime = 0.0f;


	void Awake ()
	{
		timer = PlayerPrefs.GetFloat ("gameTime");
	}

	protected void Start ()
	{
		startPos = transform.position;
		endPos = transform.position + transform.up * moveDistance;
		InvokeRepeating ("SaveGameTime", 1, 2);
	}

	void Update ()
	{
		if (GameEventManager.GetState () == GameEventManager.E_STATES.e_game) {
			timer += Time.deltaTime;

			if (Time.time > nextActionTime) { //upadate every n seconds
				nextActionTime += updatePeriod;
				if (timer < 1440) {					
					FormatTime ();
					CalculateDayPhases ();
				} else {
					timer = 0; //day over new day 12am
				}
			}
			return;
		}
	}

	void FormatTime ()
	{
		//timeText.text = timer.ToString ("mm-ddd");
		minutes = Mathf.FloorToInt (timer / 60F);
		seconds = Mathf.FloorToInt (timer - minutes * 60);
		//timeText.text = string.Format ((timer) + " {0:0}:{1:00}", minutes, seconds);
	}

	void CalculateDayPhases ()
	{
		if (timer > seasons [0].dawn_start * timeMultiplier && timer < seasons [0].night_start * timeMultiplier) {			// day
			//sun.transform.rotation = Quaternion.Euler (0, 180, (((timer / 4) + 180) * 1.25f) * 0.12f);
			//sun.transform.rotation = Quaternion.Euler (0, 180, ((timer * 0.125f) - 90)); // good one summer 
			sun.transform.rotation = Quaternion.Euler (0, 180, ((timer * 0.125f) - 90));
			if (timer > seasons [0].dawn_start * timeMultiplier && timer < seasons [0].day_start * timeMultiplier) {
				print ("dawn");
			} else if (timer > seasons [0].day_start * timeMultiplier && timer < seasons [0].dusk_start * timeMultiplier) {			
				print ("day");
			} else if (timer > seasons [0].dusk_start * timeMultiplier && timer < seasons [0].night_start * timeMultiplier) {			
				print ("dusk");
			}		
		} else if (timer > seasons [0].night_start * timeMultiplier && timer < 1440) {	
			//sun.transform.rotation = Quaternion.Euler (0, 180, (timer / 4));		
			print ("night");
		} else if (timer > 0 && timer < seasons [0].dawn_start * timeMultiplier) {	
			//sun.transform.rotation = Quaternion.Euler (0, 180, (timer / 4));		
			print ("night");
		}
	}

	void SaveGameTime ()
	{
		SaveManager.m_instance.SaveGameTime (timer);
	}

	public void ModifyGameSpeed (float speed)
	{
		Time.timeScale = speed;
	}

	public void ResetTime ()
	{
		timer = 240;
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
 