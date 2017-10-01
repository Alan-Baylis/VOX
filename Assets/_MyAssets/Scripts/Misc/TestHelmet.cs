/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;
using Photon;

namespace VOX{
	public class TestHelmet : UnityEngine.MonoBehaviour{
		
		public Renderer renderer;
		public Material noneTeamMaterial;
		public Material redTeamMaterial;
		public Material blueTeamMaterial;

		void Start(){
			PunTeams.Team team = PhotonView.Get(transform.root.gameObject).owner.GetTeam();
			renderer.material = ServerController.GetObjectFromTeam<Material>(team, redTeamMaterial, blueTeamMaterial, noneTeamMaterial);
		}

	}
}