/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;
using Photon;

namespace VOX{
	public class Grenade : GunBase{
		
		[Header("Values")]
		[Range(0f, 90f)] public float upAngle = 10f;
		public float throwDelay = 1f;
		public int grenades = 3;
		public string prefabName;

		[Header("Animation")]
		public Animation anim;
		public AnimationClip drawAnimation;
		public AnimationClip startThrowAnimation;
		public AnimationClip finishThrowAnimation;

		float t;
		
		public Player player{
			get{ return ServerController.player; }
		}

		public bool canAttack{
			get{ return Time.time > t && grenades > 0; }
		}

		public bool throwing{ get; private set; }

		void OnEnable(){
			t = Time.time + drawAnimation.length;
			throwing = false;
		}

		void Update(){
			transform.localPosition = Settings.GlobalViewmodelOffset;

			if(!GameController.IsPaused()){
				if(Input.GetKey(KeyCode.Mouse0) && canAttack){
					StartThrowing();
				}
				if(Input.GetKeyUp(KeyCode.Mouse0) && canAttack){
					FinishThrowing();
				}
			}else{
				t = Time.time + 0.1f;
			}
		}


		void StartThrowing(){
			if(throwing)
				return;

			throwing = true;

			anim.Stop();
			anim.PlayQueued(startThrowAnimation.name, QueueMode.PlayNow);
		}

		void FinishThrowing(){
			throwing = false;

			t = Time.time + throwDelay;

			anim.Stop();
			anim.PlayQueued(finishThrowAnimation.name, QueueMode.PlayNow);
			
			anim.PlayQueued(drawAnimation.name);
		}

		public void InstantiateGrenade(){
			Transform cam = player.mouseLook.mouseLookRoot;
			grenades--;

			object[] data = new object[1];
			data[0] = cam.forward + (Vector3.up * (upAngle / 90f));

			GameObject go = PhotonNetwork.Instantiate(prefabName, cam.position + cam.forward * 0.15f, cam.rotation, 0, data);
		}
	}
}