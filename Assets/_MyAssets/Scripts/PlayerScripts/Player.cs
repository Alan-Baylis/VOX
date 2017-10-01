/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;
using Photon;
using System.Collections;

namespace VOX{
	public class Player : PunBehaviour{
		
		public PlayerMovement movement;
		public PlayerMouseLook mouseLook;
		public PlayerItems items;
		public PlayerWorldModel worldModel;
		public AudioSource source;

		[Space]
		
		public Camera cam;
		public GameObject viewmodelRoot;
		public Transform raycaster;
		
		[Header("Vitals")]
		[Range(0, 100)] public int health = 100;

		[Header("Prefabs")]
		public GameObject bulletHole;
		public GameObject bloodParticle;

		[Header("CTF")]
		public FlagVisual ctfFlag;

		[Header("Local")]
		public GameObject localMuzzle;

		[Header("Audio")]
		public AudioClip[] customSounds;

		public bool grounded{
			get{
				if(photonView.isMine){
					return movement.grounded;
				}else{
					return false;
				}
			}
		}

		public bool walking{
			get{
				if(photonView.isMine){
					return movement.Moving();
				}else{
					return _walking;
				}
			}
		}

		bool _walking;

		public bool running{
			get{
				if(photonView.isMine){
					return movement.Running();
				}else{
					return _running;
				}
			}
		}

		bool _running;

		public bool crouching{
			get{
				if(photonView.isMine){
					return movement.Crouching();
				}else{
					return _crouching;
				}
			}
		}

		bool _crouching;

		public bool aiming{
			get{
				if(photonView.isMine){
					if(items.hasStarted){
						GunBase currentGunBase = items.currentGun;

						if(currentGunBase is Gun){
							return ((Gun) currentGunBase).aiming;
						}else{
							return false;
						}
					}else{
						return false;
					}
				}else{
					return _aiming;
				}
			}
		}

		bool _aiming;

		public bool reloading{
			get{
				if(photonView.isMine){
					if(items.hasStarted && items.currentGun is Gun)
						return ((Gun) items.currentGun).reloading;
					else
						return false;
				}else{
					return _reloading;
				}
			}
		}

		bool _reloading;

		public bool crouchAiming{
			get{
				if(photonView.isMine){
					return movement.CrouchAiming();
				}else{
					return _crouching && _aiming;
				}
			}
		}

		public bool isDead{
			get{ return health <= 0; }
		}



		void Awake(){
			if(photonView.isMine){
				ServerController.player = this;
				ServerController.onKill += OnKill;

				if(ServerController.roundTimer != null)
					ServerController.roundTimer.onRoundEnd += OnRoundEnd;
			}else{
				DontDestroyOnLoad(gameObject);
			}
		}

		void Start(){
			if(photonView.isMine){
				MapController.SetOfflineCamActive(false);

				/*
				ServerController.onPickupFlag += OnPickupFlag;
				ServerController.onReturnFlag += OnReturnFlag;
				*/
			}

			ServerController.RefreshScoreboard();
		}

		void OnDestroy(){
			if(photonView.isMine){
				ServerController.onKill -= OnKill;
				
				if(ServerController.roundTimer != null)
					ServerController.roundTimer.onRoundEnd -= OnRoundEnd;
				/*
				ServerController.onPickupFlag -= OnPickupFlag;
				ServerController.onReturnFlag -= OnReturnFlag;
				 */
			}
		}

		void Update(){
			if(photonView.isMine){
				SetCursorLocked(!GameController.IsPaused());

				worldModel.currentIndex = items.currentIndex;
				worldModel.Refresh(mouseLook.xRot);
			}else{
				float sendRate = 1f / (float) PhotonNetwork.sendRateOnSerialize;
				float ratio = Mathf.InverseLerp(t, t + sendRate, Time.time);

				transform.position = Vector3.Lerp(startPos, endPos, ratio);
				transform.rotation = Quaternion.Slerp(startRot, endRot, ratio);
				worldModel.Refresh(Mathf.Lerp(startXRot, endXRot, ratio));
			}
		}

		public void SetCursorLocked(bool locked){
			if(photonView.isMine){
				Cursor.visible = !locked;
				Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
			}
		}

		public void SetCursorLocked(bool locked, bool visible){
			if(photonView.isMine){
				Cursor.visible = visible;
				Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
			}
		}

		public void Recoill(Recoill recoill){
			if(photonView.isMine){
				mouseLook.Recoill(recoill);
			}
		}

		public void ShowLocalMuzzle(){
			StartCoroutine(ILocalMuzzle());
		}

		public void Reset(){
			if(photonView.isMine){
				if(isDead){
					Respawn();
					return;
				}

				Spawnpoint spawnpoint =  MapController.FindSpawnpoint(PhotonNetwork.player.GetTeam());
				transform.position = spawnpoint.position;
				transform.rotation = spawnpoint.rotation;

				mouseLook.xRot = 0f;
				items.Reset();
			}
		}
		
		Vector3 startPos;
		Quaternion startRot;
		Vector3 endPos;
		Quaternion endRot;
		float startXRot;
		float endXRot;
		float t;

		[System.NonSerialized] public int currentGunID;
		
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
			if(stream.isWriting){
				stream.SendNext(transform.position);
				stream.SendNext(transform.rotation);
				stream.SendNext(mouseLook.xRot);
				stream.SendNext(items.currentGun.info.ID);
				stream.SendNext(items.currentIndex);
				stream.SendNext(health);

				stream.SendNext(walking);
				stream.SendNext(running);

				/*stream.SendNext(crouching);
				stream.SendNext(aiming);
				stream.SendNext(reloading);*/
			}else{
				startPos = endPos;
				startRot = endRot;
				startXRot = endXRot;

				endPos = (Vector3) stream.ReceiveNext();
				endRot = (Quaternion) stream.ReceiveNext();
				endXRot = (float) stream.ReceiveNext();

				t = Time.time;

				currentGunID = (int) stream.ReceiveNext();
				worldModel.currentIndex = (int) stream.ReceiveNext();
				health = (int) stream.ReceiveNext();

				_walking = (bool) stream.ReceiveNext();
				_running = (bool) stream.ReceiveNext();

				/*_crouching = (bool) stream.ReceiveNext();
				_aiming = (bool) stream.ReceiveNext();
				_reloading = (bool) stream.ReceiveNext();*/
			}
		}

		[PunRPC]
		public void RPCPlayCustomSound(int clipIndex){
			clipIndex = Mathf.Clamp(clipIndex, 0, customSounds.Length);

			AudioClip clip = customSounds[clipIndex];
			source.PlayOneShot(clip);
		}

		[PunRPC]
		public void RPCFire(){
			AudioClip clip = GameController.GetGun(currentGunID).fireSound;
			source.PlayOneShot(clip);

			worldModel.ShowMuzzle();
		}

		[PunRPC]
		public void RPCBulletHole(Vector3 point, Vector3 normal){
			Instantiate(bulletHole, point, Quaternion.LookRotation(normal));
		}

		[PunRPC]
		public void RPCBlood(Vector3 point, Vector3 normal){
			Instantiate(bloodParticle, point, Quaternion.LookRotation(normal));
		}
		
		[PunRPC]
		public void RPCDamage(int damage, Vector3 raycasterPosition, Vector3 raycasterForward, float physicsForce, int damagerID, int damagerWeaponID){
			if(ServerController.HasRoundEnded)
				return;

			if(isDead || !photonView.isMine)
				return;

			if(health - damage <= 0){
				Die(raycasterPosition, raycasterForward, physicsForce, damagerID, damagerWeaponID);
				return;
			}

			health -= damage;
			MapController.hud.Hit(damage);
		}

		[PunRPC]
		public void RPCDamageCustomReason(int damage, string customReason){
			if(ServerController.HasRoundEnded)
				return;

			if(isDead || !photonView.isMine)
				return;

			if(health - damage <= 0){
				DieCustomReason(customReason);
				return;
			}

			health -= damage;
			MapController.hud.Hit(damage);
		}

		public void DieCustomReason(string customReason, bool respawnCoroutine = true){
			if(ServerController.HasRoundEnded)
				return;

			Vector3 velocity = movement.controller.velocity;

			worldModel.gameObject.SetActive(true);
			worldModel.enabled = true;
			worldModel.currentIndex = items.currentIndex;

			movement.controller.enabled = false;
			movement.enabled = false;
			mouseLook.enabled = false;
			items.enabled = false;
			
			MapController.SetOfflineCamActive(true);
			mouseLook.mouseLookRoot.gameObject.SetActive(false);
			
			CancelRespawn();
			if(respawnCoroutine){
				StartCoroutine("IRespawn");
			}

			/*
			if(ServerController.IsHoldingAnyFlag(photonView)){
				if(ServerController.redFlagOwner == photonView){
					ServerController.ReturnFlag(ServerController.CTFFlag.Red);
				}else if(ServerController.blueFlagOwner == photonView){
					ServerController.ReturnFlag(ServerController.CTFFlag.Blue);
				}
			}
			*/

			photonView.RPC("RPCDieCustomReason", PhotonTargets.All, velocity, customReason);
		}

		public void Die(Vector3 raycasterPosition, Vector3 raycasterForward, float physicsForce, int damagerID, int killerWeaponID){
			if(ServerController.HasRoundEnded)
				return;

			Vector3 velocity = movement.controller.velocity;

			worldModel.gameObject.SetActive(true);
			worldModel.enabled = true;
			worldModel.currentIndex = items.currentIndex;

			movement.controller.enabled = false;
			movement.enabled = false;
			mouseLook.enabled = false;
			items.enabled = false;
			
			MapController.SetOfflineCamActive(true);
			mouseLook.mouseLookRoot.gameObject.SetActive(false);
			
			CancelRespawn();
			StartCoroutine("IRespawn");

			photonView.RPC("RPCDie", PhotonTargets.All, velocity, raycasterPosition, raycasterForward, physicsForce, damagerID, killerWeaponID);
		}
		
		[PunRPC]
		public void RPCDieCustomReason(Vector3 velocity, string customReason){
			if(ServerController.HasRoundEnded)
				return;

			health = 0;

			worldModel.Die(velocity);

			ServerController.OnDieCustomReason(photonView.owner, customReason);
		}

		[PunRPC]
		public void RPCDie(Vector3 velocity, Vector3 raycasterPosition, Vector3 raycasterForward, float physicsForce, int killerID, int killerWeaponID){
			if(ServerController.HasRoundEnded)
				return;

			PhotonPlayer killer = PhotonPlayer.Find(killerID);
			GunInfo killerGun = GameController.GetGun(killerWeaponID);

			health = 0;

			worldModel.Die(velocity);

			bool headshot = false;

			RaycastHit hit;
			if(Physics.Raycast(raycasterPosition, raycasterForward, out hit, float.MaxValue, GameController.Instance.afterKillFireLayers)){
				if(hit.transform.gameObject.layer == 30){
					headshot = (hit.transform.tag == "Head") && !killerGun.disableHeadshots;

					Rigidbody rigid = hit.collider.attachedRigidbody;
					rigid.AddForceAtPosition(raycasterForward * physicsForce, hit.point, ForceMode.Impulse);
				}
			}


			ServerController.OnKill(killer, killerGun, photonView.owner, headshot);
		}

		[PunRPC]
		public void RPCRespawn(int primaryWeaponID, int secondaryWeaponID, int meleeID, int item1ID, int item2ID){
			if(photonView.isMine){
				Destroy(cam.gameObject);
				Destroy(viewmodelRoot);

				PhotonNetwork.Destroy(gameObject);
				
				object[] data = new object[5];

				if(ServerController.gamemode == ServerController.Gamemode.GG){
					int kills = ServerController.GetKills(PhotonNetwork.player);
					GunInfo ggGun = ServerController.Instance.gunGameGuns[kills];

					data[0] = ggGun.ID;
					data[1] = -1;
					data[2] = -1;
					data[3] = -1;
					data[4] = -1;
				}else{
					data[0] = primaryWeaponID;
					data[1] = secondaryWeaponID;
					data[2] = meleeID;
					data[3] = item1ID;
					data[4] = item2ID;
				}

				Spawnpoint spawnpoint = MapController.FindSpawnpoint(PhotonNetwork.player.GetTeam());
				GameObject go = PhotonNetwork.Instantiate("Player", spawnpoint.position, spawnpoint.rotation, 0, data);
			}
		}

		[PunRPC]
		public void RPCSendChatMessage(string message){
			ServerController.OnChatMessage(photonView.owner, message);
		}

		[PunRPC]
		public void RPCReset(){
			Reset();
		}

		public void Respawn(int primary, int secondary, int melee, int item1, int item2){
			photonView.RPC("RPCRespawn", photonView.owner, primary, secondary, melee, item1, item2);
		}

		public void Respawn(){
			object[] data = photonView.instantiationData;
			int primary = (int) data[0];
			int secondary = (int) data[1];
			int melee = (int) data[2];
			int item1 = (int) data[3];
			int item2 = (int) data[4];

			Respawn(primary, secondary, melee, item1, item2);
		}

		IEnumerator IRespawn(){
			yield return new WaitForSeconds(4f);

			Respawn();
		}

		IEnumerator ILocalMuzzle(){
			localMuzzle.SetActive(true);
			
			float t = Time.time;
			while(Time.time < t + Gun.muzzleDuration){
				yield return 0;
			}

			localMuzzle.SetActive(false);
		}

		public void CancelRespawn(){
			StopCoroutine("IRespawn");
		}

		public void OnRoundEnd(){
			ServerController.SaveInfoToGamejolt();

			CancelRespawn();
			MapController.hud.classCreation.Close();
		}



		public void OnKill(PhotonPlayer killer, GunInfo gun, PhotonPlayer dead, bool headshot){
			bool isKiller = killer.ID == photonView.owner.ID;

			if(ServerController.gamemode == ServerController.Gamemode.OITC){
				//OITC

				if(isKiller && photonView.isMine){
					((Gun) items.secondary).ammo++;
				}
			}else if(ServerController.gamemode == ServerController.Gamemode.GG){
				//GUNGAME

				GunInfo[] guns = ServerController.Instance.gunGameGuns;
				int kills = ServerController.GetKills(killer);

				if(kills < guns.Length){
					GunInfo newGun = guns[kills];
					
					if(isKiller){
						if(photonView.isMine){
							if(items.primary != null)
								Destroy(items.primary.gameObject);

							items.primary = items.InstantiateGun(newGun.ID);
						}

						if(worldModel.primary != null)
							Destroy(worldModel.primary.gameObject);

						worldModel.primary = worldModel.InstantiateGun(newGun.ID);

					}
				}else{
					ServerController.Instance.EndRound();
				}
			}
		}

		#region CTF
		/*
		void OnTriggerEnter(Collider coll){
			if(coll.tag == "CTFFlag"){
				Flag flag = coll.GetComponent<Flag>();
				if(flag != null){
					ServerController.PickupFlag(photonView, flag.team);
				}
			}
		}

		public void OnPickupFlag(PhotonView view, ServerController.CTFFlag flag){
			if(view == photonView){
				ctfFlag.gameObject.SetActive(true);
				ctfFlag.RefreshModel(flag);
			}
		}

		public void OnReturnFlag(ServerController.CTFFlag flag){
			if(ctfFlag.gameObject.activeSelf){
				ctfFlag.gameObject.SetActive(false);
			}
		}
		*/
		#endregion
	}
}