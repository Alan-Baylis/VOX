/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;

namespace VOX{
	public abstract class GunBase : MonoBehaviour{
		
		public GunInfo info;

		[System.NonSerialized] public MainGunsScript main;

		public void Reset(){
			if(this is Gun){
				((Gun) this).Reset();
			}
		}

		public void CrosshairRecoill(){
			Player player = ServerController.player;
			if(player != null && MapController.hud != null){
				MapController.hud.CrosshairRecoill(info.crosshairRecoil);
			}
		}
	}
}