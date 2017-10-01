/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;

namespace VOX{
	public class Spawnpoint : MonoBehaviour{
		
		public PunTeams.Team team;

		[Header("Editor Visuals")]
		public GameObject model;
		public Renderer rend;
		public Material redTeam;
		public Material blueTeam;
		public Material noneTeam;
		
		public Vector3 position{
			get{ return transform.position; }
		}

		public Quaternion rotation{
			get{ return transform.rotation; }
		}

		void Awake(){
			Destroy(model);
		}

#if UNITY_EDITOR
		void OnValidate(){
			rend.material = ServerController.GetObjectFromTeam<Material>(team, redTeam, blueTeam, noneTeam);
			gameObject.name = string.Format("Spawnpoint ({0})", ServerController.GetObjectFromTeam<string>(team, "Red", "Blue", "None"));
		}
#endif
	}
}