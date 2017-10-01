/*
* Copyright (c) Retardevs
* http://gamejolt.com/@retardevs
*/

using UnityEngine;

namespace Cirda.Utils{
	public class RotateMe : MonoBehaviour{
		
		public Vector3 rotationSpeed;

		void Update(){
			transform.Rotate(rotationSpeed * Time.deltaTime);
		}

	}
}