/*
* Copyright (c) Retardevs
* http://gamejolt.com/@retardevs
*/

using UnityEngine;

namespace Cirda.Utils{
	public class TestWeaponSwitch : MonoBehaviour{

		[Range(1, 9)] public int startWeapon = 1;
		public bool useScrollWheel = true;

		[Space]

		public GameObject weapon1;
		public GameObject weapon2;
		public GameObject weapon3;
		public GameObject weapon4;
		public GameObject weapon5;
		public GameObject weapon6;
		public GameObject weapon7;
		public GameObject weapon8;
		public GameObject weapon9;
		
		public int lastWeapon;
		public int currentIndex;

		void Start(){
			ChangeWeapon(startWeapon);
		}

		void Update(){
			if(weapon1 == null && weapon2 == null && weapon3 == null && weapon4 == null && weapon5 == null &&
				weapon6 == null && weapon7 == null && weapon8 == null && weapon9 == null
				){

				return;
			}

			if(Input.GetKeyDown(KeyCode.Alpha1))
				ChangeWeapon(1);
			if(Input.GetKeyDown(KeyCode.Alpha2))
				ChangeWeapon(2);
			if(Input.GetKeyDown(KeyCode.Alpha3))
				ChangeWeapon(3);
			if(Input.GetKeyDown(KeyCode.Alpha4))
				ChangeWeapon(4);
			if(Input.GetKeyDown(KeyCode.Alpha5))
				ChangeWeapon(5);
			if(Input.GetKeyDown(KeyCode.Alpha6))
				ChangeWeapon(6);
			if(Input.GetKeyDown(KeyCode.Alpha7))
				ChangeWeapon(7);
			if(Input.GetKeyDown(KeyCode.Alpha8))
				ChangeWeapon(8);
			if(Input.GetKeyDown(KeyCode.Alpha9))
				ChangeWeapon(9);
			
			if(Input.GetAxis("Mouse ScrollWheel") > 0f){
				ChangeWeapon(currentIndex - 1);
			}else if(Input.GetAxis("Mouse ScrollWheel") < 0f){
				ChangeWeapon(currentIndex + 1);
			}
		}

		public bool ChangeWeapon(int weapon){
			weapon = Mathf.Clamp(weapon, 1, 9);

			lastWeapon = weapon;

			if(weapon1 != null)
				weapon1.SetActive(weapon == 1);
			else if(weapon == 1){
				ChangeWeapon(weapon - 1);
				return false;
			}

			if(weapon2 != null)
				weapon2.SetActive(weapon == 2);
			else if(weapon == 2){
				ChangeWeapon(weapon - 1);
				return false;
			}

			if(weapon3 != null)
				weapon3.SetActive(weapon == 3);
			else if(weapon == 3){
				ChangeWeapon(weapon - 1);
				return false;
			}

			if(weapon4 != null)
				weapon4.SetActive(weapon == 4);
			else if(weapon == 4){
				ChangeWeapon(weapon - 1);
				return false;
			}

			if(weapon5 != null)
				weapon5.SetActive(weapon == 5);
			else if(weapon == 5){
				ChangeWeapon(weapon - 1);
				return false;
			}

			if(weapon6 != null)
				weapon6.SetActive(weapon == 6);
			else if(weapon == 6){
				ChangeWeapon(weapon - 1);
				return false;
			}

			if(weapon7 != null)
				weapon7.SetActive(weapon == 7);
			else if(weapon == 7){
				ChangeWeapon(weapon - 1);
				return false;
			}

			if(weapon8 != null)
				weapon8.SetActive(weapon == 8);
			else if(weapon == 8){
				ChangeWeapon(weapon - 1);
				return false;
			}

			if(weapon9 != null)
				weapon9.SetActive(weapon == 9);
			else if(weapon == 9){
				ChangeWeapon(weapon - 1);
				return false;
			}

			currentIndex = weapon;
			return true;
		}
	}
}