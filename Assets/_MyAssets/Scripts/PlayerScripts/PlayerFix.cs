/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;
using Photon;

namespace VOX{
	public class PlayerFix : PunBehaviour{
		
		Player player;

		void Awake(){
			player = GetComponent<Player>();
		}

		void Start(){
			if(!photonView.isMine){
				//Is other player
				player.movement.controller.enabled = false;
				player.movement.enabled = false;
				player.mouseLook.enabled = false;
				player.items.enabled = false;
				
				Destroy(player.mouseLook.mouseLookRoot.gameObject);
				
				player.worldModel.enabled = true;
				player.worldModel.gameObject.SetActive(true);
			}else{
				//Is local
				player.worldModel.enabled = false;
				player.worldModel.gameObject.SetActive(false);
			}
		}
	}
}