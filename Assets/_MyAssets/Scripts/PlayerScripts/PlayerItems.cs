/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;

namespace VOX{
	public class PlayerItems : Photon.MonoBehaviour{

		public bool useScrollWheel = true;
		
		[Header("Guns")]
		public GunBase primary;
		public GunBase secondary;
		public GunBase melee;
		public GunBase item1;
		public GunBase item2;

		[Header("Instantiation")]
		public MainGunsScript mainGunsScript;
		
		[System.NonSerialized] public bool hasStarted;
		[System.NonSerialized] public int currentIndex = 0;
		int lastIndex = -1;

		public GunBase currentGun{
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
			object[] data = photonView.instantiationData;
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
			if(ServerController.gamemode != ServerController.Gamemode.OITC)
				ChangeWeapon(1);
			else
				ChangeWeapon(2);
		}

		void Update(){
			if(photonView.isMine && !GameController.IsPaused()){
				if(primary == null && secondary == null && melee == null && item1 == null && item2 == null)
					return;

				if(Input.GetKeyDown(KeyCode.Alpha1) && primary != null)
					ChangeWeapon(1);
				if(Input.GetKeyDown(KeyCode.Alpha2) && secondary != null)
					ChangeWeapon(2);
				if(Input.GetKeyDown(KeyCode.Alpha3) && melee != null)
					ChangeWeapon(3);
				if(Input.GetKeyDown(KeyCode.Alpha4) && item1 != null)
					ChangeWeapon(4);
				if(Input.GetKeyDown(KeyCode.Alpha5) && item2 != null)
					ChangeWeapon(5);

				if(Input.GetKeyDown(KeyCode.Q) && lastIndex != -1)
					ChangeWeapon(lastIndex);
			}
		}

		public void ChangeWeapon(int weapon){
			weapon = Mathf.Clamp(weapon, 1, 5);

			if(photonView.isMine && lastIndex != currentIndex && weapon != currentIndex){
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

				lastIndex = currentIndex;
				currentIndex = weapon;
			}
		}

		public void Reset(){
			ChangeWeapon(1);

			if(primary != null)
				primary.Reset();
			if(secondary != null)
				secondary.Reset();
			if(melee != null)
				melee.Reset();
			if(item1 != null)
				item1.Reset();
			if(item2 != null)
				item2.Reset();
		}

		public GunBase InstantiateGun(int ID){
			GunInfo gunInfo = GameController.GetGun(ID);
			GameObject go = Instantiate(gunInfo.viewmodel) as GameObject;

			go.name = string.Format("VM_{0}", gunInfo.Name);

			go.transform.SetParent(mainGunsScript.transform, false);
			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = Vector3.one;

			GunBase gun = go.GetComponent<GunBase>();
			gun.info = gunInfo;
			gun.main = mainGunsScript;

			CharacterCustomization customization = go.GetComponent<CharacterCustomization>();
			customization.StartUp();

			return gun;
		}
	}
}