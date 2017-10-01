/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;

namespace VOX{
	public class OnlyOnGamemode : MonoBehaviour{
		
		public GameObject target;

		[Header("Gamemode")]
		public bool reverse;
		public ServerController.Gamemode gamemode;

		void OnEnable(){
			Refresh();
		}

		public void Refresh(){
			bool isActive = reverse ? (ServerController.gamemode != gamemode) : (ServerController.gamemode == gamemode);
			target.SetActive(isActive);
		}
	}
}