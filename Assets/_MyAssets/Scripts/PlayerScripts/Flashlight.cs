/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;

namespace VOX{
	public class Flashlight : MonoBehaviour{
		
		public Light flashlight;

		void Update(){
			if(Input.GetKeyDown(KeyCode.F)){
				flashlight.enabled = !flashlight.enabled;
			}
		}

	}
}