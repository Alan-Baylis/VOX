/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;
using System.Collections;

namespace VOX{
	public class WorldModelGun : MonoBehaviour{
		
		public GunInfo info;
		public Animator anim;
		public Transform drop;
		public GameObject muzzle;

		[Header("Character Joints")]
		public CharacterJoint rightArmCharacterJoint;
		public CharacterJoint leftArmCharacterJoint;

		void OnEnable(){
			if(muzzle != null)
				muzzle.SetActive(false);
		}

		public void Fire(){
			StartCoroutine(IMuzzle());
		}

		IEnumerator IMuzzle(){
			if(muzzle != null)
				muzzle.SetActive(true);
			
			float t = Time.time;
			while(Time.time < t + Gun.muzzleDuration){
				yield return 0;
			}
			
			if(muzzle != null)
				muzzle.SetActive(false);
		}

	}
}