/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;
using Photon;
using System.Collections.Generic;

namespace VOX{
	public class Scoreboard : PunBehaviour{

		public GameObject scoreboard;
		public GameObject scoreboardPlayerPrefab;
		
		[Header("Groups")]
		public GameObject redTeamGroup;
		public GameObject blueTeamGroup;
		public GameObject noneTeamGroup;

		Dictionary<PhotonPlayer, ScoreboardPlayer> players = new Dictionary<PhotonPlayer,ScoreboardPlayer>();

		bool needsToRefresh;

		void Awake(){
			ServerController.onRefreshScoreboard += Refresh;
		}

		void Start(){
			OnInitializeGamemode();

			PhotonPlayer[] list = PhotonNetwork.playerList;
			if(list.Length > 0){
				for(int i = 0; i < list.Length; i++){
					CreatePlayer(list[i]);
				}
			}
		}

		void Update(){
			if(needsToRefresh){
				needsToRefresh = false;
				Refresh();
			}
		}

		public void Refresh(){
			if(scoreboard == null)
				return;

			if(!scoreboard.activeInHierarchy){
				needsToRefresh = true;
				return;
			}

			needsToRefresh = false;

			float topKDRRed = 0f;
			float topKDRBlue = 0f;
			float topKDRNone = 0f;

			foreach(var pair in players){
				PhotonPlayer player = pair.Key;
				ScoreboardPlayer scoreboardPlayer = pair.Value;

				float kdr = ServerController.GetKDR(player);
				switch(player.GetTeam()){
					case PunTeams.Team.red:
						if(kdr > topKDRRed){
							topKDRRed = kdr;

							if(scoreboardPlayer != null)
								scoreboardPlayer.transform.SetAsFirstSibling();
						}
						break;
					case PunTeams.Team.blue:
						if(kdr > topKDRBlue){
							topKDRBlue = kdr;

							if(scoreboardPlayer != null)
								scoreboardPlayer.transform.SetAsFirstSibling();
						}
						break;
					default:
						if(kdr > topKDRNone){
							topKDRNone = kdr;

							if(scoreboardPlayer != null)
								scoreboardPlayer.transform.SetAsFirstSibling();
						}
						break;
				}
			}
		}

		public void OnInitializeGamemode(){
			ServerController.Gamemode gamemode = ServerController.gamemode;

			if(gamemode == ServerController.Gamemode.TDM){
				redTeamGroup.SetActive(true);
				blueTeamGroup.SetActive(true);
				noneTeamGroup.SetActive(false);
			}else{
				redTeamGroup.SetActive(false);
				blueTeamGroup.SetActive(false);
				noneTeamGroup.SetActive(true);
			}
		}

		void CreatePlayer(PhotonPlayer player){
			GameObject go = Instantiate(scoreboardPlayerPrefab) as GameObject;
			ScoreboardPlayer scoreboardPlayer = go.GetComponent<ScoreboardPlayer>();

			scoreboardPlayer.scoreboard = this;
			scoreboardPlayer.player = player;
			scoreboardPlayer.Refresh();

			players.Add(player, scoreboardPlayer);
		}

		void DestroyPlayer(PhotonPlayer player){
			ScoreboardPlayer scoreboardPlayer = null;
			if(players.TryGetValue(player, out scoreboardPlayer)){
				Destroy(scoreboardPlayer.gameObject);
			}
		}

		public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer){
			CreatePlayer(newPlayer);
		}

		public override void OnPhotonPlayerDisconnected(PhotonPlayer newPlayer){
			DestroyPlayer(newPlayer);
		}
	}
}