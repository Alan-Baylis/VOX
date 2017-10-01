using UnityEngine;
using System.Collections;

public class LightFlicker : MonoBehaviour{
	
	public float min = 0.2f;
	public float max = 1f;
	public float lerpSpeed = 5f;
	public bool flickering = true;

	Light _light;
	float targetIntensity;

	void Awake(){
		_light = GetComponent<Light>();
	}

	void Update(){
		_light.intensity = Mathf.Lerp(_light.intensity, targetIntensity, lerpSpeed * Time.deltaTime);
	}

	void FixedUpdate(){
		if(flickering){
			targetIntensity = Random.Range(min, max);
		}
	}
}