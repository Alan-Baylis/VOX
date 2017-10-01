/*
* Copyright (c) Retardevs
* http://gamejolt.com/@retardevs
*/

using UnityEngine;

namespace Cirda.Utils{
	[ExecuteInEditMode]
	public class IKChain : MonoBehaviour{
	
		public bool active = false;

		[Header("Bones")]
		public Transform upperArm;
		public Transform foreArm;
		public Transform hand;
	
		[Header("IKs")]
		public Transform handIK;
		public Transform elbowIK;

		[Header("Hand Rotation")]
		public HandRotationStyle handRotationStyle;
		public Vector3 handRotation;

		public enum HandRotationStyle{ LocalRotation, GlobalRotation, FollowIKRotation }

		void LateUpdate(){
			if(active && upperArm && foreArm && hand && handIK && elbowIK){
				float rootToMid = Vector3.Distance(upperArm.position, foreArm.position);
				float midToEnd = Vector3.Distance(foreArm.position, hand.position);
				float rootToEnd = Vector3.Distance(handIK.position, upperArm.position);

				Quaternion rootToMidQuat = Quaternion.identity;
				Quaternion rootToEndQuat = Quaternion.identity;
		
				rootToEndQuat.SetLookRotation(handIK.position - upperArm.position, elbowIK.position - upperArm.position);

				if(Mathf.Abs(rootToMid - midToEnd) > rootToEnd){
					Vector3 rot = foreArm.localEulerAngles;
					rot.z = 180f;

					foreArm.localEulerAngles = rot;

					rootToMidQuat.eulerAngles = new Vector3(0f, 90f, 0f);
				}else if(Mathf.Abs(rootToMid + midToEnd) < rootToEnd){
					Vector3 rot = foreArm.localEulerAngles;
					rot.z = 0f;

					foreArm.localEulerAngles = rot;

					rootToMidQuat.eulerAngles = new Vector3(0f, 90f, 0f);
				}else{
					Vector3 rot = foreArm.localEulerAngles;
					rot.z = Mathf.Asin((rootToMid * rootToMid + midToEnd * midToEnd - rootToEnd * rootToEnd) / (2f * rootToMid * midToEnd)) * Mathf.Rad2Deg + 90f;

					foreArm.localEulerAngles = rot;

					rootToMidQuat.eulerAngles = new Vector3(0f, 90f, Mathf.Asin((rootToEnd * rootToEnd + rootToMid * rootToMid - midToEnd * midToEnd) / (2f * rootToEnd * rootToMid)) * Mathf.Rad2Deg - 90f);
				}

				upperArm.rotation = rootToEndQuat * rootToMidQuat;
				
				if(handRotationStyle == HandRotationStyle.LocalRotation)
					hand.localEulerAngles = handRotation;
				else if(handRotationStyle == HandRotationStyle.GlobalRotation)
					hand.eulerAngles = handRotation;
				else
					hand.eulerAngles = handIK.eulerAngles + handRotation;
			}
		}
	}
}