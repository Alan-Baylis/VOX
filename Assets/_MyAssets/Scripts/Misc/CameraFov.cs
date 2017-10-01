/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;

namespace VOX{
	public class CameraFov : MonoBehaviour{
		
		public Camera cam;
		[Range(1f, 179f)] public float fov = 80f;
		public float lerpSpeed = 10f;

		void Update(){
			cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov, lerpSpeed * Time.deltaTime);
		}

	}
}