/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;
using Photon;
using System.Collections;

namespace VOX{
	public class PlayerWorldModel : PunBehaviour{
		
		public Player player;
		public Ragdoll ragdoll;

		[Space]

		[Range(-90f, 90f)] public float xRot = 0f;

		[Header("Guns")]
		public Transform gunsParent;
		public WorldModelGun primary;
		public WorldModelGun secondary;
		public WorldModelGun melee;
		public WorldModelGun item1;
		public WorldModelGun item2;
		
		[Header("Transforms")]
		public Transform torso;
		public Transform head;
		public Transform arms;

		[Header("Rigidbodies")]
		public Rigidbody torsoRigidbody;

		[Header("Rotations")]
		public Vector3 minTorsoRot;
		public Vector3 maxTorsoRot;

		[Space]

		public Vector3 minHeadRot;
		public Vector3 maxHeadRot;

		[Space]

		public Vector3 minArmsRot;
		public Vector3 maxArmsRot;

		[Header("Animations")]
		public Animator anim;

		[System.NonSerialized] public bool hasStarted;
		[System.NonSerialized] public int currentIndex = 0;
		[System.NonSerialized] public int lastIndex = -1;
		
		public WorldModelGun currentGun{
			get{
				if(!hasStarted)
					return null;

				switch(currentIndex){
					case 1:
					return primary;
					case 2:
					return secondary;
					case 3:
					return melee;
					case 4:
					return item1;
				}

				return item2;
			}
		}

		void Awake(){
			object[] data = player.photonView.instantiationData;
			int primaryID = (int) data[0];
			int secondaryID = (int) data[1];
			int meleeID = (int) data[2];
			int item1ID = (int) data[3];
			int item2ID = (int) data[4];
			
			if(primaryID >= 0)
				primary = InstantiateGun(primaryID);
			if(secondaryID >= 0)
				secondary = InstantiateGun(secondaryID);
			if(meleeID >= 0)
				melee = InstantiateGun(meleeID);
			if(item1ID >= 0)
				item1 = InstantiateGun(item1ID);
			if(item2ID >= 0)
				item2 = InstantiateGun(item2ID);

			hasStarted = true;
		}

		void Start(){
			Refresh();
		}

		void Update(){
			if(!ragdoll.isActive){
				ChangeWeapon(currentIndex);

				if(!player.photonView.isMine){
					Refresh();
				}
				
				anim.SetBool("Walking", player.walking);
				anim.SetBool("Running", player.running);
				anim.SetBool("Crouching", player.crouching);
				anim.SetBool("Aiming", player.aiming);
			}
		}

		public void Die(Vector3 velocity){
			ChangeWeapon(currentIndex);

			anim.Stop();
			anim.enabled = false;

			ragdoll.ActivateRagdoll();

			transform.SetParent(null);

			if(currentGun.drop != null)
				currentGun.drop.SetParent(null);
			
			Destroy(gameObject, 4f);
			Destroy(currentGun.drop.gameObject, 4f);

			if(velocity != Vector3.zero)
				torsoRigidbody.AddForce(velocity * 3f, ForceMode.Impulse);
		}

		public void Refresh(){
			xRot = Mathf.Clamp(xRot, -90f, 90f);
			float value = Mathf.InverseLerp(90f, -90f, xRot);
			
			torso.localEulerAngles = Vector3.Lerp(minTorsoRot, maxTorsoRot, value);
			head.localEulerAngles = Vector3.Lerp(minHeadRot, maxHeadRot, value);
			arms.localEulerAngles = Vector3.Lerp(minArmsRot, maxArmsRot, value);
		}
		
		public void Refresh(float newXRot){
			xRot = newXRot;
			Refresh();
		}

		public void ChangeWeapon(int weapon){
			if(hasStarted && lastIndex != currentIndex){
				lastIndex = currentIndex;
				weapon = Mathf.Clamp(weapon, 1, 5);

				if(primary != null)
					primary.gameObject.SetActive(weapon == 1);
				if(secondary != null)
					secondary.gameObject.SetActive(weapon == 2);
				if(melee != null)
					melee.gameObject.SetActive(weapon == 3);
				if(item1 != null)
					item1.gameObject.SetActive(weapon == 4);
				if(item2 != null)
					item2.gameObject.SetActive(weapon == 5);
			}
		}

		public WorldModelGun InstantiateGun(int ID){
			GunInfo gunInfo = GameController.GetGun(ID);
			GameObject go = Instantiate(gunInfo.worldmodel) as GameObject;

			go.name = string.Format("WM_{0}", gunInfo.Name);

			go.transform.SetParent(gunsParent.transform, false);
			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = Vector3.one;

			WorldModelGun gun = go.GetComponent<WorldModelGun>();
			gun.info = gunInfo;
			gun.leftArmCharacterJoint.connectedBody = torsoRigidbody;
			gun.rightArmCharacterJoint.connectedBody = torsoRigidbody;

			CharacterCustomization customization = go.GetComponent<CharacterCustomization>();
			customization.StartUp();

			return gun;
		}

		public void ShowMuzzle(){
			if(currentGun != null && currentGun.muzzle != null){
				StartCoroutine(IMuzzle());
			}
		}

		IEnumerator IMuzzle(){
			currentGun.muzzle.SetActive(true);
			
			float t = Time.time;
			while(Time.time < t + Gun.muzzleDuration){
				yield return 0;
			}

			currentGun.muzzle.SetActive(false);
		}
	}
}