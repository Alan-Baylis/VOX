/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;
using Photon;
using System.Collections;

namespace VOX{
	public class Gun : GunBase{
		
		public const float lerpSpeed = 14f;
		public const float aimingLerpSpeed = 20f;
		public const float muzzleDuration = 0.025f;
		
		public Animation anim;
		public GameObject muzzle;
		
		bool tryingToAim;

		Location targetLoc;
		
		float t;

		[System.NonSerialized] public float reloadingT;
		[System.NonSerialized] public int ammo;
		[System.NonSerialized] public int maxAmmo;

		float fov = 90f;


		public Player player{
			get{ return ServerController.player; }
		}

		public bool drawing{
			get{ return anim.IsPlaying(info.drawClip.name); }
		}

		public bool aiming{
			get{ return tryingToAim && canAim; }
		}

		public bool reloading{
			get{ return Time.time < reloadingT + info.reloadTime; }
		}

		public bool canAim{
			get{ return !player.running && !drawing && !reloading; }
		}

		public bool canReload{
			get{ return ammo < info.ammo && maxAmmo > 0 && !drawing && !reloading && !player.running; }
		}

		public bool canShoot{
			get{ return Time.time > t && !reloading && !drawing && !player.running && ammo > 0; }
		}



		void Awake(){
			Reset();
		}

		void OnEnable(){
			anim.PlayQueued(info.drawClip.name, QueueMode.PlayNow);
			anim.PlayQueued(info.idleClip.name, QueueMode.CompleteOthers);

			t = Time.time + info.drawClip.length;
			muzzle.SetActive(false);
			tryingToAim = false;

			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			
			Vector3 position = info.normalLoc.position + Settings.GlobalViewmodelOffset;
			Location normalLoc = new Location(position, info.normalLoc.eulerAngles, info.normalLoc.scale);
			targetLoc = normalLoc;
			
			transform.localPosition = targetLoc.position;
			transform.localRotation = targetLoc.rotation;
			transform.localScale = targetLoc.scale;
		}

		void OnDisable(){
			StopReloading();
			main.camFov.fov = fov;
		}

		void Update(){
			if(!GameController.IsPaused()){
				tryingToAim = Input.GetKey(KeyCode.Mouse1) && canAim;
				bool tryingToShoot = info.auto ? Input.GetKey(KeyCode.Mouse0) : Input.GetKeyDown(KeyCode.Mouse0);

				if(tryingToShoot && canShoot){
					Fire();
				}

				if(Input.GetKeyDown(KeyCode.R) && canReload){
					Reload();
				}
			}else{
				tryingToAim = false;
			}

			if(player.running && !drawing){
				//Running
				Vector3 position = info.runningLoc.position + Settings.GlobalViewmodelOffset;
				Location runningLoc = new Location(position, info.runningLoc.eulerAngles, info.runningLoc.scale);

				targetLoc = runningLoc;
			}else{
				//Walking or Aiming
				Vector3 position = info.normalLoc.position + Settings.GlobalViewmodelOffset;
				Location normalLoc = new Location(position, info.normalLoc.eulerAngles, info.normalLoc.scale);

				targetLoc = !aiming ? normalLoc : info.aimingLoc;
			}

			transform.localPosition = Vector3.Lerp(transform.localPosition, targetLoc.position, (aiming ? aimingLerpSpeed : lerpSpeed) * Time.deltaTime);
			transform.localRotation = Quaternion.Slerp(transform.localRotation, targetLoc.rotation, (aiming ? aimingLerpSpeed : lerpSpeed) * Time.deltaTime);
			transform.localScale = Vector3.Lerp(transform.localScale, targetLoc.scale, (aiming ? aimingLerpSpeed : lerpSpeed) * Time.deltaTime);

			main.camFov.fov = player.aiming ? fov - info.aimedFov : fov;
		}

		public void Fire(){
			if(!canShoot)
				return;

			CrosshairRecoill();

			player.photonView.RPC("RPCFire", PhotonTargets.Others);

			t = Time.time + info.RPMToFireRate(aiming ? info.aimedRPM : info.RPM);

			StartCoroutine(IMuzzle());
			player.ShowLocalMuzzle();

			ammo--;

			float multiplier = 1f;

			if(player.running){
				multiplier = info.runningMultiplier;
			}else if(player.crouching){
				multiplier = info.crouchingMultiplier;
			}else{
				multiplier = 1f;
			}

			for(int i = 0; i < info.shots; i++){
				float randomX = Random.Range(-info.coneAngle / 4f, info.coneAngle / 4f) * multiplier;
				float randomY = Random.Range(-info.coneAngle / 4f, info.coneAngle / 4f) * multiplier;

				if(aiming){
					randomX = Random.Range(-info.aimedConeAngle / 4f, info.aimedConeAngle / 4f) * multiplier;
					randomY = Random.Range(-info.aimedConeAngle / 4f, info.aimedConeAngle / 4f) * multiplier;
				}

				player.raycaster.localEulerAngles = new Vector3(randomX, randomY);

				RaycastHit hit;
				if(Physics.Raycast(player.raycaster.position, player.raycaster.forward, out hit, float.MaxValue, GameController.Instance.fireLayers)){
					if(hit.transform.gameObject.layer == 31 && hit.transform.root != transform.root){
						Player target = hit.transform.root.GetComponent<Player>();

						if(target != null && !ServerController.AreFriends(target.photonView.owner, PhotonNetwork.player)){
							int damage = info.damage.torso;
						
							if(ServerController.gamemode != ServerController.Gamemode.OITC){
								if(hit.transform.tag == "Head")
									damage = info.damage.head;
								else if(hit.transform.tag == "Arm")
									damage = info.damage.arms;
								else if(hit.transform.tag == "Leg")
									damage = info.damage.legs;
							}else{
								damage = 100;
							}

							target.photonView.RPC("RPCDamage", target.photonView.owner, damage, player.raycaster.position,
								player.raycaster.forward, info.physicsForce, player.photonView.ownerId, info.ID
							);
							
							player.photonView.RPC("RPCBlood", PhotonTargets.All, hit.point, hit.normal);

							MapController.hud.ShowHitmarker();
						}
					}else if(hit.transform.gameObject.layer != 30){
						player.photonView.RPC("RPCBulletHole", PhotonTargets.All, hit.point, hit.normal);
					}
				}
			}
			
			anim.PlayQueued(info.fireClip.name, QueueMode.PlayNow);
			anim.PlayQueued(info.idleClip.name, QueueMode.CompleteOthers);

			player.Recoill(aiming ? info.aimedRecoill : info.recoill);
			main.source.PlayOneShot(info.fireSound);
		}

		public void Reset(){
			if(ServerController.gamemode != ServerController.Gamemode.OITC){
				ammo = info.ammo;
				maxAmmo = info.startMaxAmmo;
			}else{
				ammo = 1;
				maxAmmo = -1;
			}
		}

		public void Reload(){
			if(!canReload)
				return;

			StartCoroutine(IReload());

			if(info.reloadClip != null){
				anim.PlayQueued(info.reloadClip.name, QueueMode.PlayNow);
				anim.PlayQueued(info.idleClip.name, QueueMode.CompleteOthers);
			}
		}

		public void StopReloading(){
			StopCoroutine("IReload");
			reloadingT = -1f;

			MapController.hud.StopCrosshairAnimation();
		}


		
		IEnumerator IMuzzle(){
			muzzle.SetActive(true);
			
			float t = Time.time;
			while(Time.time < t + muzzleDuration){
				yield return 0;
			}

			muzzle.SetActive(false);
		}

		IEnumerator IReload(){
			reloadingT = Time.time;
			
			float t = Time.time + (info.reloadClip == null ? info.reloadTime : info.reloadClip.length);
			while(Time.time < t){
				yield return 0;
			}
			
			int ratio = info.ammo - ammo;

			ammo = info.ammo;
			maxAmmo -= ratio;

			if(maxAmmo < 0){
				ammo += maxAmmo;
				maxAmmo = 0;
			}

			MapController.hud.FinishCrosshairAnimation();
		}
	}
}