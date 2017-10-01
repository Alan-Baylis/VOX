/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;

namespace VOX{
	public class FollowOtherCameraFov : MonoBehaviour{
		
		public Camera cam;
		public float fovMultiplier = 1f;

		Camera thisCam;

		void Start(){
			thisCam = GetComponent<Camera>();
		}

		void Update(){
			thisCam.fieldOfView = cam.fieldOfView * fovMultiplier;
		}
	}
}