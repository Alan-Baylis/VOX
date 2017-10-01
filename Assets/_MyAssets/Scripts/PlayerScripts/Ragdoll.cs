/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;

namespace VOX{
	public class Ragdoll : MonoBehaviour{
		
		[Range(0, 31)] public int ragdollLayer;

		[System.NonSerialized] public bool isActive;

		public void ActivateRagdoll(){
			isActive = true;

			Collider[] colliders = GetComponentsInChildren<Collider>();
			for(int i = 0; i < colliders.Length; i++){
				Collider col = colliders[i];

				col.isTrigger = false;
				col.gameObject.layer = ragdollLayer;
			}
			
			Rigidbody[] rigidBodies = GetComponentsInChildren<Rigidbody>();
			for(int i = 0; i < rigidBodies.Length; i++){
				rigidBodies[i].isKinematic = false;
			}
		}
	}
}