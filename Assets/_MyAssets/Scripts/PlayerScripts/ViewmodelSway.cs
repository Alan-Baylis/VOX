/*
* Copyright (c) Retardevs
* http://gamejolt.com/@retardevs
*/

using UnityEngine;

namespace VOX{
	public class ViewmodelSway: MonoBehaviour{

		public SwayTemplate template;

		float targetX;
		float targetY;

		void Update(){
			if(!GameController.IsPaused()){
				targetX = Input.GetAxis("Mouse X");
				targetY = Input.GetAxis("Mouse Y");

				int y = Mathf.RoundToInt(ServerController.player.mouseLook.mouseLookRoot.localEulerAngles.x);
				if(y == 270 || y == 90)
					targetY = 0f;
			}else{
				targetX = 0f;
				targetY = 0f;
			}

			Vector3 targetVector = new Vector3(targetY, -targetX) * template.strengh * Settings.Sensitivity;
			targetVector = Vector3.ClampMagnitude(targetVector, template.maxDistance);

			transform.localRotation = Quaternion.Slerp(
				transform.localRotation, Quaternion.Euler(targetVector),
				template.lerpSpeed > 0f ? template.lerpSpeed * Time.deltaTime : 1f
			);
		}
	}

	[System.Serializable]
	public class SwayTemplate{
		public float lerpSpeed = 5f;
		public float strengh = 3f;
		public float maxDistance = 10f;
	}
}