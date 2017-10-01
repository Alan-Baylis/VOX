/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;
using Photon;
using System.Collections;

namespace VOX{
	public class WorldModelHEGrenade : PunBehaviour{

		public GunInfo grenade;
		public Rigidbody rigid;
		public float throwForce = 15f;
		public float explodeTime = 3f;
		public float damage = 125f;

		[Header("Explosion")]
		public ParticleSystem explosionParticle;
		public float radius = 2f;
		public float maxRadius = 7f;

		[Header("Sounds")]
		public AudioSource source;

		void OnDrawGizmosSelected(){
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, radius);

			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(transform.position, maxRadius);
		}

		Vector3 forward;

		void Start(){
			if(photonView.isMine){
				object[] data = photonView.instantiationData;
				forward = (Vector3) data[0];

				rigid.AddForce(forward * throwForce, ForceMode.Impulse);
				StartCoroutine(IExplode());
			}
		}

		IEnumerator IExplode(){
			yield return new WaitForSeconds(explodeTime);

			photonView.RPC("RPCExplode", PhotonTargets.All);
			
			yield return new WaitForSeconds(0.25f);

			PhotonNetwork.Destroy(gameObject);
		}

		[PunRPC]
		void RPCExplode(){
			explosionParticle.transform.SetParent(null, true);
			explosionParticle.transform.localScale = Vector3.one;
			explosionParticle.Emit(explosionParticle.maxParticles);

			Destroy(explosionParticle.gameObject, Mathf.Max(explosionParticle.startLifetime * 1.1f, source.clip.length * 1.1f));

			source.Play();

			if(Physics.CheckSphere(transform.position, maxRadius, GameController.Instance.explosionLayers)){
				Collider[] colliders = Physics.OverlapSphere(transform.position, maxRadius, GameController.Instance.explosionLayers);

				for(int i = 0; i < colliders.Length; i++){
					Collider coll = colliders[i];
					Player player = coll.transform.root.GetComponent<Player>();
					
					if(player == null || coll.tag != "Player")
						continue;

					if(!player.photonView.isMine)
						continue;
					
					RaycastHit hit;
					if(Physics.Linecast(transform.position, player.mouseLook.mouseLookRoot.position, out hit, GameController.Instance.explosionLayers)){
						if(hit.collider == coll && !ServerController.AreFriends(photonView.owner, player.photonView.owner, true)){
							float ratio = Mathf.InverseLerp(maxRadius, radius, Mathf.Clamp(
								Vector3.Distance(player.transform.position, transform.position), radius, maxRadius
							));

							int finalDamage = Mathf.RoundToInt(damage * ratio);

							if(player.health - finalDamage > 0){
								player.photonView.RPC("RPCDamageCustomReason", player.photonView.owner, finalDamage, grenade.KillfeedName);
							}else{
								if(player.photonView.owner != photonView.owner){
									player.photonView.RPC("RPCDamage", player.photonView.owner, finalDamage,
										Vector3.zero, Vector3.forward, 0f, photonView.ownerId, grenade.ID
									);
								}else{
									player.photonView.RPC("RPCDamageCustomReason", player.photonView.owner, finalDamage, grenade.KillfeedName);
								}
							}
						}
					}
				}
			}
		}
	}
}