using UnityEngine;
using System.Collections;

public class CampfireControl : MonoBehaviour
{

	public ParticleSystem fireParticle;
	public GameObject spriteLight;

	float lightScale = 3, maxLightScale = 3;
	private float nextActionTime = 0.0f;
	float period = 1;

	public void AddFuel (int logs)
	{
		if (lightScale <= maxLightScale) {
			lightScale += logs;
		}
		if (lightScale >= maxLightScale) {
			lightScale = maxLightScale;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (GameEventManager.GetState () == GameEventManager.E_STATES.e_game) {
			if (Time.time > nextActionTime) { //upadate every n seconds
				nextActionTime += period;
				lightScale -= 0.01f;
				UpdateFireStrength ();
			}
		}
	}

	void UpdateFireStrength ()
	{
		spriteLight.transform.localScale = new Vector3 (lightScale, lightScale, lightScale);
	}
}
