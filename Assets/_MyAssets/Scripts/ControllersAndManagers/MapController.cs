/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;
using Photon;

namespace VOX{
	public class MapController : PunBehaviour{
		
		public static MapController Instance;

		public GameObject hudPrefab;
		public GameObject optionsWindowPrefab;
		public GameObject offlineCam;
		
		public static HudExample hud;
		public static OptionsWindow optionsMenu;

		public static Spawnpoint[] spawnpoints;

		void Awake(){
			Instance = this;
		}

		void Start(){
			SetupSpawnpoints();

			GameObject optionsGo = Instantiate(optionsWindowPrefab) as GameObject;
			optionsMenu = optionsGo.GetComponent<OptionsWindow>();

			GameObject hudGo = Instantiate(hudPrefab) as GameObject;
			hud = hudGo.GetComponent<HudExample>();
		}

		#region Spawnpoints
		public static void SetupSpawnpoints(){
			spawnpoints = GameObject.FindObjectsOfType<Spawnpoint>();
		}

		public static Spawnpoint FindSpawnpoint(){
			int i = UnityEngine.Random.Range(0, spawnpoints.Length);
			return spawnpoints[i];
		}

		public static Spawnpoint FindSpawnpoint(PunTeams.Team team){
			Spawnpoint spawnpoint = FindSpawnpoint();

			if(team != PunTeams.Team.none){
				while(spawnpoint.team != team){
					spawnpoint = FindSpawnpoint();
				}
			}

			return spawnpoint;
		}
		#endregion

		#region Offline Camera
		public static void SetOfflineCamActive(bool active){
			if(MapController.Instance != null && MapController.Instance.offlineCam != null)
				MapController.Instance.offlineCam.SetActive(active);
		}
		#endregion

		#region Photon Events
		public override void OnJoinedRoom(){
			SetupSpawnpoints();
		}
		#endregion
	}
}