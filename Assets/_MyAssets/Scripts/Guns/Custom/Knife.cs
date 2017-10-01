/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;

namespace VOX{
	public class Knife : GunBase{
		
		[Header("Values")]
		public float hitDistance = 2f;
		public int damage = 25;
		public float hitForce = 5f;
		public float fireRate = 0.25f;

		[Header("Sounds")]
		public AudioClip missSound;
		public AudioClip wallSound;
		public AudioClip fleshSound;

		[Header("Animation")]
		public Animation anim;
		public AnimationClip drawAnimation;
		public AnimationClip attackAnimation;

		float t;
		
		public Player player{
			get{ return ServerController.player; }
		}

		public bool canAttack{
			get{ return Time.time > t; }
		}

		void OnEnable(){
			t = Time.time + drawAnimation.length;
		}

		void OnDisable(){
			CancelInvoke("ResetSize");
			ResetSize();
		}

		void Update(){
			transform.localPosition = Settings.GlobalViewmodelOffset;

			if(!GameController.IsPaused()){
				if(Input.GetKey(KeyCode.Mouse0) && canAttack){
					Attack();
				}
			}
		}

		void Attack(){
			if(!canAttack)
				return;
			
			CrosshairRecoill();

			t = Time.time + fireRate;
			AudioClip soundToPlay = missSound;

			player.raycaster.localEulerAngles = Vector3.zero;
			Ray ray = new Ray(player.raycaster.position, player.raycaster.forward);

			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, hitDistance, GameController.Instance.fireLayers)){
				if(hit.transform.gameObject.layer == 31 && hit.transform.root != transform.root){
					Player target = hit.transform.root.GetComponent<Player>();

					if(target != null && !ServerController.AreFriends(target.photonView.owner, PhotonNetwork.player)){
						soundToPlay = fleshSound;

						int finalDamage = ServerController.gamemode != ServerController.Gamemode.OITC ? damage : 100;
						target.photonView.RPC("RPCDamage", target.photonView.owner, finalDamage, ray.origin, ray.direction, hitForce, player.photonView.ownerId, info.ID);

						player.photonView.RPC("RPCBlood", PhotonTargets.All, hit.point, hit.normal);

						MapController.hud.ShowHitmarker();
					}
				}else{
					if(hit.transform.gameObject.layer == 30){
						soundToPlay = fleshSound;
					}else{
						soundToPlay = wallSound;

						player.photonView.RPC("RPCBulletHole", PhotonTargets.All, hit.point, hit.normal);
					}
				}
			}
			
			anim.Stop();

			anim.PlayQueued(attackAnimation.name, QueueMode.PlayNow);
			anim.PlayQueued(drawAnimation.name, QueueMode.CompleteOthers);

			RandomizeSize();

			main.source.PlayOneShot(soundToPlay);

			if(soundToPlay == wallSound){
				player.photonView.RPC("RPCPlayCustomSound", PhotonTargets.Others, 1);
			}else if(soundToPlay == fleshSound){
				player.photonView.RPC("RPCPlayCustomSound", PhotonTargets.Others, 2);
			}else{
				player.photonView.RPC("RPCPlayCustomSound", PhotonTargets.Others, 0);
			}
		}
		
		void RandomizeSize(){
			int random = Random.Range(0, 2);
			float size = random == 0 ? 1f : -1f;

			transform.localScale = new Vector3(size, 1f, 1f);

			CancelInvoke("ResetSize");
			Invoke("ResetSize", attackAnimation.length);
		}

		void ResetSize(){
			transform.localScale = Vector3.one;
		}
	}
}