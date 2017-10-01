/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using GameJolt.API;
using Photon;

namespace VOX{
	public class OfflineHUD : PunBehaviour{
		
		const string loadingStatus = "Loading...";
		
		public string gameName;
		public string gameVersion;
		public GameObject connectingText;
		public GameObject icon;

		[Header("Servers")]
		public GameObject serversMenu;
		public Transform serversParent;
		public GameObject serverPrefab;
		public TMP_InputField searchField;

		[Header("Create Room")]
		public GameObject createRoomMenu;
		public TMP_InputField roomNameField;
		public Toggle[] gamemodes;

		[Header("Status")]
		public GameObject statusMenu;
		public TextMeshProUGUI statusText;

		string statusTextFormat;
		
		ServerUI[] servers;

		void Start(){
			if(Manager.Instance.CurrentUser != null){
				StartUp();
			}else{
				ShowSignInScreen();
			}
		}

		public void StartUp(){
			icon.SetActive(false);

			statusTextFormat = statusText.text;
			UpdateStatusText(true);

			if(!PhotonNetwork.connected){
				UI_ChangeMenu(-1);
				PhotonNetwork.ConnectUsingSettings(gameName + "_" + gameVersion);
			}else{
				UI_ChangeMenu(0);

				InvokeRepeating("UI_Refresh", 0f, 10f);
				InvokeRepeating("UpdateData", 1f, 10f);
			}
		}
		
		void ShowSignInScreen(){
			icon.SetActive(true);

			GameJolt.UI.Manager.Instance.ShowSignIn((bool success) => {
				if(success){
					StartUp();
				}else{
					ShowSignInScreen();
				}
			});
		}

		void UpdateData(){
			GameController.LoadGameJoltData((bool success) => {
				UpdateStatusText();
			});
		}

		void UpdateStatusText(bool loading = false){
			if(loading){
				statusText.text = string.Format(statusTextFormat, loadingStatus, loadingStatus, loadingStatus, loadingStatus);
				return;
			}

			string username = loadingStatus;

			if(Manager.Instance.CurrentUser != null)
				username = Manager.Instance.CurrentUser.Name;
			
			int kills = GameController.totalKills;
			int deaths = GameController.totalDeaths;
			float kdr = -1f;

			if(deaths > 0)
				kdr = (float) kills / (float) deaths;
			else if(kills >= 0)
				kdr = kills;
			else
				kdr = -1f;

			string killsStr = kills == -1 ? loadingStatus : kills.ToString();
			string deathsStr = deaths == -1 ? loadingStatus : deaths.ToString();
			string kdrStr = kdr == -1 ? loadingStatus : kdr.ToString("0.0");

			statusText.text = string.Format(statusTextFormat, username, killsStr, deathsStr, kdrStr);
		}

		public ServerUI CreateServer(RoomInfo room){
			GameObject go = Instantiate(serverPrefab);
			go.transform.SetParent(serversParent);

			ServerUI server = go.GetComponent<ServerUI>();
			server.Setup(room);
			
			return server;
		}

		

		public void UI_ChangeMenu(int menu){
			connectingText.SetActive(menu == -1);
			statusMenu.SetActive(menu != -1);

			serversMenu.SetActive(menu == 0);
			createRoomMenu.SetActive(menu == 1);
		}

		public void UI_Refresh(){
			PhotonNetwork.playerName = Manager.Instance.CurrentUser.Name;

			if(servers != null){
				for(int i = 0; i < servers.Length; i++){
					Destroy(servers[i].gameObject);
				}
			}

			RoomInfo[] rooms = PhotonNetwork.GetRoomList();
			if(rooms != null && rooms.Length > 0){
				servers = new ServerUI[rooms.Length];

				for(int i = 0; i < servers.Length; i++){
					servers[i] = CreateServer(rooms[i]);
				}
			}
		}

		public void UI_CreateRoom(){
			RoomOptions options = new RoomOptions();
			options.MaxPlayers = 16;

			int gamemode = 0;
			for(int i = 0; i < gamemodes.Length; i++){
				if(gamemodes[i].isOn){
					gamemode = i;
					break;
				}
			}

			options.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable(){{"gamemode", gamemode}, {"map", 0}};
			options.CustomRoomPropertiesForLobby = new string[]{"gamemode", "map"};

			PhotonNetwork.CreateRoom(roomNameField.text, options, null);
		}

		public void UI_CharacterCustomization(){
			SceneManager.LoadScene("CharacterCustomization");
		}



		public override void OnJoinedRoom(){
			CancelInvoke("UpdateData");
			CancelInvoke("UI_Refresh");
			
			//Loading.LoadMap("House", "House", "Just my house", "The subtitle says it all");
			PhotonNetwork.LoadLevel("House");
		}

		public override void OnJoinedLobby(){
			PhotonNetwork.automaticallySyncScene = true;

			UI_ChangeMenu(0);
			UI_Refresh();
			UpdateData();
		}
	}
}