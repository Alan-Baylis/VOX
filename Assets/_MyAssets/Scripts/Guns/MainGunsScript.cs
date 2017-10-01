/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;

namespace VOX{
	public class MainGunsScript : MonoBehaviour{
		
		public Transform leftHandedTransform;
		public Transform mover;
		public CameraFov camFov;
		public AudioSource source;

		void Update(){
			leftHandedTransform.localScale = new Vector3(Settings.LeftHanded ? -1f : 1f, 1f, 1f);
		}

	}
}