/*
* Copyright (c) Retardevs
* http://gamejolt.com/@retardevs
*/

using UnityEngine;

namespace VOX{
	public class Headbob : MonoBehaviour{

		public bool animate = false;
		public HeadbobTemplate template;
		
		float t = Mathf.PI / 2f;
		Vector3 defaultPos;

		void Awake(){
			defaultPos = transform.localPosition;
		}

		void Update(){
			if(animate){
				Move();
			}else{
				Return();
			}
		}

		void Move(){
			t += template.speed * Time.deltaTime;

			if(t > Mathf.PI * 2f)
				t = 0;

			Vector3 newPosition = new Vector3(Mathf.Cos(t) * template.strenghX, (-Mathf.Abs(Mathf.Sin(t)) * -template.strenghY) - (template.strenghY / 2f), 0f);
			transform.localPosition = Vector3.Lerp(transform.localPosition, newPosition, template.lerpSpeed > 0f ? template.lerpSpeed * Time.deltaTime : 1f);
		}

		void Return(){
			if(template.resetOnStop){
				t = Mathf.PI / 2f;
			}

			transform.localPosition = Vector3.Lerp(transform.localPosition, defaultPos, template.returnLerpSpeed > 0f ? template.returnLerpSpeed * Time.deltaTime : 1f);
		}
	}

	[System.Serializable]
	public class HeadbobTemplate{
		public float lerpSpeed = 10f;
		public float returnLerpSpeed = 5f;
		public float speed = 6.5f;
		public float strenghX = 0.025f;
		public float strenghY = 0.025f;
		public bool resetOnStop = false;
	}
}