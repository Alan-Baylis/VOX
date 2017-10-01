/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;

namespace VOX{
	public class FlagVisual : MonoBehaviour{
		
		public Renderer rend;
		public Material redTeamMaterial;
		public Material blueTeamMaterial;

		/*
		public void RefreshModel(ServerController.CTFFlag team){
			if(rend == null || redTeamMaterial == null || blueTeamMaterial == null)
				return;

			rend.material = team == ServerController.CTFFlag.Red ? redTeamMaterial : blueTeamMaterial;
		}
		*/
	}
}