/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;
using ExitGames.Client.Photon;
using GameJolt.API;
using System;

namespace VOX{
	public class ServerController : PhotonSingleton<ServerController>{

		protected ServerController(){}

		public enum Gamemode{ FFA = 0, TDM = 1, OITC = 2, GG = 3, CTF = 4 };
		public enum TopPlayerType{ Kills, Deaths, KDR, Points }

		public GunInfo[] gunGameGuns;

		public static Gamemode gamemode;
		public static Player player;
		public static InRoomRoundTimer roundTimer;
		
		public static bool HasRoundEnded;

		public static double ElapsedTime{
			get{
				if(roundTimer != null && !roundTimer.hasRoundEnded)
					return roundTimer.elapsedTime;

				return 0d;
			}
		}

		public static double RemainingTime{
			get{
				if(ServerController.roundTimer != null && !roundTimer.hasRoundEnded)
					return ServerController.roundTimer.remainingTime;

				return 0d;
			}
		}

		void Awake(){
			PhotonNetwork.OnEventCall += OnEventCall;
		}

		#region My Events
		public static Action<PhotonPlayer, GunInfo, PhotonPlayer, bool> onKill;
		public static Action<PhotonPlayer, string> onDieCustomReason;
		public static Action<PhotonPlayer, string> onChatMessage;
		public static Action onRefreshScoreboard;
		public static Action<Gamemode> onInitializeGamemode;
		
		public static void OnKill(PhotonPlayer killer, GunInfo gun, PhotonPlayer dead, bool headshot){
			AddKill(killer);
			AddDeath(dead);

			if(onKill != null){
				onKill(killer, gun, dead, headshot);
			}
		}

		public static void OnDieCustomReason(PhotonPlayer dead, string customReason){
			AddDeath(dead);

			if(onDieCustomReason != null){
				onDieCustomReason(dead, customReason);
			}
		}

		public static void OnChatMessage(PhotonPlayer player, string message){
			if(onChatMessage != null){
				onChatMessage(player, message);
			}
		}

		public static void RefreshScoreboard(){
			if(onRefreshScoreboard != null){
				onRefreshScoreboard();
			}
		}
		#endregion

		#region Teams
		public static bool AreFriends(PhotonPlayer player1, PhotonPlayer player2, bool returnFalseIfSamePlayer = false){
			if(player1 == player2)
				return !returnFalseIfSamePlayer;

			PunTeams.Team player1Team = player1.GetTeam();
			PunTeams.Team player2Team = player2.GetTeam();

			if(player1Team == PunTeams.Team.none || player2Team == PunTeams.Team.none){
				return false;
			}else if(player1Team == player2Team){
				return true;
			}

			return false;
		}
		
		public static T GetObjectFromTeam<T>(PunTeams.Team team, T redTeam, T blueTeam, T noneTeam){
			if(team == PunTeams.Team.red)
				return redTeam;
			else if(team == PunTeams.Team.blue)
				return blueTeam;
			else
				return noneTeam;
		}
		#endregion

		#region Custom Properties
		public static void SaveInfoToGamejolt(){
			PhotonPlayer player = PhotonNetwork.player;
			Hashtable props = player.CustomProperties;
			
			int kills = GetKills(player);
			int deaths = GetDeaths(player);

			DataStore.Get("totalKills", false, (string totalKills) => {
				if(totalKills != null){
					int totalKillsInt = 0;
					if(int.TryParse(totalKills, out totalKillsInt)){
						DataStore.Set("totalKills", (totalKillsInt + kills).ToString(), false);
					}
				}else{
					DataStore.Set("totalKills", kills.ToString(), false);
				}
			});
			DataStore.Get("totalDeaths", false, (string totalDeaths) => {
				if(totalDeaths != null){
					int totalDeathsInt = 0;
					if(int.TryParse(totalDeaths, out totalDeathsInt)){
						DataStore.Set("totalDeaths", (totalDeathsInt + deaths).ToString(), false);
					}
				}else{
					DataStore.Set("totalDeaths", deaths.ToString(), false);
				}
			});
		}

		public static void ResetInfo(PhotonPlayer player, bool resetPoints = false){
			Hashtable props = new Hashtable(){{"kills", 0}, {"deaths", 0}};

			if(resetPoints)
				props["points"] = 0;

			player.SetCustomProperties(props);

			RefreshScoreboard();
		}

		public static void AddKill(PhotonPlayer player){
			Hashtable props = new Hashtable(){{"kills", GetKills(player) + 1}};
			player.SetCustomProperties(props);

			RefreshScoreboard();
		}

		public static void AddDeath(PhotonPlayer player){
			Hashtable props = new Hashtable(){{"deaths", GetDeaths(player) + 1}};
			player.SetCustomProperties(props);

			RefreshScoreboard();
		}

		public static void AddPoint(PhotonPlayer player){
			Hashtable props = new Hashtable(){{"points", GetPoints(player) + 1}};
			player.SetCustomProperties(props);

			RefreshScoreboard();
		}

		public static int GetKills(PhotonPlayer player){
			Hashtable props = player.CustomProperties;

			if(props.ContainsKey("kills")){
				return (int) props["kills"];
			}

			return 0;
		}

		public static int GetDeaths(PhotonPlayer player){
			Hashtable props = player.CustomProperties;

			if(props.ContainsKey("deaths")){
				return (int) props["deaths"];
			}

			return 0;
		}

		public static int GetPoints(PhotonPlayer player){
			Hashtable props = player.CustomProperties;

			if(props.ContainsKey("points")){
				return (int) props["points"];
			}

			return 0;
		}
		
		public static float GetKDR(PhotonPlayer player){
			int kills = GetKills(player);
			int deaths = GetDeaths(player);
			float kdr = 0f;

			if(deaths > 0)
				kdr = (float) kills / (float) deaths;
			else
				kdr = kills;

			return kdr;
		}

		public static PhotonPlayer GetTopPlayer(TopPlayerType type){
			PhotonPlayer[] players = PhotonNetwork.playerList;

			float topValue = 0;
			PhotonPlayer topPlayer = players[UnityEngine.Random.Range(0, players.Length)];
			for(int i = 0; i < players.Length; i++){
				PhotonPlayer player = players[i];

				switch(type){
					case TopPlayerType.Kills:
						int kills = GetKills(player);

						if(kills > topValue){
							topValue = kills;
							topPlayer = player;
						}
						break;
					case TopPlayerType.Deaths:
						int deaths = GetDeaths(player);

						if(deaths > topValue){
							topValue = deaths;
							topPlayer = player;
						}
						break;
					case TopPlayerType.KDR:
						float kdr = GetKills(player);

						if(kdr > topValue){
							topValue = kdr;
							topPlayer = player;
						}
						break;
					case TopPlayerType.Points:
						int points = GetPoints(player);

						if(points > topValue){
							topValue = points;
							topPlayer = player;
						}
						break;
				}
			}

			return topPlayer;
		}
		#endregion

		#region Photon Events
		public override void OnJoinedRoom(){
			ResetInfo(PhotonNetwork.player, true);

			//roundTimer = gameObject.AddComponent<InRoomRoundTimer>();
			//roundTimer.onRoundEnd += OnRoundEnd;
		}

		public override void OnLeftRoom(){
			SaveInfoToGamejolt();

			if(roundTimer != null){
				Destroy(roundTimer);
				roundTimer = null;
			}
		}

		public override void OnPhotonCustomRoomPropertiesChanged(Hashtable hash){
			if(hash.ContainsKey("gamemode")){
				int gamemodeIndex = (int) hash["gamemode"];
				gamemode = (Gamemode) gamemodeIndex;

				InitializeGamemode();
			}
			
			/*if(hash.ContainsKey("redFlagOwner")){
				if(hash["redFlagOwner"] is string && ((string) hash["redFlagOwner"]) == "none"){
					redFlagOwner = null;

					if(onReturnFlag != null){
						onReturnFlag(CTFFlag.Red);
					}
				}else{
					int redFlagOwnerID = (int) hash["redFlagOwner"];
					PhotonView view = PhotonView.Find(redFlagOwnerID);

					redFlagOwner = view;

					if(onPickupFlag != null){
						onPickupFlag(view, CTFFlag.Red);
					}
				}
			}if(hash.ContainsKey("blueFlagOwner")){
				if(hash["blueFlagOwner"] is string && ((string) hash["blueFlagOwner"]) == "none"){
					blueFlagOwner = null;

					if(onReturnFlag != null){
						onReturnFlag(CTFFlag.Blue);
					}
				}else{
					int blueFlagOwnerID = (int) hash["blueFlagOwner"];
					PhotonView view = PhotonView.Find(blueFlagOwnerID);

					blueFlagOwner = view;

					if(onPickupFlag != null){
						onPickupFlag(view, CTFFlag.Blue);
					}
				}
			}*/
		}
		#endregion

		#region Gamemodes
		public void InitializeGamemode(){
			if(onInitializeGamemode != null){
				onInitializeGamemode(gamemode);
			}
		}
		#endregion

		#region Rounds
		public void EndRound(){
			PhotonNetwork.RaiseEvent(0, null, false, RaiseEventOptions.Default);
		}

		public void RestartRound(){
			PhotonNetwork.RaiseEvent(1, null, false, RaiseEventOptions.Default);
		}

		System.Collections.IEnumerator IOnRoundEnd(){
			if(gamemode == Gamemode.GG)
				AddPoint(GetTopPlayer(TopPlayerType.Kills));
			else
				AddPoint(GetTopPlayer(TopPlayerType.KDR));

			yield return new WaitForSeconds(10f);

			RestartRound();
		}

		public void OnEventCall(byte eventCode, object content, int senderID){
			if(eventCode == 0){
				if(PhotonNetwork.isMasterClient && !HasRoundEnded){
					StartCoroutine(IOnRoundEnd());
				}

				HasRoundEnded = true;
			}else if(eventCode == 1){
				if(PhotonNetwork.isMasterClient && HasRoundEnded){
					Player[] players = GameObject.FindObjectsOfType<Player>();
					for(int i = 0; i < players.Length; i++){
						Player player = players[i];
						player.photonView.RPC("RPCReset", player.photonView.owner);

						ResetInfo(player.photonView.owner);
					}
				}

				HasRoundEnded = false;
			}
		}
		#endregion



		#region CTF
		/*
		public enum CTFFlag{ Red, Blue }
		
		public static PhotonView redFlagOwner;
		public static PhotonView blueFlagOwner;
		
		public static Action<PhotonView, CTFFlag> onPickupFlag;
		public static Action<CTFFlag> onReturnFlag;

		public static void PickupFlag(PhotonView view, CTFFlag flag){
			PunTeams.Team playerTeam = view.owner.GetTeam();

			bool isSameTeam = (
				(playerTeam == PunTeams.Team.red && flag == ServerController.CTFFlag.Red) ||
				(playerTeam == PunTeams.Team.blue && flag == ServerController.CTFFlag.Blue)
			);

			if(isSameTeam)
				return;

			Hashtable props = null;

			if(flag == CTFFlag.Red){
				props = new Hashtable{{"redFlagOwner", view.viewID}};
			}else{
				props = new Hashtable{{"blueFlagOwner", view.viewID}};
			}

			PhotonNetwork.room.SetCustomProperties(props);
		}

		public static void ReturnFlag(CTFFlag flag){
			if(flag == CTFFlag.Red){
				if(redFlagOwner == null)
					return;

				Hashtable props = new Hashtable{{"redFlagOwner", "none"}};
				PhotonNetwork.room.SetCustomProperties(props);
			}else{
				if(blueFlagOwner == null)
					return;

				Hashtable props = new Hashtable{{"blueFlagOwner", "none"}};
				PhotonNetwork.room.SetCustomProperties(props);
			}
		}

		public static bool IsHoldingAnyFlag(PhotonView view){
			return view == redFlagOwner || view == blueFlagOwner;
		}*/
		#endregion
	}
}