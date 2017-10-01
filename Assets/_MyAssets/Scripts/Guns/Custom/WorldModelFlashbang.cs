/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;
using Photon;
using System.Collections;

namespace VOX{
	public class WorldModelFlashbang : PunBehaviour{
		
		public GunInfo flashbang;
		public Rigidbody rigid;
		public float throwForce = 15f;
		public float explodeTime = 3f;

		[Header("Explosion")]
		public GameObject explosionLight;
		public float flashDuration = 3f;
		public float fadeOutDuration = 2f;
		public float radius = 2f;
		public float maxRadius = 7f;
		public float explosionLightDuration = 0.1f;

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

		IEnumerator IExplosionLight(){
			explosionLight.transform.SetParent(null, true);
			explosionLight.transform.localScale = Vector3.one;

			explosionLight.SetActive(true);
			
			float t = Time.time;
			while(Time.time < t + explosionLightDuration){
				yield return 0;
			}

			Destroy(explosionLight);
		}

		[PunRPC]
		public void RPCExplode(){
			StartCoroutine(IExplosionLight());
			source.transform.SetParent(null, true);
			source.Play();
			
			Destroy(source.gameObject, source.clip.length * 1.1f);

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
						if(hit.collider == coll){
							Vector3 screenPoint = player.cam.WorldToViewportPoint(transform.position);
							bool isOnScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;

							float ratio = Mathf.InverseLerp(maxRadius, radius, Mathf.Clamp(
								Vector3.Distance(player.transform.position, transform.position), radius, maxRadius
							));

							if(isOnScreen){
								MapController.hud.Flash(flashDuration * ratio, fadeOutDuration * ratio);
							}else{
								MapController.hud.SmallFlash();
							}
						}
					}
				}
			}

		}
	}
}