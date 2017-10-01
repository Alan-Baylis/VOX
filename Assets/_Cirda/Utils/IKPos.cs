/*
* Copyright (c) Retardevs
* http://gamejolt.com/@retardevs
*/

using UnityEngine;

namespace Cirda.Utils{
	[ExecuteInEditMode]
	public class IKPos : MonoBehaviour{
		
		public bool changeRotation;

		[Header("Transforms")]
		public Transform transformToMove;
		public Transform target;

		void LateUpdate(){
			if(transformToMove && target){
				target.position = transformToMove.position;

				if(changeRotation)
					target.rotation = transformToMove.rotation;
			}
		}
	}
}