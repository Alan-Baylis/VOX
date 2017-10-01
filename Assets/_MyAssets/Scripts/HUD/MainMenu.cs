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
	public class MainMenu : PunBehaviour{
		
		const string loadingStatus = "Loading...";
		const float refreshRate = 20f;
		
		public GameObject connectingText;

		[Header("Main Screen")]
		public GameObject mainScreen;

		[Header("Servers")]
		public GameObject serversMenu;
		public Transform serversParent;
		public GameObject serverPrefab;
		public TMP_InputField searchField;

		[Header("Create Room")]
		public GameObject createRoomMenu;
		public TMP_InputField roomNameField;
		public Toggle[] gamemodes;
		public Toggle[] maps;

		[Header("Status")]
		public GameObject statusMenu;
		public TextMeshProUGUI statusText;

		string statusTextFormat;
		ServerUI[] servers;

		#region Unity Events
		void Start(){
			if(Manager.Instance.CurrentUser != null){
				StartUp();
				GameController.UpdatePlayerProperties();
			}else{
				ShowSignInScreen();
			}
		}
		#endregion

		#region Things
		void StartUp(){
			statusTextFormat = statusText.text;
			ChangeMenu(0);
		}
		
		void ShowSignInScreen(){
			GameJolt.UI.Manager.Instance.ShowSignIn((bool success) => {
				if(success){
					GameController.LoadGameJoltData();
					StartUp();
				}else{
					ShowSignInScreen();
				}
			});
		}

		void UpdateStatus(){
			UpdateStatusText(true);

			GameController.LoadGameJoltData((bool success) => {
				UpdateStatusText();
			});
		}
		#endregion

		#region Photon Events
		public override void OnJoinedRoom(){
			Room room = PhotonNetwork.room;
			string map = room.CustomProperties["map"] as string;

			CancelInvoke("UpdateStatus");
			CancelInvoke("Refresh");
			
			Loading.LoadMap(map, map, "This is a subtitle", "And this is a description");
		}

		public override void OnJoinedLobby(){
			PhotonNetwork.playerName = Manager.Instance.CurrentUser.Name;
			PhotonNetwork.sendRateOnSerialize = 15;
			PhotonNetwork.automaticallySyncScene = true;

			PhotonNetwork.player.SetTeam(PunTeams.Team.none);

			ChangeMenu(1);

			if(refreshRate > 0f){
				InvokeRepeating("UpdateStatus", 0f, refreshRate);
				InvokeRepeating("Refresh", 0f, refreshRate);
			}
		}
		#endregion

		#region UI
		ServerUI CreateServerUI(RoomInfo room){
			GameObject go = Instantiate(serverPrefab);
			go.transform.SetParent(serversParent);
			go.transform.localScale = Vector3.one;

			ServerUI server = go.GetComponent<ServerUI>();
			server.Setup(room);
			
			return server;
		}

		int currentMenu;

		void ChangeMenu(int menu){
			if(menu == 1 && !PhotonNetwork.connected){
				PhotonNetwork.ConnectUsingSettings(GameValues.Instance.PhotonGameVersion);
				connectingText.SetActive(true);

				currentMenu = -1;
				return;
			}else{
				connectingText.SetActive(false);
			}

			currentMenu = menu;

			if(menu == 0){
				CancelInvoke("UpdateStatus");
				CancelInvoke("Refresh");
			}

			mainScreen.SetActive(menu == 0);
			serversMenu.SetActive(menu == 1);
			createRoomMenu.SetActive(menu == 2);

			statusMenu.SetActive(menu > 0);
		}

		void Refresh(){
			if(servers != null){
				for(int i = 0; i < servers.Length; i++){
					Destroy(servers[i].gameObject);
				}
			}

			RoomInfo[] rooms = PhotonNetwork.GetRoomList();
			if(rooms != null && rooms.Length > 0){
				servers = new ServerUI[rooms.Length];

				for(int i = 0; i < servers.Length; i++){
					servers[i] = CreateServerUI(rooms[i]);
				}
			}
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
		#endregion

		#region UI Events
		public void UI_MainScreen(){
			ChangeMenu(0);
		}

		public void UI_ServersScreen(){
			ChangeMenu(1);
		}

		public void UI_CreateRoomScreen(){
			ChangeMenu(2);
		}

		public void UI_Refresh(){
			Refresh();
		}

		public void UI_CreateRoom(){
			int gamemodeID = 0;
			for(int i = 0; i < gamemodes.Length; i++){
				if(gamemodes[i].isOn){
					gamemodeID = i;
					break;
				}
			}

			int mapID = 0;
			for(int i = 0; i < maps.Length; i++){
				if(maps[i].isOn){
					mapID = i;
					break;
				}
			}
			
			MultiplayerUtils.CreateRoom(roomNameField.text, gamemodeID, mapID, 12, 300d);
		}

		public void UI_CharacterCustomization(){
			SceneManager.LoadScene("CharacterCustomization");
		}

		public void UI_Quit(){
			Application.Quit();
		}
		#endregion
	}
}