using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DayNight_GameTime : MonoBehaviour
{
	public int maxTime = 480;
	public bool useTimeText;
	public Text timeText;
	public GameObject sun, moon, background;
	public float updatePeriod = 1.0f;
	public Camera spriteLightKitCamera;
	public SeasonTypes[] seasons;

	private float timeMultiplier = 60f;
	float timer = 0;
	int minutes = 0;
	int seconds = 0;
	int day = 0;
	float dayTime, nightTime = 0;
	float sunRotationTotalAngle = 90;
	float degreesTickSun = 0, degreesTickMoon = 0;
	float sunRotationZ = 0, moonRotationZ = 0;
	private float nextActionTime = 0.0f;

	//*****************************color lerper******************************************
	bool changeColor = false;
	public float colorTransitionDuration = 8;
	float colorTransitionTimer = 0;
	DayPhases currentPhase, tempCurrentPhase;
	//*****************************CloudsSpawner*****************************************
	public float timeBetweenSpawns = 10;
	public GameObject[] cloudsPrefab;
	int cloudInUse = 0;
	float timeSinceLastSpawn;
	//*****************************CloudsMoving*****************************************
	public Transform target;
	//***********************************************************************************

	void Start ()
	{		
		timer = Bronz.LocalStore.Instance.GetInt ("gameTime");
		day = Bronz.LocalStore.Instance.GetInt ("gameDay");
		currentPhase = (DayPhases)Bronz.LocalStore.Instance.GetInt ("currentPhase");
		sunRotationZ = Bronz.LocalStore.Instance.GetFloat ("sunRotationZ");
		moonRotationZ = Bronz.LocalStore.Instance.GetFloat ("moonRotationZ");
		background.transform.position = new Vector3 (Bronz.LocalStore.Instance.GetInt ("backgroundPositionX"), 0);
		tempCurrentPhase = currentPhase;

		switch (currentPhase) {					
			case DayPhases.Day:
				spriteLightKitCamera.backgroundColor = seasons [0].dayColor;
				break;
			case DayPhases.Dusk:
				spriteLightKitCamera.backgroundColor = seasons [0].duskColor;
				break;
			case DayPhases.Night:
				spriteLightKitCamera.backgroundColor = seasons [0].nightColor;
				break;
			default:
				break;
		}

		dayTime = (seasons [0].duskStart - seasons [0].dayStart) * timeMultiplier;
		degreesTickSun = sunRotationTotalAngle / dayTime;

		nightTime = ((maxTime / timeMultiplier) - seasons [0].duskStart) * timeMultiplier;
		degreesTickMoon = sunRotationTotalAngle / nightTime;
		colorTransitionTimer = colorTransitionDuration;

		InvokeRepeating ("CalculateDayNightCycle", 1, 1);
		InvokeRepeating ("MoveCloudEveryXTime", 1, 1);
		InvokeRepeating ("SpwanNewCloudsEveryXTime", 1, 10);
	}

	void Update ()
	{
		if (GameEventManager.GetState () == GameEventManager.E_STATES.e_game) {
			timer += Time.deltaTime;

			if (currentPhase != tempCurrentPhase) {
				//print ("saving phase");
			}
			if (changeColor) {
				colorTransitionTimer -= Time.deltaTime;
				/*switch (currentPhase) {					
					case DayPhases.Day:
						spriteLightKitCamera.backgroundColor = Color.Lerp (spriteLightKitCamera.backgroundColor, seasons [0].dayColor, Time.deltaTime / colorTransitionTimer);
						break;
					case DayPhases.Dusk:
						spriteLightKitCamera.backgroundColor = Color.Lerp (spriteLightKitCamera.backgroundColor, seasons [0].duskColor, Time.deltaTime / colorTransitionTimer);
						break;
					case DayPhases.Night:
						spriteLightKitCamera.backgroundColor = Color.Lerp (spriteLightKitCamera.backgroundColor, seasons [0].nightColor, Time.deltaTime / colorTransitionTimer);
						break;
					default:
						break;
				}*/
				if (colorTransitionTimer <= 0) {
					changeColor = false;
					colorTransitionTimer = colorTransitionDuration;
				}
			}
		}
	}

	void CalculateDayNightCycle ()
	{
		if (GameEventManager.GetState () == GameEventManager.E_STATES.e_game) {
			SaveManager.m_instance.SaveGameTime ((int)timer);
			if (timer <= maxTime) {
				background.transform.localPosition = new Vector3 (timer * -1, 0, 0);
				FormatDisplayTime ();
				CalculateDayPhases ();
			} else {
				timer = 0; //day over, new day 12am
				sunRotationZ = 0;
				moonRotationZ = 0;
				day++;
				SaveManager.m_instance.SaveGameDays (day);
				LoadMapFromSave_PG.m_instance.RepaintMapItems ();
				//print ("new day");
				sun.transform.rotation = Quaternion.Euler (0, 180, -45);
				background.transform.position = new Vector3 (0, 0, 0);
			}
		}
	}

	void MoveCloudEveryXTime ()
	{
		if (GameEventManager.GetState () == GameEventManager.E_STATES.e_game) {
			foreach (GameObject cloud in cloudsPrefab) {								
				cloud.transform.position = Vector3.MoveTowards (cloud.transform.position, target.position, 5);
				if (cloud.transform.localPosition.x >= 400) {
					Destory (cloud);
				}
			}
		}
	}

	void SpwanNewCloudsEveryXTime ()
	{
		if (GameEventManager.GetState () == GameEventManager.E_STATES.e_game) {
			int ranNo = Random.Range (0, cloudsPrefab.Length);
			if (ranNo != cloudInUse && !cloudsPrefab [ranNo].activeSelf) {
				cloudsPrefab [ranNo].gameObject.SetActive (true);
				cloudInUse = ranNo;
				cloudsPrefab [ranNo].transform.localPosition = new Vector3 (this.transform.localPosition.x, Random.Range (30, -35), 0);
			}
		}
	}


	void Destory (GameObject cloud)
	{
		cloud.transform.localPosition = new Vector3 (0, cloud.transform.localPosition.y);
		cloud.SetActive (false);
	}

	void  CalculateDayPhases ()
	{
		if (timer > seasons [0].dayStart * timeMultiplier && timer < seasons [0].nightStart * timeMultiplier) { // day		
			sunRotationZ += degreesTickSun;
			sun.transform.rotation = Quaternion.Euler (0, 180, -45 + sunRotationZ);
			if (timer > seasons [0].dayStart * timeMultiplier && timer < seasons [0].duskStart * timeMultiplier) {//day
				currentPhase = DayPhases.Day;			
				changeColor = true;
			} else if (timer > seasons [0].duskStart * timeMultiplier && timer < seasons [0].nightStart * timeMultiplier) {//dusk		
				currentPhase = DayPhases.Dusk;
				moon.transform.rotation = Quaternion.Euler (0, 180, -45 + moonRotationZ);
				changeColor = true;
				moonRotationZ += degreesTickMoon;
			}		
		} else if (timer > seasons [0].nightStart * timeMultiplier && timer < maxTime) {// night
			moonRotationZ += degreesTickMoon;
			moon.transform.rotation = Quaternion.Euler (0, 180, -45 + moonRotationZ);
			currentPhase = DayPhases.Night;		
			changeColor = true;
		}

		if (currentPhase != tempCurrentPhase) { // Avoide saving playerprefs every second
			StartCoroutine ("ChangeColorOfGame");
			tempCurrentPhase = currentPhase;
			SaveManager.m_instance.SaveGameCurrentPhase ((int)currentPhase);
		}

		Bronz.LocalStore.Instance.SetFloat ("sunRotationZ", sunRotationZ);
		Bronz.LocalStore.Instance.SetFloat ("moonRotationZ", moonRotationZ);

		Bronz.LocalStore.Instance.SetInt ("backgroundPositionX", (int)background.transform.position.x);	
	}

	public IEnumerator ChangeColorOfGame ()
	{
		Debug.Log ("Starting fading!");
		float ElapsedTime = 0.0f;
		float TotalTime = 6.0f;
		while (ElapsedTime < TotalTime) {
			ElapsedTime += Time.deltaTime;
			switch (currentPhase) {					
				case DayPhases.Day:
					spriteLightKitCamera.backgroundColor = Color.Lerp (spriteLightKitCamera.backgroundColor, seasons [0].dayColor, Time.deltaTime / colorTransitionTimer);
					break;
				case DayPhases.Dusk:
					spriteLightKitCamera.backgroundColor = Color.Lerp (spriteLightKitCamera.backgroundColor, seasons [0].duskColor, Time.deltaTime / colorTransitionTimer);
					break;
				case DayPhases.Night:
					spriteLightKitCamera.backgroundColor = Color.Lerp (spriteLightKitCamera.backgroundColor, seasons [0].nightColor, Time.deltaTime / colorTransitionTimer);
					break;
				default:
					break;
			}
			yield return null;
		}
		Debug.Log ("Ending fading!");
	}

	void FormatDisplayTime ()
	{
		minutes = Mathf.FloorToInt (timer / 60F);
		seconds = Mathf.FloorToInt (timer - minutes * 60);
		timeText.text = string.Format ("{0:0}:{1:00}", minutes, seconds + " Day:" + day);// + " " + currentPhase);
	}

	#region DEBUG_FUNCTIONS

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

	#endregion

}

[System.Serializable]
public enum DayPhases
{
	Day,
	Dusk,
	Night
}

[System.Serializable]
public struct SeasonTypes
{
	public string name;
	public float dayStart;
	public Color dayColor;
	public float duskStart;
	public Color duskColor;
	public float nightStart;
	public Color nightColor;
}